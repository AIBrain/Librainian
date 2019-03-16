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
//     paypal@AIBrain.Org
//     (We're still looking into other solutions! Any ideas?)
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
// Feel free to browse any source code we *might* make available.
// 
// Project: "Librainian", "LocalDB.cs" was last formatted by Protiguous on 2019/01/29 at 10:49 PM.

namespace Librainian.Database {

    using System;
    using System.Data;
    using System.Data.Common;
    using System.Data.SqlClient;
    using System.Threading.Tasks;
    using System.Windows.Forms;
    using JetBrains.Annotations;
    using Logging;
    using Magic;
    using Measurement.Time;
    using OperatingSystem.FileSystem;

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

            if ( databaseLocation == null ) {
                databaseLocation = new Folder( Environment.SpecialFolder.LocalApplicationData, Application.ProductName );
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

                using ( var cmd = this.Connection.CreateCommand() ) {
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

    ///// <summary>
    /////     work in progress. reiventing the same damn wheel. again."
    ///// </summary>
    //public static class LocalDB {
    // ///
    // <summary>/// ///</summary>
    // public static ISqlLocalDbProvider Provider { get; } = new SqlLocalDbProvider();

    // /// <summary> /// /// </summary> private static Lazy<Folder> datebaseBaseFolder = new Lazy<Folder>();

    // //public static  ConcurrentDictionary<String, Document> DataPointers = new ConcurrentDictionary<String, Document>(); //public static  ConcurrentDictionary<String, Document> LogPointers = new
    // ConcurrentDictionary<String, Document>();

    // static LocalDB() { //const string name = "Properties"; //var instance = GetInstance( name ); //var mdf = new Document( Path.Combine( PersistenceExtensions.DataFolder.Value.FullName, String.Format( "{0}.mdf", name )
    // ) ); //var ldf = new Document( Path.Combine( PersistenceExtensions.DataFolder.Value.FullName, String.Format( "{0}.ldf", name ) ) );

    // //var list = new[ ] { mdf, ldf }.ToList(); //InstanceFiles[ name ].AddRange( list );

    // //Builders[ name ].SetPhysicalFileName( mdf.FullPathWithFileName );

    // //instance.Start(); }

    // ///
    // <summary>
    // /// Instance names in SQL Local DB are case-insensitive ///
    // </summary>
    // ///
    // <param name="name"> </param>
    // ///
    // <param name="where"></param>
    // ///
    // <returns></returns>
    // public static Boolean Start( [CanBeNull] String name, Folder where ) { if ( String.IsNullOrWhiteSpace( name ) ) { return false; }

    // try { var localDbInstance = Provider.CreateInstance( name ); var connectionStringBuilder = localDbInstance.CreateConnectionStringBuilder(); connectionStringBuilder.SetPhysicalFileName(); localDbInstance.Start();

    // return true;

    // } catch ( Exception) { return false; } }

    // ///// <summary> ///// ///// </summary> //public static ConcurrentDictionary<string, ISqlLocalDbInstance> Instances { get; } //= new ConcurrentDictionary<String, ISqlLocalDbInstance>();

    // ///// <summary> ///// ///// </summary> //public static ConcurrentSet<ConcurrentList<Document>> InstanceFiles { get; } //= new ConcurrentSet<ConcurrentList<Document>>();

    // ///// <summary> ///// ///// </summary> //public static ConcurrentDictionary<string, DbConnectionStringBuilder> Builders { get; } //= new ConcurrentDictionary<String, DbConnectionStringBuilder>();

    // //[CanBeNull] //public static ISqlLocalDbInstance Instance { // get { // return instance; // } // set { // value.Should().NotBeNull(); // instance = value; // if ( null != instance ) { // instance.Start(); //
    // OutputFolder = Path.Combine( Path.GetDirectoryName( Assembly.GetExecutingAssembly().Location ), DatabaseDirectory ); // var mdfFilename = String.Format( "{0}.mdf", DatabaseName ); // DatabaseMdfPath = Path.Combine(
    // OutputFolder, mdfFilename ); // DatabaseLogPath = Path.Combine( OutputFolder, String.Format( "{0}_log.ldf", DatabaseName ) );

    // // } // } //}

    // public static object DatabaseName { get; private set; } public static String OutputFolder { get; set; }

    // [NotNull] public static ISqlLocalDbInstance GetInstance( this String instanceName ) { ISqlLocalDbInstance result; if ( Instances.TryGetValue( instanceName, out result ) ) { return result; } Instances[ instanceName
    // ] = Provider.GetOrCreateInstance( instanceName ); result = Instances[ instanceName ]; result.Start(); return result; }

    // [NotNull] public static DbConnectionStringBuilder GetConnectionStringBuilder( this String instanceName ) { DbConnectionStringBuilder result; if ( Builders.TryGetValue( instanceName, out result ) ) { return result; }

    // Builders[ instanceName ] = GetInstance( instanceName ).CreateConnectionStringBuilder();

    // return Builders[ instanceName ]; }

    // //private static Lazy<ISqlLocalDbInstance> instanceLazy = new Lazy<ISqlLocalDbInstance>( ()
    // => Instance );

    // public static Boolean TryPut<TData>( String genericThingHere ) => false;

    //	public static Boolean TryGet<TData>( String genericThingHere, out TData result ) {
    //		//get data from localdb?
    //		//how?
    //		result = default(TData);
    //		return false;
    //	}
    //}

}