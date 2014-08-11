#region License & Information
// This notice must be kept visible in the source.
// 
// This section of source code belongs to Rick@AIBrain.Org unless otherwise specified,
// or the original license has been overwritten by the automatic formatting of this code.
// Any unmodified sections of source code borrowed from other projects retain their original license and thanks goes to the Authors.
// 
// Donations and Royalties can be paid via
// PayPal: paypal@aibrain.org
// bitcoin:1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
// bitcoin:1NzEsF7eegeEWDr5Vr9sSSgtUC4aL6axJu
// litecoin:LeUxdU2w3o6pLZGVys5xpDZvvo8DUrjBp9
// 
// Usage of the source code or compiled binaries is AS-IS.
// I am not responsible for Anything You Do.
// 
// "Librainian/LocalDB.cs" was last cleaned by Rick on 2014/08/11 at 12:37 AM
#endregion

namespace Librainian.Database {
    using System;
    using System.Data.SqlClient;
    using System.IO;
    using System.Reflection;

    internal static class LocalDB {
        public const string DBDirectory = "Data";

        public static Boolean CreateDatabase( string dbName, string dbFileName ) {
            try {
                var connectionString = String.Format( @"Data Source=(LocalDB)\v11.0;Initial Catalog=master;Integrated Security=True" );
                using ( var connection = new SqlConnection( connectionString ) ) {
                    connection.Open();
                    var cmd = connection.CreateCommand();

                    DetachDatabase( dbName );

                    cmd.CommandText = String.Format( "CREATE DATABASE {0} ON (NAME = N'{0}', FILENAME = '{1}')", dbName, dbFileName );
                    cmd.ExecuteNonQuery();
                }

                return File.Exists( dbFileName );
            }
            catch {
                throw;
            }
        }

        public static Boolean DetachDatabase( string dbName ) {
            try {
                var connectionString = String.Format( @"Data Source=(LocalDB)\v11.0;Initial Catalog=master;Integrated Security=True" );
                using ( var connection = new SqlConnection( connectionString ) ) {
                    connection.Open();
                    var cmd = connection.CreateCommand();
                    cmd.CommandText = String.Format( "exec sp_detach_db '{0}'", dbName );
                    cmd.ExecuteNonQuery();

                    return true;
                }
            }
            catch {
                return false;
            }
        }

        public static SqlConnection GetLocalDB( string dbName, Boolean deleteIfExists = false ) {
            try {
                //TODO use DirectoryInfo instead of strings.
                var ass = Path.GetDirectoryName( Assembly.GetExecutingAssembly().Location ) ?? String.Empty;
                var outputFolder = Path.Combine( ass, DBDirectory );
                var mdfFilename = dbName + ".mdf";
                var dbFileName = Path.Combine( outputFolder, mdfFilename );
                var logFileName = Path.Combine( outputFolder, String.Format( "{0}_log.ldf", dbName ) );

                // Create Data Directory If It Doesn't Already Exist.
                if ( !Directory.Exists( outputFolder ) ) {
                    Directory.CreateDirectory( outputFolder );
                }

                // If the file exists, and we want to delete old data, remove it here and create a
                // new database.
                if ( File.Exists( dbFileName ) && deleteIfExists ) {
                    if ( File.Exists( logFileName ) ) {
                        File.Delete( logFileName );
                    }
                    File.Delete( dbFileName );
                    CreateDatabase( dbName, dbFileName );
                }

                    // If the database does not already exist, create it.
                else if ( !File.Exists( dbFileName ) ) {
                    CreateDatabase( dbName, dbFileName );
                }

                // Open newly created, or old database.
                var connectionString = String.Format( @"Data Source=(LocalDB)\v11.0;AttachDBFileName={1};Initial Catalog={0};Integrated Security=True;", dbName, dbFileName );
                var connection = new SqlConnection( connectionString );
                connection.Open();
                return connection;
            }
            catch {
                throw;
            }
        }
    }
}
