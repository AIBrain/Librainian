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
// "Librainian2/Inches.cs" was last cleaned by Rick on 2014/08/08 at 2:28 PM
#endregion

namespace Librainian.Measurement.Length {
    using System;
    using System.Runtime.Serialization;

    [DataContract( IsReference = true )]
    public struct Inches {
        /// <summary>
        ///     One <see cref="Inches" /> .
        /// </summary>
        public static readonly Inches One = new Inches( inches: 1 );

        /// <summary>
        ///     Two <see cref="Inches" /> .
        /// </summary>
        public static readonly Inches Two = new Inches( inches: 2 );

        /// <summary>
        ///     About zero. :P
        /// </summary>
        public static readonly Inches MinValue = new Inches( inches: Decimal.MinValue );

        /// <summary>
        ///     About 584.9 million years.
        /// </summary>
        public static readonly Inches MaxValue = new Inches( inches: Decimal.MaxValue );

        [DataMember] public readonly Decimal Value;

        static Inches() {
            //Assert.That( One < Inch.One );
            //Assert.That( One < Foot.One );
        }

        public Inches( Decimal inches ) {
            this.Value = inches;
        }

        public Inches( Millimeters millimeters ) {
            var val = millimeters.Value/Extensions.MillimetersInSingleInch;
            this.Value = val < MinValue.Value ? MinValue.Value : ( val > MaxValue.Value ? MaxValue.Value : val );
        }

        public Inches( Centimeters centimeters ) {
            var val = centimeters.Value/Extensions.CentimetersinSingleMeter;
            this.Value = val < MinValue.Value ? MinValue.Value : ( val > MaxValue.Value ? MaxValue.Value : val );
        }

        public override int GetHashCode() {
            return this.Value.GetHashCode();
        }

        //public static Boolean operator <( Millimeter lhs, Second rhs ) { return lhs.Comparison( rhs ) < 0; }

        //public static Boolean operator >( Millimeter lhs, Second rhs ) { return lhs.Comparison( rhs ) > 0; }

        //public static Boolean operator <( Millimeter lhs, Minute rhs ) { return lhs.Comparison( rhs ) < 0; }

        //public static Boolean operator >( Millimeter lhs, Minute rhs ) { return lhs.Comparison( rhs ) > 0; } 

        //public static implicit operator TimeSpan( Inch milliseconds ) {
        //    return TimeSpan.FromMilliseconds( value: milliseconds.Value );
        //}
    }
}
