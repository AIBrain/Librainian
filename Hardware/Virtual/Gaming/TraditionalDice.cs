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

namespace Librainian.Hardware.Virtual.Gaming {

    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Collections;
    using Measurement.Time;
    using Threading;

    public interface IDice {

        /// <summary>
        ///     <para>Rolls the dice to determine which side lands face-up.</para>
        /// </summary>
        /// <returns>The side which landed face-up</returns>
        UInt16 Roll();

        IEnumerable<ushort> GetLastFewRolls();
    }

    public class TraditionalDice : IDice {
        public readonly UInt16 NumberOfSides;
        private readonly Span? _dontTrackRollsOlderThan;

        /// <summary>
        ///     Keep track of the most recent rolls.
        /// </summary>
        private readonly uint _keepTrackOfXRolls;

        private readonly ParallelList<UInt16> _lastFewRolls = new ParallelList<UInt16>();

        private readonly ConcurrentDictionary<Task, DateTime> _tasks = new ConcurrentDictionary<Task, DateTime>();

        public TraditionalDice( UInt16 numberOfSides, UInt32 keepTrackOfXRolls = 10, Span? dontTrackRollsOlderThan = null, Span? timeout = null ) {
            this.NumberOfSides = numberOfSides;
            this._keepTrackOfXRolls = keepTrackOfXRolls;
            this._dontTrackRollsOlderThan = dontTrackRollsOlderThan;

            if ( !timeout.HasValue ) {
                timeout = Seconds.Thirty;
            }
            this._lastFewRolls.TimeoutForReads = timeout.Value;
            this._lastFewRolls.TimeoutForWrites = timeout.Value;
        }

        public IEnumerable<ushort> GetLastFewRolls() {
            return this._lastFewRolls;
        }

        /// <summary>
        ///     <para>Rolls the dice to determine which side lands face-up.</para>
        /// </summary>
        /// <returns>The side which landed face-up</returns>
        public UInt16 Roll() {
            var result = ( UInt16 )( Randem.Next( this.NumberOfSides ) + 1 );
            var key = this._lastFewRolls.AddAsync( result, OnAfterAdd ).ContinueWith( task => {
                DateTime dummy;
                var removed = this._tasks.TryRemove( task, out dummy );
                String.Format( "{0}", removed ).TimeDebug();
            } );
            this._tasks.TryAdd( key, DateTime.Now );
            return result;
        }

        private void OnAfterAdd() {
            if ( this.GetLastFewRolls().Count() > this._keepTrackOfXRolls ) {
                this._lastFewRolls.TakeFirst();
            }
            if ( !this._dontTrackRollsOlderThan.HasValue ) {
                return;
            }
            foreach ( var keyValuePair in this._tasks.Where( pair => DateTime.Now - pair.Value > this._dontTrackRollsOlderThan.Value ) ) {
                DateTime dummy;
                this._tasks.TryRemove( keyValuePair.Key, out dummy );
            }
        }
    }
}