// Copyright © 1995-2018 to Rick@AIBrain.org and Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
//
// This source code contained in "Location.cs" belongs to Rick@AIBrain.org and
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
// "Librainian/Librainian/Location.cs" was last formatted by Protiguous on 2018/05/24 at 7:03 PM.

namespace Librainian.ComputerSystems.FileSystem {

    using System;
    using System.Collections.Generic;
    using JetBrains.Annotations;

    /// <summary>
    ///     Points to a local file, a directory, a LAN file, or an internet file.
    /// </summary>
    /// <remarks>
    ///     (Stored internally as a string)
    /// </remarks>
    public class Location : IEquatable<Location>, IComparable<Location>, IComparable {

        private Int32 HashCode => this.Address.GetHashCode();

        /// <summary>
        ///     A local file, LAN, UNC, or a URI.
        /// </summary>
        [NotNull]
        public String Address { get; }

        private Location() => this.Address = String.Empty;

        public Location( [NotNull] String location ) {
            if ( String.IsNullOrWhiteSpace( location ) ) { throw new ArgumentException( "Value cannot be null or whitespace.", nameof( location ) ); }

            location = location.Trim();

            if ( String.IsNullOrWhiteSpace( location ) ) { throw new ArgumentException( "Value cannot be null or whitespace.", nameof( location ) ); }

            this.Address = new Uri( uriString: location ).AbsoluteUri;
        }

        public static Boolean Equals( [CanBeNull] Location left, [CanBeNull] Location right ) {
            if ( left is null && right is null ) { return true; }

            if ( left != null && right is null ) { return false; }

            if ( left is null ) { return false; }

            return left.HashCode == right.HashCode;
        }

        public static Boolean operator !=( Location left, Location right ) => !Equals( left: left, right: right );

        public static Boolean operator <( Location left, Location right ) => Comparer<Location>.Default.Compare( x: left, y: right ) < 0;

        public static Boolean operator <=( Location left, Location right ) => Comparer<Location>.Default.Compare( x: left, y: right ) <= 0;

        public static Boolean operator ==( Location left, Location right ) => Equals( left: left, right: right );

        public static Boolean operator >( Location left, Location right ) => Comparer<Location>.Default.Compare( x: left, y: right ) > 0;

        public static Boolean operator >=( Location left, Location right ) => Comparer<Location>.Default.Compare( x: left, y: right ) >= 0;

        public Int32 CompareTo( Object obj ) {
            if ( obj is null ) { return 1; }

            if ( ReferenceEquals( this, obj ) ) { return 0; }

            if ( !( obj is Location ) ) { throw new ArgumentException( $"Object must be of type {nameof( Location )}" ); }

            return this.CompareTo( other: ( Location )obj );
        }

        public Int32 CompareTo( Location other ) {
            if ( ReferenceEquals( this, other ) ) { return 0; }

            if ( other is null ) { return 1; }

            return String.Compare( strA: this.Address, strB: other.Address, comparisonType: StringComparison.Ordinal );
        }

        public Boolean Equals( Location other ) => other != null && this.Address == other.Address;

        public override Boolean Equals( Object obj ) => this.Equals( other: obj as Location );

        public override Int32 GetHashCode() => this.HashCode;
    }
}