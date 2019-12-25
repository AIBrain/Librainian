// Copyright © Rick@AIBrain.org and Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
//
// This source code contained in "Milliseconds.cs" belongs to Protiguous@Protiguous.com and
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
// Project: "Librainian", "Milliseconds.cs" was last formatted by Protiguous on 2019/11/20 at 5:43 AM.

namespace LibrainianCore.Measurement.Time {

    using System;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;
    using System.Numerics;
    using Extensions;
    using Maths;
    using Parsing;

    [DebuggerDisplay( "{" + nameof( ToString ) + "(),nq}" )]
    [JsonObject]
    [Immutable]
    public class Milliseconds : IComparable<Milliseconds>, IQuantityOfTime {

        /// <summary>1000</summary>
        public const UInt16 InOneSecond = 1000;

        [NotNull]
        public static Milliseconds Default { get; } = new Milliseconds( default );

        /// <summary>Ten <see cref="Milliseconds" /> s.</summary>
        [NotNull]
        public static Milliseconds Fifteen { get; } = new Milliseconds( 15 );

        /// <summary>Five <see cref="Milliseconds" /> s.</summary>
        [NotNull]
        public static Milliseconds Five { get; } = new Milliseconds( 5 );

        /// <summary>Five Hundred <see cref="Milliseconds" /> s.</summary>
        [NotNull]
        public static Milliseconds FiveHundred { get; } = new Milliseconds( 500 );

        /// <summary>111. 1 Hertz (9 <see cref="Milliseconds" />).</summary>
        [NotNull]
        public static Milliseconds Hertz111 { get; } = new Milliseconds( 9 );

        /// <summary>97 <see cref="Milliseconds" /> s.</summary>
        [NotNull]
        public static Milliseconds NinetySeven { get; } = new Milliseconds( 97 );

        /// <summary>One <see cref="Milliseconds" />.</summary>
        [NotNull]
        public static Milliseconds One { get; } = new Milliseconds( 1 );

        /// <summary>One <see cref="Milliseconds" /> s.</summary>
        [NotNull]
        public static Milliseconds OneHundred { get; } = new Milliseconds( 100 );

        /// <summary>One Thousand Nine <see cref="Milliseconds" /> (Prime).</summary>
        [NotNull]
        public static Milliseconds OneThousandNine { get; } = new Milliseconds( 1009 );

        /// <summary>Sixteen <see cref="Milliseconds" />.</summary>
        [NotNull]
        public static Milliseconds Sixteen { get; } = new Milliseconds( 16 );

        /// <summary>Ten <see cref="Milliseconds" /> s.</summary>
        [NotNull]
        public static Milliseconds Ten { get; } = new Milliseconds( 10 );

        /// <summary>Three <see cref="Milliseconds" /> s.</summary>
        [NotNull]
        public static Milliseconds Three { get; } = new Milliseconds( 3 );

        /// <summary>Three Three Three <see cref="Milliseconds" />.</summary>
        public static Milliseconds ThreeHundredThirtyThree { get; } = new Milliseconds( 333 );

        /// <summary>Two <see cref="Milliseconds" /> s.</summary>
        [NotNull]
        public static Milliseconds Two { get; } = new Milliseconds( 2 );

        /// <summary>Two Hundred <see cref="Milliseconds" />.</summary>
        [NotNull]
        public static Milliseconds TwoHundred { get; } = new Milliseconds( 200 );

        /// <summary>Two Hundred Eleven <see cref="Milliseconds" /> (Prime).</summary>
        [NotNull]
        public static Milliseconds TwoHundredEleven { get; } = new Milliseconds( 211 );

        /// <summary>Two Thousand Three <see cref="Milliseconds" /> (Prime).</summary>
        [NotNull]
        public static Milliseconds TwoThousandThree { get; } = new Milliseconds( 2003 );

        //faster WPM than a female (~240wpm)
        /// <summary>Zero <see cref="Milliseconds" />.</summary>
        [NotNull]
        public static Milliseconds Zero { get; } = new Milliseconds( 0 );

        [JsonProperty]
        public Rational Value { get; }

