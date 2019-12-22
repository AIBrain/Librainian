// Copyright © Rick@AIBrain.org and Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
//
// This source code contained in "Attoseconds.cs" belongs to Protiguous@Protiguous.com and
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
// Project: "Librainian", "Attoseconds.cs" was last formatted by Protiguous on 2019/12/04 at 10:27 PM.

namespace Librainian.Measurement.Time {

    using System;
    using System.Diagnostics;
    using System.Numerics;
    using Extensions;
    using JetBrains.Annotations;
    using Maths;
    using Newtonsoft.Json;
    using Parsing;
    using Rationals;

    [DebuggerDisplay( "{" + nameof( ToString ) + "(),nq}" )]
    [JsonObject]
    [Immutable]
    public class Attoseconds : IQuantityOfTime {

        /// <summary>1000</summary>
        /// <see cref="Femtoseconds" />
        public const UInt16 InOneFemtosecond = 1000;

        /// <summary>Ten <see cref="Attoseconds" /> s.</summary>
        public static Attoseconds Fifteen { get; } = new Attoseconds( 15 );

        /// <summary>Five <see cref="Attoseconds" /> s.</summary>
        public static Attoseconds Five { get; } = new Attoseconds( 5 );

        /// <summary>Five Hundred <see cref="Attoseconds" /> s.</summary>
        public static Attoseconds FiveHundred { get; } = new Attoseconds( 500 );

        /// <summary>111. 1 Hertz <see cref="Attoseconds" />.</summary>
        public static Attoseconds Hertz111 { get; } = new Attoseconds( 9 );

        /// <summary>One <see cref="Attoseconds" />.</summary>
        /// <remarks>the time it takes for light to travel the length of three hydrogen atoms</remarks>
        public static Attoseconds One { get; } = new Attoseconds( 1 );

        /// <summary><see cref="OneHundred" /><see cref="Attoseconds" />.</summary>
        /// <remarks>fastest ever view of molecular motion</remarks>
        public static Attoseconds OneHundred { get; } = new Attoseconds( 100 );

        /// <summary>One Thousand Nine <see cref="Attoseconds" /> (Prime).</summary>
        public static Attoseconds OneThousandNine { get; } = new Attoseconds( 1009 );

        /// <summary>Sixteen <see cref="Attoseconds" />.</summary>
        public static Attoseconds Sixteen { get; } = new Attoseconds( 16 );

        /// <summary><see cref="SixtySeven" /><see cref="Attoseconds" />.</summary>
        /// <remarks>the shortest pulses of laser light yet created</remarks>
        public static Attoseconds SixtySeven { get; } = new Attoseconds( 67 );

        /// <summary>Ten <see cref="Attoseconds" /> s.</summary>
        public static Attoseconds Ten { get; } = new Attoseconds( 10 );

        /// <summary>Three <see cref="Attoseconds" /> s.</summary>
        public static Attoseconds Three { get; } = new Attoseconds( 3 );

        /// <summary>Three Three Three <see cref="Attoseconds" />.</summary>
        public static Attoseconds ThreeHundredThirtyThree { get; } = new Attoseconds( 333 );

        /// <summary><see cref="ThreeHundredTwenty" /><see cref="Attoseconds" />.</summary>
        /// <remarks>estimated time it takes electrons to transfer between atoms</remarks>
        public static Attoseconds ThreeHundredTwenty { get; } = new Attoseconds( 320 );

        /// <summary><see cref="Twelve" /><see cref="Attoseconds" />.</summary>
        /// <remarks>record for shortest time interval measured as of 12 May 2010</remarks>
        public static Attoseconds Twelve { get; } = new Attoseconds( 12 );

        /// <summary><see cref="TwentyFour" /><see cref="Attoseconds" />.</summary>
        /// <remarks>the atomic unit of time</remarks>
        public static Attoseconds TwentyFour { get; } = new Attoseconds( 24 );

        /// <summary>Two <see cref="Attoseconds" /> s.</summary>
        public static Attoseconds Two { get; } = new Attoseconds( 2 );

        /// <summary><see cref="TwoHundred" /><see cref="Attoseconds" />.</summary>
        /// <remarks>(approximately) – half-life of beryllium-8, maximum time available for the triple-alpha process for the synthesis of carbon and heavier elements in stars</remarks>
        public static Attoseconds TwoHundred { get; } = new Attoseconds( 200 );

