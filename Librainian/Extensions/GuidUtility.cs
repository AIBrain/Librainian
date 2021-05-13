// Copyright © Protiguous. All Rights Reserved.
// This entire copyright notice and license must be retained and must be kept visible in any binaries, libraries, repositories, or source code (directly or derived) from our binaries, libraries, projects, solutions, or applications.
// All source code belongs to Protiguous@Protiguous.com unless otherwise specified or the original license has been overwritten by formatting. (We try to avoid it from happening, but it does accidentally happen.)
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
// File "GuidUtility.cs" last formatted on 2020-08-14 at 8:33 PM.

namespace Librainian.Extensions {

	using System;
	using System.Security.Cryptography;
	using System.Text;
	using JetBrains.Annotations;

	/// <summary>Helper methods for working with <see cref="Guid" />.</summary>
	/// <see cref="http://github.com/LogosBible/Logos.Utility/blob/master/src/Logos.Utility/GuidUtility.cs" />
	public static class GuidUtility {

		/// <summary>The namespace for fully-qualified domain names (from RFC 4122, Appendix C).</summary>
		public static Guid DnsNamespace { get; } = new( "6ba7b810-9dad-11d1-80b4-00c04fd430c8" );

		/// <summary>The namespace for ISO OIDs (from RFC 4122, Appendix C).</summary>
		public static Guid IsoOidNamespace { get; } = new( "6ba7b812-9dad-11d1-80b4-00c04fd430c8" );

		/// <summary>The namespace for URLs (from RFC 4122, Appendix C).</summary>
		public static Guid UrlNamespace { get; } = new( "6ba7b811-9dad-11d1-80b4-00c04fd430c8" );

		/// <summary>Creates a name-based UUID using the algorithm from RFC 4122 §4.3.</summary>
		/// <param name="namespaceId">The ID of the namespace.</param>
		/// <param name="name">       The name (within that namespace).</param>
		/// <returns>A UUID derived from the namespace and name.</returns>
		/// <remarks>
		///     See
		///     <a href="http://code.logos.com/blog/2011/04/generating_a_deterministic_guid.html"> Generating a deterministic GUID </a>
		///     .
		/// </remarks>
		public static Guid Create( Guid namespaceId, [NotNull] String name ) => Create( namespaceId, name, 5 );

		/// <summary>Creates a name-based UUID using the algorithm from RFC 4122 §4.3.</summary>
		/// <param name="namespaceId">The ID of the namespace.</param>
		/// <param name="name">       The name (within that namespace).</param>
		/// <param name="version">
		///     The version number of the UUID to create; this value must be either 3 (for MD5 hashing) or 5 (for
		///     SHA-1 hashing).
		/// </param>
		/// <returns>A UUID derived from the namespace and name.</returns>
		/// <remarks>
		///     See
		///     <a href="http://code.logos.com/blog/2011/04/generating_a_deterministic_guid.html"> Generating a deterministic GUID </a>
		///     .
		/// </remarks>
		public static Guid Create( Guid namespaceId, [NotNull] String name, Int32 version ) {
			if ( name is null ) {
				throw new ArgumentNullException( nameof( name ) );
			}

			if ( version is not 3 and not 5 ) {
				throw new ArgumentOutOfRangeException( nameof( version ), "version must be either 3 or 5." );
			}

			// convert the name to a sequence of octets (as defined by the standard or conventions of its namespace) (step 3)
			// ASSUME: UTF-8 encoding is always appropriate
			var nameBytes = Encoding.UTF8.GetBytes( name );

			// convert the namespace UUID to network order (step 3)
			var namespaceBytes = namespaceId.ToByteArray();
			namespaceBytes.SwapByteOrder();

			// compute the hash of the name space ID concatenated with the name (step 4)
			Byte[] hash;

			using ( var algorithm = version == 3 ? ( HashAlgorithm )MD5.Create() : SHA1.Create() ) {
				algorithm.TransformBlock( namespaceBytes, 0, namespaceBytes.Length, null, 0 );
				algorithm.TransformFinalBlock( nameBytes, 0, nameBytes.Length );
				hash = algorithm.Hash;
			}

			// most bytes from the hash are copied straight to the bytes of the new GUID (steps 5-7, 9, 11-12)
			var newGuid = new Byte[ 16 ];
			Buffer.BlockCopy( hash, 0, newGuid, 0, 16 );

			// set the four most significant bits (bits 12 through 15) of the time_hi_and_version field to the appropriate 4-bit version number from Section 4.1.3 (step 8)
			newGuid[ 6 ] = ( Byte )( ( newGuid[ 6 ] & 0x0F ) | ( version << 4 ) );

			// set the two most significant bits (bits 6 and 7) of the clock_seq_hi_and_reserved to zero and one, respectively (step 10)
			newGuid[ 8 ] = ( Byte )( ( newGuid[ 8 ] & 0x3F ) | 0x80 );

			// convert the resulting UUID to local byte order (step 13)
			newGuid.SwapByteOrder();

			return new Guid( newGuid );
		}

		// Converts a GUID (expressed as a byte array) to/from network order (MSB-first).
		public static void SwapByteOrder( [NotNull] this Byte[] guid ) {
			guid.SwapBytes( 0, 3 );
			guid.SwapBytes( 1, 2 );
			guid.SwapBytes( 4, 5 );
			guid.SwapBytes( 6, 7 );
		}

		public static void SwapBytes( [NotNull] this Byte[] bytes, Int32 leftIndex, Int32 rightIndex ) {
			var temp = bytes[ leftIndex ];
			bytes[ leftIndex ] = bytes[ rightIndex ];
			bytes[ rightIndex ] = temp;
		}
	}
}