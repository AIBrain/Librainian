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
// Contact me by email if you have any questions or helpful criticism.
// 
// "Librainian 2015/LocalDB.cs" was last cleaned by Rick on 2015/02/15 at 7:00 AM
#endregion

namespace Librainian.Database {
	using System;
	using System.Data.SqlClient;
	using System.Threading.Tasks;
	using IO;
	using JetBrains.Annotations;
	using Measurement.Time;
	using Threading;

	public class LocalDB : IDisposable {

		[NotNull]
		public String ConnectionString { get; }

		[NotNull]
		public String DatabaseName { get; }

		[NotNull]
		public Folder DatabaseLocation { get; }

		[NotNull]
		public Document DatabaseMdf { get; }

		[NotNull]
		public Document DatabaseLog { get; }

		[NotNull]
		public SqlConnection Connection { get; }

		public Span ReadTimeout { get; }
		public Span WriteTimeout { get; }

		/// <summary>
		/// 
		/// </summary>
		/// <param name="databaseName"></param>
		/// <param name="databaseLocation"></param>
		/// <param name="timeoutForReads"></param>
		/// <param name="timeoutForWrites"></param>
		public LocalDB( [NotNull] String databaseName, [NotNull] Folder databaseLocation, Span timeoutForReads, Span timeoutForWrites ) {
			if ( String.IsNullOrWhiteSpace( databaseName ) ) {
				throw new ArgumentNullException( nameof( databaseName ) );
			}
			if ( databaseLocation == null ) {
				throw new ArgumentNullException( nameof( databaseLocation ) );
			}

			"Building SQL connection string...".Info();

			this.DatabaseName = databaseName;

			this.DatabaseLocation = databaseLocation;
			this.DatabaseLocation.Create();

			this.DatabaseMdf = new Document( this.DatabaseLocation, $"{this.DatabaseName}.mdf" );
			this.DatabaseLog = new Document( this.DatabaseLocation, $"{this.DatabaseName}.ldf" );

			this.ReadTimeout = timeoutForReads;
			this.WriteTimeout = timeoutForWrites;

			this.ConnectionString = String.Format( @"Data Source=(localdb)\v12.0;Integrated Security=True;MultipleActiveResultSets=True;" );	//AttachDBFileName={0};	, this.DatabaseMdf.FullPathWithFileName

		    // ReSharper disable once UseObjectOrCollectionInitializer
		    this.Connection = new SqlConnection( this.ConnectionString );
		    this.Connection.InfoMessage += ( sender, args ) => args.Message.Info();
		    this.Connection.StateChange += ( sender, args ) => $"{args.OriginalState} -> {args.CurrentState}".Info();
		    this.Connection.Disposed += ( sender, args ) => $"Disposing SQL connection {args}".Info();

		    $"Attempting connection to {this.DatabaseMdf}...".Info();

			this.Connection.Open();
			this.Connection.ServerVersion.Info();
		}

		public void Dispose() => this.DetachDatabaseAsync().Wait( ReadTimeout + WriteTimeout );

		public async Task DetachDatabaseAsync() {
			try {
				await this.Connection.OpenAsync();

				using (var cmd = this.Connection.CreateCommand()) {
					cmd.CommandText = String.Format( "ALTER DATABASE {0} SET SINGLE_USER WITH ROLLBACK IMMEDIATE; exec sp_detach_db '{0}'", this.DatabaseName );
					await cmd.ExecuteNonQueryAsync();
				}

			}
			catch ( SqlException exception ) {
				exception.More();
			}
		}
		//rename to InitDatabase?
		/*
				public SqlConnection Init() {

					//// If the database does not already exist, create it.
					//var connectionString = String.Format( @"Data Source=(LocalDB)\v11.0;Integrated Security=True" );	//Initial Catalog=master;
					//using (var connection = new SqlConnection( connectionString )) {
					//	connection.Open();
					//	var cmd = connection.CreateCommand();
					//	this.DetachDatabase();
					//	cmd.CommandText = String.Format( "CREATE DATABASE {0} ON (NAME = N'{0}', FILENAME = '{1}')", this.DatabaseName, this.DatabaseMdf );
					//	cmd.ExecuteNonQuery();
					//}

					// Open newly created, or old database.
					return new SqlConnection( this.ConnectionString );
				}
		*/



		/*
				public static SqlConnection GetLocalDB( string dbName, bool deleteIfExists = false ) {
					try {
						var outputFolder = Path.Combine( Path.GetDirectoryName( Assembly.GetExecutingAssembly().Location ), DB_DIRECTORY );
						var mdfFilename = dbName + ".mdf";
						var dbFileName = Path.Combine( outputFolder, mdfFilename );
						var logFileName = Path.Combine( outputFolder, String.Format( "{0}_log.ldf", dbName ) );
						// Create Data Directory If It Doesn't Already Exist.
						if ( !Directory.Exists( outputFolder ) ) {
							Directory.CreateDirectory( outputFolder );
						}

						// If the file exists, and we want to delete old data, remove it here and create a new database.
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
		*/

	}