        /// <summary>Two Hundred Eleven <see cref="Attoseconds" /> (Prime).</summary>
        public static Attoseconds TwoHundredEleven { get; } = new Attoseconds( 211 );

        /// <summary>Two Thousand Three <see cref="Attoseconds" /> (Prime).</summary>
        public static Attoseconds TwoThousandThree { get; } = new Attoseconds( 2003 );

        /// <summary>Zero <see cref="Attoseconds" />.</summary>
        public static Attoseconds Zero { get; } = new Attoseconds( 0 );

        [JsonProperty]
        public Rational Value { get; }

        public Attoseconds( Decimal value ) => this.Value = ( Rational )value;

        public Attoseconds( Rational value ) => this.Value = value;

        public Attoseconds( Int64 value ) => this.Value = value;

        public Attoseconds( BigInteger value ) => this.Value = value;

        [NotNull]
        public static Attoseconds Combine( [NotNull] Attoseconds left, [NotNull] Attoseconds right ) {
            if ( left is null ) {
                throw new ArgumentNullException( paramName: nameof( left ) );
            }

            if ( right is null ) {
                throw new ArgumentNullException( paramName: nameof( right ) );
            }

            return new Attoseconds( left.Value + right.Value );
        }

        [NotNull]
        public static Attoseconds Combine( [NotNull] Attoseconds left, Decimal attoseconds ) {
            if ( left is null ) {
                throw new ArgumentNullException( paramName: nameof( left ) );
            }

            return new Attoseconds( left.Value + ( Rational )attoseconds );
        }

        /// <summary>
        ///     <para>static equality test</para>
        /// </summary>
        /// <param name="left"> </param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Boolean Equals( [CanBeNull] Attoseconds left, [CanBeNull] Attoseconds right ) {
            if ( ReferenceEquals( left, right ) ) {
                return true;
            }

            if ( left is null || right is null ) {
                return false;
            }

            return left.Value == right.Value;
        }

        [NotNull]
        public static implicit operator Femtoseconds( [NotNull] Attoseconds attoseconds ) {
            if ( attoseconds is null ) {
                throw new ArgumentNullException( paramName: nameof( attoseconds ) );
            }

            return attoseconds.ToFemtoseconds();
        }

        [NotNull]
        public static implicit operator SpanOfTime( [NotNull] Attoseconds attoseconds ) {
            if ( attoseconds is null ) {
                throw new ArgumentNullException( paramName: nameof( attoseconds ) );
            }

            return new SpanOfTime( planckTimes: attoseconds.ToPlanckTimes().Value );
        }

        [CanBeNull]
        public static implicit operator Zeptoseconds( [NotNull] Attoseconds attoseconds ) {
            if ( attoseconds is null ) {
                throw new ArgumentNullException( paramName: nameof( attoseconds ) );
            }

            return attoseconds.ToZeptoseconds();
        }

        [NotNull]
        public static Attoseconds operator -( [NotNull] Attoseconds left, Decimal attoseconds ) {
            if ( left is null ) {
                throw new ArgumentNullException( paramName: nameof( left ) );
            }

            return Combine( left, -attoseconds );
        }

        public static Boolean operator !=( [CanBeNull] Attoseconds left, [CanBeNull] Attoseconds right ) => !Equals( left, right );

        [NotNull]
        public static Attoseconds operator +( [NotNull] Attoseconds left, [NotNull] Attoseconds right ) {
            if ( left is null ) {
                throw new ArgumentNullException( paramName: nameof( left ) );
            }

            if ( right is null ) {
                throw new ArgumentNullException( paramName: nameof( right ) );
            }

            return Combine( left, right );
        }

        [NotNull]
        public static Attoseconds operator +( [NotNull] Attoseconds left, Decimal attoseconds ) {
            if ( left is null ) {
                throw new ArgumentNullException( paramName: nameof( left ) );
            }

            return Combine( left, attoseconds );
        }

