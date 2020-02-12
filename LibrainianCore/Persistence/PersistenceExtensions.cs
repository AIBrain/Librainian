// Copyright © 2020 Protiguous. All Rights Reserved.
// 
// This entire copyright notice and license must be retained and must be kept visible in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
// 
// This source code contained in "PersistenceExtensions.cs" belongs to Protiguous@Protiguous.com unless otherwise specified or the original license has been overwritten by
// formatting. (We try to avoid it from happening, but it does accidentally happen.)
// 
// Any unmodified portions of source code gleaned from other projects still retain their original license and our thanks goes to those Authors.
// If you find your code in this source code, please let us know so we can properly attribute you and include the proper license and/or copyright.
// 
// If you want to use any of our code in a commercial project, you must contact Protiguous@Protiguous.com for permission and a quote.
// 
// Donations are accepted (for now) via
//     bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//     PayPal: Protiguous@Protiguous.com
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
// Project: "LibrainianCore", File: "PersistenceExtensions.cs" was last formatted by Protiguous on 2020/02/01 at 10:44 AM.

namespace LibrainianCore.Persistence {

    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.IO.IsolatedStorage;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Runtime.Serialization;
    using System.Runtime.Serialization.Formatters.Binary;
    using System.Text;
    using System.Threading;
    using Converters;
    using JetBrains.Annotations;
    using Logging;
    using Measurement.Time;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Serialization;
    using OperatingSystem.FileSystem;
    using Threading;

    // ReSharper disable RedundantUsingDirective
    using Path = OperatingSystem.FileSystem.Pri.LongPath.Path;
    using DirectoryInfo = OperatingSystem.FileSystem.Pri.LongPath.DirectoryInfo;
    using FileInfo = OperatingSystem.FileSystem.Pri.LongPath.FileInfo;
    using FileSystemInfo = OperatingSystem.FileSystem.Pri.LongPath.FileSystemInfo;
    using Directory = OperatingSystem.FileSystem.Pri.LongPath.Directory;
    using File = OperatingSystem.FileSystem.Pri.LongPath.File;
    // ReSharper restore RedundantUsingDirective

    public static class PersistenceExtensions {

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
        /// <summary></summary>
        /// <typeparam name="TType"></typeparam>
        /// <param name="storedAsString"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="EncoderFallbackException"></exception>
        /// <exception cref="FormatException"></exception>
        /// <exception cref="System.Xml.XmlException"></exception>
        [CanBeNull]
        public static TType Deserialize<TType>( [CanBeNull] this String storedAsString ) {
            try {
                return storedAsString.FromJSON<TType>();
            }
            catch ( SerializationException exception ) {
                exception.Log();
            }

            return default;
        }

        public static Boolean DeserializeDictionary<TKey, TValue>( [CanBeNull] this ConcurrentDictionary<TKey, TValue> toDictionary, [CanBeNull] Folder folder,
            [CanBeNull] String calledWhat, [CanBeNull] String extension = ".xml" ) where TKey : IComparable<TKey> {
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

                return ( T ) binaryFormatter.Deserialize( memoryStream );
            }
        }

        /// <summary>Can the file be read from at this moment in time ?</summary>
        /// <param name="isf">     </param>
        /// <param name="document"></param>
        /// <returns></returns>
        public static Boolean FileCanBeRead( [NotNull] this IsolatedStorageFile isf, [NotNull] Document document ) {
            if ( isf is null ) {
                throw new ArgumentNullException( paramName: nameof( isf ) );
            }

            if ( document is null ) {
                throw new ArgumentNullException( paramName: nameof( document ) );
            }

            try {
                using ( var stream = isf.OpenFile( document.FileName, mode: FileMode.Open, access: FileAccess.Read, share: FileShare.Read ) ) {
                    try {
                        return stream.Seek( offset: 0, origin: SeekOrigin.End ) > 0;
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
                throw new ArgumentNullException( paramName: nameof( isf ) );
            }

            if ( document is null ) {
                throw new ArgumentNullException( paramName: nameof( document ) );
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

        /// <summary></summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="InvalidDataContractException">
        /// the type being serialized does not conform to data contract rules. For example, the <see cref="DataContractAttribute" /> attribute
        /// has not been applied to the type.
        /// </exception>
        /// <exception cref="SerializationException">there is a problem with the instance being serialized.</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [CanBeNull]
        public static String Serialize<T>( [NotNull] this T obj ) {
            if ( Equals( obj, default ) ) {
                throw new ArgumentNullException( nameof( obj ) );
            }

            try {
                return obj.ToJSON();
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
        public static Boolean SerializeDictionary<TKey, TValue>( [CanBeNull] this IDictionary<TKey, TValue> dictionary, [CanBeNull] Folder folder,
            [CanBeNull] String calledWhat, [CanBeNull] IProgress<Single> progress = null, [CanBeNull] String extension = ".xml" ) where TKey : IComparable<TKey> {
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
               
                if ( !folder.Create() ) {
                    throw new DirectoryNotFoundException( folder.FullPath );
                }

                var itemCount = ( UInt64 ) dictionary.LongCount();

                String.Format( "Serializing {1} {2} to {0} ...", folder.FullPath, itemCount, calledWhat ).Info();

                var currentLine = 0f;

                var backThen = DateTime.UtcNow.ToGuid();

                var fileName = $"{backThen}{extension}"; //let the time change the file name over time

                var document = new Document( folder, fileName );

                var writer = File.AppendText( document.FullPath );

                var fileCount = UInt64.MinValue + 1;

                foreach ( var pair in dictionary ) {
                    currentLine++;

                    var data = ( pair.Key, pair.Value ).Serialize();

                    var hereNow = DateTime.UtcNow.ToGuid();

                    if ( backThen != hereNow ) {
                        if ( progress != null ) {
                            var soFar = currentLine / itemCount;
                            progress.Report( soFar );
                        }

                        using ( writer ) { }

                        fileName = $"{hereNow}.xml"; //let the file name change over time so we don't have bigHuge monolithic files.
                        using var newdocument = new Document( folder, fileName );
                        writer = File.AppendText( newdocument.FullPath );
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
        public static Byte[] Serializer<T>( [NotNull] this T self ) {
            if ( self is null ) {
                throw new ArgumentNullException( paramName: nameof( self ) );
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

        /// <summary>Return this object as a JSON string.</summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj">       </param>
        /// <param name="formatting"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [CanBeNull]
        public static String ToJSON<T>( [CanBeNull] this T obj, Formatting formatting = Formatting.None ) => JsonConvert.SerializeObject( obj, formatting, Jss.Value );

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
                            ReferenceLoopHandling = ReferenceLoopHandling.Serialize, PreserveReferencesHandling = PreserveReferencesHandling.All
                        };

                        serializer.Serialize( jw, self );
                    }
                }
            }

            return true;
        }

        private class MyContractResolver : DefaultContractResolver {

            [NotNull]
            protected override IList<JsonProperty> CreateProperties( [CanBeNull] Type type, MemberSerialization memberSerialization ) {
                var list = base.CreateProperties( type, memberSerialization );

                foreach ( var prop in list ) {
                    prop.Ignored = false; // Don't ignore any property
                }

                return list;
            }

        }

    }

}