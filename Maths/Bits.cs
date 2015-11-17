// Copyright 2015 Rick@AIBrain.org.
// 
// This notice must be kept visible in the source.
// 
// This section of source code belongs to Rick@AIBrain.Org unless otherwise specified, or the
// original license has been overwritten by the automatic formatting of this code. Any unmodified
// sections of source code borrowed from other projects retain their original license and thanks
// goes to the Authors.
// 
// Donations and Royalties can be paid via
// PayPal: paypal@aibrain.org
// bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
// litecoin: LeUxdU2w3o6pLZGVys5xpDZvvo8DUrjBp9
// 
// Usage of the source code or compiled binaries is AS-IS. I am not responsible for Anything You Do.
// 
// Contact me by email if you have any questions or helpful criticism.
// 
// "Librainian/Bits.cs" was last cleaned by Rick on 2015/06/12 at 3:00 PM

namespace Librainian.Maths {

    using System;
    using System.Linq;

    public static class Bits {
        private static readonly Boolean IsLe = BitConverter.IsLittleEndian;

        public static Byte[] GetBytes(Int16 value) => Order( BitConverter.GetBytes( value ) );

        public static Byte[] GetBytes(UInt16 value) => Order( BitConverter.GetBytes( value ) );

        public static Byte[] GetBytes(Int32 value) => Order( BitConverter.GetBytes( value ) );

        public static Byte[] GetBytes(UInt32 value) => Order( BitConverter.GetBytes( value ) );

        public static Byte[] GetBytes(Int64 value) => Order( BitConverter.GetBytes( value ) );

        public static Byte[] GetBytes(UInt64 value) => Order( BitConverter.GetBytes( value ) );

        public static Byte[] GetBytes(UInt256 value) => value.ToByteArray();

        public static Byte[] Order(Byte[] value) => IsLe ? value : value.Reverse().ToArray();

        public static Int32 ToInt32(Byte[] value, Int32 startIndex = 0) => BitConverter.ToInt32( Order( value ), startIndex );

        public static String ToString(Byte[] value, Int32 startIndex = 0) => BitConverter.ToString( Order( value ), startIndex );

        public static UInt16 ToUInt16(Byte[] value, Int32 startIndex = 0) => BitConverter.ToUInt16( Order( value ), startIndex );

        public static UInt256 ToUInt256(Byte[] value) => new UInt256( value );

        public static UInt32 ToUInt32(Byte[] value, Int32 startIndex = 0) => BitConverter.ToUInt32( Order( value ), startIndex );

        public static UInt64 ToUInt64(Byte[] value, Int32 startIndex = 0) => BitConverter.ToUInt64( Order( value ), startIndex );
    }
}