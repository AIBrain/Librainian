// Copyright © Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible in any binaries, libraries, repositories, or source code (directly or derived) from our binaries, libraries, projects, solutions, or applications.
//
// All source code belongs to Protiguous@Protiguous.com unless otherwise specified or the original license has been overwritten by formatting. (We try to avoid it from happening, but it does accidentally happen.)
//
// Any unmodified portions of source code gleaned from other sources still retain their original license and our thanks goes to those Authors.
// If you find your code unattributed in this source code, please let us know so we can properly attribute you and include the proper license and/or copyright(s).
//
// If you want to use any of our code in a commercial project, you must contact Protiguous@Protiguous.com for permission, license, and a quote.
//
// Donations, payments, and royalties are accepted via bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2 and PayPal: Protiguous@Protiguous.com
//
// ====================================================================
// Disclaimer:  Usage of the source code or binaries is AS-IS.
//     No warranties are expressed, implied, or given.
//     We are NOT responsible for Anything You Do With Our Code.
//     We are NOT responsible for Anything You Do With Our Executables.
//     We are NOT responsible for Anything You Do With Your Computer.
// ====================================================================
//
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
// For business inquiries, please contact me at Protiguous@Protiguous.com.
//
// Our software can be found at "https://Protiguous.com/Software"
// Our GitHub address is "https://github.com/Protiguous".

#nullable enable

namespace Librainian.Persistence {

	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Diagnostics;
	using System.IO;
	using System.Linq;
	using System.Threading;
	using System.Threading.Tasks;
	using Exceptions;
	using FileSystem;
	using JetBrains.Annotations;
	using Logging;
	using Maths;
	using Measurement.Time;
	using Microsoft.Database.Isam.Config;
	using Microsoft.Isam.Esent.Collections.Generic;
	using Microsoft.Isam.Esent.Interop.Windows81;
	using Newtonsoft.Json;
	using OperatingSystem.Compression;
	using Parsing;
	using Utilities;

	/// <summary>
	///     <para>
	///         Allows the <see cref="PersistentDictionary{TKey,TValue}" /> class to persist almost any object by using
	///         Newtonsoft.Json.
	///     </para>
	/// </summary>
	/// <see cref="http://managedesent.codeplex.com/wikipage?title=PersistentDictionaryDocumentation" />
	/// <remarks>//TODO This class is in severe need of unit testing.</remarks>
	[DebuggerDisplay( "{" + nameof( ToString ) + "(),nq}" )]
	[JsonObject]
	public class PersistTable<TKey, TValue> : ABetterClassDispose, IDictionary<TKey, TValue?> where TKey : IComparable<TKey> {

		[JsonProperty]
		private PersistentDictionary<TKey, String?> Dictionary { get; }

		/// <summary>No path given?</summary>
		public Folder Folder { get; }

		public Int32 Count => this.Dictionary.Count;

		public Boolean IsReadOnly => this.Dictionary.IsReadOnly;

		public ICollection<TKey> Keys => this.Dictionary.Keys ?? throw new InvalidOperationException();

		/// <summary>This deserializes the list of values.. I have a feeling this cannot be very fast.</summary>
		public ICollection<TValue?> Values =>
			( ICollection<TValue?> )( this.Dictionary.Values ?? throw new InvalidOperationException() ).Select( value => {
				if ( value != null ) {
					return value.FromCompressedBase64().FromJSON<TValue>();
				}

				return default( TValue? );  //BUG is this intended?
			} );

		/// <summary></summary>
		/// <param name="key"></param>
		/// <returns></returns>
		public TValue? this[ TKey? key ] {
			[CanBeNull]
			get {
				if ( key is null ) {
					return default( TValue? );  //intended. null key returns default
				}

				if ( this.Dictionary.TryGetValue( key, out var storedValue ) && storedValue != null ) {
					return storedValue.FromCompressedBase64().FromJSON<TValue>();
				}

				return default( TValue? );
			}

			set {
				if ( key is null ) {
					return; //doesn't actually get stored
				}

				if ( value is null ) {
					this.Dictionary.Remove( key );

					return;
				}

				this.Dictionary[ key ] = value.ToJSON()?.ToCompressedBase64();
			}
		}

		public PersistTable( Environment.SpecialFolder specialFolder, String tableName ) : this( new Folder( specialFolder, null, tableName ) ) { }

		public PersistTable( Environment.SpecialFolder specialFolder, String? subFolder, String tableName ) : this( new Folder( specialFolder, subFolder,
			tableName ) ) { }

		public PersistTable( Folder folder, String tableName ) : this( Path.Combine( folder.FullPath, tableName ) ) { }

		public PersistTable( Folder folder, String subFolder, String tableName ) :
			this( Path.Combine( folder.FullPath, subFolder, tableName ) ) { }

		public PersistTable( Folder folder, Boolean testForReadWriteAccess = false ) {
			try {
				this.Folder = folder ?? throw new ArgumentEmptyException( nameof( folder ) );

				this.Folder.Info.Create();
				this.Folder.Info.Refresh();

				if ( !this.Folder.Info.Exists ) {
					throw new DirectoryNotFoundException( $"Unable to find or create the folder `{this.Folder.FullPath}`." );
				}

				var customConfig = new DatabaseConfig {
					CreatePathIfNotExist = true,
					DefragmentSequentialBTrees = true,
					EnableShrinkDatabase = ShrinkDatabaseGrbit.On
				};

				this.Dictionary = new PersistentDictionary<TKey, String?>( this.Folder.FullPath, customConfig );

				if ( testForReadWriteAccess && !this.TestForReadWriteAccess( CancellationToken.None ).Result ) {
					throw new IOException( $"Read/write permissions denied in folder {this.Folder.FullPath}." );
				}
			}
			catch ( Exception exception ) {
				exception.Log();

				throw;
			}
		}

