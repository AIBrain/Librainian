// Copyright © Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible in any binaries, libraries, repositories, or source code (directly or derived) from our binaries, libraries, projects, solutions, or applications.
//
// All source code belongs to Protiguous@Protiguous.com unless otherwise specified or the original license has been overwritten by formatting. (We try to avoid it from happening, but it does accidentally happen.)
//
// Any unmodified portions of source code gleaned from other sources still retain their original license and our thanks goes to those Authors.
// If you find your code unattributed in this source code, please let us know so we can properly attribute you and include the proper license and/or copyright(s).
// If you want to use any of our code in a commercial project, you must contact Protiguous@Protiguous.com for permission, license, and a quote.
//
// Donations, payments, and royalties are accepted via bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2 and PayPal: Protiguous@Protiguous.com
//
// ====================================================================
// Disclaimer:  Usage of the source code or binaries is AS-IS.
// No warranties are expressed, implied, or given.
// We are NOT responsible for Anything You Do With Our Code.
// We are NOT responsible for Anything You Do With Our Executables.
// We are NOT responsible for Anything You Do With Your Computer.
// ====================================================================
//
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
// For business inquiries, please contact me at Protiguous@Protiguous.com.
// Our software can be found at "https://Protiguous.Software/"
// Our GitHub address is "https://github.com/Protiguous".
//
// File "PersistenceExtensions.cs" last touched on 2021-03-07 at 5:23 PM by Protiguous.

#nullable enable

namespace Librainian.Persistence;

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Converters;
using Exceptions;
using FileSystem;
using Logging;
using Measurement.Time;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

public static class PersistenceExtensions {

	/// <summary>
	///     <para><see cref="Folder" /> to store application data.</para>
	///     <para>
	///         <see cref="Environment.SpecialFolder.LocalApplicationData" />
	///     </para>
	/// </summary>
	public static readonly Lazy<Folder> LocalDataFolder = new( () => {
		var folder = new Folder( Environment.SpecialFolder.LocalApplicationData );

		if ( !folder.Info.Exists ) {
			folder.Info.Create();
		}

		return folder;
	} );

	public static readonly ThreadLocal<JsonSerializer> LocalJsonSerializers = new( () => new JsonSerializer {
		ReferenceLoopHandling = ReferenceLoopHandling.Serialize,
		PreserveReferencesHandling = PreserveReferencesHandling.All
	}, true );

	public static readonly ThreadLocal<StreamingContext> StreamingContexts = new( () => new StreamingContext( StreamingContextStates.All ), true );

	public static ThreadLocal<JsonSerializerSettings> Jss { get; } = new( () => new JsonSerializerSettings {

		//TODO ContractResolver needs testing
		ContractResolver = new MyContractResolver(),
		TypeNameHandling = TypeNameHandling.Auto,
		NullValueHandling = NullValueHandling.Ignore,
		DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate,
		ReferenceLoopHandling = ReferenceLoopHandling.Serialize,
		PreserveReferencesHandling = PreserveReferencesHandling.All
	}, true );

	/// <summary>Bascially just calls <see cref="FromJSON{T}"/>.</summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="storedAsString"></param>
	/// <exception cref="ArgumentEmptyException"></exception>
	/// <exception cref="EncoderFallbackException"></exception>
	/// <exception cref="FormatException"></exception>
	/// <exception cref="System.Xml.XmlException"></exception>
	public static T? Deserialize<T>( this String? storedAsString ) {
		try {
			return storedAsString.FromJSON<T>();
		}
		catch ( SerializationException exception ) {
			exception.Log();
		}

		return default( T? );
	}

