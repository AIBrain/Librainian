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
//
// Disclaimer:  Usage of the source code or binaries is AS-IS.
// No warranties are expressed, implied, or given.
// We are NOT responsible for Anything You Do With Our Code.
// We are NOT responsible for Anything You Do With Our Executables.
// We are NOT responsible for Anything You Do With Your Computer.
//
//
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
// For business inquiries, please contact me at Protiguous@Protiguous.com.
// Our software can be found at "https://Protiguous.com/Software/"
// Our GitHub address is "https://github.com/Protiguous".
//
// File "FILE_TIME.cs" last formatted on 2022-12-22 at 5:15 PM by Protiguous.

namespace Librainian.FileSystem.Pri.LongPath;

using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

[StructLayout( LayoutKind.Sequential )]
public struct FILE_TIME {

	public FILE_TIME( Int64 fileTime ) {
		this.ftTimeLow = ( UInt32 )fileTime;
		this.ftTimeHigh = ( UInt32 )( fileTime >> 32 );
	}

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public Int64 ToTicks() => ( ( Int64 )this.ftTimeHigh << 32 ) + this.ftTimeLow;

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public DateTime ToDateTime() => DateTime.FromFileTime( this.ToTicks() );

	public readonly UInt32 ftTimeLow;

	public readonly UInt32 ftTimeHigh;
}