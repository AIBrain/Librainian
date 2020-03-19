// Copyright © 2020 Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible in any binaries, libraries, repositories, and source code (directly or derived)
// from our binaries, libraries, projects, or solutions.
//
// This source code contained in "Disk.cs" belongs to Protiguous@Protiguous.com unless otherwise specified or the original license has been overwritten
// by formatting. (We try to avoid it from happening, but it does accidentally happen.)
//
// Any unmodified portions of source code gleaned from other projects still retain their original license and our thanks goes to those Authors.
// If you find your code in this source code, please let us know so we can properly attribute you and include the proper license and/or copyright.
//
// If you want to use any of our code in a commercial project, you must contact Protiguous@Protiguous.com for permission and a quote.
//
// Donations are accepted via bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2 and PayPal: Protiguous@Protiguous.com
//
// =========================================================
// Disclaimer:  Usage of the source code or binaries is AS-IS.
//    No warranties are expressed, implied, or given.
//    We are NOT responsible for Anything You Do With Our Code.
//    We are NOT responsible for Anything You Do With Our Executables.
//    We are NOT responsible for Anything You Do With Your Computer.
// =========================================================
//
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
// For business inquiries, please contact me at Protiguous@Protiguous.com.
//
// Our website can be found at "https://Protiguous.com/"
// Our software can be found at "https://Protiguous.Software/"
// Our GitHub address is "https://github.com/Protiguous".
// Feel free to browse any source code we make available.
//
// Project: "LibrainianCore", File: "Disk.cs" was last formatted by Protiguous on 2020/03/16 at 3:03 PM.

namespace Librainian.ComputerSystem.Devices {

    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using Extensions;
    using JetBrains.Annotations;
    using OperatingSystem.FileSystem;

    // ReSharper disable RedundantUsingDirective
    // ReSharper restore RedundantUsingDirective

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

        public Disk( [NotNull] Document document ) : this( driveLetter: document.FullPath[ index: 0 ] ) { }

        public Disk( [NotNull] Folder folder ) : this( driveLetter: folder.FullPath[ index: 0 ] ) { }

        public Disk( [NotNull] String fullpath ) : this( driveLetter: fullpath[ index: 0 ] ) { }

        public Disk( [NotNull] DriveInfo info ) : this( folder: new Folder( fullPath: info.RootDirectory.FullName ) ) =>
            this.Info = info ?? throw new ArgumentNullException( paramName: nameof( info ) );

        public Disk( Char driveLetter ) {
            this.DriveLetter = Char.ToUpper( c: driveLetter, culture: CultureInfo.CurrentCulture );

            if ( this.DriveLetter < 'A' || this.DriveLetter > 'Z' ) {
                throw new ArgumentOutOfRangeException( paramName: nameof( driveLetter ), actualValue: driveLetter,
                    message: $"The specified drive \"{driveLetter}\" is outside of the range A through Z." );
            }

            this.Info = new DriveInfo( driveName: this.DriveLetter.ToString() );
        }

        public Disk() : this( fullpath: Environment.CurrentDirectory ) { }

        [NotNull]
        public static IEnumerable<Disk> GetDrives() => DriveInfo.GetDrives().Select( selector: drive => new Disk( info: drive ) );

        /// <summary></summary>
        /// <returns></returns>
        public Boolean Exists() => this.Info.IsReady && !String.IsNullOrWhiteSpace( value: this.Info.Name );

        public UInt64 FreeSpace() => this.Info.IsReady ? ( UInt64 )this.Info.AvailableFreeSpace : 0;

        [NotNull]
        public IEnumerable<IFolder> GetFolders( [CanBeNull] String searchPattern = "*" ) {
            var root = new Folder( fullPath: this.Info.RootDirectory.FullName );

            return root.GetFolders( searchPattern: searchPattern );
        }

        [NotNull]
        public override String ToString() => this.DriveLetter.ToString();
    }
}