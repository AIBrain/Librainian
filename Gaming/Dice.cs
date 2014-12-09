// This notice must be kept visible in the source.
// 
// This section of source code belongs to Rick@AIBrain.Org unless otherwise specified, or the
// original license has been overwritten by the automatic formatting of this code. Any unmodified
// sections of source code borrowed from other projects retain their original license and thanks
// goes to the Authors.
// 
// Donations and Royalties can be paid via
// PayPal: paypal@aibrain.org
// bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
// bitcoin: 1NzEsF7eegeEWDr5Vr9sSSgtUC4aL6axJu
// litecoin: LeUxdU2w3o6pLZGVys5xpDZvvo8DUrjBp9
// 
// Usage of the source code or compiled binaries is AS-IS. I am not responsible for Anything You Do.
// 
// Contact me by email if you have any questions or helpful criticism.
// 
// "Librainian/Dice.cs" was last cleaned by Rick on 2014/12/09 at 6:06 AM

namespace Librainian.Gaming {

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;
    using Collections;
    using FluentAssertions;
    using Measurement.Time;
    using Threading;

    [DataContract(IsReference = true)]
    public class Dice {

        /// <summary>
        /// Keep track of the most recent rolls.
        /// </summary>
        private readonly uint _keepTrackOfRecentRolls;

        public Dice( UInt16 numberOfSides = 6, UInt32 keepTrackOfRecentRolls = 10 ) {
            this.NumberOfSides = numberOfSides;

            this._keepTrackOfRecentRolls = keepTrackOfRecentRolls;

            this.LastFewRolls = new ParallelList<UInt16>
            {
                TimeoutForReads = Seconds.Thirty,
                TimeoutForWrites = Seconds.Thirty
            };

            this.Roll();
        }

        public UInt16 GetCurrentSideFaceUp { get; private set; }

        [DataMember]
        public UInt16 NumberOfSides { get; private set; }

        [DataMember]
        private ParallelList<UInt16> LastFewRolls { get; set; }

        public IEnumerable<UInt16> GetLastFewRolls() => this.LastFewRolls;

        /// <summary>
        /// <para>Rolls the dice to determine which side lands face-up.</para>
        /// </summary>
        /// <returns>The side which landed face-up</returns>
        public UInt16 Roll() {
            this.GetCurrentSideFaceUp = ( UInt16 )( Randem.Next( this.NumberOfSides ) + 1 );
            this.GetCurrentSideFaceUp.Should().BeInRange( 1, this.NumberOfSides );
            this.LastFewRolls.Add( this.GetCurrentSideFaceUp, this.Trim );
            return this.GetCurrentSideFaceUp;
        }

        /// <summary>
        /// <para></para>
        /// </summary>
        public void Trim() {
            while ( this.GetLastFewRolls().Count() > this._keepTrackOfRecentRolls ) {
                this.LastFewRolls.RemoveAt( 0 );
            }
        }
    }
}