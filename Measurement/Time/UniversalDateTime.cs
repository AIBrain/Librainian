// Copyright 2016 Protiguous.
//
// This notice must be kept visible in the source.
//
// This section of source code belongs to Protiguous@Protiguous.com unless otherwise specified, or the
// original license has been overwritten by the automatic formatting of this code. Any unmodified
// sections of source code borrowed from other projects retain their original license and thanks
// goes to the Authors.
//
// Donations and royalties can be paid via
//  
//  bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//  
//
// Usage of the source code or compiled binaries is AS-IS. I am not responsible for Anything You Do.
//
// Contact me by email if you have any questions or helpful criticism.
//
// "Librainian/UniversalDateTime.cs" was last cleaned by Protiguous on 2016/06/18 at 10:55 PM

namespace Librainian.Measurement.Time {

    using System;
    using System.Diagnostics;
    using System.Numerics;
    using Extensions;
    using Newtonsoft.Json;

    /// <summary>
    ///     <para>Absolute universal date and time.</para>
    ///     <para><see cref="PlanckTimes" /> since the big bang of <i>this</i> universe.</para>
    /// </summary>
    /// <seealso cref="http://wikipedia.org/wiki/Lol" />
    [Immutable]
    [JsonObject]
    [DebuggerDisplay( "ToString()" )]
    public struct UniversalDateTime : IComparable<UniversalDateTime> {

        public UniversalDateTime( BigInteger planckTimesSinceBigBang ) {
            this.Value = planckTimesSinceBigBang;
            var span = new Span( planckTimes: this.Value );

            //TODO
            this.Date = new Date( span );
            this.Time = new Time( span );
        }

        public UniversalDateTime( DateTime dateTime ) {
            var span = CalcSpanSince( dateTime );

            this.Value = span.TotalPlanckTimes.Value;
            this.Date = new Date( span ); //we can use span here because the values have been normalized. Should()Have()Been()?
            this.Time = new Time( span );

            //this.Time = new Time();
        }

        /// <summary>
        ///     <para>1 planck times</para>
        /// </summary>
        public static UniversalDateTime One { get; } = new UniversalDateTime( BigInteger.One );

        /// <summary>
        ///     <para>The value of this constant is equivalent to 00:00:00.0000000, January 1, 0001.</para>
        ///     <para>430,000,000,000,000,000 seconds</para>
        /// </summary>
        public static PlanckTimes PlancksUpToMinDateTime { get; } = new PlanckTimes( new Seconds( 4.3E17m ) );

        /// <summary>
        ///     <para>0 planck times</para>
        /// </summary>
        public static UniversalDateTime TheBeginning { get; } = new UniversalDateTime( BigInteger.Zero );

        public static UniversalDateTime Unix { get; } = new UniversalDateTime( Epochs.Unix );

        /// <summary>
        /// </summary>
        [JsonProperty]
        public Date Date {
            get;
        }

        public static UniversalDateTime Now => new UniversalDateTime( DateTime.UtcNow );

        /// <summary>
        /// </summary>
        [JsonProperty]
        public Time Time {
            get;
        }

        /// <summary>
        ///     <para><see cref="PlanckTimes" /> since the big bang of <i>this</i> universe.</para>
        /// </summary>
        [JsonProperty]
        public BigInteger Value {
            get;
        }

        /// <summary>
        ///     Given a <see cref="DateTime" />, calculate the <see cref="Span" />.
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static Span CalcSpanSince( DateTime dateTime ) {
            var sinceThen = new Span( dateTime - DateTime.MinValue );
            var plancksSinceThen = sinceThen.TotalPlanckTimes;
            var span = new Span( planckTimes: PlancksUpToMinDateTime.Value + plancksSinceThen.Value );
            return span;
        }

        public static UniversalDateTime operator -( UniversalDateTime left, UniversalDateTime right ) => Combine( left, -right );

        public static UniversalDateTime operator -( UniversalDateTime universalDateTime ) => new UniversalDateTime( universalDateTime.Value * -1 );

        public static Boolean operator <( UniversalDateTime left, UniversalDateTime right ) => left.Value < right.Value;

        public static Boolean operator >( UniversalDateTime left, UniversalDateTime right ) => left.Value > right.Value;

        public Int32 CompareTo( UniversalDateTime other ) => this.Value.CompareTo( other.Value );

        /// <summary>
        ///     Returns the fully qualified type name of this instance.
        /// </summary>
        /// <returns>
        ///     A <see cref="T:System.String" /> containing a fully qualified type name.
        /// </returns>
        public override String ToString() => this.Value.ToString();

	    private static UniversalDateTime Combine( UniversalDateTime left, BigInteger value ) => new UniversalDateTime( left.Value + value );

        private static UniversalDateTime Combine( UniversalDateTime left, UniversalDateTime right ) => Combine( left, right.Value );
    }
}