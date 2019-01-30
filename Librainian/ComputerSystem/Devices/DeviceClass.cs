// Copyright © Rick@AIBrain.org and Protiguous. All Rights Reserved.
//
// this entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
//
// this source code contained in "DeviceClass.cs" belongs to Protiguous@Protiguous.com and
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
//     paypal@AIBrain.Org
//     (We're still looking into other solutions! Any ideas?)
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
// Feel free to browse any source code we *might* make available.
//
// Project: "Librainian", "DeviceClass.cs" was last formatted by Protiguous on 2018/07/10 at 8:53 PM.

namespace Librainian.ComputerSystem.Devices {

    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.IO;
    using System.Runtime.InteropServices;
    using System.Text;
    using JetBrains.Annotations;
    using Magic;
    using OperatingSystem;

    /// <summary>
    ///     A generic base class for physical device classes.
    /// </summary>
    public abstract class DeviceClass : ABetterClassDispose {

        private Guid _classGuid;

        private IntPtr _deviceInfoSet;

        /// <summary>
        ///     Gets the device class's guid.
        /// </summary>
        public Guid ClassGuid => this._classGuid;

        /// <summary>
        ///     Initializes a new instance of the DeviceClass class.
        /// </summary>
        /// <param name="classGuid"> A device class Guid.</param>
        /// <param name="hwndParent">
        ///     The handle of the top-level window to be used for any user interface or IntPtr.Zero for no
        ///     handle.
        /// </param>
        private DeviceClass( Guid classGuid, IntPtr hwndParent ) {
            this._classGuid = classGuid;

            this._deviceInfoSet = NativeMethods.SetupDiGetClassDevs( ref this._classGuid, "" /*was 0*/, hwndParent, NativeMethods.DIGCF_DEVICEINTERFACE | NativeMethods.DIGCF_PRESENT );

            var lastError = Marshal.GetLastWin32Error();

            if ( this._deviceInfoSet == ( IntPtr )( -1 ) ) { throw new Win32Exception( lastError ); }
        }

        protected DeviceClass( Guid classGuid ) : this( classGuid, IntPtr.Zero ) { }

        [NotNull]
        protected virtual Device CreateDevice( [NotNull] DeviceClass deviceClass, NativeMethods.SP_DEVINFO_DATA deviceInfoData, String path, Int32 index, Int32 disknum = -1 ) =>
            new Device( deviceClass, deviceInfoData, path, index, disknum );

        internal NativeMethods.SP_DEVINFO_DATA GetInfo( Int32 dnDevInst ) {
            var sb = new StringBuilder( 1024 );
            var hr = NativeMethods.CM_Get_Device_ID( dnDevInst, sb, sb.Capacity, 0 );

            if ( hr != 0 ) { throw new Win32Exception( hr ); }

            var devData = new NativeMethods.SP_DEVINFO_DATA();
            devData.cbSize = ( UInt32 )Marshal.SizeOf( devData );

            if ( !NativeMethods.SetupDiOpenDeviceInfo( this._deviceInfoSet, sb.ToString(), IntPtr.Zero, 0, devData ) ) { throw new Win32Exception( Marshal.GetLastWin32Error() ); }

            return devData;
        }

        internal String GetProperty( NativeMethods.SP_DEVINFO_DATA devData, UInt32 property, String defaultValue ) {

            const Int32 propertyBufferSize = 1024;

            var propertyBuffer = new Byte[ propertyBufferSize ];

            if ( !NativeMethods.SetupDiGetDeviceRegistryProperty( this._deviceInfoSet, ref devData, property, out var propertyRegDataType, propertyBuffer, propertyBufferSize, out var requiredSize ) ) {

                //Marshal.FreeHGlobal( propertyBuffer );
                var error = Marshal.GetLastWin32Error();

                if ( error != NativeMethods.ERROR_INVALID_DATA ) { throw new Win32Exception( error ); }

                return defaultValue;
            }

            //var value = Marshal.PtrToStringAuto( propertyBuffer );
            //Marshal.FreeHGlobal( propertyBuffer );
            return Encoding.Default.GetString( propertyBuffer );
        }

        internal UInt32 GetProperty( NativeMethods.SP_DEVINFO_DATA devData, UInt32 property, UInt32 defaultValue ) {

            var propertyBufferSize = ( UInt32 )Marshal.SizeOf( typeof( UInt32 ) );

            //var propertyBuffer = Marshal.AllocHGlobal( propertyBufferSize );
            var propertyBuffer = new Byte[ propertyBufferSize ];

            if ( !NativeMethods.SetupDiGetDeviceRegistryProperty( this._deviceInfoSet, ref devData, property, out var propertyRegDataType, propertyBuffer, propertyBufferSize, out var requiredSize ) ) {

                //Marshal.FreeHGlobal( propertyBuffer );
                var error = Marshal.GetLastWin32Error();

                if ( error != NativeMethods.ERROR_INVALID_DATA ) { throw new Win32Exception( error ); }

                return defaultValue;
            }

            //var value = ( Int32 )Marshal.PtrToStructure( propertyBuffer, typeof( Int32 ) );
            //Marshal.FreeHGlobal( propertyBuffer );
            //return value;
            return Convert.ToUInt32( propertyBuffer );
        }

