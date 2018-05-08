// Copyright 2018 Protiguous.
//
// This notice must be kept visible in the source.
//
// This section of source code belongs to Protiguous@Protiguous.com unless otherwise specified, or the
// original license has been overwritten by the automatic formatting of this code. Any unmodified
// sections of source code borrowed from other projects retain their original license and thanks
// goes to the Authors.
//
// Donations and royalties can be paid via
//  
//  bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//  
//
// Usage of the source code or compiled binaries is AS-IS. I am not responsible for Anything You Do.
//
// Contact me by email if you have any questions or helpful criticism.
//
// "Librainian/Bits.cs" was last cleaned by Protiguous on 2016/06/18 at 10:52 PM

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