#region License & Information

// This notice must be kept visible in the source.
//
// This section of source code belongs to Rick@AIBrain.Org unless otherwise specified,
// or the original license has been overwritten by the automatic formatting of this code.
// Any unmodified sections of source code borrowed from other projects retain their original license and thanks goes to the Authors.
//
// Donations and Royalties can be paid via
// PayPal: paypal@aibrain.org
// bitcoin:1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
// bitcoin:1NzEsF7eegeEWDr5Vr9sSSgtUC4aL6axJu
// litecoin:LeUxdU2w3o6pLZGVys5xpDZvvo8DUrjBp9
//
// Usage of the source code or compiled binaries is AS-IS.
// I am not responsible for Anything You Do.
//
// Contact me by email if you have any questions or helpful criticism.
//
// "Librainian/PersistenceExtensions.cs" was last cleaned by Rick on 2014/09/06 at 7:37 AM

#endregion License & Information

namespace Librainian.Persistence {
    using System;
    using System.Collections.Concurrent;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.IO;
    using System.IO.Compression;
    using System.IO.IsolatedStorage;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.Serialization;
    using System.Runtime.Serialization.Formatters;
    using System.Security;
    using System.ServiceModel;
    using System.Text;
    using System.Threading;
    using System.Windows.Forms;
    using System.Xml;
    using Annotations;
    using CodeFluent.Runtime.BinaryServices;
    using Collections;
    using Extensions;
    using IO;
    using IO.Streams;
    using Measurement.Time;
    using Parsing;
    using Threading;

    public static class PersistenceExtensions {

        ///// <summary>
        /////   Attempts to Add() the specified filename into the collection.
        ///// </summary>
        ///// <typeparam name = "T"></typeparam>
        ///// <param name = "collection"></param>
        ///// <param name = "fileName"></param>
        ///// <returns></returns>
        //public static Boolean LoadCollection< T >( this IProducerConsumerCollection< T > collection, String fileName ) {
        //    if ( collection == null ) {
        //        throw new ArgumentNullException( "collection" );
        //    }
        //    if ( fileName == null ) {
        //        throw new ArgumentNullException( "fileName" );
        //    }
        //    IProducerConsumerCollection< T > temp;
        //    if ( Storage.Load( out temp, fileName ) ) {
        //        if ( null != temp ) {
        //            var result = Parallel.ForEach( temp, collection.Add );
        //            return result.IsCompleted;
        //        }
        //    }
        //    return false;
        //}

        [NotNull]
        internal static readonly ThreadLocal<NetDataContractSerializer> Serializers = new ThreadLocal<NetDataContractSerializer>( () => new NetDataContractSerializer( context: StreamingContexts.Value, maxItemsInObjectGraph: Int32.MaxValue, ignoreExtensionDataObject: false, assemblyFormat: FormatterAssemblyStyle.Simple, surrogateSelector: null ) );

        ///// <summary>
        /////   Attempts to Add() the specified filename into the collection.
        ///// </summary>
        ///// <typeparam name = "T"></typeparam>
        ///// <param name = "collection"></param>
        ///// <param name = "fileName"></param>
        ///// <returns></returns>
        //public static Boolean LoadCollection<T>( this ConcurrentSet<T> collection, String fileName ) where T : class {
        //    if ( collection == null ) {
        //        throw new ArgumentNullException( "collection" );
        //    }
        //    if ( fileName == null ) {
        //        throw new ArgumentNullException( "fileName" );
        //    }
        //    ConcurrentSet<T> temp;
        //    if ( Storage.Load( out temp, fileName ) ) {
        //        if ( null != temp ) {
        //            collection.AddRange( temp );
        //            return true;
        //        }
        //    }
        //    return false;
        //}
        internal static readonly ThreadLocal<StreamingContext> StreamingContexts = new ThreadLocal<StreamingContext>( () => new StreamingContext( StreamingContextStates.All ) );

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TType"></typeparam>
        /// <param name="storedAsString"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="EncoderFallbackException"></exception>
        /// <exception cref="FormatException"></exception>
        /// <exception cref="XmlException"></exception>
        [CanBeNull]
        public static TType Deserialize<TType>( this String storedAsString ) where TType : class {
            try {
                var byteArray = Encoding.UTF8.GetBytes( storedAsString ); //we can .Base64Encode() if we need.

                using ( var ms = new MemoryStream( byteArray ) ) {
                    ms.Position = 0;
                    var serializer = Serializers.Value;
                    var deSerialized = serializer.ReadObject( ms ) as TType;
                    return deSerialized;
                }
            }
            catch ( SerializationException exception ) {
                exception.Error();
            }
            return null;
        }

