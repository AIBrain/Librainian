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
// File "AppRegistry.cs" last formatted on 2020-08-14 at 8:44 PM.

#nullable enable
namespace Librainian.Persistence {

	using System;
	using System.Diagnostics;
	using System.Linq;
	using System.Reflection;
	using Converters;
	using Exceptions;
	using JetBrains.Annotations;
	using Logging;
	using Microsoft.Win32;
	using Parsing;

	public static class AppRegistry {

		static AppRegistry() {
			var assembly = Assembly.GetEntryAssembly() ?? Assembly.GetExecutingAssembly();

			Software = TheUser.CreateSubKey( nameof( Software ), true );

			if ( Software is null ) {
				throw new ArgumentEmptyException( $"Application {nameof( AppRegistry ).DoubleQuote()} key {nameof( Software ).DoubleQuote()} is null." );
			}

			var company = assembly.GetCustomAttributes( typeof( AssemblyCompanyAttribute ) ).OfType<AssemblyCompanyAttribute>().FirstOrDefault()?.Company
								  .Replace( "&", ParsingConstants.Singlespace ).Trimmed();

			if ( String.IsNullOrEmpty( company ) ) {
				throw new InvalidOperationException( $"Unable to find {nameof( AssemblyCompanyAttribute ).DoubleQuote()}." );
			}

			TheCompany = Software.CreateSubKey( company, true );

			if ( TheCompany is null ) {
				throw new ArgumentEmptyException( $"Application {nameof( AppRegistry )} key {nameof( TheCompany ).DoubleQuote()} is null." );
			}

			var product = assembly.GetCustomAttributes( typeof( AssemblyProductAttribute ) ).OfType<AssemblyProductAttribute>().FirstOrDefault()?.Product.Trimmed();

			if ( String.IsNullOrEmpty( product ) ) {
				throw new InvalidOperationException( $"Unable to find {nameof( AssemblyProductAttribute ).DoubleQuote()}." );
			}

			TheApplication = TheCompany.CreateSubKey( product, true );

			if ( TheApplication is null ) {
				throw new ArgumentEmptyException( $"Application {nameof( AppRegistry )} key {nameof( TheApplication )} is null." );
			}
		}

		/// <summary>Registry key for the user's software.</summary>
		[NotNull]
		public static RegistryKey Software { get; }

		/// <summary>Registry key for the application.</summary>
		[NotNull]
		public static RegistryKey TheApplication { get; }

		/// <summary>Registry key for the product's company.</summary>
		[NotNull]
		public static RegistryKey TheCompany { get; }

		/// <summary>Registry key for the current user;</summary>
		[NotNull]
		public static RegistryKey TheUser { get; } = Registry.CurrentUser;

