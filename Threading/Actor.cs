// Copyright © 1995-2018 to Rick@AIBrain.org and Protiguous. All Rights Reserved. This ENTIRE copyright notice and file header MUST BE KEPT VISIBLE in any source code derived from or used from our libraries and projects.
//
// ========================================================= This section of source code, "Actor.cs", belongs to Rick@AIBrain.org and Protiguous@Protiguous.com unless otherwise specified OR the original license has been
// overwritten by the automatic formatting. (We try to avoid that from happening, but it does happen.)
//
// Any unmodified portions of source code gleaned from other projects still retain their original license and our thanks goes to those Authors. =========================================================
//
// Donations (more please!), royalties from any software that uses any of our code, and license fees can be paid to us via bitcoin at the address 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2.
//
// ========================================================= Usage of the source code or compiled binaries is AS-IS. No warranties are expressed or implied. I am NOT responsible for Anything You Do With Our Code. =========================================================
//
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
//
// "Librainian/Actor.cs" was last cleaned by Protiguous on 2018/05/15 at 4:23 AM.

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
            if ( null != action ) { this.Current.TheAct = action; }
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