        internal Guid GetProperty( NativeMethods.SP_DEVINFO_DATA devData, UInt32 property, Guid defaultValue ) {

            var propertyBufferSize = ( UInt32 )Marshal.SizeOf( typeof( Guid ) );

            var propertyBuffer = new Byte[ propertyBufferSize ];

            if ( !NativeMethods.SetupDiGetDeviceRegistryProperty( this._deviceInfoSet, ref devData, property, out var propertyRegDataType, propertyBuffer, propertyBufferSize, out var requiredSize ) ) {

                //Marshal.FreeHGlobal( propertyBuffer );
                var error = Marshal.GetLastWin32Error();

                if ( error != NativeMethods.ERROR_INVALID_DATA ) { throw new Win32Exception( error ); }

                return defaultValue;
            }

            return new Guid( propertyBuffer );

            //var value = ( Guid )Marshal.PtrToStructure( propertyBuffer, typeof( Guid ) );
            //Marshal.FreeHGlobal( propertyBuffer );
            //return value;
        }

        /// <summary>
        ///     Dispose any disposable members.
        /// </summary>
        public override void DisposeManaged() { }

        /// <summary>
        ///     Dispose of COM objects, etc...
        /// </summary>
        public override void DisposeNative() {
            if ( this._deviceInfoSet != IntPtr.Zero ) {
                NativeMethods.SetupDiDestroyDeviceInfoList( this._deviceInfoSet );
                this._deviceInfoSet = IntPtr.Zero;
            }

            base.DisposeNative();
        }

        /// <summary>
        ///     Gets the list of devices of this device class.
        /// </summary>
        /// <returns>The devices.</returns>
        /// <exception cref="System.ComponentModel.Win32Exception"></exception>
        [NotNull]
        public IEnumerable<Device> GetDevices() {
            var devices = new HashSet<Device>();
            var index = 0;

            while ( true ) {
                var interfaceData = new NativeMethods.SP_DEVICE_INTERFACE_DATA();
                interfaceData.cbSize = ( UInt32 )Marshal.SizeOf( interfaceData );

                if ( !NativeMethods.SetupDiEnumDeviceInterfaces( this._deviceInfoSet, default, ref this._classGuid, index, interfaceData ) ) {
                    var error = Marshal.GetLastWin32Error();

                    if ( error != NativeMethods.ERROR_NO_MORE_ITEMS ) { throw new Win32Exception( error ); }

                    break;
                }

                var devData = new NativeMethods.SP_DEVINFO_DATA();
                devData.cbSize = ( UInt32 )Marshal.SizeOf( devData );
                var size = 0;

                if ( !NativeMethods.SetupDiGetDeviceInterfaceDetail( this._deviceInfoSet, interfaceData, IntPtr.Zero, 0, ref size, devData ) ) {
                    var error = Marshal.GetLastWin32Error();

                    if ( error != NativeMethods.ERROR_INSUFFICIENT_BUFFER ) { throw new Win32Exception( error ); }
                }

                var buffer = Marshal.AllocHGlobal( size );
                var detailData = new NativeMethods.SP_DEVICE_INTERFACE_DETAIL_DATA();
                detailData.cbSize = Marshal.SizeOf( detailData );

                Marshal.WriteInt32( buffer, IntPtr.Size );

                //Marshal.StructureToPtr(detailData, buffer, false);

                if ( !NativeMethods.SetupDiGetDeviceInterfaceDetail( this._deviceInfoSet, interfaceData, buffer, size, ref size, devData ) ) {
                    try { throw new Win32Exception( Marshal.GetLastWin32Error() ); }
                    finally { Marshal.FreeHGlobal( buffer ); }
                }

                var strPtr = new IntPtr( buffer.ToInt64() + 4 );
                var devicePath = Marshal.PtrToStringAuto( strPtr );

                //IntPtr pDevicePath = (IntPtr)((int)buffer + Marshal.SizeOf(typeof(int)));
                //string devicePath = Marshal.PtrToStringAuto(pDevicePath);
                Marshal.FreeHGlobal( buffer );

                if ( this._classGuid.Equals( new Guid( NativeMethods.GUID_DEVINTERFACE_DISK ) ) ) {

                    // Find disks
                    var hFile = NativeMethods.CreateFile( devicePath, 0, FileShare.ReadWrite, IntPtr.Zero, FileMode.Open, 0, IntPtr.Zero );

                    if ( hFile.IsInvalid ) { throw new Win32Exception( Marshal.GetLastWin32Error() ); }

                    UInt32 bytesReturned = 0;
                    const UInt32 numBufSize = 0x1000; // some big size
                    var numBuffer = Marshal.AllocHGlobal( ( IntPtr )numBufSize );
                    NativeMethods.STORAGE_DEVICE_NUMBER disknum;

                    try {
                        if ( !NativeMethods.DeviceIoControl( hFile.DangerousGetHandle(), NativeMethods.IOCTL_STORAGE_GET_DEVICE_NUMBER, IntPtr.Zero, 0, numBuffer, numBufSize, out bytesReturned, IntPtr.Zero ) ) {
                            Console.WriteLine( "IOCTL failed." );
                        }
                    }
                    catch ( Exception ex ) { Console.WriteLine( "Exception calling ioctl: " + ex ); }
                    finally { hFile.DangerousGetHandle().CloseHandle(); }

                    if ( bytesReturned > 0 ) { disknum = ( NativeMethods.STORAGE_DEVICE_NUMBER )Marshal.PtrToStructure( numBuffer, typeof( NativeMethods.STORAGE_DEVICE_NUMBER ) ); }
                    else {
                        disknum = new NativeMethods.STORAGE_DEVICE_NUMBER {
                            DeviceNumber = -1,
                            DeviceType = -1,
                            PartitionNumber = -1
                        };
                    }

                    var device = this.CreateDevice( this, devData, devicePath, index, disknum.DeviceNumber );
                    devices.Add( device );

                    Marshal.FreeHGlobal( hFile.DangerousGetHandle() );
                }
                else {
                    var device = this.CreateDevice( this, devData, devicePath, index );
                    devices.Add( device );
                }

                index++;
            }

            //devices.Sort();	//why?

            return devices;
        }
    }
}