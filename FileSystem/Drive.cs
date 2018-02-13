// Copyright 2016 Rick@AIBrain.org.
//
// This notice must be kept visible in the source.
//
// This section of source code belongs to Rick@AIBrain.Org unless otherwise specified, or the
// original license has been overwritten by the automatic formatting of this code. Any unmodified
// sections of source code borrowed from other projects retain their original license and thanks
// goes to the Authors.
//
// Donations and royalties can be paid via
//  PayPal: paypal@aibrain.org
//  bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//  litecoin: LeUxdU2w3o6pLZGVys5xpDZvvo8DUrjBp9
//
// Usage of the source code or compiled binaries is AS-IS. I am not responsible for Anything You Do.
//
// Contact me by email if you have any questions or helpful criticism.
//
// "Librainian/Drive.cs" was last cleaned by Rick on 2016/06/18 at 10:51 PM

namespace Librainian.FileSystem {

    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Numerics;
    using Extensions;
    using FluentAssertions;
    using JetBrains.Annotations;

    /// <summary>
    /// 
    /// </summary>
    /// <remarks>http://superuser.com/questions/341497/whats-the-difference-between-a-disk-and-a-drive</remarks>
    [Immutable]
    public class Drive {

        [NotNull]
        public DriveInfo Info {
            get;
        }

        public Drive( Document document ) : this( document.FullPathWithFileName[ 0 ] ) {
        }

        public Drive( Folder folder ) : this( folder.FullName[ 0 ] ) {
        }

        public Drive( String fullpath ) : this( fullpath[ 0 ] ) {
        }

        public Drive( [NotNull] DriveInfo info ) : this( new Folder( info.RootDirectory ) ) {
            if ( info == null ) {
                throw new ArgumentNullException( nameof( info ) );
            }
            this.Info = info;
        }

        public Drive( Char driveLetter ) {
            this.DriveLetter = Char.ToUpper( driveLetter );

            this.DriveLetter.Should().BeGreaterOrEqualTo( 'A' );
            this.DriveLetter.Should().BeLessOrEqualTo( 'Z' );

            if ( ( this.DriveLetter < 'A' ) || ( this.DriveLetter > 'Z' ) ) {
                throw new ArgumentOutOfRangeException( nameof( driveLetter ), driveLetter, $"The specified drive \"{driveLetter}\" is outside of the range A through Z." );
            }

            this.Info = new DriveInfo( this.DriveLetter.ToString() );
        }

        public Drive() : this( Environment.CurrentDirectory ) {
        }

        public Char DriveLetter {
            get;
        }

        public String RootDirectory => this.Info.RootDirectory.Name;

        public static IEnumerable<Drive> GetDrives() {
            return DriveInfo.GetDrives().Select( drive => new Drive( drive ) );
        }

        /// <summary>
        /// </summary>
        /// <returns></returns>
        public Boolean Exists() => this.Info.IsReady && !String.IsNullOrWhiteSpace( this.Info.Name );

        public BigInteger FreeSpace() {
            if ( this.Info.IsReady ) {
                return this.Info.AvailableFreeSpace;
            }
            return BigInteger.Zero;
        }

        public IEnumerable<Folder> GetFolders( String searchPattern = "*" ) {
            var root = new Folder( this.Info.RootDirectory );
            return root.BetterGetFolders( searchPattern );
        }

        public override String ToString() => this.DriveLetter.ToString();
    }

}