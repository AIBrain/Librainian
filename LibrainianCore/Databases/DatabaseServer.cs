// Copyright © Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
//
// This source code contained in "DatabaseServer.cs" belongs to Protiguous@Protiguous.com
// unless otherwise specified or the original license has been overwritten by formatting.
// (We try to avoid it from happening, but it does accidentally happen.)
//
// Any unmodified portions of source code gleaned from other projects still retain their original
// license and our thanks goes to those Authors. If you find your code in this source code, please
// let us know so we can properly attribute you and include the proper license and/or copyright.
//
// If you want to use any of our code in a commercial project, you must contact
// Protiguous@Protiguous.com for permission and a quote.
//
// Donations are accepted (for now) via
//     bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//     PayPal: Protiguous@Protiguous.com
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
// Project: "Librainian", "DatabaseServer.cs" was last formatted by Protiguous on 2020/01/31 at 12:24 AM.

namespace LibrainianCore.Databases {

    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Common;
    using System.Data.SqlClient;
    using System.Diagnostics;
    using System.Linq;
    using System.Reflection;
    using System.Threading;
    using System.Threading.Tasks;
    using Collections.Extensions;
    using Collections.Sets;
    using Converters;
    using Internet;
    using JetBrains.Annotations;
    using Logging;
    using Maths;
    using Measurement.Time;
    using OperatingSystem.FileSystem.Pri.LongPath;
    using Parsing;
    using Utilities;

    public class DatabaseServer : ABetterClassDispose, IDatabase {

        public TimeSpan CommandTimeout { get; set; } = Minutes.Ten;

        [CanBeNull]
        public String Sproc { get; set; }

        public Int32? ExecuteNonQuery( [NotNull] String query, Int32 retries, CommandType commandType, [CanBeNull] params SqlParameter[] parameters ) {
            if ( String.IsNullOrWhiteSpace( value: query ) ) {
                throw new ArgumentException( message: "Value cannot be null or whitespace.", nameof( query ) );
            }

            this.Sproc = query;

            TryAgain:
            --retries;

            try {
                using var connection = new SqlConnection( this.ConnectionString );

                using var command = new SqlCommand {
                    Connection = connection,
                    CommandTimeout = ( Int32 )this.CommandTimeout.TotalSeconds,
                    CommandType = commandType,
                    CommandText = query
                };

                command.PopulateParameters( parameters );

                connection.Open();

                return command.ExecuteNonQuery();
            }
            catch ( InvalidOperationException exception ) {
                exception.Log( Rebuild( query, parameters ) );

                if ( retries.Any() ) {
                    goto TryAgain;
                }
            }
            catch ( SqlException exception ) {
                exception.Log( Rebuild( query, parameters ) );

                if ( retries.Any() ) {
                    goto TryAgain;
                }
            }
            catch ( DbException exception ) {
                exception.Log( Rebuild( query, parameters ) );

                if ( retries.Any() ) {
                    goto TryAgain;
                }
            }

            return default;
        }

        /// <summary>Opens and then closes a <see cref="SqlConnection" />.</summary>
        /// <returns></returns>
        public Int32? ExecuteNonQuery( [NotNull] String query, CommandType commandType, [CanBeNull] params SqlParameter[] parameters ) {
            if ( String.IsNullOrWhiteSpace( value: query ) ) {
                throw new ArgumentException( message: "Value cannot be null or whitespace.", nameof( query ) );
            }

            this.Sproc = query;

            try {
                using var connection = new SqlConnection( this.ConnectionString );

                using var command = new SqlCommand( query, connection ) {
                    CommandType = commandType,
                    CommandTimeout = ( Int32 )this.CommandTimeout.TotalSeconds
                };

                command.PopulateParameters( parameters );

                connection.Open();

                return command.ExecuteNonQuery();
            }
            catch ( SqlException exception ) {
                exception.Log( Rebuild( query, parameters ) );
            }
            catch ( DbException exception ) {
                exception.Log( Rebuild( query, parameters ) );
            }

            return default;
        }

