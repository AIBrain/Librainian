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
// File "IFolder.cs" last touched on 2021-03-10 at 4:40 AM by Protiguous.

namespace Librainian.FileSystem {

	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Security;
	using System.Threading;
	using System.Threading.Tasks;
	using ComputerSystem.Devices;
	using JetBrains.Annotations;
	using DirectoryInfo = Pri.LongPath.DirectoryInfo;

	public interface IFolder : IEquatable<IFolder> {

		[NotNull]
		String FullPath { get; }

		/// <summary>The <see cref="IFolder" /> class is built around <see cref="DirectoryInfo" />.</summary>
		[NotNull]
		DirectoryInfo Info { get; }

		[NotNull]
		String Name { get; }

		/// <summary></summary>
		/// <param name="searchPattern"></param>
		/// <param name="randomize">    </param>
		/// <returns></returns>
		[NotNull]
		IEnumerable<IFolder> BetterGetFolders( [CanBeNull] String? searchPattern = "*", Boolean randomize = false );

		/// <summary>Return a list of all <see cref="IFolder" /> matching the <paramref name="searchPattern" />.</summary>
		/// <param name="token"></param>
		/// <param name="searchPattern"></param>
		/// <param name="randomize">Return the folders in random order.</param>
		/// <returns></returns>
		[NotNull]
		Task<List<Folder>> BetterGetFoldersAsync( CancellationToken token, [CanBeNull] String? searchPattern = "*", Boolean randomize = true );

		/// <summary>Returns a copy of the folder instance.</summary>
		/// <returns></returns>
		[NotNull]
		IFolder Clone();

		/// <summary>
		///     <para>Returns True if the folder exists.</para>
		/// </summary>
		/// <returns></returns>
		/// See also:
		/// <see cref="Delete"></see>
		Boolean Create();

		/// <summary>
		///     <para>Returns True if the folder no longer exists.</para>
		/// </summary>
		/// <returns></returns>
		/// <see cref="Create"></see>
		Boolean Delete();

		/// <summary>Returns true if the <see cref="IFolder" /> currently exists.</summary>
		/// <exception cref="System.IO.IOException"></exception>
		/// <exception cref="SecurityException"></exception>
		/// <exception cref="System.IO.PathTooLongException"></exception>
		Boolean Exists();

		/// <summary>Free space available to the current user.</summary>
		/// <returns></returns>
		UInt64 GetAvailableFreeSpace();

		/// <summary>
		///     <para>Returns an enumerable collection of <see cref="Document" /> in the current directory.</para>
		/// </summary>
		/// <returns></returns>
		[NotNull]
		IEnumerable<Document> GetDocuments();

		[NotNull]
		IEnumerable<Document> GetDocuments( [NotNull] String searchPattern );

		[NotNull]
		IEnumerable<Document> GetDocuments( [NotNull] IEnumerable<String> searchPatterns );

		[NotNull]
		Disk GetDrive();

		[NotNull]
		IEnumerable<IFolder> GetFolders( [CanBeNull] String? searchPattern, SearchOption searchOption = SearchOption.AllDirectories );

		Int32 GetHashCode();

		[CanBeNull]
		IFolder GetParent();

		/// <summary>
		///     <para>Check if this <see cref="IFolder" /> contains any <see cref="IFolder" /> or <see cref="Document" /> .</para>
		/// </summary>
		/// <returns></returns>
		Boolean IsEmpty();

		void OpenWithExplorer();

		void Refresh();

		/// <summary>
		///     <para>Shorten the full path with "..."</para>
		/// </summary>
		/// <returns></returns>
		String ToCompactFormat();

		/// <summary>Returns a String that represents the current object.</summary>
		/// <returns>A String that represents the current object.</returns>
		[NotNull]
		String ToString();

	}

}