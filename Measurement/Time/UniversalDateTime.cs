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
// "Librainian2/UniversalDateTime.cs" was last cleaned by Rick on 2014/08/08 at 2:30 PM
#endregion

namespace Librainian.Measurement.Time {
    using System;
    using System.Numerics;
    using System.Web.UI;
    using Annotations;
    using FluentAssertions;

    /// <summary>
    ///     <para>Absolute universal date and time.</para>
    ///     <para><see cref="PlanckTimes" /> since the big bang of <i>this</i> universe.</para>
    /// </summary>
    /// <seealso cref="http://wikipedia.org/wiki/Lol" />
    public struct UniversalDateTime : IComparable<UniversalDateTime> {

        [UsedImplicitly]
        public static readonly PlanckTimes PlancksUpTo1900 = new PlanckTimes( new Seconds( 4.415E17m ).ToPlanckTimes() );


        public static readonly UniversalDateTime One = new UniversalDateTime( BigInteger.One );

        public static readonly UniversalDateTime TheBeginning = new UniversalDateTime( BigInteger.Zero );

        public static readonly UniversalDateTime UNIX = new UniversalDateTime( Epochs.UNIX );

        /// <summary>
        /// </summary>
        public readonly Date Date;

        /// <summary>
        /// </summary>
        public readonly Time Time;

        /// <summary>
        ///     <para><see cref="PlanckTimes" /> since the big bang of <i>this</i> universe.</para>
        /// </summary>
        public readonly BigInteger Value;

        public UniversalDateTime( BigInteger planckTimesSinceBigBang ) {
            this.Value = planckTimesSinceBigBang;
            var span = new Span( planckTimes: this.Value );
            this.Date = new Date( span );
            this.Time = new Time( span );
        }

        public UniversalDateTime( DateTime dateTime ) {
            BigInteger timebackwhen;
            BigInteger timePassedSinceThen;
            var span = CalcSpanSince( dateTime, out timebackwhen, out timePassedSinceThen );

            this.Value = span.TotalPlanckTimes;
            this.Date = new Date( span  ); //we can use span here because the values have been normalized.
            this.Time = new Time( span ); //we can use span here because the values have been normalized.
        }

        private static Span CalcSpanSince( DateTime dateTime, out BigInteger timebackwhen, out BigInteger timePassedSinceThen ) {
            timebackwhen = PlancksUpTo1900.Value;
            timePassedSinceThen = new Span( dateTime - DateTime.MinValue ).TotalPlanckTimes;
            var span = new Span( planckTimes: timebackwhen + timePassedSinceThen );
            return span;
        }

        public int CompareTo( UniversalDateTime other ) {
            return this.Value.CompareTo( other.Value );
        }

        public static UniversalDateTime operator -( UniversalDateTime left, UniversalDateTime right ) {
            return Combine( left, -right );
        }

        public static UniversalDateTime operator -( UniversalDateTime universalDateTime ) {
            return new UniversalDateTime( universalDateTime.Value * -1 );
        }

        public static Boolean operator <( UniversalDateTime left, UniversalDateTime right ) {
            return left.Value < right.Value;
        }

        public static Boolean operator >( UniversalDateTime left, UniversalDateTime right ) {
            return left.Value > right.Value;
        }

        private static UniversalDateTime Combine( UniversalDateTime left, BigInteger value ) {
            return new UniversalDateTime( left.Value + value );
        }

        private static UniversalDateTime Combine( UniversalDateTime left, UniversalDateTime right ) {
            return Combine( left, right.Value );
        }
    }
}