        [ItemCanBeNull]
        public async Task<Int32?> ExecuteNonQueryAsync( [NotNull] String query, CommandType commandType, [CanBeNull] params SqlParameter[] parameters ) {
            this.Sproc = query ?? throw new ArgumentNullException( nameof( query ) );

            try {
                using var connection = new SqlConnection( this.ConnectionString );

                using var command = new SqlCommand( query, connection ) {
                    CommandType = commandType,
                    CommandTimeout = ( Int32 )this.CommandTimeout.TotalSeconds
                };

                command.PopulateParameters( parameters );

                using var open = connection.OpenAsync( this.Token );

                await open.ConfigureAwait( false );

                return await command.ExecuteNonQueryAsync( this.Token ).ConfigureAwait( false );
            }
            catch ( SqlException exception ) {
                if ( !exception.SQLTimeout() ) {
                    exception.Log( Rebuild( query, parameters ) );
                }
            }
            catch ( DbException exception ) {
                exception.Log( Rebuild( query, parameters ) );
            }

            return default;
        }

        /// <summary>Returns a <see cref="DataTable" /></summary>
        /// <param name="query">      </param>
        /// <param name="commandType"></param>
        /// <param name="table">      </param>
        /// <param name="parameters"> </param>
        /// <returns></returns>
        public Boolean ExecuteReader( String query, CommandType commandType, [NotNull] out DataTable table, [CanBeNull] params SqlParameter[] parameters ) {
            if ( query.IsNullOrWhiteSpace() ) {
                throw new ArgumentNullException( nameof( query ) );
            }

            this.Sproc = query;

            table = new DataTable();

            try {

                using var connection = new SqlConnection( this.ConnectionString );

                using var command = new SqlCommand( query, connection ) {
                    CommandType = commandType,
                    CommandTimeout = ( Int32 )this.CommandTimeout.TotalSeconds
                };

                command.PopulateParameters( parameters );

                connection.Open();

                using var bob = command.ExecuteReader();

                if ( bob != null ) {
                    table.BeginLoadData();
                    table.Load( bob );
                    table.EndLoadData();
                }

                return true;
            }
            catch ( SqlException exception ) {
                exception.Log( Rebuild( query, parameters ) );
            }
            catch ( DbException exception ) {
                exception.Log( Rebuild( query, parameters ) );
            }

            return default;
        }

        /// <param name="query">      </param>
        /// <param name="commandType"></param>
        /// <param name="parameters"> </param>
        [ItemCanBeNull]
        public async Task<DataTableReader> ExecuteReaderAsyncDataReader( [NotNull] String query, CommandType commandType, [CanBeNull] params SqlParameter[] parameters ) {
            if ( String.IsNullOrWhiteSpace( value: query ) ) {
                throw new ArgumentException( message: "Value cannot be null or whitespace.", nameof( query ) );
            }

            this.Sproc = query;

            try {

                using var connection = new SqlConnection( this.ConnectionString );

                using var command = new SqlCommand( query, connection ) {
                    CommandType = commandType,
                    CommandTimeout = ( Int32 )this.CommandTimeout.TotalSeconds
                };

                command.PopulateParameters( parameters );

                using var open = connection.OpenAsync( this.Token );
                await open.ConfigureAwait( false );
                using var reader = command.ExecuteReaderAsync( this.Token );

                if ( reader != null ) {
                    using var readerAsync = await reader.ConfigureAwait( false );
                    using var table = readerAsync.ToDataTable();

                    return table.CreateDataReader();
                }
            }
            catch ( SqlException exception ) {
                exception.Log( Rebuild( query, parameters ) );
            }
            catch ( DbException exception ) {
                exception.Log( Rebuild( query, parameters ) );
            }

            return default;
        }

        /// <summary>Returns a <see cref="DataTable" /></summary>
        /// <param name="query">      </param>
        /// <param name="commandType"></param>
        /// <param name="parameters"> </param>
        /// <returns></returns>
        [ItemNotNull]
        public async Task<DataTable> ExecuteReaderDataTableAsync( [NotNull] String query, CommandType commandType, [CanBeNull] params SqlParameter[] parameters ) {
            if ( String.IsNullOrWhiteSpace( value: query ) ) {
                throw new ArgumentException( message: "Value cannot be null or whitespace.", nameof( query ) );
            }

            this.Sproc = query;

            var table = new DataTable();

            try {
                using var connection = new SqlConnection( this.ConnectionString );

                using var command = new SqlCommand( query, connection ) {
                    CommandType = commandType,
                    CommandTimeout = ( Int32 )this.CommandTimeout.TotalSeconds
                };

                command.PopulateParameters( parameters );

                using var open = connection.OpenAsync( this.Token );
                await open.ConfigureAwait( false );

                using var reader = command.ExecuteReaderAsync( this.Token );

                if ( reader != null ) {
                    table.BeginLoadData();
                    table.Load( await reader.ConfigureAwait( false ) );
                    table.EndLoadData();
                }
            }
            catch ( SqlException exception ) {
                exception.Log( Rebuild( query, parameters ) );
                table.Clear();
            }
            catch ( DbException exception ) {
                exception.Log( Rebuild( query, parameters ) );
                table.Clear();
            }

            return table;
        }

