// Copyright 2018 Protiguous.
//
// This notice must be kept visible in the source.
//
// This section of source code belongs to Protiguous@Protiguous.com unless otherwise specified or the original license has been overwritten by the automatic formatting of this code.
//
// Any unmodified sections of source code borrowed from other projects retain their original license and thanks goes to the Authors.
//
// Donations, royalties, and licenses can be paid via bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//
// Usage of the source code or compiled binaries is AS-IS. I am not responsible for Anything You Do.
//
// Contact me by email if you have any questions or helpful criticism.
//
// "Librainian/IHierarchyWrapperProvider.cs" was last cleaned by Protiguous on 2018/05/12 at 1:18 AM

namespace Librainian.AmazedSaint {

    using System;
    using System.Collections.Generic;

    public interface IHierarchyWrapperProvider<T> {

        IEnumerable<KeyValuePair<String, T>> Attributes { get; }

        IEnumerable<T> Elements { get; }

        Object InternalContent { get; set; }

        String InternalName { get; set; }

        T InternalParent { get; set; }

        Object InternalValue { get; set; }

        void AddAttribute( String key, T value );

        void AddElement( T element );

        T Attribute( String name );

        T Element( String name );

        Object GetAttributeValue( String name );

        Boolean HasAttribute( String name );

        void RemoveAttribute( String key );

        void RemoveElement( T element );

        void SetAttributeValue( String name, Object obj );
    }
}