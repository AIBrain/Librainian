// Copyright © Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible in any binaries, libraries, repositories, or source code (directly or derived) from our binaries, libraries, projects, solutions, or applications.
//
// All source code belongs to Protiguous@Protiguous.com unless otherwise specified or the original license has been overwritten by formatting. (We try to avoid it from happening, but it does accidentally happen.)
//
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
// File "UsableSemaphoreThrottle.cs" last touched on 2021-03-07 at 1:44 PM by Protiguous.

namespace Librainian.Threading {

	using System;
	using System.Threading.Tasks;
	using JetBrains.Annotations;

	/// <summary>http://www.tomdupont.net/2016/03/how-to-release-semaphore-with-using.html</summary>
	public class UsableSemaphoreThrottle : IUsableSemaphore {

		[NotNull]
		private readonly IUsableSemaphore _semaphore;

		[NotNull]
		private readonly IThrottle _throttle;

		public UsableSemaphoreThrottle( TimeSpan interval, Int32 initialCount ) {
			this._throttle = new Throttle( interval );
			this._semaphore = new UsableSemaphoreSlim( initialCount );
		}

		public UsableSemaphoreThrottle( TimeSpan interval, Int32 initialCount, Int32 maxCount ) {
			this._throttle = new Throttle( interval );
			this._semaphore = new UsableSemaphoreSlim( initialCount, maxCount );
		}

		public UsableSemaphoreThrottle( [NotNull] IThrottle throttle, [NotNull] IUsableSemaphore semaphore ) {
			this._throttle = throttle ?? throw new ArgumentNullException( nameof( throttle ) );
			this._semaphore = semaphore ?? throw new ArgumentNullException( nameof( semaphore ) );
		}

		public void Dispose() {
			this._semaphore.Dispose();
			GC.SuppressFinalize( this );
		}

		public async Task<IUsableSemaphoreWrapper> WaitAsync() {
			IUsableSemaphoreWrapper? wrapper = default;

			try {
				wrapper = await this._semaphore.WaitAsync().ConfigureAwait( false );
				await this._throttle.WaitAsync().ConfigureAwait( false );

				return wrapper;
			}
			catch ( Exception ) {
				wrapper?.Dispose();

				throw;
			}
		}
	}
}