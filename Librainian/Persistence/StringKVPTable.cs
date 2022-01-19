// Copyright © Protiguous. All Rights Reserved.
// 
// This entire copyright notice and license must be retained and must be kept visible in any binaries, libraries, repositories, or source code (directly or derived) from our binaries, libraries, projects, solutions, or applications.
// 
// All source code belongs to Protiguous@Protiguous.com unless otherwise specified or the original license has been overwritten by formatting. (We try to avoid it from happening, but it does accidentally happen.)
// 
// Any unmodified portions of source code gleaned from other sources still retain their original license and our thanks goes to those Authors.
// If you find your code unattributed in this source code, please let us know so we can properly attribute you and include the proper license and/or copyright(s).
// If you want to use any of our code in a commercial project, you must contact Protiguous@Protiguous.com for permission, license, and a quote.
// 
// Donations, payments, and royalties are accepted via bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2 and PayPal: Protiguous@Protiguous.com
// 
// ====================================================================
// Disclaimer:  Usage of the source code or binaries is AS-IS.
// No warranties are expressed, implied, or given.
// We are NOT responsible for Anything You Do With Our Code.
// We are NOT responsible for Anything You Do With Our Executables.
// We are NOT responsible for Anything You Do With Your Computer.
// ====================================================================
// 
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
// For business inquiries, please contact me at Protiguous@Protiguous.com.
// Our software can be found at "https://Protiguous.com/Software/"
// Our GitHub address is "https://github.com/Protiguous".
// 
// File "StringKVPTable.cs" last formatted on 2022-12-22 at 5:20 PM by Protiguous.

namespace Librainian.Persistence;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using Exceptions;
using FileSystem;
using Logging;
using Maths;
using Measurement.Time;
using Microsoft.Database.Isam.Config;
using Microsoft.Isam.Esent.Collections.Generic;
using Newtonsoft.Json;
using OperatingSystem.Compression;
using Parsing;
using PooledAwait;
using Utilities;
using Utilities.Disposables;

/// <summary>
///     <para>
///         Allows the <see cref="PersistentDictionary{TKey,TValue}" /> class to persist a
///         <see cref="KeyValuePair{TKey,TValue}" /> of base64 compressed strings.
///     </para>
/// </summary>
/// <see cref="http://managedesent.codeplex.com/wikipage?title=PersistentDictionaryDocumentation" />
[DebuggerDisplay( "{" + nameof( ToString ) + "(),nq}" )]
[JsonObject]
public sealed class StringKVPTable : ABetterClassDispose, IDictionary<String, String?> {

	private StringKVPTable() => throw new NotImplementedException();

	public StringKVPTable( Environment.SpecialFolder specialFolder, String tableName ) : this( new Folder( specialFolder, null, tableName ) ) { }

	public StringKVPTable( Environment.SpecialFolder specialFolder, String? subFolder, String tableName ) : this( new Folder( specialFolder, subFolder, tableName ) ) { }

	public StringKVPTable( Byte specialFolder, String? subFolder, String tableName ) : this( new Folder( ( Environment.SpecialFolder ) specialFolder,
		subFolder, tableName ) ) { }

	public StringKVPTable( Folder folder, String tableName ) : this( Path.Combine( folder.FullPath, tableName ) ) { }

	public StringKVPTable( Folder folder, String subFolder, String tableName ) : this( Path.Combine( folder.FullPath, subFolder, tableName ) ) { }

	public StringKVPTable( Folder folder ) {
		if ( folder is null ) {
			throw new ArgumentEmptyException( nameof( folder ) );
		}

		try {
			this.Folder = folder;

			this.Folder.Info.Create();
			this.Folder.Info.Refresh();

			if ( !this.Folder.Info.Exists ) {
				throw new DirectoryNotFoundException( $"Unable to find or create the folder {this.Folder.FullPath.SmartQuote()}." );
			}

			var customConfig = new DatabaseConfig {
				CreatePathIfNotExist = true,
				DefragmentSequentialBTrees = true
			};

			this.Dictionary = new PersistentDictionary<String, String?>( this.Folder.FullPath, customConfig );
		}
		catch ( Exception exception ) {
			exception.Log();
			throw;
		}
	}

	public StringKVPTable( String fullpath ) : this( new Folder( fullpath ) ) { }

	[JsonProperty]
	private PersistentDictionary<String, String?> Dictionary { get; }

