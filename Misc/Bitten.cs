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
// "Librainian/Bitten.cs" was last cleaned by Rick on 2014/08/11 at 12:40 AM
#endregion

namespace Librainian.Misc {
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.InteropServices;

    [StructLayout( LayoutKind.Sequential )]
    [Serializable]
    [ComVisible( true )]
    [Obsolete( "untested" )]
    public struct Bitten {
        public static readonly Bitten Empty;

        private readonly byte _d;

        private readonly byte _e;

        private readonly byte _f;

        private readonly byte _g;

        private readonly byte _h;

        private readonly byte _i;

        private readonly byte _j;

        private readonly byte _k;

        /// Creates a new guid from an array of bytes.
        public Bitten( IList< byte > b ) {
            this._d = b[ 0 ];
            this._e = b[ 1 ];
            this._f = b[ 2 ];
            this._g = b[ 3 ];
            this._h = b[ 4 ];
            this._i = b[ 5 ];
            this._j = b[ 6 ];
            this._k = b[ 7 ];
        }

        public Bitten( byte d, byte e, byte f, byte g, byte h, byte i, byte j, byte k ) {
            this._d = d;
            this._e = e;
            this._f = f;
            this._g = g;
            this._h = h;
            this._i = i;
            this._j = j;
            this._k = k;
        }

        [Flags]
        private enum GuidStyles {
            None = 0x00000000,

            AllowParenthesis = 0x00000001, //Allow the guid to be enclosed in parens
            AllowBraces = 0x00000002, //Allow the guid to be enclosed in braces 
            AllowDashes = 0x00000004, //Allow the guid to contain dash group separators 
            AllowHexPrefix = 0x00000008, //Allow the guid to contain {0xdd,0xdd}
            RequireParenthesis = 0x00000010, //Require the guid to be enclosed in parens 
            RequireBraces = 0x00000020, //Require the guid to be enclosed in braces
            RequireDashes = 0x00000040, //Require the guid to contain dash group separators
            RequireHexPrefix = 0x00000080, //Require the guid to contain {0xdd,0xdd}

            HexFormat = RequireBraces | RequireHexPrefix, /* X */

            NumberFormat = None, /* N */

            DigitFormat = RequireDashes, /* D */

            BraceFormat = RequireBraces | RequireDashes, /* B */

            ParenthesisFormat = RequireParenthesis | RequireDashes, /* P */

            Any = AllowParenthesis | AllowBraces | AllowDashes | AllowHexPrefix,
        }

        // Creates a new guid based on the value in the String.  The value is made up
        // of hex digits speared by the dash ("-"). The String may begin and end with 
        // brackets ("{", "}").
        //
        // The String must be of the form dddddddd-dddd-dddd-dddd-dddddddddddd. where
        // d is a hex digit. (That is 8 hex digits, followed by 4, then 4, then 4, 
        // then 12) such as: "CA761232-ED42-11CE-BACD-00AA0057B223"
        // 
        public Bitten( String g ) {
            if ( g == null ) {
                throw new ArgumentNullException( "g" );
            }
            this = Parse( g );
        }

        public static Bitten Parse( String input ) {
            Guid result;
            return Guid.TryParse( input, out result ) ? new Bitten( result.ToByteArray().Skip( 8 ).ToList() ) : Empty;
        }

        // Returns an unsigned byte array containing the GUID. 
        public byte[] ToByteArray() {
            var g = new byte[8];

            g[ 0 ] = this._d;
            g[ 1 ] = this._e;
            g[ 2 ] = this._f;
            g[ 3 ] = this._g;
            g[ 4 ] = this._h;
            g[ 5 ] = this._i;
            g[ 6 ] = this._j;
            g[ 7 ] = this._k;

            return g;
        }

        public Boolean Equals( Bitten g ) {
            // Now compare each of the elements
            if ( g._d != this._d ) {
                return false;
            }
            if ( g._e != this._e ) {
                return false;
            }
            if ( g._f != this._f ) {
                return false;
            }
            if ( g._g != this._g ) {
                return false;
            }
            if ( g._h != this._h ) {
                return false;
            }
            if ( g._i != this._i ) {
                return false;
            }
            if ( g._j != this._j ) {
                return false;
            }
            return g._k == this._k;
        }

        public static Boolean operator ==( Bitten a, Bitten b ) {
            // Now compare each of the elements

            if ( a._d != b._d ) {
                return false;
            }
            if ( a._e != b._e ) {
                return false;
            }
            if ( a._f != b._f ) {
                return false;
            }
            if ( a._g != b._g ) {
                return false;
            }
            if ( a._h != b._h ) {
                return false;
            }
            if ( a._i != b._i ) {
                return false;
            }
            if ( a._j != b._j ) {
                return false;
            }
            return a._k == b._k;
        }

        public static Boolean operator !=( Bitten a, Bitten b ) {
            return !( a == b );
        }

        private static char HexToChar( int a ) {
            a = a & 0xf;
            return ( char ) ( ( a > 9 ) ? a - 10 + 0x61 : a + 0x30 );
        }

        private static int HexsToChars( IList< char > guidChars, int offset, int a, int b, Boolean hex = false ) {
            if ( hex ) {
                guidChars[ offset++ ] = '0';
                guidChars[ offset++ ] = 'x';
            }
            guidChars[ offset++ ] = HexToChar( a >> 4 );
            guidChars[ offset++ ] = HexToChar( a );
            if ( hex ) {
                guidChars[ offset++ ] = ',';
                guidChars[ offset++ ] = '0';
                guidChars[ offset++ ] = 'x';
            }
            guidChars[ offset++ ] = HexToChar( b >> 4 );
            guidChars[ offset++ ] = HexToChar( b );
            return offset;
        }

        public override String ToString() {
            var strLength = 8;
            var guidChars = new char[strLength];

            var offset = 0;

            // [{|(]dddddddd[-]dddd[-]dddd[-]dddd[-]dddddddddddd[}|)] 

            offset = HexsToChars( guidChars, offset, this._d, this._e );
            if ( true ) {
                guidChars[ offset++ ] = '-';
            }
            offset = HexsToChars( guidChars, offset, this._f, this._g );
            offset = HexsToChars( guidChars, offset, this._h, this._i );
            offset = HexsToChars( guidChars, offset, this._j, this._k );

            return new String( value: guidChars, startIndex: 0, length: strLength );
        }

        //public Boolean Equals( Bitten other ) {
        //    return other._d == this._d && other._e == this._e && other._f == this._f && other._g == this._g && other._h == this._h && other._i == this._i && other._j == this._j && other._k == this._k;
        //}

        public override Boolean Equals( object obj ) {
            if ( ReferenceEquals( null, obj ) ) {
                return false;
            }
            if ( obj.GetType() != typeof ( Bitten ) ) {
                return false;
            }
            return this.Equals( ( Bitten ) obj );
        }

        public override int GetHashCode() {
            unchecked {
                // ReSharper disable NonReadonlyFieldInGetHashCode
                var result = this._d.GetHashCode();
                result = ( result*397 ) ^ this._e.GetHashCode();
                result = ( result*397 ) ^ this._f.GetHashCode();
                result = ( result*397 ) ^ this._g.GetHashCode();
                result = ( result*397 ) ^ this._h.GetHashCode();
                result = ( result*397 ) ^ this._i.GetHashCode();
                result = ( result*397 ) ^ this._j.GetHashCode();
                result = ( result*397 ) ^ this._k.GetHashCode();
                // ReSharper restore NonReadonlyFieldInGetHashCode
                return result;
            }
        }
    }
}