	///// <summary>
	/////     work in progress. reiventing the same damn wheel. again."
	///// </summary>
	//public static class LocalDB {

	//	/// <summary>
	//	/// 
	//	/// </summary>
	//	public static ISqlLocalDbProvider Provider { get; } = new SqlLocalDbProvider();

	//	/// <summary>
	//	/// 
	//	/// </summary>
	//	private static Lazy<Folder> datebaseBaseFolder = new Lazy<Folder>();

	//	//public static readonly ConcurrentDictionary<String, Document> DataPointers = new ConcurrentDictionary<String, Document>();
	//	//public static readonly ConcurrentDictionary<String, Document> LogPointers = new ConcurrentDictionary<String, Document>();

	//	static LocalDB() {
	//		//const string name = "Properties";
	//		//var instance = GetInstance( name );

	//		//var mdf = new Document( Path.Combine( PersistenceExtensions.DataFolder.Value.FullName, String.Format( "{0}.mdf", name ) ) );
	//		//var ldf = new Document( Path.Combine( PersistenceExtensions.DataFolder.Value.FullName, String.Format( "{0}.ldf", name ) ) );

	//		//var list = new[ ] { mdf, ldf }.ToList();
	//		//InstanceFiles[ name ].AddRange( list );

	//		//Builders[ name ].SetPhysicalFileName( mdf.FullPathWithFileName );

	//		//instance.Start();
	//	}

	//	/// <summary>
	//	///  Instance names in SQL Local DB are case-insensitive
	//	/// </summary>
	//	/// <param name="name"></param>
	//	/// <param name="where"></param>
	//	/// <returns></returns>
	//	public static Boolean Start( [CanBeNull] String name, Folder where ) {
	//		if ( String.IsNullOrWhiteSpace( name ) ) {
	//			return false;
	//		}

	//		try {
	//			var localDbInstance = Provider.CreateInstance( name );
	//			var connectionStringBuilder = localDbInstance.CreateConnectionStringBuilder();
	//			connectionStringBuilder.SetPhysicalFileName();
	//               localDbInstance.Start();

	//			return true;

	//		}
	//		catch ( Exception) {
	//			return false;
	//		}
	//	}

	//	///// <summary>
	//	///// 
	//	///// </summary>
	//	//public static ConcurrentDictionary<string, ISqlLocalDbInstance> Instances { get; }
	//	//= new ConcurrentDictionary<String, ISqlLocalDbInstance>();

	//	///// <summary>
	//	///// 
	//	///// </summary>
	//	//public static ConcurrentSet<ConcurrentList<Document>> InstanceFiles { get; }
	//	//= new ConcurrentSet<ConcurrentList<Document>>();

	//	///// <summary>
	//	///// 
	//	///// </summary>
	//	//public static ConcurrentDictionary<string, DbConnectionStringBuilder> Builders { get; }
	//	//= new ConcurrentDictionary<String, DbConnectionStringBuilder>();

	//	//[CanBeNull]
	//	//public static ISqlLocalDbInstance Instance {
	//	//    get {
	//	//        return instance;
	//	//    }
	//	//    set {
	//	//        value.Should().NotBeNull();
	//	//        instance = value;
	//	//        if ( null != instance ) {
	//	//            instance.Start();
	//	//            OutputFolder = Path.Combine( Path.GetDirectoryName( Assembly.GetExecutingAssembly().Location ), DatabaseDirectory );
	//	//            var mdfFilename = String.Format( "{0}.mdf", DatabaseName );
	//	//            DatabaseMdfPath = Path.Combine( OutputFolder, mdfFilename );
	//	//            DatabaseLogPath = Path.Combine( OutputFolder, String.Format( "{0}_log.ldf", DatabaseName ) );

	//	//        }
	//	//    }
	//	//}

	//	public static object DatabaseName { get; private set; }
	//	public static String OutputFolder { get; set; }

	//	[NotNull]
	//	public static ISqlLocalDbInstance GetInstance( this String instanceName ) {
	//		ISqlLocalDbInstance result;
	//		if ( Instances.TryGetValue( instanceName, out result ) ) {
	//			return result;
	//		}
	//		Instances[ instanceName ] = Provider.GetOrCreateInstance( instanceName );
	//		result = Instances[ instanceName ];
	//		result.Start();
	//		return result;
	//	}

	//	[NotNull]
	//	public static DbConnectionStringBuilder GetConnectionStringBuilder( this String instanceName ) {
	//		DbConnectionStringBuilder result;
	//		if ( Builders.TryGetValue( instanceName, out result ) ) {
	//			return result;
	//		}

	//		Builders[ instanceName ] = GetInstance( instanceName ).CreateConnectionStringBuilder();

	//		return Builders[ instanceName ];
	//	}

	//	//private static Lazy<ISqlLocalDbInstance> instanceLazy = new Lazy<ISqlLocalDbInstance>( () => Instance );

	//	public static Boolean TryPut<TData>( String genericThingHere ) => false;

	//	public static Boolean TryGet<TData>( String genericThingHere, out TData result ) {
	//		//get data from localdb?
	//		//how?
	//		result = default(TData);
	//		return false;
	//	}
	//}
}
