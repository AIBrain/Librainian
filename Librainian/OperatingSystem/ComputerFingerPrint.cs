// Copyright © 2020 Protiguous. All Rights Reserved.
// 
// This entire copyright notice and license must be retained and must be kept visible in any binaries, libraries, repositories, and source code (directly or derived)
// from our binaries, libraries, projects, or solutions.
// 
// This source code contained in "ComputerFingerPrint.cs" belongs to Protiguous@Protiguous.com unless otherwise specified or the original license has been overwritten
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
// Project: "Librainian", File: "ComputerFingerPrint.cs" was last formatted by Protiguous on 2020/03/16 at 5:02 PM.

namespace Librainian.OperatingSystem {

    using System;
    using JetBrains.Annotations;
    using Parsing;
    using WMI;

    /// <summary>Generates a n*16 byte Unique Identification code (hash) of a computer Example: 4876-8DB5-EE85-69D3-FE52-8CF7-395D-2EA9</summary>
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
            WMIExtensions.Identifier( wmiClass: "Win32_BIOS", wmiProperty: "Manufacturer" ) +
            WMIExtensions.Identifier( wmiClass: "Win32_BIOS", wmiProperty: "SMBIOSBIOSVersion" ) +
            WMIExtensions.Identifier( wmiClass: "Win32_BIOS", wmiProperty: "IdentificationCode" ) +
            WMIExtensions.Identifier( wmiClass: "Win32_BIOS", wmiProperty: "SerialNumber" ) + WMIExtensions.Identifier( wmiClass: "Win32_BIOS", wmiProperty: "ReleaseDate" ) +
            WMIExtensions.Identifier( wmiClass: "Win32_BIOS", wmiProperty: "Version" );

        [NotNull]
        private static String CPUID() {

            //Uses first CPU identifier available in order of preference
            //Don't get all identifiers, as it is very time consuming
            var retVal = WMIExtensions.Identifier( wmiClass: "Win32_Processor", wmiProperty: "UniqueId" );

            if ( !retVal.IsNullOrEmpty() ) {
                return retVal;
            }

            retVal = WMIExtensions.Identifier( wmiClass: "Win32_Processor", wmiProperty: "ProcessorId" );

            if ( !retVal.IsNullOrEmpty() ) {
                return retVal;
            }

            retVal = WMIExtensions.Identifier( wmiClass: "Win32_Processor", wmiProperty: "Name" );

            if ( retVal == String.Empty ) //If no Name, use Manufacturer
            {
                retVal = WMIExtensions.Identifier( wmiClass: "Win32_Processor", wmiProperty: "Manufacturer" );
            }

            //Add clock speed for extra security
            retVal += WMIExtensions.Identifier( wmiClass: "Win32_Processor", wmiProperty: "MaxClockSpeed" );

            return retVal;
        }

        //Main physical hard drive ID
        [NotNull]
        private static String DiskId() =>
            WMIExtensions.Identifier( wmiClass: "Win32_DiskDrive", wmiProperty: "Model" ) +
            WMIExtensions.Identifier( wmiClass: "Win32_DiskDrive", wmiProperty: "Manufacturer" ) +
            WMIExtensions.Identifier( wmiClass: "Win32_DiskDrive", wmiProperty: "Signature" ) +
            WMIExtensions.Identifier( wmiClass: "Win32_DiskDrive", wmiProperty: "TotalHeads" );

        //First enabled network card ID
        [NotNull]
        private static String MacId() => WMIExtensions.Identifier( wmiClass: "Win32_NetworkAdapterConfiguration", wmiProperty: "MACAddress", wmiMustBeTrue: "IPEnabled" );

        //Motherboard ID
        [NotNull]
        private static String MotherboardId() =>
            WMIExtensions.Identifier( wmiClass: "Win32_BaseBoard", wmiProperty: "Model" ) +
            WMIExtensions.Identifier( wmiClass: "Win32_BaseBoard", wmiProperty: "Manufacturer" ) +
            WMIExtensions.Identifier( wmiClass: "Win32_BaseBoard", wmiProperty: "Name" ) +
            WMIExtensions.Identifier( wmiClass: "Win32_BaseBoard", wmiProperty: "SerialNumber" );

        //Primary video controller ID
        [NotNull]
        private static String VideoId() =>
            WMIExtensions.Identifier( wmiClass: "Win32_VideoController", wmiProperty: "DriverVersion" ) +
            WMIExtensions.Identifier( wmiClass: "Win32_VideoController", wmiProperty: "Name" );

    }

}