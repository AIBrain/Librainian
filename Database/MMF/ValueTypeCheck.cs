// Copyright © 1995-2018 to Rick@AIBrain.org and Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
//
// This source code contained in "ValueTypeCheck.cs" belongs to Rick@AIBrain.org and
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
// Usage of the source code or binaries is AS-IS.
// No warranties are expressed, implied, or given.
// We are NOT responsible for Anything You Do With Our Code.
// =========================================================
//
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
// For business inquiries, please contact me at Protiguous@Protiguous.com
//
// "Librainian/Librainian/ValueTypeCheck.cs" was last formatted by Protiguous on 2018/05/24 at 7:06 PM.

namespace Librainian.Database.MMF {

    using System;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.InteropServices;

    /// <summary>
    ///     Check if a Type is a value type
    /// </summary>
    internal class ValueTypeCheck {

        private Type Type { get; }

        public ValueTypeCheck( Type objectType ) => this.Type = objectType;

        private static Boolean HasMarshalDefinedSize( MemberInfo info ) {
            var customAttributes = info.GetCustomAttributes( typeof( MarshalAsAttribute ), true );

            if ( customAttributes.Length == 0 ) { return false; }

            var attribute = ( MarshalAsAttribute )customAttributes[0];

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