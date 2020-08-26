// Copyright © Protiguous. All Rights Reserved.
// This entire copyright notice and license must be retained and must be kept visible in any binaries, libraries, repositories, or source code (directly or derived) from our binaries, libraries, projects, solutions, or applications.
// All source code belongs to Protiguous@Protiguous.com unless otherwise specified or the original license has been overwritten by formatting. (We try to avoid it from happening, but it does accidentally happen.)
// Any unmodified portions of source code gleaned from other sources still retain their original license and our thanks goes to those Authors.
// If you find your code unattributed in this source code, please let us know so we can properly attribute you and include the proper license and/or copyright(s).
// If you want to use any of our code in a commercial project, you must contact Protiguous@Protiguous.com for permission, license, and a quote.
// 
// Donations, payments, and royalties are accepted via bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2 and PayPal: Protiguous@Protiguous.com
// 
// ====================================================================
// Disclaimer:  Usage of the source code or binaries is AS-IS.
// No warranties are expressed, implied, or given.
// We are NOT responsible for Anything You Do With Our Code.
// We are NOT responsible for Anything You Do With Our Executables.
// We are NOT responsible for Anything You Do With Your Computer.
// ====================================================================
// 
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
// For business inquiries, please contact me at Protiguous@Protiguous.com.
// Our software can be found at "https://Protiguous.Software/"
// Our GitHub address is "https://github.com/Protiguous".
// 
// File "CountDownWatch.cs" last formatted on 2020-08-14 at 8:38 PM.

#nullable enable

namespace Librainian.Measurement.Time {

	using System;
	using JetBrains.Annotations;
	using Logging;
	using Newtonsoft.Json;
	using Threading;
	using Threadsafe;

	/// <summary>
	///     The 'reverse' of the Stopwatch struct.
	///     <remarks>
	///         //TODO needs unit testing.
	///     </remarks>
	/// </summary>
	[JsonObject( MemberSerialization.Fields )]
	public class CountDownWatch {

		/// <summary></summary>
		/// <param name="countdown"></param>
		/// <param name="liftoff">Action to invoke when countdown reaches zero.</param>
		public CountDownWatch( TimeSpan countdown, [CanBeNull] Action? liftoff = null ) {
			if ( countdown < TimeSpan.Zero ) {
				throw new ArgumentOutOfRangeException( nameof( countdown ), "Must be a positive value." );
			}

			this.Countdown = countdown;
			this.TargetTime = DateTime.UtcNow.Add( this.Countdown );

			this.Liftoff = () => {
				try {
					this.HasLaunched = true;
					liftoff?.Invoke();
				}
				catch ( Exception exception ) {
					exception.Log();
				}
			};

			this.Timer = this.Countdown.CreateTimer( () => {
				this.Stop();
				this.Liftoff();
			} ).Once();
		}

		[NotNull]
		private Action Liftoff { get; }

		private FluentTimer Timer { get; set; }

		public TimeSpan Countdown { get; }

		public VolatileBoolean HasLaunched { get; private set; }

		public VolatileBoolean IsRunning { get; private set; }

		public DateTime TargetTime { get; private set; }

		public DateTime WhenStarted { get; private set; }

		public DateTime WhenStopped { get; private set; }

		public TimeSpan Remaining() {
			if ( this.IsRunning ) {
				return this.Countdown.Subtract( DateTime.UtcNow - this.WhenStarted );
			}

			if ( this.HasLaunched ) {
				return this.Countdown.Subtract( this.WhenStopped - this.WhenStarted );
			}

			throw new InvalidOperationException( "???" );
		}

		public void Start() {
			this.WhenStarted = DateTime.UtcNow;
			this.IsRunning = true;
			this.Timer.Start();
		}

		public void Stop() {
			this.IsRunning = false;
			this.WhenStopped = DateTime.UtcNow;
			this.Timer.Stop();
		}

	}

}