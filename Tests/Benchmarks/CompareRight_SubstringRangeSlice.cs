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
// File "CompareRight_SubstringRangeSlice.cs" last touched on 2021-08-23 at 5:45 AM by Protiguous.

namespace Benchmarks;

using System;
using BenchmarkDotNet.Attributes;
using Librainian.Exceptions;
using Librainian.Parsing;

[MemoryDiagnoser]
public class CompareRightSubstringRangeSlice {

	private const String Default_TestAddress = "al. Księcia Józefa Poniatowskiego 1, 03-901 Warszawa";

	[Benchmark]
	public void WithRightSlice() {
		var local = Default_TestAddress[ .. ];
		var right = local.AsSpan().Right( "Warszawa".Length );
		if ( right != "Warszawa" ) {
			throw new NullException( nameof( right ) );
		}
	}

	[Benchmark( Baseline = true )]
	public void WithRightOldWay() {
		var local = Default_TestAddress[..];
		var right = local.Right( "Warszawa".Length );
		if ( right != "Warszawa" ) {
			throw new NullException( nameof( right ) );
		}
	}

}