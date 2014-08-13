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
// "Librainian/Dice.cs" was last cleaned by Rick on 2014/08/11 at 12:38 AM
#endregion

namespace Librainian.Hardware.Virtual.Gaming {
    using System;
    using System.Collections;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Collections;
    using Measurement.Time;
    using Threading;

    public class Dice {

        private readonly uint _keepTrackOfXRolls;
        public readonly UInt16 NumberOfSides;

        private readonly ParallelList<UInt16> _lastFewRolls = new ParallelList<UInt16>();

        public IEnumerable<UInt16> LastFewRolls {
            get {
                return this._lastFewRolls;
            }
        }

        public Dice( UInt16 numberOfSides, UInt32 keepTrackOfXRolls = 10 ) {
            this._keepTrackOfXRolls = keepTrackOfXRolls;
            this.NumberOfSides = numberOfSides;
        }

        private readonly ConcurrentDictionary<Task, DateTime> _tasks = new ConcurrentDictionary<Task, DateTime>();

        /// <summary>
        ///     Rolls the dice to determine which side lands face-up
        /// </summary>
        /// <returns>The side which landed face-up</returns>
        public UInt16 Roll() {
            var result = ( UInt16 )( Randem.Next( this.NumberOfSides ) + 1 );
            Task key = this._lastFewRolls.AddAsync( result ).ContinueWith( task => {
                if ( this.LastFewRolls.Count() > this._keepTrackOfXRolls ) {
                    this._lastFewRolls.TakeFirst();
                }
                DateTime dummy;
                this._tasks.TryRemove( task, out dummy );
                foreach ( var keyValuePair in _tasks.Where( pair => pair.Value < DateTime.Now.AddMinutes( -10 ) ) ) {
                    this._tasks.TryRemove( keyValuePair.Key, out dummy );
                }
            } );
            _tasks.TryAdd( key, DateTime.Now );
            return result;
        }
    }
}
