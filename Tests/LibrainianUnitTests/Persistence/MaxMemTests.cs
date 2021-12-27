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
// File "MaxMemTests.cs" last touched on 2021-03-07 at 3:20 PM by Protiguous.

namespace LibrainianUnitTests.Persistence;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

//[TestFixture]
public static class MaxMemTests {

	//[Test]
	public static void test_max_ReaderWriterLockSlim() {
		GC.Collect( 2, GCCollectionMode.Forced, true, true );
		GC.Collect( 2, GCCollectionMode.Forced, true, true );

		const Int32 limit = Int32.MaxValue / 2;

		var list = new List<ReaderWriterLockSlim>( limit );

		for ( var i = 0; i < limit; i++ ) {
			try {
				list.Add( new ReaderWriterLockSlim() ); //134,217,728
			}
			catch ( OutOfMemoryException ) {
				Console.WriteLine( "Trimming list.." );
				list.TrimExcess();
				Console.Write( "Collecting garbage.." );
				GC.Collect( 2, GCCollectionMode.Forced, true, true );
				Console.WriteLine( "collected." );
			}
			catch ( Exception exception ) {
				Debug.WriteLine( exception.ToString() );

				break;
			}

		}

		Console.WriteLine( $"ReaderWriterLockSlim count={list.Count}." );
	}

}