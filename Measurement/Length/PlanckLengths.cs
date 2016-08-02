// Copyright 2016 Rick@AIBrain.org.
//
// This notice must be kept visible in the source.
//
// This section of source code belongs to Rick@AIBrain.Org unless otherwise specified, or the
// original license has been overwritten by the automatic formatting of this code. Any unmodified
// sections of source code borrowed from other projects retain their original license and thanks
// goes to the Authors.
//
// Donations and royalties can be paid via
//  PayPal: paypal@aibrain.org
//  bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//  litecoin: LeUxdU2w3o6pLZGVys5xpDZvvo8DUrjBp9
//
// Usage of the source code or compiled binaries is AS-IS. I am not responsible for Anything You Do.
//
// Contact me by email if you have any questions or helpful criticism.
//
// "Librainian/PlanckLengths.cs" was last cleaned by Rick on 2016/06/18 at 10:53 PM

namespace Librainian.Measurement.Length {

    using System;
    using System.Diagnostics;
    using System.Numerics;
    using Newtonsoft.Json;
    using NUnit.Framework;

    /// <summary></summary>
    /// <seealso cref="http://en.wikipedia.org/wiki/Plank_length" />
    [DebuggerDisplay( "{ToString(),nq}" )]
    [JsonObject]
    public struct PlanckLengths : IComparable<PlanckLengths> {

        /// <summary>One <see cref="PlanckLengths" />.</summary>
        public static readonly PlanckLengths One = new PlanckLengths( planckLengths: 1 );

        /// <summary>One <see cref="PlanckLengths" />.</summary>
        public static readonly PlanckLengths Two = new PlanckLengths( planckLengths: 2 );

        /// <summary>Zero <see cref="PlanckLengths" />.</summary>
        public static readonly PlanckLengths Zero = new PlanckLengths( planckLengths: 0 );

        [JsonProperty]
        public readonly BigInteger Value;

        static PlanckLengths() {
            Assert.That( Zero.Value < One.Value );
            Assert.That( One.Value < Two.Value );
            Assert.That( Two.Value > One.Value );
            Assert.That( One.Value > Zero.Value );
        }

        public PlanckLengths( BigInteger planckLengths ) : this() {
            this.Value = planckLengths;
        }

        public Int32 CompareTo( PlanckLengths other ) => this.Value.CompareTo( other.Value );

        //public static Boolean operator >( PlanckUnits lhs, Minutes rhs ) {
        //    return lhs.Comparison( rhs ) > 0;
        //}
        public override Int32 GetHashCode() => this.Value.GetHashCode();

        public override String ToString() => $"{this.Value}";

        //public static implicit operator Span( PlanckUnits milliseconds ) {
        //    return Span.FromMilliseconds( milliseconds: milliseconds.Value );
        //}

        //public static Boolean operator <( PlanckUnits lhs, PlanckUnits rhs ) {
        //    return lhs.Value.CompareTo( rhs.Value ) < 0;
        //}

        //public static Boolean operator <( PlanckUnits lhs, Seconds rhs ) {
        //    return lhs.Comparison( rhs ) < 0;
        //}

        //public static Boolean operator <( PlanckUnits lhs, Minutes rhs ) {
        //    return lhs.Comparison( rhs ) < 0;
        //}

        //public static Boolean operator >( PlanckUnits lhs, PlanckUnits rhs ) {
        //    return lhs.Value.CompareTo( rhs.Value ) > 0;
        //}

        //public static Boolean operator >( PlanckUnits lhs, Seconds rhs ) {
        //    return lhs.Comparison( rhs ) > 0;
        //}
    }
}