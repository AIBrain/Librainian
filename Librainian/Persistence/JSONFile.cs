// Copyright © Protiguous. All Rights Reserved.
// This entire copyright notice and license must be retained and must be kept visible in any binaries, libraries, repositories, or source code (directly or derived) from our binaries, libraries, projects, solutions, or applications.
// All source code belongs to Protiguous@Protiguous.com unless otherwise specified or the original license has been overwritten by formatting. (We try to avoid it from happening, but it does accidentally happen.)
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
// Our software can be found at "https://Protiguous.Software/"
// Our GitHub address is "https://github.com/Protiguous".
//
// File "JSONFile.cs" last formatted on 2020-08-14 at 8:44 PM.

#nullable enable

namespace Librainian.Persistence;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Exceptions;
using FileSystem;
using Logging;
using Maths;
using Measurement.Time;
using Newtonsoft.Json;
using Utilities;
using DataType = System.Collections.Concurrent.ConcurrentDictionary<System.String, System.String?>;
using ReadOnlyDataType = System.Collections.Generic.IReadOnlyDictionary<System.String, System.String?>;
using SectionType = System.Collections.Concurrent.ConcurrentDictionary<System.String, System.Collections.Concurrent.ConcurrentDictionary<System.String, System.String?>>;

/// <summary>Persist a document to and from a JSON formatted text document.</summary>
/// TODO Needs testing. A lot of testing.
[JsonObject]
public class JSONFile {

	[JsonProperty]
	private SectionType Data {
		[DebuggerStepThrough]
		get;
	} = new();

	public IEnumerable<String?> AllKeys => this.Sections.SelectMany( section => this.Data[section].Keys );

		
	[JsonProperty]
	public Document? Document { get; set; }

	public IEnumerable<String> Sections => this.Data.Keys;

	public ReadOnlyDataType? this[String? section] {
		[DebuggerStepThrough]
		[NeedsTesting]
		get {
			if ( String.IsNullOrEmpty( section ) ) {
				return default( ReadOnlyDataType );
			}

			if ( this.Data.ContainsKey( section ) ) {
				if ( this.Data.TryGetValue( section, out var result ) ) {
					return result;
				}
			}

			return default( ReadOnlyDataType );
		}
	}

		
	/// <summary>
	/// 
	/// </summary>
	/// <param name="section"></param>
	/// <param name="key"></param>
	public String? this[String? section, String? key] {
		[DebuggerStepThrough]
		[NeedsTesting]
		get {
			if ( String.IsNullOrEmpty( section ) ) {
				return default( String? );
			}

			if ( String.IsNullOrEmpty( key ) ) {
				return default( String? );
			}

			if ( !this.Data.ContainsKey( section ) ) {
				return default( String? );
			}

			return this.Data[section].TryGetValue( key, out var value ) ? value : null;
		}

		[DebuggerStepThrough]
		set {
			if ( String.IsNullOrEmpty( section ) ) {
				return;
			}

			if ( String.IsNullOrEmpty( key ) ) {
				return;
			}

			this.Add( section, (key, value) );
		}
	}

		
	/// <summary>
	/// 
	/// </summary>
	/// <param name="document"></param>
	/// <param name="cancellationToken"></param>
	public JSONFile( Document? document, CancellationToken cancellationToken ) : this() {
		this.Document = document;

		this.Document?.ContainingingFolder().Create( cancellationToken );

		this.Read( cancellationToken ).Wait( cancellationToken );
	}

	public JSONFile() { }

	/// <summary>(Trims whitespaces from section and key)</summary>
	/// <param name="section"></param>
	/// <param name="pair"></param>
	public Boolean Add( String section, KeyValuePair<String, String?> pair ) {
		if ( String.IsNullOrWhiteSpace( section ) ) {
			throw new ArgumentException( "Argument is null or whitespace", nameof( section ) );
		}

		section = section.Trim();

		if ( String.IsNullOrWhiteSpace( section ) ) {
			throw new ArgumentException( "Argument is null or whitespace", nameof( section ) );
		}

		var retries = 10;
		TryAgain:

		if ( !this.Data.ContainsKey( section ) ) {
			this.Data.TryAdd( section, new DataType() );
		}

		try {
			(var key, var value) = pair;
			this.Data[section][key.Trim()] = value;

			return true;
		}
		catch ( KeyNotFoundException exception ) {
			retries--;

			if ( retries.Any() ) {
				goto TryAgain;
			}

			exception.Log();
		}

		return false;
	}

