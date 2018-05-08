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
// "Librainian/ComputerFingerPrint.cs" was last cleaned by Protiguous on 2016/06/18 at 10:55 PM

namespace Librainian.OperatingSystem {

    using System;
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
        private static String BiosId() => WMIExtensions.Identifier( "Win32_BIOS", "Manufacturer" )
										  + WMIExtensions.Identifier( "Win32_BIOS", "SMBIOSBIOSVersion" )
										  + WMIExtensions.Identifier( "Win32_BIOS", "IdentificationCode" )
										  + WMIExtensions.Identifier( "Win32_BIOS", "SerialNumber" )
										  + WMIExtensions.Identifier( "Win32_BIOS", "ReleaseDate" )
										  + WMIExtensions.Identifier( "Win32_BIOS", "Version" );

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
        private static String DiskId() => WMIExtensions.Identifier( "Win32_DiskDrive", "Model" ) + WMIExtensions.Identifier( "Win32_DiskDrive", "Manufacturer" ) + WMIExtensions.Identifier( "Win32_DiskDrive", "Signature" ) + WMIExtensions.Identifier( "Win32_DiskDrive", "TotalHeads" );

	    //First enabled network card ID
        private static String MacId() => WMIExtensions.Identifier( "Win32_NetworkAdapterConfiguration", "MACAddress", "IPEnabled" );

	    //Motherboard ID
        private static String MotherboardId() => WMIExtensions.Identifier( "Win32_BaseBoard", "Model" ) + WMIExtensions.Identifier( "Win32_BaseBoard", "Manufacturer" ) + WMIExtensions.Identifier( "Win32_BaseBoard", "Name" ) + WMIExtensions.Identifier( "Win32_BaseBoard", "SerialNumber" );

	    //Primary video controller ID
        private static String VideoId() => WMIExtensions.Identifier( "Win32_VideoController", "DriverVersion" ) + WMIExtensions.Identifier( "Win32_VideoController", "Name" );

    }
}
