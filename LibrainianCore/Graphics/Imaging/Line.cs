// Copyright © 2020 Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible in any binaries, libraries, repositories, and source code (directly or derived)
// from our binaries, libraries, projects, or solutions.
//
// This source code contained in "Line.cs" belongs to Protiguous@Protiguous.com unless otherwise specified or the original license has been overwritten
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
// Project: "LibrainianCore", File: "Line.cs" was last formatted by Protiguous on 2020/03/16 at 3:05 PM.

namespace Librainian.Graphics.Imaging {

    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.InteropServices;
    using System.Threading.Tasks;
    using JetBrains.Annotations;
    using Newtonsoft.Json;

    /// <summary>A horizontal line of <see cref="Pixel" />.</summary>
    [JsonObject]
    [StructLayout( layoutKind: LayoutKind.Sequential )]
    public class Line : IEquatable<Line>, IEnumerable<Pixel>, IEqualityComparer<Line> {

        public IEnumerator<Pixel> GetEnumerator() => this.Pixels.AsEnumerable().GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

        Boolean IEqualityComparer<Line>.Equals( Line x, Line y ) => Equals( left: x, right: y );

        /// <summary>Returns a hash code for the specified object.</summary>
        /// <returns>A hash code for the specified object.</returns>
        /// <param name="obj">The <see cref="Object" /> for which a hash code is to be returned.</param>
        /// <exception cref="ArgumentNullException">The type of <paramref name="obj" /> is a reference type and <paramref name="obj" /> is null.</exception>
        public Int32 GetHashCode( Line obj ) => this.Pixels.GetHashCode();

        /// <summary>Indicates whether the current object is equal to another object of the same type.</summary>
        /// <returns>true if the current object is equal to the <paramref name="other" /> parameter; otherwise, false.</returns>
        /// <param name="other">An object to compare with this object.</param>
        public Boolean Equals( Line other ) => Equals( left: this, right: other );

        /// <summary>Determines whether the specified object is equal to the current object.</summary>
        /// <param name="obj">The object to compare with the current object.</param>
        /// <returns><see langword="true" /> if the specified object  is equal to the current object; otherwise, <see langword="false" />.</returns>
        public override Boolean Equals( Object obj ) => Equals( left: this, right: obj as Line );

        /// <summary>Serves as the default hash function.</summary>
        /// <returns>A hash code for the current object.</returns>

        // ReSharper disable 3 NonReadonlyMemberInGetHashCode
        public override Int32 GetHashCode() => (this.Count, this.Pixels, this._checksum).GetHashCode();

        /// <summary>Returns a value that indicates whether the values of two <see cref="Line" /> objects are equal.</summary>
        /// <param name="left">The first value to compare.</param>
        /// <param name="right">The second value to compare.</param>
        /// <returns>true if the <paramref name="left" /> and <paramref name="right" /> parameters have the same value; otherwise, false.</returns>
        public static Boolean operator ==( [CanBeNull] Line left, [CanBeNull] Line right ) => Equals( left: left, right: right );

        /// <summary>Returns a value that indicates whether two <see cref="Line" /> objects have different values.</summary>
        /// <param name="left">The first value to compare.</param>
        /// <param name="right">The second value to compare.</param>
        /// <returns>true if <paramref name="left" /> and <paramref name="right" /> are not equal; otherwise, false.</returns>
        public static Boolean operator !=( [CanBeNull] Line left, [CanBeNull] Line right ) => !Equals( left: left, right: right );

        /// <summary>How many pixels should be in this line?</summary>
        [JsonProperty]
        public UInt64 Count;

        /// <summary>An array of pixels</summary>
        /// <remarks>I'd prefer a list instead of an array.</remarks>
        [JsonProperty]
        [NotNull]
        public Pixel[] Pixels;

        /// <summary>Returns the zero-based <see cref="Pixel" /> or null if not found.</summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public Pixel? this[ UInt64 index ] {
            get {
                if ( index <= this.Count ) {
                    return this.Pixels[ index ];
                }

                return null;
            }

            set {

                if ( value.HasValue && index <= this.Count ) {
                    this.Pixels[ index ] = value.Value;
                }
            }
        }

        /// <summary>Construct a <see cref="Line" /> from an array of <see cref="Pixel" />.</summary>
        /// <param name="pixels"></param>
        public Line( [NotNull] Pixel[] pixels ) {
            if ( pixels is null ) {
                throw new ArgumentNullException( paramName: nameof( pixels ) );
            }

            this.Pixels = pixels.ToArray();
            this.Count = ( UInt64 )this.Pixels.LongLength;
        }

        private UInt64? _checksum;

        /// <summary>Checksum of the pixels (to guard against corruption).</summary>
        /// <remarks>Should include the <see cref="Count" /> to prevent buffer overflows.</remarks>
        public async Task<UInt64> Checksum() {
            if ( this._checksum is null ) {
                this._checksum = await this.CalculateChecksumAsync().ConfigureAwait( continueOnCapturedContext: false );
            }

            var checksum = this._checksum;

            if ( checksum.HasValue ) {
                return checksum.Value;
            }

            throw new InvalidOperationException();
        }

        [NotNull]
        private Task<UInt64> CalculateChecksumAsync() =>
            Task.Run( function: () => {
                var checksum = ( UInt64 )0;

                foreach ( var pixel in this.Pixels ) {
                    unchecked {
                        checksum += ( UInt64 )pixel.GetHashCode();
                    }
                }

                return checksum;
            } );

        /// <summary>Static comparison type.</summary>
        /// <param name="left"> </param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Boolean Equals( [CanBeNull] Line left, [CanBeNull] Line right ) {
            if ( ReferenceEquals( objA: left, objB: right ) ) {
                return true;
            }

            if ( left is null || right is null ) {
                return default;
            }

            if ( left.Checksum().Result != right.Checksum().Result || left.Count != right.Count ) {
                return default; //TODO ugh... .Result
            }

            return left.Pixels.SequenceEqual( second: right.Pixels );
        }
    }
}