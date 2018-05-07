// Copyright 2015 Protiguous.
//
// This notice must be kept visible in the source.
//
// This section of source code belongs to Protiguous@Protiguous.com unless otherwise specified, or the original license has been overwritten by the automatic formatting of this code.
// Any unmodified sections of source code borrowed from other projects retain their original license and thanks goes to the Authors.
//
// Donations and royalties can be paid via
// PayPal: paypal@Protiguous.com
// bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
// 
//
// Usage of the source code or compiled binaries is AS-IS.
// I am not responsible for Anything You Do.
//
// Contact me by email if you have any questions or helpful criticism.
//
// "Fantasy Core/Compass.cs" was last cleaned by Protiguous on 2016/01/03 at 12:25 AM

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

        /// <summary>
        /// Init with a random direction
        /// </summary>
        public Compass() : this( Randem.NextSingle( Minimum, Maximum ) ) {
        }

        /// <summary>
        /// ctor with <paramref name="degrees"/>.
        /// </summary>
        /// <param name="degrees"></param>
        public Compass( Single degrees ) => this.Degrees = degrees;

	    public Single Degrees {
            get => this._degrees;

	        set {
                if ( Single.IsNaN( value ) ) {
                    throw new ArgumentOutOfRangeException( nameof( value ), "Value is out of range 0 to 360" );
                }
                if ( Single.IsInfinity( value ) ) {
                    throw new ArgumentOutOfRangeException( nameof( value ), "Value is out of range 0 to 360" );
                }

                while ( value < Minimum ) {
                    value += Maximum;
                }
                while ( value > Maximum ) {
                    value -= Maximum;
                }

                value.Should()
                     .BeGreaterOrEqualTo( Minimum );
                value.Should()
                     .BeLessOrEqualTo( Maximum );

                this._degrees = value;
            }
        }

        public Boolean RotateLeft( Single byAmount = ( Single )Math.PI ) {
            if ( Single.IsNaN( byAmount ) ) {
                return false;
            }
            if ( Single.IsInfinity( byAmount ) ) {
                return false;
            }

            //TODO would a Lerp here make turning smoother?
            this.Degrees -= byAmount;

            return true;
        }

        /// <summary>Clockwise from a top-down view.</summary>
        /// <param name="byAmount"></param>
        /// <returns></returns>
        public Boolean RotateRight( Single byAmount = ( Single )Math.PI ) {
            if ( Single.IsNaN( byAmount ) ) {
                return false;
            }
            if ( Single.IsInfinity( byAmount ) ) {
                return false;
            }

            //TODO would a Lerp here make turning smoother?
            this.Degrees += byAmount;

            return true;
        }
    }
}