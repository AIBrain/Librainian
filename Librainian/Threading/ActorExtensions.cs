// Copyright © Rick@AIBrain.Org and Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our source code, binaries, libraries, projects, or solutions.
//
// This source code contained in "ActorExtensions.cs" belongs to Protiguous@Protiguous.com
// and Rick@AIBrain.org and unless otherwise specified or the original license has been
// overwritten by automatic formatting.
// (We try to avoid it from happening, but it does accidentally happen.)
//
// Any unmodified portions of source code gleaned from other projects still retain their original
// license and our Thanks goes to those Authors. If you find your code in this source code, please
// let us know so we can properly attribute you and include the proper license and/or copyright.
//
// If you want to use any of our code, you must contact Protiguous@Protiguous.com or
// Sales@AIBrain.org for permission and a quote.
//
// Donations are accepted (for now) via
//    bitcoin:1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//    paypal@AIBrain.Org
//    (We're still looking into other solutions! Any ideas?)
//
// =========================================================
// Disclaimer:  Usage of the source code or binaries is AS-IS.
//    No warranties are expressed, implied, or given.
//    We are NOT responsible for Anything You Do With Our Code.
//    We are NOT responsible for Anything You Do With Our Executables.
//    We are NOT responsible for Anything You Do With Your Computer.
// =========================================================
//
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
// For business inquiries, please contact me at Protiguous@Protiguous.com .
//
// Our website can be found at "https://Protiguous.com/"
// Our software can be found at "https://Protiguous.Software/"
// Our GitHub address is "https://github.com/Protiguous".
// Feel free to browse any source code we *might* make available.
//
// ***  Project "Librainian"  ***
// File "ActorExtensions.cs" was last formatted by Protiguous on 2018/06/26 at 1:42 AM.

namespace Librainian.Threading {

	using System;
	using System.Linq;
	using System.Threading.Tasks;
	using JetBrains.Annotations;
	using NUnit.Framework;

	public static class ActorExtensions {

		/// <summary>
		///     ".....and....ACTION!"
		/// </summary>
		/// <param name="actor"></param>
		[ItemNotNull]
		public static async Task<Actor> Act( [NotNull] this Actor actor ) {

			//var cancel = new CancellationToken( false );
			foreach ( var player in actor.Actions.GetConsumingEnumerable().Where( player1 => null != player1 ) ) {

				//if ( null == player.TheAct ) {
				//    throw new ActorException( because: "the player.TheAct is null" );
				//}

				if ( !player.ActingTimeout.HasValue ) {
					throw new ActorException( because: "the player.TheTimeout is null" );
				}

				if ( null == player.OnSuccess ) {
					Console.WriteLine();
				}

				if ( player.TheAct != null ) {
					var task = Task.Run( () => player.TheAct() );
					var runner = Task.Delay( player.ActingTimeout.Value );

					await Task.WhenAny( task, runner );
				}
			}

			return actor;
		}

		[Test]
		public static async Task ActorTest() {
			var bobDole = Actor.Do( () => Console.WriteLine( "Hello." ) ).LimitActing( TimeSpan.FromMilliseconds( 1 ), () => Console.WriteLine( "..uh..what?" ) )
				.Then( () => Console.WriteLine( "This is Bob Dole." ) ).Then( () => Console.WriteLine( "I just wanted to say." ) ).Then( () => Console.WriteLine( "Some about." ) ).EndScene().EndScene();

			//var bobBush = Actor.Do( () => {
			//    //repeat with some delays. to cause the limit on purpose
			//    Console.WriteLine( "Holler there." );
			//    Console.WriteLine( "Holler there." );
			//} ).LimitActing( TimeSpan.FromMilliseconds( 1 ), () => Console.WriteLine( "..uh..what?" ) ).Then( () => Console.WriteLine( "This is Bob Bush." ) ).Then( () => Console.WriteLine( "I just wanted to say." ) ).Then( () => Console.WriteLine( "Some about." ) ).EndScene().EndScene();

			var answer = await bobDole.Act();
			Console.WriteLine( answer );
		}

		[NotNull]
		public static Actor LimitActing( [NotNull] this Actor actor, TimeSpan timeSpan, Action onTimeout ) {
			actor.Current.ActingTimeout = timeSpan; //if there was a value already there, just overwrite it.
			actor.Current.OnTimeout = onTimeout; //if there was a value already there, just overwrite it.

			return actor;
		}

		public static Actor Then( [NotNull] this Actor actor, Action action ) {
			if ( null == actor.Current.OnSuccess ) {
				actor.Current.OnSuccess = action;

				return actor;
			}

			return actor.IsReady() ? actor.EndScene().Then( action ) : actor;
		}
	}
}