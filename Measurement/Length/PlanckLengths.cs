// Copyright © 1995-2018 to Rick@AIBrain.org and Protiguous.
// All Rights Reserved.
//
// This ENTIRE copyright notice and file header MUST BE KEPT
// VISIBLE in any source code derived from or used from our
// libraries and projects.
//
// =========================================================
// This section of source code, "PlanckLengths.cs",
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
// "Librainian/Librainian/PlanckLengths.cs" was last cleaned by Protiguous on 2018/05/15 at 10:47 PM.

namespace Librainian.Measurement.Length {

    using System;
    using System.Diagnostics;
    using System.Numerics;
    using FluentAssertions;
    using Newtonsoft.Json;
    using NUnit.Framework;

    /// <summary>
    /// </summary>
    /// <seealso cref="http://en.wikipedia.org/wiki/Plank_length" />
    [DebuggerDisplay( "{" + nameof( ToString ) + "(),nq}" )]
    [JsonObject]
    public struct PlanckLengths : IComparable<PlanckLengths> {

        /// <summary>
        ///     One <see cref="PlanckLengths" />.
        /// </summary>
        public static readonly PlanckLengths One = new PlanckLengths( planckLengths: 1 );

        /// <summary>
        ///     One <see cref="PlanckLengths" />.
        /// </summary>
        public static readonly PlanckLengths Two = new PlanckLengths( planckLengths: 2 );

        /// <summary>
        ///     Zero <see cref="PlanckLengths" />.
        /// </summary>
        public static readonly PlanckLengths Zero = new PlanckLengths( planckLengths: 0 );

        [JsonProperty]
        public readonly BigInteger Value;

        public PlanckLengths( BigInteger planckLengths ) : this() => this.Value = planckLengths;

        public Int32 CompareTo( PlanckLengths other ) => this.Value.CompareTo( other.Value );

        //public static Boolean operator >( PlanckUnits left, Minutes rhs ) {
        //    return left.Comparison( rhs ) > 0;
        //}
        public override Int32 GetHashCode() => this.Value.GetHashCode();

        public override String ToString() => $"{this.Value}";

        //public static implicit operator Span( PlanckUnits milliseconds ) {
        //    return Span.FromMilliseconds( milliseconds: milliseconds.Value );
        //}

        //public static Boolean operator <( PlanckUnits left, PlanckUnits rhs ) {
        //    return left.Value.CompareTo( rhs.Value ) < 0;
        //}

        //public static Boolean operator <( PlanckUnits left, Seconds rhs ) {
        //    return left.Comparison( rhs ) < 0;
        //}

        //public static Boolean operator <( PlanckUnits left, Minutes rhs ) {
        //    return left.Comparison( rhs ) < 0;
        //}

        //public static Boolean operator >( PlanckUnits left, PlanckUnits rhs ) {
        //    return left.Value.CompareTo( rhs.Value ) > 0;
        //}

        //public static Boolean operator >( PlanckUnits left, Seconds rhs ) {
        //    return left.Comparison( rhs ) > 0;
        //}
    }

    [TestFixture]
    public static class TestPlanckLengths {

        [Test]
        public static void TestSmallValuePlancks() {

            PlanckLengths.One.Should().BeLessThan( PlanckLengths.Two );
            PlanckLengths.Two.Should().BeGreaterThan( PlanckLengths.One );
        }
    }
}