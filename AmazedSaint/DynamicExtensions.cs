// Copyright © 1995-2018 to Rick@AIBrain.org and Protiguous. All Rights Reserved. This ENTIRE copyright notice and file header MUST BE KEPT VISIBLE in any source code derived from or used from our libraries and projects.
//
// ========================================================= This section of source code, "DynamicExtensions.cs", belongs to Rick@AIBrain.org and Protiguous@Protiguous.com unless otherwise specified OR the original
// license has been overwritten by the automatic formatting. (We try to avoid that from happening, but it does happen.)
//
// Any unmodified portions of source code gleaned from other projects still retain their original license and our thanks goes to those Authors. =========================================================
//
// Donations (more please!), royalties from any software that uses any of our code, and license fees can be paid to us via bitcoin at the address 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2.
//
// ========================================================= Usage of the source code or compiled binaries is AS-IS. No warranties are expressed or implied. I am NOT responsible for Anything You Do With Our Code. =========================================================
//
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
//
// "Librainian/DynamicExtensions.cs" was last cleaned by Protiguous on 2018/05/15 at 1:28 AM.

namespace Librainian.AmazedSaint {

    using System;
    using System.Linq;
    using System.Xml.Linq;

    /// <summary>
    /// Extension methods for our ElasticObject. See
    /// http: //amazedsaint.blogspot.com/2010/02/introducing-elasticobject-for-net-40.html for details
    /// </summary>
    public static class DynamicExtensions {

        /// <summary>
        /// Build an expando from an XElement
        /// </summary>
        /// <param name="el"></param>
        /// <returns></returns>
        public static ElasticObject ElasticFromXElement( XElement el ) {
            var exp = new ElasticObject();

            if ( !String.IsNullOrEmpty( el.Value ) ) { exp.InternalValue = el.Value; }

            exp.InternalName = el.Name.LocalName;

            foreach ( var a in el.Attributes() ) { exp.CreateOrGetAttribute( a.Name.LocalName, a.Value ); }

            var textNode = el.Nodes().FirstOrDefault();

            if ( textNode is XText ) { exp.InternalContent = textNode.ToString(); }

            foreach ( var child in el.Elements().Select( ElasticFromXElement ) ) {
                child.InternalParent = exp;
                exp.AddElement( child );
            }

            return exp;
        }

        /// <summary>
        /// Converts an XElement to the expando
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        public static dynamic ToElastic( this XElement e ) => ElasticFromXElement( e );

        /// <summary>
        /// Converts an expando to XElement
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        public static XElement ToXElement( this ElasticObject e ) => XElementFromElastic( e );

        /// <summary>
        /// Returns an XElement from an ElasticObject
        /// </summary>
        /// <param name="elastic"></param>
        /// <returns></returns>
        public static XElement XElementFromElastic( ElasticObject elastic ) {
            var exp = new XElement( elastic.InternalName );

            foreach ( var a in elastic.Attributes.Where( a => a.Value.InternalValue != null ) ) { exp.Add( new XAttribute( a.Key, a.Value.InternalValue ) ); }

            if ( elastic.InternalContent is String s ) { exp.Add( new XText( s ) ); }

            foreach ( var child in elastic.Elements.Select( XElementFromElastic ) ) { exp.Add( child ); }

            return exp;
        }
    }
}