// Copyright 2018 Protiguous.
// 
// This notice must be kept visible in the source.
// 
// This section of source code belongs to Protiguous@Protiguous.com unless otherwise specified, or the
// original license has been overwritten by the automatic formatting of this code. Any unmodified
// sections of source code borrowed from other projects retain their original license and thanks
// goes to the Authors.
// 
// Donations and royalties can be paid via
//  
//  bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//  
// 
// Usage of the source code or compiled binaries is AS-IS. I am not responsible for Anything You Do.
// 
// Contact me by email if you have any questions or helpful criticism.
// 
// "Librainian/LocationClass.cs" was last cleaned by Protiguous on 2018/01/28 at 9:50 AM

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
    public class Location : IEquatable< Location >, IComparable< Location >, IComparable {
        private readonly Int32 _hashCode;

        public Location( [ NotNull ] String location ) {
            if ( String.IsNullOrWhiteSpace( value: location ) ) {
                throw new ArgumentException( "Value cannot be null or whitespace.",nameof(location) );
            }

            location = location.Trim();

            if ( String.IsNullOrWhiteSpace( value: location ) ) {
                throw new ArgumentException( "Value cannot be null or whitespace.",nameof(location) );
            }

            this.Address = new Uri( uriString: location ).AbsoluteUri;
            this._hashCode = this.Address.GetHashCode();
        }

        private Location() {
            this.Address = String.Empty;
            this._hashCode = 0;
        }

        /// <summary>
        ///     A local file, LAN, UNC, or a URI.
        /// </summary>
        [ NotNull ]
        public String Address { get; }

        public Int32 CompareTo( Object obj ) {
            if ( obj is null ) {
                return 1;
            }

            if ( ReferenceEquals( this, obj ) ) {
                return 0;
            }

            if ( !( obj is Location ) ) {
                throw new ArgumentException( message: $"Object must be of type {nameof(Location)}" );
            }

            return this.CompareTo( other: ( Location )obj );
        }

        public Int32 CompareTo( Location other ) {
            if ( ReferenceEquals( this, other ) ) {
                return 0;
            }

            if ( other is null ) {
                return 1;
            }

            return String.Compare( strA: this.Address, strB: other.Address, comparisonType: StringComparison.Ordinal );
        }

        public Boolean Equals( Location other ) => other != null && this.Address == other.Address;

        public static Boolean Equals( [ CanBeNull ] Location left, [ CanBeNull ] Location right ) {
            if ( left is null && right is null ) {
                return true;
            }

            if ( left != null && right is null ) {
                return false;
            }

            if ( left is null ) {
                return false;
            }

            return left._hashCode == right._hashCode;
        }

        public override Boolean Equals( Object obj ) => this.Equals( other: obj as Location );

        public override Int32 GetHashCode() => this._hashCode;

        public static Boolean operator ==( Location left, Location right ) => Equals( left: left, right: right );
        public static Boolean operator !=( Location left, Location right ) => !Equals( left: left, right: right );

        public static Boolean operator <( Location left, Location right ) => Comparer< Location >.Default.Compare( x: left, y: right ) < 0;

        public static Boolean operator >( Location left, Location right ) => Comparer< Location >.Default.Compare( x: left, y: right ) > 0;

        public static Boolean operator <=( Location left, Location right ) => Comparer< Location >.Default.Compare( x: left, y: right ) <= 0;

        public static Boolean operator >=( Location left, Location right ) => Comparer< Location >.Default.Compare( x: left, y: right ) >= 0;
    }
}