        /// <summary>
        ///     <para>Returns the first column of the first row.</para>
        /// </summary>
        /// <param name="query">      </param>
        /// <param name="commandType"></param>
        /// <param name="parameters"> </param>
        /// <returns></returns>
        [CanBeNull]
        public T ExecuteScalar<T>( String query, CommandType commandType, [CanBeNull] params SqlParameter[] parameters ) {
            if ( String.IsNullOrWhiteSpace( value: query ) ) {
                throw new ArgumentException( message: "Value cannot be null or whitespace.", nameof( query ) );
            }

            this.Sproc = query;

            try {
                using var connection = new SqlConnection( this.ConnectionString );

                using var command = new SqlCommand( query, connection ) {
                    CommandType = commandType,
                    CommandTimeout = ( Int32 )this.CommandTimeout.TotalSeconds
                };

                command.PopulateParameters( parameters );

                connection.Open();

                return command.ExecuteScalar().Cast<T>();
            }
            catch ( SqlException exception ) {
                exception.Log( Rebuild( query, parameters ) );

                throw;
            }
            catch ( DbException exception ) {
                exception.Log( Rebuild( query, parameters ) );

                throw;
            }
        }

        /// <summary>
        ///     <para>Returns the first column of the first row.</para>
        /// </summary>
        /// <param name="query">      </param>
        /// <param name="commandType"></param>
        /// <param name="parameters"> </param>
        /// <returns></returns>
        [ItemCanBeNull]
        public async Task<T> ExecuteScalarAsync<T>( String query, CommandType commandType, [CanBeNull] params SqlParameter[] parameters ) {
            if ( query.IsNullOrWhiteSpace() ) {
                throw new ArgumentNullException( nameof( query ) );
            }

            this.Sproc = query;

            try {
                using var connection = new SqlConnection( this.ConnectionString );

                using var command = new SqlCommand( query, connection ) {
                    CommandType = commandType,
                    CommandTimeout = ( Int32 )this.CommandTimeout.TotalSeconds
                };

                command.PopulateParameters( parameters );

                using var open = connection.OpenAsync( this.Token );
                await open.ConfigureAwait( false );

                using var run = command.ExecuteScalarAsync( this.Token );
                var scalar = await run.ConfigureAwait( false );

                return scalar.Cast<T>();
            }
            catch ( InvalidCastException exception ) {

                //TIP: check for SQLServer returning a Double when you expect a Single (float in SQL).
                exception.Log( Rebuild( query, parameters ) );

                throw;
            }
            catch ( SqlException exception ) {
                exception.Log( Rebuild( query, parameters ) );

                throw;
            }
            catch ( DbException exception ) {
                exception.Log( Rebuild( query, parameters ) );

                throw;
            }
        }


        /// <summary>
        ///     <para>Run a query, no rows expected to be read.</para>
        ///     <para>Does not catch any exceptions.</para>
        /// </summary>
        /// <param name="sproc"></param>
        /// <param name="commandType"></param>
        /// <param name="parameters"></param>
        public async Task NonQueryAsync( [NotNull] String sproc, CommandType commandType, [CanBeNull] params SqlParameter[] parameters ) {
            if ( String.IsNullOrWhiteSpace( value: sproc ) ) {
                throw new ArgumentException( message: "Value cannot be null or whitespace.", nameof( sproc ) );
            }

            this.Sproc = $"Executing SQL command {sproc}.";

            using var connection = new SqlConnection( this.ConnectionString );

            using var command = new SqlCommand {
                Connection = connection,
                CommandType = commandType,
                CommandTimeout = ( Int32 )this.CommandTimeout.TotalSeconds,
                CommandText = sproc
            };

            command.PopulateParameters( parameters );

            using var open = connection.OpenAsync( this.Token );
            await open.ConfigureAwait( false );
            await command.ExecuteNonQueryAsync( this.Token ).ConfigureAwait( false );
        }

