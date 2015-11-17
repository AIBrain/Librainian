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
// "Librainian/Line.cs" was last cleaned by Rick on 2015/06/12 at 2:55 PM

namespace Librainian.Graphics.Imaging {

    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.InteropServices;
    using System.Runtime.Serialization;
    using System.Threading.Tasks;
    using JetBrains.Annotations;

    /// <summary>A horizontal line of <see cref="Pixel" />.</summary>
    [DataContract]
    [Serializable]
    [StructLayout( LayoutKind.Explicit )]
    public class Line : IEquatable<Line>, IEnumerable<Pixel>, IEqualityComparer<Line> {

        /// <summary>Construct a <see cref="Line" /> from an array of <see cref="Pixel" />.</summary>
        /// <param name="pixels"></param>
        public Line([NotNull] Pixel[] pixels) {
            if ( pixels == null ) {
                throw new ArgumentNullException( nameof( pixels ) );
            }
            this.Pixels = pixels.ToArray();
            this.Count = ( UInt64 )this.Pixels.LongLength;
            this.Checksum = CalculateChecksumAsync( Pixels ).Result;
        }

        public static async Task<UInt64> CalculateChecksumAsync(IEnumerable<Pixel> pixels) => await Task.Run( () => {
            var checksum = UInt64.MinValue;
            foreach ( var pixel in pixels ) {
                unchecked {
                    checksum = ( checksum + ( UInt64 )pixel.GetHashCode() ) / 2;
                }
            }
            return checksum;
        } );

        /// <summary>Checksum of the pixels (to guard against corruption).</summary>
        /// <remarks>Should include the <see cref="Count" /> to prevent buffer overflows.</remarks>
        [DataMember]
        [FieldOffset( 0 )]
        public UInt64 Checksum;

        /// <summary>How many pixels should be in this line?</summary>
        [DataMember]
        [FieldOffset( sizeof(UInt64) * 1 )]
        public UInt64 Count;

        /// <summary>An array of pixels</summary>
        /// <remarks>I'd prefer a list instead of an array.</remarks>
        [DataMember]
        [FieldOffset( sizeof(UInt64) * 2 )]
        [NotNull]
        public Pixel[] Pixels;

        /// <summary>Returns the zero-based <see cref="Pixel" /> or null if not found.</summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public Pixel? this[ UInt64 index ] {
            get {
                var pixels = this.Pixels;
                if ( index <= this.Count ) {
                    return pixels[ index ];
                }
                return null;
            }

            set {
                var pixels = this.Pixels;
                if ( value.HasValue && ( index <= this.Count ) ) {
                    pixels[ index ] = value.Value;
                }
            }
        }

        /// <summary>Static comparison type.</summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Boolean Equal([CanBeNull] Line left, [CanBeNull] Line right) {
            if ( ( left == null ) || ( right == null ) ) {
                return false;
            }

            if ( left.Checksum != right.Checksum ) {
                return false;
            }
            if ( left.Count != right.Count ) {
                return false;
            }
            return left.Pixels.SequenceEqual( right.Pixels );
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other" /> parameter;
        /// otherwise, false.
        /// </returns>
        /// <param name="other">An object to compare with this object.</param>
        public Boolean Equals(Line other) => Equal( this, other );

        /// <summary>Returns an enumerator that iterates through the collection.</summary>
        /// <returns>
        /// A <see cref="T:System.Collections.Generic.IEnumerator`1" /> that can be used to iterate
        /// through the collection.
        /// </returns>
        public IEnumerator<Pixel> GetEnumerator() => this.Pixels.AsEnumerable().GetEnumerator();

        /// <summary>Returns an enumerator that iterates through a collection.</summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator" /> object that can be used to iterate
        /// through the collection.
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        /// <summary>Determines whether the specified objects are equal.</summary>
        /// <returns>true if the specified objects are equal; otherwise, false.</returns>
        /// <param name="x">The first object of type <paramref name="T" /> to compare.</param>
        /// <param name="y">The second object of type <paramref name="T" /> to compare.</param>
        public Boolean Equals(Line x, Line y) => Equal( x, y );

        /// <summary>Returns a hash code for the specified object.</summary>
        /// <returns>A hash code for the specified object.</returns>
        /// <param name="obj">
        /// The <see cref="T:System.Object" /> for which a hash code is to be returned.
        /// </param>
        /// <exception cref="T:System.ArgumentNullException">
        /// The type of <paramref name="obj" /> is a reference type and <paramref name="obj" /> is null.
        /// </exception>
        public Int32 GetHashCode(Line obj) => this.Pixels.GetHashCode();
    }
}