	public static async Task<Boolean> DeserializeDictionary<TKey, TValue>(
		this ConcurrentDictionary<TKey, TValue> toDictionary,
		Folder folder,
		String? calledWhat,
		String? extension,
		CancellationToken cancellationToken
	) where TKey : IComparable<TKey> {
		if ( folder == null ) {
			throw new ArgumentEmptyException( nameof( folder ) );
		}

		try {

			//Report.Enter();
			var stopwatch = Stopwatch.StartNew();

			extension ??= ".xml";

			var exists = await folder.Exists( cancellationToken ).ConfigureAwait( false );

			if ( exists != true ) {
				return false;
			}

			var fileCount = UInt64.MinValue;
			var before = toDictionary.Count;

			//enumerate all the files with the wildcard *.extension

			await foreach ( var document in folder.EnumerateDocuments( $"*.{extension}", cancellationToken ).ConfigureAwait( false ) ) {
				var length = await document.Length( cancellationToken ).ConfigureAwait( false );

				if ( length < 1 ) {
					continue;
				}

				try {
					fileCount++;
					var lines = File.ReadLines( document.FullPath ).AsParallel();

					lines.ForAll( line => {
						try {
							if ( String.IsNullOrWhiteSpace( line ) ) {
								return;
							}

							(var key, var value) = line.Deserialize<(TKey, TValue)>();

							toDictionary[key] = value;
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

			var after = toDictionary.Count;

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
		return false;
	}

	/// <summary>See also <see cref="Serializer" />.</summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="bytes"></param>
	[Obsolete]
	public static T Deserializer<T>( this Byte[] bytes ) {
		using var memoryStream = new MemoryStream( bytes );

		var binaryFormatter = new BinaryFormatter();

		return ( T )binaryFormatter.Deserialize( memoryStream );
	}

	/// <summary>Can the file be read from at this moment in time ?</summary>
	/// <param name="isf">     </param>
	/// <param name="document"></param>
	public static Boolean FileCanBeRead( this IsolatedStorageFile isf, Document document ) {
		if ( isf is null ) {
			throw new ArgumentEmptyException( nameof( isf ) );
		}

		if ( document is null ) {
			throw new ArgumentEmptyException( nameof( document ) );
		}

		try {
			using var stream = isf.OpenFile( document.FileName, FileMode.Open, FileAccess.Read, FileShare.Read );

			try {
				return stream.Seek( 0, SeekOrigin.End ) > 0;
			}
			catch ( ArgumentException exception ) {
				exception.Log();
			}
		}
		catch ( IsolatedStorageException exception ) {
			exception.Log();
		}
		catch ( ArgumentEmptyException exception ) {
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

		return false;
	}

	public static Boolean FileCannotBeRead( this IsolatedStorageFile isf, Document document ) {
		if ( isf is null ) {
			throw new ArgumentEmptyException( nameof( isf ) );
		}

		if ( document is null ) {
			throw new ArgumentEmptyException( nameof( document ) );
		}

		return !isf.FileCanBeRead( document );
	}

	/// <summary>Return this JSON string as an object.</summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="data"></param>
	public static T? FromJSON<T>( this String? data ) {
		if ( String.IsNullOrWhiteSpace( data ) ) {
			return default( T );
		}

		return JsonConvert.DeserializeObject<T>( data, Jss.Value );
	}

	/// <summary>Basically just calls <see cref="ToJSON{T}"/>.</summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="self"></param>
	/// <exception cref="ArgumentEmptyException"></exception>
	/// <exception cref="InvalidDataContractException">
	///     the type being serialized does not conform to data contract rules. For example, the
	///     <see cref="DataContractAttribute" /> attribute
	///     has not been applied to the type.
	/// </exception>
	/// <exception cref="SerializationException">there is a problem with the instance being serialized.</exception>
	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static String? Serialize<T>( [DisallowNull] this T self ) {
		if ( self is null ) {
			throw new ArgumentEmptyException( nameof( self ) );
		}

		try {
			return self.ToJSON();
		}
		catch ( SerializationException exception ) {
			exception.Log();
		}
		catch ( InvalidDataContractException exception ) {
			exception.Log();
		}

		return default( String? );
	}

	/// <summary>
	///     <para>Persist the <paramref name="dictionary" /> into <paramref name="folder" />.</para>
	/// </summary>
	/// <typeparam name="TKey"></typeparam>
	/// <typeparam name="TValue"></typeparam>
	/// <param name="dictionary"></param>
	/// <param name="folder">    </param>
	/// <param name="calledWhat"></param>
	/// <param name="cancellationToken"></param>
	/// <param name="progress">  </param>
	/// <param name="extension"> </param>
	public static async Task<Boolean> SerializeDictionary<TKey, TValue>(
		this IDictionary<TKey, TValue> dictionary,
		Folder folder,
		String? calledWhat, CancellationToken cancellationToken,
		IProgress<Single>? progress = null,
		String? extension = ".xml"
	) where TKey : IComparable<TKey> {
		if ( !dictionary.Any() ) {
			return false;
		}

		try {

			//Report.Enter();
			var stopwatch = Stopwatch.StartNew();

			if ( await folder.Create( cancellationToken ).ConfigureAwait( false ) == false ) {
				throw new DirectoryNotFoundException( folder.FullPath );
			}

			var itemCount = ( UInt64 )dictionary.Count;

			String.Format( "Serializing {1} {2} to {0} ...", folder.FullPath, itemCount, calledWhat ).Info();

			var currentLine = 0f;

			var backThen = DateTime.UtcNow.ToGuid();

			var fileName = $"{backThen}{extension}"; //let the time change the file name over time

			var document = new Document( folder, fileName );

			var writer = File.AppendText( document.FullPath );

			var fileCount = UInt64.MinValue + 1;

			foreach ( var pair in dictionary ) {
				currentLine++;

				var data = (pair.Key, pair.Value).Serialize();

				var hereNow = DateTime.UtcNow.ToGuid();

				if ( backThen != hereNow ) {
					if ( progress != null ) {
						var soFar = currentLine / itemCount;
						progress.Report( soFar );
					}

					await using ( writer.ConfigureAwait( false ) ) { }

					fileName = $"{hereNow}.xml"; //let the file name change over time so we don't have bigHuge monolithic files.
					using var newdocument = new Document( folder, fileName );
					writer = File.AppendText( newdocument.FullPath );
					fileCount++;
					backThen = DateTime.UtcNow.ToGuid();
				}

				await writer.WriteLineAsync( data ).ConfigureAwait( false );
			}

			await using ( writer.ConfigureAwait( false ) ) { }

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
		return false;
	}

	/*

	/// <summary>See also <see cref="Deserializer{T}" />.</summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="self"></param>
	/// <returns></returns>
	[CanBeNull]
	public static Byte[]? Serializer<T>( [NotNull] this T self ) {
		if ( self is null ) {
			throw new ArgumentEmptyException( nameof( self ) );
		}

		try {
			using var memoryStream = new MemoryStream();

			var binaryFormatter = new BinaryFormatter();
			binaryFormatter.Serialize( memoryStream, self );

			return memoryStream.ToArray();
		}
		catch ( Exception exception ) {
			exception.Log();
		}

		return default;
	}
	*/

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
		if (attribute.IsNullOrWhiteSpace()) { throw new ArgumentEmptyException(nameof(attribute)); }

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

	/// <summary>Return this object as a JSON string or null.</summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="self">       </param>
	/// <param name="formatting"></param>
	public static String? ToJSON<T>( this T? self, Formatting formatting = Formatting.None ) =>
		self is null ? default( String? ) : JsonConvert.SerializeObject( self, formatting, Jss.Value );

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
		if (attribute is null) { throw new ArgumentEmptyException(nameof(attribute)); }

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
		catch (ArgumentEmptyException exception) { exception.Log(); }
		catch (SerializationException exception) { exception.Log(); }
		catch (Exception exception) { exception.Log(); }

		return default;
	}
	*/

	/*

	/// <summary>Persist the <paramref name="self" /> to a JSON text file.</summary>
	/// <typeparam name="TKey"></typeparam>
	/// <param name="self">    </param>
	/// <param name="document">  </param>
	/// <param name="overwrite"> </param>
	/// <param name="formatting"></param>
	/// <returns></returns>
	public static Boolean TrySave<TKey>( [CanBeNull] this TKey self, [NotNull] IDocument document, Boolean overwrite = true, Formatting formatting = Formatting.None ) {
		if ( document is null ) {
			throw new ArgumentEmptyException( nameof( document ) );
		}

		if ( overwrite && document.Exists() ) {
			document.Delete();
		}

		using var snag = new SingleAccess( document );

		if ( !snag.Snagged ) {
			return default;
		}

		using var writer = File.AppendText( document.FullPath );

		using JsonWriter jw = new JsonTextWriter( writer );

		jw.Formatting = formatting;

		//see also http://stackoverflow.com/a/8711702/956364
		var serializer = new JsonSerializer {
			ReferenceLoopHandling = ReferenceLoopHandling.Serialize, PreserveReferencesHandling = PreserveReferencesHandling.All
		};

		serializer.Serialize( jw, self );

		return true;
	}
	*/

	private class MyContractResolver : DefaultContractResolver {

		protected override IList<JsonProperty> CreateProperties( Type? type, MemberSerialization memberSerialization ) {
			var list = base.CreateProperties( type, memberSerialization );

			foreach ( var prop in list ) {
				prop.Ignored = false; // Don't ignore any property
			}

			return list;
		}
	}
}