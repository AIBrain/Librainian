﻿// Copyright © Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible in any binaries, libraries, repositories, or source code (directly or derived) from our binaries, libraries, projects, solutions, or applications.
//
// All source code belongs to Protiguous@Protiguous.com unless otherwise specified or the original license has been overwritten by formatting. (We try to avoid it from happening, but it does accidentally happen.)
//
// Any unmodified portions of source code gleaned from other sources still retain their original license and our thanks goes to those Authors.
// If you find your code unattributed in this source code, please let us know so we can properly attribute you and include the proper license and/or copyright(s).
//
// If you want to use any of our code in a commercial project, you must contact Protiguous@Protiguous.com for permission, license, and a quote.
//
// Donations, payments, and royalties are accepted via bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2 and PayPal: Protiguous@Protiguous.com
//
// ====================================================================
// Disclaimer:  Usage of the source code or binaries is AS-IS.
//     No warranties are expressed, implied, or given.
//     We are NOT responsible for Anything You Do With Our Code.
//     We are NOT responsible for Anything You Do With Our Executables.
//     We are NOT responsible for Anything You Do With Your Computer.
// ====================================================================
//
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
// For business inquiries, please contact me at Protiguous@Protiguous.com.
//
// Our software can be found at "https://Protiguous.Software/"
// Our GitHub address is "https://github.com/Protiguous".

namespace Librainian.OperatingSystem {

    using System;
    using Converters;
    using JetBrains.Annotations;
    using Microsoft.Win32;
    using Parsing;

    /// <summary>Static class that adds convenient methods for getting information on the running computers basic hardware and os setup.</summary>
    /// <remarks>Adapted from <see cref="http://stackoverflow.com/a/37755503/956364" />.</remarks>
    public static class Info {

        private const String CurrentVersion = @"SOFTWARE\Microsoft\Windows NT\CurrentVersion";

        /// <summary>Returns the Windows build. "rs1_release"</summary>
        [CanBeNull]
        public static String BuildBranch() => ( TryGetRegistryKeyHKLM( CurrentVersion, "BuildBranch", out var value ) ? value : null ) as String;

        /// <summary>Returns the Windows build. "14393"</summary>
        public static UInt32? BuildMajor() {

            if ( TryGetRegistryKeyHKLM( CurrentVersion, "CurrentBuildNumber", out var value ) ) {
                return Convert.ToUInt32( value );
            }

            if ( TryGetRegistryKeyHKLM( CurrentVersion, "CurrentBuild", out value ) ) {
                return Convert.ToUInt32( value );
            }

            return null;
        }

        /// <summary>Returns the Windows build.</summary>
        public static UInt32? BuildMinor() {

            if ( TryGetRegistryKeyHKLM( CurrentVersion, "UBR", out var value ) ) {
                return Convert.ToUInt32( value );
            }

            return null;
        }

        /// <summary>Returns whether or not the current computer is a server or not.</summary>
        public static Boolean? IsServer() {
            if ( TryGetRegistryKeyHKLM( CurrentVersion, "InstallationType", out var installationType ) ) {
                return !installationType?.ToString().Like( "Client" );
            }

            return null;
        }

        /// <summary>Returns the Windows release id.</summary>
        public static UInt32? ReleaseId() =>
            TryGetRegistryKeyHKLM( CurrentVersion, "ReleaseId", out var value ) ? Convert.ToUInt32( value ) : ( UInt32? ) null;

        public static Boolean TryGetRegistryKeyHKLM( [NotNull] String path, [NotNull] String key, [CanBeNull] out Object value ) {
            if ( path is null ) {
                throw new ArgumentNullException( nameof( path ) );
            }

            if ( key is null ) {
                throw new ArgumentNullException( nameof( key ) );
            }

            value = null;

            try {
				using var rk = Registry.LocalMachine.OpenSubKey( path );

				if ( rk is null ) {
					return default;
				}

				value = rk.GetValue( key );

				return value != null;
			}
            catch {
                return default;
            }
        }

        /// <summary>Returns the Windows major version number for this computer.</summary>
        public static UInt32? VersionMajor() =>
            TryGetRegistryKeyHKLM( CurrentVersion, "CurrentMajorVersionNumber", out var value ) ? value.Cast<UInt32?>() : null;

        /// <summary>Returns the Windows minor version number for this computer.</summary>
        public static UInt32? VersionMinor() =>
            TryGetRegistryKeyHKLM( CurrentVersion, "CurrentMinorVersionNumber", out var value ) ? value.Cast<UInt32?>() : null;

    }

}