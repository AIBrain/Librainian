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
// "Librainian/SimpleHierarchyWrapper.cs" was last cleaned by Rick on 2014/08/11 at 12:36 AM
#endregion

namespace Librainian.AmazedSaint {
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    ///     A concrete hierarchy wrapper
    /// </summary>
    public class SimpleHierarchyWrapper : IElasticHierarchyWrapper {
        private readonly Dictionary< String, ElasticObject > _attributes = new Dictionary< String, ElasticObject >();

        private readonly Dictionary< String, List< ElasticObject > > _elements = new Dictionary< String, List< ElasticObject > >();

        #region IElasticHierarchyWrapper Members
        public IEnumerable< KeyValuePair< String, ElasticObject > > Attributes { get { return this._attributes; } }

        public IEnumerable< ElasticObject > Elements {
            get {
                var result = this._elements.SelectMany( list => list.Value );
                return result;
            }
        }

        public object InternalContent { get; set; }

        public String InternalName { get; set; }

        public ElasticObject InternalParent { get; set; }

        public object InternalValue { get; set; }

        public void AddAttribute( String key, ElasticObject value ) => this._attributes.Add( key, value );

        public void AddElement( ElasticObject element ) {
            if ( !this._elements.ContainsKey( element.InternalName ) ) {
                this._elements[ element.InternalName ] = new List< ElasticObject >();
            }
            this._elements[ element.InternalName ].Add( element );
        }

        public ElasticObject Attribute( String name ) => this.HasAttribute( name ) ? this._attributes[ name ] : null;

        public ElasticObject Element( String name ) => this.Elements.FirstOrDefault( item => item.InternalName == name );

        public object GetAttributeValue( String name ) => this._attributes[ name ].InternalValue;

        public Boolean HasAttribute( String name ) => this._attributes.ContainsKey( name );

        public void RemoveAttribute( String key ) => this._attributes.Remove( key );

        public void RemoveElement( ElasticObject element ) {
            if ( !this._elements.ContainsKey( element.InternalName ) ) {
                return;
            }
            if ( this._elements[ element.InternalName ].Contains( element ) ) {
                this._elements[ element.InternalName ].Remove( element );
            }
        }

        public void SetAttributeValue( String name, object obj ) => this._attributes[ name ].InternalValue = obj;
        #endregion IElasticHierarchyWrapper Members
    }
}
