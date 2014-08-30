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
// "Librainian/Centimeters.cs" was last cleaned by Rick on 2014/08/11 at 12:39 AM
#endregion

namespace Librainian.Measurement.Length {
    using System;
    using System.Runtime.Serialization;

    [DataContract( IsReference = true )]
    [Serializable]
    public struct Centimeters {
        /// <summary>
        ///     One <see cref="Centimeters" /> .
        /// </summary>
        public static readonly Centimeters One = new Centimeters( centimeters: 1 );

        /// <summary>
        ///     Two <see cref="Centimeters" /> .
        /// </summary>
        public static readonly Centimeters Two = new Centimeters( centimeters: 2 );

        /// <summary>
        ///     About zero. :P
        /// </summary>
        public static readonly Centimeters MinValue = new Centimeters( centimeters:Decimal.MinValue );

        /// <summary>
        ///     About 584.9 million years.
        /// </summary>
        public static readonly Centimeters MaxValue = new Centimeters( centimeters:Decimal.MaxValue );

        [DataMember] public readonly  Decimal Value;

        static Centimeters() {
            //Assert.That( One < Inch.One );
            //Assert.That( One < Feet.One );
        }

        public Centimeters(Decimal centimeters ) {
            this.Value = centimeters;
        }

        public Centimeters( Millimeters millimeters ) {
            var val = millimeters.Value/Extensions.MillimetersInSingleCentimeter;
            this.Value = val < MinValue.Value ? MinValue.Value : ( val > MaxValue.Value ? MaxValue.Value : val );
        }

        public Centimeters( Meters meters ) {
            var val = meters.Value/Extensions.CentimetersinSingleMeter;
            this.Value = val < MinValue.Value ? MinValue.Value : ( val > MaxValue.Value ? MaxValue.Value : val );
        }

        public override int GetHashCode() {
            return this.Value.GetHashCode();
        }

        //public static Boolean operator <( Millimeter lhs, Second rhs ) { return lhs.Comparison( rhs ) < 0; }

        //public static Boolean operator >( Millimeter lhs, Second rhs ) { return lhs.Comparison( rhs ) > 0; }

        //public static Boolean operator <( Millimeter lhs, Minute rhs ) { return lhs.Comparison( rhs ) < 0; }

        //public static Boolean operator >( Millimeter lhs, Minute rhs ) { return lhs.Comparison( rhs ) > 0; } 

        //public static implicit operator TimeSpan( Centimeter milliseconds ) {
        //    return TimeSpan.FromMilliseconds( value: milliseconds.Value );
        //}
    }
}
