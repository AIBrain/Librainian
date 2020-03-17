// Copyright © 2020 Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible in any binaries, libraries, repositories, and source code (directly or derived)
// from our binaries, libraries, projects, or solutions.
//
// This source code contained in "DeviceClass.cs" belongs to Protiguous@Protiguous.com unless otherwise specified or the original license has been overwritten
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
// Project: "Librainian", File: "DeviceClass.cs" was last formatted by Protiguous on 2020/03/16 at 2:53 PM.

namespace Librainian.ComputerSystem.Devices {

    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.IO;
    using System.Runtime.InteropServices;
    using System.Text;
    using JetBrains.Annotations;
    using OperatingSystem;
    using Utilities;

    /// <summary>A generic base class for physical device classes.</summary>
    public abstract class DeviceClass : ABetterClassDispose {

        private Guid _classGuid;

        private IntPtr _deviceInfoSet;

        /// <summary>Gets the device class's guid.</summary>
        public Guid ClassGuid => this._classGuid;

        /// <summary>Initializes a new instance of the DeviceClass class.</summary>
        /// <param name="classGuid"> A device class Guid.</param>
        /// <param name="hwndParent">The handle of the top-level window to be used for any user interface or IntPtr.Zero for no handle.</param>
        private DeviceClass( Guid classGuid, IntPtr hwndParent ) {
            this._classGuid = classGuid;

            this._deviceInfoSet = NativeMethods.SetupDiGetClassDevs( classGuid: ref this._classGuid, enumerator: "" /*was 0*/, hwndParent: hwndParent,
                flags: NativeMethods.DIGCF_DEVICEINTERFACE | NativeMethods.DIGCF_PRESENT );

            var lastError = Marshal.GetLastWin32Error();

            if ( this._deviceInfoSet == ( IntPtr )( -1 ) ) {
                throw new Win32Exception( error: lastError );
            }
        }

        protected DeviceClass( Guid classGuid ) : this( classGuid: classGuid, hwndParent: IntPtr.Zero ) { }

        [NotNull]
        protected virtual Device CreateDevice( [NotNull] DeviceClass deviceClass, NativeMethods.SP_DEVINFO_DATA deviceInfoData, [CanBeNull] String? path, Int32 index,
            Int32 disknum = -1 ) =>
            new Device( deviceClass: deviceClass, deviceInfoData: deviceInfoData, path: path, index: index, diskNumber: disknum );

        internal NativeMethods.SP_DEVINFO_DATA GetInfo( Int32 dnDevInst ) {
            var sb = new StringBuilder( capacity: 1024 );
            var hr = NativeMethods.CM_Get_Device_ID( dnDevInst: dnDevInst, buffer: sb, bufferLen: sb.Capacity, ulFlags: 0 );

            if ( hr != 0 ) {
                throw new Win32Exception( error: hr );
            }

            var devData = new NativeMethods.SP_DEVINFO_DATA();
            devData.cbSize = ( UInt32 )Marshal.SizeOf( structure: devData );

            if ( !NativeMethods.SetupDiOpenDeviceInfo( deviceInfoSet: this._deviceInfoSet, deviceInstanceId: sb.ToString(), hwndParent: IntPtr.Zero, openFlags: 0,
                deviceInfoData: devData ) ) {
                throw new Win32Exception( error: Marshal.GetLastWin32Error() );
            }

            return devData;
        }

        [NotNull]
        internal String GetProperty( NativeMethods.SP_DEVINFO_DATA devData, UInt32 property, [NotNull] String defaultValue ) {
            if ( defaultValue == null ) {
                throw new ArgumentNullException( paramName: nameof( defaultValue ) );
            }

            const Int32 propertyBufferSize = 1024;

            var propertyBuffer = new Byte[ propertyBufferSize ];

            if ( !NativeMethods.SetupDiGetDeviceRegistryProperty( deviceInfoSet: this._deviceInfoSet, deviceInfoData: ref devData, property: property,
                propertyRegDataType: out var propertyRegDataType, propertyBuffer: propertyBuffer, propertyBufferSize: propertyBufferSize,
                requiredSize: out var requiredSize ) ) {

                //Marshal.FreeHGlobal( propertyBuffer );
                var error = Marshal.GetLastWin32Error();

                if ( error != NativeMethods.ERROR_INVALID_DATA ) {
                    throw new Win32Exception( error: error );
                }

                return defaultValue;
            }

            //var value = Marshal.PtrToStringAuto( propertyBuffer );
            //Marshal.FreeHGlobal( propertyBuffer );
            return Encoding.Default.GetString( bytes: propertyBuffer );
        }

