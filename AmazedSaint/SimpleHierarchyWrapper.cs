// Copyright © 1995-2018 to Rick@AIBrain.org and Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
//
// This source code contained in "SimpleHierarchyWrapper.cs" belongs to Rick@AIBrain.org and
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
// "Librainian/Librainian/SimpleHierarchyWrapper.cs" was last formatted by Protiguous on 2018/05/24 at 6:57 PM.

namespace Librainian.AmazedSaint {

    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    ///     A concrete hierarchy wrapper
    /// </summary>
    public class SimpleHierarchyWrapper : IElasticHierarchyWrapper {

        private Dictionary<String, ElasticObject> _attributes { get; } = new Dictionary<String, ElasticObject>();

        private Dictionary<String, List<ElasticObject>> _elements { get; } = new Dictionary<String, List<ElasticObject>>();

        public IEnumerable<KeyValuePair<String, ElasticObject>> Attributes => this._attributes;

        public IEnumerable<ElasticObject> Elements => this._elements.SelectMany( list => list.Value );

        public Object InternalContent { get; set; }

        public String InternalName { get; set; }

        public ElasticObject InternalParent { get; set; }

        public Object InternalValue { get; set; }

        public void AddAttribute( String key, ElasticObject value ) => this._attributes.Add( key, value );

        public void AddElement( ElasticObject element ) {
            if ( !this._elements.ContainsKey( element.InternalName ) ) { this._elements[element.InternalName] = new List<ElasticObject>(); }

            this._elements[element.InternalName].Add( element );
        }

        public ElasticObject Attribute( String name ) => this.HasAttribute( name ) ? this._attributes[name] : null;

        public ElasticObject Element( String name ) => this.Elements.FirstOrDefault( item => item.InternalName == name );

        public Object GetAttributeValue( String name ) => this._attributes[name].InternalValue;

        public Boolean HasAttribute( String name ) => this._attributes.ContainsKey( name );

        public void RemoveAttribute( String key ) => this._attributes.Remove( key );

        public void RemoveElement( ElasticObject element ) {
            if ( !this._elements.ContainsKey( element.InternalName ) ) { return; }

            if ( this._elements[element.InternalName].Contains( element ) ) { this._elements[element.InternalName].Remove( element ); }
        }

        public void SetAttributeValue( String name, Object obj ) => this._attributes[name].InternalValue = obj;
    }
}