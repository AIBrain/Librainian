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
//
// Disclaimer:  Usage of the source code or binaries is AS-IS.
// No warranties are expressed, implied, or given.
// We are NOT responsible for Anything You Do With Our Code.
// We are NOT responsible for Anything You Do With Our Executables.
// We are NOT responsible for Anything You Do With Your Computer.
//
//
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
// For business inquiries, please contact me at Protiguous@Protiguous.com.
// Our software can be found at "https://Protiguous.com/Software/"
// Our GitHub address is "https://github.com/Protiguous".
//
// File "Location.cs" last formatted on 2022-12-22 at 5:16 PM by Protiguous.

namespace Librainian.FileSystem;

using System;
using System.Collections.Generic;

/// <summary>Points to a local file, a directory, a LAN file, or an internet file.</summary>
/// <remarks>(Stored internally as a string)</remarks>
/// <remarks>Just a concept.. not in use yet.</remarks>
public class Location : IEquatable<Location>, IComparable<Location>, IComparable {

	private Location() => this.Address = String.Empty;

	public Location( String location ) {
		if ( String.IsNullOrWhiteSpace( location ) ) {
			throw new ArgumentException( "Value cannot be null or whitespace.", nameof( location ) );
		}

		location = location.Trim();

		if ( String.IsNullOrWhiteSpace( location ) ) {
			throw new ArgumentException( "Value cannot be null or whitespace.", nameof( location ) );
		}

		this.Address = new Uri( location ).AbsoluteUri;
	}

	private Int32 HashCode => this.Address.GetHashCode();

	/// <summary>A local file, LAN, UNC, or a URI.</summary>
	public String Address { get; }

	public static Boolean Equals( Location? left, Location? right ) {
		if ( left is null || right is null ) {
			return false;
		}

		return left.HashCode.Equals( right.HashCode ) && left.Address.Equals( right.Address, StringComparison.Ordinal );
	}

	public static Boolean operator !=( Location? left, Location? right ) => !Equals( left, right );

	public static Boolean operator <( Location? left, Location? right ) => Comparer<Location>.Default.Compare( left, right ) < 0;

	public static Boolean operator <=( Location? left, Location? right ) => Comparer<Location>.Default.Compare( left, right ) <= 0;

	public static Boolean operator ==( Location? left, Location? right ) => Equals( left, right );

	public static Boolean operator >( Location? left, Location? right ) => Comparer<Location>.Default.Compare( left, right ) > 0;

	public static Boolean operator >=( Location? left, Location? right ) => Comparer<Location>.Default.Compare( left, right ) >= 0;

	public Int32 CompareTo( Object? obj ) {
		if ( obj is null ) {
			return 1;
		}

		if ( ReferenceEquals( this, obj ) ) {
			return 0;
		}

		if ( obj is Location location ) {
			return this.CompareTo( location );
		}

		throw new ArgumentException( $"Object must be of type {nameof( Location )}" );
	}

	public Int32 CompareTo( Location? other ) {
		if ( ReferenceEquals( this, other ) ) {
			return 0;
		}

		if ( other is null ) {
			return 1;
		}

		return String.Compare( this.Address, other.Address, StringComparison.Ordinal );
	}

	public Boolean Equals( Location? other ) => Equals( this, other );

	public override Boolean Equals( Object? obj ) => Equals( this, obj as Location );

	public override Int32 GetHashCode() => this.HashCode;
}