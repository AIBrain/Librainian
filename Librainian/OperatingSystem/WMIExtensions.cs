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
// File "WMIExtensions.cs" last formatted on 2022-12-22 at 5:18 PM by Protiguous.

namespace Librainian.OperatingSystem;

using System;
using System.Management;
using Converters;
using Parsing;

public static class WMIExtensions {

	public static String Identifier( String wmiClass, String wmiProperty, String wmiMustBeTrue ) {
		if ( String.IsNullOrWhiteSpace( wmiClass ) ) {
			throw new ArgumentException( "Value cannot be null or whitespace.", nameof( wmiClass ) );
		}

		if ( String.IsNullOrWhiteSpace( wmiProperty ) ) {
			throw new ArgumentException( "Value cannot be null or whitespace.", nameof( wmiProperty ) );
		}

		if ( String.IsNullOrWhiteSpace( wmiMustBeTrue ) ) {
			throw new ArgumentException( "Value cannot be null or whitespace.", nameof( wmiMustBeTrue ) );
		}

		using var managementClass = new ManagementClass( wmiClass );

		var instances = managementClass.GetInstances();

		foreach ( var baseObject in instances ) {
			if ( baseObject is ManagementObject managementObject && managementObject[ wmiMustBeTrue ].ToBoolean() && managementObject[ wmiProperty ] is String identifier ) {
				try {
					return identifier;
				}
				catch {

					// ignored
				}
			}
		}

		return String.Empty;
	}

	public static String Identifier( String wmiClass, String wmiProperty ) {
		if ( String.IsNullOrWhiteSpace( wmiClass ) ) {
			throw new ArgumentException( "Value cannot be null or whitespace.", nameof( wmiClass ) );
		}

		if ( String.IsNullOrWhiteSpace( wmiProperty ) ) {
			throw new ArgumentException( "Value cannot be null or whitespace.", nameof( wmiProperty ) );
		}

		using var managementClass = new ManagementClass( wmiClass );

		var instances = managementClass.GetInstances();

		foreach ( var baseObject in instances ) {
			try {
				if ( baseObject is ManagementObject managementObject && managementObject[ wmiProperty ] is String identifier ) {
					return identifier;
				}
			}
			catch {

				// ignored
			}
		}

		return String.Empty;
	}

	public static ManagementObjectCollection QueryWMI( String? machineName, String scope, String query ) {
		if ( String.IsNullOrWhiteSpace( query ) ) {
			throw new ArgumentException( "Value cannot be null or whitespace.", nameof( query ) );
		}

		if ( String.IsNullOrWhiteSpace( scope ) ) {
			throw new ArgumentException( "Value cannot be null or whitespace.", nameof( scope ) );
		}

		var conn = new ConnectionOptions();
		var nameSpace = @"\\";
		machineName = machineName.Trimmed();
		nameSpace += machineName != String.Empty ? machineName : ".";
		nameSpace += $@"\root\{scope}";
		var managementScope = new ManagementScope( nameSpace, conn );
		var wmiQuery = new ObjectQuery( query );

		using var moSearcher = new ManagementObjectSearcher( managementScope, wmiQuery );

		return moSearcher.Get();
	}

	public static ManagementObjectCollection WmiQuery( this String query ) {
		if ( String.IsNullOrWhiteSpace( query ) ) {
			throw new ArgumentException( "Value cannot be null or whitespace.", nameof( query ) );
		}

		var oQuery = new ObjectQuery( query );

		using var oSearcher = new ManagementObjectSearcher( oQuery );

		return oSearcher.Get();
	}
}