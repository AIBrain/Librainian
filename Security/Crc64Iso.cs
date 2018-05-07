// Copyright 2018 Protiguous
//
// This notice must be kept visible in the source.
//
// This section of source code belongs to Protiguous@Protiguous.com unless otherwise specified, or the
// original license has been overwritten by the automatic formatting of this code. Any unmodified
// sections of source code borrowed from other projects retain their original license and thanks
// goes to the Authors.
//
// Donations, royalties, and licenses can be paid via bitcoin:1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//
// Usage of the source code or compiled binaries is AS-IS. I am not responsible for Anything You Do.
//
// Contact me by email if you have any questions or helpful criticism.
//
// "Librainian/Crc64Iso.cs" was last cleaned by Protiguous on 2018/05/06 at 2:22 PM

namespace Librainian.Security {

    using System;

    /// <summary>
    ///     <seealso cref="Crc64" />
    /// </summary>
    /// <copyright>Damien Guard. All rights reserved.</copyright>
    /// <seealso cref="http://github.com/damieng/DamienGKit/blob/master/CSharp/DamienG.Library/Security/Cryptography/Crc64.cs" />
    public class Crc64Iso : Crc64 {
        public const UInt64 Iso3309Polynomial = 0xD800000000000000;
        internal static UInt64[] Table;

        public Crc64Iso() : base( polynomial: Iso3309Polynomial ) {
        }

        public Crc64Iso( UInt64 seed ) : base( polynomial: Iso3309Polynomial, seed: seed ) {
        }

        public static UInt64 Compute( Byte[] buffer ) => Compute( seed: DefaultSeed, buffer: buffer );

        public static UInt64 Compute( UInt64 seed, Byte[] buffer ) {
            if ( Table is null ) {
                Table = CreateTable( polynomial: Iso3309Polynomial );
            }

            return CalculateHash( seed: seed, table: Table, buffer: buffer, start: 0, size: buffer.Length );
        }
    }
}