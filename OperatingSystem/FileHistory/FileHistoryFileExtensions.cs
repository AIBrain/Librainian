// Copyright 2016 Rick@AIBrain.org.
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
// "Librainian/FileHistoryFileExtensions.cs" was last cleaned by Rick on 2016/06/18 at 10:55 PM

namespace Librainian.OperatingSystem.FileHistory {

    using System;
    using System.IO;
    using FileSystem;
    using JetBrains.Annotations;

    public static class FileHistoryFileExtensions {

        /// <summary>
        ///     Attempt to parse a filename into a <see cref="FileHistoryFileExtensions" /> parts.
        /// </summary>
        /// <param name="original"></param>
        /// <param name="folder"></param>
        /// <param name="filename">(includes the extension)</param>
        /// <param name="when">
        ///     <para>Returns the <see cref="DateTime" /> part of this <see cref="Document" /> or null.</para>
        /// </param>
        /// <returns></returns>
        public static Boolean TryParse( [NotNull] this Document original, [CanBeNull] out Folder folder, [CanBeNull] out String filename, out DateTime? when ) {
            if ( original == null ) {
                throw new ArgumentNullException( nameof( original ) );
            }

            filename = null;
            folder = original.Folder;
            when = null;

            var extension = Path.GetExtension( original.FullPathWithFileName ).Trim();

            var value = Path.GetFileNameWithoutExtension( original.FileName ).Trim();

            var posA = value.LastIndexOf( '(' );
            var posB = value.LastIndexOf( "UTC)", StringComparison.Ordinal );
            if ( ( posA == -1 ) || ( posB == -1 ) || ( posB < posA ) ) {
                return false;
            }

            var datepart = value.Substring( posA + 1, posB - ( posA + 1 ) );
            var parts = datepart.Split( new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries );
            parts[ 0 ] = parts[ 0 ].Replace( '_', '/' );
            parts[ 1 ] = parts[ 1 ].Replace( '_', ':' );
            datepart = parts[ 0 ] + " " + parts[ 1 ];

            DateTime result;
            if ( DateTime.TryParse( datepart, out result ) ) {
                when = result;
                if ( posA < 1 ) {
                    posA = 1;
                }
                filename = value.Substring( 0, posA - "(".Length ) + value.Substring( posB + "UTC)".Length ) + extension;

                return true;
            }
            filename = value + extension;
            return false;
        }
    }
}