        internal UInt32 GetProperty( NativeMethods.SP_DEVINFO_DATA devData, UInt32 property, UInt32 defaultValue ) {

            var propertyBufferSize = ( UInt32 )Marshal.SizeOf( t: typeof( UInt32 ) );

            //var propertyBuffer = Marshal.AllocHGlobal( propertyBufferSize );
            var propertyBuffer = new Byte[ propertyBufferSize ];

            if ( !NativeMethods.SetupDiGetDeviceRegistryProperty( deviceInfoSet: this._deviceInfoSet, deviceInfoData: ref devData, property: property,
                propertyRegDataType: out var propertyRegDataType, propertyBuffer: propertyBuffer, propertyBufferSize: propertyBufferSize,
                requiredSize: out var requiredSize ) ) {

                //Marshal.FreeHGlobal( propertyBuffer );
                var error = Marshal.GetLastWin32Error();

                if ( error != NativeMethods.ERROR_INVALID_DATA ) {
                    throw new Win32Exception( error: error );
                }

                return defaultValue;
            }

            //var value = ( Int32 )Marshal.PtrToStructure( propertyBuffer, typeof( Int32 ) );
            //Marshal.FreeHGlobal( propertyBuffer );
            //return value;
            return Convert.ToUInt32( value: propertyBuffer );
        }

        internal Guid GetProperty( NativeMethods.SP_DEVINFO_DATA devData, UInt32 property, Guid defaultValue ) {

            var propertyBufferSize = ( UInt32 )Marshal.SizeOf( t: typeof( Guid ) );

            var propertyBuffer = new Byte[ propertyBufferSize ];

            if ( !NativeMethods.SetupDiGetDeviceRegistryProperty( deviceInfoSet: this._deviceInfoSet, deviceInfoData: ref devData, property: property,
                propertyRegDataType: out var propertyRegDataType, propertyBuffer: propertyBuffer, propertyBufferSize: propertyBufferSize,
                requiredSize: out var requiredSize ) ) {

                //Marshal.FreeHGlobal( propertyBuffer );
                var error = Marshal.GetLastWin32Error();

                if ( error != NativeMethods.ERROR_INVALID_DATA ) {
                    throw new Win32Exception( error: error );
                }

                return defaultValue;
            }

            return new Guid( b: propertyBuffer );

            //var value = ( Guid )Marshal.PtrToStructure( propertyBuffer, typeof( Guid ) );
            //Marshal.FreeHGlobal( propertyBuffer );
            //return value;
        }

        /// <summary>Dispose any disposable members.</summary>
        public override void DisposeManaged() { }

        /// <summary>Dispose of COM objects, etc...</summary>
        public override void DisposeNative() {
            if ( this._deviceInfoSet != IntPtr.Zero ) {
                NativeMethods.SetupDiDestroyDeviceInfoList( deviceInfoSet: this._deviceInfoSet );
                this._deviceInfoSet = IntPtr.Zero;
            }
        }

