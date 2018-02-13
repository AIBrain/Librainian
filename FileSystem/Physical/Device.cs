namespace Librainian.FileSystem.Physical {

    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Text;
    using OperatingSystem;

    /// <summary>
    ///     A generic base class for physical devices.
    /// </summary>
    [TypeConverter( typeof( ExpandableObjectConverter ) )]
    public class Device : IComparable {
        private DeviceCapabilities _capabilities = DeviceCapabilities.Unknown;
        private String _class;
        private String _classGuid;
        private String _description;

        private String _friendlyName;
        private Device _parent;

        internal Device( DeviceClass deviceClass, NativeMethods.SP_DEVINFO_DATA deviceInfoData, String path, Int32 index, Int32 disknum = -1 ) {
	        this.DeviceClass = deviceClass ?? throw new ArgumentNullException( nameof( deviceClass ) );
            this.Path = path; // may be null
            this.DeviceInfoData = deviceInfoData ?? throw new ArgumentNullException( nameof( deviceInfoData ) );
            this.Index = index;
            this.DiskNumber = disknum;
        }

        /// <summary>
        ///     Gets the device's class instance.
        /// </summary>
        [Browsable( false )]
        public DeviceClass DeviceClass {
            get;
        }

        public Int32 DiskNumber {
            get;
        }

        /// <summary>
        ///     Gets the device's index.
        /// </summary>
        public Int32 Index {
            get;
        }

        /// <summary>
        ///     Gets the device's path.
        /// </summary>
        public String Path {
            get;
        }

        private NativeMethods.SP_DEVINFO_DATA DeviceInfoData {
            get;
        }

        /// <summary>
        ///     Compares the current instance with another object of the same type.
        /// </summary>
        /// <param name="obj">An object to compare with this instance.</param>
        /// <returns>A 32-bit signed integer that indicates the relative order of the comparands.</returns>
        public virtual Int32 CompareTo( Object obj ) {
	        if ( !( obj is Device device ) ) {
                throw new ArgumentException();
            }

            return this.Index.CompareTo( device.Index );
        }

        /// <summary>
        ///     Ejects the device.
        /// </summary>
        /// <param name="allowUI">Pass true to allow the Windows shell to display any related UI element, false otherwise.</param>
        /// <returns>null if no error occured, otherwise a contextual text.</returns>
        public String Eject( Boolean allowUI ) {
            foreach ( var device in this.GetRemovableDevices() ) {
                if ( allowUI ) {
                    NativeMethods.CM_Request_Device_Eject_NoUi( device.GetInstanceHandle(), IntPtr.Zero, null, 0, 0 );

                    // don't handle errors, there should be a UI for this
                }
                else {
                    var sb = new StringBuilder( 1024 );

					var hr = NativeMethods.CM_Request_Device_Eject( device.GetInstanceHandle(), out var veto, sb, sb.Capacity, 0 );
					if ( hr != 0 ) {
                        throw new Win32Exception( hr );
                    }

                    if ( veto != NativeMethods.PNP_VETO_TYPE.Ok ) {
                        return veto.ToString();
                    }
                }
            }
            return null;
        }

        /// <summary>
        ///     Gets the device's capabilities.
        /// </summary>
        public DeviceCapabilities GetCapabilities() {
            if ( this._capabilities == DeviceCapabilities.Unknown ) {
                this._capabilities = ( DeviceCapabilities )this.DeviceClass.GetProperty( this.DeviceInfoData, NativeMethods.SPDRP_CAPABILITIES, 0 );
            }
            return this._capabilities;
        }

        /// <summary>
        ///     Gets the device's class name.
        /// </summary>
        public String GetClass() => this._class ?? ( this._class = this.DeviceClass.GetProperty( this.DeviceInfoData, NativeMethods.SPDRP_CLASS, null ) );

        /// <summary>
        ///     Gets the device's class Guid as a string.
        /// </summary>
        public String GetClassGuid() => this._classGuid ?? ( this._classGuid = this.DeviceClass.GetProperty( this.DeviceInfoData, NativeMethods.SPDRP_CLASSGUID, null ) );

	    /// <summary>
        ///     Gets the device's description.
        /// </summary>
        public String GetDescription() => this._description ?? ( this._description = this.DeviceClass.GetProperty( this.DeviceInfoData, NativeMethods.SPDRP_DEVICEDESC, null ) );

	    /// <summary>
        ///     Gets the device's friendly name.
        /// </summary>
        public String GetFriendlyName() => this._friendlyName ?? ( this._friendlyName = this.DeviceClass.GetProperty( this.DeviceInfoData, NativeMethods.SPDRP_FRIENDLYNAME, null ) );

        /// <summary>
        ///     Gets the device's instance handle.
        /// </summary>
        public UInt32 GetInstanceHandle() => this.DeviceInfoData.devInst;

        /// <summary>
        ///     Gets the device's parent device or null if this device has not parent.
        /// </summary>
        public Device GetParent() {
            if ( this._parent != null ) {
                return this._parent;
            }
            var parentDevInst = 0;
            var hr = NativeMethods.CM_Get_Parent( ref parentDevInst, this.DeviceInfoData.devInst, 0 );
            if ( hr == 0 ) {
                this._parent = new Device( this.DeviceClass, this.DeviceClass.GetInfo( parentDevInst ), null, -1 );
            }
            return this._parent;
        }

        /// <summary>
        ///     Gets this device's list of removable devices.
        ///     Removable devices are parent devices that can be removed.
        /// </summary>
        public virtual IEnumerable<Device> GetRemovableDevices() {
            if ( ( this.GetCapabilities() & DeviceCapabilities.Removable ) != 0 ) {
                yield return this;
            }
            else {
                if ( this.GetParent() == null ) {
                    yield break;
                }
                foreach ( var device in this.GetParent().GetRemovableDevices() ) {
                    yield return device;
                }
            }
        }

        /// <summary>
        ///     Gets a value indicating whether this device is a USB device.
        /// </summary>
        public virtual Boolean IsUsb() {
            if ( this.GetClass() == "USB" ) {
                return true;
            }

            return this.GetParent() != null && this.GetParent().IsUsb();
        }
    }

}