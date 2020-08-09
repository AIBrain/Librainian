// Copyright © Protiguous. All Rights Reserved.
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

namespace Librainian.ComputerSystem {

	using System;
	using System.Linq;
	using System.Management;
	using JetBrains.Annotations;
	using Logging;

	public class WinSatScores {

		[NotNull]
		private ManagementObjectSearcher Instance =>
			this.Searcher.Value ?? throw new InvalidOperationException( $"Unable to use {nameof( ManagementObjectSearcher )}." );

		[NotNull]
		public Lazy<ManagementObjectSearcher> Searcher { get; } = new Lazy<ManagementObjectSearcher>( () =>
			new ManagementObjectSearcher( "root\\CIMV2", "SELECT * FROM Win32_WinSAT" ) );

		public Single? CPU() {
			try {
				foreach ( var queryObj in this.Instance.Get().OfType<ManagementObject>() ) {
					if ( Single.TryParse( queryObj[ "CPUScore" ]?.ToString(), out var result ) ) {
						return result;
					}
				}
			}
			catch ( ManagementException exception ) {
				exception.Log();
			}

			return null;
		}

		public Single? D3D() {
			try {
				foreach ( var queryObj in this.Instance.Get().OfType<ManagementObject>() ) {
					if ( Single.TryParse( queryObj[ "D3DScore" ]?.ToString(), out var result ) ) {
						return result;
					}
				}
			}
			catch ( ManagementException exception ) {
				exception.Log();
			}

			return null;
		}

		public Single? Disk() {
			try {
				foreach ( var queryObj in this.Instance.Get().OfType<ManagementObject>() ) {
					if ( Single.TryParse( queryObj[ "DiskScore" ]?.ToString(), out var result ) ) {
						return result;
					}
				}
			}
			catch ( ManagementException exception ) {
				exception.Log();
			}

			return null;
		}

		public Single? Graphics() {
			try {
				foreach ( var queryObj in this.Instance.Get().OfType<ManagementObject>() ) {
					if ( Single.TryParse( queryObj[ "GraphicsScore" ]?.ToString(), out var result ) ) {
						return result;
					}
				}
			}
			catch ( ManagementException exception ) {
				exception.Log();
			}

			return null;
		}

		public Single? Memory() {
			try {
				foreach ( var queryObj in this.Instance.Get().OfType<ManagementObject>() ) {
					if ( Single.TryParse( queryObj[ "MemoryScore" ]?.ToString(), out var result ) ) {
						return result;
					}
				}
			}
			catch ( ManagementException exception ) {
				exception.Log();
			}

			return null;
		}

		[CanBeNull]
		public Object TimeTaken() {
			try {
				foreach ( var queryObj in this.Instance.Get().OfType<ManagementObject>() ) {
					return queryObj[ "TimeTaken" ];
				}
			}
			catch ( ManagementException exception ) {
				exception.Log();
			}

			return null;
		}

		public Int32? WinSAT_AssessmentState() {
			try {
				foreach ( var queryObj in this.Instance.Get().OfType<ManagementObject>() ) {
					if ( Int32.TryParse( queryObj[ "WinSATAssessmentState" ]?.ToString(), out var result ) ) {
						return result;
					}
				}
			}
			catch ( ManagementException exception ) {
				exception.Log();
			}

			return null;
		}

		public Single? WinSPRLevel() {
			try {
				foreach ( var queryObj in this.Instance.Get().OfType<ManagementObject>() ) {
					if ( Single.TryParse( queryObj[ "WinSPRLevel" ]?.ToString(), out var result ) ) {
						return result;
					}
				}
			}
			catch ( ManagementException exception ) {
				exception.Log();
			}

			return null;
		}
	}
}