        [NotNull]
        public DataTableReader QueryAdHoc( [NotNull] String sql, [CanBeNull] params SqlParameter[] parameters ) {
            if ( String.IsNullOrWhiteSpace( value: sql ) ) {
                throw new ArgumentException( message: "Value cannot be null or whitespace.", nameof( sql ) );
            }

            this.Sproc = $"Executing AdHoc SQL: {sql.DoubleQuote()}.";

            using var connection = new SqlConnection( this.ConnectionString );

            using var command = new SqlCommand {
                Connection = connection,
                CommandTimeout = ( Int32 )this.CommandTimeout.TotalSeconds,
                CommandType = CommandType.Text,
                CommandText = sql
            };

            command.PopulateParameters( parameters );

            connection.Open();

            using var reader = command.ExecuteReader();
            using var table = reader.ToDataTable();

            return table.CreateDataReader();
        }

        [NotNull]
        [ItemNotNull]
        public async Task<DataTableReader> QueryAdHocAsync( [NotNull] String sql, [CanBeNull] params SqlParameter[] parameters ) {
            if ( String.IsNullOrWhiteSpace( value: sql ) ) {
                throw new ArgumentException( message: "Value cannot be null or whitespace.", nameof( sql ) );
            }

            this.Sproc = $"Executing AdHoc SQL: {sql.DoubleQuote()}.";

            using var connection = new SqlConnection( this.ConnectionString );

            using var command = new SqlCommand {
                Connection = connection,
                CommandTimeout = ( Int32 )this.CommandTimeout.TotalSeconds,
                CommandType = CommandType.Text,
                CommandText = sql
            };

            command.PopulateParameters( parameters );

            using var open = connection.OpenAsync( this.Token );
            await open.ConfigureAwait( false );

            var execute = command.ExecuteReaderAsync( this.Token );

            if ( execute != null ) {
                using var reader = await execute.ConfigureAwait( false );
                using var table = reader.ToDataTable();

                return table.CreateDataReader();
            }

            using var blank = new DataTable();
            using var another = new DataTableReader( blank );

            return another;
        }

        /// <summary>Simplest possible database connection.
        /// <para>Connect and then run <paramref name="sproc" />.</para>
        /// </summary>
        /// <param name="sproc"></param>
        /// <param name="commandType"></param>
        /// <param name="parameters"></param>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        [ItemCanBeNull]
        public async Task<SqlDataReader> QueryAsync( [NotNull] String sproc, CommandType commandType, [CanBeNull] params SqlParameter[] parameters ) {
            if ( String.IsNullOrWhiteSpace( value: sproc ) ) {
                throw new ArgumentException( message: "Value cannot be null or whitespace.", nameof( sproc ) );
            }

            this.Sproc = sproc;

            using var connection = new SqlConnection( this.ConnectionString );

            using var command = new SqlCommand {
                Connection = connection,
                CommandType = commandType,
                CommandTimeout = ( Int32 )this.CommandTimeout.TotalSeconds,
                CommandText = sproc
            };

            command.PopulateParameters( parameters );

            using var open = connection.OpenAsync( this.Token );
            await open.ConfigureAwait( false );

            using var reader = command.ExecuteReaderAsync( this.Token );

            if ( reader != null ) {
                return await reader.ConfigureAwait( false );
            }

            return default;
        }

        /// <summary>Returns a <see cref="DataTable" /></summary>
        /// <param name="query">     </param>
        /// <param name="commandType"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        [CanBeNull]
        [ItemCanBeNull]
        public IEnumerable<TResult> QueryList<TResult>( String query, CommandType commandType, [CanBeNull] params SqlParameter[] parameters ) {
            if ( query.IsNullOrWhiteSpace() ) {
                throw new ArgumentNullException( nameof( query ) );
            }

            this.Sproc = query;

            try {
                using var connection = new SqlConnection( this.ConnectionString );

                using var command = new SqlCommand( query, connection ) {
                    CommandType = commandType,
                    CommandTimeout = ( Int32 )this.CommandTimeout.TotalSeconds
                };

                command.PopulateParameters( parameters );

                connection.Open();
                var reader = command.ExecuteReader();

                if ( reader != null ) {
                    return GenericPopulatorExtensions.CreateList<TResult>( reader );
                }
            }
            catch ( SqlException exception ) {
                exception.Log( Rebuild( query, parameters ) );
            }
            catch ( DbException exception ) {
                exception.Log( Rebuild( query, parameters ) );
            }

            return default;
        }

