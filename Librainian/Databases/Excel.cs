﻿// Copyright © 2020 Protiguous. All Rights Reserved.
// 
// This entire copyright notice and license must be retained and must be kept visible in any binaries, libraries, repositories, and source code (directly or derived)
// from our binaries, libraries, projects, or solutions.
// 
// This source code contained in "Excel.cs" belongs to Protiguous@Protiguous.com unless otherwise specified or the original license has been overwritten
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
// Project: "Librainian", File: "Excel.cs" was last formatted by Protiguous on 2020/03/18 at 10:23 AM.

namespace Librainian.Databases {

    using System;
    using System.Data;
    using System.Data.OleDb;
    using JetBrains.Annotations;
    using Logging;

    public class Excel {

        [NotNull]
        private String ConnectionString { get; }

        private String Path { get; }

        public Excel( [CanBeNull] String? path, Boolean hasHeaders, Boolean hasMixedData ) {
            this.Path = path;

            var strBuilder = new OleDbConnectionStringBuilder {
                Provider = "Microsoft.Jet.OLEDB.4.0", DataSource = this.Path
            };

            strBuilder.Add( "Extended Properties", String.Format( "Excel 8.0;HDR={0}{1}Imex={2}{1}", hasHeaders ? "Yes" : "No", ';', hasMixedData ? "2" : "0" ) );

            this.ConnectionString = strBuilder.ToString();
        }

        [NotNull]
        public String[] GetColumnsList( [CanBeNull] String? worksheet ) {
            var columns = Array.Empty<String>();

            try {

                using var connection = new OleDbConnection( this.ConnectionString );

                connection.Open();

                var tableColumns = connection.GetSchema( "Columns", new[] {
                    null, null, worksheet + '$', null
                } );

                columns = new String[ tableColumns.Rows.Count ];

                for ( var i = 0; i < columns.Length; i++ ) {
                    columns[ i ] = ( String ) tableColumns.Rows[ i ][ "COLUMN_NAME" ];
                }
            }
            catch ( OleDbException exception ) {
                exception.Log();
            }

            return columns;
        }

        [CanBeNull]
        public DataSet GetWorkplace() {
            try {
                using var connection = new OleDbConnection( this.ConnectionString );

                using var adaptor = new OleDbDataAdapter( "SELECT * FROM *", connection );

                var workplace = new DataSet();
                adaptor.FillSchema( workplace, SchemaType.Source );
                adaptor.Fill( workplace );

                return workplace;
            }
            catch ( OleDbException exception ) {
                exception.Log();
            }

            return null;
        }

        [CanBeNull]
        public DataTable GetWorksheet( [CanBeNull] String? worksheet ) {
            try {
                using var connection = new OleDbConnection( this.ConnectionString );

                using var adaptor = new OleDbDataAdapter( $"SELECT * FROM [{worksheet}$]", connection ) {
                    SelectCommand = new OleDbCommand( worksheet )
                };

                using var ws = new DataTable( worksheet );

                adaptor.FillSchema( ws, SchemaType.Source );
                adaptor.Fill( ws );

                return ws;
            }
            catch ( OleDbException exception ) {
                exception.Log();
            }

            return null;
        }

        [NotNull]
        public String[] GetWorksheetList() {
            var worksheets = Array.Empty<String>();

            try {

                using var connection = new OleDbConnection( this.ConnectionString );

                connection.Open();
                var tableWorksheets = connection.GetSchema( "Tables" );

                worksheets = new String[ tableWorksheets.Rows.Count ];

                for ( var i = 0; i < worksheets.Length; i++ ) {
                    worksheets[ i ] = ( String ) tableWorksheets.Rows[ i ][ "TABLE_NAME" ];
                    worksheets[ i ] = worksheets[ i ].Remove( worksheets[ i ].Length - 1 ).Trim( '"', '\'' );

                    // removes the trailing $ and other characters appended in the table name
                    while ( worksheets[ i ].EndsWith( "$", StringComparison.OrdinalIgnoreCase ) ) {
                        worksheets[ i ] = worksheets[ i ].Remove( worksheets[ i ].Length - 1 ).Trim( '"', '\'' );
                    }
                }
            }
            catch ( OleDbException exception ) {
                exception.Log();
            }

            return worksheets;
        }

    }

}