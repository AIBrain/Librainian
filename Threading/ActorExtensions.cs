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
// "Librainian/ActorExtensions.cs" was last cleaned by Protiguous on 2016/06/18 at 10:57 PM

namespace Librainian.Threading {

    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using NUnit.Framework;

    public static class ActorExtensions {

        /// <summary>".....and....ACTION!"</summary>
        /// <param name="actor"></param>
        public static async Task<Actor> Act( this Actor actor ) {

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

                    var result = await Task.WhenAny( task, runner );
                }
            }

            return actor;
        }

        [Test]
        public static async Task ActorTest() {
            var bobDole = Actor.Do( () => Console.WriteLine( "Hello." ) ).LimitActing( TimeSpan.FromMilliseconds( 1 ), () => Console.WriteLine( "..uh..what?" ) ).Then( () => Console.WriteLine( "This is Bob Dole." ) ).Then( () => Console.WriteLine( "I just wanted to say." ) ).Then( () => Console.WriteLine( "Some about." ) ).EndScene().EndScene();

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

            if ( actor.IsReady() ) {
                return actor.EndScene().Then( action );
            }

            return actor;
        }
    }
}