		/// <summary>
		///     <para>Gets the value of the current user's software's company's application's folder's key.</para>
		/// </summary>
		/// <param name="folder"></param>
		/// <param name="key">   </param>
		/// <returns></returns>
		[CanBeNull]
		public static Object? Get( [NotNull] String folder, [NotNull] String key ) {
			if ( String.IsNullOrWhiteSpace( folder ) ) {
				throw new ArgumentException( "Value cannot be null or whitespace.", nameof( folder ) );
			}

			if ( String.IsNullOrWhiteSpace( key ) ) {
				throw new ArgumentException( "Value cannot be null or whitespace.", nameof( key ) );
			}

			try {
				using var registryKey = TheApplication.OpenSubKey( folder, RegistryKeyPermissionCheck.ReadSubTree );

				return registryKey?.GetValue( key );
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
		/// <param name="subkey"></param>
		/// <returns></returns>
		[CanBeNull]
		public static Object? Get( [NotNull] String folder, [NotNull] String key, [NotNull] String subkey ) {
			if ( String.IsNullOrWhiteSpace( folder ) ) {
				throw new ArgumentException( "Value cannot be null or whitespace.", nameof( folder ) );
			}

			if ( String.IsNullOrWhiteSpace( key ) ) {
				throw new ArgumentException( "Value cannot be null or whitespace.", nameof( key ) );
			}

			if ( String.IsNullOrWhiteSpace( subkey ) ) {
				throw new ArgumentException( "Value cannot be null or whitespace.", nameof( subkey ) );
			}

			try {
				using var registryKey = TheApplication.OpenSubKey( folder, RegistryKeyPermissionCheck.ReadSubTree );

				using var subKey = registryKey?.OpenSubKey( subkey, RegistryKeyPermissionCheck.ReadSubTree );

				return subKey?.GetValue( key );
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
		public static Boolean? GetBoolean( [NotNull] String folder, [NotNull] String key ) {
			if ( String.IsNullOrWhiteSpace( folder ) ) {
				throw new ArgumentException( "Value cannot be null or whitespace.", nameof( folder ) );
			}

			if ( String.IsNullOrWhiteSpace( key ) ) {
				throw new ArgumentException( "Value cannot be null or whitespace.", nameof( key ) );
			}

			try {
				using var registryKey = TheApplication.OpenSubKey( folder, RegistryKeyPermissionCheck.ReadSubTree );

				return registryKey?.GetValue( key )?.ToBooleanOrNull();
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
		/// <param name="subkey"></param>
		[Pure]
		[CanBeNull]
		public static Boolean? GetBoolean( [NotNull] String folder, [NotNull] String key, [NotNull] String subkey ) {
			if ( String.IsNullOrWhiteSpace( folder ) ) {
				throw new ArgumentException( "Value cannot be null or whitespace.", nameof( folder ) );
			}

			if ( String.IsNullOrWhiteSpace( key ) ) {
				throw new ArgumentException( "Value cannot be null or whitespace.", nameof( key ) );
			}

			if ( String.IsNullOrWhiteSpace( subkey ) ) {
				throw new ArgumentException( "Value cannot be null or whitespace.", nameof( subkey ) );
			}

			try {
				using var registryKey = TheApplication.OpenSubKey( folder, RegistryKeyPermissionCheck.ReadSubTree );

				using var subKey = registryKey?.OpenSubKey( subkey, RegistryKeyPermissionCheck.ReadSubTree );

				return subKey?.GetValue( key )?.ToBooleanOrNull();
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
		public static Byte? GetByte( [NotNull] String folder, [NotNull] String key ) {
			if ( String.IsNullOrWhiteSpace( folder ) ) {
				throw new ArgumentException( "Value cannot be null or whitespace.", nameof( folder ) );
			}

			if ( String.IsNullOrWhiteSpace( key ) ) {
				throw new ArgumentException( "Value cannot be null or whitespace.", nameof( key ) );
			}

			try {
				using var registryKey = TheApplication.OpenSubKey( folder, RegistryKeyPermissionCheck.ReadSubTree );

				return registryKey?.GetValue( key )?.ToByteOrNull();
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
		/// <param name="subkey"></param>
		[Pure]
		[CanBeNull]
		public static Byte? GetByte( [NotNull] String folder, [NotNull] String key, [NotNull] String subkey ) {
			if ( String.IsNullOrWhiteSpace( folder ) ) {
				throw new ArgumentException( "Value cannot be null or whitespace.", nameof( folder ) );
			}

			if ( String.IsNullOrWhiteSpace( key ) ) {
				throw new ArgumentException( "Value cannot be null or whitespace.", nameof( key ) );
			}

			if ( String.IsNullOrWhiteSpace( subkey ) ) {
				throw new ArgumentException( "Value cannot be null or whitespace.", nameof( subkey ) );
			}

			try {
				using var registryKey = TheApplication.OpenSubKey( folder, RegistryKeyPermissionCheck.ReadSubTree );

				using var subKey = registryKey?.OpenSubKey( subkey, RegistryKeyPermissionCheck.ReadSubTree );

				return subKey?.GetValue( key )?.ToByteOrNull();
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
		public static Int32? GetInt32( [NotNull] String folder, [NotNull] String key ) {
			if ( String.IsNullOrWhiteSpace( folder ) ) {
				throw new ArgumentException( "Value cannot be null or whitespace.", nameof( folder ) );
			}

			if ( String.IsNullOrWhiteSpace( key ) ) {
				throw new ArgumentException( "Value cannot be null or whitespace.", nameof( key ) );
			}

			try {
				using var registryKey = TheApplication.OpenSubKey( folder, RegistryKeyPermissionCheck.ReadSubTree );

				return registryKey?.GetValue( key )?.ToIntOrNull();
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
		/// <param name="subkey"></param>
		[Pure]
		[CanBeNull]
		public static Int32? GetInt32( [NotNull] String folder, [NotNull] String key, [NotNull] String subkey ) {
			if ( String.IsNullOrWhiteSpace( folder ) ) {
				throw new ArgumentException( "Value cannot be null or whitespace.", nameof( folder ) );
			}

			if ( String.IsNullOrWhiteSpace( key ) ) {
				throw new ArgumentException( "Value cannot be null or whitespace.", nameof( key ) );
			}

			if ( String.IsNullOrWhiteSpace( subkey ) ) {
				throw new ArgumentException( "Value cannot be null or whitespace.", nameof( subkey ) );
			}

			try {
				using var registryKey = TheApplication.OpenSubKey( folder, RegistryKeyPermissionCheck.ReadSubTree );

				using var subKey = registryKey?.OpenSubKey( subkey, RegistryKeyPermissionCheck.ReadSubTree );

				return subKey?.GetValue( key )?.ToIntOrNull();
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
		public static Int64? GetInt64( [NotNull] String folder, [NotNull] String key ) {
			if ( String.IsNullOrWhiteSpace( folder ) ) {
				throw new ArgumentException( "Value cannot be null or whitespace.", nameof( folder ) );
			}

			if ( String.IsNullOrWhiteSpace( key ) ) {
				throw new ArgumentException( "Value cannot be null or whitespace.", nameof( key ) );
			}

			try {
				using var registryKey = TheApplication.OpenSubKey( folder, RegistryKeyPermissionCheck.ReadSubTree );

				if ( Int64.TryParse( registryKey?.GetValue( key )?.ToString(), out var result ) ) {
					return result;
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
		/// <param name="subkey"></param>
		[Pure]
		[CanBeNull]
		public static Int64? GetInt64( [NotNull] String folder, [NotNull] String key, [NotNull] String subkey ) {
			if ( String.IsNullOrWhiteSpace( folder ) ) {
				throw new ArgumentException( "Value cannot be null or whitespace.", nameof( folder ) );
			}

			if ( String.IsNullOrWhiteSpace( key ) ) {
				throw new ArgumentException( "Value cannot be null or whitespace.", nameof( key ) );
			}

			if ( String.IsNullOrWhiteSpace( subkey ) ) {
				throw new ArgumentException( "Value cannot be null or whitespace.", nameof( subkey ) );
			}

			try {
				using var registryKey = TheApplication.OpenSubKey( folder, RegistryKeyPermissionCheck.ReadSubTree );

				using var subKey = registryKey?.OpenSubKey( subkey, RegistryKeyPermissionCheck.ReadSubTree );

				if ( Int64.TryParse( subKey?.GetValue( key )?.ToString(), out var result ) ) {
					return result;
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
		[DebuggerStepThrough]
		public static String? GetString( [NotNull] String folder, [NotNull] String key ) {
			if ( String.IsNullOrWhiteSpace( folder ) ) {
				throw new ArgumentException( "Value cannot be null or whitespace.", nameof( folder ) );
			}

			if ( String.IsNullOrWhiteSpace( key ) ) {
				throw new ArgumentException( "Value cannot be null or whitespace.", nameof( key ) );
			}

			try {
				using var registryKey = TheApplication.OpenSubKey( folder, RegistryKeyPermissionCheck.ReadSubTree );

				return registryKey?.GetValue( key )?.ToStringOrNull();
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
		/// <param name="subkey"></param>
		[Pure]
		[CanBeNull]
		public static String? GetString( [NotNull] String folder, [NotNull] String key, [NotNull] String subkey ) {
			if ( String.IsNullOrWhiteSpace( folder ) ) {
				throw new ArgumentException( "Value cannot be null or whitespace.", nameof( folder ) );
			}

			if ( String.IsNullOrWhiteSpace( key ) ) {
				throw new ArgumentException( "Value cannot be null or whitespace.", nameof( key ) );
			}

			if ( String.IsNullOrWhiteSpace( subkey ) ) {
				throw new ArgumentException( "Value cannot be null or whitespace.", nameof( subkey ) );
			}

			try {
				using var registryKey = TheApplication.OpenSubKey( folder, RegistryKeyPermissionCheck.ReadSubTree );

				using var subKey = registryKey?.OpenSubKey( subkey, RegistryKeyPermissionCheck.ReadSubTree );

				return subKey?.GetValue( key )?.ToStringOrNull();
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
		/// <param name="kind">  </param>
		/// <returns></returns>
		public static Boolean Set<T>( [NotNull] String folder, [NotNull] String key, [CanBeNull] T value, RegistryValueKind kind ) {
			if ( String.IsNullOrWhiteSpace( folder ) ) {
				throw new ArgumentException( "Value cannot be null or whitespace.", nameof( folder ) );
			}

			if ( String.IsNullOrWhiteSpace( key ) ) {
				throw new ArgumentException( "Value cannot be null or whitespace.", nameof( key ) );
			}

			using var regFolder = TheApplication.CreateSubKey( folder, RegistryKeyPermissionCheck.ReadWriteSubTree );

			if ( regFolder is null ) {
				$"Error creating subkey {folder}".Break();

				return default;
			}

			try {
				if ( value is null ) {
					regFolder.DeleteValue( key, false );
				}
				else {
					regFolder.SetValue( key, value, kind );
				}

				return true;
			}
			catch ( Exception exception ) {
				exception.Break();
			}

			return default;
		}

		/// <summary>
		///     <para>Sets the <paramref name="value" /> of the current user's software's company's application's folder's key.</para>
		/// </summary>
		/// <param name="folder"></param>
		/// <param name="key">   </param>
		/// <param name="subkey"></param>
		/// <param name="value"> </param>
		/// <param name="kind">  </param>
		/// <returns></returns>
		public static Boolean Set<T>( [NotNull] String folder, [NotNull] String key, [NotNull] String subkey, [CanBeNull] T value, RegistryValueKind kind ) {
			if ( String.IsNullOrWhiteSpace( folder ) ) {
				throw new ArgumentException( "Value cannot be null or whitespace.", nameof( folder ) );
			}

			if ( String.IsNullOrWhiteSpace( key ) ) {
				throw new ArgumentException( "Value cannot be null or whitespace.", nameof( key ) );
			}

			if ( String.IsNullOrWhiteSpace( subkey ) ) {
				throw new ArgumentException( "Value cannot be null or whitespace.", nameof( subkey ) );
			}

			using var registryKey = TheApplication.CreateSubKey( folder, RegistryKeyPermissionCheck.ReadWriteSubTree );

			if ( registryKey is null ) {
				return default;
			}

			using var subKey = registryKey.CreateSubKey( subkey, RegistryKeyPermissionCheck.ReadWriteSubTree );

			if ( subKey is null ) {
				$"Error creating subkey {folder}".Break();

				return default;
			}

			try {
				if ( value is null ) {
					subKey.DeleteValue( key );
				}
				else {
					subKey.SetValue( key, value, kind );
				}

				return true;
			}
			catch ( Exception exception ) {
				exception.Break();
			}

			return default;
		}

		/// <summary>
		///     <para>Sets the <paramref name="value" /> of the current user's software's company's application's folder's key.</para>
		/// </summary>
		/// <param name="folder"></param>
		/// <param name="key">   </param>
		/// <param name="value"> </param>
		public static void Set( [NotNull] String folder, [NotNull] String key, [CanBeNull] String? value ) => Set( folder, key, value, RegistryValueKind.String );

		/// <summary>
		///     <para>Sets the <paramref name="value" /> of the current user's software's company's application's folder's key.</para>
		/// </summary>
		/// <param name="folder"></param>
		/// <param name="key">   </param>
		/// <param name="value"> </param>
		public static void Set( [NotNull] String folder, [NotNull] String key, Int32 value ) => Set( folder, key, value, RegistryValueKind.DWord );

		/// <summary>
		///     <para>Sets the <paramref name="value" /> of the current user's software's company's application's folder's key.</para>
		/// </summary>
		/// <param name="folder"></param>
		/// <param name="key">   </param>
		/// <param name="value"> </param>
		public static void Set( [NotNull] String folder, [NotNull] String key, Int64 value ) => Set( folder, key, value, RegistryValueKind.QWord );

	}

}