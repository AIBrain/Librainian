namespace Librainian.Maths {
    using System;
    using System.Linq;

    public static class Bits {
        private static readonly bool IsLE = BitConverter.IsLittleEndian;

        public static byte[] GetBytes( Int16 value ) {
            return Order( BitConverter.GetBytes( value ) );
        }

        public static byte[] GetBytes( UInt16 value ) {
            return Order( BitConverter.GetBytes( value ) );
        }

        public static byte[] GetBytes( Int32 value ) {
            return Order( BitConverter.GetBytes( value ) );
        }

        public static byte[] GetBytes( UInt32 value ) {
            return Order( BitConverter.GetBytes( value ) );
        }

        public static byte[] GetBytes( Int64 value ) {
            return Order( BitConverter.GetBytes( value ) );
        }

        public static byte[] GetBytes( UInt64 value ) {
            return Order( BitConverter.GetBytes( value ) );
        }

        public static byte[] GetBytes( UInt256 value ) {
            return value.ToByteArray();
        }

        public static string ToString( byte[] value, int startIndex = 0 ) {
            return BitConverter.ToString( Order( value ), startIndex );
        }

        public static UInt16 ToUInt16( byte[] value, int startIndex = 0 ) {
            return BitConverter.ToUInt16( Order( value ), startIndex );
        }

        public static Int32 ToInt32( byte[] value, int startIndex = 0 ) {
            return BitConverter.ToInt32( Order( value ), startIndex );
        }

        public static UInt32 ToUInt32( byte[] value, int startIndex = 0 ) {
            return BitConverter.ToUInt32( Order( value ), startIndex );
        }

        public static UInt64 ToUInt64( byte[] value, int startIndex = 0 ) {
            return BitConverter.ToUInt64( Order( value ), startIndex );
        }

        public static UInt256 ToUInt256( byte[] value ) {
            return new UInt256( value );
        }

        public static byte[] Order( byte[] value ) {
            return IsLE ? value : value.Reverse().ToArray();
        }
    }
}
