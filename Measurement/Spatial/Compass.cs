// Copyright © 1995-2018 to Rick@AIBrain.org and Protiguous.
// All Rights Reserved.
//
// This ENTIRE copyright notice and file header MUST BE KEPT
// VISIBLE in any source code derived from or used from our
// libraries and projects.
//
// =========================================================
// This section of source code, "Compass.cs",
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
// "Librainian/Librainian/Compass.cs" was last cleaned by Protiguous on 2018/05/15 at 10:47 PM.

namespace Librainian.Measurement.Spatial {

    using System;
    using System.Runtime.Serialization;
    using FluentAssertions;
    using Maths;

    /// <summary>small number, constrained to between 0 and 360, with wrapping</summary>
    [DataContract( IsReference = false )]
    public class Compass {

        private volatile Single _degrees;
        public const Single Maximum = 360;

        public const Single Minimum = 0;

        public Single Degrees {
            get => this._degrees;

            set {
                if ( Single.IsNaN( value ) ) { throw new ArgumentOutOfRangeException( nameof( value ), "Value is out of range 0 to 360" ); }

                if ( Single.IsInfinity( value ) ) { throw new ArgumentOutOfRangeException( nameof( value ), "Value is out of range 0 to 360" ); }

                while ( value < Minimum ) { value += Maximum; }

                while ( value > Maximum ) { value -= Maximum; }

                value.Should().BeGreaterOrEqualTo( Minimum );
                value.Should().BeLessOrEqualTo( Maximum );

                this._degrees = value;
            }
        }

        /// <summary>
        ///     Init with a random direction
        /// </summary>
        public Compass() : this( Randem.NextSingle( Minimum, Maximum ) ) { }

        /// <summary>
        ///     ctor with <paramref name="degrees" />.
        /// </summary>
        /// <param name="degrees"></param>
        public Compass( Single degrees ) => this.Degrees = degrees;

        public Boolean RotateLeft( Single byAmount = ( Single )Math.PI ) {
            if ( Single.IsNaN( byAmount ) ) { return false; }

            if ( Single.IsInfinity( byAmount ) ) { return false; }

            //TODO would a Lerp here make turning smoother?
            this.Degrees -= byAmount;

            return true;
        }

        /// <summary>Clockwise from a top-down view.</summary>
        /// <param name="byAmount"></param>
        /// <returns></returns>
        public Boolean RotateRight( Single byAmount = ( Single )Math.PI ) {
            if ( Single.IsNaN( byAmount ) ) { return false; }

            if ( Single.IsInfinity( byAmount ) ) { return false; }

            //TODO would a Lerp here make turning smoother?
            this.Degrees += byAmount;

            return true;
        }
    }
}