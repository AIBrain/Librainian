#region License & Information

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
// bitcoin: 1NzEsF7eegeEWDr5Vr9sSSgtUC4aL6axJu
// litecoin: LeUxdU2w3o6pLZGVys5xpDZvvo8DUrjBp9
//
// Usage of the source code or compiled binaries is AS-IS. I am not responsible for Anything You Do.
//
// "Librainian2/ValueTypeCheck.cs" was last cleaned by Rick on 2014/08/08 at 2:26 PM

#endregion License & Information

namespace Librainian.Database.MMF {

    using System;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.InteropServices;

    /// <summary>
    /// Check if a Type is a value type
    /// </summary>
    internal class ValueTypeCheck {
        private readonly Type _type;

        public ValueTypeCheck( Type objectType ) {
            this._type = objectType;
        }

        internal Boolean OnlyValueTypes() {
            return this._type.IsPrimitive || this.PropertySizesAreDefined() && this.FieldSizesAreDefined();
        }

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

        private Boolean FieldSizesAreDefined() {
            return this._type.GetFields( BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic ).Where( fieldInfo => !fieldInfo.FieldType.IsPrimitive ).All( HasMarshalDefinedSize );
        }

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