// Copyright © 1995-2018 to Rick@AIBrain.org and Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
//
// This source code contained in "ElasticObject.cs" belongs to Rick@AIBrain.org and
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
//
// "Librainian/Librainian/ElasticObject.cs" was last formatted by Protiguous on 2018/05/21 at 10:49 PM.

namespace Librainian.AmazedSaint {

    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Dynamic;
    using System.Linq;
    using System.Linq.Expressions;

    /// <summary>
    ///     See http://amazedsaint.blogspot.com/2010/02/introducing-elasticobject-for-net-40.html for details
    /// </summary>
    public class ElasticObject : DynamicObject, IElasticHierarchyWrapper, INotifyPropertyChanged {

        public ElasticObject() : this( $"id={Guid.NewGuid()}" ) { }

        public ElasticObject( String name, Object value = null ) {
            this.InternalName = name;
            this.InternalValue = value;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private IElasticHierarchyWrapper ElasticProvider { get; } = new SimpleHierarchyWrapper();

        private NodeType NodeType { get; set; } = NodeType.Element;

        public Object InternalContent {
            get => this.ElasticProvider.InternalContent;

            set => this.ElasticProvider.InternalContent = value;
        }

        /// <summary>
        ///     Fully qualified name
        /// </summary>
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
            get => this.ElasticProvider.InternalName;

            set => this.ElasticProvider.InternalName = value;
        }

        public ElasticObject InternalParent {
            get => this.ElasticProvider.InternalParent;

            set => this.ElasticProvider.InternalParent = value;
        }

        public Object InternalValue {
            get => this.ElasticProvider.InternalValue;

            set => this.ElasticProvider.InternalValue = value;
        }

        public IEnumerable<KeyValuePair<String, ElasticObject>> Attributes => this.ElasticProvider.Attributes;

        public IEnumerable<ElasticObject> Elements => this.ElasticProvider.Elements;

        private void OnPropertyChanged( String prop ) => this.PropertyChanged?.Invoke( this, new PropertyChangedEventArgs( prop ) );

        /// <summary>
        ///     Add a member to this element, with the specified value
        /// </summary>
        /// <param name="memberName"></param>
        /// <param name="value">     </param>
        /// <returns></returns>
        internal ElasticObject CreateOrGetAttribute( String memberName, Object value ) {
            if ( !this.HasAttribute( memberName ) ) { this.AddAttribute( memberName, new ElasticObject( memberName, value ) ); }

            return this.Attribute( memberName );
        }

        public void AddAttribute( String key, ElasticObject value ) {
            value.NodeType = NodeType.Attribute;
            value.InternalParent = this;
            this.ElasticProvider.AddAttribute( key, value );
        }

        public void AddElement( ElasticObject element ) {
            element.NodeType = NodeType.Element;
            element.InternalParent = this;
            this.ElasticProvider.AddElement( element );
        }

        public ElasticObject Attribute( String name ) => this.ElasticProvider.Attribute( name );

        public ElasticObject Element( String name ) => this.ElasticProvider.Element( name );

        public Object GetAttributeValue( String name ) => this.ElasticProvider.GetAttributeValue( name );

        public Boolean HasAttribute( String name ) => this.ElasticProvider.HasAttribute( name );

        public void RemoveAttribute( String key ) => this.ElasticProvider.RemoveAttribute( key );

        public void RemoveElement( ElasticObject element ) => this.ElasticProvider.RemoveElement( element );

        public void SetAttributeValue( String name, Object obj ) => this.ElasticProvider.SetAttributeValue( name, obj );

