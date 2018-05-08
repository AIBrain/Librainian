// Copyright 2018 Protiguous.
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
// "Librainian/VotallyI.cs" was last cleaned by Protiguous on 2016/07/08 at 8:33 AM

namespace Librainian.Maths.Numbers {

    using System;
    using System.Diagnostics;
    using System.Threading;
    using JetBrains.Annotations;
    using Newtonsoft.Json;
    using Numerics;

    /// <summary>
    ///     <para>threadsafe, keep integer count of Yes or No votes.</para>
    /// </summary>
    [JsonObject]
    [DebuggerDisplay( "{" + nameof( ToString ) + "(),nq}" )]
    public class VotallyI {

        /// <summary>No vote for either.</summary>
        public static readonly VotallyI Zero = new VotallyI( votesYes: 0, votesNo: 0 );

        /// <summary>ONLY used in the getter and setter.</summary>
        private UInt64 _votesNo;

        /// <summary>ONLY used in the getter and setter.</summary>
        private UInt64 _votesYes;

        public VotallyI( UInt64 votesYes = 0, UInt64 votesNo = 0 ) {
            this.Yes = votesYes;
            this.No = votesNo;
        }

        public UInt64 No {
            get => Thread.VolatileRead( ref this._votesNo );

	        private set => Thread.VolatileWrite( ref this._votesNo, value );
        }

        public UInt64 Votes => this.Yes + this.No;

        public UInt64 Yes {
            get => Thread.VolatileRead( ref this._votesYes );

	        private set => Thread.VolatileWrite( ref this._votesYes, value );
        }

        /// <summary>
        ///     Add in the votes from another <see cref="VotallyI" />.
        /// </summary>
        /// <param name="right"></param>
        public void Add( [NotNull] VotallyI right ) {
            if ( right is null ) {
                throw new ArgumentNullException( nameof( right ) );
            }
            VoteYes( right.Yes );
            VoteNo( right.No );
        }

        public static VotallyI Combine( [NotNull] VotallyI left, [NotNull] VotallyI right ) {
            if ( left is null ) {
                throw new ArgumentNullException( nameof( left ) );
            }
            if ( right is null ) {
                throw new ArgumentNullException( nameof( right ) );
            }
            var result = left;
            result.VoteYes( right.Yes );
            result.VoteNo( right.No );
            return result;
        }

        public Double ChanceNo() {
            try {
                var votes = this.Votes;
                if ( !votes.Near( 0 ) ) {
                    var result = new BigRational( this.No, votes );
                    return ( Double )result;
                }
            }
            catch ( DivideByZeroException exception ) {
                exception.More();
            }
            return 0;
        }

        public Double ChanceYes() {
            try {
                var votes = this.Votes;
                if ( votes.Near( 0 ) ) {
                    return 0;
                }
                var chance = new BigRational( this.Yes, votes );
                return ( Double )chance;
            }
            catch ( DivideByZeroException exception ) {
                exception.More();
                return 0;
            }
        }

        public VotallyI Clone() => new VotallyI( votesYes: this.Yes, votesNo: this.No );

        public UInt64 HalfOfVotes() => this.Votes / 2;

        public Boolean IsLandslideNo() => this.IsNoWinning() && this.No > this.HalfOfVotes();

        public Boolean IsLandslideYes() => this.IsYesWinning() && this.Yes > this.HalfOfVotes();

        public Boolean IsNoWinning() => this.No > this.Yes && this.Yes > 1 && this.No > 1;

        public Boolean IsProtiguous() => this.IsTied() && this.Votes >= 2;

        public Boolean IsTied() => this.Yes == this.No;

        public Boolean IsYesWinning() => this.Yes > this.No && this.Yes > 1 && this.No > 1;

        public override String ToString() => $"{this.ChanceYes():P1} yes vs {this.ChanceNo():p1} no of {this.Votes} votes.";

        /// <summary>
        ///     <para>Increments the votes for candidate <see cref="No" /> by <paramref name="votes" />.</para>
        /// </summary>
        public void VoteNo( UInt64 votes = 1 ) => this.No += votes;

        /// <summary>
        ///     <para>Increments the votes for candidate <see cref="Yes" /> by <paramref name="votes" />.</para>
        /// </summary>
        public void VoteYes( UInt64 votes = 1 ) => this.Yes += votes;

        /// <summary>
        ///     <para>Increments the votes for candidate <see cref="No" /> by <paramref name="votes" />.</para>
        /// </summary>
        public void WithdrawNoVote( UInt64 votes = 1 ) => this.No -= votes;

        /// <summary>
        ///     <para>Increments the votes for candidate <see cref="Yes" /> by <paramref name="votes" />.</para>
        /// </summary>
        public void WithdrawYesVote( UInt64 votes = 1 ) => this.Yes -= votes;
    }
}