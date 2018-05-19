// Copyright © 1995-2018 to Rick@AIBrain.org and Protiguous.
// All Rights Reserved.
//
// This ENTIRE copyright notice and file header MUST BE KEPT
// VISIBLE in any source code derived from or used from our
// libraries and projects.
//
// =========================================================
// This section of source code, "Class1.cs",
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
// "Librainian/Librainian/Class1.cs" was last cleaned by Protiguous on 2018/05/16 at 4:52 AM.

namespace Librainian.Persistence {

    using System;
    using System.ComponentModel;
    using System.IO;
    using System.IO.Compression;
    using System.IO.IsolatedStorage;
    using System.Runtime.Serialization;
    using JetBrains.Annotations;

    [Obsolete( "Container for obsolete Persistence classes." )]
    public class ObsoletePersistenceClasses {

        /// <summary>
        ///     Deserialize from an IsolatedStorageFile.
        /// </summary>
        /// <param name="obj" />
        /// <param name="fileName" />
        /// <param name="feedback"> </param>
        /// <returns></returns>
        [Obsolete]
        public static Boolean Load<TSource>( out TSource obj, [NotNull] String fileName, ProgressChangedEventHandler feedback = null ) where TSource : class {
            if ( fileName is null ) { throw new ArgumentNullException( nameof( fileName ) ); }

            obj = default;

            try {
                if ( IsolatedStorageFile.IsEnabled && !String.IsNullOrWhiteSpace( fileName ) ) {
                    using ( var isolatedStorageFile = IsolatedStorageFile.GetMachineStoreForDomain() ) {
                        var dir = Path.GetDirectoryName( fileName );

                        if ( !String.IsNullOrWhiteSpace( dir ) && !isolatedStorageFile.DirectoryExists( dir ) ) { isolatedStorageFile.CreateDirectory( dir ); }

                        if ( !isolatedStorageFile.FileExists( fileName ) ) { return false; }

                        //if ( 0 == isf.GetFileNames( fileName ).GetLength( 0 ) ) { return false; }

                        var deletefile = false;

                        try {
                            using ( var test = isolatedStorageFile.OpenFile( fileName, FileMode.Open, FileAccess.Read, FileShare.Read ) ) {
                                var length = test.Seek( 0, SeekOrigin.End );

                                if ( length <= 3 ) { deletefile = true; }
                            }
                        }
                        catch ( IsolatedStorageException exception ) {
                            exception.More();

                            return false;
                        }

                        try {
                            if ( deletefile ) {
                                isolatedStorageFile.DeleteFile( fileName );

                                return false;
                            }
                        }
                        catch ( IsolatedStorageException exception ) {
                            exception.More();

                            return false;
                        }

                        try {
                            var fileStream = new IsolatedStorageFileStream( fileName, mode: FileMode.Open, access: FileAccess.Read, isf: isolatedStorageFile );
                            var ext = Path.GetExtension( fileName );
                            var useCompression = ext.EndsWith( "Z", ignoreCase: true, culture: null );

                            if ( useCompression ) {
                                using ( var decompress = new GZipStream( stream: fileStream, mode: CompressionMode.Decompress, leaveOpen: true ) ) { obj = PersistenceExtensions.Deserialize<TSource>( stream: decompress, feedback: feedback ); }
                            }
                            else {

                                //obj = Deserialize<TSource>( stream: isfs, feedback: feedback );
                                var serializer = new NetDataContractSerializer();
                                obj = serializer.Deserialize( fileStream ) as TSource;
                            }

                            return obj != default( TSource );
                        }
                        catch ( InvalidOperationException exception ) { exception.More(); }
                        catch ( ArgumentNullException exception ) { exception.More(); }
                        catch ( SerializationException exception ) {
                            deletefile = true;
                            exception.More();
                        }
                        catch ( Exception exception ) { exception.More(); }

                        try {
                            if ( deletefile ) {
                                isolatedStorageFile.DeleteFile( fileName );

                                return false;
                            }
                        }
                        catch ( IsolatedStorageException exception ) {
                            exception.More();

                            return false;
                        }
                    }
                }
            }
            catch ( IsolatedStorageException exception ) { exception.More(); }

            return false;
        }

    }
}