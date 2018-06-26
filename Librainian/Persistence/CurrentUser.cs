// Copyright © 1995-2018 to Rick@AIBrain.org and Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
//
// This source code contained in "CurrentUser.cs" belongs to Rick@AIBrain.org and
// Protiguous@Protiguous.com unless otherwise specified or the original license has
// been overwritten by automatic formatting.
// (We try to avoid it from happening, but it does accidentally happen.)
//
// Any unmodified portions of source code gleaned from other projects still retain their original
// license and our thanks goes to those Authors. If you find your code in this source code, please
// let us know so we can properly attribute you and include the proper license and/or copyright.
//
// Donations, royalties from any software that uses any of our code, or license fees can be paid
// to us via bitcoin at the address 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2.
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
// Our software can be found at "https://Protiguous.Software/"
// Our GitHub address is "https://github.com/Protiguous".
// Feel free to browse any source code we might have available.
//
// ***  Project "Librainian"  ***
// File "CurrentUser.cs" was last formatted by Protiguous on 2018/06/06 at 9:16 PM.

namespace Librainian.Persistence {

	using System;
	using System.Diagnostics;
	using System.IO;
	using System.Linq;
	using JetBrains.Annotations;
	using Microsoft.Win32;

	public static class CurrentUser {

		public const Boolean Writeable = true;

		/// <summary>
		///     <para>The current executable's name under the <see cref="Protiguous" /> key.</para>
		///     <para>Use this one for setting and getting.</para>
		/// </summary>
		/// <value></value>
		[CanBeNull]
		internal static RegistryKey Application {
			get {
				var name = Path.GetFileName( Process.GetCurrentProcess().ProcessName );

				return Protiguous.GetSubKeyNames().Contains( name ) ? Protiguous.OpenSubKey( name, Writeable ) : Protiguous.CreateSubKey( name, Writeable );
			}
		}

		/// <summary>
		///     Current user. (Be careful with this one!)
		/// </summary>
		internal static RegistryKey HKCU => Registry.CurrentUser;

		/// <summary>
		///     The <see cref="Protiguous" /> key under the <see cref="Software" /> key.
		/// </summary>
		[CanBeNull]
		internal static RegistryKey Protiguous =>
			Software.GetSubKeyNames().Contains( nameof( Protiguous ) ) ? Software.OpenSubKey( nameof( Protiguous ), Writeable ) : Software.CreateSubKey( nameof( Protiguous ), Writeable );

		/// <summary>
		///     The <see cref="Software" /> key under the <see cref="HKCU" /> key.
		/// </summary>
		[CanBeNull]
		internal static RegistryKey Software => HKCU.GetSubKeyNames().Contains( nameof( Software ) ) ? HKCU.OpenSubKey( nameof( Software ), Writeable ) : HKCU.CreateSubKey( nameof( Software ), Writeable );

		/// <summary>
		/// By default, this retrieves the registry key under HKCU\Software\Protiguous\ProcessName.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="key"></param>
		/// <returns></returns>
		public static T Retrieve<T>( [NotNull] this String key ) {
			var subkey = Application;
			if ( subkey == null ) {
				throw new ArgumentNullException( paramName: nameof( subkey ) );
			}

			if ( String.IsNullOrWhiteSpace( value: key ) ) {
				throw new ArgumentException( message: "Value cannot be null or whitespace.", paramName: nameof( key ) );
			}

			//var kind = subkey.GetValueKind( key );
			var value = subkey.GetValue( key );

			if ( value is T ) {
				return ( T )subkey.GetValue( key );
			}

			return default;
		}

		/// <summary>
		/// By default, this stores the key under HKCU\Software\Protiguous\ProcessName.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="key"></param>
		/// <param name="value"></param>
		/// <returns></returns>
		public static Boolean Store<T>( [NotNull] this String key, T value ) {
			var subkey = Application;

			if ( subkey is null ) {
				throw new ArgumentNullException( paramName: nameof( subkey ) );
			}

			if ( String.IsNullOrWhiteSpace( value: key ) ) {
				throw new ArgumentException( message: "Value cannot be null or whitespace.", paramName: nameof( key ) );
			}

			try {
				switch ( value ) {
					case String[] _: {
							subkey.SetValue( key, value, RegistryValueKind.MultiString );

							return true;
						}
					case String _:
						subkey.SetValue( key, value, RegistryValueKind.String );

						return true;

					case UInt64 _:
					case Int64 _:
						subkey.SetValue( key, value, RegistryValueKind.QWord );

						return true;

					case UInt32 _:
					case Int32 _:
					case UInt16 _:
					case Int16 _:
					case SByte _:
					case Byte _:
						subkey.SetValue( key, value, RegistryValueKind.DWord );

						return true;

					case Byte[] _:
						subkey.SetValue( key, value, RegistryValueKind.Binary );

						return true;

					default: {
							subkey.SetValue( key, value, RegistryValueKind.Unknown );

							return true;
						}
				}
			}
			catch ( Exception exception ) {
				exception.More();
			}

			return false;
		}
	}
}