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
// Contact me by email if you have any questions or helpful criticism.
//
// "Librainian/Dice.cs" was last cleaned by Rick on 2014/08/13 at 12:09 PM

#endregion License & Information

namespace Librainian.Gaming {

    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;
    using System.Threading.Tasks;
    using Annotations;
    using Collections;
    using FluentAssertions;
    using Measurement.Time;
    using Threading;

    [DataContract( IsReference = true )]
    public class Dice : IDice {
        private readonly Span? _dontTrackRollsOlderThan;

        /// <summary>
        ///     Keep track of the most recent rolls.
        /// </summary>
        private readonly uint _keepTrackOfXRolls;

        public Dice( UInt16 numberOfSides = 6, UInt32 keepTrackOfXRolls = 10, Span? dontTrackRollsOlderThan = null, Span? timeout = null ) {
            this.Tasks = new ConcurrentDictionary<Task, DateTime>();
            this.LastFewRolls = new ParallelList<UInt16>();
            this.NumberOfSides = numberOfSides;
            this._keepTrackOfXRolls = keepTrackOfXRolls;
            this._dontTrackRollsOlderThan = dontTrackRollsOlderThan;

            if ( !timeout.HasValue ) {
                timeout = Seconds.Thirty;
            }
            this.LastFewRolls.TimeoutForReads = timeout.Value;
            this.LastFewRolls.TimeoutForWrites = timeout.Value;

            this.RollTheDice();
        }

        public ushort GetCurrentSideFaceUp { get; private set; }

        [DataMember]
        public UInt16 NumberOfSides {
            get;
            private set;
        }

        [DataMember]
        private ParallelList<UInt16> LastFewRolls {
            get;
            set;
        }

        private ConcurrentDictionary<Task, DateTime> Tasks {
            get;
            set;
        }

        public IEnumerable<ushort> GetLastFewRolls() {
            return this.LastFewRolls;
        }

        public void RollTheDice() {
            this.GetCurrentSideFaceUp = ( UInt16 )( Randem.Next( this.NumberOfSides ) + 1 );
            this.GetCurrentSideFaceUp.Should().BeInRange( 1, this.NumberOfSides );
        }

        /// <summary>
        ///     <para>Rolls the dice to determine which side lands face-up.</para>
        /// </summary>
        /// <returns>The side which landed face-up</returns>
        public UInt16 Roll() {
            this.RollTheDice();
            var with = this.LastFewRolls.AddAsync( this.GetCurrentSideFaceUp, this.Cleanup ).ContinueWith( task => {
                DateTime dummy;
                var removed = this.Tasks.TryRemove( task, out dummy );
                if ( removed ) {
                    String.Format( "Old roll {0} removed", dummy ).TimeDebug();
                }
            } );
            this.Tasks.TryAdd( with, DateTime.Now );
            return this.GetCurrentSideFaceUp;
        }

        public async void RollAsync( [NotNull] Action<UInt16> afterRoll ) {
            if ( afterRoll == null ) {
                throw new ArgumentNullException( "onRoll" );
            }
            await Task.Run( () => this.RollTheDice() ).ContinueWith( task => afterRoll( this.GetCurrentSideFaceUp ) );
        }

        private void Cleanup() {
            while ( this.GetLastFewRolls().Count() > this._keepTrackOfXRolls ) {
                this.LastFewRolls.RemoveAt( 0 );
            }

            if ( !this._dontTrackRollsOlderThan.HasValue ) {
                return;
            }
            foreach ( var key in this.Tasks.Where( pair => DateTime.Now - pair.Value > this._dontTrackRollsOlderThan.Value ).Select( pair => pair.Key ) ) {
                DateTime unused;
                this.Tasks.TryRemove( key, out unused );
            }
        }
    }
}