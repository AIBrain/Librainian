// Copyright © Rick@AIBrain.org and Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
//
// This source code contained in "Cache.cs" belongs to Protiguous@Protiguous.com and
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
// Project: "Librainian", "Cache.cs" was last formatted by Protiguous on 2018/07/13 at 1:35 AM.

namespace Librainian.Persistence {

	using System;
	using System.Collections.Generic;
	using System.Data.SqlClient;
	using System.Diagnostics;
	using System.IO;
	using System.Linq;
	using System.Runtime.Caching;
	using System.Text;
	using Collections;
	using JetBrains.Annotations;
	using Parsing;

	public static class Cache {

		/// <summary>
		///     The string to place between each key part.
		/// </summary>
		private const String Separator = "⦀";

		/// <summary>
		///     Gets a reference to the default <see cref="T:System.Runtime.Caching.MemoryCache" /> instance.
		/// </summary>
		[NotNull]
		public static readonly MemoryCache Memory = MemoryCache.Default;

		/// <summary>
		///     Build a unique string given the <paramref name="parts" />. ( <see cref="Separator" /> between each item)
		/// </summary>
		/// <param name="parts"></param>
		/// <returns></returns>
		/// <exception cref="ArgumentNullException"></exception>
		[DebuggerStepThrough]
		public static TrimmedString BuildKey<T>( [NotNull] params T[] parts ) {
			if ( parts is null ) { throw new ArgumentNullException( nameof( parts ) ).More(); }

			if ( parts.Length == 2 && parts[ 1 ] is ConcurrentHashset<SqlParameter> collection ) {
				var kvp = collection.ToArray().Select( parameter => new {
					parameter.ParameterName,
					parameter.Value
				} );

				return new TrimmedString( parts[ 0 ] + Separator + kvp.ToStrings( Separator ) );
			}

			return new TrimmedString( parts.ToStrings( Separator ) );
		}

		/// <summary>
		///     Build a unique string given the <paramref name="collection" />. ( <see cref="Separator" /> between each item)
		/// </summary>
		/// <param name="collection"></param>
		/// <returns></returns>
		/// <exception cref="ArgumentNullException"></exception>
		[DebuggerStepThrough]
		public static TrimmedString BuildKey( [NotNull] ConcurrentHashset<SqlParameter> collection ) {
			if ( collection is null ) { throw new ArgumentNullException( paramName: nameof( collection ) ); }

			var parts = collection.Select( parm => new KeyValuePair<String, String>( parm.ParameterName, parm.Value.ToString() ) );

			return new TrimmedString( parts.ToStrings( Separator ) );
		}

		/// <summary>
		///     Untested.
		/// </summary>
		/// <param name="cache">   </param>
		/// <param name="filename"></param>
		/// <returns></returns>
		public static Boolean SaveContents( [NotNull] this MemoryCache cache, [NotNull] FileInfo filename ) {
			if ( cache == null ) { throw new ArgumentNullException( nameof( cache ) ); }

			if ( filename == null ) { throw new ArgumentNullException( nameof( filename ) ); }

			//var serializer = JsonSerializer.Create(); //TODO
			//serializer.Serialize( writer, cache );

			using ( var outfile = new FileStream( filename.FullName, FileMode.Create ) ) {
				var writer = new BinaryWriter( outfile, Encoding.Unicode );
				writer.Seek( 0, SeekOrigin.Begin );

				foreach ( var pair in cache ) {
					writer.Write( pair.Key.Length );
					writer.Write( pair.Key );
					writer.Write( Separator );
					var val = Convert.ToString( pair.Value );
					writer.Write( val.Length );
					writer.Write( val ); //TODO json this value?
				}

				return true;
			}
		}

		/// <summary>
		///     Untested.
		/// </summary>
		/// <param name="cache">   </param>
		/// <param name="name">    </param>
		/// <param name="filename"></param>
		/// <returns></returns>
		public static Boolean TryLoadCacheContents( [NotNull] out MemoryCache cache, [NotNull] String name, [NotNull] FileInfo filename ) {
			if ( filename == null ) { throw new ArgumentNullException( nameof( filename ) ); }

			cache = new MemoryCache( name );

			if ( !filename.Exists ) { return false; }

			//TODO binaryreader each KVP

			return false;
		}
	}
}