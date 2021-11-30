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
// File "FileAttributeData.cs" last touched on 2021-10-13 at 4:26 PM by Protiguous.

namespace Librainian.FileSystem;

using System;
using System.Diagnostics;
using System.IO;
using Exceptions;
using OperatingSystem;

/// <summary>Modern version of <see cref="NativeMethods.WIN32_FILE_ATTRIBUTE_DATA" />.</summary>
public struct FileAttributeData {

	public DateTime? CreationTime { get; internal set; }

	public Boolean? Exists { get; internal set; }

	public FileAttributes? FileAttributes { get; internal set; }

	public Int64? FileHashCode { get; internal set; }

	public UInt64? FileSize { get; internal set; }

	public DateTime? LastAccessTime { get; internal set; }

	public DateTime? LastWriteTime { get; internal set; }

	/// <summary>Populate all properties with null values.</summary>
	public FileAttributeData( Boolean _ = true ) : this() => this.Reset();

	/// <summary>Populates from a <see cref="NativeMethods.WIN32_FILE_ATTRIBUTE_DATA" /> struct.</summary>
	/// <param name="fileAttributeData"></param>
	[DebuggerStepThrough]
	public FileAttributeData( NativeMethods.WIN32_FILE_ATTRIBUTE_DATA fileAttributeData ) {
		this.FileAttributes = fileAttributeData.dwFileAttributes;
		this.CreationTime = fileAttributeData.ftCreationTime.ToDateTime();
		this.LastAccessTime = fileAttributeData.ftLastAccessTime.ToDateTime();
		this.LastWriteTime = fileAttributeData.ftLastWriteTime.ToDateTime();
		this.FileSize = ( ( UInt64 )fileAttributeData.nFileSizeHigh << 32 ) + fileAttributeData.nFileSizeLow;
		this.Exists = true;
		this.FileHashCode = default( Int64? );
	}

	/// <summary>Populates from a <see cref="NativeMethods.Win32FindData" /> struct.</summary>
	/// <param name="findData"></param>
	[DebuggerStepThrough]
	public FileAttributeData( NativeMethods.Win32FindData findData ) {
		this.FileAttributes = findData.dwFileAttributes;
		this.CreationTime = findData.ftCreationTime.ToDateTime();
		this.LastAccessTime = findData.ftLastAccessTime.ToDateTime();
		this.LastWriteTime = findData.ftLastWriteTime.ToDateTime();
		this.FileSize = ( ( UInt64 )findData.nFileSizeHigh << 32 ) + findData.nFileSizeLow;
		this.Exists = true;
		this.FileHashCode = default( Int64? );
	}

	[DebuggerStepThrough]
	public FileAttributeData( Boolean exists, FileAttributes attributes, DateTime creationTime, DateTime lastAccessTime, DateTime lastWriteTime, UInt64 fileSize ) {
		this.Exists = exists;
		this.FileAttributes = attributes;
		this.CreationTime = creationTime;
		this.LastAccessTime = lastAccessTime;
		this.LastWriteTime = lastWriteTime;
		this.FileSize = fileSize;
		this.FileHashCode = default( Int64? );
	}

	public Boolean Refresh( String fullPath, Boolean throwOnError = true ) {
		this.Reset();

		if ( String.IsNullOrWhiteSpace( fullPath ) ) {
			throw new NullException(  nameof( fullPath ) );
		}

		var handle = NativeMethods.FindFirstFile( fullPath, out var data );

		if ( handle?.IsInvalid == true ) {
			if ( throwOnError ) {
				NativeMethods.HandleLastError( fullPath );
			}
			else {
				this.Reset();

				return false;
			}
		}

		var fileAttributeData = new FileAttributeData( data );
		this.Exists = fileAttributeData.Exists;
		this.FileAttributes = fileAttributeData.FileAttributes;
		this.CreationTime = fileAttributeData.CreationTime;
		this.LastAccessTime = fileAttributeData.LastAccessTime;
		this.LastWriteTime = fileAttributeData.LastWriteTime;
		this.FileSize = fileAttributeData.FileSize;
		this.FileHashCode = default( Int64? );

		return true;
	}

	/// <summary>Reset known information about file to defaults.</summary>
	[DebuggerStepThrough]
	public void Reset() {
		this.Exists = default( Boolean? );
		this.FileAttributes = default( FileAttributes? );
		this.CreationTime = default( DateTime? );
		this.LastAccessTime = default( DateTime? );
		this.LastWriteTime = default( DateTime? );
		this.FileSize = default( UInt64? );
		this.FileHashCode = default( Int64? );
	}

	/*
	public Task<Boolean> GetHash() {

		//crc32/64?
		return Task.Run( () => {
			return ( FileAttributes, this.CreationTime ).GetHashCode();
		} );
	}
	*/

}