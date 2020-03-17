// Copyright © 2020 Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible in any binaries, libraries, repositories, and source code (directly or derived)
// from our binaries, libraries, projects, or solutions.
//
// This source code contained in "PersistenceExtensions.cs" belongs to Protiguous@Protiguous.com unless otherwise specified or the original license has been overwritten
// by formatting. (We try to avoid it from happening, but it does accidentally happen.)
//
// Any unmodified portions of source code gleaned from other projects still retain their original license and our thanks goes to those Authors.
// If you find your code in this source code, please let us know so we can properly attribute you and include the proper license and/or copyright.
//
// If you want to use any of our code in a commercial project, you must contact Protiguous@Protiguous.com for permission and a quote.
//
// Donations are accepted via bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2 and PayPal: Protiguous@Protiguous.com
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
// For business inquiries, please contact me at Protiguous@Protiguous.com.
//
// Our website can be found at "https://Protiguous.com/"
// Our software can be found at "https://Protiguous.Software/"
// Our GitHub address is "https://github.com/Protiguous".
// Feel free to browse any source code we make available.
//
// Project: "Librainian", File: "PersistenceExtensions.cs" was last formatted by Protiguous on 2020/03/16 at 3:01 PM.

namespace Librainian.Persistence {

    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Configuration;
    using System.Diagnostics;
    using System.IO;
    using System.IO.Compression;
    using System.IO.IsolatedStorage;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.Serialization;
    using System.Runtime.Serialization.Formatters;
    using System.Runtime.Serialization.Formatters.Binary;
    using System.Security;
    using System.ServiceModel;
    using System.Text;
    using System.Threading;
    using System.Windows.Forms;
    using Collections.Lists;
    using Converters;
    using JetBrains.Annotations;
    using Logging;
    using Measurement.Time;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Serialization;
    using OperatingSystem.FileSystem;
    using OperatingSystem.Streams;
    using Parsing;
    using Threading;
    using Directory = OperatingSystem.FileSystem.Pri.LongPath.Directory;
    using DirectoryInfo = OperatingSystem.FileSystem.Pri.LongPath.DirectoryInfo;
    using File = OperatingSystem.FileSystem.Pri.LongPath.File;

    // ReSharper disable RedundantUsingDirective
    using Path = OperatingSystem.FileSystem.Pri.LongPath.Path;

    // ReSharper restore RedundantUsingDirective

    public static class PersistenceExtensions {

        public static readonly Lazy<Document> DataDocument = new Lazy<Document>( () => {
            var document = new Document( LocalDataFolder.Value, Application.ExecutablePath + ".data" );

            if ( !document.Exists() ) {

                // ReSharper disable once ReturnValueOfPureMethodIsNotUsed
                document.AppendText( String.Empty );
            }

            return document;
        } );

        /// <summary>
        ///     <para><see cref="Folder" /> to store application data.</para>
        ///     <para>
        ///         <see cref="Environment.SpecialFolder.LocalApplicationData" />
        ///     </para>
        /// </summary>
        public static readonly Lazy<Folder> LocalDataFolder = new Lazy<Folder>( () => {
            var folder = new Folder( Environment.SpecialFolder.LocalApplicationData );

            if ( !folder.Exists() ) {
                folder.Create();
            }

            return folder;
        } );

        [NotNull]
        public static readonly ThreadLocal<JsonSerializer> LocalJsonSerializers = new ThreadLocal<JsonSerializer>( () => new JsonSerializer {
            ReferenceLoopHandling = ReferenceLoopHandling.Serialize,
            PreserveReferencesHandling = PreserveReferencesHandling.All
        }, true );

        ///// <summary>
        /////   Attempts to Add() the specified filename into the collection.
        ///// </summary>
        ///// <typeparam name = "T"></typeparam>
        ///// <param name = "collection"></param>
        ///// <param name = "fileName"></param>
        ///// <returns></returns>
        //public static Boolean LoadCollection< T >( this IProducerConsumerCollection< T > collection, String fileName ) {
        //    if ( collection is null ) {
        //        throw new ArgumentNullException( "collection" );
        //    }
        //    if ( fileName is null ) {
        //        throw new ArgumentNullException( "fileName" );
        //    }
        //    IProducerConsumerCollection< T > temp;
        //    if ( Storage.Load( out temp, fileName ) ) {
        //        if ( null != temp ) {
        //            var result = Parallel.ForEach( temp, collection.Add );
        //            return result.IsCompleted;
        //        }
        //    }
        //    return default;
        //}
        [NotNull]
        public static readonly ThreadLocal<NetDataContractSerializer> Serializers = new ThreadLocal<NetDataContractSerializer>(
            () => new NetDataContractSerializer( StreamingContexts.Value, Int32.MaxValue, false, FormatterAssemblyStyle.Simple, null ), true );

        public static readonly ThreadLocal<StreamingContext> StreamingContexts =
            new ThreadLocal<StreamingContext>( () => new StreamingContext( StreamingContextStates.All ), true );

