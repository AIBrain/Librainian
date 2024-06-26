// Copyright � Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible in any binaries, libraries, repositories,
// or source code (directly or derived) from our binaries, libraries, projects, solutions, or applications.
//
// All source code belongs to Protiguous@Protiguous.com unless otherwise specified or the original license has been overwritten
// by formatting. (We try to avoid it from happening, but it does accidentally happen.)
//
// Any unmodified portions of source code gleaned from other sources still retain their original license and our thanks goes to
// those Authors. If you find your code unattributed in this source code, please let us know so we can properly attribute you
// and include the proper license and/or copyright(s). If you want to use any of our code in a commercial project, you must
// contact Protiguous@Protiguous.com for permission, license, and a quote.
//
// Donations, payments, and royalties are accepted via bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2 and PayPal: Protiguous@Protiguous.com
//
// ====================================================================
// Disclaimer:  Usage of the source code or binaries is AS-IS. No warranties are expressed, implied, or given. We are NOT
// responsible for Anything You Do With Our Code. We are NOT responsible for Anything You Do With Our Executables. We are NOT
// responsible for Anything You Do With Your Computer. ====================================================================
//
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
// For business inquiries, please contact me at Protiguous@Protiguous.com. Our software can be found at
// "https://Protiguous.com/Software/" Our GitHub address is "https://github.com/Protiguous".
//
// File "Device.cs" last formatted on 2021-11-30 at 7:16 PM by Protiguous.

#nullable enable

namespace Librainian.ComputerSystem.Devices;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using Exceptions;
using OperatingSystem;

/// <summary>A generic base class for physical devices.</summary>
[TypeConverter( typeof( ExpandableObjectConverter ) )]
public class Device /*: IComparable<Device>*/ {

	private DeviceCapabilities _capabilities = DeviceCapabilities.Unknown;

	private String? _class;

	private String? _classGuid;

	private String? _description;

	private String? _friendlyName;

	private Boolean? _isUsb;

	private Device? _parent;

	public Device( DeviceClass deviceClass, NativeMethods.SP_DEVINFO_DATA? deviceInfoData, String? path, Int32 index, Int32? diskNumber = null ) {
		this.DeviceClass = deviceClass ?? throw new NullException( nameof( deviceClass ) );
		this.Path = path; // may be null
		this.DeviceInfoData = deviceInfoData ?? throw new NullException( nameof( deviceInfoData ) );
		this.Index = index;
		this.DiskNumber = diskNumber;
	}

	private NativeMethods.SP_DEVINFO_DATA DeviceInfoData { get; }

	/// <summary>Gets the device's class instance.</summary>
	[Browsable( false )]
	public DeviceClass DeviceClass { get; }

	public Int32? DiskNumber { get; }

	/// <summary>Gets the device's index.</summary>
	public Int32 Index { get; }

	/// <summary>Gets the device's path.</summary>
	public String? Path { get; }

	/// <summary>Compares the current instance with another object of the same type.</summary>
	/// <param name="obj">An object to compare with this instance.</param>
	/// <returns>A 32-bit signed integer that indicates the relative order of the comparands.</returns>
	public virtual Int32 CompareTo( Object obj ) {
		if ( obj is not Device device ) {
			throw new NullException( nameof( device ) );
		}

		return this.Index.CompareTo( device.Index );
	}

	/// <summary>Ejects the device.</summary>
	/// <param name="allowUI">Pass true to allow the Windows shell to display any related UI element, false otherwise.</param>
	/// <returns>null if no error occured, otherwise a contextual text.</returns>
	public String? Eject( Boolean allowUI ) {
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

		return default;
	}

	/// <summary>Gets the device's capabilities.</summary>
	public DeviceCapabilities GetCapabilities() {
		if ( this._capabilities == DeviceCapabilities.Unknown ) {
			this._capabilities = ( DeviceCapabilities )this.DeviceClass.GetProperty( this.DeviceInfoData, NativeMethods.SPDRP_CAPABILITIES, 0 );
		}

		return this._capabilities;
	}

	/// <summary>Gets the device's class name.</summary>
	public String GetClass() =>
		( this._class ??= this.DeviceClass.GetProperty( this.DeviceInfoData, NativeMethods.SPDRP_CLASS, null ) ) ?? throw new InvalidOperationException();

	/// <summary>Gets the device's class Guid as a string.</summary>
	public String GetClassGuid() =>
		( this._classGuid ??= this.DeviceClass.GetProperty( this.DeviceInfoData, NativeMethods.SPDRP_CLASSGUID, null ) ) ?? throw new InvalidOperationException();

	/// <summary>Gets the device's description.</summary>
	public String GetDescription() =>
		( this._description ??= this.DeviceClass.GetProperty( this.DeviceInfoData, NativeMethods.SPDRP_DEVICEDESC, null ) ) ?? throw new InvalidOperationException();

	/// <summary>Gets the device's friendly name.</summary>
	public String GetFriendlyName() =>
		( this._friendlyName ??= this.DeviceClass.GetProperty( this.DeviceInfoData, NativeMethods.SPDRP_FRIENDLYNAME, null ) ) ?? throw new InvalidOperationException();

	/// <summary>Gets the device's instance handle.</summary>
	public UInt32 GetInstanceHandle() => this.DeviceInfoData.devInst;

	/// <summary>Gets this device's list of removable devices. Removable devices are parent devices that can be removed.</summary>
	public virtual IEnumerable<Device> GetRemovableDevices() {
		if ( ( this.GetCapabilities() & DeviceCapabilities.Removable ) != 0 ) {
			yield return this;
		}
		else {
			var parent = this.Parent();

			if ( parent is null ) {
				yield break;
			}

			foreach ( var device in parent.GetRemovableDevices() ) {
				yield return device;
			}
		}
	}

	/// <summary>Gets a value indicating whether this device is a USB device.</summary>
	public virtual Boolean IsUsb() {
		this._isUsb ??= this.GetClass().ToUpper().Contains( "USB" );

		if ( this._isUsb != true ) {
			this._isUsb = this.Parent()?.IsUsb();
		}

		return this._isUsb == true;
	}

	/// <summary>Gets the device's parent device or null if this device has not parent.</summary>
	public Device? Parent() {
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
}