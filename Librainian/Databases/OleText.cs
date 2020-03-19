// Copyright © 2020 Protiguous. All Rights Reserved.
// 
// This entire copyright notice and license must be retained and must be kept visible in any binaries, libraries, repositories, and source code (directly or derived)
// from our binaries, libraries, projects, or solutions.
// 
// This source code contained in "OleText.cs" belongs to Protiguous@Protiguous.com unless otherwise specified or the original license has been overwritten
// by formatting. (We try to avoid it from happening, but it does accidentally happen.)
// 
// Any unmodified portions of source code gleaned from other projects still retain their original license and our thanks goes to those Authors.
// If you find your code in this source code, please let us know so we can properly attribute you and include the proper license and/or copyright.
// 
// If you want to use any of our code in a commercial project, you must contact Protiguous@Protiguous.com for permission and a quote.
// 
// Donations are accepted via bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2 and PayPal: Protiguous@Protiguous.com
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
// For business inquiries, please contact me at Protiguous@Protiguous.com.
// 
// Our website can be found at "https://Protiguous.com/"
// Our software can be found at "https://Protiguous.Software/"
// Our GitHub address is "https://github.com/Protiguous".
// Feel free to browse any source code we make available.
// 
// Project: "Librainian", File: "OleText.cs" was last formatted by Protiguous on 2020/03/18 at 10:23 AM.

namespace Librainian.Databases {

    using System;
    using System.Data;
    using System.Data.OleDb;
    using JetBrains.Annotations;
    using Logging;

    public class OleText {

        private String ConnectionString { get; }

        private Char Delimiter { get; }

        private String Path { get; }

        public OleText( [CanBeNull] String? path, Boolean hasHeaders, Char delimiter ) {
            this.Path = path;
            this.Delimiter = delimiter;

            var connectionStringBuilder = new OleDbConnectionStringBuilder {
                Provider = "Microsoft.Jet.OLEDB.4.0", DataSource = path
            };

            connectionStringBuilder.Add( "Extended Properties", "Excel 8.0;" + $"HDR={( hasHeaders ? "Yes" : "No" )}{';'}" );
            this.ConnectionString = connectionStringBuilder.ToString();
        }

        [NotNull]
        public String[] GetColumnsList( [CanBeNull] String? worksheet ) {
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
                    columns[ i ] = ( String ) tableColumns.Rows[ i ][ "COLUMN_NAME" ];
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

        [NotNull]
        public DataTable GetWorksheet( [CanBeNull] String? worksheet ) {
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
                    worksheets[ i ] = ( String ) tableWorksheets.Rows[ i ][ "TABLE_NAME" ];
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