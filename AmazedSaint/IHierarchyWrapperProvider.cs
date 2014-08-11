#region License & Information
// This notice must be kept visible in the source.
// 
// This section of source code belongs to Rick@AIBrain.Org unless otherwise specified,
// or the original license has been overwritten by the automatic formatting of this code.
// Any unmodified sections of source code borrowed from other projects retain their original license and thanks goes to the Authors.
// 
// Donations and Royalties can be paid via
// PayPal: paypal@aibrain.org
// bitcoin:1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
// bitcoin:1NzEsF7eegeEWDr5Vr9sSSgtUC4aL6axJu
// litecoin:LeUxdU2w3o6pLZGVys5xpDZvvo8DUrjBp9
// 
// Usage of the source code or compiled binaries is AS-IS.
// I am not responsible for Anything You Do.
// 
// "Librainian/IHierarchyWrapperProvider.cs" was last cleaned by Rick on 2014/08/11 at 12:36 AM
#endregion

namespace Librainian.AmazedSaint {
    using System;
    using System.Collections.Generic;

    public interface IHierarchyWrapperProvider< T > {
        IEnumerable< KeyValuePair< string, T > > Attributes { get; }

        IEnumerable< T > Elements { get; }

        object InternalContent { get; set; }

        String InternalName { get; set; }

        T InternalParent { get; set; }

        object InternalValue { get; set; }

        void AddAttribute( String key, T value );

        void AddElement( T element );

        T Attribute( String name );

        T Element( String name );

        object GetAttributeValue( String name );

        Boolean HasAttribute( String name );

        void RemoveAttribute( String key );

        void RemoveElement( T element );

        void SetAttributeValue( String name, object obj );
    }
}
