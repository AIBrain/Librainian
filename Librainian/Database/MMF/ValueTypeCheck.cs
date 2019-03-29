﻿// Copyright © Rick@AIBrain.org and Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
//
// This source code contained in "ValueTypeCheck.cs" belongs to Protiguous@Protiguous.com and
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
// Project: "Librainian", "ValueTypeCheck.cs" was last formatted by Protiguous on 2018/07/10 at 8:59 PM.

namespace Librainian.Database.MMF {

	using System;
	using System.Linq;
	using System.Reflection;
	using System.Runtime.InteropServices;
	using JetBrains.Annotations;

	/// <summary>
	///     Check if a Type is a value type
	/// </summary>
	internal class ValueTypeCheck {

		private Type Type { get; }

		public ValueTypeCheck( Type objectType ) => this.Type = objectType;

		private static Boolean HasMarshalDefinedSize( [NotNull] MemberInfo info ) {
			var customAttributes = info.GetCustomAttributes( typeof( MarshalAsAttribute ), true );

			if ( customAttributes.Length == 0 ) { return false; }

			var attribute = ( MarshalAsAttribute ) customAttributes[ 0 ];

			if ( attribute.Value == UnmanagedType.Currency ) { return true; }

			return attribute.SizeConst > 0;
		}

		private Boolean FieldSizesAreDefined() =>
			this.Type.GetFields( BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic ).Where( fieldInfo => !fieldInfo.FieldType.IsPrimitive ).All( HasMarshalDefinedSize );

		private Boolean PropertySizesAreDefined() {
			foreach ( var propertyInfo in this.Type.GetProperties( BindingFlags.Instance | BindingFlags.FlattenHierarchy | BindingFlags.Public | BindingFlags.NonPublic ) ) {
				if ( !propertyInfo.CanRead || !propertyInfo.CanWrite ) { return false; }

				if ( !propertyInfo.PropertyType.IsPrimitive && !HasMarshalDefinedSize( propertyInfo ) ) { return false; }
			}

			return true;
		}

		internal Boolean OnlyValueTypes() => this.Type.IsPrimitive || this.PropertySizesAreDefined() && this.FieldSizesAreDefined();
	}
}