	/// <summary>No path given?</summary>
	public Folder Folder { get; }

	public String? this[ params String[] keys ] {
		[NeedsTesting]
		get {
			if ( keys is null ) {
				throw new ArgumentEmptyException( nameof( keys ) );
			}

			var key = CacheKeyBuilder.BuildKey( keys );

			if ( this.Dictionary.TryGetValue( key, out var storedValue ) ) {
				return storedValue?.FromCompressedBase64();
			}

			return default( String? );
		}

		set {
			if ( keys is null ) {
				throw new ArgumentEmptyException( nameof( keys ) );
			}

			var key = CacheKeyBuilder.BuildKey( keys );

			if ( String.IsNullOrEmpty( value ) ) {
				this.Dictionary.Remove( key );

				return;
			}

			this.Dictionary[ key ] = value.ToCompressedBase64();
		}
	}

	public Int32 Count => this.Dictionary.Count;

	public Boolean IsReadOnly => this.Dictionary.IsReadOnly;

	public ICollection<String> Keys {
		get {
			var keys = this.Dictionary.Keys;
			return keys switch {
				null => ( ICollection<String> ) Enumerable.Empty<String>(),
				var _ => keys
			};
		}
	}

	public ICollection<String?> Values {
		get {
			var values = this.Dictionary.Values;
			return values switch {
				null => ( ICollection<String?> ) Enumerable.Empty<String>(),
				var _ => ( ICollection<String?> ) values.Select( value => value?.FromCompressedBase64() )
			};
		}
	}

	/// <summary></summary>
	/// <param name="key"></param>
	public String? this[ String key ] {
		[NeedsTesting]
		get {
			if ( key is null ) {
				throw new ArgumentEmptyException( nameof( key ) );
			}

			if ( this.Dictionary.TryGetValue( key, out var storedValue ) ) {
				return storedValue?.FromCompressedBase64();
			}

			return default( String? );
		}

		set {
			if ( key is null ) {
				throw new ArgumentEmptyException( nameof( key ) );
			}

			if ( String.IsNullOrEmpty( value ) ) {
				this.Dictionary.Remove( key );

				return;
			}

			this.Dictionary[ key ] = value.ToCompressedBase64();
		}
	}

	public void Add( String key, String? value ) {
		if ( value is not null ) {
			this[ key ] = value;
		}
	}

	public void Add( KeyValuePair<String, String?> item ) {
		( var key, var value ) = item;
		this[ key ] = value;
	}

	public void Clear() => this.Dictionary.Clear();

	public Boolean Contains( KeyValuePair<String, String?> item ) {
		( var key, var s ) = item;
		var value = s?.ToJSON()?.ToCompressedBase64();

		var asItem = new KeyValuePair<String, String?>( key, value );

		return this.Dictionary.Contains( asItem );
	}

	public Boolean ContainsKey( String key ) => this.Dictionary.ContainsKey( key );

	public void CopyTo( KeyValuePair<String, String?>[] array, Int32 arrayIndex ) => throw new NotImplementedException(); //this.Dictionary.CopyTo( array, arrayIndex ); ??

	public IEnumerator<KeyValuePair<String, String?>> GetEnumerator() => this.Items().GetEnumerator();

	/// <summary>Returns an enumerator that iterates through a collection.</summary>
	/// <returns>An <see cref="IEnumerator" /> object that can be used to iterate through the collection.</returns>
	IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

	/// <summary>Removes the element with the specified key from the <see cref="IDictionary" /> .</summary>
	/// <returns>
	///     true if the element is successfully removed; otherwise, false. This method also returns false if
	///     <paramref name="key" /> was not found in the
	///     original <see cref="IDictionary" /> .
	/// </returns>
	/// <param name="key">The key of the element to remove.</param>
	/// <exception cref="ArgumentEmptyException"><paramref name="key" /> is null.</exception>
	/// <exception cref="NotSupportedException">The <see cref="IDictionary" /> is read-only.</exception>
	public Boolean Remove( String key ) => this.Dictionary.ContainsKey( key ) && this.Dictionary.Remove( key );

