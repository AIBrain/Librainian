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
// Our software can be found at "https://Protiguous.Software/"
// Our GitHub address is "https://github.com/Protiguous".
// 
// File "IniFile.cs" last touched on 2021-12-12 at 3:01 AM by Protiguous.

#nullable enable

namespace Librainian.Persistence.InIFiles;

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Exceptions;
using FileSystem;
using Logging;
using Maths;
using Newtonsoft.Json;
using Parsing;
using PooledAwait;
using Utilities;

/// <summary>
///     A human readable/editable text <see cref="Document" /> with <see cref="KeyValuePair{TKey,TValue}" /> under common
///     <see
///         cref="Sections" />
///     .
/// </summary>
/// <remarks>TODO Needs extensive unit testing.</remarks>
[JsonObject]
public class IniFile {

	public enum LineType {

		Unknown,

		Comment,

		Section,

		KVP

	}

	public const String SectionBegin = "[";

	public const String SectionEnd = "]";

	[DebuggerStepThrough]
	public IniFile( IDocument document ) {
		if ( document is null ) {
			throw new NullException( nameof( document ) );
		}

		var _ = this.Add( document, CancellationToken.None );
	}

	public IniFile( String data, CancellationToken cancellationToken ) {
		if ( String.IsNullOrWhiteSpace( data ) ) {
			throw new NullException( nameof( data ) );
		}

		//cheat: write out to temp file, read in, then delete temp file
		var document = Document.GetTempDocument();

		try {
			var _ = document.AppendText( data, cancellationToken );
			var __ = this.Add( document, cancellationToken );
		}
		finally {
			document.Delete( cancellationToken );
		}
	}

	public IniFile() { }

	[JsonProperty]
	private ConcurrentDictionary<String, IniSection?> Data {
		[DebuggerStepThrough]
		get;
	} = new();

	public IEnumerable<String> Sections => this.Data.Keys;

	public IniSection? this[ String? section ] {
		[DebuggerStepThrough]
		[NeedsTesting]
		get {
			if ( String.IsNullOrEmpty( section ) ) {
				return default( IniSection? );
			}

			if ( !this.Data.ContainsKey( section ) ) {
				return default( IniSection? );
			}

			if ( this.Data.TryGetValue( section, out var result ) ) {
				return result;
			}

			return default( IniSection? );
		}

		set {
			if ( String.IsNullOrEmpty( section ) ) {
				return;
			}

			if ( this.Data.ContainsKey( section ) ) {
				//TODO merge, not overwrite
				this.Data[ section ] = value;

				return;
			}

			this.Data[ section ] = value;
		}
	}

	public String? this[ String? section, String? key ] {
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

			return this.Data[ section ]?.FirstOrDefault( line => line.Key.Like( key ) )?.Value ?? default( String? );
		}

