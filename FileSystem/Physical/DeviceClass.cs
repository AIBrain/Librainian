namespace Librainian.FileSystem.Physical {

    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.IO;
    using System.Runtime.InteropServices;
    using System.Text;
    using Magic;
    using OperatingSystem;

    /// <summary>
    ///     A generic base class for physical device classes.
    /// </summary>
    public abstract class DeviceClass : ABetterClassDispose {

        private Guid _classGuid;

        private IntPtr _deviceInfoSet;

        protected DeviceClass( Guid classGuid ) : this( classGuid, IntPtr.Zero ) { }

        /// <summary>
        ///     Initializes a new instance of the DeviceClass class.
        /// </summary>
        /// <param name="classGuid">A device class Guid.</param>
        /// <param name="hwndParent">
        ///     The handle of the top-level window to be used for any user interface or IntPtr.Zero for no
        ///     handle.
        /// </param>
        private DeviceClass( Guid classGuid, IntPtr hwndParent ) {
            this._classGuid = classGuid;

            this._deviceInfoSet = NativeMethods.SetupDiGetClassDevs( ref this._classGuid, "" /*was 0*/, hwndParent, NativeMethods.DIGCF_DEVICEINTERFACE | NativeMethods.DIGCF_PRESENT );

            var lastError = Marshal.GetLastWin32Error();
            if ( this._deviceInfoSet == ( IntPtr )( -1 ) ) {
                throw new Win32Exception( lastError );
            }
        }

        /// <summary>
        ///     Gets the device class's guid.
        /// </summary>
        public Guid ClassGuid => this._classGuid;

        /// <summary>
        /// Dispose of COM objects, etc...
        /// </summary>
        protected override void DisposeNative() {
            if ( this._deviceInfoSet != IntPtr.Zero ) {
                NativeMethods.SetupDiDestroyDeviceInfoList( this._deviceInfoSet );
                this._deviceInfoSet = IntPtr.Zero;
            }
            base.DisposeNative();
        }

        /// <summary>
        /// Dispose any disposable members.
        /// </summary>
        protected override void DisposeManaged() {
        }

        /// <summary>
        ///     Gets the list of devices of this device class.
        /// </summary>
        /// <returns>
        ///     The devices.
        /// </returns>
        /// <exception cref="System.ComponentModel.Win32Exception">
        /// </exception>
        public IEnumerable<Device> GetDevices() {
            var devices = new List<Device>();
            var index = 0;
            while ( true ) {
                var interfaceData = new NativeMethods.SP_DEVICE_INTERFACE_DATA();
                interfaceData.cbSize = ( UInt32 )Marshal.SizeOf( interfaceData );
                if ( !NativeMethods.SetupDiEnumDeviceInterfaces( this._deviceInfoSet, null, ref this._classGuid, index, interfaceData ) ) {
                    var error = Marshal.GetLastWin32Error();
                    if ( error != NativeMethods.ERROR_NO_MORE_ITEMS ) {
                        throw new Win32Exception( error );
                    }

                    break;
                }

                var devData = new NativeMethods.SP_DEVINFO_DATA();
                devData.cbSize = ( UInt32 )Marshal.SizeOf( devData );
                var size = 0;
                if ( !NativeMethods.SetupDiGetDeviceInterfaceDetail( this._deviceInfoSet, interfaceData, IntPtr.Zero, 0, ref size, devData ) ) {
                    var error = Marshal.GetLastWin32Error();
                    if ( error != NativeMethods.ERROR_INSUFFICIENT_BUFFER ) {
                        throw new Win32Exception( error );
                    }
                }

                var buffer = Marshal.AllocHGlobal( size );
                var detailData = new NativeMethods.SP_DEVICE_INTERFACE_DETAIL_DATA();
                detailData.cbSize = Marshal.SizeOf( detailData );

                Marshal.WriteInt32( buffer, IntPtr.Size );

                //Marshal.StructureToPtr(detailData, buffer, false);

                if ( !NativeMethods.SetupDiGetDeviceInterfaceDetail( this._deviceInfoSet, interfaceData, buffer, size, ref size, devData ) ) {
                    try {
                        throw new Win32Exception( Marshal.GetLastWin32Error() );
                    }
                    finally {
                        Marshal.FreeHGlobal( buffer );
                    }


                }

                var strPtr = new IntPtr( buffer.ToInt64() + 4 );
                var devicePath = Marshal.PtrToStringAuto( strPtr );

                //IntPtr pDevicePath = (IntPtr)((int)buffer + Marshal.SizeOf(typeof(int)));
                //string devicePath = Marshal.PtrToStringAuto(pDevicePath);
                Marshal.FreeHGlobal( buffer );

                if ( this._classGuid.Equals( new Guid( NativeMethods.GUID_DEVINTERFACE_DISK ) ) ) {
                    // Find disks
                    var hFile = NativeMethods.CreateFile( devicePath, 0, FileShare.ReadWrite, IntPtr.Zero, FileMode.Open, 0, IntPtr.Zero );
                    if ( hFile.IsInvalid ) {
                        throw new Win32Exception( Marshal.GetLastWin32Error() );
                    }

                    UInt32 bytesReturned = 0;
                    UInt32 numBufSize = 0x400; // some big size
                    var numBuffer = Marshal.AllocHGlobal( ( IntPtr )numBufSize );
                    NativeMethods.STORAGE_DEVICE_NUMBER disknum;

                    try {
                        if ( !NativeMethods.DeviceIoControl( hFile.DangerousGetHandle(), NativeMethods.IOCTL_STORAGE_GET_DEVICE_NUMBER, IntPtr.Zero, 0, numBuffer, numBufSize, out bytesReturned, IntPtr.Zero ) ) {
                            Console.WriteLine( "IOCTL failed." );
                        }
                    }
                    catch ( Exception ex ) {
                        Console.WriteLine( "Exception calling ioctl: " + ex );
                    }
                    finally {
                        NativeMethods.CloseHandle( hFile.DangerousGetHandle() );
                    }

                    if ( bytesReturned > 0 ) {
                        disknum = ( NativeMethods.STORAGE_DEVICE_NUMBER )Marshal.PtrToStructure( numBuffer, typeof( NativeMethods.STORAGE_DEVICE_NUMBER ) );
                    }
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

            devices.Sort();

            return devices;
        }

        internal NativeMethods.SP_DEVINFO_DATA GetInfo( Int32 dnDevInst ) {
            var sb = new StringBuilder( 1024 );
            var hr = NativeMethods.CM_Get_Device_ID( dnDevInst, sb, sb.Capacity, 0 );
            if ( hr != 0 ) {
                throw new Win32Exception( hr );
            }

            var devData = new NativeMethods.SP_DEVINFO_DATA();
            devData.cbSize = ( UInt32 )Marshal.SizeOf( devData );
            if ( !NativeMethods.SetupDiOpenDeviceInfo( this._deviceInfoSet, sb.ToString(), IntPtr.Zero, 0, devData ) ) {
                throw new Win32Exception( Marshal.GetLastWin32Error() );
            }

            return devData;
        }

        internal String GetProperty( NativeMethods.SP_DEVINFO_DATA devData, UInt32 property, String defaultValue ) {
            if ( devData == null ) {
                throw new ArgumentNullException( nameof( devData ) );
            }
			const Int32 propertyBufferSize = 1024;

			var propertyBuffer = new Byte[ propertyBufferSize ];
            if ( !NativeMethods.SetupDiGetDeviceRegistryProperty( this._deviceInfoSet, ref devData, property, out var propertyRegDataType, propertyBuffer, propertyBufferSize, out var requiredSize ) ) {
                //Marshal.FreeHGlobal( propertyBuffer );
                var error = Marshal.GetLastWin32Error();
                if ( error != NativeMethods.ERROR_INVALID_DATA ) {
                    throw new Win32Exception( error );
                }

                return defaultValue;
            }

            //var value = Marshal.PtrToStringAuto( propertyBuffer );
            //Marshal.FreeHGlobal( propertyBuffer );
            return Encoding.Default.GetString( propertyBuffer );
        }

        internal UInt32 GetProperty( NativeMethods.SP_DEVINFO_DATA devData, UInt32 property, UInt32 defaultValue ) {
            if ( devData == null ) {
                throw new ArgumentNullException( nameof( devData ) );
            }
			var propertyBufferSize = ( UInt32 )Marshal.SizeOf( typeof( UInt32 ) );

			//var propertyBuffer = Marshal.AllocHGlobal( propertyBufferSize );
			var propertyBuffer = new Byte[ propertyBufferSize ];
            if ( !NativeMethods.SetupDiGetDeviceRegistryProperty( this._deviceInfoSet, ref devData, property, out var propertyRegDataType, propertyBuffer, propertyBufferSize, out var requiredSize ) ) {
                //Marshal.FreeHGlobal( propertyBuffer );
                var error = Marshal.GetLastWin32Error();
                if ( error != NativeMethods.ERROR_INVALID_DATA ) {
                    throw new Win32Exception( error );
                }

                return defaultValue;
            }

            //var value = ( Int32 )Marshal.PtrToStructure( propertyBuffer, typeof( Int32 ) );
            //Marshal.FreeHGlobal( propertyBuffer );
            //return value;
            return Convert.ToUInt32( propertyBuffer );
        }

        internal Guid GetProperty( NativeMethods.SP_DEVINFO_DATA devData, UInt32 property, Guid defaultValue ) {
            if ( devData == null ) {
                throw new ArgumentNullException( nameof( devData ) );
            }
			var propertyBufferSize = ( UInt32 )Marshal.SizeOf( typeof( Guid ) );

			var propertyBuffer = new Byte[ propertyBufferSize ];
            if ( !NativeMethods.SetupDiGetDeviceRegistryProperty( this._deviceInfoSet, ref devData, property, out var propertyRegDataType, propertyBuffer, propertyBufferSize, out var requiredSize ) ) {
                //Marshal.FreeHGlobal( propertyBuffer );
                var error = Marshal.GetLastWin32Error();
                if ( error != NativeMethods.ERROR_INVALID_DATA ) {
                    throw new Win32Exception( error );
                }

                return defaultValue;
            }

            return new Guid( propertyBuffer );

            //var value = ( Guid )Marshal.PtrToStructure( propertyBuffer, typeof( Guid ) );
            //Marshal.FreeHGlobal( propertyBuffer );
            //return value;
        }

        protected virtual Device CreateDevice( DeviceClass deviceClass, NativeMethods.SP_DEVINFO_DATA deviceInfoData, String path, Int32 index, Int32 disknum = -1 ) => new Device( deviceClass, deviceInfoData, path, index, disknum );

    }

}