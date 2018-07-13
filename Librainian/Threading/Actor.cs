// Copyright © Rick@AIBrain.org and Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
//
// This source code contained in "Actor.cs" belongs to Protiguous@Protiguous.com and
// Rick@AIBrain.org unless otherwise specified or the original license has
// been overwritten by formatting.
// (We try to avoid it from happening, but it does accidentally happen.)
//
// Any unmodified portions of source code gleaned from other projects still retain their original
// license and our thanks goes to those Authors. If you find your code in this source code, please
// let us know so we can properly attribute you and include the proper license and/or copyright.
//
// If you want to use any of our code, you must contact Protiguous@Protiguous.com or
// Sales@AIBrain.org for permission and a quote.
//
// Donations are accepted (for now) via
//     bitcoin:1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//     paypal@AIBrain.Org
//     (We're still looking into other solutions! Any ideas?)
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
// For business inquiries, please contact me at Protiguous@Protiguous.com
//
// Our website can be found at "https://Protiguous.com/"
// Our software can be found at "https://Protiguous.Software/"
// Our GitHub address is "https://github.com/Protiguous".
// Feel free to browse any source code we *might* make available.
//
// Project: "Librainian", "Actor.cs" was last formatted by Protiguous on 2018/07/13 at 1:39 AM.

namespace Librainian.Threading {

	using System;
	using System.Collections.Concurrent;
	using System.Diagnostics;
	using System.Threading;
	using FluentAssertions;
	using JetBrains.Annotations;
	using Magic;

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

	/// <summary>
	///     Fluent Actor test class.
	/// </summary>
	/// <copyright>
	///     Protiguous 2018
	/// </copyright>
	/// <remarks>This class was just an experimental idea..</remarks>
	public class Actor : ABetterClassDispose {

		[NotNull]
		internal Player Current;

		internal BlockingCollection<Player> Actions { get; } = new BlockingCollection<Player>();

		private Actor() {
			this.Current = new Player();
			this.Current.Should().NotBeNull( because: "out of memory or something?" );
			Debug.WriteLine( "Created a new player." );
		}

		public Actor( [CanBeNull] Action action ) : this() {
			if ( null != action ) { this.Current.TheAct = action; }
		}

		/// <summary>
		///     The beginning.
		/// </summary>
		/// <param name="action"></param>
		/// <returns></returns>
		[NotNull]
		public static Actor Do( Action action ) => new Actor( action );

		/// <summary>
		///     add a scene if everything <see cref="IsReady" />.
		/// </summary>
		/// <returns></returns>
		[NotNull]
		public Actor AddScene() {
			if ( this.IsReady() ) {
				this.Actions.Add( this.Current );
				this.Current = new Player();
			}

			return this;
		}

		/// <summary>
		///     Dispose any disposable members.
		/// </summary>
		public override void DisposeManaged() => this.Actions.Dispose();

		/// <summary>
		///     add a scene.
		/// </summary>
		/// <returns></returns>
		[NotNull]
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
	}
}