// Copyright © 1995-2018 to Rick@AIBrain.org and Protiguous.
// All Rights Reserved.
//
// This ENTIRE copyright notice and file header MUST BE KEPT
// VISIBLE in any source code derived from or used from our
// libraries and projects.
//
// =========================================================
// This section of source code, "Crc64Iso.cs",
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
// "Librainian/Librainian/Crc64Iso.cs" was last formatted by Protiguous on 2018/05/17 at 3:59 PM.

namespace Librainian.Security {

    using System;

    /// <summary>
    ///     <seealso cref="Crc64" />
    /// </summary>
    /// <copyright>Damien Guard. All rights reserved.</copyright>
    /// <seealso cref="http://github.com/damieng/DamienGKit/blob/master/CSharp/DamienG.Library/Security/Cryptography/Crc64.cs" />
    public class Crc64Iso : Crc64 {

        internal static UInt64[] Table;

        public const UInt64 Iso3309Polynomial = 0xD800000000000000;

        public Crc64Iso() : base( polynomial: Iso3309Polynomial ) { }

        public Crc64Iso( UInt64 seed ) : base( polynomial: Iso3309Polynomial, seed: seed ) { }

        public static UInt64 Compute( Byte[] buffer ) => Compute( seed: DefaultSeed, buffer: buffer );

        public static UInt64 Compute( UInt64 seed, Byte[] buffer ) {
            if ( Table is null ) { Table = CreateTable( polynomial: Iso3309Polynomial ); }

            return CalculateHash( seed: seed, table: Table, buffer: buffer, start: 0, size: buffer.Length );
        }
    }
}