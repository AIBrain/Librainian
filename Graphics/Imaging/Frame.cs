// Copyright © 1995-2018 to Rick@AIBrain.org and Protiguous.
// All Rights Reserved.
//
// This ENTIRE copyright notice and file header MUST BE KEPT
// VISIBLE in any source code derived from or used from our
// libraries and projects.
//
// =========================================================
// This section of source code, "Frame.cs",
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
// "Librainian/Librainian/Frame.cs" was last cleaned by Protiguous on 2018/05/15 at 10:43 PM.

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

            if ( left.Checksum != right.Checksum ) { return false; }

            if ( left.LineCount != right.LineCount ) { return false; }

            if ( left.Lines.LongLength != right.Lines.LongLength ) { return false; }

            return left.Lines.SequenceEqual( right.Lines );
        }
    }
}