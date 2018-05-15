// Copyright © 1995-2018 to Rick@AIBrain.org and Protiguous. All Rights Reserved. This ENTIRE copyright notice and file header MUST BE KEPT VISIBLE in any source code derived from or used from our libraries and projects.
//
// ========================================================= This section of source code, "ValueTypeCheck.cs", belongs to Rick@AIBrain.org and Protiguous@Protiguous.com unless otherwise specified OR the original license
// has been overwritten by the automatic formatting. (We try to avoid that from happening, but it does happen.)
//
// Any unmodified portions of source code gleaned from other projects still retain their original license and our thanks goes to those Authors. =========================================================
//
// Donations (more please!), royalties from any software that uses any of our code, and license fees can be paid to us via bitcoin at the address 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2.
//
// ========================================================= Usage of the source code or compiled binaries is AS-IS. No warranties are expressed or implied. I am NOT responsible for Anything You Do With Our Code. =========================================================
//
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
//
// "Librainian/ValueTypeCheck.cs" was last cleaned by Protiguous on 2018/05/15 at 1:34 AM.

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

        public ValueTypeCheck( Type objectType ) => this._type = objectType;

        private static Boolean HasMarshalDefinedSize( MemberInfo info ) {
            var customAttributes = info.GetCustomAttributes( typeof( MarshalAsAttribute ), true );

            if ( customAttributes.Length == 0 ) { return false; }

            var attribute = ( MarshalAsAttribute )customAttributes[0];

            if ( attribute.Value == UnmanagedType.Currency ) { return true; }

            return attribute.SizeConst > 0;
        }

        private Boolean FieldSizesAreDefined() =>
            this._type.GetFields( BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic ).Where( fieldInfo => !fieldInfo.FieldType.IsPrimitive ).All( HasMarshalDefinedSize );

        private Boolean PropertySizesAreDefined() {
            foreach ( var propertyInfo in this._type.GetProperties( BindingFlags.Instance | BindingFlags.FlattenHierarchy | BindingFlags.Public | BindingFlags.NonPublic ) ) {
                if ( !propertyInfo.CanRead || !propertyInfo.CanWrite ) { return false; }

                if ( !propertyInfo.PropertyType.IsPrimitive && !HasMarshalDefinedSize( propertyInfo ) ) { return false; }
            }

            return true;
        }

        internal Boolean OnlyValueTypes() => this._type.IsPrimitive || this.PropertySizesAreDefined() && this.FieldSizesAreDefined();
    }
}