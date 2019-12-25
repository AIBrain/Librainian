// Copyright © Rick@AIBrain.org and Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
//
// This source code contained in "Text.cs" belongs to Protiguous@Protiguous.com and
// Rick@AIBrain.org unless otherwise specified or the original license has
// been overwritten by formatting.
// (We try to avoid it from happening, but it does accidentally happen.)
//
// Any unmodified portions of source code gleaned from other projects still retain their original
// license and our thanks goes to those Authors. If you find your code in this source code, please
// let us know so we can properly attribute you and include the proper license and/or copyright.
//
// If you want to use any of our code, you must contact Protiguous@Protiguous.com or
// Sales@AIBrain.org for permission and a quote.
//
// Donations are accepted (for now) via
//     bitcoin:1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//     PayPal:Protiguous@Protiguous.com
//     (We're always looking into other solutions.. Any ideas?)
//
// =========================================================
// Disclaimer:  Usage of the source code or binaries is AS-IS.
//    No warranties are expressed, implied, or given.
//    We are NOT responsible for Anything You Do With Our Code.
//    We are NOT responsible for Anything You Do With Our Executables.
//    We are NOT responsible for Anything You Do With Your Computer.
// =========================================================
//
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
// For business inquiries, please contact me at Protiguous@Protiguous.com
//
// Our website can be found at "https://Protiguous.com/"
// Our software can be found at "https://Protiguous.Software/"
// Our GitHub address is "https://github.com/Protiguous".
// Feel free to browse any source code we make available.
//
// Project: "Librainian", "Text.cs" was last formatted by Protiguous on 2019/08/08 at 7:01 AM.

namespace LibrainianCore.Databases {

    using System;
    using System.Data;
    using System.Diagnostics.CodeAnalysis;
    using Logging;

    public class Text {

        private String ConnectionString { get; }

        private Char Delimiter { get; }

        private String Path { get; }

        public Text( String path, Boolean hasHeaders, Char delimiter ) {
            this.Path = path;
            this.Delimiter = delimiter;

            var connectionStringBuilder = new OleDbConnectionStringBuilder {
                Provider = "Microsoft.Jet.OLEDB.4.0",
                DataSource = path
            };

            connectionStringBuilder.Add( "Extended Properties", "Excel 8.0;" + $"HDR={( hasHeaders ? "Yes" : "No" )}{';'}" );
            this.ConnectionString = connectionStringBuilder.ToString();
        }

        [NotNull]
        public String[] GetColumnsList( String worksheet ) {
            String[] columns = { };

            try {
                var connection = new OleDbConnection( this.ConnectionString );
                connection.Open();

                var tableColumns = connection.GetSchema( "Columns", new[] {
                    null, null, worksheet + '$', null
                } );

                connection.Close();

                columns = new String[ tableColumns.Rows.Count ];

                for ( var i = 0; i < columns.Length; i++ ) {
                    columns[ i ] = ( String )tableColumns.Rows[ i ][ "COLUMN_NAME" ];
                }
            }
            catch ( Exception exception ) {
                exception.Log();
            }

            return columns;
        }

        [NotNull]
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

        [SuppressMessage( "Microsoft.Security", "CA2100:Review SQL queries for security vulnerabilities" )]
        [NotNull]
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

        [NotNull]
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
                }
            }
            catch ( OleDbException exception ) {
                exception.Log();
            }

            return worksheets;
        }
    }
}