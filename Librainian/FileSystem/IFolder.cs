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
// File "IFolder.cs" last touched on 2021-10-13 at 4:26 PM by Protiguous.

namespace Librainian.FileSystem;

using System;
using System.Collections.Generic;
using System.IO;
using System.Security;
using System.Threading;
using ComputerSystem.Devices;
using PooledAwait;

public interface IFolder : IEquatable<IFolder> {

	String FullPath { get; }

	/// <summary>The <see cref="IFolder" /> class is built around <see cref="DirectoryInfo" />.</summary>
	DirectoryInfo Info { get; }

	String Name { get; }

	/// <summary>
	///     <para>Returns True if the folder exists.</para>
	/// </summary>
	/// See also:
	/// <see cref="Delete"></see>
	PooledValueTask<Boolean> Create( CancellationToken cancellationToken );

	/// <summary>
	///     <para>Returns True if the folder no longer exists.</para>
	/// </summary>
	/// <see cref="Create"></see>
	PooledValueTask<Boolean> Delete( CancellationToken cancellationToken );

	IAsyncEnumerable<Document> EnumerateDocuments( IEnumerable<String> searchPatterns, CancellationToken cancellationToken );

	IAsyncEnumerable<Document> EnumerateDocuments( String? searchPattern, CancellationToken cancellationToken );

	IAsyncEnumerable<Folder> EnumerateFolders( String? searchPattern, SearchOption searchOption, CancellationToken cancellationToken );

	/// <summary>Returns true if the <see cref="IFolder" /> currently exists.</summary>
	/// <exception cref="System.IO.IOException"></exception>
	/// <exception cref="SecurityException"></exception>
	/// <exception cref="System.IO.PathTooLongException"></exception>
	PooledValueTask<Boolean> Exists( CancellationToken cancellationToken );

	/// <summary>Returns true if the <see cref="IFolder" /> currently exists.</summary>
	/// <exception cref="System.IO.IOException"></exception>
	/// <exception cref="SecurityException"></exception>
	/// <exception cref="System.IO.PathTooLongException"></exception>
	Boolean ExistsSync();

	/// <summary>Free space available to the current user.</summary>
	PooledValueTask<UInt64> GetAvailableFreeSpace();

	Disk GetDrive();

	/// <summary>
	///     Sync version
	/// </summary>
	Boolean GetExists();

	Int32 GetHashCode();

	IFolder? GetParent();

	/// <summary>
	///     <para>Check if this <see cref="IFolder" /> contains any <see cref="IFolder" /> or <see cref="Document" /> .</para>
	/// </summary>
	/// <param name="cancellationToken"></param>
	PooledValueTask<Boolean> IsFolderEmpty( CancellationToken cancellationToken );

	void OpenWithExplorer();

	PooledValueTask<DirectoryInfo> Refresh( CancellationToken cancellationToken );

	/// <summary>
	///     <para>Shorten the full path with "..."</para>
	/// </summary>
	String ToCompactFormat();

	/// <summary>Returns a String that represents the current object.</summary>
	/// <returns>A String that represents the current object.</returns>
	String ToString();

}