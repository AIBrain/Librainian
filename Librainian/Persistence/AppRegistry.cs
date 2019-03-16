// Copyright © Rick@AIBrain.org and Protiguous. All Rights Reserved.
// 
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
// 
// This source code contained in "Registry.cs" belongs to Protiguous@Protiguous.com and
// Rick@AIBrain.org unless otherwise specified or the original license has
// been overwritten by formatting.
// (We try to avoid it from happening, but it does accidentally happen.)
// 
// Any unmodified portions of source code gleaned from other projects still retain their original
// license and our thanks goes to those Authors. If you find your code in this source code, please
// let us know so we can properly attribute you and include the proper license and/or copyright.
// 
// If you want to use any of our code, you must contact Protiguous@Protiguous.com or
// Sales@AIBrain.org for permission and a quote.
// 
// Donations are accepted (for now) via
//     bitcoin:1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//     paypal@AIBrain.Org
//     (We're still looking into other solutions! Any ideas?)
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
// For business inquiries, please contact me at Protiguous@Protiguous.com
// 
// Our website can be found at "https://Protiguous.com/"
// Our software can be found at "https://Protiguous.Software/"
// Our GitHub address is "https://github.com/Protiguous".
// Feel free to browse any source code we *might* make available.
// 
// Project: "Librainian", "Registry.cs" was last formatted by Protiguous on 2018/10/14 at 9:31 AM.

namespace Librainian.Persistence {

	using System;
	using System.Windows.Forms;
	using Converters;
	using Exceptions;
	using JetBrains.Annotations;
	using Logging;
	using Microsoft.Win32;
	using Parsing;

	public static class AppRegistry {

		/// <summary>
		///     Registry key for the application.
		/// </summary>
		[NotNull]
		public static RegistryKey OurApplication { get; }

		/// <summary>
		///     Registry key for the product's company.
		/// </summary>
		[NotNull]
		public static RegistryKey OurCompany { get; }

		/// <summary>
		///     Registry key for the current user;
		/// </summary>
		[NotNull]
		public static RegistryKey OurUser { get; } = Registry.CurrentUser;

		/// <summary>
		///     Registry key for the user's software.
		/// </summary>
		[NotNull]
		public static RegistryKey Software { get; }

		static AppRegistry() {
			if ( OurUser is null ) {
				throw new ArgumentEmptyException( $"Registry folder {nameof( Registry.CurrentUser )} is null!" );
			}

			Software = OurUser.CreateSubKey( nameof( Software ), true );

			if ( Software is null ) {
				throw new ArgumentEmptyException( $"Application {nameof( AppRegistry )} folder {nameof( Software )} is null!" );
			}

			OurCompany = Software.CreateSubKey( Application.CompanyName.Replace( "&", String.Empty ), true );

			if ( OurCompany is null ) {
				throw new ArgumentEmptyException( $"Application {nameof( AppRegistry )} folder {nameof( Application.CompanyName )} is null!" );
			}

			OurApplication = OurCompany.CreateSubKey( Application.ProductName, true );

			if ( OurApplication is null ) {
				throw new ArgumentEmptyException( $"Application {nameof( AppRegistry )} folder {nameof( Application.ProductName )} is null!" );
			}
		}

		/// <summary>
		///     <para>Gets the value of the current user's software's company's application's folder's key.</para>
		/// </summary>
		/// <param name="folder"></param>
		/// <param name="key">   </param>
		/// <returns></returns>
		[CanBeNull]
		public static Object Get( TrimmedString folder, TrimmedString key ) {
			if ( folder.IsEmpty() ) {
				throw new ArgumentEmptyException( nameof( folder ) );
			}

			if ( key.IsEmpty() ) {
				throw new ArgumentEmptyException( nameof( key ) );
			}

			try {
				using ( var subKey = OurApplication.OpenSubKey( folder, RegistryKeyPermissionCheck.ReadSubTree ) ) {
					return subKey?.GetValue( key );
				}
			}
			catch ( Exception exception ) {
				exception.Log();
			}

			return null;
		}

		/// <summary>
		///     <para>Sets the <paramref name="value" /> of the current user's software's company's application's folder's key.</para>
		/// </summary>
		/// <param name="folder"></param>
		/// <param name="key">   </param>
		/// <param name="value"> </param>
		/// <param name="kind">  </param>
		/// <returns></returns>
		public static Boolean Set( TrimmedString folder, TrimmedString key, [CanBeNull] Object value, RegistryValueKind kind ) {
			if ( folder.IsEmpty() ) {
				throw new ArgumentException( "Value cannot be null or whitespace.", paramName: nameof( folder ) );
			}

			if ( key.IsEmpty() ) {
				throw new ArgumentException( "Value cannot be null or whitespace.", paramName: nameof( key ) );
			}

			using ( var regFolder = OurApplication.CreateSubKey( folder, RegistryKeyPermissionCheck.ReadWriteSubTree ) ) {
				if ( regFolder == null ) {
					$"Error creating subkey {folder}".Break();

					return false;
				}

				try {
					if ( value == null ) {
						regFolder.DeleteValue( key );
					}
					else {
						regFolder.SetValue( key, value, kind );
					}

					return true;
				}
				catch ( Exception exception) {
					exception.Break();
				}

				return false;
			}
		}

