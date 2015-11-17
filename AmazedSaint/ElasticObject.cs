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
// "Librainian/ElasticObject.cs" was last cleaned by Rick on 2015/06/12 at 2:50 PM

namespace Librainian.AmazedSaint {

    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Dynamic;
    using System.Linq;
    using System.Linq.Expressions;

    /// <summary>
    /// See http://amazedsaint.blogspot.com/2010/02/introducing-elasticobject-for-net-40.html for details
    /// </summary>
    public class ElasticObject : DynamicObject, IElasticHierarchyWrapper, INotifyPropertyChanged {
        private readonly IElasticHierarchyWrapper _elasticProvider = new SimpleHierarchyWrapper();
        private NodeType _nodeType = NodeType.Element;

        public IEnumerable<KeyValuePair<String, ElasticObject>> Attributes => this._elasticProvider.Attributes;

        public IEnumerable<ElasticObject> Elements => this._elasticProvider.Elements;

        public Object InternalContent {
            get {
                return this._elasticProvider.InternalContent;
            }

            set {
                this._elasticProvider.InternalContent = value;
            }
        }

        /// <summary>Fully qualified name</summary>
        public String InternalFullName {
            get {
                var path = this.InternalName;
                var parent = this.InternalParent;

                while ( parent != null ) {
                    path = $"{parent.InternalName}_{path}";
                    parent = parent.InternalParent;
                }

                return path;
            }
        }

        public String InternalName {
            get {
                return this._elasticProvider.InternalName;
            }

            set {
                this._elasticProvider.InternalName = value;
            }
        }

        public ElasticObject InternalParent {
            get {
                return this._elasticProvider.InternalParent;
            }

            set {
                this._elasticProvider.InternalParent = value;
            }
        }