		[NeedsTesting]
		[DebuggerStepThrough]
		set {
			if ( String.IsNullOrEmpty( section ) ) {
				return;
			}

			if ( String.IsNullOrEmpty( key ) ) {
				return;
			}

			this.Add( section, key, value );
		}
	}

	[DebuggerStepThrough]
	private static String Encode( IniLine line ) => $"{line ?? throw new NullException( nameof( line ) )}";

	[DebuggerStepThrough]
	private static String Encode( String section ) => $"{SectionBegin}{section.TrimStart()}{SectionEnd}";

	private IniSection EnsureDataSection( String section ) {
		if ( section is null ) {
			throw new NullException( nameof( section ) );
		}

		lock ( this.Data ) {
			if ( !this.Data.ContainsKey( section ) ) {
				this.Data[ section ] = new IniSection();
			}

			return this.Data[ section ];
		}
	}

	private Int32 FindKVLine( String line, String section, Int32 counter ) {
		if ( line is null ) {
			throw new NullException( nameof( line ) );
		}

		if ( String.IsNullOrWhiteSpace( section ) ) {
			throw new NullException( nameof( section ) );
		}

		if ( line.Contains( IniLine.PairSeparator, StringComparison.OrdinalIgnoreCase ) ) {
			var pos = line.IndexOf( IniLine.PairSeparator, StringComparison.OrdinalIgnoreCase );
			var key = line[ ..pos ].Trimmed();

			if ( !String.IsNullOrEmpty( key ) ) {
				var value = line[ ( pos + IniLine.PairSeparator.Length ).. ].Trimmed();

				if ( this.Add( section, key, value ) ) {
					counter++;
				}
			}
		}

		return counter;
	}

	private Boolean FoundComment( String line, String section ) {
		if ( String.IsNullOrWhiteSpace( line ) ) {
			return false;
		}

		if ( String.IsNullOrWhiteSpace( section ) ) {
			throw new NullException( nameof( section ) );
		}

		if ( line.StartsWith( IniLine.CommentHeaders ).status.IsGood() && this.Add( section, line, null ) ) {
			return true;
		}

		return false;
	}

	private Boolean WriteSection( IDocument document, String section ) {
		if ( document is null ) {
			throw new NullException( nameof( document ) );
		}

		if ( section is null ) {
			throw new NullException( nameof( section ) );
		}

		if ( !this.Data.TryGetValue( section, out var dict ) ) {
			return false; //section not found
		}

		try {
			using var writer = File.AppendText( document.FullPath );

			writer.WriteLine( Encode( section ) );

			foreach ( var pair in dict.OrderBy( pair => pair.Key ) ) {
				writer.WriteLine( Encode( pair ) );
			}

			writer.WriteLine( String.Empty );

			return true;
		}
		catch ( Exception exception ) {
			exception.Log();
		}

		return false;
	}

	private async Task<Boolean> WriteSectionAsync( IDocument document, String section, CancellationToken cancellationToken ) {
		if ( document is null ) {
			throw new NullException( nameof( document ) );
		}

		if ( section is null ) {
			throw new NullException( nameof( section ) );
		}

		try {
			if ( !this.Data.TryGetValue( section, out var dict ) ) {
				return false; //section not found
			}

			var sb = new StringBuilder();

			sb.Append( Encode( section ) );

			foreach ( var pair in dict.OrderBy( pair => pair.Key ) ) {
				if ( cancellationToken.IsCancellationRequested ) {
					return false;
				}

				sb.Append( Encode( pair ) );
			}

			await File.AppendAllTextAsync( document.FullPath, sb.ToString(), cancellationToken ).ConfigureAwait( false );

			return true;
		}
		catch ( Exception exception ) {
			exception.Log();
		}

		return false;
	}

	public static LineType GuessLineType( String line ) {
		if ( line.StartsWith( IniLine.CommentHeaders, StringComparison.CurrentCultureIgnoreCase ).status.IsGood() ) {
			return LineType.Comment;
		}

		if ( line.StartsWith( SectionBegin, StringComparison.CurrentCultureIgnoreCase ) && line.EndsWith( SectionEnd, StringComparison.CurrentCultureIgnoreCase ) ) {
			return LineType.Section;
		}

		return line.Contains( IniLine.PairSeparator, StringComparison.CurrentCultureIgnoreCase ) ? LineType.KVP : LineType.Unknown;
	}

	public static String MakeLineType( LineType lineType, String key, String? value = default ) {
		key = key.Trimmed() ?? String.Empty;

		if ( lineType == LineType.Unknown ) {
			return key;
		}

		if ( GuessLineType( key ) == lineType ) {
			return key;
		}

		if ( lineType == LineType.Comment ) {
			return $"{IniLine.CommentHeaders.First()} {key}";
		}

		if ( lineType == LineType.Section ) {
			return $"{key}{IniLine.PairSeparator}{value.Trimmed()}";
		}

		return key;
	}

	/*
	private static Boolean FoundSection( [NeedsTesting] String? line, [NeedsTesting] out String? section ) {
		section = default;

		line = line.Trimmed();

		if ( String.IsNullOrEmpty( line ) ) {
			return false;
		}

		if ( line.StartsWith( SectionBegin ) && line.EndsWith( SectionEnd ) ) {
			section = line.Substring( SectionBegin.Length, line.Length - ( SectionBegin.Length + SectionEnd.Length ) ).Trimmed();

			if ( !String.IsNullOrEmpty( section ) ) {
				return true;
			}
		}

		return false;
	}
	*/

	public Boolean Add( String? section, String key, String? value ) {
		var sect = section.Trimmed() ?? String.Empty;

		var k = key.Trimmed() ?? throw new NullException( nameof( key ) );

		var retries = 10;
		TryAgain:

		try {
			var dataSection = this.EnsureDataSection( sect );

			var found = dataSection.FirstOrDefault( line => line.Key.Like( k ) );

			if ( found == default( Object ) ) {
				dataSection.Add( k, value );

				return true;
			}

			found.Value = value;

			return true;
		}
		catch ( KeyNotFoundException exception ) {
			exception.Log();
			retries--;

			if ( retries.Any() ) {
				goto TryAgain;
			}
		}

		return false;
	}

	[DebuggerStepThrough]
	public async PooledValueTask<Boolean> Add( IDocument document, CancellationToken cancellationToken ) {
		if ( document is null ) {
			throw new NullException( nameof( document ) );
		}

		if ( await document.Exists( cancellationToken ).ConfigureAwait( false ) == false ) {
			return false;
		}

		try {
			var lines = File.ReadLines( document.FullPath ).Where( line => !String.IsNullOrWhiteSpace( line ) );

			this.Add( lines );

			return true;
		}
		catch ( IOException exception ) {
			//file in use by another app
			exception.Log();

			return false;
		}
		catch ( OutOfMemoryException exception ) {
			//file is big-huge! As my daughter would say.
			exception.Log();

			return false;
		}
	}

	public void Add( String text ) {
		if ( text is null ) {
			throw new NullException( nameof( text ) );
		}

		text = text.Replace( Environment.NewLine, "\n" );

		var lines = text.Split( '\n', StringSplitOptions.RemoveEmptyEntries );

		this.Add( lines );
	}

	public void Add( IEnumerable<String> lines ) {
		if ( lines is null ) {
			throw new NullException( nameof( lines ) );
		}

		String? currentSection = default;

		foreach ( var line in lines ) {
			var lineType = GuessLineType( line );

			switch ( lineType ) {
				case LineType.Unknown: {
					//TODO Do nothing? or add to "bottom" of the "top" of lines, ie Global-Comments-No-Section
					break;
				}

				case LineType.Comment: {
					this.Add( currentSection ?? String.Empty, line, null );

					break;
				}

				case LineType.Section: {
					currentSection = line.Substring( SectionBegin.Length, line.Length - ( SectionBegin.Length + SectionEnd.Length ) ).Trimmed();

					break;
				}

				case LineType.KVP: {
					var pos = line.IndexOf( IniLine.PairSeparator, StringComparison.OrdinalIgnoreCase );
					var key = line[ ..pos ].Trimmed();

					if ( !String.IsNullOrEmpty( key ) ) {
						var value = line[ ( pos + IniLine.PairSeparator.Length ).. ].Trimmed();

						this.Add( currentSection, key, value );
					}

					break;
				}

				default:
					throw new ArgumentOutOfRangeException( nameof( lines ) );
			}
		}
	}

	/// <summary>Return the entire structure as a JSON formatted String.</summary>
	public String AsJSON() {
		var tempDocument = Document.GetTempDocument();

		var writer = File.CreateText( tempDocument.FullPath );

		using ( JsonWriter jw = new JsonTextWriter( writer ) ) {
			jw.Formatting = Formatting.Indented;
			var serializer = new JsonSerializer();
			serializer.Serialize( jw, this.Data );
		}

		var text = File.ReadAllText( tempDocument.FullPath );

		return text;
	}

	/// <summary>Removes all data from all sections.</summary>
	public Boolean Clear() {
		Parallel.ForEach( this.Data.Keys, section => this.TryRemove( section ) );

		return !this.Data.Keys.Any();
	}

	/// <summary>Save the data to the specified document, overwriting it by default.</summary>
	/// <param name="document"></param>
	/// <param name="cancellationToken"></param>
	/// <param name="overwrite"></param>
	public async Task<Boolean> Save( IDocument document, CancellationToken cancellationToken, Boolean overwrite = true ) {
		if ( document is null ) {
			throw new NullException( nameof( document ) );
		}

		if ( await document.Exists( cancellationToken ).ConfigureAwait( false ) ) {
			if ( overwrite ) {
				await document.Delete( cancellationToken ).ConfigureAwait( false );
			}
			else {
				return false;
			}
		}

		await foreach ( var section in this.Data.Keys.OrderBy( section => section ).ToAsyncEnumerable().WithCancellation( cancellationToken ).ConfigureAwait( false ) ) {
			await this.WriteSectionAsync( document, section, cancellationToken ).ConfigureAwait( false );
		}

		return true;
	}

	[DebuggerStepThrough]
	public Boolean TryRemove( String section ) {
		if ( section is null ) {
			throw new NullException( nameof( section ) );
		}

		return this.Data.TryRemove( section, out var _ );
	}

	[DebuggerStepThrough]
	public Boolean TryRemove( String section, String key ) {
		if ( String.IsNullOrWhiteSpace( section ) ) {
			throw new NullException( nameof( section ) );
		}

		if ( String.IsNullOrWhiteSpace( key ) ) {
			throw new NullException( nameof( key ) );
		}

		if ( this.Data.ContainsKey( section ) ) {
			return this.Data[ section ].Remove( key );
		}

		return false;
	}

}