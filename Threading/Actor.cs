// Copyright 2018 Protiguous.
//
// This notice must be kept visible in the source.
//
// This section of source code belongs to Protiguous@Protiguous.com unless otherwise specified, or the original license has been overwritten by the automatic formatting of this code.
//
// Any unmodified sections of source code borrowed from other projects retain their original license and thanks goes to the Authors.
//
// Donations, royalties, and licenses can be paid via bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//
// Usage of the source code or compiled binaries is AS-IS. I am not responsible for Anything You Do.
//
// Contact me by email if you have any questions or helpful criticism.
//
// "Librainian/Actor.cs" was last cleaned by Protiguous on 2018/05/09 at 1:10 PM

namespace Librainian.Threading {

    using System;
    using System.Collections.Concurrent;
    using System.Diagnostics;
    using System.Threading;
    using FluentAssertions;
    using JetBrains.Annotations;
    using Magic;

    /// <summary>
    /// Fluent Actor test class.
    /// </summary>
    /// <copyright>
    ///     Protiguous 2018
    /// </copyright>
    /// <remarks>This class was just an experimental idea..</remarks>
    public class Actor : ABetterClassDispose {
        internal readonly BlockingCollection<Player> Actions = new BlockingCollection<Player>();

        [NotNull]
        internal Player Current;

        private Actor() {
            this.Current = new Player();
            this.Current.Should().NotBeNull( because: "out of memory or something?" );
            Debug.WriteLine( "Created a new player." );
        }

        public Actor( Action action ) : this() {
            if ( null != action ) {
                this.Current.TheAct = action;
            }
        }

        /// <summary>
        /// The beginning.
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        public static Actor Do( Action action ) => new Actor( action );

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
        /// Dispose any disposable members.
        /// </summary>
        public override void DisposeManaged() => this.Actions.Dispose();

        /// <summary>
        /// add a scene.
        /// </summary>
        /// <returns></returns>
        public Actor EndScene() {

            //how to inform user that we still need X? throw new ActorException?
            Debug.WriteLine( $"Adding Scene Ending (IsReady={this.IsReady()})." );

            this.Actions.Add( this.Current );
            this.Current = new Player();

            return this;
        }

        public Boolean IsReady() {
            Debug.WriteLine( "Checking if the player is ready." );
            this.Current.Should().NotBeNull( because: "logic" );

            if ( null == this.Current.TheAct ) {
                Debug.WriteLine( "The player is missing {0}.", nameof( this.Current.TheAct ) );
                return false;
            }

            if ( null == this.Current.ActingTimeout ) {
                Debug.WriteLine( "The player is missing {0}.", nameof( this.Current.ActingTimeout ) );
                return false;
            }

            if ( null == this.Current.OnTimeout ) {
                Debug.WriteLine( "The player is missing {0}.", nameof( this.Current.OnTimeout ) );
                return false;
            }

            if ( null == this.Current.OnSuccess ) {
                Debug.WriteLine( "The player is missing {0}.", nameof( this.Current.OnSuccess ) );
                return false;
            }

            Debug.WriteLine( "The player is ready." );
            return true;
        }

        internal class Player {

            [CanBeNull]
            public TimeSpan? ActingTimeout { get; internal set; }

            [CanBeNull]
            public CancellationToken? CancellationToken { get; internal set; }

            [CanBeNull]
            public Action OnSuccess { get; internal set; }

            [CanBeNull]
            public Action OnTimeout { get; internal set; }

            [CanBeNull]
            public Action TheAct { get; internal set; }

            //would events be okay here? are either Action or Events serializable?
        }
    }
}