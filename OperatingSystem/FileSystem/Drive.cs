// Copyright 2015 Rick@AIBrain.org.
// 
// This notice must be kept visible in the source.
// 
// This section of source code belongs to Rick@AIBrain.Org unless otherwise specified, or the original license has been overwritten by the automatic formatting of this code.
// Any unmodified sections of source code borrowed from other projects retain their original license and thanks goes to the Authors.
// 
// Donations and royalties can be paid via
// PayPal: paypal@aibrain.org
// bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
// litecoin: LeUxdU2w3o6pLZGVys5xpDZvvo8DUrjBp9
// 
// Usage of the source code or compiled binaries is AS-IS.
// I am not responsible for Anything You Do.
// 
// Contact me by email if you have any questions or helpful criticism.
//  
// "Librainian/Drive.cs" was last cleaned by Rick on 2015/11/13 at 11:30 PM

namespace Librainian.OperatingSystem.FileSystem {

    using System;
    using System.IO;
    using Extensions;
    using FluentAssertions;

    [Immutable]
    public class Drive {

        private readonly Char _driveLetter;

        internal readonly DriveInfo Info;

        public Drive( Document document ) : this( document.FullPathWithFileName[ 0 ] ) { }

        public Drive( Folder folder ) : this( folder.FullName[ 0 ] ) { }

        public Drive( String fullpath ) : this( fullpath[ 0 ] ) { }

        public Drive( Char driveLetter ) {
            this._driveLetter = driveLetter.ToString()
                                           .ToUpper()[ 0 ];

            this._driveLetter.Should()
                .BeGreaterOrEqualTo( 'A' );
            this._driveLetter.Should()
                .BeLessOrEqualTo( 'Z' );

            if ( ( this._driveLetter < 'A' ) || ( this._driveLetter > 'Z' ) ) {
                throw new ArgumentOutOfRangeException( nameof( driveLetter ), driveLetter, $"The specified drive \"{driveLetter}\" is outside of the range A through Z." );
            }

            this.Info = new DriveInfo( this._driveLetter.ToString() );
        }

        private Drive() {
            throw new NotImplementedException();
        }

        /// <summary>
        /// </summary>
        /// <returns></returns>
        //TODO needs testing
        public Boolean Exists() => this.Info.IsReady && ( this.Info.AvailableFreeSpace > 0 ) && !String.IsNullOrWhiteSpace( this.Info.ToString() );

        public override String ToString() => this._driveLetter.ToString();

    }

}
