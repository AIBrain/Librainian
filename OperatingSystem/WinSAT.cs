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
                var dugum = xnList?.Item(0);
                return ( CpuScore: Convert.ToDecimal( dugum[ "CpuScore" ] ),
                    MemoryScore: Convert.ToDecimal( dugum[ "MemoryScore" ] ),
                    GraphicsScore: Convert.ToDecimal( dugum[ "GraphicsScore" ] ),
                    GamingScore: Convert.ToDecimal( dugum[ "GamingScore" ] ),
                    DiskScore: Convert.ToDecimal( dugum[ "DiskScore" ] ) );

            }

            return ( 0, 0, 0, 0, 0 );
        }

    }
}