	/// <summary>Removes the first occurrence of a specific object from the <see cref="ICollection" /> .</summary>
	/// <returns>
	///     true if <paramref name="item" /> was successfully removed from the <see cref="ICollection" /> ; otherwise, false.
	///     This method also returns false if
	///     <paramref name="item" /> is not found in the original <see cref="ICollection" /> .
	/// </returns>
	/// <param name="item">The object to remove from the <see cref="ICollection" /> .</param>
	/// <exception cref="NotSupportedException">The <see cref="ICollection" /> is read-only.</exception>
	public Boolean Remove( KeyValuePair<String, String?> item ) {
		( var key, var s ) = item;
		var value = s.ToJSON()?.ToCompressedBase64();
		var asItem = new KeyValuePair<String, String?>( key, value );

		return this.Dictionary.Remove( asItem );
	}

	/// <summary>Gets the value associated with the specified key.</summary>
	/// <returns>
	///     true if the object that implements <see cref="IDictionary" /> contains an element with the specified key;
	///     otherwise, false.
	/// </returns>
	/// <param name="key">The key whose value to get.</param>
	/// <param name="value">
	///     When this method returns, the value associated with the specified key, if the key is found; otherwise, the default
	///     value for the type of the
	///     <paramref name="value" /> parameter. This parameter is passed uninitialized.
	/// </param>
	/// <exception cref="ArgumentEmptyException"><paramref name="key" /> is null.</exception>
	public Boolean TryGetValue( String key, out String value ) {
		if ( key is null ) {
			throw new ArgumentEmptyException( nameof( key ) );
		}

		if ( this.Dictionary.TryGetValue( key, out var storedValue ) ) {
			if ( storedValue != null ) {
				value = storedValue.FromCompressedBase64();
				return true;
			}
		}

		value = String.Empty;
		return false;
	}

	/// <summary>Return true if we can read/write in the <see cref="Folder" /> .</summary>
	private async PooledValueTask<Boolean> TestForReadWriteAccess( CancellationToken cancellationToken ) {
		try {
			var document = this.Folder.TryGetTempDocument();

			var text = Randem.NextString( 64, true, true, true, true );
			await document.AppendText( text, cancellationToken ).ConfigureAwait( false );

			await document.TryDeleting( Seconds.One, cancellationToken ).ConfigureAwait( false );

			return !await document.Exists( cancellationToken ).ConfigureAwait( false );
		}
		catch { }

		return false;
	}

	public void Add( (String key, String value) kvp ) => this[ kvp.key ] = kvp.value;

	/// <summary>Dispose any disposable managed fields or properties.</summary>
	public override void DisposeManaged() {
		Trace.Write( $"Disposing of {nameof( this.Dictionary )}..." );

		using ( this.Dictionary ) { }

		Trace.WriteLine( "done." );
	}

	/// <summary>Force all changes to be written to disk.</summary>
	public void Flush() => this.Dictionary.Flush();

	public async PooledValueTask<Status> Initialize( CancellationToken cancellationToken ) {
		if ( String.IsNullOrWhiteSpace( this.Dictionary.Database?.ToString() ) ) {
			new DirectoryNotFoundException( $"Unable to find or create the folder `{this.Folder.FullPath}`." ).Log();
			return Status.Exception;
		}

		if ( await this.TestForReadWriteAccess( cancellationToken ).ConfigureAwait( false ) ) {
			new IOException( $"Read/write permissions denied in folder {this.Folder.FullPath}." ).Log();
			return Status.Exception;
		}

		return Status.Good;
	}

	/// <summary>All <see cref="KeyValuePair{TKey,TValue }" /> , with the <see cref="String" /> deserialized.</summary>
	public IEnumerable<KeyValuePair<String, String?>> Items() =>
		this.Dictionary.Select( pair => new KeyValuePair<String, String?>( pair.Key, pair.Value?.FromCompressedBase64() ) );

	public void Save() => this.Flush();

	/// <summary>Returns a string that represents the current object.</summary>
	/// <returns>A string that represents the current object.</returns>
	public override String ToString() => $"{this.Count} items";

	//should be all that's needed..
	public void TryAdd( String key, String? value ) {
		if ( key is null ) {
			throw new ArgumentEmptyException( nameof( key ) );
		}

		if ( !this.Dictionary.ContainsKey( key ) ) {
			this[ key ] = value;
		}
	}

	public Boolean TryRemove( String key ) {
		if ( key is null ) {
			throw new ArgumentEmptyException( nameof( key ) );
		}

		return this.Dictionary.ContainsKey( key ) && this.Dictionary.Remove( key );
	}

}