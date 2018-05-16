// Copyright © 1995-2018 to Rick@AIBrain.org and Protiguous.
// All Rights Reserved.
//
// This ENTIRE copyright notice and file header MUST BE KEPT
// VISIBLE in any source code derived from or used from our
// libraries and projects.
//
// =========================================================
// This section of source code, "FileHistoryFileExtensions.cs",
// belongs to Rick@AIBrain.org and Protiguous@Protiguous.com
// unless otherwise specified OR the original license has been
// overwritten by the automatic formatting.
//
// (We try to avoid that from happening, but it does happen.)
//
// Any unmodified portions of source code gleaned from other
// projects still retain their original license and our thanks
// goes to those Authors.
// =========================================================
//
// Donations (more please!), royalties from any software that
// uses any of our code, and license fees can be paid to us via
// bitcoin at the address 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2.
//
// =========================================================
// Usage of the source code or compiled binaries is AS-IS.
// No warranties are expressed or implied.
// I am NOT responsible for Anything You Do With Our Code.
// =========================================================
//
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
//
// "Librainian/Librainian/FileHistoryFileExtensions.cs" was last cleaned by Protiguous on 2018/05/15 at 10:48 PM.

namespace Librainian.OperatingSystem.FileHistory {

    using System;
    using System.IO;
    using FileSystem;
    using JetBrains.Annotations;

    public static class FileHistoryFileExtensions {

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
        public static Boolean TryParse( [NotNull] this Document original, [CanBeNull] out Folder folder, [CanBeNull] out String filename, out DateTime? when ) {
            if ( original is null ) { throw new ArgumentNullException( nameof( original ) ); }

            filename = null;
            folder = original.Folder;
            when = null;

            var extension = Path.GetExtension( original.FullPathWithFileName ).Trim();

            var value = Path.GetFileNameWithoutExtension( original.FileName() ).Trim();

            var posA = value.LastIndexOf( '(' );
            var posB = value.LastIndexOf( "UTC)", comparisonType: StringComparison.Ordinal );

            if ( posA == -1 || posB == -1 || posB < posA ) { return false; }

            var datepart = value.Substring( startIndex: posA + 1, posB - ( posA + 1 ) );
            var parts = datepart.Split( separator: new[] { ' ' }, options: StringSplitOptions.RemoveEmptyEntries );
            parts[0] = parts[0].Replace( oldChar: '_', newChar: '/' );
            parts[1] = parts[1].Replace( oldChar: '_', newChar: ':' );
            datepart = parts[0] + " " + parts[1];

            if ( DateTime.TryParse( s: datepart, result: out var result ) ) {
                when = result;

                if ( posA < 1 ) { posA = 1; }

                filename = value.Substring( startIndex: 0, posA - "(".Length ) + value.Substring( startIndex: posB + "UTC)".Length ) + extension;

                return true;
            }

            filename = value + extension;

            return false;
        }
    }
}