        [NotNull]
        public static ThreadLocal<JsonSerializerSettings> Jss { get; } = new ThreadLocal<JsonSerializerSettings>( () => new JsonSerializerSettings {

            //ContractResolver = new MyContractResolver(),
            TypeNameHandling = TypeNameHandling.Auto,
            NullValueHandling = NullValueHandling.Ignore,
            DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate,
            ReferenceLoopHandling = ReferenceLoopHandling.Serialize,
            PreserveReferencesHandling = PreserveReferencesHandling.Objects //All?
        }, true );

        [NotNull]
        private static Document GetStaticFile( this Environment.SpecialFolder specialFolder ) {
            var path = Path.Combine( Environment.GetFolderPath( specialFolder ), nameof( Settings ) );

            if ( !Directory.Exists( path ) ) {
                Directory.CreateDirectory( path );
            }

            var destinationFile = Path.Combine( path, "StaticSettings.exe" );

            if ( !File.Exists( destinationFile ) ) {
                using ( File.Create( destinationFile ) ) { }
            }

            return new Document( destinationFile );
        }

        /// <summary></summary>
        /// <typeparam name="TType"></typeparam>
        /// <param name="storedAsString"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="EncoderFallbackException"></exception>
        /// <exception cref="FormatException"></exception>
        /// <exception cref="System.Xml.XmlException"></exception>
        [CanBeNull]
        public static TType Deserialize<TType>( [CanBeNull] this String? storedAsString ) {
            try {
                return storedAsString.FromJSON<TType>();
            }
            catch ( SerializationException exception ) {
                exception.Log();
            }

            return default;
        }

        [CanBeNull]
        public static TSource Deserialize<TSource>( [NotNull] Stream stream, [CanBeNull] ProgressChangedEventHandler feedback = null ) where TSource : class {
            if ( null == stream ) {
                throw new ArgumentNullException( nameof( stream ) );
            }

            using ( var cs = new ProgressStream( stream ) ) {

                cs.ProgressChanged += feedback;

                using ( var bs = new BufferedStream( cs, 16384 ) ) {
                    var formatter = new NetDataContractSerializer();

                    return formatter.Deserialize( bs ) as TSource;
                }
            }
        }

        public static Boolean DeserializeDictionary<TKey, TValue>( [CanBeNull] this ConcurrentDictionary<TKey, TValue> toDictionary, [CanBeNull] Folder folder,
            [CanBeNull] String? calledWhat, [CanBeNull] String? extension = ".xml" ) where TKey : IComparable<TKey> {
            try {

                //Report.Enter();
                var stopwatch = Stopwatch.StartNew();

                if ( null == toDictionary ) {
                    return default;
                }

                if ( folder?.Exists() != true ) {
                    return default;
                }

                var fileCount = UInt64.MinValue;
                var before = toDictionary.LongCount();

                //enumerate all the files with the wildcard *.extension
                var documents = folder.GetDocuments( $"*{extension}" );

                foreach ( var document in documents ) {
                    var length = document.Length;

                    if ( length < 1 ) {
                        document.Delete();

                        continue;
                    }

                    try {
                        fileCount++;
                        var lines = File.ReadLines( document.FullPath ).AsParallel();

                        lines.ForAll( line => {
                            try {
                                var tuple = line.Deserialize<Tuple<TKey, TValue>>();

                                if ( tuple != null ) {
                                    toDictionary[ tuple.Item1 ] = tuple.Item2;
                                }
                            }
                            catch ( Exception lineexception ) {
                                lineexception.Log();
                            }
                        } );
                    }
                    catch ( Exception exception ) {
                        exception.Log();
                    }
                }

                var after = toDictionary.LongCount();

                stopwatch.Stop();
                String.Format( "Deserialized {0} {3} from {1} files in {2}.", after - before, fileCount, stopwatch.Elapsed.Simpler(), calledWhat ).Info();

                return true;
            }
            catch ( Exception exception ) {
                exception.Log();
            }

            //finally {
            //    //Report.Exit();
            //}
            return default;
        }

        /// <summary>See also <see cref="Serializer{T}" />.</summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="bytes"></param>
        /// <returns></returns>
        [CanBeNull]
        public static T Deserializer<T>( [CanBeNull] this Byte[] bytes ) {
            if ( bytes == default ) {
                return default;
            }

            using ( var memoryStream = new MemoryStream( bytes ) ) {
                var binaryFormatter = new BinaryFormatter();

                return ( T )binaryFormatter.Deserialize( memoryStream );
            }
        }