	/// <summary>(Trims whitespaces from section and key)</summary>
	/// <param name="section"></param>
	/// <param name="pair"></param>
	public Boolean Add( String section, (String k, String? v) tuple ) {
		if ( String.IsNullOrWhiteSpace( section ) ) {
			throw new ArgumentException( "Argument is null or whitespace", nameof( section ) );
		}

		section = section.Trim();

		if ( String.IsNullOrWhiteSpace( section ) ) {
			throw new ArgumentException( "Argument is null or whitespace", nameof( section ) );
		}

		var retries = 10;
		TryAgain:

		if ( !this.Data.ContainsKey( section ) ) {
			this.Data.TryAdd( section, new DataType() );
		}

		try {
			(var key, var value) = tuple;
			this.Data[section][key.Trim()] = value;

			return true;
		}
		catch ( KeyNotFoundException exception ) {
			retries--;

			if ( retries.Any() ) {
				goto TryAgain;
			}

			exception.Log();
		}

		return false;
	}

	/// <summary>Removes all data from all sections.</summary>
	public Boolean Clear( CancellationToken cancellationToken ) {
		Parallel.ForEach( this.Data.Keys.TakeWhile( _ => !cancellationToken.IsCancellationRequested ), section => this.TryRemove( section ) );

		return !this.Data.Keys.Any();
	}

	public async Task<Boolean> Read( CancellationToken cancellationToken ) {
		var document = this.Document;

		if ( document is null ) {
			return false;
		}

		var exists = await document.Exists( cancellationToken ).ConfigureAwait( false );

		if ( exists != true ) {
			return false;
		}

		try {
			(var status, var data) = await document.LoadJSON<SectionType>( null, cancellationToken ).ConfigureAwait( false );

			if ( !status.IsGood() ) {
				return false;
			}

			if ( data != null ) {
				var result = Parallel.ForEach( data.Keys.AsParallel(), section => Parallel.ForEach( data[section].Keys.AsParallel().AsUnordered(), key => {
					if ( !String.IsNullOrEmpty( key ) ) {
						this.Add( section, new KeyValuePair<String, String?>( key, data[section][key] ) );
					}
				} ) );

				return result.IsCompleted;
			}
		}
		catch ( JsonException exception ) {
			exception.Log();
		}
		catch ( IOException exception ) {

			//file in use by another app
			exception.Log();
		}
		catch ( OutOfMemoryException exception ) {

			//file is huge
			exception.Log();
		}

		return false;
	}

	/// <summary>Returns a string that represents the current object.</summary>
	/// <returns>A string that represents the current object.</returns>
	public override String ToString() => $"{this.Sections.Count()} sections, {this.AllKeys.Count()} keys";

	[DebuggerStepThrough]
	public Boolean TryRemove( String section ) {
		if ( section == null ) {
			throw new ArgumentEmptyException( nameof( section ) );
		}

		return this.Data.TryRemove( section, out var _ );
	}

	[DebuggerStepThrough]
	public Boolean TryRemove( String section, String? key ) {
		if ( section == null ) {
			throw new ArgumentEmptyException( nameof( section ) );
		}

		if ( key is null ) {
			return false;
		}
		if ( !this.Data.ContainsKey( section ) ) {
			return false;
		}

		return this.Data[section].TryRemove( key, out var _ );
	}

	/// <summary>Saves the <see cref="Data" /> to the <see cref="Document" />.</summary>
	public async Task<Boolean> Write( CancellationToken cancellationToken ) {
		var document = this.Document;

		if ( document is null ) {
			return false;
		}

		await document.TryDeleting( Seconds.One, cancellationToken ).ConfigureAwait( false );

		var json = this.Data.ToJSON( Formatting.Indented );
		if ( json is null ) {
			return false;
		}

		await document.AppendText( json, cancellationToken ).ConfigureAwait( false );
		return true;
	}
}