        /// <summary>
        ///     Persist an object to an IsolatedStorageFile.<br />
        ///     Mark class with [DataContract( Namespace = "http://aibrain.org" )]<br />
        ///     Mark fields with [DataMember, OptionalField] to serialize (both public and private).<br />
        ///     Properties have to have both the Getter and the Setter.<br />
        /// </summary>
        /// <param name="collection"></param>
        /// <param name="fileName"></param>
        /// <returns>Returns True if the object was saved.</returns>
        [Obsolete( "Not in use yet." )]
        public static Boolean SaveCollection<T>( this IProducerConsumerCollection<T> collection, String fileName ) {
            if ( collection == null ) {
                throw new ArgumentNullException( "collection" );
            }
            if ( String.IsNullOrWhiteSpace( fileName ) ) {
                throw new ArgumentNullException( "fileName" );
            }

            return collection.Saver( fileName: fileName );
        }

        /// <summary>
        ///     Persist an object to an IsolatedStorageFile.<br />
        ///     Mark class with [DataContract( Namespace = "http://aibrain.org" )]<br />
        ///     Mark fields with [DataMember, OptionalField] to serialize (both public and private).<br />
        ///     Properties have to have both the Getter and the Setter.<br />
        /// </summary>
        /// <param name="collection"></param>
        /// <param name="fileName"></param>
        /// <returns>Returns True if the object was saved.</returns>
        [Obsolete( "Not in use yet." )]
        public static Boolean SaveCollection<T>( this ThreadSafeList<T> collection, String fileName ) where T : class {
            if ( collection == null ) {
                throw new ArgumentNullException( "collection" );
            }
            if ( String.IsNullOrWhiteSpace( fileName ) ) {
                throw new ArgumentNullException( "fileName" );
            }

            return collection.Saver( fileName: fileName );
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TType"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        [CanBeNull]
        public static String Serialize<TType>( this TType obj ) where TType : class {
            try {
                using ( var stream = new MemoryStream() ) {
                    var serializer = Serializers.Value;
                    serializer.WriteObject( stream, obj );
                    return stream.ReadToEnd();
                }
            }
            catch ( SerializationException exception ) {
                exception.Error();
            }
            catch ( InvalidDataContractException exception ) {
                exception.Error();
            }
            return null;
        }

        /// <summary>
        /// <para><see cref="Folder"/> to store (and pull) application data (current user, local machine, per executable).</para>
        /// <para><see cref="Environment.SpecialFolder.LocalApplicationData"/></para>
        /// </summary>
        public static readonly Lazy<Folder> DataFolder = new Lazy<Folder>( () => {
            //var folderPath = Environment.GetFolderPath( Environment.SpecialFolder.LocalApplicationData );
            //var fileNameWithoutExtension = Path.GetFileNameWithoutExtension( ( Assembly.GetExecutingAssembly() ?? Assembly.GetEntryAssembly() ).Location );
            //var fullPath = Path.Combine( folderPath, fileNameWithoutExtension );

            var folder = new Folder( Environment.SpecialFolder.LocalApplicationData, Application.CompanyName, Application.ProductName );
            if ( !folder.Exists() ) {
                folder.Create();
            }
            return folder;
        } );

        [Obsolete]
        public static Boolean EnableIsolatedStorageCompression() {
            using ( var isf = IsolatedStorageFile.GetMachineStoreForDomain() ) {
                var myType = isf.GetType();
                var myFields = myType.GetFields( bindingAttr: BindingFlags.Instance | BindingFlags.NonPublic );
                if ( myFields.All( f => f.Name != "m_RootDir" ) ) {
                    return false;
                }
                var myField = myType.GetField( name: "m_RootDir", bindingAttr: BindingFlags.Instance | BindingFlags.NonPublic );
                if ( myField == null ) {
                    return false;
                }
                var path = myField.GetValue( isf ) as String;
                if ( String.IsNullOrWhiteSpace( path ) ) {
                    return false;
                }
                try {
                    var dir = new DirectoryInfo( path );
                    if ( dir.Exists ) {
                        var result = dir.SetCompression( true );
                        if ( result ) {
                            String.Format( "Enabled compression in IsolatedStorage @ {0}", path ).TimeDebug();
                        }

                        return result;
                    }
                }
                catch ( SecurityException exception ) {
                    exception.Error();
                }
                catch ( ArgumentException exception ) {
                    exception.Error();
                }
                catch ( PathTooLongException exception ) {
                    exception.Error();
                }
            }
            return false;
        }

        /// <summary>
        ///     Deserialize from an IsolatedStorageFile.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="fileName"></param>
        /// <returns></returns>
        [Obsolete]
        public static T Load<T>( [CanBeNull] String fileName ) where T : class, new() {
            try {
                if ( String.IsNullOrEmpty( fileName ) ) {
                    return new T();
                }

                using ( var isf = IsolatedStorageFile.GetMachineStoreForDomain() ) {
                    var dir = Path.GetDirectoryName( fileName );
                    if ( !String.IsNullOrEmpty( dir ) ) {
                        isf.CreateDirectory( dir );
                    }

                    if ( 0 == isf.GetFileNames( fileName ).GetLength( 0 ) ) {
                        return new T();
                    }

                    try {
                        using ( var isfs = new IsolatedStorageFileStream( fileName, FileMode.Open, FileAccess.Read, isf ) ) {
                            //var serializer = new DataContractSerializer( typeof( T ) );
                            var serializer = new NetDataContractSerializer();
                            var obj = serializer.ReadObject( isfs ) as T;
                            isfs.Close();
                            return obj;
                        }
                    }
                    catch ( SerializationException exception ) {
                        exception.Error();
                    }
                    catch ( IsolatedStorageException exception ) {
                        exception.Error();
                    }
                }
            }
            catch ( Exception exception ) {
                exception.Error();
            }
            return new T();
        }

        /// <summary>
        ///     Deserialize from an IsolatedStorageFile.
        /// </summary>
        /// <param name="obj" />
        /// <param name="fileName" />
        /// <returns></returns>
        [Obsolete]
        public static Boolean LoadValue<T>( out T obj, String fileName ) where T : struct {
            obj = default( T );
            try {
                if ( String.IsNullOrEmpty( fileName ) ) {
                    return false;
                }
                using ( var isf = IsolatedStorageFile.GetMachineStoreForDomain() ) {
                    var dir = Path.GetDirectoryName( fileName );
                    if ( !String.IsNullOrWhiteSpace( dir ) && !isf.DirectoryExists( dir ) ) {
                        isf.CreateDirectory( dir );
                    }

                    if ( !isf.FileExists( fileName ) ) {
                        return false;
                    }

                    var deletefile = false;
                    try {
                        using ( var test = isf.OpenFile( fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite ) ) {
                            var length = test.Seek( 0, SeekOrigin.End );
                            if ( length <= 3 ) {
                                deletefile = true;
                            }
                        }
                    }
                    catch ( IsolatedStorageException exception ) {
                        exception.Error();
                        return false;
                    }

                    try {
                        if ( deletefile ) {
                            isf.DeleteFile( fileName );
                            return false;
                        }
                    }
                    catch ( IsolatedStorageException exception ) {
                        exception.Error();
                        return false;
                    }

                    try {
                        using ( var isfs = new IsolatedStorageFileStream( path: fileName, mode: FileMode.Open, access: FileAccess.Read, isf: isf ) ) {
                            //var serializer = new DataContractSerializer( typeof ( T ) );
                            var serializer = new NetDataContractSerializer();

                            var ext = Path.GetExtension( path: fileName );
                            var useCompression = ext.EndsWith( value: "Z", ignoreCase: true, culture: null );

                            if ( useCompression ) {
                                using ( var decompress = new GZipStream( stream: isfs, mode: CompressionMode.Decompress, leaveOpen: true ) ) {
                                    obj = ( T )serializer.ReadObject( stream: decompress );
                                }
                            }
                            else {
                                obj = ( T )serializer.ReadObject( stream: isfs );
                            }

                            return !Equals( obj, default( T ) );
                        }
                    }
                    catch ( InvalidOperationException exception ) {
                        exception.Error();
                        return false;
                    }
                    catch ( ArgumentNullException exception ) {
                        exception.Error();
                        return false;
                    }
                    catch ( SerializationException exception ) {
                        exception.Error();
                        return false;
                    }
                    catch ( Exception exception ) {
                        exception.Error();
                        return false;
                    }
                }
            }
            catch ( IsolatedStorageException exception ) {
                exception.Error();
            }
            return false;
        }

        /// <summary>
        ///     Persist an object to an IsolatedStorageFile.<br />
        ///     Mark class with [DataContract( Namespace = "http://aibrain.org" )]<br />
        ///     Mark fields with [DataMember, OptionalField] to serialize (both public and private).<br />
        ///     Properties have to have both the Getter and the Setter.<br />
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="objectToSerialize"></param>
        /// <param name="fileName"></param>
        /// <returns>Returns True if the object was saved.</returns>
        public static Boolean Saver<TSource>( [CanBeNull] this TSource objectToSerialize, [NotNull] String fileName ) where TSource : class {
            //TODO pass in a backup flag to save the newest copy with the backup time.
            // or a Backup class ?

            if ( null == objectToSerialize ) {
                return false;
            }

            if ( fileName == null ) {
                throw new ArgumentNullException( "fileName" );
            }

            try {
                if ( !IsolatedStorageFile.IsEnabled || String.IsNullOrWhiteSpace( fileName ) ) {
                    return false;
                }

                using ( var snag = new FileSingleton( "IsolatedStorageFile.GetMachineStoreForDomain()" ) ) {
                    using ( var isf = IsolatedStorageFile.GetMachineStoreForDomain() ) {
                        snag.Dispose();

                        try {
                            var dir = Path.GetDirectoryName( fileName ) ?? String.Empty;
                            if ( !String.IsNullOrWhiteSpace( dir ) && !isf.DirectoryExists( dir ) ) {
                                isf.CreateDirectory( dir );
                            }
                        }
                        catch ( IsolatedStorageException exception ) {
                            exception.Error();
                            return false;
                        }
                        catch ( PathTooLongException exception ) {
                            exception.Error();
                            return false;
                        }
                        catch ( ArgumentException exception ) {
                            exception.Error();
                            return false;
                        }

                        try {
                            var isfs = new IsolatedStorageFileStream( fileName, isf.FileExists( path: fileName ) ? FileMode.Truncate : FileMode.CreateNew, FileAccess.Write, isf );
                            using ( isfs ) {
                                //var serializer = new DataContractSerializer( typeof( TSource ) );

                                var context = new StreamingContext( StreamingContextStates.All );
                                //var surrogateSelector = new SurrogateSelector();

                                //if we ever need to add in surrogates..
                                //var timeronSerializationSurrogate = new TimeronSerializationSurrogate();
                                //surrogateSelector.AddSurrogate( typeof( Timeron ), context, timeronSerializationSurrogate );

                                var serializer = new NetDataContractSerializer( context: context, maxItemsInObjectGraph: Int32.MaxValue, ignoreExtensionDataObject: false, assemblyFormat: FormatterAssemblyStyle.Simple, surrogateSelector: null /*surrogateSelector*/ );

                                var extension = Path.GetExtension( path: fileName );
                                var useCompression = !String.IsNullOrWhiteSpace( extension ) && extension.EndsWith( value: "Z", ignoreCase: true, culture: null );

                                if ( useCompression ) {
                                    using ( var compress = new GZipStream( isfs, CompressionMode.Compress, leaveOpen: true ) ) {
                                        serializer.Serialize( compress, objectToSerialize );
                                    }
                                }
                                else {
                                    serializer.Serialize( isfs, objectToSerialize );
                                }
                                isfs.Close();
                                return true;
                            }
                        }
                        catch ( InvalidDataContractException exception ) {
                            exception.Error();
                        }
                        catch ( SerializationException exception ) {
                            exception.Error();
                        }
                        catch ( QuotaExceededException exception ) {
                            exception.Error();
                        }
                        catch ( ArgumentNullException exception ) {
                            exception.Error();
                        }
                        catch ( ArgumentException exception ) {
                            exception.Error();
                        }
                    }
                }
            }
            catch ( IsolatedStorageException exception ) {
                exception.Error();
            }
            catch ( SecurityException exception ) {
                exception.Error();
            }
            catch ( Exception exception ) {
                exception.Error();
            }
            return false;
        }

        /// <summary>
        ///     Persist an object to an IsolatedStorageFile.<br />
        ///     Mark class with [DataContract( Namespace = "http://aibrain.org" )]<br />
        ///     Mark fields with [DataMember, OptionalField] to serialize (both public and private).<br />
        ///     Fields cannot have JUST the Getter or the Setter, has to have both.<br />
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="obj"></param>
        /// <param name="fileName"></param>
        /// <returns>Returns True if the object was saved.</returns>
        public static Boolean SaveValue<TSource>( this TSource obj, [NotNull] String fileName ) where TSource : struct {
            if ( fileName == null ) {
                throw new ArgumentNullException( "fileName" );
            }
            try {
                if ( String.IsNullOrEmpty( fileName ) ) {
                    return false;
                }
                try {
                    using ( var isf = IsolatedStorageFile.GetMachineStoreForDomain() ) {
                        String dir;
                        try {
                            dir = Path.GetDirectoryName( fileName );
                        }
                        catch ( PathTooLongException exception ) {
                            exception.Error();
                            return false;
                        }
                        catch ( ArgumentException exception ) {
                            exception.Error();
                            return false;
                        }
                        if ( !String.IsNullOrEmpty( dir ) && !isf.DirectoryExists( dir ) ) {
                            try {
                                isf.CreateDirectory( dir );
                            }
                            catch ( IsolatedStorageException exception ) {
                                exception.Error();
                                return false;
                            }
                        }

                        try {
                            using ( var isfs = new IsolatedStorageFileStream( fileName, FileMode.Create, FileAccess.Write, isf ) ) {
                                //var serializer = new DataContractSerializer( typeof ( T ) );
                                var serializer = new NetDataContractSerializer();

                                var ext = Path.GetExtension( path: fileName );
                                var useCompression = ext.EndsWith( value: "Z", ignoreCase: true, culture: null );

                                if ( useCompression ) {
                                    using ( var compress = new GZipStream( isfs, CompressionMode.Compress, leaveOpen: true ) ) {
                                        serializer.WriteObject( compress, obj );
                                    }
                                }
                                else {
                                    serializer.WriteObject( isfs, obj );
                                }
                                isfs.Close();
                                return true;
                            }
                        }
                        catch ( InvalidDataContractException exception ) {
                            exception.Error();
                        }
                        catch ( SerializationException exception ) {
                            exception.Error();
                        }
                        catch ( QuotaExceededException exception ) {
                            exception.Error();
                        }
                        catch ( ArgumentNullException exception ) {
                            exception.Error();
                        }
                        catch ( ArgumentException exception ) {
                            exception.Error();
                        }
                    }
                }
                catch ( IsolatedStorageException exception ) {
                    exception.Error();
                }
            }
            catch ( SecurityException exception ) {
                exception.Error();
            }
            catch ( Exception exception ) {
                exception.Error();
            }
            return false;
        }

        /// <summary>
        ///     Deserialize from an IsolatedStorageFile.
        /// </summary>
        /// <param name="obj" />
        /// <param name="fileName" />
        /// <param name="feedback"></param>
        /// <returns></returns>
        [Obsolete]
        public static Boolean Load<TSource>( out TSource obj, [NotNull] String fileName, ProgressChangedEventHandler feedback = null ) where TSource : class {
            if ( fileName == null ) {
                throw new ArgumentNullException( "fileName" );
            }
            obj = default( TSource );
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
                            exception.Error();
                            return false;
                        }

                        try {
                            if ( deletefile ) {
                                isolatedStorageFile.DeleteFile( fileName );
                                return false;
                            }
                        }
                        catch ( IsolatedStorageException exception ) {
                            exception.Error();
                            return false;
                        }

                        try {
                            using ( var fileStream = new IsolatedStorageFileStream( path: fileName, mode: FileMode.Open, access: FileAccess.Read, isf: isolatedStorageFile ) ) {
                                var ext = Path.GetExtension( path: fileName );
                                var useCompression = ext.EndsWith( value: "Z", ignoreCase: true, culture: null );

                                if ( useCompression ) {
                                    using ( var decompress = new GZipStream( stream: fileStream, mode: CompressionMode.Decompress, leaveOpen: true ) ) {
                                        obj = Deserialize<TSource>( stream: decompress, feedback: feedback );
                                    }
                                }
                                else {
                                    //obj = Deserialize<TSource>( stream: isfs, feedback: feedback );
                                    var serializer = new NetDataContractSerializer();
                                    obj = serializer.Deserialize( fileStream ) as TSource;
                                }
                                fileStream.Close();
                                return obj != default( TSource );
                            }
                        }
                        catch ( InvalidOperationException exception ) {
                            exception.Error();
                        }
                        catch ( ArgumentNullException exception ) {
                            exception.Error();
                        }
                        catch ( SerializationException exception ) {
                            deletefile = true;
                            exception.Error();
                        }
                        catch ( Exception exception ) {
                            exception.Error();
                        }

                        try {
                            if ( deletefile ) {
                                isolatedStorageFile.DeleteFile( fileName );
                                return false;
                            }
                        }
                        catch ( IsolatedStorageException exception ) {
                            exception.Error();
                            return false;
                        }
                    }
                }
            }
            catch ( IsolatedStorageException exception ) {
                exception.Error();
            }
            return false;
        }

        public static TSource Deserialize<TSource>( Stream stream, ProgressChangedEventHandler feedback = null ) where TSource : class {
            if ( null == stream ) {
                throw new ArgumentNullException( "stream" );
            }

            using ( var cs = new ProgressStream( stream ) ) {
                if ( feedback != null ) {
                    cs.ProgressChanged += feedback;
                }

                using ( var bs = new BufferedStream( stream: cs, bufferSize: 16384 ) ) {
                    var formatter = new NetDataContractSerializer();
                    return formatter.Deserialize( bs ) as TSource;
                }
            }
        }

        /// <summary>
        ///     Deserialize from an IsolatedStorageFile.
        /// </summary>
        /// <param name="fullPathAndFileName" />
        /// <param name="onLoad"> </param>
        /// <param name="feedback"></param>
        /// <returns></returns>
        public static Boolean Loader<TSource>( [NotNull] this String fullPathAndFileName, [CanBeNull] Action<TSource> onLoad = null, ProgressChangedEventHandler feedback = null ) where TSource : class {
            if ( fullPathAndFileName == null ) {
                throw new ArgumentNullException( "fullPathAndFileName" );
            }
            try {
                if ( IsolatedStorageFile.IsEnabled && !String.IsNullOrWhiteSpace( fullPathAndFileName ) ) {
                    using ( var snag = new FileSingleton( "IsolatedStorageFile.GetMachineStoreForDomain()" ) ) {
                        using ( var isf = IsolatedStorageFile.GetMachineStoreForDomain() ) {
                            snag.Dispose();
                            var dir = Path.GetDirectoryName( fullPathAndFileName ) ?? String.Empty;

                            if ( !String.IsNullOrWhiteSpace( dir ) && !isf.DirectoryExists( dir ) ) {
                                isf.CreateDirectory( dir );
                            }

                            if ( isf.FileExists( fullPathAndFileName ) && FileCanBeRead( isf, fullPathAndFileName ) ) {
                                try {
                                    using ( var isfs = new IsolatedStorageFileStream( path: fullPathAndFileName, mode: FileMode.Open, access: FileAccess.Read, isf: isf ) ) {
                                        var ext = Path.GetExtension( path: fullPathAndFileName );
                                        var useCompression = !String.IsNullOrWhiteSpace( ext ) && ext.EndsWith( value: "Z", ignoreCase: true, culture: null );

                                        TSource result;
                                        if ( useCompression ) {
                                            using ( var decompress = new GZipStream( stream: isfs, mode: CompressionMode.Decompress, leaveOpen: true ) ) {
                                                result = Deserialize<TSource>( stream: decompress, feedback: feedback );
                                            }
                                        }
                                        else {
                                            result = Deserialize<TSource>( stream: isfs, feedback: feedback );
                                        }
                                        if ( onLoad != null ) {
                                            onLoad( result );
                                        }
                                        return true;
                                    }
                                }
                                catch ( InvalidOperationException exception ) {
                                    exception.Error();
                                }
                                catch ( ArgumentNullException exception ) {
                                    exception.Error();
                                }
                                catch ( SerializationException exception ) {
                                    exception.Error();
                                }
                                catch ( Exception exception ) {
                                    exception.Error();
                                }
                            }
                        }
                    }
                }
            }
            catch ( IsolatedStorageException exception ) {
                exception.Error();
            }
            return false;
        }

        /// <summary>
        ///     Can the file be read from at this moment in time ?
        /// </summary>
        /// <param name="isf"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        private static Boolean FileCanBeRead( IsolatedStorageFile isf, String fileName ) {
            try {
                using ( var stream = isf.OpenFile( path: fileName, mode: FileMode.Open, access: FileAccess.Read, share: FileShare.Read ) ) {
                    try {
                        return stream.Seek( offset: 0, origin: SeekOrigin.End ) > 0;
                    }
                    catch ( ArgumentException exception ) {
                        exception.Error();
                    }
                }
            }
            catch ( IsolatedStorageException exception ) {
                exception.Error();
            }
            catch ( ArgumentNullException exception ) {
                exception.Error();
            }
            catch ( ArgumentException exception ) {
                exception.Error();
            }
            catch ( DirectoryNotFoundException exception ) {
                exception.Error();
            }
            catch ( FileNotFoundException exception ) {
                exception.Error();
            }
            catch ( ObjectDisposedException exception ) {
                exception.Error();
            }
            return false;
        }

        /// <summary>
        ///     Deserialize from an IsolatedStorageFile.
        /// </summary>
        /// <param name="fileName" />
        /// <param name="feedback"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        [Obsolete]
        public static TSource LoadOrCreate<TSource>( [NotNull] String fileName, ProgressChangedEventHandler feedback = null, [NotNull] params Object[] parameters ) where TSource : class, new() {
            if ( fileName == null ) {
                throw new ArgumentNullException( "fileName" );
            }
            if ( parameters == null ) {
                throw new ArgumentNullException( "parameters" );
            }
            try {
                if ( IsolatedStorageFile.IsEnabled && !String.IsNullOrWhiteSpace( fileName ) ) {
                    using ( var isf = IsolatedStorageFile.GetMachineStoreForDomain() ) {
                        var dir = Path.GetDirectoryName( fileName );

                        if ( !String.IsNullOrWhiteSpace( dir ) && !isf.DirectoryExists( dir ) ) {
                            isf.CreateDirectory( dir );
                        }

                        if ( isf.FileExists( fileName ) && FileCanBeRead( isf, fileName ) ) {
                            try {
                                using ( var isfs = new IsolatedStorageFileStream( path: fileName, mode: FileMode.Open, access: FileAccess.Read, isf: isf ) ) {
                                    var ext = Path.GetExtension( path: fileName );
                                    var useCompression = !String.IsNullOrWhiteSpace( ext ) && ext.EndsWith( value: "Z", ignoreCase: true, culture: null );

                                    if ( !useCompression ) {
                                        return Deserialize<TSource>( stream: isfs, feedback: feedback );
                                    }
                                    using ( var decompress = new GZipStream( stream: isfs, mode: CompressionMode.Decompress, leaveOpen: true ) ) {
                                        return Deserialize<TSource>( stream: decompress, feedback: feedback );
                                    }
                                }
                            }
                            catch ( InvalidOperationException exception ) {
                                exception.Error();
                            }
                            catch ( ArgumentNullException exception ) {
                                exception.Error();
                            }
                            catch ( SerializationException exception ) {
                                exception.Error();
                            }
                            catch ( Exception exception ) {
                                exception.Error();
                            }
                        }
                    }
                }
            }
            catch ( IsolatedStorageException exception ) {
                exception.Error();
            }
            return new TSource();
        }

        /// <summary>
        ///     <para>Attempts to deserialize an NTFS alternate stream with the <paramref name="attribute" /> to the file <paramref name="location" />.</para>
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="attribute"></param>
        /// <param name="value"></param>
        /// <param name="location"></param>
        /// <returns></returns>
        public static Boolean TryGet<TSource>( this String attribute, out TSource value, String location = null ) {
            if ( attribute == null ) {
                throw new ArgumentNullException( "attribute" );
            }

            value = default( TSource );

            try {
                if ( location.IsNullOrWhiteSpace() ) {
                    location = DataFolder.Value.FullName;
                }
                var filename = String.Format( "{0}:{1}", location, attribute );

                if ( !NtfsAlternateStream.Exists( filename ) ) {
                    return false;
                }
                using ( var fs = NtfsAlternateStream.Open( path: filename, access: FileAccess.Read, mode: FileMode.Open, share: FileShare.None ) ) {
                    var serializer = new NetDataContractSerializer();
                    value = ( TSource )serializer.Deserialize( fs );
                }
                return true;
            }
            catch ( InvalidOperationException exception ) {
                exception.Error();
            }
            catch ( ArgumentNullException exception ) {
                exception.Error();
            }
            catch ( SerializationException exception ) {
                exception.Error();
            }
            catch ( Exception exception ) {
                exception.Error();
            }
            return false;
        }

        /// <summary>
        ///     Attempts to serialize this object to an NTFS alternate stream with the index of
        ///     <paramref name="attribute" />.
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="objectToSerialize"></param>
        /// <param name="attribute"></param>
        /// <param name="location"></param>
        /// <returns></returns>
        public static Boolean TrySave<TSource>( this TSource objectToSerialize, [NotNull] String attribute, String location = null ) {
            if ( attribute == null ) {
                throw new ArgumentNullException( "attribute" );
            }
            try {
                if ( String.IsNullOrWhiteSpace( location ) ) {
                    location = DataFolder.Value.FullName;
                }
                var filename = String.Format( "{0}:{1}", location, attribute );
                var context = new StreamingContext( StreamingContextStates.All );

                using ( var fs = NtfsAlternateStream.Open( path: filename, access: FileAccess.Write, mode: FileMode.Create, share: FileShare.None ) ) {
                    var serializer = new NetDataContractSerializer( context: context, maxItemsInObjectGraph: Int32.MaxValue, ignoreExtensionDataObject: false, assemblyFormat: FormatterAssemblyStyle.Simple, surrogateSelector: null );
                    serializer.Serialize( fs, objectToSerialize );
                }
                return true;
            }
            catch ( SerializationException exception ) {
                exception.Error();
            }
            catch ( Exception exception ) {
                exception.Error();
            }
            return false;
        }

        public static Boolean FileCannotBeRead( this IsolatedStorageFile isf, String fileName ) {
            return !FileCanBeRead( isf: isf, fileName: fileName );
        }

        public static Boolean DeserializeDictionary<TKey, TValue>( this ConcurrentDictionary<TKey, TValue> toDictionary, Folder folder, [CanBeNull] IProgress<Single> progress = null, String extension = ".xml" )
        where TKey : IComparable<TKey> {

            try {
                Report.Enter();
                var stopwatch = Stopwatch.StartNew();

                if ( null == toDictionary ) {
                    return false;
                }
                if ( null == folder ) {
                    return false;
                }
                if ( !folder.Exists() ) {
                    return false;
                }

                var fileCount = UInt64.MinValue ;
                var before = toDictionary.LongCount();

                //enumerate all the files with the wildcard *.extension
                var documents = folder.GetDocuments( String.Format( "*{0}", extension ), SearchOption.TopDirectoryOnly );

                foreach ( var document in documents ) {
                    try {
                        fileCount ++;
                        var lines = File.ReadLines( document.FullPathWithFileName ).AsParallel();
                        lines.ForAll( line => {
                            try {
                                var tuple = line.Deserialize<Tuple<TKey, TValue>>();
                                if ( tuple != null ) {
                                    toDictionary[ tuple.Item1 ] = tuple.Item2;
                                }
                            }
                            catch ( Exception lineexception ) {
                                lineexception.Error();
                            }
                        } );
                    }
                    catch ( Exception exception ) {
                        exception.Error();
                    }
                }

                var after = toDictionary.LongCount();

                stopwatch.Stop();
                Report.Info( String.Format( "Deserialized {0} items from {1} files in {2}.", after - before, fileCount, stopwatch.Elapsed.Simpler() ) );

                return true;
            }
            catch ( Exception exception ) {
                exception.Error();
            }
            finally {
                Report.Exit();
            }
            return false;
        }

        /// <summary>
        /// <para>Persist the <paramref name="dictionary"/> into <paramref name="folder"/>.</para>
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="dictionary"></param>
        /// <param name="folder"></param>
        /// <param name="progress"></param>
        /// <param name="extension"></param>
        /// <returns></returns>
        public static Boolean SerializeDictionary<TKey, TValue>( [CanBeNull] this ConcurrentDictionary<TKey, TValue> dictionary, [CanBeNull] Folder folder, [CanBeNull] IProgress<Single> progress = null, String extension = ".xml" )
       where TKey : IComparable<TKey> {

            if ( null == dictionary ) {
                return false;
            }
            if ( null == folder ) {
                return false;
            }
            if ( !dictionary.Any() ) {
                return false;
            }

            try {

                Report.Enter();
                var stopwatch = Stopwatch.StartNew();

                if ( !folder.Exists() ) {
                    folder.Create();
                }

                if ( !folder.Exists() ) {
                    throw new DirectoryNotFoundException( folder.FullName );
                }

                var itemCount = ( UInt64 )dictionary.LongCount();

                Report.Info( String.Format( "Serializing {1} items to {0} ...", folder.FullName, itemCount ) );

                var currentLine = 0f;

                var backThen = DateTime.UtcNow.ToGuid();

                var fileName = String.Format( "{0}{1}", backThen, extension );     //let the time change the file name over time

                var document = new Document( folder, fileName );

                var writer = File.AppendText( document.FullPathWithFileName );

                var fileCount = UInt64.MinValue + 1;

                foreach ( var pair in dictionary ) {

                    currentLine++;

                    var tuple = new Tuple<TKey, TValue>( pair.Key, pair.Value );  //convert the struct to a class

                    var data = tuple.Serialize();

                    var hereNow = DateTime.UtcNow.ToGuid();

                    if ( backThen != hereNow ) {

                        if ( progress != null ) {
                            var soFar = currentLine / itemCount;
                            progress.Report( soFar );
                        }

                        using ( writer ) {
                            writer.Flush();
                            writer.Close();
                        }
                        fileName = String.Format( "{0}.xml", hereNow );     //let the file name change over time so we don't have bigHuge monolithic files.
                        document = new Document( folder, fileName );
                        writer = File.AppendText( document.FullPathWithFileName );
                        fileCount++;
                        backThen = DateTime.UtcNow.ToGuid();
                    }

                    writer.WriteLine( data );
                }

                using ( writer ) {
                    writer.Flush();
                    writer.Close();
                }

                stopwatch.Stop();
                Report.Info( String.Format( "Serialized {1} items in {0} into {2} files.", stopwatch.Elapsed.Simpler(), itemCount, fileCount ) );

                return true;
            }
            catch ( Exception exception ) {
                exception.Error();
            }
            finally {
                Report.Exit();
            }
            return false;
        }


    }
}