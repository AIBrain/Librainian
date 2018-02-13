// Copyright 2018 Rick@AIBrain.org.
// 
// This notice must be kept visible in the source.
// 
// This section of source code belongs to Rick@AIBrain.Org unless otherwise specified, or the
// original license has been overwritten by the automatic formatting of this code. Any unmodified
// sections of source code borrowed from other projects retain their original license and thanks
// goes to the Authors.
// 
// Donations and royalties can be paid via
//  PayPal: paypal@aibrain.org
//  bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//  litecoin: LeUxdU2w3o6pLZGVys5xpDZvvo8DUrjBp9
// 
// Usage of the source code or compiled binaries is AS-IS. I am not responsible for Anything You Do.
// 
// Contact me by email if you have any questions or helpful criticism.
// 
// "Librainian/FileHistoryFileExtensions.cs" was last cleaned by Rick on 2018/02/01 at 9:51 PM

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
        public static Boolean TryParse( [ NotNull ] this Document original, [ CanBeNull ] out Folder folder, [ CanBeNull ] out String filename, out DateTime? when ) {
            if ( original == null ) {
                throw new ArgumentNullException( paramName: nameof(original) );
            }

            filename = null;
            folder = original.Folder;
            when = null;

            var extension = Path.GetExtension( path: original.FullPathWithFileName ).Trim();

            var value = Path.GetFileNameWithoutExtension( path: original.FileName() ).Trim();

            var posA = value.LastIndexOf( value: '(' );
            var posB = value.LastIndexOf( "UTC)", comparisonType: StringComparison.Ordinal );
            if ( posA == -1 || posB == -1 || posB < posA ) {
                return false;
            }

            var datepart = value.Substring( startIndex: posA + 1, length: posB - ( posA + 1 ) );
            var parts = datepart.Split( separator: new[] { ' ' }, options: StringSplitOptions.RemoveEmptyEntries );
            parts[ 0 ] = parts[ 0 ].Replace( oldChar: '_', newChar: '/' );
            parts[ 1 ] = parts[ 1 ].Replace( oldChar: '_', newChar: ':' );
            datepart = parts[ 0 ] + " " + parts[ 1 ];

            if ( DateTime.TryParse( s: datepart, result: out var result ) ) {
                when = result;
                if ( posA < 1 ) {
                    posA = 1;
                }

                filename = value.Substring( startIndex: 0, length: posA - "(".Length ) + value.Substring( startIndex: posB + "UTC)".Length ) + extension;

                return true;
            }

            filename = value + extension;
            return false;
        }
    }
}