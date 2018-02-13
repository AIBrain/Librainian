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
// "Librainian/VotallyD.cs" was last cleaned by Rick on 2016/06/18 at 10:52 PM

namespace Librainian.Maths.Numbers {

    using System;
    using System.Diagnostics;
    using System.Threading;
    using JetBrains.Annotations;
    using Newtonsoft.Json;

    /// <summary>
    ///     <para>Keep track of votes for candidate A and candidate B.</para>
    /// </summary>
    [JsonObject]
    [DebuggerDisplay( "{" + nameof( ToString ) + "(),nq}" )]
    public class VotallyD : ICloneable {

        /// <summary>No vote for either.</summary>
        public static readonly VotallyD Zero = new VotallyD( votesForA: 0, votesForB: 0 );

        /// <summary>ONLY used in the getter and setter.</summary>
        [JsonProperty]
        private Double _aVotes;

        /// <summary>ONLY used in the getter and setter.</summary>
        [JsonProperty]
        private Double _bVotes;

        public VotallyD( Double votesForA = 0, Double votesForB = 0 ) {
            this.A = votesForA;
            this.B = votesForB;
        }

        public Double A {
            get => Thread.VolatileRead( ref this._aVotes );

	        private set => Thread.VolatileWrite( ref this._aVotes, value );
        }

        public Double B {
            get => Thread.VolatileRead( ref this._bVotes );

	        private set => Thread.VolatileWrite( ref this._bVotes, value );
        }

        public Double ChanceB {
            get {
                var votes = this.Votes;
                return votes.Near( 0 ) ? 0 : this.B / votes;
            }
        }

        public Boolean IsAWinning => this.A > this.B;

        public Boolean IsBWinning => this.B > this.A;

        public Boolean IsLandslideA => this.IsAWinning && this.A > this.HalfOfVotes();

        public Boolean IsProtiguous => this.IsTied() && this.Votes > 1;

        /// <summary>
        ///     <see cref="A" /> + <see cref="B" />
        /// </summary>
        public Double Votes => this.A + this.B;

        public static VotallyD Combine( [NotNull] VotallyD left, [NotNull] VotallyD right ) {
            if ( left == null ) {
                throw new ArgumentNullException( nameof( left ) );
            }
            if ( right == null ) {
                throw new ArgumentNullException( nameof( right ) );
            }
            var result = left;
            result.ForA( right.A );
            result.ForB( right.B );
            return result;
        }

        public Double ChanceA() {
            var votes = this.Votes;
            return votes.Near( 0 ) ? 0 : this.A / votes;
        }

        /// <summary>
        ///     <para>Increments the votes for candidate <see cref="A" /> by <paramref name="votes" />.</para>
        /// </summary>
        public void ForA( Double votes = 1 ) {
            this.A += votes;
            if ( this.A <= 0 ) {
                this.A = 0;
            }
        }

        /// <summary>
        ///     <para>Increments the votes for candidate <see cref="B" /> by <paramref name="votes" />.</para>
        /// </summary>
        public void ForB( Double votes = 1 ) {
            this.B += votes;
            if ( this.B <= 0 ) {
                this.B = 0;
            }
        }

        public Double HalfOfVotes() => this.Votes / 2;

        public VotallyD Clone() => new VotallyD( votesForA: this.A, votesForB: this.B );

        Object ICloneable.Clone() => this.Clone();

	    public Boolean IsTied() => this.A.Near( this.B );

        public override String ToString() => $"A has {this.ChanceA():P1} and B has {this.ChanceB:P1} of {this.Votes:F1} votes.";

        /// <summary>
        ///     <para>Increments the votes for candidate <see cref="A" /> by <paramref name="votes" />.</para>
        /// </summary>
        public void WithdrawVoteForA( Double votes = 1 ) {
            this.A -= votes;
            if ( this.A <= 0 ) {
                this.A = 0;
            }
        }

        /// <summary>
        ///     <para>Increments the votes for candidate <see cref="B" /> by <paramref name="votes" />.</para>
        /// </summary>
        public void WithdrawVoteForB( Double votes = 1 ) {
            this.B -= votes;
            if ( this.B <= 0 ) {
                this.B = 0;
            }
        }
    }
}