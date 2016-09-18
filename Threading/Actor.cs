// Copyright 2016 Rick@AIBrain.org.
//
// This notice must be kept visible in the source.
//
// This section of source code belongs to Rick@AIBrain.Org unless otherwise specified, or the
// original license has been overwritten by the automatic formatting of this code. Any unmodified
// sections of source code borrowed from other projects retain their original license and thanks
// goes to the Authors.
//
// Donations and royalties can be paid via
//  PayPal: paypal@aibrain.org
//  bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//  litecoin: LeUxdU2w3o6pLZGVys5xpDZvvo8DUrjBp9
//
// Usage of the source code or compiled binaries is AS-IS. I am not responsible for Anything You Do.
//
// Contact me by email if you have any questions or helpful criticism.
//
// "Librainian/Actor.cs" was last cleaned by Rick on 2016/06/18 at 10:57 PM

namespace Librainian.Threading {

    using System;
    using System.Collections.Concurrent;
    using System.Diagnostics;
    using System.Threading;
    using FluentAssertions;
    using JetBrains.Annotations;
    using Magic;

    /// <summary>Fluent Actor test class.</summary>
    /// <copyright>Rick@AIBrain.org 2014</copyright>
    /// <remarks>Don't use this class, was just an idea...</remarks>
    public class Actor : ABetterClassDispose {
        internal readonly BlockingCollection<Player> Actions = new BlockingCollection<Player>();

        [NotNull]
        internal Player Current;

        public Actor( Action action ) : this() {
            if ( null != action ) {
                this.Current.TheAct = action;
            }
        }

        private Actor() {
            this.Current = new Player();
            this.Current.Should().NotBeNull( because: "out of memory or something?" );
            Debug.WriteLine( "Created a new player." );
        }

        /// <summary>The beginning.</summary>
        /// <param name="action"></param>
        /// <returns></returns>
        public static Actor Do( Action action ) => new Actor( action );

        /// <summary>add a scene if everything <see cref="IsReady" />.</summary>
        /// <returns></returns>
        public Actor AddScene() {
            if ( this.IsReady() ) {
                this.Actions.Add( this.Current );
                this.Current = new Player();
            }

            return this;
        }

        /// <summary>add a scene.</summary>
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
            public TimeSpan? ActingTimeout {
                get; internal set;
            }

            [CanBeNull]
            public CancellationToken? CancellationToken {
                get; internal set;
            }

            [CanBeNull]
            public Action OnSuccess {
                get; internal set;
            }

            [CanBeNull]
            public Action OnTimeout {
                get; internal set;
            }

            [CanBeNull]
            public Action TheAct {
                get; internal set;
            }

            //would events be okay here? are either Action or Events serializable?
        }

        /// <summary>
        /// Dispose any disposable members.
        /// </summary>
        protected override void DisposeManaged() { this.Actions.Dispose(); }

    }
}