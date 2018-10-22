// Copyright © Rick@AIBrain.Org and Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our source code, binaries, libraries, projects, or solutions.
//
// This source code contained in "CoundownWatchTests.cs" belongs to Protiguous@Protiguous.com
// and Rick@AIBrain.org and unless otherwise specified or the original license has been
// overwritten by automatic formatting.
// (We try to avoid it from happening, but it does accidentally happen.)
//
// Any unmodified portions of source code gleaned from other projects still retain their original
// license and our Thanks goes to those Authors. If you find your code in this source code, please
// let us know so we can properly attribute you and include the proper license and/or copyright.
//
// If you want to use any of our code, you must contact Protiguous@Protiguous.com or
// Sales@AIBrain.org for permission and a quote.
//
// Donations are accepted (for now) via
//    bitcoin:1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//    paypal@AIBrain.Org
//    (We're still looking into other solutions! Any ideas?)
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
// Our website can be found at "https://Protiguous.com/"
// Our software can be found at "https://Protiguous.Software/"
// Our GitHub address is "https://github.com/Protiguous".
// Feel free to browse any source code we *might* make available.
//
// ***  Project "LibrainianTests"  ***
// File "CoundownWatchTests.cs" was last formatted by Protiguous on 2018/06/26 at 5:22 PM.

namespace LibrainianTests {

	using System.Threading;
	using FluentAssertions;
	using Librainian.Logging;
	using Librainian.Measurement.Time;
	using NUnit.Framework;

	[TestFixture]
	public static class CoundownWatchTests {

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
}