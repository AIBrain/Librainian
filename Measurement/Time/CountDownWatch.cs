// Copyright © 1995-2018 to Rick@AIBrain.org and Protiguous. All Rights Reserved.
// 
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
// 
// This source code contained in "CountDownWatch.cs" belongs to Rick@AIBrain.org and
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
// Our software can be found at "https://Protiguous.Software/"
// Our GitHub address is "https://github.com/Protiguous".
// Feel free to browse any source code we might have available.
// 
// ***  Project "Librainian"  ***
// File "CountDownWatch.cs" was last formatted by Protiguous on 2018/06/04 at 4:13 PM.

namespace Librainian.Measurement.Time {

	using System;
	using System.Threading;
	using FluentAssertions;
	using JetBrains.Annotations;
	using Newtonsoft.Json;
	using NUnit.Framework;
	using Threading;
	using Timer = System.Timers.Timer;

	[TestFixture]
	public static class TestCoundownWatch {

		[Test]
		public static void TestCountdown() {
			var watch = new CountDownWatch( Seconds.Three, () => "Launched!".Info() );
			watch.Start();

			do {
				$"{watch.Remaining().Simpler()}".Info();
				Thread.Sleep( 333 );
			} while ( !watch.HasLaunched() );

			watch.Remaining().Should().BeLessThan( Seconds.One );

			$"{watch.Remaining().Simpler()}".Info();
			$"{watch.Remaining().Simpler()}".Info();
			$"{watch.Remaining().Simpler()}".Info();
		}

	}

	/// <summary>
	///     The 'reverse' of the <see cref="StopWatch" /> class.
	///     //TODO needs unit testing.
	/// </summary>
	[JsonObject( MemberSerialization.Fields )]
	public class CountDownWatch {

		public TimeSpan Countdown { get; }

		public DateTime TargetTime { get; private set; }

		public DateTime WhenStarted { get; private set; }

		public DateTime WhenStopped { get; private set; }

		[NotNull]
		private Action Liftoff { get; }

		private Timer Timer { get; set; }

		private volatile Boolean _hasLaunched;

		private volatile Boolean _isRunning;

		public Boolean HasLaunched() => this._hasLaunched;

		public Boolean IsRunning() => this._isRunning;

		public TimeSpan Remaining() {
			if ( this.IsRunning() ) { return this.Countdown.Subtract( DateTime.UtcNow - this.WhenStarted ); }

			if ( this.HasLaunched() ) { return this.Countdown.Subtract( this.WhenStopped - this.WhenStarted ); }

			throw new InvalidOperationException( "???" );
		}

		public void Start() {
			this.WhenStarted = DateTime.UtcNow;
			this._isRunning = true;

			this.Timer = this.Countdown.CreateTimer( () => {
				this.Stop();
				this.Liftoff();
			} ).Once().AndStart();
		}

		public void Stop() {
			this._isRunning = false;
			this.WhenStopped = DateTime.UtcNow;
			this.Timer.Stop();
		}

		/// <summary>
		/// </summary>
		/// <param name="countdown"></param>
		/// <param name="liftoff">Action to invoke when countdown reaches zero.</param>
		public CountDownWatch( TimeSpan countdown, Action liftoff = null ) {
			if ( countdown < TimeSpan.Zero ) { throw new ArgumentOutOfRangeException( nameof( countdown ), "Must be a positive value." ); }

			this.Countdown = countdown;
			this.TargetTime = DateTime.UtcNow.Add( this.Countdown );

			this.Liftoff = () => {
				try {
					this._hasLaunched = true;
					liftoff?.Invoke();
				}
				catch ( Exception exception ) { exception.More(); }
			};
		}

	}

}