        public static Boolean operator <( [NotNull] Attoseconds left, [NotNull] Attoseconds right ) {
            if ( left is null ) {
                throw new ArgumentNullException( paramName: nameof( left ) );
            }

            if ( right is null ) {
                throw new ArgumentNullException( paramName: nameof( right ) );
            }

            return left.Value < right.Value;
        }

        public static Boolean operator ==( [CanBeNull] Attoseconds left, [CanBeNull] Attoseconds right ) => Equals( left, right );

        public static Boolean operator >( [NotNull] Attoseconds left, [NotNull] Attoseconds right ) {
            if ( left is null ) {
                throw new ArgumentNullException( paramName: nameof( left ) );
            }

            if ( right is null ) {
                throw new ArgumentNullException( paramName: nameof( right ) );
            }

            return left.Value > right.Value;
        }

        public Int32 CompareTo( [NotNull] Attoseconds other ) {
            if ( other is null ) {
                throw new ArgumentNullException( paramName: nameof( other ) );
            }

            return this.Value.CompareTo( other.Value );
        }

        /// <summary>
        /// Compares the current instance with another object of the same type and returns an integer that indicates whether the current instance precedes, follows, or occurs in the
        /// same position in the sort order as the other object.
        /// </summary>
        /// <param name="other">An object to compare with this instance. </param>
        /// <returns>
        /// A value that indicates the relative order of the objects being compared. The return value has these meanings: Value Meaning Less than zero This instance precedes
        /// <paramref name="other" /> in the sort order.  Zero This instance occurs in the same position in the sort order as <paramref name="other" />. Greater than zero This instance
        /// follows <paramref name="other" /> in the sort order.
        /// </returns>
        public Int32 CompareTo( [NotNull] IQuantityOfTime other ) {
            if ( other is null ) {
                throw new ArgumentNullException( paramName: nameof( other ) );
            }

            return this.ToPlanckTimes().Value.CompareTo( other.ToPlanckTimes().Value );
        }

        /// <summary>
        /// Compares the current instance with another object of the same type and returns an integer that indicates whether the current instance precedes, follows, or occurs in the
        /// same position in the sort order as the other object.
        /// </summary>
        /// <param name="obj">An object to compare with this instance. </param>
        /// <returns>
        /// A value that indicates the relative order of the objects being compared. The return value has these meanings: Value Meaning Less than zero This instance precedes
        /// <paramref name="obj" /> in the sort order. Zero This instance occurs in the same position in the sort order as <paramref name="obj" />. Greater than zero This instance follows
        /// <paramref name="obj" /> in the sort order.
        /// </returns>
        /// <exception cref="T:System.ArgumentException"><paramref name="obj" /> is not the same type as this instance.</exception>
        public Int32 CompareTo( [CanBeNull] Object obj ) {
            if ( obj is null ) {
                return 1;
            }

            return obj is Attoseconds other ? this.CompareTo( other ) : throw new ArgumentException( $"Object must be of type {nameof( Attoseconds )}" );
        }

        public override Boolean Equals( [CanBeNull] Object obj ) => obj is Attoseconds attoseconds && Equals( this, attoseconds );

        public override Int32 GetHashCode() => this.Value.GetHashCode();

        /// <summary>Convert to a larger unit.</summary>
        /// <returns></returns>
        [NotNull]
        public Femtoseconds ToFemtoseconds() => new Femtoseconds( this.Value / InOneFemtosecond );

        [NotNull]
        public PlanckTimes ToPlanckTimes() => new PlanckTimes( ( Rational )PlanckTimes.InOneAttosecond * this.Value );

        [NotNull]
        public Seconds ToSeconds() => throw new NotImplementedException();

        public override String ToString() {
            if ( this.Value > MathConstants.DecimalMaxValueAsBigRational ) {
                var whole = this.Value.WholePart;

                return $"{whole} {whole.PluralOf( "as" )}";
            }

            var dec = ( Decimal )this.Value;

            return $"{dec} {dec.PluralOf( "as" )}";
        }

        public TimeSpan ToTimeSpan() => TimeSpan.FromSeconds( ( Double )this.ToSeconds().Value );

        /// <summary>Convert to a smaller unit.</summary>
        /// <returns></returns>
        [NotNull]
        public Zeptoseconds ToZeptoseconds() => new Zeptoseconds( this.Value * Zeptoseconds.InOneAttosecond );
    }
}