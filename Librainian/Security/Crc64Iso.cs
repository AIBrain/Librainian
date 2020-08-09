// Copyright � Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible in any binaries, libraries, repositories, or source code (directly or derived) from our binaries, libraries, projects, solutions, or applications.
//
// All source code belongs to Protiguous@Protiguous.com unless otherwise specified or the original license has been overwritten by formatting. (We try to avoid it from happening, but it does accidentally happen.)
//
// Any unmodified portions of source code gleaned from other sources still retain their original license and our thanks goes to those Authors.
// If you find your code unattributed in this source code, please let us know so we can properly attribute you and include the proper license and/or copyright(s).
//
// If you want to use any of our code in a commercial project, you must contact Protiguous@Protiguous.com for permission, license, and a quote.
//
// Donations, payments, and royalties are accepted via bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2 and PayPal: Protiguous@Protiguous.com
//
// ====================================================================
// Disclaimer:  Usage of the source code or binaries is AS-IS.
//     No warranties are expressed, implied, or given.
//     We are NOT responsible for Anything You Do With Our Code.
//     We are NOT responsible for Anything You Do With Our Executables.
//     We are NOT responsible for Anything You Do With Your Computer.
// ====================================================================
//
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
// For business inquiries, please contact me at Protiguous@Protiguous.com.
//
// Our software can be found at "https://Protiguous.Software/"
// Our GitHub address is "https://github.com/Protiguous".

namespace Librainian.Security {

    using System;
    using JetBrains.Annotations;

    /// <summary>
    ///     <see cref="CRC64" />
    /// </summary>
    /// <copyright>Damien Guard. All rights reserved.</copyright>
    /// <see cref="http://github.com/damieng/DamienGKit/blob/master/CSharp/DamienG.Library/Security/Cryptography/Crc64.cs" />
    public class Crc64Iso : CRC64 {

        public const UInt64 Iso3309Polynomial = 0xD800000000000000;

        internal static UInt64[] Table;

        public Crc64Iso() : base( Iso3309Polynomial ) { }

        public Crc64Iso( UInt64 seed ) : base( Iso3309Polynomial, seed ) { }

        public static UInt64 Compute( [NotNull] Byte[] buffer ) => Compute( DefaultSeed, buffer );

        public static UInt64 Compute( UInt64 seed, [NotNull] Byte[] buffer ) {
            if ( Table is null ) {
                Table = CreateTable( Iso3309Polynomial );
            }

            return CalculateHash( seed, Table, buffer, 0, buffer.Length );
        }

    }

}