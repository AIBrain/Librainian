// Copyright © Protiguous. All Rights Reserved.
// This entire copyright notice and license must be retained and must be kept visible in any binaries, libraries, repositories, or source code (directly or derived) from our binaries, libraries, projects, solutions, or applications.
// All source code belongs to Protiguous@Protiguous.com unless otherwise specified or the original license has been overwritten by formatting. (We try to avoid it from happening, but it does accidentally happen.)
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
// File "Location.cs" last formatted on 2020-08-14 at 8:40 PM.

namespace Librainian.OperatingSystem.FileSystem {

	using System;
	using System.Collections.Generic;
	using JetBrains.Annotations;

	/// <summary>Points to a local file, a directory, a LAN file, or an internet file.</summary>
	/// <remarks>(Stored internally as a string)</remarks>
	public class Location : IEquatable<Location>, IComparable<Location>, IComparable {

		private Location() => this.Address = String.Empty;

		public Location( [NotNull] String location ) {
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
		[NotNull]
		public String Address { get; }

		public Int32 CompareTo( [CanBeNull] Object? obj ) {
			if ( obj is null ) {
				return 1;
			}

			if ( ReferenceEquals( this, obj ) ) {
				return 0;
			}

			if ( !( obj is Location ) ) {
				throw new ArgumentException( $"Object must be of type {nameof( Location )}" );
			}

			return this.CompareTo( ( Location )obj );
		}

		public Int32 CompareTo( [CanBeNull] Location other ) {
			if ( ReferenceEquals( this, other ) ) {
				return 0;
			}

			if ( other is null ) {
				return 1;
			}

			return String.Compare( this.Address, other.Address, StringComparison.Ordinal );
		}

		public Boolean Equals( Location other ) => other != null && this.Address == other.Address;

		public static Boolean Equals( [CanBeNull] Location left, [CanBeNull] Location right ) {
			if ( left is null && right is null ) {
				return true;
			}

			if ( left != null && right is null ) {
				return default;
			}

			if ( left is null ) {
				return default;
			}

			return left.HashCode == right.HashCode;
		}

		public static Boolean operator !=( [CanBeNull] Location left, [CanBeNull] Location right ) => !Equals( left, right );

		public static Boolean operator <( [CanBeNull] Location left, [CanBeNull] Location right ) => Comparer<Location>.Default.Compare( left, right ) < 0;

		public static Boolean operator <=( [CanBeNull] Location left, [CanBeNull] Location right ) => Comparer<Location>.Default.Compare( left, right ) <= 0;

		public static Boolean operator ==( [CanBeNull] Location left, [CanBeNull] Location right ) => Equals( left, right );

		public static Boolean operator >( [CanBeNull] Location left, [CanBeNull] Location right ) => Comparer<Location>.Default.Compare( left, right ) > 0;

		public static Boolean operator >=( [CanBeNull] Location left, [CanBeNull] Location right ) => Comparer<Location>.Default.Compare( left, right ) >= 0;

		public override Boolean Equals( Object obj ) => this.Equals( obj as Location );

		public override Int32 GetHashCode() => this.HashCode;

	}

}