		/// <summary>
		///     <para>Gets the value of the current user's software's company's application's folder's key.</para>
		/// </summary>
		/// <param name="folder"></param>
		/// <param name="key">   </param>
		[Pure]
		[CanBeNull]
		public static Byte? GetByte( TrimmedString folder, TrimmedString key ) {
			if ( folder.IsEmpty() ) {
				throw new ArgumentEmptyException( nameof( folder ) );
			}

			if ( key.IsEmpty() ) {
				throw new ArgumentEmptyException( nameof( key ) );
			}

			try {
				using ( var subKey = OurApplication.OpenSubKey( folder, RegistryKeyPermissionCheck.ReadSubTree ) ) {
					var p = subKey?.GetValue( key );

					if ( p == null ) {
						return default;
					}

					return p.ToByteOrNull();
				}
			}
			catch ( Exception exception ) {
				exception.Log();
			}

			return default;
		}

		/// <summary>
		///     <para>Gets the value of the current user's software's company's application's folder's key.</para>
		/// </summary>
		/// <param name="folder"></param>
		/// <param name="key">   </param>
		[Pure]
		[CanBeNull]
		public static Int32? GetInt32( TrimmedString folder, TrimmedString key ) {
			if ( folder.IsEmpty() ) {
				throw new ArgumentEmptyException( nameof( folder ) );
			}

			if ( key.IsEmpty() ) {
				throw new ArgumentEmptyException( nameof( key ) );
			}

			try {
				using ( var subKey = OurApplication.OpenSubKey( folder, RegistryKeyPermissionCheck.ReadSubTree ) ) {
					var p = subKey?.GetValue( key );

					if ( p == null ) {
						return default;
					}

					return p.ToIntOrNull();
				}
			}
			catch ( Exception exception ) {
				exception.Log();
			}

			return default;
		}

		/// <summary>
		///     <para>Gets the value of the current user's software's company's application's folder's key.</para>
		/// </summary>
		/// <param name="folder"></param>
		/// <param name="key">   </param>
		[Pure]
		[CanBeNull]
		public static String GetString( TrimmedString folder, TrimmedString key ) {
			if ( folder.IsEmpty() ) {
				throw new ArgumentEmptyException( nameof( folder ) );
			}

			if ( key.IsEmpty() ) {
				throw new ArgumentEmptyException( nameof( key ) );
			}

			try {
				using ( var subKey = OurApplication.OpenSubKey( folder, RegistryKeyPermissionCheck.ReadSubTree ) ) {
					var p = subKey?.GetValue( key );

					if ( p == null ) {
						return default;
					}

					return p.ToStringOrNull();
				}
			}
			catch ( Exception exception ) {
				exception.Log();
			}

			return default;
		}

		/// <summary>
		///     <para>Gets the value of the current user's software's company's application's folder's key.</para>
		/// </summary>
		/// <param name="folder"></param>
		/// <param name="key">   </param>
		[Pure]
		[CanBeNull]
		public static Boolean? GetBoolean( TrimmedString folder, TrimmedString key ) {
			if ( folder.IsEmpty() ) {
				throw new ArgumentEmptyException( nameof( folder ) );
			}

			if ( key.IsEmpty() ) {
				throw new ArgumentEmptyException( nameof( key ) );
			}

			try {
				using ( var subKey = OurApplication.OpenSubKey( folder, RegistryKeyPermissionCheck.ReadSubTree ) ) {
					var p = subKey?.GetValue( key );

					if ( p == null ) {
						return default;
					}

					return p.ToBooleanOrNull();
				}
			}
			catch ( Exception exception ) {
				exception.Log();
			}

			return default;
		}

		/// <summary>
		///     <para>Gets the value of the current user's software's company's application's folder's key.</para>
		/// </summary>
		/// <param name="folder"></param>
		/// <param name="key">   </param>
		[Pure]
		[CanBeNull]
		public static Int64? GetInt64( TrimmedString folder, TrimmedString key ) {
			if ( folder.IsEmpty() ) {
				throw new ArgumentEmptyException( nameof( folder ) );
			}

			if ( key.IsEmpty() ) {
				throw new ArgumentEmptyException( nameof( key ) );
			}

			try {
				using ( var subKey = OurApplication.OpenSubKey( folder, RegistryKeyPermissionCheck.ReadSubTree ) ) {
					var p = subKey?.GetValue( key );

					if ( p == null ) {
						return default;
					}

					if ( Int64.TryParse( p.ToString(), out var result ) ) {
						return result;
					}

				}
			}
			catch ( Exception exception ) {
				exception.Log();
			}

			return default;
		}

		/// <summary>
		///     <para>Sets the <paramref name="value" /> of the current user's software's company's application's folder's key.</para>
		/// </summary>
		/// <param name="folder"></param>
		/// <param name="key">   </param>
		/// <param name="value"> </param>
		public static void Set( TrimmedString folder, TrimmedString key, String value ) => Set( folder, key, value, RegistryValueKind.String );

		/// <summary>
		///     <para>Sets the <paramref name="value" /> of the current user's software's company's application's folder's key.</para>
		/// </summary>
		/// <param name="folder"></param>
		/// <param name="key">   </param>
		/// <param name="value"> </param>
		public static void Set( TrimmedString folder, TrimmedString key, Int32 value ) => Set( folder, key, value, RegistryValueKind.DWord );

		/// <summary>
		///     <para>Sets the <paramref name="value" /> of the current user's software's company's application's folder's key.</para>
		/// </summary>
		/// <param name="folder"></param>
		/// <param name="key">   </param>
		/// <param name="value"> </param>
		public static void Set( TrimmedString folder, TrimmedString key, Int64 value ) => Set( folder, key, value, RegistryValueKind.QWord );

	}

}