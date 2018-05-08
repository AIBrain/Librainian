// Copyright 2016 Protiguous.
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
// "Librainian/Info.cs" was last cleaned by Protiguous on 2016/12/02 at 9:10 PM

namespace Librainian.OperatingSystem {

	using System;
	using JetBrains.Annotations;
	using Microsoft.Win32;

	/// <summary>
	///     Static class that adds convenient methods for getting information on the running computers
	///     basic hardware and os setup.
	/// </summary>
	/// <remarks>Adapted from <see cref="http://stackoverflow.com/a/37755503/956364" />.</remarks>
	public static class Info {

		private const String CurrentVersion = @"SOFTWARE\Microsoft\Windows NT\CurrentVersion";

		/// <summary>
		///     Returns the Windows build. "rs1_release"
		/// </summary>
		[CanBeNull]
		public static String BuildBranch() =>

#pragma warning disable CC0021 // Use nameof
			TryGetRegistryKeyHKLM( CurrentVersion, "BuildBranch", out var value ) ? value : null;
#pragma warning restore CC0021 // Use nameof


		/// <summary>
		///     Returns the Windows build. "14393"
		/// </summary>
		public static UInt32? BuildMajor() {

			if ( TryGetRegistryKeyHKLM( CurrentVersion, "CurrentBuildNumber", out var value ) ) {
				return Convert.ToUInt32( value );
			}

			if ( TryGetRegistryKeyHKLM( CurrentVersion, "CurrentBuild", out value ) ) {
				return Convert.ToUInt32( value );
			}

			return null;
		}

		/// <summary>
		///     Returns the Windows build.
		/// </summary>
		public static UInt32? BuildMinor() {

			if ( TryGetRegistryKeyHKLM( CurrentVersion, "UBR", out var value ) ) {
				return Convert.ToUInt32( value );
			}

			return null;
		}

		/// <summary>
		///     Returns whether or not the current computer is a server or not.
		/// </summary>
		public static Boolean? IsServer() {
			if ( TryGetRegistryKeyHKLM( CurrentVersion, "InstallationType", out var installationType ) ) {
				return !installationType.Like( "Client" );
			}

			return null;
		}

		/// <summary>
		///     Returns the Windows release id.
		/// </summary>
		public static UInt32? ReleaseId() => TryGetRegistryKeyHKLM( CurrentVersion, "ReleaseId", out var value ) ? Convert.ToUInt32( value ) : null;

		public static Boolean TryGetRegistryKeyHKLM( [ NotNull ] String path, [ NotNull ] String key, out dynamic value ) {
			if ( path is null ) {
				throw new ArgumentNullException( nameof( path ) );
			}
			if ( key is null ) {
				throw new ArgumentNullException( nameof( key ) );
			}
			value = null;
			try {
				using ( var rk = Registry.LocalMachine.OpenSubKey( path ) ) {
					if ( rk is null ) {
						return false;
					}
					value = rk.GetValue( key );
					return value != null;
				}
			}

			catch {
				return false;
			}

		}

		/// <summary>
		///     Returns the Windows major version number for this computer.
		/// </summary>
		public static UInt32? VersionMajor() => TryGetRegistryKeyHKLM( CurrentVersion, "CurrentMajorVersionNumber", out var value ) ? ( UInt32? )( UInt32 )value : null;

		/// <summary>
		///     Returns the Windows minor version number for this computer.
		/// </summary>
		public static UInt32? VersionMinor() => TryGetRegistryKeyHKLM( CurrentVersion, "CurrentMinorVersionNumber", out var value ) ? ( UInt32? )( UInt32 )value : null;

	}

}