        //faster WPM than a female (~240wpm)
        public Milliseconds( Decimal value ) => this.Value = ( Rational )value;

        public Milliseconds( Rational value ) => this.Value = value;

        public Milliseconds( Int64 value ) => this.Value = value;

        public Milliseconds( BigInteger value ) => this.Value = value;

        public Milliseconds( Double value ) => this.Value = ( Rational )value;

        [NotNull]
        public static Milliseconds Combine( [NotNull] Milliseconds left, Rational milliseconds ) {
            if ( left is null ) {
                throw new ArgumentNullException( paramName: nameof( left ) );
            }

            return new Milliseconds( left.Value + milliseconds );
        }

        [NotNull]
        public static Milliseconds Combine( [NotNull] Milliseconds left, BigInteger milliseconds ) {
            if ( left is null ) {
                throw new ArgumentNullException( paramName: nameof( left ) );
            }

            return new Milliseconds( left.Value + milliseconds );
        }

        /// <summary>
        ///     <para>static equality test</para>
        /// </summary>
        /// <param name="left"> </param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Boolean Equals( [CanBeNull] Milliseconds left, [CanBeNull] Milliseconds right ) {
            if ( left is null && right is null ) {
                return true;
            }

            if ( left is null || right is null ) {
                return false;
            }

            return left.Value == right.Value;
        }

        public static explicit operator Double( [NotNull] Milliseconds milliseconds ) {
            if ( milliseconds is null ) {
                throw new ArgumentNullException( paramName: nameof( milliseconds ) );
            }

            return ( Double )milliseconds.Value;
        }

        /// <summary>Implicitly convert the number of <paramref name="milliseconds" /> to <see cref="Microseconds" />.</summary>
        /// <param name="milliseconds"></param>
        /// <returns></returns>
        [CanBeNull]
        public static implicit operator Microseconds( [NotNull] Milliseconds milliseconds ) {
            if ( milliseconds is null ) {
                throw new ArgumentNullException( paramName: nameof( milliseconds ) );
            }

            return milliseconds.ToMicroseconds();
        }

        public static implicit operator Rational( [NotNull] Milliseconds milliseconds ) {
            if ( milliseconds is null ) {
                throw new ArgumentNullException( paramName: nameof( milliseconds ) );
            }

            return milliseconds.Value;
        }

        [NotNull]
        public static implicit operator Seconds( [NotNull] Milliseconds milliseconds ) {
            if ( milliseconds is null ) {
                throw new ArgumentNullException( paramName: nameof( milliseconds ) );
            }

            return milliseconds.ToSeconds();
        }

        [NotNull]
        public static implicit operator SpanOfTime( [NotNull] Milliseconds milliseconds ) {
            if ( milliseconds is null ) {
                throw new ArgumentNullException( paramName: nameof( milliseconds ) );
            }

            return new SpanOfTime( milliseconds: milliseconds );
        }

        public static implicit operator TimeSpan( [NotNull] Milliseconds milliseconds ) {
            if ( milliseconds is null ) {
                throw new ArgumentNullException( paramName: nameof( milliseconds ) );
            }

            return TimeSpan.FromMilliseconds( ( Double )milliseconds.Value );
        }

        [NotNull]
        public static Milliseconds operator -( [NotNull] Milliseconds milliseconds ) {
            if ( milliseconds is null ) {
                throw new ArgumentNullException( paramName: nameof( milliseconds ) );
            }

            return new Milliseconds( milliseconds.Value * -1 );
        }

        [NotNull]
        public static Milliseconds operator -( [NotNull] Milliseconds left, [NotNull] Milliseconds right ) {
            if ( left is null ) {
                throw new ArgumentNullException( paramName: nameof( left ) );
            }

            if ( right is null ) {
                throw new ArgumentNullException( paramName: nameof( right ) );
            }

            return Combine( left, -right.Value );
        }

        [NotNull]
        public static Milliseconds operator -( [NotNull] Milliseconds left, Decimal milliseconds ) {
            if ( left is null ) {
                throw new ArgumentNullException( paramName: nameof( left ) );
            }

            return Combine( left, ( Rational )( -milliseconds ) );
        }

