// Copyright 2016 Protiguous.
//
// This notice must be kept visible in the source.
//
// This section of source code belongs to Protiguous@Protiguous.com unless otherwise specified, or the
// original license has been overwritten by the automatic formatting of this code. Any unmodified
// sections of source code borrowed from other projects retain their original license and thanks
// goes to the Authors.
//
// Donations and royalties can be paid via
//  
//  bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//  
//
// Usage of the source code or compiled binaries is AS-IS. I am not responsible for Anything You Do.
//
// Contact me by email if you have any questions or helpful criticism.
//
// "Librainian/DynamicExtensions.cs" was last cleaned by Protiguous on 2016/06/18 at 10:50 PM

namespace Librainian.AmazedSaint {

    using System;
    using System.Linq;
    using System.Xml.Linq;

    /// <summary>
    ///     Extension methods for our ElasticObject. See
    ///     http: //amazedsaint.blogspot.com/2010/02/introducing-elasticobject-for-net-40.html for details
    /// </summary>
    public static class DynamicExtensions {

        /// <summary>Build an expando from an XElement</summary>
        /// <param name="el"></param>
        /// <returns></returns>
        public static ElasticObject ElasticFromXElement( XElement el ) {
            var exp = new ElasticObject();

            if ( !String.IsNullOrEmpty( el.Value ) ) {
                exp.InternalValue = el.Value;
            }

            exp.InternalName = el.Name.LocalName;

            foreach ( var a in el.Attributes() ) {
                exp.CreateOrGetAttribute( a.Name.LocalName, a.Value );
            }

            var textNode = el.Nodes().FirstOrDefault();
            if ( textNode is XText ) {
                exp.InternalContent = textNode.ToString();
            }

            foreach ( var child in el.Elements().Select( ElasticFromXElement ) ) {
                child.InternalParent = exp;
                exp.AddElement( child );
            }
            return exp;
        }

        /// <summary>Converts an XElement to the expando</summary>
        /// <param name="e"></param>
        /// <returns></returns>
        public static dynamic ToElastic( this XElement e ) => ElasticFromXElement( e );

        /// <summary>Converts an expando to XElement</summary>
        /// <param name="e"></param>
        /// <returns></returns>
        public static XElement ToXElement( this ElasticObject e ) => XElementFromElastic( e );

        /// <summary>Returns an XElement from an ElasticObject</summary>
        /// <param name="elastic"></param>
        /// <returns></returns>
        public static XElement XElementFromElastic( ElasticObject elastic ) {
            var exp = new XElement( elastic.InternalName );

            foreach ( var a in elastic.Attributes.Where( a => a.Value.InternalValue != null ) ) {
                exp.Add( new XAttribute( a.Key, a.Value.InternalValue ) );
            }

	        if ( elastic.InternalContent is String s ) {
                exp.Add( new XText( s ) );
            }

            foreach ( var child in elastic.Elements.Select( XElementFromElastic ) ) {
                exp.Add( child );
            }
            return exp;
        }
    }
}
