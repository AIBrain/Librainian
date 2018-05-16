// Copyright © 1995-2018 to Rick@AIBrain.org and Protiguous.
// All Rights Reserved.
//
// This ENTIRE copyright notice and file header MUST BE KEPT
// VISIBLE in any source code derived from or used from our
// libraries and projects.
//
// =========================================================
// This section of source code, "SimpleHierarchyWrapper.cs",
// belongs to Rick@AIBrain.org and Protiguous@Protiguous.com
// unless otherwise specified OR the original license has been
// overwritten by the automatic formatting.
//
// (We try to avoid that from happening, but it does happen.)
//
// Any unmodified portions of source code gleaned from other
// projects still retain their original license and our thanks
// goes to those Authors.
// =========================================================
//
// Donations (more please!), royalties from any software that
// uses any of our code, and license fees can be paid to us via
// bitcoin at the address 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2.
//
// =========================================================
// Usage of the source code or compiled binaries is AS-IS.
// No warranties are expressed or implied.
// I am NOT responsible for Anything You Do With Our Code.
// =========================================================
//
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
//
// "Librainian/Librainian/SimpleHierarchyWrapper.cs" was last cleaned by Protiguous on 2018/05/15 at 10:37 PM.

namespace Librainian.AmazedSaint {

    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    ///     A concrete hierarchy wrapper
    /// </summary>
    public class SimpleHierarchyWrapper : IElasticHierarchyWrapper {

        private readonly Dictionary<String, ElasticObject> _attributes = new Dictionary<String, ElasticObject>();

        private readonly Dictionary<String, List<ElasticObject>> _elements = new Dictionary<String, List<ElasticObject>>();

        public IEnumerable<KeyValuePair<String, ElasticObject>> Attributes => this._attributes;

        public IEnumerable<ElasticObject> Elements {
            get {
                var result = this._elements.SelectMany( list => list.Value );

                return result;
            }
        }

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