        /// <summary>Gets the list of devices of this device class.</summary>
        /// <returns>The devices.</returns>
        /// <exception cref="Win32Exception"></exception>
        [NotNull]
        public IEnumerable<Device> GetDevices() {
            var devices = new HashSet<Device>();
            var index = 0;

            while ( true ) {
                var interfaceData = new NativeMethods.SP_DEVICE_INTERFACE_DATA();
                interfaceData.cbSize = ( UInt32 )Marshal.SizeOf( structure: interfaceData );

                if ( !NativeMethods.SetupDiEnumDeviceInterfaces( deviceInfoSet: this._deviceInfoSet, deviceInfoData: default, interfaceClassGuid: ref this._classGuid,
                    memberIndex: index, deviceInterfaceData: interfaceData ) ) {
                    var error = Marshal.GetLastWin32Error();

                    if ( error != NativeMethods.ERROR_NO_MORE_ITEMS ) {
                        throw new Win32Exception( error: error );
                    }

                    break;
                }

                var devData = new NativeMethods.SP_DEVINFO_DATA();
                devData.cbSize = ( UInt32 )Marshal.SizeOf( structure: devData );
                var size = 0;

                if ( !NativeMethods.SetupDiGetDeviceInterfaceDetail( deviceInfoSet: this._deviceInfoSet, deviceInterfaceData: interfaceData,
                    deviceInterfaceDetailData: IntPtr.Zero, deviceInterfaceDetailDataSize: 0, requiredSize: ref size, deviceInfoData: devData ) ) {
                    var error = Marshal.GetLastWin32Error();

                    if ( error != NativeMethods.ERROR_INSUFFICIENT_BUFFER ) {
                        throw new Win32Exception( error: error );
                    }
                }

                var buffer = Marshal.AllocHGlobal( cb: size );
                var detailData = new NativeMethods.SP_DEVICE_INTERFACE_DETAIL_DATA();
                detailData.cbSize = Marshal.SizeOf( structure: detailData );

                Marshal.WriteInt32( ptr: buffer, val: IntPtr.Size );

                //Marshal.StructureToPtr(detailData, buffer, false);

                if ( !NativeMethods.SetupDiGetDeviceInterfaceDetail( deviceInfoSet: this._deviceInfoSet, deviceInterfaceData: interfaceData, deviceInterfaceDetailData: buffer,
                    deviceInterfaceDetailDataSize: size, requiredSize: ref size, deviceInfoData: devData ) ) {
                    try {
                        throw new Win32Exception( error: Marshal.GetLastWin32Error() );
                    }
                    finally {
                        Marshal.FreeHGlobal( hglobal: buffer );
                    }
                }

                var strPtr = new IntPtr( value: buffer.ToInt64() + 4 );
                var devicePath = Marshal.PtrToStringAuto( ptr: strPtr );

                //IntPtr pDevicePath = (IntPtr)((int)buffer + Marshal.SizeOf(typeof(int)));
                //string devicePath = Marshal.PtrToStringAuto(pDevicePath);
                Marshal.FreeHGlobal( hglobal: buffer );

                if ( this._classGuid.Equals( g: new Guid( g: NativeMethods.GUID_DEVINTERFACE_DISK ) ) ) {

                    // Find disks
                    var hFile = NativeMethods.CreateFile( lpFileName: devicePath, dwDesiredAccess: 0, dwShareMode: FileShare.ReadWrite, lpSecurityAttributes: IntPtr.Zero,
                        dwCreationDisposition: FileMode.Open, dwFlagsAndAttributes: 0, hTemplateFile: IntPtr.Zero );

                    if ( hFile.IsInvalid ) {
                        throw new Win32Exception( error: Marshal.GetLastWin32Error() );
                    }

                    UInt32 bytesReturned = 0;
                    const UInt32 numBufSize = 0x1000; // some big size
                    var numBuffer = Marshal.AllocHGlobal( cb: ( IntPtr )numBufSize );
                    NativeMethods.STORAGE_DEVICE_NUMBER disknum;

                    try {
                        if ( !NativeMethods.DeviceIoControl( hDevice: hFile.DangerousGetHandle(), dwIoControlCode: NativeMethods.IOCTL_STORAGE_GET_DEVICE_NUMBER,
                            lpInBuffer: IntPtr.Zero, nInBufferSize: 0, lpOutBuffer: numBuffer, nOutBufferSize: numBufSize, lpBytesReturned: out bytesReturned,
                            lpOverlapped: IntPtr.Zero ) ) {
                            Console.WriteLine( value: "IOCTL failed." );
                        }
                    }
                    catch ( Exception ex ) {
                        Console.WriteLine( value: "Exception calling ioctl: " + ex );
                    }
                    finally {
                        hFile.DangerousGetHandle().CloseHandle();
                    }

                    if ( bytesReturned > 0 ) {
                        disknum = ( NativeMethods.STORAGE_DEVICE_NUMBER )Marshal.PtrToStructure( ptr: numBuffer,
                            structureType: typeof( NativeMethods.STORAGE_DEVICE_NUMBER ) );
                    }
                    else {
                        disknum = new NativeMethods.STORAGE_DEVICE_NUMBER {
                            DeviceNumber = -1,
                            DeviceType = -1,
                            PartitionNumber = -1
                        };
                    }

                    var device = this.CreateDevice( deviceClass: this, deviceInfoData: devData, path: devicePath, index: index, disknum: disknum.DeviceNumber );
                    devices.Add( item: device );

                    Marshal.FreeHGlobal( hglobal: hFile.DangerousGetHandle() );
                }
                else {
                    var device = this.CreateDevice( deviceClass: this, deviceInfoData: devData, path: devicePath, index: index );
                    devices.Add( item: device );
                }

                index++;
            }

            //devices.Sort();	//why?

            return devices;
        }
    }
}