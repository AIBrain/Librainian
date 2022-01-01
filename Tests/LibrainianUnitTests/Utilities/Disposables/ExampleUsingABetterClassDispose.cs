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
// File "ExampleUsingABetterClassDispose.cs" last touched on 2021-03-07 at 3:20 PM by Protiguous.

#nullable enable

namespace LibrainianUnitTests.Utilities.Disposables;

using System;
using System.Diagnostics;
using System.IO;
using Librainian.Maths;
using Librainian.Utilities.Disposables;
using Microsoft.IO;
using NUnit.Framework;

[TestFixture]
public class ExampleUsingABetterClassDispose : ABetterClassDispose {

	private static RecyclableMemoryStreamManager MemoryStreamManager { get; } = new( MathConstants.Sizes.OneMegaByte, MathConstants.Sizes.OneGigaByte );

	private RecyclableMemoryStream? _memoryStream = new(MemoryStreamManager);

	private SysComObject? _sysComObject = new();

	public ExampleUsingABetterClassDispose() : base( nameof( ExampleUsingABetterClassDispose ) ) => this._sysComObject?.ReserveMemory();

	[Test]
	public override void DisposeManaged() {
		using ( this._memoryStream ) {
			this._memoryStream = null;
		}

		base.DisposeManaged();
	}

	[Test]
	public override void DisposeNative() {
		this._sysComObject?.ReleaseMemory();
		this._sysComObject = null;
		base.DisposeNative();
	}

}

/// <summary>
///     A fake COM interface object.
/// </summary>
public class SysComObject {

	private static readonly Random RNG = new();

	private Byte[]? fakeInternalMemoryAllocation;

	public SysComObject() => this.fakeInternalMemoryAllocation = null;

	public void ReleaseMemory() {
		Debug.Assert( this.fakeInternalMemoryAllocation != null, "" );
		this.fakeInternalMemoryAllocation = null;
		Debug.Assert( this.fakeInternalMemoryAllocation == null, "" );
	}

	public void ReserveMemory() {
		this.fakeInternalMemoryAllocation = new Byte[ RNG.Next( 128, 256 ) ];
		Debug.Assert( this.fakeInternalMemoryAllocation != null, "" );
		Debug.WriteLine( $"{this.fakeInternalMemoryAllocation.Length} bytes allocated to fake COM object." );
	}

}