		public PersistTable( String fullpath ) : this( new Folder( fullpath ) ) { }

		/// <summary>Return true if we can read/write in the <see cref="Folder" /> .</summary>
		/// <returns></returns>
		private async Task<Boolean> TestForReadWriteAccess( CancellationToken cancellationToken ) {
			try {
				using var document = this.Folder.TryGetTempDocument();

				var text = Randem.NextString( 64, true, true, true, true );
				await document.AppendText( text, cancellationToken ).ConfigureAwait( false );

				await document.TryDeleting( Seconds.Ten, cancellationToken ).ConfigureAwait( false );

				return true;
			}
			catch { }

			return false;
		}

		public void Add( TKey key, TValue? value ) => this[ key ] = value;

		public void Add( KeyValuePair<TKey, TValue?> item ) => this[ item.Key ] = item.Value;

		public void Clear() => this.Dictionary.Clear();

		public Boolean Contains( KeyValuePair<TKey, TValue?> item ) {
			(var key, var value) = item;
			var compressedBase64 = value.ToJSON()?.ToCompressedBase64();
			var asItem = new KeyValuePair<TKey, String?>( key, compressedBase64 );

			return this.Dictionary.Contains( asItem );
		}

		public Boolean Contains( (TKey, TValue) item ) {
			(var key, var value) = item;
			var compressedBase64 = value.ToJSON()?.ToCompressedBase64();
			var asItem = new KeyValuePair<TKey, String?>( key, compressedBase64 );

			return this.Dictionary.Contains( asItem );
		}

		public Boolean ContainsKey( TKey key ) => this.Dictionary.ContainsKey( key );

		public void CopyTo( KeyValuePair<TKey, TValue?>[] array, Int32 arrayIndex ) => throw new NotImplementedException(); //this.Dictionary.CopyTo( array, arrayIndex ); ??

		/// <summary>Dispose any disposable managed fields or properties.</summary>
		public override void DisposeManaged() {
			Trace.Write( $"Disposing of {nameof( this.Dictionary )}..." );

			using ( this.Dictionary ) { }

			Trace.WriteLine( "done." );
		}

		public void Flush() => this.Dictionary.Flush();

		public IEnumerator<KeyValuePair<TKey, TValue?>> GetEnumerator() => this.Items().GetEnumerator();

		public async Task Initialize( CancellationToken cancellationToken ) {
			if ( !await this.Folder.Create( cancellationToken ).ConfigureAwait( false ) ) {
				throw new DirectoryNotFoundException( $"Unable to find or create the folder {this.Folder.FullPath.SmartQuote()}." );
			}
		}

		/// <summary>All <see cref="KeyValuePair{TKey,TValue}" /> , with the <see cref="TValue" /> deserialized.</summary>
		/// <returns></returns>
		public IEnumerable<KeyValuePair<TKey, TValue?>> Items() {
			foreach ( var pair in this.Dictionary ) {
				if ( pair.Value != null ) {
					var keyValuePair = new KeyValuePair<TKey, TValue?>( pair.Key, pair.Value.FromCompressedBase64().FromJSON<TValue?>() );

					yield return keyValuePair;
				}
			}
		}

		public Boolean Remove( TKey key ) => this.Dictionary.ContainsKey( key ) && this.Dictionary.Remove( key );

		public Boolean Remove( KeyValuePair<TKey, TValue?> item ) {
			var value = item.Value.ToJSON()?.ToCompressedBase64();
			var asItem = new KeyValuePair<TKey, String?>( item.Key, value );

			return this.Dictionary.Remove( asItem );
		}

		/// <summary>Returns a string that represents the current object.</summary>
		/// <returns>A string that represents the current object.</returns>
		public override String ToString() => $"{this.Count} items";

		public void TryAdd( TKey key, TValue? value ) {
			if ( key is null ) {
				throw new ArgumentEmptyException( nameof( key ) );
			}

			if ( !this.Dictionary.ContainsKey( key ) ) {
				this[ key ] = value;
			}
		}

		/// <summary>Gets the value associated with the specified key.</summary>
		/// <returns>
		///     true if the object that implements <see cref="IDictionary" /> contains an element with the specified key;
		///     otherwise, false.
		/// </returns>
		/// <param name="key">  The key whose value to get.</param>
		/// <param name="value">
		///     When this method returns, the value associated with the specified key, if the key is found; otherwise, the default
		///     value for the type of the
		///     <paramref name="value" /> parameter. This parameter is passed uninitialized.
		/// </param>
		/// <exception cref="ArgumentEmptyException"><paramref name="key" /> is null.</exception>
		public Boolean TryGetValue( TKey key, out TValue? value ) {
			if ( key is null ) {
				throw new ArgumentEmptyException( nameof( key ) );
			}

			value = default( TValue );

			if ( this.Dictionary.TryGetValue( key, out var storedValue ) && storedValue != null ) {
				value = storedValue.FromCompressedBase64().FromJSON<TValue>();

				return true;
			}

			return false;
		}

		public Boolean TryRemove( TKey key ) {
			if ( key is null ) {
				throw new ArgumentEmptyException( nameof( key ) );
			}

			return this.Dictionary.ContainsKey( key ) && this.Dictionary.Remove( key );
		}

		/// <summary>Returns an enumerator that iterates through a collection.</summary>
		/// <returns>An <see cref="IEnumerator" /> object that can be used to iterate through the collection.</returns>
		IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
	}
}