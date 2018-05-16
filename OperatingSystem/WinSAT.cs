// Copyright © 1995-2018 to Rick@AIBrain.org and Protiguous.
// All Rights Reserved.
//
// This ENTIRE copyright notice and file header MUST BE KEPT
// VISIBLE in any source code derived from or used from our
// libraries and projects.
//
// =========================================================
// This section of source code, "WinSAT.cs",
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
// "Librainian/Librainian/WinSAT.cs" was last cleaned by Protiguous on 2018/05/15 at 10:48 PM.

namespace Librainian.OperatingSystem {

    using System;
    using System.Linq;
    using System.Xml;
    using FileSystem;

    public static class WinSAT {

        private static (Decimal CpuScore, Decimal MemoryScore, Decimal GraphicsScore, Decimal GamingScore, Decimal DiskScore) ReadXML() {
            var folder = new Folder( Environment.SpecialFolder.Windows, "Performance\\WinSAT\\DataStore" );

            var documents = folder.GetDocuments( "*.xml" );

            var newest = documents.Where( document => document.Info.Name.Contains( "Formal.Assessment" ) ).OrderByDescending( document => document.Info.LastWriteTime ).FirstOrDefault();

            var xml = new XmlDocument();

            if ( newest != null ) {
                xml.Load( newest.FullPathWithFileName );
                var xnList = xml.SelectNodes( "/WinSAT/WinSPR" );
                var dugum = xnList?.Item( 0 );

                return (CpuScore: Convert.ToDecimal( dugum["CpuScore"] ), MemoryScore: Convert.ToDecimal( dugum["MemoryScore"] ), GraphicsScore: Convert.ToDecimal( dugum["GraphicsScore"] ),
                    GamingScore: Convert.ToDecimal( dugum["GamingScore"] ), DiskScore: Convert.ToDecimal( dugum["DiskScore"] ));
            }

            return (0, 0, 0, 0, 0);
        }
    }
}