        [Obsolete( "Will this ever be used in the future?" )]
        public static Boolean EnableIsolatedStorageCompression() {
            using ( var isf = IsolatedStorageFile.GetMachineStoreForDomain() ) {
                var myType = isf.GetType();
                var myFields = myType.GetFields( BindingFlags.Instance | BindingFlags.NonPublic );

                var myField = myFields.FirstOrDefault( f => f.Name.Like( "m_RootDir" ) );

                var path = myField?.GetValue( isf ) as String;

                if ( String.IsNullOrWhiteSpace( path ) ) {
                    return default;
                }

                try {
                    var dir = new DirectoryInfo( path );

                    if ( dir.Exists ) {
                        var result = dir.SetCompression( true );

                        if ( result ) {
                            $"Enabled compression in IsolatedStorage @ {path}".Info();
                        }

                        return result;
                    }
                }
                catch ( SecurityException exception ) {
                    exception.Log();
                }
                catch ( ArgumentException exception ) {
                    exception.Log();
                }
                catch ( PathTooLongException exception ) {
                    exception.Log();
                }
            }

            return default;
        }

        /// <summary>Can the file be read from at this moment in time ?</summary>
        /// <param name="isf">     </param>
        /// <param name="document"></param>
        /// <returns></returns>
        public static Boolean FileCanBeRead( [NotNull] this IsolatedStorageFile isf, [NotNull] Document document ) {
            if ( isf is null ) {
                throw new ArgumentNullException( nameof( isf ) );
            }

            if ( document is null ) {
                throw new ArgumentNullException( nameof( document ) );
            }

            try {
                using ( var stream = isf.OpenFile( document.FileName, FileMode.Open, FileAccess.Read, FileShare.Read ) ) {
                    try {
                        return stream.Seek( 0, SeekOrigin.End ) > 0;
                    }
                    catch ( ArgumentException exception ) {
                        exception.Log();
                    }
                }
            }
            catch ( IsolatedStorageException exception ) {
                exception.Log();
            }
            catch ( ArgumentNullException exception ) {
                exception.Log();
            }
            catch ( ArgumentException exception ) {
                exception.Log();
            }
            catch ( DirectoryNotFoundException exception ) {
                exception.Log();
            }
            catch ( FileNotFoundException exception ) {
                exception.Log();
            }
            catch ( ObjectDisposedException exception ) {
                exception.Log();
            }

            return default;
        }

        public static Boolean FileCannotBeRead( [NotNull] this IsolatedStorageFile isf, [NotNull] Document document ) {
            if ( isf is null ) {
                throw new ArgumentNullException( nameof( isf ) );
            }

            if ( document is null ) {
                throw new ArgumentNullException( nameof( document ) );
            }

            return !isf.FileCanBeRead( document );
        }

        /// <summary>Return this JSON string as an object.</summary>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="data"></param>
        /// <returns></returns>
        [CanBeNull]
        public static TKey FromJSON<TKey>( [CanBeNull] this String data ) =>
            String.IsNullOrWhiteSpace( data ) ? default : JsonConvert.DeserializeObject<TKey>( data, Jss.Value );

