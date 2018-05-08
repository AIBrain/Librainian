// Copyright 2018 Protiguous.
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
// "Librainian/ValueTypeCheck.cs" was last cleaned by Protiguous on 2016/06/18 at 10:50 PM

namespace Librainian.Database.MMF {

    using System;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.InteropServices;

    /// <summary>Check if a Type is a value type</summary>
    internal class ValueTypeCheck {
        private readonly Type _type;

        public ValueTypeCheck( Type objectType ) => this._type = objectType;

	    internal Boolean OnlyValueTypes() => this._type.IsPrimitive || this.PropertySizesAreDefined() && this.FieldSizesAreDefined();

        private static Boolean HasMarshalDefinedSize( MemberInfo info ) {
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