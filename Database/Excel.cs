// Copyright 2018 Protiguous.
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
// "Librainian/Excel.cs" was last cleaned by Protiguous on 2016/06/18 at 10:50 PM

namespace Librainian.Database {

    using System;
    using System.Data;
    using System.Data.OleDb;
    using JetBrains.Annotations;

    public class Excel {

        public Excel( String path, Boolean hasHeaders, Boolean hasMixedData ) {
            this.Path = path;
            var strBuilder = new OleDbConnectionStringBuilder { Provider = "Microsoft.Jet.OLEDB.4.0", DataSource = path };
            strBuilder.Add( "Extended Properties", String.Format( "Excel 8.0;HDR={0}{1}Imex={2}{1}", hasHeaders ? "Yes" : "No", ';', hasMixedData ? "2" : "0" ) );
            this.ConnectionString = strBuilder.ToString();
        }

        private String ConnectionString {
            get;
        }

        private String Path {
            get;
        }

        public String[] GetColumnsList( String worksheet ) {
            String[] columns = { };

            try {
                DataTable tableColumns;
                using ( var connection = new OleDbConnection( this.ConnectionString ) ) {
                    connection.Open();
                    tableColumns = connection.GetSchema( "Columns", new[] { null, null, worksheet + '$', null } );
                }

                columns = new String[ tableColumns.Rows.Count ];

                for ( var i = 0; i < columns.Length; i++ ) {
                    columns[ i ] = ( String )tableColumns.Rows[ i ][ "COLUMN_NAME" ];
                }
            }
            catch ( OleDbException exception ) {
                exception.More();
            }

            return columns;
        }

        [CanBeNull]
        public DataSet GetWorkplace() {
            try {
                using ( var connection = new OleDbConnection( this.ConnectionString ) ) {
                    using ( var adaptor = new OleDbDataAdapter( "SELECT * FROM *", connection ) ) {
                        var workplace = new DataSet();
                        adaptor.FillSchema( workplace, SchemaType.Source );
                        adaptor.Fill( workplace );
                        return workplace;
                    }
                }
            }
            catch ( OleDbException exception ) {
                exception.More();
            }
            return null;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage( "Microsoft.Security", "CA2100:Review SQL queries for security vulnerabilities" )]
        [CanBeNull]
        public DataTable GetWorksheet( String worksheet ) {
            try {
                using ( var connection = new OleDbConnection( this.ConnectionString ) ) {
                    using ( var adaptor = new OleDbDataAdapter( $"SELECT * FROM [{worksheet}$]", connection ) { SelectCommand= new OleDbCommand(worksheet)} ) {
                        
                        var ws = new DataTable( worksheet );
                        adaptor.FillSchema( ws, SchemaType.Source );
                        adaptor.Fill( ws );
                        return ws;
                    }
                }
            }
            catch ( OleDbException exception ) {
                exception.More();
            }
            return null;
        }

        public String[] GetWorksheetList() {
            String[] worksheets = { };

            try {
                DataTable tableWorksheets;
                using ( var connection = new OleDbConnection( this.ConnectionString ) ) {
                    connection.Open();
                    tableWorksheets = connection.GetSchema( "Tables" );
                }

                worksheets = new String[ tableWorksheets.Rows.Count ];

                for ( var i = 0; i < worksheets.Length; i++ ) {
                    worksheets[ i ] = ( String )tableWorksheets.Rows[ i ][ "TABLE_NAME" ];
                    worksheets[ i ] = worksheets[ i ].Remove( worksheets[ i ].Length - 1 ).Trim( '"', '\'' );

                    // removes the trailing $ and other characters appended in the table name
                    while ( worksheets[ i ].EndsWith( "$" ) ) {
                        worksheets[ i ] = worksheets[ i ].Remove( worksheets[ i ].Length - 1 ).Trim( '"', '\'' );
                    }
                }
            }
            catch ( OleDbException exception ) {
                exception.More();
            }

            return worksheets;
        }
    }
}