        /// <summary>Deserialize from an IsolatedStorageFile.</summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="fileName"></param>
        /// <returns></returns>
        [CanBeNull]
        [Obsolete( "Use something else. I don't care anymore." )]
        public static T Load<T>( [CanBeNull] String? fileName ) where T : class, new() {
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
                            var serializer = new NetDataContractSerializer();
                            var obj = serializer.ReadObject( isfs ) as T;

                            return obj;
                        }
                    }
                    catch ( SerializationException exception ) {
                        exception.Log();
                    }
                    catch ( IsolatedStorageException exception ) {
                        exception.Log();
                    }
                }
            }
            catch ( Exception exception ) {
                exception.Log();
            }

            return new T();
        }

        //[Obsolete( "Use JSON methods" )]
        public static Boolean Loader<TSource>( [NotNull] this Document document, [CanBeNull] Action<TSource> onLoad = null,
            [CanBeNull] ProgressChangedEventHandler feedback = null ) where TSource : class {
            if ( document is null ) {
                throw new ArgumentNullException( nameof( document ) );
            }

            try {
                if ( IsolatedStorageFile.IsEnabled ) {
                    using ( var _ = new SingleAccess( "IsolatedStorageFile.GetMachineStoreForDomain()" ) ) {
                        using ( var isf = IsolatedStorageFile.GetMachineStoreForDomain() ) {
                            var dir = Path.GetDirectoryName( document.FullPath );

                            if ( !String.IsNullOrWhiteSpace( dir ) && !isf.DirectoryExists( dir ) ) {
                                isf.CreateDirectory( dir );
                            }

                            if ( isf.FileCanBeRead( document ) ) {
                                try {
                                    var isfs = new IsolatedStorageFileStream( document.FullPath, FileMode.Open, FileAccess.Read, isf );
                                    var ext = document.Extension();
                                    var useCompression = !String.IsNullOrWhiteSpace( ext ) && ext.EndsWith( "Z", true, null );

                                    TSource result;

                                    if ( useCompression ) {
                                        using ( var decompress = new GZipStream( isfs, CompressionMode.Decompress, true ) ) {
                                            result = Deserialize<TSource>( decompress, feedback );
                                        }
                                    }
                                    else {
                                        result = Deserialize<TSource>( isfs, feedback );
                                    }

                                    onLoad?.Invoke( result );

                                    return true;
                                }
                                catch ( InvalidOperationException exception ) {
                                    exception.Log();
                                }
                                catch ( ArgumentNullException exception ) {
                                    exception.Log();
                                }
                                catch ( SerializationException exception ) {
                                    exception.Log();
                                }
                                catch ( Exception exception ) {
                                    exception.Log();
                                }
                            }
                        }
                    }
                }
            }
            catch ( IsolatedStorageException exception ) {
                exception.Log();
            }

            return default;
        }

        /// <summary>Deserialize from an IsolatedStorageFile.</summary>
        /// <param name="feedback">            </param>
        /// <returns></returns>
        /// <summary>Deserialize from an IsolatedStorageFile.</summary>
        /// <param name="document" />
        /// <param name="parameters"></param>
        /// <returns></returns>
        [CanBeNull]
        [Obsolete( "Use JSON serializers" )]
        public static TSource LoadOrCreate<TSource>( [NotNull] Document document, [CanBeNull] ProgressChangedEventHandler feedback = null,
            [NotNull] params Object[] parameters ) where TSource : class, new() {
            if ( document is null ) {
                throw new ArgumentNullException( nameof( document ) );
            }

            if ( parameters is null ) {
                throw new ArgumentNullException( nameof( parameters ) );
            }

            try {
                if ( IsolatedStorageFile.IsEnabled ) {
                    using ( var isf = IsolatedStorageFile.GetMachineStoreForDomain() ) {
                        var dir = Path.GetDirectoryName( document.FullPath );

                        if ( !String.IsNullOrWhiteSpace( dir ) && !isf.DirectoryExists( dir ) ) {
                            isf.CreateDirectory( dir );
                        }

                        if ( isf.FileCanBeRead( document ) ) {
                            try {
                                var isfs = new IsolatedStorageFileStream( document.FullPath, FileMode.Open, FileAccess.Read, isf );
                                var ext = document.Extension();
                                var useCompression = !String.IsNullOrWhiteSpace( ext ) && ext.EndsWith( "Z", true, null );

                                if ( !useCompression ) {
                                    return Deserialize<TSource>( isfs, feedback );
                                }

                                using ( var decompress = new GZipStream( isfs, CompressionMode.Decompress, true ) ) {
                                    return Deserialize<TSource>( decompress, feedback );
                                }
                            }
                            catch ( InvalidOperationException exception ) {
                                exception.Log();
                            }
                            catch ( ArgumentNullException exception ) {
                                exception.Log();
                            }
                            catch ( SerializationException exception ) {
                                exception.Log();
                            }
                            catch ( Exception exception ) {
                                exception.Log();
                            }
                        }
                    }
                }
            }
            catch ( IsolatedStorageException exception ) {
                exception.Log();
            }

            return new TSource();
        }

        /// <summary>Deserialize from an IsolatedStorageFile.</summary>
        /// <param name="obj" />
        /// <param name="fileName" />
        /// <returns></returns>
        [Obsolete( "Use JSON serializers" )]
        public static Boolean LoadValue<T>( out T obj, [CanBeNull] String? fileName ) where T : struct {
            obj = default;

            try {
                if ( String.IsNullOrEmpty( fileName ) ) {
                    return default;
                }

                using ( var isf = IsolatedStorageFile.GetMachineStoreForDomain() ) {
                    var dir = Path.GetDirectoryName( fileName );

                    if ( !String.IsNullOrWhiteSpace( dir ) && !isf.DirectoryExists( dir ) ) {
                        isf.CreateDirectory( dir );
                    }

                    if ( !isf.FileExists( fileName ) ) {
                        return default;
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
                        exception.Log();

                        return default;
                    }

                    try {
                        if ( deletefile ) {
                            isf.DeleteFile( fileName );

                            return default;
                        }
                    }
                    catch ( IsolatedStorageException exception ) {
                        exception.Log();

                        return default;
                    }

                    try {
                        var isfs = new IsolatedStorageFileStream( fileName, FileMode.Open, FileAccess.Read, isf );

                        //var serializer = new DataContractSerializer( typeof ( T ) );
                        var serializer = new NetDataContractSerializer();

                        var ext = Path.GetExtension( fileName );
                        var useCompression = ext.EndsWith( "Z", true, null );

                        if ( useCompression ) {
                            using ( var decompress = new GZipStream( isfs, CompressionMode.Decompress, true ) ) {
                                obj = ( T )serializer.ReadObject( decompress );
                            }
                        }
                        else {
                            obj = ( T )serializer.ReadObject( isfs );
                        }

                        return !Equals( obj, default );
                    }
                    catch ( InvalidOperationException exception ) {
                        exception.Log();

                        return default;
                    }
                    catch ( ArgumentNullException exception ) {
                        exception.Log();

                        return default;
                    }
                    catch ( SerializationException exception ) {
                        exception.Log();

                        return default;
                    }
                    catch ( Exception exception ) {
                        exception.Log();

                        return default;
                    }
                }
            }
            catch ( IsolatedStorageException exception ) {
                exception.Log();
            }

            return default;
        }

        /// <summary>
        /// Persist an object to an IsolatedStorageFile. <br /> Mark class with [DataContract( Namespace = "http://Protiguous.com" )] <br /> Mark fields with [DataMember,
        /// OptionalField] to serialize (both public and private). <br /> Properties have to have both the Getter and the Setter. <br />
        /// </summary>
        /// <param name="collection"></param>
        /// <param name="fileName">  </param>
        /// <returns>Returns True if the object was saved.</returns>
        [Obsolete( "Not in use yet." )]
        public static Boolean SaveCollection<T>( [NotNull] this IProducerConsumerCollection<T> collection, [NotNull] String fileName ) {
            if ( collection is null ) {
                throw new ArgumentNullException( nameof( collection ) );
            }

            if ( String.IsNullOrWhiteSpace( fileName ) ) {
                throw new ArgumentNullException( nameof( fileName ) );
            }

            return collection.Saver( fileName );
        }

        /// <summary>
        /// Persist an object to an IsolatedStorageFile. <br /> Mark class with [DataContract( Namespace = "http://Protiguous.com" )] <br /> Mark fields with [DataMember,
        /// OptionalField] to serialize (both public and private). <br /> Properties have to have both the Getter and the Setter. <br />
        /// </summary>
        /// <param name="collection"></param>
        /// <param name="fileName">  </param>
        /// <returns>Returns True if the object was saved.</returns>
        [Obsolete( "Not in use yet." )]
        public static Boolean SaveCollection<T>( [NotNull] this ConcurrentList<T> collection, [NotNull] String fileName ) where T : class {
            if ( collection is null ) {
                throw new ArgumentNullException( nameof( collection ) );
            }

            if ( String.IsNullOrWhiteSpace( fileName ) ) {
                throw new ArgumentNullException( nameof( fileName ) );
            }

            return collection.Saver( fileName );
        }

        //[Obsolete( "Use JSON methods" )]
        public static Boolean Saver<TSource>( [CanBeNull] this TSource objectToSerialize, [NotNull] String fileName ) where TSource : class {

            //TODO pass in a backup flag to save the newest copy with the backup time.
            // or a Backup class ?

            if ( null == objectToSerialize ) {
                return default;
            }

            if ( fileName is null ) {
                throw new ArgumentNullException( nameof( fileName ) );
            }

            try {
                if ( !IsolatedStorageFile.IsEnabled || String.IsNullOrWhiteSpace( fileName ) ) {
                    return default;
                }

                using ( var _ = new SingleAccess( "IsolatedStorageFile.GetMachineStoreForDomain()" ) ) {
                    using ( var isf = IsolatedStorageFile.GetMachineStoreForDomain() ) {
                        try {
                            var dir = Path.GetDirectoryName( fileName );

                            if ( !String.IsNullOrWhiteSpace( dir ) && !isf.DirectoryExists( dir ) ) {
                                isf.CreateDirectory( dir );
                            }
                        }
                        catch ( IsolatedStorageException exception ) {
                            exception.Log();

                            return default;
                        }
                        catch ( PathTooLongException exception ) {
                            exception.Log();

                            return default;
                        }
                        catch ( ArgumentException exception ) {
                            exception.Log();

                            return default;
                        }

                        try {
                            var isfs = new IsolatedStorageFileStream( fileName, isf.FileExists( fileName ) ? FileMode.Truncate : FileMode.CreateNew, FileAccess.Write, isf );

                            var context = new StreamingContext( StreamingContextStates.All );

                            var serializer = new NetDataContractSerializer( context, Int32.MaxValue, false, FormatterAssemblyStyle.Simple, null /*surrogateSelector*/ );

                            var extension = Path.GetExtension( fileName );
                            var useCompression = !String.IsNullOrWhiteSpace( extension ) && extension.EndsWith( "Z", true, null );

                            if ( useCompression ) {
                                using ( var compress = new GZipStream( isfs, CompressionMode.Compress, true ) ) {
                                    serializer.Serialize( compress, objectToSerialize );
                                }
                            }
                            else {
                                serializer.Serialize( isfs, objectToSerialize );
                            }

                            return true;
                        }
                        catch ( InvalidDataContractException exception ) {
                            exception.Log();
                        }
                        catch ( SerializationException exception ) {
                            exception.Log();
                        }
                        catch ( QuotaExceededException exception ) {
                            exception.Log();
                        }
                        catch ( ArgumentNullException exception ) {
                            exception.Log();
                        }
                        catch ( ArgumentException exception ) {
                            exception.Log();
                        }
                    }
                }
            }
            catch ( IsolatedStorageException exception ) {
                exception.Log();
            }
            catch ( SecurityException exception ) {
                exception.Log();
            }
            catch ( Exception exception ) {
                exception.Log();
            }

            return default;
        }

        /// <summary>
        /// Persist an object to an IsolatedStorageFile. <br /> Mark class with [DataContract( Namespace = "http://Protiguous.com" )] <br /> Mark fields with [DataMember,
        /// OptionalField] to serialize (both public and private). <br /> Fields cannot have JUST the Getter or the Setter, has to have both. <br />
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="obj">     </param>
        /// <param name="fileName"></param>
        /// <returns>Returns True if the object was saved.</returns>
        public static Boolean SaveValue<TSource>( this TSource obj, [NotNull] String fileName ) where TSource : struct {
            if ( fileName is null ) {
                throw new ArgumentNullException( nameof( fileName ) );
            }

            try {
                if ( String.IsNullOrEmpty( fileName ) ) {
                    return default;
                }

                try {
                    using ( var isf = IsolatedStorageFile.GetMachineStoreForDomain() ) {
                        String dir;

                        try {
                            dir = Path.GetDirectoryName( fileName );
                        }
                        catch ( PathTooLongException exception ) {
                            exception.Log();

                            return default;
                        }
                        catch ( ArgumentException exception ) {
                            exception.Log();

                            return default;
                        }

                        if ( !String.IsNullOrEmpty( dir ) && !isf.DirectoryExists( dir ) ) {
                            try {
                                isf.CreateDirectory( dir );
                            }
                            catch ( IsolatedStorageException exception ) {
                                exception.Log();

                                return default;
                            }
                        }

                        try {
                            var isfs = new IsolatedStorageFileStream( fileName, FileMode.Create, FileAccess.Write, isf );

                            //var serializer = new DataContractSerializer( typeof ( T ) );
                            var serializer = new NetDataContractSerializer();

                            var ext = Path.GetExtension( fileName );
                            var useCompression = ext.EndsWith( "Z", true, null );

                            if ( useCompression ) {
                                using ( var compress = new GZipStream( isfs, CompressionMode.Compress, true ) ) {
                                    serializer.WriteObject( compress, obj );
                                }
                            }
                            else {
                                serializer.WriteObject( isfs, obj );
                                isfs.Close();
                            }

                            return true;
                        }
                        catch ( InvalidDataContractException exception ) {
                            exception.Log();
                        }
                        catch ( SerializationException exception ) {
                            exception.Log();
                        }
                        catch ( QuotaExceededException exception ) {
                            exception.Log();
                        }
                        catch ( ArgumentNullException exception ) {
                            exception.Log();
                        }
                        catch ( ArgumentException exception ) {
                            exception.Log();
                        }
                    }
                }
                catch ( IsolatedStorageException exception ) {
                    exception.Log();
                }
            }
            catch ( SecurityException exception ) {
                exception.Log();
            }
            catch ( Exception exception ) {
                exception.Log();
            }

            return default;
        }

        /// <summary></summary>
        /// <typeparam name="TType"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="InvalidDataContractException">
        /// the type being serialized does not conform to data contract rules. For example, the <see cref="DataContractAttribute" /> attribute
        /// has not been applied to the type.
        /// </exception>
        /// <exception cref="SerializationException">there is a problem with the instance being serialized.</exception>
        /// <exception cref="QuotaExceededException">
        /// the maximum number of objects to serialize has been exceeded. Check the <see cref="DataContractSerializer.MaxItemsInObjectGraph" />
        /// property.
        /// </exception>
        [CanBeNull]
        public static String Serialize<TType>( [NotNull] this TType obj ) {
            if ( Equals( obj, default ) ) {
                throw new ArgumentNullException( nameof( obj ) );
            }

            try {

                //using ( var stream = new MemoryStream() ) {
                //Serializers.Value.WriteObject( stream, obj );
                //return stream.ReadToEnd();
                return obj.ToJSON();

                //}
            }
            catch ( SerializationException exception ) {
                exception.Log();
            }
            catch ( InvalidDataContractException exception ) {
                exception.Log();
            }

            return null;
        }

        /// <summary>
        ///     <para>Persist the <paramref name="dictionary" /> into <paramref name="folder" />.</para>
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="dictionary"></param>
        /// <param name="folder">    </param>
        /// <param name="calledWhat"></param>
        /// <param name="progress">  </param>
        /// <param name="extension"> </param>
        /// <returns></returns>
        public static Boolean SerializeDictionary<TKey, TValue>( [CanBeNull] this ConcurrentDictionary<TKey, TValue> dictionary, [CanBeNull] Folder folder,
            [CanBeNull] String? calledWhat, [CanBeNull] IProgress<Single> progress = null, [CanBeNull] String? extension = ".xml" ) where TKey : IComparable<TKey> {
            if ( null == dictionary ) {
                return default;
            }

            if ( null == folder ) {
                return default;
            }

            if ( !dictionary.Any() ) {
                return default;
            }

            try {

                //Report.Enter();
                var stopwatch = Stopwatch.StartNew();

                if ( !folder.Exists() ) {
                    folder.Create();
                }

                if ( !folder.Exists() ) {
                    throw new DirectoryNotFoundException( folder.FullPath );
                }

                var itemCount = ( UInt64 )dictionary.LongCount();

                String.Format( "Serializing {1} {2} to {0} ...", folder.FullPath, itemCount, calledWhat ).Info();

                var currentLine = 0f;

                var backThen = DateTime.UtcNow.ToGuid();

                var fileName = $"{backThen}{extension}"; //let the time change the file name over time

                var document = new Document( folder, fileName );

                var writer = File.AppendText( document.FullPath );

                var fileCount = UInt64.MinValue + 1;

                foreach ( var pair in dictionary ) {
                    currentLine++;

                    var tuple = new Tuple<TKey, TValue>( pair.Key, pair.Value ); //convert the struct to a class

                    var data = tuple.Serialize();

                    var hereNow = DateTime.UtcNow.ToGuid();

                    if ( backThen != hereNow ) {
                        if ( progress != null ) {
                            var soFar = currentLine / itemCount;
                            progress.Report( soFar );
                        }

                        using ( writer ) { }

                        fileName = $"{hereNow}.xml"; //let the file name change over time so we don't have bigHuge monolithic files.
                        document = new Document( folder, fileName );
                        writer = File.AppendText( document.FullPath );
                        fileCount++;
                        backThen = DateTime.UtcNow.ToGuid();
                    }

                    writer.WriteLine( data );
                }

                using ( writer ) { }

                stopwatch.Stop();
                String.Format( "Serialized {1} {3} in {0} into {2} files.", stopwatch.Elapsed.Simpler(), itemCount, fileCount, calledWhat ).Info();

                return true;
            }
            catch ( Exception exception ) {
                exception.Log();
            }

            //finally {
            //    //Report.Exit();
            //}
            return default;
        }

        /// <summary>See also <see cref="Deserializer{T}" />.</summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="self"></param>
        /// <returns></returns>
        [CanBeNull]
        public static Byte[]? Serializer<T>( [NotNull] this T self ) {
            if ( self is null ) {
                throw new ArgumentNullException( nameof( self ) );
            }

            try {
                using ( var memoryStream = new MemoryStream() ) {
                    var binaryFormatter = new BinaryFormatter();
                    binaryFormatter.Serialize( memoryStream, self );

                    return memoryStream.ToArray();
                }
            }
            catch ( Exception exception ) {
                exception.Log();
            }

            return default;
        }

        /*

        /// <summary>
        ///     Attempts to serialize this object to an NTFS alternate stream with the index of <paramref name="attribute" />. Use
        ///     <see cref="TryLoad{TSource}" /> to load an object.
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="objectToSerialize"></param>
        /// <param name="attribute">        </param>
        /// <param name="destination"></param>
        /// <param name="compression"></param>
        /// <returns></returns>
        public static Boolean Save<TSource>(this TSource objectToSerialize, [NotNull] String attribute, Document destination, CompressionLevel compression = CompressionLevel.Fastest)
        {
            if (attribute.IsNullOrWhiteSpace()) { throw new ArgumentNullException(nameof(attribute)); }

            try
            {

                var json = objectToSerialize.ToJSON();

                if (json.IsNullOrWhiteSpace()) { return default; }

                var data = Encoding.Unicode.GetBytes(json).Compress(CompressionLevel.Fastest);

                var filename = $"{destination.FullPathWithFileName}:{attribute}";

                using (var fs = NtfsAlternateStream.Open(filename, access: FileAccess.Write, mode: FileMode.Create, share: FileShare.None)) { fs.Write(data, 0, data.Length); }

                return true;
            }
            catch (SerializationException exception) { exception.Log(); }
            catch (Exception exception) { exception.Log(); }

            return default;
        }
        */

        /// <summary>Set a static <paramref name="key" /> to the <paramref name="value" />.</summary>
        /// <param name="key">  </param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static Boolean Settings( [NotNull] String key, [NotNull] String value ) {
            if ( String.IsNullOrWhiteSpace( key ) ) {
                throw new ArgumentException( "Value cannot be null or whitespace.", nameof( key ) );
            }

            if ( String.IsNullOrWhiteSpace( value ) ) {
                throw new ArgumentException( "Value cannot be null or whitespace.", nameof( value ) );
            }

            return Environment.SpecialFolder.LocalApplicationData.Settings( key, value );
        }

        /// <summary>Set a static <paramref name="key" /> to the <paramref name="value" />.</summary>
        /// <param name="specialFolder"></param>
        /// <param name="key">          </param>
        /// <param name="value">        </param>
        /// <returns></returns>
        public static Boolean Settings( this Environment.SpecialFolder specialFolder, [NotNull] String key, [CanBeNull] String? value ) {
            if ( !Enum.IsDefined( typeof( Environment.SpecialFolder ), specialFolder ) ) {
                throw new InvalidEnumArgumentException( nameof( specialFolder ), ( Int32 )specialFolder, typeof( Environment.SpecialFolder ) );
            }

            if ( String.IsNullOrWhiteSpace( key ) ) {
                throw new ArgumentException( "Value cannot be null or whitespace.", nameof( key ) );
            }

            try {
                var configFile = ConfigurationManager.OpenExeConfiguration( specialFolder.GetStaticFile().FullPath );
                var settings = configFile.AppSettings.Settings;

                if ( settings[ key ] is null ) {
                    settings.Add( key, value );
                }
                else {
                    settings[ key ].Value = value;
                }

                configFile.Save( ConfigurationSaveMode.Modified );
                ConfigurationManager.RefreshSection( configFile.AppSettings.SectionInformation.Name );

                return true;
            }
            catch ( ConfigurationErrorsException exception ) {
                exception.Log();
            }

            return default;
        }

        /// <summary>Return the value of the given <paramref name="key" />.</summary>
        /// <param name="key"></param>
        /// <returns></returns>
        [CanBeNull]
        public static String? Settings( [NotNull] String key ) {
            if ( key is null ) {
                throw new ArgumentNullException( nameof( key ) );
            }

            return Environment.SpecialFolder.CommonApplicationData.Settings( key );
        }

        /// <summary>Return the value of the given <paramref name="key" />.</summary>
        /// <param name="specialFolder"></param>
        /// <param name="key">          </param>
        /// <returns></returns>
        [CanBeNull]
        public static String? Settings( this Environment.SpecialFolder specialFolder, [NotNull] String key ) {
            if ( key is null ) {
                throw new ArgumentNullException( nameof( key ) );
            }

            try {
                var configFile = ConfigurationManager.OpenExeConfiguration( specialFolder.GetStaticFile().FullPath );

                return configFile.AppSettings.Settings[ key ]?.Value;
            }
            catch ( ConfigurationErrorsException exception ) {
                exception.Log();
            }

            return null;
        }

        /// <summary>Return this object as a JSON string.</summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj">       </param>
        /// <param name="formatting"></param>
        /// <returns></returns>
        [CanBeNull]
        public static String? ToJSON<T>( [CanBeNull] this T obj, Formatting formatting = Formatting.None ) => JsonConvert.SerializeObject( obj, formatting, Jss.Value );

        /*

        /// <summary>
        ///     <para>
        ///         Attempts to deserialize an NTFS alternate stream with the <paramref name="attribute" /> to the file
        ///         <paramref name="location" />.
        ///     </para>
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="attribute"></param>
        /// <param name="value">    </param>
        /// <param name="location"> </param>
        /// <see cref="TrySave{TKey}" />
        /// <returns></returns>
        public static Boolean TryLoad<TSource>([NotNull] this String attribute, out TSource value, String location = null) {
            if (attribute is null) { throw new ArgumentNullException(nameof(attribute)); }

            value = default;

            try {
                if (location.IsNullOrWhiteSpace()) { location = LocalDataFolder.Value.FullPath; }

                var filename = $"{location}:{attribute}";

                if (!NtfsAlternateStream.Exists(filename)) { return default; }

                using (var fs = NtfsAlternateStream.Open(filename, access: FileAccess.Read, mode: FileMode.Open, share: FileShare.None)) {
                    var serializer = new NetDataContractSerializer();
                    value = (TSource)serializer.Deserialize(fs);
                }

                return true;
            }
            catch (InvalidOperationException exception) { exception.Log(); }
            catch (ArgumentNullException exception) { exception.Log(); }
            catch (SerializationException exception) { exception.Log(); }
            catch (Exception exception) { exception.Log(); }

            return default;
        }
        */

        /// <summary>Persist the <paramref name="self" /> to a JSON text file.</summary>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="self">    </param>
        /// <param name="document">  </param>
        /// <param name="overwrite"> </param>
        /// <param name="formatting"></param>
        /// <returns></returns>
        public static Boolean TrySave<TKey>( [CanBeNull] this TKey self, [NotNull] IDocument document, Boolean overwrite = true, Formatting formatting = Formatting.None ) {
            if ( document is null ) {
                throw new ArgumentNullException( nameof( document ) );
            }

            if ( overwrite && document.Exists() ) {
                document.Delete();
            }

            using ( var snag = new SingleAccess( document ) ) {
                if ( !snag.Snagged ) {
                    return default;
                }

                using ( var writer = File.AppendText( document.FullPath ) ) {
                    using ( JsonWriter jw = new JsonTextWriter( writer ) ) {
                        jw.Formatting = formatting;

                        //see also http://stackoverflow.com/a/8711702/956364
                        var serializer = new JsonSerializer {
                            ReferenceLoopHandling = ReferenceLoopHandling.Serialize,
                            PreserveReferencesHandling = PreserveReferencesHandling.All
                        };

                        serializer.Serialize( jw, self );
                    }
                }
            }

            return true;
        }

        internal class MyContractResolver : DefaultContractResolver {

            [NotNull]
            protected override IList<JsonProperty> CreateProperties( [CanBeNull] Type type, MemberSerialization memberSerialization ) {
                var list = base.CreateProperties( type, memberSerialization );

                foreach ( var prop in list ) {
                    if ( prop != null ) {
                        prop.Ignored = false; // Don't ignore any property
                    }
                }

                return list;
            }
        }
    }
}