// Copyright © Protiguous. All Rights Reserved.
// 
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
// 
// This source code contained in "Seconds.cs" belongs to Protiguous@Protiguous.com
// unless otherwise specified or the original license has been overwritten by formatting.
// (We try to avoid it from happening, but it does accidentally happen.)
// 
// Any unmodified portions of source code gleaned from other projects still retain their original
// license and our thanks goes to those Authors. If you find your code in this source code, please
// let us know so we can properly attribute you and include the proper license and/or copyright.
// 
// If you want to use any of our code in a commercial project, you must contact
// Protiguous@Protiguous.com for permission and a quote.
// 
// Donations are accepted (for now) via
//     bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//     PayPal: Protiguous@Protiguous.com
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
// Project: "Librainian", "Seconds.cs" was last formatted by Protiguous on 2020/01/31 at 12:27 AM.

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

    /// <summary>
    ///     <para>
    ///     Under the International System of Units, since 1967 the second has been defined as the duration of 9192631770 periods of the radiation corresponding to the transition
    ///     between the two hyperfine levels of the ground state of the caesium 133 atom.
    ///     </para>
    /// </summary>
    [JsonObject]
    [DebuggerDisplay( "{" + nameof( ToString ) + "(),nq}" )]
    [Immutable]
    public class Seconds : IQuantityOfTime, IEquatable<Seconds> {

        public Boolean Equals( Seconds other ) => other != null && this.Value.Equals( other.Value );

        public override Int32 GetHashCode() => this.Value.GetHashCode();

        public PlanckTimes ToPlanckTimes() => new PlanckTimes( this.Value * ( Rational ) PlanckTimes.InOneSecond );

        [NotNull]
        public Seconds ToSeconds() => new Seconds( this.Value );

        public override String ToString() {
            if ( this.Value > MathConstants.DecimalMaxValueAsBigRational ) {
                var whole = this.Value.WholePart;

                return $"{whole} {whole.PluralOf( "second" )}";
            }

            var dec = ( Decimal ) this.Value;

            return $"{dec} {dec.PluralOf( "second" )}";
        }

        public TimeSpan ToTimeSpan() => TimeSpan.FromSeconds( ( Double ) this.Value );

        /// <summary><see cref="Five" /><see cref="Seconds" />.</summary>
        public static Seconds Five { get; } = new Seconds( 5 );

        /// <summary><see cref="One" /><see cref="Seconds" />.</summary>
        public static Seconds One { get; } = new Seconds( 1 );

        /// <summary><see cref="OnePointFive" /><see cref="Seconds" />.</summary>
        public static Seconds OnePointFive { get; } = new Seconds( 1.5 );

        /// <summary><see cref="Seven" /><see cref="Seconds" />.</summary>
        public static Seconds Seven { get; } = new Seconds( 7 );

        /// <summary><see cref="Ten" /><see cref="Seconds" />.</summary>
        public static Seconds Ten { get; } = new Seconds( 10 );

        /// <summary><see cref="Thirteen" /><see cref="Seconds" />.</summary>
        public static Seconds Thirteen { get; } = new Seconds( 13 );

        /// <summary><see cref="Thirty" /><see cref="Seconds" />.</summary>
        public static Seconds Thirty { get; } = new Seconds( 30 );

        /// <summary><see cref="Three" /><see cref="Seconds" />.</summary>
        public static Seconds Three { get; } = new Seconds( 3 );

        /// <summary><see cref="Twenty" /><see cref="Seconds" />.</summary>
        public static Seconds Twenty { get; } = new Seconds( 20 );

        /// <summary><see cref="Two" /><see cref="Seconds" />.</summary>
        public static Seconds Two { get; } = new Seconds( 2 );

        /// <summary></summary>
        public static Seconds Zero { get; } = new Seconds( 0 );

        [JsonProperty]
        public Rational Value { get; }

        /// <summary>31536000</summary>
        public const UInt32 InOneCommonYear = 31536000;

        /// <summary>86400</summary>
        public const UInt32 InOneDay = 86400;

        /// <summary>3600</summary>
        public const UInt16 InOneHour = 3600;

        /// <summary>60</summary>
        public const Byte InOneMinute = 60;

        /// <summary>2635200 (30.5 days)</summary>
        public const UInt32 InOneMonth = 2635200;

        /// <summary>604800</summary>
        public const UInt32 InOneWeek = 604800;

        public Seconds( Decimal value ) => this.Value = ( Rational ) value;

        public Seconds( Double value ) => this.Value = ( Rational ) value;

        public Seconds( Rational value ) => this.Value = value;

        public Seconds( Int64 value ) => this.Value = value;

        public Seconds( BigInteger value ) => this.Value = value;

        [NotNull]
        [Pure]
        public static Seconds Combine( [NotNull] Seconds left, [NotNull] Seconds right ) => Combine( left, right.Value );

        [NotNull]
        public static Seconds Combine( [NotNull] Seconds left, Rational seconds ) => new Seconds( left.Value + seconds );

        [NotNull]
        public static Seconds Combine( [NotNull] Seconds left, BigInteger seconds ) => new Seconds( left.Value + seconds );

        /// <summary>
        ///     <para>static equality test</para>
        /// </summary>
        /// <param name="left"> </param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Boolean Equals( [CanBeNull] Seconds left, [CanBeNull] Seconds right ) {
            if ( ReferenceEquals( left, right ) ) {
                return true;
            }

            if ( left is null || right is null ) {
                return default;
            }

            return left.Value == right.Value;
        }

        /// <summary>Implicitly convert the number of <paramref name="seconds" /> to <see cref="Milliseconds" />.</summary>
        /// <param name="seconds"></param>
        /// <returns></returns>
        [CanBeNull]
        public static implicit operator Milliseconds( [NotNull] Seconds seconds ) => seconds.ToMilliseconds();

        /// <summary>Implicitly convert the number of <paramref name="seconds" /> to <see cref="Minutes" />.</summary>
        /// <param name="seconds"></param>
        /// <returns></returns>
        [CanBeNull]
        public static implicit operator Minutes( [NotNull] Seconds seconds ) => seconds.ToMinutes();

        [NotNull]
        public static implicit operator SpanOfTime( [NotNull] Seconds seconds ) => new SpanOfTime( seconds: seconds );

        /// <summary>Returns a <see cref="TimeSpan" /></summary>
        /// <param name="seconds"></param>
        public static implicit operator TimeSpan( [NotNull] Seconds seconds ) {

            if ( seconds.Value >= ( Int64 ) TimeSpan.MaxValue.TotalSeconds ) {
                return TimeSpan.MaxValue;
            }

            if ( seconds.Value <= ( Int64 ) TimeSpan.MinValue.TotalSeconds ) {
                return TimeSpan.MinValue;
            }

            return TimeSpan.FromSeconds( ( Double ) seconds.Value );
        }

        [NotNull]
        public static Seconds operator -( [NotNull] Seconds seconds ) => new Seconds( seconds.Value * -1 );

        [NotNull]
        public static Seconds operator -( [NotNull] Seconds left, [CanBeNull] Seconds right ) => Combine( left: left, right: -right );

        [NotNull]
        public static Seconds operator -( [NotNull] Seconds left, Decimal seconds ) => Combine( left, ( Rational ) ( -seconds ) );

        public static Boolean operator !=( [NotNull] Seconds left, [NotNull] Seconds right ) => !Equals( left, right );

        [NotNull]
        public static Seconds operator +( [NotNull] Seconds left, [NotNull] Seconds right ) => Combine( left, right );

        [NotNull]
        public static Seconds operator +( [NotNull] Seconds left, Decimal seconds ) => Combine( left, ( Rational ) seconds );

        [NotNull]
        public static Seconds operator +( [NotNull] Seconds left, BigInteger seconds ) => Combine( left, seconds );

        public static Boolean operator <( [NotNull] Seconds left, [NotNull] Seconds right ) => left.Value < right.Value;

        public static Boolean operator <( [CanBeNull] Seconds left, [CanBeNull] Milliseconds right ) => left < ( Seconds ) right;

        public static Boolean operator <( [CanBeNull] Seconds left, [CanBeNull] Minutes right ) => ( Minutes ) left < right;

        public static Boolean operator ==( [NotNull] Seconds left, [NotNull] Seconds right ) {
            if ( left is null ) {
                throw new ArgumentNullException( nameof( left ) );
            }

            if ( right is null ) {
                throw new ArgumentNullException( nameof( right ) );
            }

            return Equals( left, right );
        }

        public static Boolean operator >( [CanBeNull] Seconds left, [CanBeNull] Minutes right ) => ( Minutes ) left > right;

        public static Boolean operator >( [NotNull] Seconds left, [NotNull] Seconds right ) => left.Value > right.Value;

        public static Boolean operator >( [CanBeNull] Seconds left, [CanBeNull] Milliseconds right ) => left > ( Seconds ) right;

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
                throw new ArgumentNullException( nameof( other ) );
            }

            return this.ToPlanckTimes().Value.CompareTo( other.ToPlanckTimes().Value );
        }

        public override Boolean Equals( Object obj ) => Equals( this, obj as Seconds );

        [NotNull]
        public Milliseconds ToMilliseconds() => new Milliseconds( this.Value * Milliseconds.InOneSecond );

        [NotNull]
        public Minutes ToMinutes() => new Minutes( this.Value / InOneMinute );

        [NotNull]
        public Weeks ToWeeks() => new Weeks( this.Value / InOneWeek );

        [NotNull]
        public Years ToYears() => new Years( this.Value / InOneCommonYear );

    }

}