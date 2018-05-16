// Copyright © 1995-2018 to Rick@AIBrain.org and Protiguous.
// All Rights Reserved.
//
// This ENTIRE copyright notice and file header MUST BE KEPT
// VISIBLE in any source code derived from or used from our
// libraries and projects.
//
// =========================================================
// This section of source code, "Location.cs",
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
// "Librainian/Librainian/Location.cs" was last cleaned by Protiguous on 2018/05/15 at 10:41 PM.

namespace Librainian.FileSystem {

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

        private readonly Int32 _hashCode;

        private Location() {
            this.Address = String.Empty;
            this._hashCode = 0;
        }

        public Location( [NotNull] String location ) {
            if ( String.IsNullOrWhiteSpace( location ) ) { throw new ArgumentException( "Value cannot be null or whitespace.", nameof( location ) ); }

            location = location.Trim();

            if ( String.IsNullOrWhiteSpace( location ) ) { throw new ArgumentException( "Value cannot be null or whitespace.", nameof( location ) ); }

            this.Address = new Uri( uriString: location ).AbsoluteUri;
            this._hashCode = this.Address.GetHashCode();
        }

        /// <summary>
        ///     A local file, LAN, UNC, or a URI.
        /// </summary>
        [NotNull]
        public String Address { get; }

        public static Boolean Equals( [CanBeNull] Location left, [CanBeNull] Location right ) {
            if ( left is null && right is null ) { return true; }

            if ( left != null && right is null ) { return false; }

            if ( left is null ) { return false; }

            return left._hashCode == right._hashCode;
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

        public override Int32 GetHashCode() => this._hashCode;
    }
}