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
// File "Disk.cs" last touched on 2021-03-07 at 3:46 PM by Protiguous.

namespace Librainian.ComputerSystem.Devices {

	using System;
	using System.Collections.Generic;
	using System.Globalization;
	using System.IO;
	using System.Linq;
	using System.Runtime.CompilerServices;
	using System.Threading;
	using System.Threading.Tasks;
	using Extensions;
	using FileSystem;
	using JetBrains.Annotations;

	/// <summary>
	///     <para>A Drive contains 1 or more <see cref="Disk" />.</para>
	/// </summary>
	/// <remarks>http://superuser.com/questions/341497/whats-the-difference-between-a-disk-and-a-drive</remarks>
	[Immutable]
	public class Disk {

		public Char DriveLetter { get; }

		[NotNull]
		public DriveInfo Info { get; }

		[NotNull]
		public String RootDirectory => this.Info.RootDirectory.Name;

		public Disk( [NotNull] Document document ) : this( document.FullPath[ 0 ] ) { }

		public Disk( [NotNull] Folder folder ) : this( folder.FullPath[ 0 ] ) { }

		public Disk( [NotNull] String fullpath ) : this( fullpath[ 0 ] ) { }

		public Disk( [NotNull] DriveInfo info ) : this( info.RootDirectory.FullName[ 0 ] ) { }

		public Disk( Char driveLetter ) {
			this.DriveLetter = Char.ToUpper( driveLetter, CultureInfo.CurrentCulture );

			if ( this.DriveLetter is < 'A' or > 'Z' ) {
				throw new ArgumentOutOfRangeException( nameof( driveLetter ), driveLetter, $"The specified drive \"{driveLetter}\" is outside of the range A through Z." );
			}

			this.Info = new DriveInfo( this.DriveLetter.ToString() );
		}

		/// <summary>
		///     Assume <see cref="Environment.CurrentDirectory" />.
		/// </summary>
		public Disk() : this( Environment.CurrentDirectory[ 0 ] ) { }

		[NotNull]
		public static async IAsyncEnumerable<Disk> GetDrives( [EnumeratorCancellation] CancellationToken cancellationToken ) {
			await foreach ( var driveInfo in DriveInfo.GetDrives().ToAsyncEnumerable().WithCancellation( cancellationToken ) ) {
				if ( driveInfo != null ) {
					yield return new Disk( driveInfo );
				}
			}
		}

		[NotNull]
		public IAsyncEnumerable<IFolder> EnumerateFolders( CancellationToken cancellationToken, [CanBeNull] String? searchPattern = "*" ) {
			var root = new Folder( this.Info.RootDirectory.FullName );

			return root.EnumerateFolders( searchPattern, SearchOption.TopDirectoryOnly, cancellationToken );
		}

		/// <summary></summary>
		/// <returns></returns>
		public Boolean Exists() => this.Info.IsReady && !String.IsNullOrWhiteSpace( this.Info.Name );

		public UInt64 FreeSpace() => this.Info.IsReady ? ( UInt64 )this.Info.AvailableFreeSpace : 0;

		[NotNull]
		public override String ToString() => this.DriveLetter.ToString();
	}
}