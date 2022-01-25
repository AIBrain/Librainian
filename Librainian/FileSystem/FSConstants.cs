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
// File "FSConstants.cs" last formatted on 2022-12-22 at 5:16 PM by Protiguous.

namespace Librainian.FileSystem;

using System;
using System.Runtime.CompilerServices;

/// <summary>constants lifted from winioctl.h from platform sdk</summary>
internal static class FSConstants {

	private const UInt32 FileAnyAccess = 0;

	private const UInt32 FileDeviceFileSystem = 0x00000009;

	private const UInt32 FileSpecialAccess = FileAnyAccess;

	private const UInt32 MethodBuffered = 0;

	private const UInt32 MethodNeither = 3;

	public static readonly UInt32 FsctlGetRetrievalPointers = CTL_CODE( FileDeviceFileSystem, 28, MethodNeither, FileAnyAccess );

	public static readonly UInt32 FsctlGetVolumeBitmap = CTL_CODE( FileDeviceFileSystem, 27, MethodNeither, FileAnyAccess );

	public static readonly UInt32 FsctlMoveFile = CTL_CODE( FileDeviceFileSystem, 29, MethodBuffered, FileSpecialAccess );

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	private static UInt32 CTL_CODE( UInt32 deviceType, UInt32 function, UInt32 method, UInt32 access ) => ( deviceType << 16 ) | ( access << 14 ) | ( function << 2 ) | method;
}