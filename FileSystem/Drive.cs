// Copyright © 1995-2018 to Rick@AIBrain.org and Protiguous.
// All Rights Reserved.
//
// This ENTIRE copyright notice and file header MUST BE KEPT
// VISIBLE in any source code derived from or used from our
// libraries and projects.
//
// =========================================================
// This section of source code, "Drive.cs",
// belongs to Rick@AIBrain.org and Protiguous@Protiguous.com
// unless otherwise specified OR the original license has been
// overwritten by the automatic formatting.
//
// (We try to avoid that from happening, but it does happen.)
//
// Any unmodified portions of source code gleaned from other
// projects still retain their original license and our thanks
// goes to those Authors.
// =========================================================
//
// Donations (more please!), royalties from any software that
// uses any of our code, and license fees can be paid to us via
// bitcoin at the address 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2.
//
// =========================================================
// Usage of the source code or compiled binaries is AS-IS.
// No warranties are expressed or implied.
// I am NOT responsible for Anything You Do With Our Code.
// =========================================================
//
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
//
// "Librainian/Librainian/Drive.cs" was last cleaned by Protiguous on 2018/05/15 at 10:41 PM.

namespace Librainian.FileSystem {

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

        public Char DriveLetter { get; }

        [NotNull]
        public DriveInfo Info { get; }

        public String RootDirectory => this.Info.RootDirectory.Name;

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