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
// File "FileHistoryFile.cs" last touched on 2021-12-16 at 6:44 AM by Protiguous.

namespace Librainian.FileSystem.FileHistory;

using System;
using System.IO;
using Exceptions;

public class FileHistoryFile {

	private readonly String _filename;

	private readonly IFolder _folder;

	private readonly DateTime? _when;

	public FileHistoryFile( Document biglongpath ) {
		this.OriginalPath = biglongpath;
		this.IsFileHistoryFile = TryParseFileHistoryFile( biglongpath, out this._folder, out this._filename, out this._when );
	}

	/// <summary>(includes the extension)</summary>
	public String FileName => this._filename;

	public IFolder Folder => this._folder;

	public IDocument FullPathAndName => new Document( this.Folder, this.FileName );

	public Boolean IsFileHistoryFile { get; }

	public Document OriginalPath { get; }

	public DateTime? When => this._when;

	/// <summary>Attempt to parse a filename into <see cref="FileHistoryFile" /> parts().</summary>
	/// <param name="original"></param>
	/// <param name="folder"></param>
	/// <param name="filename">(includes the extension)</param>
	/// <param name="when">
	///     <para>Returns the <see cref="DateTime" /> part of this <see cref="Document" /> or null.</para>
	/// </param>
	public static Boolean TryParseFileHistoryFile( Document original, out IFolder? folder, out String? filename, out DateTime? when ) {
		if ( original is null ) {
			throw new ArgumentEmptyException( nameof( original ) );
		}

		filename = null;
		folder = original.ContainingingFolder();
		when = null;

		var extension = Path.GetExtension( original.FullPath ).Trim();

		var value = Path.GetFileNameWithoutExtension( original.FileName ).Trim();

		var posA = value.LastIndexOf( '(' );
		var posB = value.LastIndexOf( "UTC)", StringComparison.Ordinal );

		if ( posA == -1 || posB == -1 || posB < posA ) {
			return false;
		}

		var datepart = value[ ( posA + 1 )..posB ];

		var parts = datepart.Split( ' ', StringSplitOptions.RemoveEmptyEntries );

		parts[ 0 ] = parts[ 0 ].Replace( '_', '/' );
		parts[ 1 ] = parts[ 1 ].Replace( '_', ':' );
		datepart = $"{parts[ 0 ]} {parts[ 1 ]}";

		if ( DateTime.TryParse( datepart, out var result ) ) {
			when = result;

			if ( posA < 1 ) {
				posA = 1;
			}

			filename = $"{value[ ..( posA - "(".Length ) ]}{value[ ( posB + "UTC)".Length ).. ]}{extension}";

			return true;
		}

		filename = $"{value}{extension}";

		return false;
	}

}