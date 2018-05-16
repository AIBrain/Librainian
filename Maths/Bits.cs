// Copyright © 1995-2018 to Rick@AIBrain.org and Protiguous.
// All Rights Reserved.
//
// This ENTIRE copyright notice and file header MUST BE KEPT
// VISIBLE in any source code derived from or used from our
// libraries and projects.
//
// =========================================================
// This section of source code, "Bits.cs",
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
// "Librainian/Librainian/Bits.cs" was last cleaned by Protiguous on 2018/05/15 at 10:45 PM.

namespace Librainian.Maths {

    using System;
    using System.Linq;
    using Numbers;

    public static class Bits {

        public static Boolean IsLittleEndian { get; } = BitConverter.IsLittleEndian;

        public static Byte[] GetBytes( this Int16 value ) => Order( BitConverter.GetBytes( value ) );

        public static Byte[] GetBytes( this UInt16 value ) => Order( BitConverter.GetBytes( value ) );

        public static Byte[] GetBytes( this Int32 value ) => Order( BitConverter.GetBytes( value ) );

        public static Byte[] GetBytes( this UInt32 value ) => Order( BitConverter.GetBytes( value ) );

        public static Byte[] GetBytes( this Int64 value ) => Order( BitConverter.GetBytes( value ) );

        public static Byte[] GetBytes( this UInt64 value ) => Order( BitConverter.GetBytes( value ) );

        public static Byte[] GetBytes( this UInt256 value ) => value.ToByteArray();

        public static Byte[] Order( this Byte[] value ) => IsLittleEndian ? value : value.Reverse().ToArray();

        public static Int32 ToInt32( this Byte[] value, Int32 startIndex = 0 ) => BitConverter.ToInt32( Order( value ), startIndex );

        public static String ToString( this Byte[] value, Int32 startIndex = 0 ) => BitConverter.ToString( Order( value ), startIndex );

        public static UInt16 ToUInt16( this Byte[] value, Int32 startIndex = 0 ) => BitConverter.ToUInt16( Order( value ), startIndex );

        public static UInt256 ToUInt256( this Byte[] value ) => new UInt256( value );

        public static UInt32 ToUInt32( this Byte[] value, Int32 startIndex = 0 ) => BitConverter.ToUInt32( Order( value ), startIndex );

        //public static UInt64 ToUInt64( this Byte[] value, Int32 startIndex = 0 ) => BitConverter.ToUInt64( Order( value ), startIndex );
    }
}