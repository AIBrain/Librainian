// Copyright © 1995-2018 to Rick@AIBrain.org and Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
//
// This source code contained in "Drive.cs" belongs to Rick@AIBrain.org and
// Protiguous@Protiguous.com unless otherwise specified or the original license has
// been overwritten by automatic formatting.
// (We try to avoid it from happening, but it does accidentally happen.)
//
// Any unmodified portions of source code gleaned from other projects still retain their original
// license and our thanks goes to those Authors. If you find your code in this source code, please
// let us know so we can properly attribute you and include the proper license and/or copyright.
//
// Donations, royalties from any software that uses any of our code, or license fees can be paid
// to us via bitcoin at the address 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2.
//
// =========================================================
// Usage of the source code or binaries is AS-IS.
// No warranties are expressed, implied, or given.
// We are NOT responsible for Anything You Do With Our Code.
// =========================================================
//
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
// For business inquiries, please contact me at Protiguous@Protiguous.com
//
// "Librainian/Librainian/Drive.cs" was last formatted by Protiguous on 2018/05/24 at 7:02 PM.

namespace Librainian.ComputerSystems.FileSystem {

    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using Extensions;
    using FluentAssertions;
    using JetBrains.Annotations;

    /// <summary>
    ///     A <see cref="Drive" /> contains 1 or more disks.
    /// </summary>
    /// <remarks>http://superuser.com/questions/341497/whats-the-difference-between-a-disk-and-a-drive</remarks>
    [Immutable]
    public class Drive {

        public Char DriveLetter { get; }

        [NotNull]
        public DriveInfo Info { get; }

        public String RootDirectory => this.Info.RootDirectory.Name;

        public Drive( Document document ) : this( document.FullPathWithFileName[0] ) { }

        public Drive( Folder folder ) : this( folder.FullName[0] ) { }

        public Drive( String fullpath ) : this( fullpath[0] ) { }

        public Drive( [NotNull] DriveInfo info ) : this( new Folder( info.RootDirectory ) ) => this.Info = info ?? throw new ArgumentNullException( nameof( info ) );

        public Drive( Char driveLetter ) {
            this.DriveLetter = Char.ToUpper( driveLetter );

            this.DriveLetter.Should().BeGreaterOrEqualTo( 'A' );
            this.DriveLetter.Should().BeLessOrEqualTo( 'Z' );

            if ( this.DriveLetter < 'A' || this.DriveLetter > 'Z' ) {
                throw new ArgumentOutOfRangeException( nameof( driveLetter ), driveLetter, $"The specified drive \"{driveLetter}\" is outside of the range A through Z." );
            }

            this.Info = new DriveInfo( this.DriveLetter.ToString() );
        }

        public Drive() : this( Environment.CurrentDirectory ) { }

        public static IEnumerable<Drive> GetDrives() => DriveInfo.GetDrives().Select( drive => new Drive( drive ) );

        /// <summary>
        /// </summary>
        /// <returns></returns>
        public Boolean Exists() => this.Info.IsReady && !String.IsNullOrWhiteSpace( this.Info.Name );

        public UInt64 FreeSpace() => this.Info.IsReady ? ( UInt64 )this.Info.AvailableFreeSpace : 0;

        public IEnumerable<Folder> GetFolders( String searchPattern = "*" ) {
            var root = new Folder( this.Info.RootDirectory );

            return root.BetterGetFolders( searchPattern );
        }

        public override String ToString() => this.DriveLetter.ToString();
    }
}