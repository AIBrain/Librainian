// Copyright © Rick@AIBrain.org and Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
//
// This source code contained in "Bitten.cs" belongs to Protiguous@Protiguous.com and
// Rick@AIBrain.org unless otherwise specified or the original license has
// been overwritten by formatting.
// (We try to avoid it from happening, but it does accidentally happen.)
//
// Any unmodified portions of source code gleaned from other projects still retain their original
// license and our thanks goes to those Authors. If you find your code in this source code, please
// let us know so we can properly attribute you and include the proper license and/or copyright.
//
// If you want to use any of our code, you must contact Protiguous@Protiguous.com or
// Sales@AIBrain.org for permission and a quote.
//
// Donations are accepted (for now) via
//     bitcoin:1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//     PayPal:Protiguous@Protiguous.com
//     (We're always looking into other solutions.. Any ideas?)
//
// =========================================================
// Disclaimer:  Usage of the source code or binaries is AS-IS.
//    No warranties are expressed, implied, or given.
//    We are NOT responsible for Anything You Do With Our Code.
//    We are NOT responsible for Anything You Do With Our Executables.
//    We are NOT responsible for Anything You Do With Your Computer.
// =========================================================
//
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
// For business inquiries, please contact me at Protiguous@Protiguous.com
//
// Our website can be found at "https://Protiguous.com/"
// Our software can be found at "https://Protiguous.Software/"
// Our GitHub address is "https://github.com/Protiguous".
// Feel free to browse any source code we make available.
//
// Project: "Librainian", "Bitten.cs" was last formatted by Protiguous on 2019/11/07 at 2:04 PM.

namespace Librainian.Misc {

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.InteropServices;
    using JetBrains.Annotations;
    using Newtonsoft.Json;

    /// <summary>
    ///     Wow, this an old old idea that didn't work. Please don't use it in production!
    ///     I just don't have the heart to delete it yet.
    /// </summary>
    [StructLayout( LayoutKind.Sequential )]
    [JsonObject]
    [ComVisible( true )]
    [Obsolete( "untested" )]
    public struct Bitten {

        public static readonly Bitten Empty;

        private readonly Byte _d;

        private readonly Byte _e;

        private readonly Byte _f;

        private readonly Byte _g;

        private readonly Byte _h;

        private readonly Byte _i;

        private readonly Byte _j;

        private readonly Byte _k;

        /// Creates a new guid from an array of bytes.
        public Bitten( [NotNull] IList<Byte> b ) {
            this._d = b[ 0 ];
            this._e = b[ 1 ];
            this._f = b[ 2 ];
            this._g = b[ 3 ];
            this._h = b[ 4 ];
            this._i = b[ 5 ];
            this._j = b[ 6 ];
            this._k = b[ 7 ];
        }

        public Bitten( Byte d, Byte e, Byte f, Byte g, Byte h, Byte i, Byte j, Byte k ) {
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

            Any = AllowParenthesis | AllowBraces | AllowDashes | AllowHexPrefix
        }

        // Creates a new guid based on the value in the String. The value is made up of hex digits
        // speared by the dash ("-"). The String may begin and end with brackets ("{", "}").
        //
        // The String must be of the form dddddddd-dddd-dddd-dddd-dddddddddddd. where d is a hex
        // digit. (That is 8 hex digits, followed by 4, then 4, then 4, then 12) such as: "CA761232-ED42-11CE-BACD-00AA0057B223"
        public Bitten( [NotNull] String g ) {
            if ( g is null ) {
                throw new ArgumentNullException( nameof( g ) );
            }

            this = Parse( g );
        }

        public static Bitten Parse( [NotNull] String input ) {
            if ( String.IsNullOrWhiteSpace( value: input ) ) {
                throw new ArgumentException( message: "Value cannot be null or whitespace.",  nameof( input ) );
            }

            return Guid.TryParse( input, out var result ) ? new Bitten( result.ToByteArray().Skip( 8 ).ToList() ) : Empty;
        }

        // Returns an unsigned byte array containing the GUID.
        [NotNull]
        public Byte[] ToByteArray() {
            var g = new Byte[ 8 ];

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

        public Boolean Equals( Bitten g ) => this == g;

        public static Boolean operator ==( Bitten a, Bitten b ) =>
            a._d == b._d && a._e == b._e && a._f == b._f && a._g == b._g && a._h == b._h && a._i == b._i && a._j == b._j && a._k == b._k;

        public static Boolean operator !=( Bitten a, Bitten b ) => !( a == b );

        private static Char HexToChar( Int32 a ) {
            a &= 0xf;

            return ( Char )( a > 9 ? a - 10 + 0x61 : a + 0x30 );
        }

        private static Int32 HexsToChars( [NotNull] IList<Char> guidChars, Int32 offset, Int32 a, Int32 b, Boolean hex = false ) {
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
            const Int32 strLength = 8;
            var guidChars = new Char[ strLength ];

            var offset = 0;

            // [{|(]dddddddd[-]dddd[-]dddd[-]dddd[-]dddddddddddd[}|)]
            offset = HexsToChars( guidChars, offset, this._d, this._e );

            if ( true ) {
                guidChars[ offset++ ] = '-';
            }

            offset = HexsToChars( guidChars, offset, this._f, this._g );
            offset = HexsToChars( guidChars, offset, this._h, this._i );

            // ReSharper disable once RedundantAssignment
            offset = HexsToChars( guidChars, offset, this._j, this._k );

            return new String( guidChars, startIndex: 0, strLength );
        }

        public override Boolean Equals( Object obj ) {
            if ( obj is null ) {
                return false;
            }

            if ( obj.GetType() != typeof( Bitten ) ) {
                return false;
            }

            return this.Equals( ( Bitten )obj );
        }

        public override Int32 GetHashCode() {
            unchecked {
                var result = this._d.GetHashCode();
                result = ( result * 397 ) ^ this._e.GetHashCode();
                result = ( result * 397 ) ^ this._f.GetHashCode();
                result = ( result * 397 ) ^ this._g.GetHashCode();
                result = ( result * 397 ) ^ this._h.GetHashCode();
                result = ( result * 397 ) ^ this._i.GetHashCode();
                result = ( result * 397 ) ^ this._j.GetHashCode();
                result = ( result * 397 ) ^ this._k.GetHashCode();

                return result;
            }
        }
    }
}