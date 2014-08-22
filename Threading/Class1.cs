
namespace Librainian.Threading {
    using System;
    using System.Collections.Concurrent;
    using System.Diagnostics;
    using System.Threading;
    using System.Threading.Tasks;
    using Annotations;
    using FluentAssertions;
    using NUnit.Framework;

    /// <summary>
    /// Thrown when the actor fails.
    /// </summary>
    public class ActorException : Exception {

        public ActorException( String because ) {
            this.Reason = because;
        }

        public String Reason { get; private set; }
    }

    /// <summary>
    /// Fluent Actor test class.
    /// </summary>
    /// <copyright>Rick@AIBrain.org 2014</copyright>
    public class Actor {

        internal class Player {

            [CanBeNull]
            public Action TheAct { get; internal set; }

            [CanBeNull]
            public TimeSpan? ActingTimeout { get; internal set; }

            [CanBeNull]
            public Action OnTimeout { get; internal set; }  //would events be okay here? are either Action or Events serializable?

            [CanBeNull]
            public Action OnSuccess { get; internal set; }

            [CanBeNull]
            public CancellationToken? CancellationToken { get; internal set; }
        }

        [NotNull]
        internal Player Current = new Player();

        internal readonly BlockingCollection<Player> Actions = new BlockingCollection<Player>();

        private Actor() {
            this.Current = new Player();
            this.Current.Should().NotBeNull( because: "out of memory or something?" );
            Debug.WriteLine( "Created a new player." );
        }

        public Actor( Action action )
            : this() {
            if ( null != action ) {
                this.Current.TheAct = action;
            }
        }

        public Boolean IsReady() {
            Debug.WriteLine( "Checking if the player is ready." );
            this.Current.Should().NotBeNull( because: "logic" );

            if ( null == this.Current.TheAct ) {
                Debug.WriteLine( "The player is missing {0}.", this.Current.TheAct );
                return false;
            }

            if ( null == this.Current.ActingTimeout ) {
                Debug.WriteLine( "The player is missing {0}.", this.Current.ActingTimeout );
                return false;
            }

            if ( null == this.Current.OnTimeout ) {
                Debug.WriteLine( "The player is missing {0}.", this.Current.OnTimeout );
                return false;
            }

            if ( null == this.Current.OnSuccess ) {
                Debug.WriteLine( "The player is missing {0}.", this.Current.OnSuccess );
                return false;
            }

            Debug.WriteLine( "The player is ready." );
            return true;
        }

        /// <summary>
        /// add a scene if everything <see cref="IsReady"/>.
        /// </summary>
        /// <returns></returns>
        public Actor AddScene() {

            if ( this.IsReady() ) {
                this.Actions.Add( this.Current );
                this.Current = new Player();
            }

            return this;
        }

        /// <summary>
        /// add a scene.
        /// </summary>
        /// <returns></returns>
        public Actor EndScene() {

            //how to inform user that we still need X? throw new ActorException?
            Debug.WriteLine( "Adding Scene Ending (IsReady={0}).", this.IsReady() );

            this.Actions.Add( this.Current );
            this.Current = new Player();

            return this;
        }


        /// <summary>
        /// The beginning.
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        public static Actor Do( Action action ) {
            return new Actor( action );
        }


    }

    public static class ActorExtensions {

        [Test]
        public static async void ActorTest() {

            var bobDole = Actor.Do( () => Console.WriteLine( "Hello." ) )
                .LimitActing( TimeSpan.FromMilliseconds( 1 ), () => Console.WriteLine( "..uh..what?" ) )
                .Then( () => Console.WriteLine( "This is Bob Dole." ) )
                .Then( () => Console.WriteLine( "I just wanted to say." ) )
                .Then( () => Console.WriteLine( "Some about." ) )
                .EndScene()
                .EndScene();

            var bobBush = Actor.Do( () => {
                //repeat with some delays. to cause the limit on purpose
                Console.WriteLine( "Holler there." );
                Console.WriteLine( "Holler there." );
            } )
                .LimitActing( TimeSpan.FromMilliseconds( 1 ), () => Console.WriteLine( "..uh..what?" ) )
                .Then( () => Console.WriteLine( "This is Bob Bush." ) )
                .Then( () => Console.WriteLine( "I just wanted to say." ) )
                .Then( () => Console.WriteLine( "Some about." ) )
                .EndScene()
                .EndScene();

            var answer = await bobDole.Act();
            Console.WriteLine( answer );

        }

        public static Actor LimitActing( this Actor actor, TimeSpan timeSpan, Action onTimeout ) {
            actor.Current.ActingTimeout = timeSpan;    //if there was a value already there, just overwrite it.
            actor.Current.OnTimeout = onTimeout;    //if there was a value already there, just overwrite it.
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

        /// <summary>
        /// ".....and....ACTION!"
        /// </summary>
        /// <param name="actor"></param>
        public static async Task<Actor> Act( this Actor actor ) {
            //var cancel = new CancellationToken( false );
            foreach ( var player1 in actor.Actions.GetConsumingEnumerable() ) {
                if ( null == player1 ) {
                    continue;
                }

                var player = player1;

                if ( null == player.TheAct ) {
                    throw new ActorException( because: "the player.TheAct is null" );
                }

                if ( !player.ActingTimeout.HasValue ) {
                    throw new ActorException( because: "the player.TheTimeout is null" );
                }

                if ( null == player.OnSuccess ) {
                    Console.WriteLine();
                }


                var task = Task.Run( () => player.TheAct() );
                var runner = Task.Delay( player.ActingTimeout.Value );

                var result = await Task.WhenAny( task, runner );



            }

            return actor;
        }


    }
}
