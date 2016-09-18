// Copyright 2016 Rick@AIBrain.org.
//
// This notice must be kept visible in the source.
//
// This section of source code belongs to Rick@AIBrain.Org unless otherwise specified, or the
// original license has been overwritten by the automatic formatting of this code. Any unmodified
// sections of source code borrowed from other projects retain their original license and thanks
// goes to the Authors.
//
// Donations and royalties can be paid via
//  PayPal: paypal@aibrain.org
//  bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//  litecoin: LeUxdU2w3o6pLZGVys5xpDZvvo8DUrjBp9
//
// Usage of the source code or compiled binaries is AS-IS. I am not responsible for Anything You Do.
//
// Contact me by email if you have any questions or helpful criticism.
//
// "Librainian/Frame.cs" was last cleaned by Rick on 2016/06/18 at 10:51 PM

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
            //if ( ( left == null ) || ( right == null ) ) {
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