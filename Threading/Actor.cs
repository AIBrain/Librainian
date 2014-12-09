namespace Librainian.Threading {
    using System;
    using System.Collections.Concurrent;
    using System.Diagnostics;
    using System.Threading;
    using Annotations;
    using FluentAssertions;

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
        public static Actor Do( Action action ) => new Actor( action );
    }
}