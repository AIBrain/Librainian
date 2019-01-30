// Copyright © Rick@AIBrain.org and Protiguous. All Rights Reserved.
// 
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
// 
// This source code contained in "FileAttributeData.cs" belongs to Protiguous@Protiguous.com and
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
//     paypal@AIBrain.Org
//     (We're still looking into other solutions! Any ideas?)
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
// Feel free to browse any source code we *might* make available.
// 
// Project: "Librainian", "FileAttributeData.cs" was last formatted by Protiguous on 2019/01/29 at 10:46 PM.

namespace Librainian.ComputerSystem.FileSystem {

    using System;
    using System.IO;
    using JetBrains.Annotations;
    using OperatingSystem;

    /// <summary>
    ///     Modern version of <see cref="NativeMethods.WIN32_FILE_ATTRIBUTE_DATA" />.
    /// </summary>
    public struct FileAttributeData {

        public DateTime? CreationTime { get; protected internal set; }

        public Boolean? Exists { get; protected internal set; }

        public FileAttributes? FileAttributes { get; protected internal set; }

        public Int64? FileHashCode { get; protected internal set; }

        public UInt64? FileSize { get; protected internal set; }

        public DateTime? LastAccessTime { get; protected internal set; }

        public DateTime? LastWriteTime { get; protected internal set; }

        /// <summary>
        ///     Populate all properties with null values.
        /// </summary>
        public FileAttributeData( Boolean _ = true ) : this() => this.Reset();

        /// <summary>
        ///     Populates from a <see cref="NativeMethods.WIN32_FILE_ATTRIBUTE_DATA" /> struct.
        /// </summary>
        /// <param name="fileAttributeData"></param>
        public FileAttributeData( NativeMethods.WIN32_FILE_ATTRIBUTE_DATA fileAttributeData ) {
            this.FileAttributes = fileAttributeData.dwFileAttributes;
            this.CreationTime = fileAttributeData.ftCreationTime.ToDateTime();
            this.LastAccessTime = fileAttributeData.ftLastAccessTime.ToDateTime();
            this.LastWriteTime = fileAttributeData.ftLastWriteTime.ToDateTime();
            this.FileSize = ( ( UInt64 ) fileAttributeData.nFileSizeHigh << 32 ) + fileAttributeData.nFileSizeLow;
            this.Exists = true;
            this.FileHashCode = default;
        }

        /// <summary>
        ///     Populates from a <see cref="NativeMethods.Win32FindData" /> struct.
        /// </summary>
        /// <param name="findData"></param>
        public FileAttributeData( NativeMethods.Win32FindData findData ) {
            this.FileAttributes = findData.dwFileAttributes;
            this.CreationTime = findData.ftCreationTime.ToDateTime();
            this.LastAccessTime = findData.ftLastAccessTime.ToDateTime();
            this.LastWriteTime = findData.ftLastWriteTime.ToDateTime();
            this.FileSize = ( ( UInt64 ) findData.nFileSizeHigh << 32 ) + findData.nFileSizeLow;
            this.Exists = true;
            this.FileHashCode = default;
        }

        public FileAttributeData( Boolean exists, FileAttributes attributes, DateTime creationTime, DateTime lastAccessTime, DateTime lastWriteTime, UInt64 fileSize ) {
            this.Exists = exists;
            this.FileAttributes = attributes;
            this.CreationTime = creationTime;
            this.LastAccessTime = lastAccessTime;
            this.LastWriteTime = lastWriteTime;
            this.FileSize = fileSize;
            this.FileHashCode = default;
        }

        public Boolean Refresh( [NotNull] String fullPath, Boolean throwOnError = true ) {
            this.Reset();

            if ( String.IsNullOrWhiteSpace( value: fullPath ) ) {
                throw new ArgumentException( message: "Value cannot be null or whitespace.", paramName: nameof( fullPath ) );
            }

            var handle = NativeMethods.FindFirstFile( fullPath, out var data );

            if ( handle.IsInvalid ) {
                if ( throwOnError ) {
                    NativeMethods.HandleLastError( fullPath );
                }
                else {
                    this.Reset();

                    return false;
                }
            }

            var fileAttributeData = new FileAttributeData( data );
            this.Exists = fileAttributeData.Exists;
            this.FileAttributes = fileAttributeData.FileAttributes;
            this.CreationTime = fileAttributeData.CreationTime;
            this.LastAccessTime = fileAttributeData.LastAccessTime;
            this.LastWriteTime = fileAttributeData.LastWriteTime;
            this.FileSize = fileAttributeData.FileSize;
            this.FileHashCode = default;

            return true;
        }

        /// <summary>
        ///     Reset known information about file to defaults.
        /// </summary>
        public void Reset() {
            this.Exists = default;
            this.FileAttributes = default;
            this.CreationTime = default;
            this.LastAccessTime = default;
            this.LastWriteTime = default;
            this.FileSize = default;
            this.FileHashCode = default;
        }

        /*
        public Task<Boolean> GetHash() {

            //crc32/64?
            return Task.Run( () => {

                return ( FileAttributes, this.CreationTime ).GetHashCode();
            } );
        }
        */

    }

}