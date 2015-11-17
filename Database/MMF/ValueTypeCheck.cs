// Copyright 2015 Rick@AIBrain.org.
// 
// This notice must be kept visible in the source.
// 
// This section of source code belongs to Rick@AIBrain.Org unless otherwise specified, or the
// original license has been overwritten by the automatic formatting of this code. Any unmodified
// sections of source code borrowed from other projects retain their original license and thanks
// goes to the Authors.
// 
// Donations and Royalties can be paid via
// PayPal: paypal@aibrain.org
// bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
// litecoin: LeUxdU2w3o6pLZGVys5xpDZvvo8DUrjBp9
// 
// Usage of the source code or compiled binaries is AS-IS. I am not responsible for Anything You Do.
// 
// Contact me by email if you have any questions or helpful criticism.
// 
// "Librainian/ValueTypeCheck.cs" was last cleaned by Rick on 2015/06/12 at 2:53 PM

namespace Librainian.Database.MMF {

    using System;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.InteropServices;

    /// <summary>Check if a Type is a value type</summary>
    internal class ValueTypeCheck {
        private readonly Type _type;

        public ValueTypeCheck(Type objectType) {
            this._type = objectType;
        }

        internal Boolean OnlyValueTypes() => this._type.IsPrimitive || ( this.PropertySizesAreDefined() && this.FieldSizesAreDefined() );

        private static Boolean HasMarshalDefinedSize(MemberInfo info) {
            var customAttributes = info.GetCustomAttributes( typeof( MarshalAsAttribute ), true );
            if ( customAttributes.Length == 0 ) {
                return false;
            }
            var attribute = ( MarshalAsAttribute )customAttributes[ 0 ];
            if ( attribute.Value == UnmanagedType.Currency ) {
                return true;
            }
            return attribute.SizeConst > 0;
        }

        private Boolean FieldSizesAreDefined() => this._type.GetFields( BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic ).Where( fieldInfo => !fieldInfo.FieldType.IsPrimitive ).All( HasMarshalDefinedSize );

        private Boolean PropertySizesAreDefined() {
            foreach ( var propertyInfo in
                this._type.GetProperties( BindingFlags.Instance | BindingFlags.FlattenHierarchy | BindingFlags.Public | BindingFlags.NonPublic ) ) {
                if ( !propertyInfo.CanRead || !propertyInfo.CanWrite ) {
                    return false;
                }
                if ( !propertyInfo.PropertyType.IsPrimitive && !HasMarshalDefinedSize( propertyInfo ) ) {
                    return false;
                }
            }
            return true;
        }
    }
}