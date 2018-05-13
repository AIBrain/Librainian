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
// "Librainian/XMLExtensions.cs" was last cleaned by Protiguous on 2018/05/12 at 1:23 AM

namespace Librainian.Extensions {

    using System;
    using System.Collections.Generic;
    using System.Xml;
    using System.Xml.Linq;

    public static class XMLExtensions {

        private static IEnumerable<XElement> SimpleStreamAxis( String inputUrl, String elementName ) {
            using ( var reader = XmlReader.Create( inputUrl ) ) {
                reader.MoveToContent();

                while ( reader.Read() ) {
                    if ( reader.NodeType != XmlNodeType.Element ) {
                        continue;
                    }

                    if ( reader.Name != elementName ) {
                        continue;
                    }

                    if ( XNode.ReadFrom( reader ) is XElement el ) {
                        yield return el;
                    }
                }
            }
        }
    }
}