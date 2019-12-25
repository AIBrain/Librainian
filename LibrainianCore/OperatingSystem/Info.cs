// Copyright © Rick@AIBrain.org and Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
//
// This source code contained in "Info.cs" belongs to Protiguous@Protiguous.com and
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
//     PayPal:Protiguous@Protiguous.com
//     (We're always looking into other solutions.. Any ideas?)
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
// Feel free to browse any source code we make available.
//
// Project: "Librainian", "Info.cs" was last formatted by Protiguous on 2019/11/20 at 4:57 AM.

namespace LibrainianCore.OperatingSystem {

    using System;
    using System.Diagnostics.CodeAnalysis;
    using Converters;
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
        public static UInt32? ReleaseId() => TryGetRegistryKeyHKLM( CurrentVersion, "ReleaseId", out var value ) ? Convert.ToUInt32( value ) : ( UInt32? )null;

        public static Boolean TryGetRegistryKeyHKLM( [NotNull] String path, [NotNull] String key, [CanBeNull] out Object value ) {
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

        /// <summary>Returns the Windows major version number for this computer.</summary>
        public static UInt32? VersionMajor() => TryGetRegistryKeyHKLM( CurrentVersion, "CurrentMajorVersionNumber", out var value ) ? value.Cast<UInt32?>() : null;

        /// <summary>Returns the Windows minor version number for this computer.</summary>
        public static UInt32? VersionMinor() => TryGetRegistryKeyHKLM( CurrentVersion, "CurrentMinorVersionNumber", out var value ) ? value.Cast<UInt32?>() : null;
    }
}