        public static Boolean operator !=( [NotNull] Milliseconds left, [NotNull] Milliseconds right ) {
            if ( left is null ) {
                throw new ArgumentNullException( paramName: nameof( left ) );
            }

            if ( right is null ) {
                throw new ArgumentNullException( paramName: nameof( right ) );
            }

            return !Equals( left, right );
        }

        [NotNull]
        public static Milliseconds operator +( [NotNull] Milliseconds left, [NotNull] Milliseconds right ) {
            if ( left is null ) {
                throw new ArgumentNullException( paramName: nameof( left ) );
            }

            if ( right is null ) {
                throw new ArgumentNullException( paramName: nameof( right ) );
            }

            return Combine( left, right.Value );
        }

        [NotNull]
        public static Milliseconds operator +( [NotNull] Milliseconds left, Decimal milliseconds ) {
            if ( left is null ) {
                throw new ArgumentNullException( paramName: nameof( left ) );
            }

            return Combine( left, ( Rational )milliseconds );
        }

        [NotNull]
        public static Milliseconds operator +( [NotNull] Milliseconds left, BigInteger milliseconds ) {
            if ( left is null ) {
                throw new ArgumentNullException( paramName: nameof( left ) );
            }

            return Combine( left, milliseconds );
        }

        public static Boolean operator <( [NotNull] Milliseconds left, [NotNull] Milliseconds right ) {
            if ( left is null ) {
                throw new ArgumentNullException( paramName: nameof( left ) );
            }

            if ( right is null ) {
                throw new ArgumentNullException( paramName: nameof( right ) );
            }

            return left.Value < right.Value;
        }

        public static Boolean operator <( [NotNull] Milliseconds left, [NotNull] Seconds right ) {
            if ( left is null ) {
                throw new ArgumentNullException( paramName: nameof( left ) );
            }

            if ( right is null ) {
                throw new ArgumentNullException( paramName: nameof( right ) );
            }

            return ( Seconds )left < right;
        }

        public static Boolean operator ==( [CanBeNull] Milliseconds left, [CanBeNull] Milliseconds right ) => Equals( left, right );

        public static Boolean operator >( [NotNull] Milliseconds left, [NotNull] Milliseconds right ) {
            if ( left is null ) {
                throw new ArgumentNullException( paramName: nameof( left ) );
            }

            if ( right is null ) {
                throw new ArgumentNullException( paramName: nameof( right ) );
            }

            return left.Value > right.Value;
        }

        [Pure]
        public static Boolean operator >( [NotNull] Milliseconds left, [NotNull] Seconds right ) {
            if ( left is null ) {
                throw new ArgumentNullException( paramName: nameof( left ) );
            }

            if ( right is null ) {
                throw new ArgumentNullException( paramName: nameof( right ) );
            }

            return ( Seconds )left > right;
        }

        public Int32 CompareTo( [NotNull] Milliseconds other ) {
            if ( other is null ) {
                throw new ArgumentNullException( paramName: nameof( other ) );
            }

            return this.Value.CompareTo( other.Value );
        }

        public Boolean Equals( [CanBeNull] Milliseconds other ) => Equals( this, other );

        public override Boolean Equals( [CanBeNull] Object obj ) => Equals( this, obj as Milliseconds );

        public override Int32 GetHashCode() => this.Value.GetHashCode();

        [NotNull]
        public Microseconds ToMicroseconds() => new Microseconds( this.Value * Microseconds.InOneMillisecond );

        [NotNull]
        public PlanckTimes ToPlanckTimes() => new PlanckTimes( ( Rational )PlanckTimes.InOneMillisecond * this.Value );

        [NotNull]
        public Seconds ToSeconds() => new Seconds( this.Value / InOneSecond );

        public override String ToString() {
            if ( this.Value > MathConstants.DecimalMaxValueAsBigRational ) {
                var whole = this.Value.WholePart;

                return $"{whole} {whole.PluralOf( "millisecond" )}";
            }

            var dec = ( Decimal )this.Value;

            return $"{dec} {dec.PluralOf( "millisecond" )}";
        }

        public TimeSpan ToTimeSpan() => this;
    }
}