        /// <summary>
        ///     Interpret the invocation of a binary operation
        /// </summary>
        /// <param name="binder">todo: describe binder parameter on TryBinaryOperation</param>
        /// <param name="arg">   todo: describe arg parameter on TryBinaryOperation</param>
        /// <param name="result">todo: describe result parameter on TryBinaryOperation</param>
        public override Boolean TryBinaryOperation( BinaryOperationBinder binder, Object arg, out Object result ) {
            if ( binder.Operation == ExpressionType.LeftShiftAssign && this.NodeType == NodeType.Element ) {
                this.InternalContent = arg;
                result = this;

                return true;
            }

            if ( binder.Operation == ExpressionType.LeftShiftAssign && this.NodeType == NodeType.Attribute ) {
                this.InternalValue = arg;
                result = this;

                return true;
            }

            switch ( binder.Operation ) {
                case ExpressionType.LeftShift:

                    if ( arg is String s ) {
                        var exp = new ElasticObject( s ) { NodeType = NodeType.Element };
                        this.AddElement( exp );
                        result = exp;

                        return true;
                    }

                    if ( arg is ElasticObject o ) {
                        var eobj = o;

                        if ( !this.Elements.Contains( eobj ) ) { this.AddElement( eobj ); }

                        result = eobj;

                        return true;
                    }

                    break;

                case ExpressionType.LessThan: {
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

                        if ( arg is ElasticObject elasticObject ) {

                            //TODO
                            //HACK
                            //this.AddAttribute( memberName, eobj );
                            result = elasticObject;

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

        /// <summary>
        ///     Handle the indexer operations
        /// </summary>
        /// <param name="binder"> todo: describe binder parameter on TryGetIndex</param>
        /// <param name="indexes">todo: describe indexes parameter on TryGetIndex</param>
        /// <param name="result"> todo: describe result parameter on TryGetIndex</param>
        public override Boolean TryGetIndex( GetIndexBinder binder, Object[] indexes, out Object result ) {
            if ( indexes.Length == 1 && indexes[0] is null ) { result = this.ElasticProvider.Elements.ToList(); }
            else if ( indexes.Length == 1 && indexes[0] is Int32 indx ) { result = this.Elements.ElementAt( indx ); }
            else if ( indexes.Length == 1 && indexes[0] is Func<Object, Boolean> filter ) { result = this.Elements.Where( filter ).ToList(); }
            else { result = this.Elements.Where( c => indexes.Cast<String>().Contains( c.InternalName ) ).ToList(); }

            return true;
        }

        /// <summary>
        ///     Catch a get member invocation
        /// </summary>
        /// <param name="binder"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public override Boolean TryGetMember( GetMemberBinder binder, out Object result ) {
            if ( this.ElasticProvider.HasAttribute( binder.Name ) ) { result = this.ElasticProvider.Attribute( binder.Name ).InternalValue; }
            else {
                var obj = this.ElasticProvider.Element( binder.Name );

                if ( obj != null ) { result = obj; }
                else {
                    var exp = new ElasticObject( binder.Name );
                    this.ElasticProvider.AddElement( exp );
                    result = exp;
                }
            }

            return true;
        }

        /// <summary>
        ///     Interpret a method call
        /// </summary>
        /// <param name="binder"></param>
        /// <param name="args">  </param>
        /// <param name="result"></param>
        /// <returns></returns>
        public override Boolean TryInvokeMember( InvokeMemberBinder binder, Object[] args, out Object result ) {
            var obj = new ElasticObject( binder.Name );
            this.AddElement( obj );
            result = obj;

            return true;
        }

        /// <summary>
        ///     Catch a set member invocation
        /// </summary>
        /// <param name="binder">todo: describe binder parameter on TrySetMember</param>
        /// <param name="value"> todo: describe value parameter on TrySetMember</param>
        public override Boolean TrySetMember( SetMemberBinder binder, Object value ) {
            var memberName = binder.Name;

            if ( value is ElasticObject o ) {
                var eobj = o;

                if ( !this.Elements.Contains( eobj ) ) { this.AddElement( eobj ); }
            }
            else {
                if ( !this.ElasticProvider.HasAttribute( memberName ) ) { this.ElasticProvider.AddAttribute( memberName, new ElasticObject( memberName, value ) ); }
                else { this.ElasticProvider.SetAttributeValue( memberName, value ); }
            }

            this.OnPropertyChanged( memberName );

            return true;
        }

        /// <summary>
        ///     Try the unary operation.
        /// </summary>
        /// <param name="binder">todo: describe binder parameter on TryUnaryOperation</param>
        /// <param name="result">todo: describe result parameter on TryUnaryOperation</param>
        public override Boolean TryUnaryOperation( UnaryOperationBinder binder, out Object result ) {
            if ( binder.Operation == ExpressionType.OnesComplement ) {
                result = this.NodeType == NodeType.Element ? this.InternalContent : this.InternalValue;

                return true;
            }

            return base.TryUnaryOperation( binder, out result );
        }
    }
}