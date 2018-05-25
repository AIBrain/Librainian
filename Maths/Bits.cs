// Copyright © 1995-2018 to Rick@AIBrain.org and Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
//
// This source code contained in "Bits.cs" belongs to Rick@AIBrain.org and
// Protiguous@Protiguous.com unless otherwise specified or the original license has
// been overwritten by automatic formatting.
// (We try to avoid it from happening, but it does accidentally happen.)
//
// Any unmodified portions of source code gleaned from other projects still retain their original
// license and our thanks goes to those Authors. If you find your code in this source code, please
// let us know so we can properly attribute you and include the proper license and/or copyright.
//
// Donations, royalties from any software that uses any of our code, or license fees can be paid
// to us via bitcoin at the address 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2.
//
// =========================================================
// Usage of the source code or binaries is AS-IS.
// No warranties are expressed, implied, or given.
// We are NOT responsible for Anything You Do With Our Code.
// =========================================================
//
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
// For business inquiries, please contact me at Protiguous@Protiguous.com
//
// "Librainian/Librainian/Bits.cs" was last formatted by Protiguous on 2018/05/24 at 7:20 PM.

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