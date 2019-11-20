// Copyright © Rick@AIBrain.org and Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
//
// This source code contained in "Frame.cs" belongs to Protiguous@Protiguous.com and
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
// Project: "Librainian", "Frame.cs" was last formatted by Protiguous on 2019/08/08 at 7:42 AM.

namespace Librainian.Graphics.Imaging {

    using System;
    using System.Linq;
    using System.Runtime.InteropServices;
    using JetBrains.Annotations;
    using Newtonsoft.Json;

    [JsonObject]
    [StructLayout( LayoutKind.Sequential )]
    public struct Frame : IEquatable<Frame> {

        public static readonly String DefaultHeader = "EFGFrame";

        /// <summary>Checksum of the page (guard against corruption).</summary>
        /// <remarks>
        ///     Should include the <see cref="LineCount" /> and <see cref="Delay" /> to prevent buffer
        ///     overflows and timeouts.
        /// </remarks>
        [JsonProperty]

        //[FieldOffset( sizeof( UInt64 ) * 1 )]
        public UInt64 Checksum;

        /// <summary>How many milliseconds to display this frame?</summary>
        [JsonProperty]

        //[FieldOffset( sizeof( UInt64 ) * 3 )]
        public UInt64 Delay;

        /// <summary></summary>
        [JsonProperty]

        //[FieldOffset( sizeof( UInt64 ) * 0 )]
        public UInt64 Identity;

        /// <summary>How many lines should be in this frame?</summary>
        [JsonProperty]

        //[FieldOffset( sizeof( UInt64 ) * 2 )]
        public UInt64 LineCount;

        /// <summary>An array of <see cref="Line" />.</summary>
        [JsonProperty]

        //[FieldOffset( sizeof( UInt64 ) * 4 )]
        public Line[] Lines;

        /// <summary>
        ///     Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <returns>
        ///     true if the current object is equal to the <paramref name="other" /> parameter;
        ///     otherwise, false.
        /// </returns>
        /// <param name="other">An object to compare with this object.</param>
        public Boolean Equals( Frame other ) => Equal( this, other );

        /// <summary>static comparision</summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        [Pure]
        public static Boolean Equal( Frame left, Frame right ) {

            //if ( ( left is null ) || ( right is null ) ) {
            //    return false;
            //}

            if ( left.Checksum != right.Checksum ) {
                return false;
            }

            if ( left.LineCount != right.LineCount ) {
                return false;
            }

            if ( left.Lines.LongLength != right.Lines.LongLength ) {
                return false;
            }

            return left.Lines.SequenceEqual( right.Lines );
        }
    }
}