        public void UseDatabase( [NotNull] String dbName ) {
            if ( String.IsNullOrWhiteSpace( value: dbName ) ) {
                throw new ArgumentException( message: "Value cannot be null or whitespace.", nameof( dbName ) );
            }

            using ( var _ = this.QueryAdHoc( $"USE {dbName.Bracket()};" ) ) { }
        }

        public async Task UseDatabaseAsync( [NotNull] String dbName ) {
            if ( String.IsNullOrWhiteSpace( value: dbName ) ) {
                throw new ArgumentException( message: "Value cannot be null or whitespace.", nameof( dbName ) );
            }

            using ( var _ = await this.QueryAdHocAsync( $"USE {dbName.Bracket()};" ).ConfigureAwait( false ) ) { }
        }

        [NotNull]
        private String ConnectionString { get; }

        private CancellationToken Token { get; }

        /// <summary>The parameter collection for this database connection.</summary>
        [NotNull]
        internal ConcurrentHashset<SqlParameter> ParameterSet { get; } = new ConcurrentHashset<SqlParameter>();

        /// <summary>
        ///     <para>Create a database object to the specified server.</para>
        /// </summary>
        public DatabaseServer( [CanBeNull] SqlConnectionStringBuilder builder, [CanBeNull] String useDatabase = null, CancellationToken? token = default ) : this(
            builder?.ConnectionString, useDatabase, token ) { }

        /// <summary></summary>
        /// <param name="connectionString"></param>
        /// <param name="useDatabase"></param>
        /// <param name="token"></param>
        /// <exception cref="InvalidOperationException"></exception>
        public DatabaseServer( String connectionString, [CanBeNull] String useDatabase = null, CancellationToken? token = default ) {
            this.Token = token ?? CancellationToken.None;
            this.ConnectionString = connectionString ?? throw new ArgumentNullException( nameof( connectionString ) );

            useDatabase = useDatabase.Trimmed();

            if ( !String.IsNullOrWhiteSpace( useDatabase ) ) {

                var builder = new SqlConnectionStringBuilder( connectionString ) {
                    InitialCatalog = useDatabase.Bracket()
                };

                this.ConnectionString = builder.ConnectionString;
            }
        }

        [CanBeNull]
        public DataTableReader ExecuteDataReader( [NotNull] String query, CommandType commandType, [CanBeNull] params SqlParameter[] parameters ) {
            if ( String.IsNullOrWhiteSpace( value: query ) ) {
                throw new ArgumentException( message: "Value cannot be null or whitespace.", nameof( query ) );
            }

            this.Sproc = query;

            try {

                using var connection = new SqlConnection( this.ConnectionString );

                using var command = new SqlCommand( query, connection ) {
                    CommandType = commandType,
                    CommandTimeout = ( Int32 )this.CommandTimeout.TotalSeconds
                };

                command.PopulateParameters( parameters );

                connection.Open();

                using var reader = command.ExecuteReader( CommandBehavior.CloseConnection );

                if ( reader != null ) {
                    using var table = reader.ToDataTable();

                    return table.CreateDataReader();
                }
            }
            catch ( SqlException exception ) {
                exception.Log( Rebuild( query, parameters ) );
            }
            catch ( DbException exception ) {
                exception.Log( Rebuild( query, parameters ) );
            }

            return default;
        }

#if VERBOSE

        ~DatabaseServer() {
            $"Warning: We have an undisposed Database() connection somewhere. This could cause a memory leak. Query={this.Sproc.DoubleQuote()}".Log();
        }

#endif

        /// <summary>Dispose of any <see cref="IDisposable" /> (managed) fields or properties in this method.</summary>
        public override void DisposeManaged() { }