        public Object InternalValue {
            get {
                return this._elasticProvider.InternalValue;
            }

            set {
                this._elasticProvider.InternalValue = value;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public ElasticObject() : this( $"id={Guid.NewGuid()}" ) {
        }

        public ElasticObject(String name, Object value = null) {
            this.InternalName = name;
            this.InternalValue = value;
        }

        public void AddAttribute(String key, ElasticObject value) {
            value._nodeType = NodeType.Attribute;
            value.InternalParent = this;
            this._elasticProvider.AddAttribute( key, value );
        }

        public void AddElement(ElasticObject element) {
            element._nodeType = NodeType.Element;
            element.InternalParent = this;
            this._elasticProvider.AddElement( element );
        }

        public ElasticObject Attribute(String name) => this._elasticProvider.Attribute( name );

        public ElasticObject Element(String name) => this._elasticProvider.Element( name );

        public Object GetAttributeValue(String name) => this._elasticProvider.GetAttributeValue( name );

        public Boolean HasAttribute(String name) => this._elasticProvider.HasAttribute( name );

        public void RemoveAttribute(String key) => this._elasticProvider.RemoveAttribute( key );

        public void RemoveElement(ElasticObject element) => this._elasticProvider.RemoveElement( element );

        public void SetAttributeValue(String name, Object obj) => this._elasticProvider.SetAttributeValue( name, obj );

        /// <summary>Interpret the invocation of a binary operation</summary>
        public override Boolean TryBinaryOperation(BinaryOperationBinder binder, Object arg, out Object result) {
            if ( ( binder.Operation == ExpressionType.LeftShiftAssign ) && ( this._nodeType == NodeType.Element ) ) {
                this.InternalContent = arg;
                result = this;
                return true;
            }
            if ( ( binder.Operation == ExpressionType.LeftShiftAssign ) && ( this._nodeType == NodeType.Attribute ) ) {
                this.InternalValue = arg;
                result = this;
                return true;
            }
            switch ( binder.Operation ) {
                case ExpressionType.LeftShift:
                    var s = arg as String;
                    if ( s != null ) {
                        var exp = new ElasticObject( s ) {
                            _nodeType = NodeType.Element
                        };
                        this.AddElement( exp );
                        result = exp;
                        return true;
                    }
                    var o = arg as ElasticObject;
                    if ( o != null ) {
                        var eobj = o;
                        if ( !this.Elements.Contains( eobj ) ) {
                            this.AddElement( eobj );
                        }
                        result = eobj;
                        return true;
                    }
                    break;

                case ExpressionType.LessThan:
                    {
                        var memberName = arg as String;
                        if ( arg is String ) {
                            if ( !this.HasAttribute( memberName ) ) {
                                var att = new ElasticObject( memberName );
                                this.AddAttribute( memberName, att );
                                result = att;
                                return true;
                            }
                            throw new InvalidOperationException( $"An attribute with name {memberName} already exists" );
                        }
                        var elasticObject = arg as ElasticObject;
                        if ( elasticObject != null ) {
                            var eobj = elasticObject;

                            //TODO
                            //HACK
                            //this.AddAttribute( memberName, eobj );
                            result = eobj;
                            return true;
                        }
                    }
                    break;

                case ExpressionType.GreaterThan:
                    if ( arg is FormatType ) {
                        result = this.ToXElement();
                        return true;
                    }
                    break;
            }
            return base.TryBinaryOperation( binder, arg, out result );
        }

        /// <summary>Handle the indexer operations</summary>
        public override Boolean TryGetIndex(GetIndexBinder binder, Object[] indexes, out Object result) {
            if ( ( indexes.Length == 1 ) && ( indexes[ 0 ] == null ) ) {
                result = this._elasticProvider.Elements.ToList();
            }
            else if ( ( indexes.Length == 1 ) && indexes[ 0 ] is Int32 ) {
                var indx = ( Int32 )indexes[ 0 ];
                var elmt = this.Elements.ElementAt( indx );
                result = elmt;
            }
            else if ( ( indexes.Length == 1 ) && indexes[ 0 ] is Func<Object, Boolean> ) {
                var filter = indexes[ 0 ] as Func<Object, Boolean>;
                result = this.Elements.Where( filter ).ToList();
            }
            else {
                result = this.Elements.Where( c => indexes.Cast<String>().Contains( c.InternalName ) ).ToList();
            }

            return true;
        }

        /// <summary>Catch a get member invocation</summary>
        /// <param name="binder"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public override Boolean TryGetMember(GetMemberBinder binder, out Object result) {
            if ( this._elasticProvider.HasAttribute( binder.Name ) ) {
                result = this._elasticProvider.Attribute( binder.Name ).InternalValue;
            }
            else {
                var obj = this._elasticProvider.Element( binder.Name );
                if ( obj != null ) {
                    result = obj;
                }
                else {
                    var exp = new ElasticObject( binder.Name );
                    this._elasticProvider.AddElement( exp );
                    result = exp;
                }
            }

            return true;
        }

        /// <summary>Interpret a method call</summary>
        /// <param name="binder"></param>
        /// <param name="args"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public override Boolean TryInvokeMember(InvokeMemberBinder binder, Object[] args, out Object result) {
            var obj = new ElasticObject( binder.Name );
            this.AddElement( obj );
            result = obj;
            return true;
        }

        /// <summary>Catch a set member invocation</summary>
        public override Boolean TrySetMember(SetMemberBinder binder, Object value) {
            var memberName = binder.Name;

            var o = value as ElasticObject;
            if ( o != null ) {
                var eobj = o;
                if ( !this.Elements.Contains( eobj ) ) {
                    this.AddElement( eobj );
                }
            }
            else {
                if ( !this._elasticProvider.HasAttribute( memberName ) ) {
                    this._elasticProvider.AddAttribute( memberName, new ElasticObject( memberName, value ) );
                }
                else {
                    this._elasticProvider.SetAttributeValue( memberName, value );
                }
            }

            this.OnPropertyChanged( memberName );

            return true;
        }

        /// <summary>Try the unary operation.</summary>
        public override Boolean TryUnaryOperation(UnaryOperationBinder binder, out Object result) {
            if ( binder.Operation == ExpressionType.OnesComplement ) {
                result = this._nodeType == NodeType.Element ? this.InternalContent : this.InternalValue;
                return true;
            }
            return base.TryUnaryOperation( binder, out result );
        }

        /// <summary>Add a member to this element, with the specified value</summary>
        /// <param name="memberName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        internal ElasticObject CreateOrGetAttribute(String memberName, Object value) {
            if ( !this.HasAttribute( memberName ) ) {
                this.AddAttribute( memberName, new ElasticObject( memberName, value ) );
            }

            return this.Attribute( memberName );
        }

        private void OnPropertyChanged(String prop) => this.PropertyChanged?.Invoke( this, new PropertyChangedEventArgs( prop ) );
    }
}