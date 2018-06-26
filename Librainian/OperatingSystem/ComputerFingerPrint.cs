// Copyright © Rick@AIBrain.Org and Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our source code, binaries, libraries, projects, or solutions.
//
// This source code contained in "ComputerFingerPrint.cs" belongs to Protiguous@Protiguous.com
// and Rick@AIBrain.org and unless otherwise specified or the original license has been
// overwritten by automatic formatting.
// (We try to avoid it from happening, but it does accidentally happen.)
//
// Any unmodified portions of source code gleaned from other projects still retain their original
// license and our Thanks goes to those Authors. If you find your code in this source code, please
// let us know so we can properly attribute you and include the proper license and/or copyright.
//
// If you want to use any of our code, you must contact Protiguous@Protiguous.com or
// Sales@AIBrain.org for permission and a quote.
//
// Donations are accepted (for now) via
//    bitcoin:1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//    paypal@AIBrain.Org
//    (We're still looking into other solutions! Any ideas?)
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
// For business inquiries, please contact me at Protiguous@Protiguous.com .
//
// Our website can be found at "https://Protiguous.com/"
// Our software can be found at "https://Protiguous.Software/"
// Our GitHub address is "https://github.com/Protiguous".
// Feel free to browse any source code we *might* make available.
//
// ***  Project "Librainian"  ***
// File "ComputerFingerPrint.cs" was last formatted by Protiguous on 2018/06/26 at 1:34 AM.

namespace Librainian.OperatingSystem {

	using System;
	using JetBrains.Annotations;
	using Parsing;
	using WMI;

	/// <summary>
	///     Generates a n*16 byte Unique Identification code (hash) of a computer
	///     Example: 4876-8DB5-EE85-69D3-FE52-8CF7-395D-2EA9
	/// </summary>
	public static class ComputerFingerPrint {

		//TODO totally unfinished and untested.

		//        public static String bob() {
		//            DriveListEx diskInfo = new DriveListEx();
		//            diskInfo.Load();
		//String serialNo = diskInfo[ 0 ].SerialNumber;

		// }

		/*
                private static String _fingerPrint = String.Empty;
        */

		//public static String Value() {
		//    if ( !String.IsNullOrEmpty( _fingerPrint ) ) {
		//        return _fingerPrint;
		//    }
		//    _fingerPrint = GetHash( "CPU >> " + CPUID() + "\nBIOS >> " + biosId() + "\nBASE >> " + baseId()

		//    //+"\nDISK >> "+ diskId() + "\nVIDEO >> " +
		//    videoId() + "\nMAC >> " + macId()
		//    )
		//    ;
		//    return _fingerPrint;
		//}

		//Return a hardware identifier

		//Return a hardware identifier

		//BIOS Identifier
		[NotNull]
		private static String BiosId() =>
			WMIExtensions.Identifier( "Win32_BIOS", "Manufacturer" ) + WMIExtensions.Identifier( "Win32_BIOS", "SMBIOSBIOSVersion" ) + WMIExtensions.Identifier( "Win32_BIOS", "IdentificationCode" ) +
			WMIExtensions.Identifier( "Win32_BIOS", "SerialNumber" ) + WMIExtensions.Identifier( "Win32_BIOS", "ReleaseDate" ) + WMIExtensions.Identifier( "Win32_BIOS", "Version" );

		private static String CPUID() {

			//Uses first CPU identifier available in order of preference
			//Don't get all identifiers, as it is very time consuming
			var retVal = WMIExtensions.Identifier( "Win32_Processor", "UniqueId" );

			if ( !retVal.IsNullOrEmpty() ) {
				return retVal;
			}

			retVal = WMIExtensions.Identifier( "Win32_Processor", "ProcessorId" );

			if ( !retVal.IsNullOrEmpty() ) {
				return retVal;
			}

			retVal = WMIExtensions.Identifier( "Win32_Processor", "Name" );

			if ( retVal == String.Empty ) //If no Name, use Manufacturer
			{
				retVal = WMIExtensions.Identifier( "Win32_Processor", "Manufacturer" );
			}

			//Add clock speed for extra security
			retVal += WMIExtensions.Identifier( "Win32_Processor", "MaxClockSpeed" );

			return retVal;
		}

		//Main physical hard drive ID
		[NotNull]
		private static String DiskId() =>
			WMIExtensions.Identifier( "Win32_DiskDrive", "Model" ) + WMIExtensions.Identifier( "Win32_DiskDrive", "Manufacturer" ) + WMIExtensions.Identifier( "Win32_DiskDrive", "Signature" ) +
			WMIExtensions.Identifier( "Win32_DiskDrive", "TotalHeads" );

		//First enabled network card ID
		private static String MacId() => WMIExtensions.Identifier( "Win32_NetworkAdapterConfiguration", "MACAddress", "IPEnabled" );

		//Motherboard ID
		[NotNull]
		private static String MotherboardId() =>
			WMIExtensions.Identifier( "Win32_BaseBoard", "Model" ) + WMIExtensions.Identifier( "Win32_BaseBoard", "Manufacturer" ) + WMIExtensions.Identifier( "Win32_BaseBoard", "Name" ) +
			WMIExtensions.Identifier( "Win32_BaseBoard", "SerialNumber" );

		//Primary video controller ID
		[NotNull]
		private static String VideoId() => WMIExtensions.Identifier( "Win32_VideoController", "DriverVersion" ) + WMIExtensions.Identifier( "Win32_VideoController", "Name" );
	}
}