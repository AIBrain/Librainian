// Copyright © Rick@AIBrain.org and Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
//
// This source code contained in "FileHistoryFile.cs" belongs to Protiguous@Protiguous.com and
// Rick@AIBrain.org unless otherwise specified or the original license has
// been overwritten by formatting.
// (We try to avoid it from happening, but it does accidentally happen.)
//
// Any unmodified portions of source code gleaned from other projects still retain their original
// license and our thanks goes to those Authors. If you find your code in this source code, please
// let us know so we can properly attribute you and include the proper license and/or copyright.
//
// If you want to use any of our code, you must contact Protiguous@Protiguous.com or
// Sales@AIBrain.org for permission and a quote.
//
// Donations are accepted (for now) via
//     bitcoin:1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//     PayPal:Protiguous@Protiguous.com
//     (We're always looking into other solutions.. Any ideas?)
//
// =========================================================
// Disclaimer:  Usage of the source code or binaries is AS-IS.
//    No warranties are expressed, implied, or given.
//    We are NOT responsible for Anything You Do With Our Code.
//    We are NOT responsible for Anything You Do With Our Executables.
//    We are NOT responsible for Anything You Do With Your Computer.
// =========================================================
//
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
// For business inquiries, please contact me at Protiguous@Protiguous.com
//
// Our website can be found at "https://Protiguous.com/"
// Our software can be found at "https://Protiguous.Software/"
// Our GitHub address is "https://github.com/Protiguous".
// Feel free to browse any source code we make available.
//
// Project: "Librainian", "FileHistoryFile.cs" was last formatted by Protiguous on 2019/08/08 at 9:15 AM.

namespace LibrainianCore.OperatingSystem.FileSystem.FileHistory {

    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;

    public class FileHistoryFile {

        private readonly String _filename;

        private readonly IFolder _folder;

        private readonly DateTime? _when;

        [NotNull]
        public IDocument FullPathAndName => new Document( folder: this.Folder, filename: this.FileName );

        public Boolean IsFileHistoryFile { get; }

        public Document OriginalPath { get; }

        public DateTime? When => this._when;

        /// <summary>
        ///     (includes the extension)
        /// </summary>
        public String FileName => this._filename;

        public IFolder Folder => this._folder;

        public FileHistoryFile( [NotNull] Document biglongpath ) {
            this.OriginalPath = biglongpath;
            this.IsFileHistoryFile = TryParseFileHistoryFile( biglongpath, folder: out this._folder, filename: out this._filename, when: out this._when );
        }

        /// <summary>
        ///     Attempt to parse a filename into <see cref="FileHistoryFile" /> parts().
        /// </summary>
        /// <param name="original"></param>
        /// <param name="folder"></param>
        /// <param name="filename">(includes the extension)</param>
        /// <param name="when">
        ///     <para>Returns the <see cref="DateTime" /> part of this <see cref="Document" /> or null.</para>
        /// </param>
        /// <returns></returns>
        public static Boolean TryParseFileHistoryFile( [NotNull] Document original, [CanBeNull] out IFolder folder, [CanBeNull] out String filename, out DateTime? when ) {
            if ( original is null ) {
                throw new ArgumentNullException( nameof( original ) );
            }

            filename = null;
            folder = original.ContainingingFolder();
            when = null;

            var extension = Path.GetExtension( original.FullPath ).Trim();

            var value = Path.GetFileNameWithoutExtension( original.FileName ).Trim();

            var posA = value.LastIndexOf( '(' );
            var posB = value.LastIndexOf( "UTC)", comparisonType: StringComparison.Ordinal );

            if ( posA == -1 || posB == -1 || posB < posA ) {
                return false;
            }

            var datepart = value.Substring( startIndex: posA + 1, posB - ( posA + 1 ) );

            var parts = datepart.Split( separator: new[] {
                ' '
            }, options: StringSplitOptions.RemoveEmptyEntries );

            parts[ 0 ] = parts[ 0 ].Replace( oldChar: '_', newChar: '/' );
            parts[ 1 ] = parts[ 1 ].Replace( oldChar: '_', newChar: ':' );
            datepart = $"{parts[ 0 ]} {parts[ 1 ]}";

            if ( DateTime.TryParse( datepart, result: out var result ) ) {
                when = result;

                if ( posA < 1 ) {
                    posA = 1;
                }

                filename = $"{value.Substring( startIndex: 0, posA - "(".Length )}{value.Substring( startIndex: posB + "UTC)".Length )}{extension}";

                return true;
            }

            filename = $"{value}{extension}";

            return false;
        }
    }
}