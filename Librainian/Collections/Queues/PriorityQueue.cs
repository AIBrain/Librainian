// Copyright © Protiguous. All Rights Reserved.
// This entire copyright notice and license must be retained and must be kept visible in any binaries, libraries, repositories, or source code (directly or derived) from our binaries, libraries, projects, solutions, or applications.
// All source code belongs to Protiguous@Protiguous.com unless otherwise specified or the original license has been overwritten by formatting. (We try to avoid it from happening, but it does accidentally happen.)
// Any unmodified portions of source code gleaned from other sources still retain their original license and our thanks goes to those Authors.
// If you find your code unattributed in this source code, please let us know so we can properly attribute you and include the proper license and/or copyright(s).
// If you want to use any of our code in a commercial project, you must contact Protiguous@Protiguous.com for permission, license, and a quote.
// 
// Donations, payments, and royalties are accepted via bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2 and PayPal: Protiguous@Protiguous.com
// 
// ====================================================================
// Disclaimer:  Usage of the source code or binaries is AS-IS.
// No warranties are expressed, implied, or given.
// We are NOT responsible for Anything You Do With Our Code.
// We are NOT responsible for Anything You Do With Our Executables.
// We are NOT responsible for Anything You Do With Your Computer.
// ====================================================================
// 
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
// For business inquiries, please contact me at Protiguous@Protiguous.com.
// Our software can be found at "https://Protiguous.Software/"
// Our GitHub address is "https://github.com/Protiguous".
// 
// File "PriorityQueue.cs" last formatted on 2020-08-14 at 8:31 PM.

namespace Librainian.Collections.Queues {

	using System;
	using System.Collections;
	using System.Collections.Concurrent;
	using System.Collections.Generic;
	using System.Linq;
	using JetBrains.Annotations;
	using Newtonsoft.Json;

	/// <summary>The highest priority is the highest</summary>
	[JsonObject]
	public class PriorityQueue<TValue> : IEnumerable<KeyValuePair<Single, TValue>> {

		[JsonProperty]
		[NotNull]
		private ConcurrentDictionary<Single, TValue> Dictionary { get; } = new ConcurrentDictionary<Single, TValue>( Environment.ProcessorCount, 1 );

		/// <summary>Returns an enumerator that iterates through a collection.</summary>
		/// <returns>An <see cref="IEnumerator" /> object that can be used to iterate through the collection.</returns>
		public IEnumerator GetEnumerator() => this.Dictionary.GetEnumerator();

		/// <summary>Returns an enumerator that iterates through the collection.</summary>
		/// <returns>An enumerator that can be used to iterate through the collection.</returns>
		IEnumerator<KeyValuePair<Single, TValue>> IEnumerable<KeyValuePair<Single, TValue>>.GetEnumerator() => this.Dictionary.GetEnumerator();

		/// <summary>Inject the specified priority.</summary>
		/// <param name="item">    </param>
		/// <param name="priority"></param>
		public void Add( [CanBeNull] TValue item, Single priority ) =>

			//while ( this.Dictionary.ContainsKey( priority ) ) {
			//    priority += Constants.EpsilonSingle;
			//}
			this.Dictionary[priority] = item;

		//    this.Add( item, priority );
		//    this.ReNormalize();
		//}
		[CanBeNull]
		public TValue PullNext() {
			var highest = this.Dictionary.OrderByDescending( pair => pair.Key ).FirstOrDefault();

			return highest.Value;
		}

		// Single priority; switch ( positionial ) { case Positionial.Highest: { priority = this.Dictionary.Max( pair => pair.Key ) + Constants.EpsilonSingle; break; } case Positionial.Highish: { priority =
		// Randem.NextSingle( this.Dictionary.Average( pair => pair.Key ), this.Dictionary.Max( pair => pair.Key ) ) + Constants.EpsilonSingle; break; } case Positionial.Middle: { priority = this.Dictionary.Average( pair
		// => pair.Key ) + Constants.EpsilonSingle; break; } case Positionial.Lowish: { priority = Randem.NextSingle( this.Dictionary.Min( pair => pair.Key ), this.Dictionary.Average( pair => pair.Key ) ) -
		// Constants.EpsilonSingle; break; } case Positionial.Lowest: { priority = this.Dictionary.Min( pair => pair.Key ) - Constants.EpsilonSingle; break; } default: throw new ArgumentOutOfRangeException( nameof(
		// positionial ) ); }
		/// <summary>Renumber everything to between 0 and 1, evenly distributed and I need it to be *fast*.</summary>
		public void ReNormalize() {
			if ( this.Dictionary.IsEmpty ) {
				return;
			}

			var max = this.Dictionary.Max( pair => pair.Key );
			var min = this.Dictionary.Min( pair => pair.Key );
			var maxMinusMin = max - min;

			if ( Math.Abs( maxMinusMin ) < Single.Epsilon ) {
				return;
			}

			//I know other items could sneak in in here, but I don't care...
			foreach ( var key in this.Dictionary.Keys ) {
				var newPriority = ( key - min ) / maxMinusMin;

				if ( Math.Abs( key - newPriority ) < Single.Epsilon ) {
					continue;
				}

				this.Dictionary.TryRemove( key, out var value );
				this.Add( value, newPriority );
			}
		}

		///// <param name="item"></param>
		///// <param name="positionial"></param>
		//public void Add( TValue item, Positionial positionial ) {
		//    if ( this.Dictionary.IsEmpty ) {
		//        this.Add( item, Randem.NextSingle(0,1) );
		//        this.ReNormalize();
		//        return;
		//    }
		//public PriorityTime this[ TValue key ] {
		//    get {
		//        PriorityTime result;
		//        if ( !this.Dictionary.TryGetValue( key, out result ) ) {
		//            result = new PriorityTime();
		//        }
		//        return result;
		//    }

		//    set {
		//        this.Dictionary.AddOrUpdate(key, addValue: value, updateValueFactory: ( s, n ) => n );
		//    }
		//}

		///// <summary>
		/////     Calls the
		/////     <see
		/////         cref="ConcurrentDictionary{TKey,TValue}.AddOrUpdate(TKey,System.Func{TKey,TValue},System.Func{TKey,TValue,TValue})" />
		/////     method.
		///// </summary>
		///// <param name="item"></param>
		///// <param name="priorityTime"></param>
		//public void Add( TValue item, PriorityTime priorityTime ) {
		//    this.Dictionary.AddOrUpdate( item, priorityTime, ( o, f ) => priorityTime );
		//}
		//[CanBeNull]
		//public TValue Highest() => this.Dictionary.OrderByDescending( pair => pair.Value.Priority )
		//                                       .ThenBy( pair => pair.Value.Time )
		//                                       .Select( pair => pair.Key )
		//                                       .FirstOrDefault();

		//public ICollection<TValue> Keys() => this.Dictionary.Keys;

		//[CanBeNull]
		//public TValue Lowest() => this.Dictionary.OrderBy( pair => pair.Value.Priority )
		//                              .ThenBy( pair => pair.Value.Time )
		//                              .Select( pair => pair.Key )
		//                              .FirstOrDefault();

		//[CanBeNull]
		//public TValue PeekNext() => this.Highest();

		//[CanBeNull]
		//public Tuple<TValue, Single, DateTime> PullNext() {
		//    var peekNext = this.PeekNext();
		//    if ( null == peekNext ) {
		//        return default;
		//    }
		//    PriorityTime priorityTime;
		//    return this.Dictionary.TryRemove( peekNext, out priorityTime ) ? new Tuple<TValue, Single, DateTime>( peekNext, priorityTime.Priority, priorityTime.Time ) : null;
		//}

	}

}