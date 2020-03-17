// Copyright © 2020 Protiguous. All Rights Reserved.
// 
// This entire copyright notice and license must be retained and must be kept visible in any binaries, libraries, repositories, and source code (directly or derived)
// from our binaries, libraries, projects, or solutions.
// 
// This source code contained in "Frame.cs" belongs to Protiguous@Protiguous.com unless otherwise specified or the original license has been overwritten
// by formatting. (We try to avoid it from happening, but it does accidentally happen.)
// 
// Any unmodified portions of source code gleaned from other projects still retain their original license and our thanks goes to those Authors.
// If you find your code in this source code, please let us know so we can properly attribute you and include the proper license and/or copyright.
// 
// If you want to use any of our code in a commercial project, you must contact Protiguous@Protiguous.com for permission and a quote.
// 
// Donations are accepted via bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2 and PayPal: Protiguous@Protiguous.com
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
// For business inquiries, please contact me at Protiguous@Protiguous.com.
// 
// Our website can be found at "https://Protiguous.com/"
// Our software can be found at "https://Protiguous.Software/"
// Our GitHub address is "https://github.com/Protiguous".
// Feel free to browse any source code we make available.
// 
// Project: "Librainian", File: "Frame.cs" was last formatted by Protiguous on 2020/03/16 at 4:45 PM.

namespace Librainian.Graphics.Imaging {

    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using JetBrains.Annotations;
    using Newtonsoft.Json;

    [JsonObject]
    public class Frame : IEquatable<Frame> {

        /// <summary>Indicates whether the current object is equal to another object of the same type.</summary>
        /// <returns>true if the current object is equal to the <paramref name="other" /> parameter; otherwise, false.</returns>
        /// <param name="other">An object to compare with this object.</param>
        public Boolean Equals( Frame other ) => Equals( left: this, right: other );

        /// <summary>Checksum of the page (guard against corruption).</summary>
        /// <remarks>Should include the <see cref="LineCount" /> and <see cref="Delay" /> to prevent buffer overflows and timeouts.</remarks>
        [JsonProperty]
        public UInt64 Checksum;

        /// <summary>How many milliseconds to display this frame?</summary>
        [JsonProperty]
        public UInt64 Delay;

        /// <summary></summary>
        [JsonProperty]
        public UInt64 Identity;

        /// <summary>How many lines should be in this frame?</summary>
        [JsonProperty]
        public UInt64 LineCount;

        /// <summary>An array of <see cref="Line" />.</summary>
        [JsonProperty]
        public Line[] Lines;

        public static readonly String DefaultHeader = "EFGFrame";

        /// <summary>static comparision</summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        [Pure]
        public static Boolean Equals( Frame left, Frame right ) {
            if ( ReferenceEquals( objA: left, objB: right ) ) {
                return true;
            }

            if ( left is null || right is null ) {
                return default;
            }

            if ( left.Checksum != right.Checksum ) {
                return default;
            }

            if ( left.LineCount != right.LineCount ) {
                return default;
            }

            if ( left.Lines.LongLength != right.Lines.LongLength ) {
                return default;
            }

            return left.Lines.SequenceEqual( second: right.Lines );
        }

        /// <summary>Returns a value that indicates whether two <see cref="T:Librainian.Graphics.Imaging.Frame" /> objects have different values.</summary>
        /// <param name="left">The first value to compare.</param>
        /// <param name="right">The second value to compare.</param>
        /// <returns>true if <paramref name="left" /> and <paramref name="right" /> are not equal; otherwise, false.</returns>
        public static Boolean operator !=( [CanBeNull] Frame left, [CanBeNull] Frame right ) => !left.Equals( other: right );

        /// <summary>Returns a value that indicates whether the values of two <see cref="T:Librainian.Graphics.Imaging.Frame" /> objects are equal.</summary>
        /// <param name="left">The first value to compare.</param>
        /// <param name="right">The second value to compare.</param>
        /// <returns>true if the <paramref name="left" /> and <paramref name="right" /> parameters have the same value; otherwise, false.</returns>
        public static Boolean operator ==( [CanBeNull] Frame left, [CanBeNull] Frame right ) => left.Equals( other: right );

        /// <summary>Indicates whether this instance and a specified object are equal.</summary>
        /// <param name="obj">The object to compare with the current instance.</param>
        /// <returns><see langword="true" /> if <paramref name="obj" /> and this instance are the same type and represent the same value; otherwise, <see langword="false" />.</returns>
        public override Boolean Equals( [CanBeNull] Object obj ) => Equals( objA: this, objB: obj is Frame frame ? frame : default );

        /// <summary>Returns the hash code for this instance.</summary>
        /// <returns>A 32-bit signed integer that is the hash code for this instance.</returns>
        [SuppressMessage( category: "ReSharper", checkId: "NonReadonlyMemberInGetHashCode" )]
        public override Int32 GetHashCode() {
            unchecked {
                var hashCode = this.Checksum.GetHashCode();
                hashCode = ( hashCode * 397 ) ^ this.Identity.GetHashCode();
                hashCode = ( hashCode * 397 ) ^ this.LineCount.GetHashCode();

                return hashCode;
            }
        }

    }

}