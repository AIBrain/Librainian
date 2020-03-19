// Copyright © 2020 Protiguous. All Rights Reserved.
// 
// This entire copyright notice and license must be retained and must be kept visible in any binaries, libraries, repositories, and source code (directly or derived)
// from our binaries, libraries, projects, or solutions.
// 
// This source code contained in "Info.cs" belongs to Protiguous@Protiguous.com unless otherwise specified or the original license has been overwritten
// by formatting. (We try to avoid it from happening, but it does accidentally happen.)
// 
// Any unmodified portions of source code gleaned from other projects still retain their original license and our thanks goes to those Authors.
// If you find your code in this source code, please let us know so we can properly attribute you and include the proper license and/or copyright.
// 
// If you want to use any of our code in a commercial project, you must contact Protiguous@Protiguous.com for permission and a quote.
// 
// Donations are accepted via bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2 and PayPal: Protiguous@Protiguous.com
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
// For business inquiries, please contact me at Protiguous@Protiguous.com.
// 
// Our website can be found at "https://Protiguous.com/"
// Our software can be found at "https://Protiguous.Software/"
// Our GitHub address is "https://github.com/Protiguous".
// Feel free to browse any source code we make available.
// 
// Project: "LibrainianCore", File: "Info.cs" was last formatted by Protiguous on 2020/03/16 at 3:10 PM.

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
        public static String BuildBranch() => ( TryGetRegistryKeyHKLM( path: CurrentVersion, key: "BuildBranch", value: out var value ) ? value : null ) as String;

        /// <summary>Returns the Windows build. "14393"</summary>
        public static UInt32? BuildMajor() {

            if ( TryGetRegistryKeyHKLM( path: CurrentVersion, key: "CurrentBuildNumber", value: out var value ) ) {
                return Convert.ToUInt32( value: value );
            }

            if ( TryGetRegistryKeyHKLM( path: CurrentVersion, key: "CurrentBuild", value: out value ) ) {
                return Convert.ToUInt32( value: value );
            }

            return null;
        }

        /// <summary>Returns the Windows build.</summary>
        public static UInt32? BuildMinor() {

            if ( TryGetRegistryKeyHKLM( path: CurrentVersion, key: "UBR", value: out var value ) ) {
                return Convert.ToUInt32( value: value );
            }

            return null;
        }

        /// <summary>Returns whether or not the current computer is a server or not.</summary>
        public static Boolean? IsServer() {
            if ( TryGetRegistryKeyHKLM( path: CurrentVersion, key: "InstallationType", value: out var installationType ) ) {
                return !installationType?.ToString().Like( right: "Client" );
            }

            return null;
        }

        /// <summary>Returns the Windows release id.</summary>
        public static UInt32? ReleaseId() =>
            TryGetRegistryKeyHKLM( path: CurrentVersion, key: "ReleaseId", value: out var value ) ? Convert.ToUInt32( value: value ) : ( UInt32? ) null;

        public static Boolean TryGetRegistryKeyHKLM( [NotNull] String path, [NotNull] String key, [CanBeNull] out Object value ) {
            if ( path is null ) {
                throw new ArgumentNullException( paramName: nameof( path ) );
            }

            if ( key is null ) {
                throw new ArgumentNullException( paramName: nameof( key ) );
            }

            value = null;

            try {
                using ( var rk = Registry.LocalMachine.OpenSubKey( name: path ) ) {
                    if ( rk is null ) {
                        return default;
                    }

                    value = rk.GetValue( name: key );

                    return value != null;
                }
            }
            catch {
                return default;
            }
        }

        /// <summary>Returns the Windows major version number for this computer.</summary>
        public static UInt32? VersionMajor() =>
            TryGetRegistryKeyHKLM( path: CurrentVersion, key: "CurrentMajorVersionNumber", value: out var value ) ? value.Cast<UInt32?>() : null;

        /// <summary>Returns the Windows minor version number for this computer.</summary>
        public static UInt32? VersionMinor() =>
            TryGetRegistryKeyHKLM( path: CurrentVersion, key: "CurrentMinorVersionNumber", value: out var value ) ? value.Cast<UInt32?>() : null;

    }

}