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
// "Librainian/Crc64Iso.cs" was last cleaned by Rick on 2014/09/08 at 11:35 AM
#endregion

namespace Librainian.Security {
    using System;

    /// <summary>
    ///     <seealso cref="Crc64" />
    /// </summary>
    /// <copyright>Damien Guard.  All rights reserved.</copyright>
    /// <seealso cref="http://github.com/damieng/DamienGKit/blob/master/CSharp/DamienG.Library/Security/Cryptography/Crc64.cs" />
    public class Crc64Iso : Crc64 {
        public const UInt64 Iso3309Polynomial = 0xD800000000000000;
        internal static UInt64[] Table;

        public Crc64Iso() : base( Iso3309Polynomial ) { }

        public Crc64Iso( UInt64 seed ) : base( Iso3309Polynomial, seed ) { }

        public static UInt64 Compute( byte[] buffer ) {
            return Compute( DefaultSeed, buffer );
        }

        public static UInt64 Compute( UInt64 seed, byte[] buffer ) {
            if ( Table == null ) {
                Table = CreateTable( Iso3309Polynomial );
            }

            return CalculateHash( seed, Table, buffer, 0, buffer.Length );
        }
    }
}
