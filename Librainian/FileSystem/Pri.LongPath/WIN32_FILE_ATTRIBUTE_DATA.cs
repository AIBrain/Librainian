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
// Our software can be found at "https://Protiguous.com/Software/"
// Our GitHub address is "https://github.com/Protiguous".
//
// File "WIN32_FILE_ATTRIBUTE_DATA.cs" last formatted on 2022-12-22 at 7:20 AM by Protiguous.

namespace Librainian.FileSystem.Pri.LongPath;

using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;

[Serializable]
[SuppressMessage( "Design", "CA1051:Do not declare visible instance fields" )]
public struct WIN32_FILE_ATTRIBUTE_DATA {

	public FileAttributes fileAttributes;

	public Int32 fileSizeHigh;

	public Int32 fileSizeLow;

	public UInt32 ftCreationTimeHigh;

	public UInt32 ftCreationTimeLow;

	public UInt32 ftLastAccessTimeHigh;

	public UInt32 ftLastAccessTimeLow;

	public UInt32 ftLastWriteTimeHigh;

	public UInt32 ftLastWriteTimeLow;

	public void PopulateFrom( WIN32_FIND_DATA findData ) {
		this.fileAttributes = findData.dwFileAttributes;
		this.ftCreationTimeLow = ( UInt32 )findData.ftCreationTime.dwLowDateTime;
		this.ftCreationTimeHigh = ( UInt32 )findData.ftCreationTime.dwHighDateTime;
		this.ftLastAccessTimeLow = ( UInt32 )findData.ftLastAccessTime.dwLowDateTime;
		this.ftLastAccessTimeHigh = ( UInt32 )findData.ftLastAccessTime.dwHighDateTime;
		this.ftLastWriteTimeLow = ( UInt32 )findData.ftLastWriteTime.dwLowDateTime;
		this.ftLastWriteTimeHigh = ( UInt32 )findData.ftLastWriteTime.dwHighDateTime;
		this.fileSizeHigh = findData.nFileSizeHigh;
		this.fileSizeLow = findData.nFileSizeLow;
	}
}