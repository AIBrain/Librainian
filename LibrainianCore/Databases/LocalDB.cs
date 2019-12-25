// Copyright © Rick@AIBrain.org and Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
//
// This source code contained in "LocalDB.cs" belongs to Protiguous@Protiguous.com and
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
// Project: "Librainian", "LocalDB.cs" was last formatted by Protiguous on 2019/12/10 at 7:07 AM.

namespace LibrainianCore.Databases {

    using System;
    using System.Data;
    using System.Data.Common;
    using System.Diagnostics.CodeAnalysis;
    using System.Net.Mime;
    using System.Threading.Tasks;
    using Logging;
    using Measurement.Time;
    using OperatingSystem.FileSystem;
    using Utilities;

    public class LocalDb : ABetterClassDispose {

        [NotNull]
        public SqlConnection Connection { get; }

        [NotNull]
        public String ConnectionString { get; }

        [NotNull]
        public Folder DatabaseLocation { get; }

        [NotNull]
        public Document DatabaseLog { get; }

        [NotNull]
        public Document DatabaseMdf { get; }

        [NotNull]
        public String DatabaseName { get; }

        public TimeSpan ReadTimeout { get; }

        public TimeSpan WriteTimeout { get; }

        public LocalDb( [NotNull] String databaseName, [CanBeNull] Folder databaseLocation = null, TimeSpan? timeoutForReads = null, TimeSpan? timeoutForWrites = null ) {
            if ( String.IsNullOrWhiteSpace( databaseName ) ) {
                throw new ArgumentNullException( nameof( databaseName ) );
            }

            if ( databaseLocation is null ) {
                databaseLocation = new Folder( Environment.SpecialFolder.LocalApplicationData, MediaTypeNames.Application.ProductName );
            }

            this.ReadTimeout = timeoutForReads.GetValueOrDefault( Seconds.Thirty );
            this.WriteTimeout = timeoutForWrites.GetValueOrDefault( this.ReadTimeout + Seconds.Thirty );

            this.DatabaseName = databaseName;

            this.DatabaseLocation = databaseLocation;

            if ( !this.DatabaseLocation.Exists() ) {
                this.DatabaseLocation.Create();
                this.DatabaseLocation.Info.SetCompression( false );
            }

            "Building SQL connection string...".Info();

            this.DatabaseMdf = new Document( this.DatabaseLocation, $"{this.DatabaseName}.mdf" );
            this.DatabaseLog = new Document( this.DatabaseLocation, $"{this.DatabaseName}_log.ldf" ); //TODO does localdb even use a log file?

            this.ConnectionString = @"Data Source=(localdb)\MSSQLLocalDB;Integrated Security=True;Initial Catalog=master;Integrated Security=True;";

            if ( this.DatabaseMdf.Exists() == false ) {
                using ( var connection = new SqlConnection( this.ConnectionString ) ) {
                    connection.Open();
                    var command = connection.CreateCommand();
                    command.CommandText = String.Format( "CREATE DATABASE {0} ON (NAME = N'{0}', FILENAME = '{1}')", this.DatabaseName, this.DatabaseMdf.FullPath );
                    command.ExecuteNonQuery();
                }
            }

            this.ConnectionString =
                $@"Data Source=(localdb)\MSSQLLocalDB;Integrated Security=True;Initial Catalog={this.DatabaseName};AttachDBFileName={this.DatabaseMdf.FullPath};";

            this.Connection = new SqlConnection( this.ConnectionString );

            this.Connection.Disposed += ( sender, args ) => $"Disposing SQL connection {args}".Info();

            this.Connection.StateChange += ( sender, args ) => $"{args.OriginalState} -> {args.CurrentState}".Info();

            this.Connection.InfoMessage += ( sender, args ) => args.Message.Info();

            $"Attempting connection to {this.DatabaseMdf}...".Info();
            this.Connection.Open();
            this.Connection.ServerVersion.Info();
            this.Connection.Close();
        }

        public async Task DetachDatabaseAsync() {
            try {
                if ( this.Connection.State == ConnectionState.Closed ) {
                    await this.Connection.OpenAsync().ConfigureAwait( false );
                }

                using var cmd = this.Connection.CreateCommand();

                if ( cmd != null ) {
                    cmd.CommandText = String.Format( "ALTER DATABASE {0} SET SINGLE_USER WITH ROLLBACK IMMEDIATE; exec sp_detach_db N'{0}'", this.DatabaseName );
                    await cmd.ExecuteNonQueryAsync().ConfigureAwait( false );
                }
            }
            catch ( SqlException exception ) {
                exception.Log();
            }
            catch ( DbException exception ) {
                exception.Log();
            }
        }

        public override void DisposeManaged() => this.DetachDatabaseAsync().Wait( this.ReadTimeout + this.WriteTimeout );
    }
}