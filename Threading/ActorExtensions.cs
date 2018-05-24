// Copyright © 1995-2018 to Rick@AIBrain.org and Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
//
// This source code contained in "ActorExtensions.cs" belongs to Rick@AIBrain.org and
// Protiguous@Protiguous.com unless otherwise specified or the original license has
// been overwritten by automatic formatting.
// (We try to avoid it from happening, but it does accidentally happen.)
//
// Any unmodified portions of source code gleaned from other projects still retain their original
// license and our thanks goes to those Authors. If you find your code in this source code, please
// let us know so we can properly attribute you and include the proper license and/or copyright.
//
// Donations, royalties from any software that uses any of our code, or license fees can be paid
// to us via bitcoin at the address 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2.
//
// =========================================================
// Usage of the source code or binaries is AS-IS.
// No warranties are expressed, implied, or given.
// We are NOT responsible for Anything You Do With Our Code.
// =========================================================
//
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
//
// "Librainian/Librainian/ActorExtensions.cs" was last formatted by Protiguous on 2018/05/22 at 3:51 PM.

namespace Librainian.Threading {

    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using NUnit.Framework;

    public static class ActorExtensions {

        /// <summary>
        ///     ".....and....ACTION!"
        /// </summary>
        /// <param name="actor"></param>
        public static async Task<Actor> Act( this Actor actor ) {

            //var cancel = new CancellationToken( false );
            foreach ( var player in actor.Actions.GetConsumingEnumerable().Where( player1 => null != player1 ) ) {

                //if ( null == player.TheAct ) {
                //    throw new ActorException( because: "the player.TheAct is null" );
                //}

                if ( !player.ActingTimeout.HasValue ) { throw new ActorException( because: "the player.TheTimeout is null" ); }

                if ( null == player.OnSuccess ) { Console.WriteLine(); }

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
            var bobDole = Actor.Do( () => Console.WriteLine( "Hello." ) ).LimitActing( TimeSpan.FromMilliseconds( 1 ), () => Console.WriteLine( "..uh..what?" ) ).Then( () => Console.WriteLine( "This is Bob Dole." ) )
                .Then( () => Console.WriteLine( "I just wanted to say." ) ).Then( () => Console.WriteLine( "Some about." ) ).EndScene().EndScene();

            //var bobBush = Actor.Do( () => {
            //    //repeat with some delays. to cause the limit on purpose
            //    Console.WriteLine( "Holler there." );
            //    Console.WriteLine( "Holler there." );
            //} ).LimitActing( TimeSpan.FromMilliseconds( 1 ), () => Console.WriteLine( "..uh..what?" ) ).Then( () => Console.WriteLine( "This is Bob Bush." ) ).Then( () => Console.WriteLine( "I just wanted to say." ) ).Then( () => Console.WriteLine( "Some about." ) ).EndScene().EndScene();

            var answer = await bobDole.Act();
            Console.WriteLine( answer );
        }

        public static Actor LimitActing( this Actor actor, TimeSpan timeSpan, Action onTimeout ) {
            actor.Current.ActingTimeout = timeSpan; //if there was a value already there, just overwrite it.
            actor.Current.OnTimeout = onTimeout; //if there was a value already there, just overwrite it.

            return actor;
        }

        public static Actor Then( this Actor actor, Action action ) {
            if ( null == actor.Current.OnSuccess ) {
                actor.Current.OnSuccess = action;

                return actor;
            }

            return actor.IsReady() ? actor.EndScene().Then( action ) : actor;
        }
    }
}