// Copyright 2015 Rick@AIBrain.org.
// 
// This notice must be kept visible in the source.
// 
// This section of source code belongs to Rick@AIBrain.Org unless otherwise specified, or the original license has been overwritten by the automatic formatting of this code.
// Any unmodified sections of source code borrowed from other projects retain their original license and thanks goes to the Authors.
// 
// Donations and royalties can be paid via
// PayPal: paypal@aibrain.org
// bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
// litecoin: LeUxdU2w3o6pLZGVys5xpDZvvo8DUrjBp9
// 
// Usage of the source code or compiled binaries is AS-IS.
// I am not responsible for Anything You Do.
// 
// Contact me by email if you have any questions or helpful criticism.
//  
// "Librainian/Text.cs" was last cleaned by Rick on 2015/11/14 at 6:30 AM

namespace Librainian.Database {

    using System;
    using System.Data;
    using System.Data.OleDb;

    public class Text {

        public Text( String path, Boolean hasHeaders, Char delimiter ) {
            this.Path = path;
            this.Delimiter = delimiter;
            var connectionStringBuilder = new OleDbConnectionStringBuilder {Provider = "Microsoft.Jet.OLEDB.4.0", DataSource = path};
            connectionStringBuilder.Add( "Extended Properties", "Excel 8.0;" + $"HDR={( hasHeaders ? "Yes" : "No" )}{';'}" );
            this.ConnectionString = connectionStringBuilder.ToString();
        }

        private String Path { get; }

        private Char Delimiter { get; }

        private String ConnectionString { get; }

        public String[] GetWorksheetList() {
            String[] worksheets = {};

            try {
                DataTable tableWorksheets;
                using ( var connection = new OleDbConnection( this.ConnectionString ) ) {
                    connection.Open();
                    tableWorksheets = connection.GetSchema( "Tables" );
                }

                worksheets = new String[tableWorksheets.Rows.Count];

                for ( var i = 0; i < worksheets.Length; i++ ) {
                    worksheets[ i ] = ( String ) tableWorksheets.Rows[ i ][ "TABLE_NAME" ];
                    worksheets[ i ] = worksheets[ i ].Remove( worksheets[ i ].Length - 1 )
                                                     .Trim( '"', '\'' );
                }
            }
            catch ( OleDbException exception ) {
                exception.More();
            }

            return worksheets;
        }

        public String[] GetColumnsList( String worksheet ) {
            String[] columns = {};

            try {
                var connection = new OleDbConnection( this.ConnectionString );
                connection.Open();
                var tableColumns = connection.GetSchema( "Columns", new[] {null, null, worksheet + '$', null} );
                connection.Close();

                columns = new String[tableColumns.Rows.Count];

                for ( var i = 0; i < columns.Length; i++ ) {
                    columns[ i ] = ( String ) tableColumns.Rows[ i ][ "COLUMN_NAME" ];
                }
            }
            catch ( Exception exception ) {
                exception.More();
            }

            return columns;
        }

        public DataTable GetWorksheet( String worksheet ) {
            using ( var connection = new OleDbConnection( this.ConnectionString ) ) {
                using ( var adaptor = new OleDbDataAdapter( $"SELECT * FROM [{worksheet}$]", connection ) ) {
                    var ws = new DataTable( worksheet );
                    adaptor.FillSchema( ws, SchemaType.Source );
                    adaptor.Fill( ws );
                    return ws;
                }
            }
        }

        public DataSet GetWorkplace() {
            using ( var connection = new OleDbConnection( this.ConnectionString ) ) {
                using ( var adaptor = new OleDbDataAdapter( "SELECT * FROM *", connection ) ) {
                    var workplace = new DataSet();
                    adaptor.FillSchema( workplace, SchemaType.Source );
                    adaptor.Fill( workplace );
                    return workplace;
                }
            }
        }

    }

}