        [DebuggerStepThrough]
        [NotNull]
        private static String Rebuild( [NotNull] String query, [CanBeNull] IEnumerable<SqlParameter> parameters = null ) {
            if ( String.IsNullOrWhiteSpace( value: query ) ) {
                throw new ArgumentException( message: "Value cannot be null or whitespace.", nameof( query ) );
            }

            if ( parameters is null ) {
                return $"exec {query}";
            }

            return
                $"exec {query} {parameters.Where( parameter => !( parameter is null ) ).Select( parameter => $"{parameter.ParameterName}={parameter.Value?.ToString().SingleQuote() ?? String.Empty}" ).ToStrings( "," )}; ";
        }

        public static async Task<Boolean> CreateDatabase( [NotNull] String databaseName, [NotNull] String connectionString ) {
            databaseName = databaseName.Trimmed();

            if ( String.IsNullOrWhiteSpace( value: databaseName ) ) {
                throw new ArgumentException( message: "Value cannot be null or whitespace.", nameof( databaseName ) );
            }

            if ( String.IsNullOrWhiteSpace( value: connectionString ) ) {
                throw new ArgumentException( message: "Value cannot be null or whitespace.", nameof( connectionString ) );
            }

            try {
                using var db = new DatabaseServer( connectionString, useDatabase: "master" );

                await db.QueryAdHocAsync( $"create database {databaseName.Bracket()};" ).ConfigureAwait( false );

                return true;
            }
            catch ( SqlException exception ) {
                if ( !exception.SQLTimeout() ) {
                    exception.Log();
                }
            }

            return default;
        }



        [NotNull]
        public static SqlConnectionStringBuilder PopulateConnectionStringBuilder( [NotNull] String serverName, [NotNull] String instanceName, TimeSpan connectTimeout,
            [CanBeNull] Credentials credentials = default ) {
            if ( String.IsNullOrWhiteSpace( value: serverName ) ) {
                throw new ArgumentException( message: "Value cannot be null or whitespace.", nameof( serverName ) );
            }

            if ( String.IsNullOrWhiteSpace( value: instanceName ) ) {
                throw new ArgumentException( message: "Value cannot be null or whitespace.", nameof( instanceName ) );
            }

            var builder = new SqlConnectionStringBuilder {
                DataSource = $@"{serverName}\{instanceName}",
                ApplicationIntent = ApplicationIntent.ReadWrite,
                ApplicationName = Assembly.GetEntryAssembly()?.Location.GetFileNameWithoutExtension(),
                ConnectRetryCount = 3,
                ConnectTimeout = ( Int32 )connectTimeout.TotalSeconds,
                ConnectRetryInterval = 1,
                PacketSize = 8060,
                Pooling = true
            };

            //security
            if ( String.IsNullOrWhiteSpace( credentials?.Username ) ) {
                builder.Remove( nameof( builder.UserID ) );
                builder.Remove( nameof( builder.Password ) );
                builder.IntegratedSecurity = true;
            }
            else {
                builder.Remove( "Integrated Security" );
                builder.Remove( "Authentication" );
                builder.IntegratedSecurity = false;
                builder.UserID = credentials.UserID;
                builder.Password = credentials.Password;
            }

            return builder;
        }

        /*
        [NotNull]
        [DebuggerStepThrough]
        public static async Task StartAnySQLBrowsers( TimeSpan timeout ) {
            await Task.Run( () => {

                "Searching for any database servers...".Log();

                var machines = new ConcurrentHashset<String> {
                    "Thor", Dns.GetHostName(), "127.0.0.1" //my development servers' names. Feel free to add your server.
                };

                var stopwatch = Stopwatch.StartNew();

                var services = machines.Where( s => !String.IsNullOrWhiteSpace( s ) ).Select( machineName => {
                    var service = new ServiceController {
                        ServiceName = "SQLBrowser", MachineName = machineName
                    };

                    if ( service.Status != ServiceControllerStatus.Running ) {
                        service.Start();
                    }

                    return service;
                } ).ToList();

                while ( stopwatch.Elapsed < timeout ) {
                    var notRunningYet = services.Where( controller =>
                        controller?.Status.In( ServiceControllerStatus.StartPending, ServiceControllerStatus.Running ) == false );

                    Parallel.ForEach( notRunningYet, controller => controller?.WaitForStatus( ServiceControllerStatus.Running, timeout ) );

                    if ( services.Any( controller => controller?.Status.In( ServiceControllerStatus.Running ) == true ) ) {
                        return;
                    }
                }
            } ).ConfigureAwait( false );
        }
        */
    }
}