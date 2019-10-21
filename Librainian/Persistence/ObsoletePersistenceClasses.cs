// Copyright © Rick@AIBrain.org and Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
//
// This source code contained in "ObsoletePersistenceClasses.cs" belongs to Protiguous@Protiguous.com and
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
// Project: "Librainian", "ObsoletePersistenceClasses.cs" was last formatted by Protiguous on 2019/08/08 at 9:29 AM.

namespace Librainian.Persistence {

    using System;
    using System.ComponentModel;
    using System.IO;
    using System.IO.Compression;
    using System.IO.IsolatedStorage;
    using System.Runtime.Serialization;
    using JetBrains.Annotations;
    using Logging;

    [Obsolete( "Container for obsolete Persistence classes." )]
    public static class ObsoletePersistenceClasses {

        /// <summary>
        ///     Deserialize from an IsolatedStorageFile.
        /// </summary>
        /// <param name="obj" />
        /// <param name="fileName" />
        /// <param name="feedback"> </param>
        /// <returns></returns>
        [Obsolete]
        public static Boolean Load<TSource>( [CanBeNull] out TSource obj, [NotNull] String fileName, [CanBeNull] ProgressChangedEventHandler feedback = null )
            where TSource : class {
            if ( fileName == null ) {
                throw new ArgumentNullException( nameof( fileName ) );
            }

            obj = default;

            try {
                if ( IsolatedStorageFile.IsEnabled && !String.IsNullOrWhiteSpace( fileName ) ) {
                    using ( var isolatedStorageFile = IsolatedStorageFile.GetMachineStoreForDomain() ) {
                        var dir = Path.GetDirectoryName( fileName );

                        if ( !String.IsNullOrWhiteSpace( dir ) && !isolatedStorageFile.DirectoryExists( dir ) ) {
                            isolatedStorageFile.CreateDirectory( dir );
                        }

                        if ( !isolatedStorageFile.FileExists( fileName ) ) {
                            return false;
                        }

                        //if ( 0 == isf.GetFileNames( fileName ).GetLength( 0 ) ) { return false; }

                        var deletefile = false;

                        try {
                            using ( var test = isolatedStorageFile.OpenFile( fileName, FileMode.Open, FileAccess.Read, FileShare.Read ) ) {
                                var length = test.Seek( 0, SeekOrigin.End );

                                if ( length <= 3 ) {
                                    deletefile = true;
                                }
                            }
                        }
                        catch ( IsolatedStorageException exception ) {
                            exception.Log();

                            return false;
                        }

                        try {
                            if ( deletefile ) {
                                isolatedStorageFile.DeleteFile( fileName );

                                return false;
                            }
                        }
                        catch ( IsolatedStorageException exception ) {
                            exception.Log();

                            return false;
                        }

                        try {
                            var fileStream = new IsolatedStorageFileStream( fileName, mode: FileMode.Open, access: FileAccess.Read, isf: isolatedStorageFile );
                            var ext = Path.GetExtension( fileName );
                            var useCompression = ext.EndsWith( "Z", ignoreCase: true, culture: null );

                            if ( useCompression ) {
                                using ( var decompress = new GZipStream( stream: fileStream, mode: CompressionMode.Decompress, leaveOpen: true ) ) {
                                    obj = PersistenceExtensions.Deserialize<TSource>( stream: decompress, feedback: feedback );
                                }
                            }
                            else {

                                //obj = Deserialize<TSource>( stream: isfs, feedback: feedback );
                                var serializer = new NetDataContractSerializer();
                                obj = serializer.Deserialize( fileStream ) as TSource;
                            }

                            return obj != default( TSource );
                        }
                        catch ( InvalidOperationException exception ) {
                            exception.Log();
                        }
                        catch ( ArgumentNullException exception ) {
                            exception.Log();
                        }
                        catch ( SerializationException exception ) {
                            deletefile = true;
                            exception.Log();
                        }
                        catch ( Exception exception ) {
                            exception.Log();
                        }

                        try {
                            if ( deletefile ) {
                                isolatedStorageFile.DeleteFile( fileName );

                                return false;
                            }
                        }
                        catch ( IsolatedStorageException exception ) {
                            exception.Log();

                            return false;
                        }
                    }
                }
            }
            catch ( IsolatedStorageException exception ) {
                exception.Log();
            }

            return false;
        }
    }
}