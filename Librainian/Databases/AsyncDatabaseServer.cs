// Copyright © Rick@AIBrain.org and Protiguous. All Rights Reserved.
// 
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
// 
// This source code contained in "AsyncDatabaseServer.cs" belongs to Protiguous@Protiguous.com and
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
// Project: "Librainian", "AsyncDatabaseServer.cs" was last formatted by Protiguous on 2019/11/06 at 4:54 AM.

namespace Librainian.Databases {

    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Common;
    using System.Data.Sql;
    using System.Data.SqlClient;
    using System.Diagnostics;
    using System.Linq;
    using System.Net;
    using System.ServiceProcess;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Windows.Forms;
    using Collections.Extensions;
    using Collections.Sets;
    using Internet;
    using JetBrains.Annotations;
    using Logging;
    using Magic;
    using Maths;
    using Parsing;

    public class AsyncDatabaseServer : ABetterClassDispose, IDatabase {

        public TimeSpan CommandTimeout { get; set; } = TimeSpan.FromMinutes( 2 );

        [CanBeNull]
        public String Sproc { get; set; }

        public TimeSpan Timeout { get; set; } = TimeSpan.FromMinutes( 1 );

        public void Add<T>( [NotNull] String name, SqlDbType type, [CanBeNull] T value ) {
            if ( String.IsNullOrWhiteSpace( value: name ) ) {
                throw new ArgumentException( message: "Value cannot be null or whitespace.", paramName: nameof( name ) );
            }

            this.ParameterSet.Add( new SqlParameter( name, type ) {
                Value = value
            } );
        }

        public Int32? ExecuteNonQuery( [NotNull] String query, Int32 retries, [CanBeNull] params SqlParameter[] parameters ) {
            if ( String.IsNullOrWhiteSpace( value: query ) ) {
                throw new ArgumentException( message: "Value cannot be null or whitespace.", paramName: nameof( query ) );
            }

            this.Sproc = query;

            TryAgain:
            --retries;

            try {
                using var connection = new SqlConnection( this.ConnectionString );

                using var command = new SqlCommand( );
                command.Connection = connection;
                command.CommandTimeout = ( Int32 )this.CommandTimeout.TotalSeconds;
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = query;

                if ( null != parameters ) {
                    foreach ( var parameter in parameters ) {
                        command.Parameters.Add( parameter );
                    }
                }

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

        /// <summary>
        ///     Opens and then closes a <see cref="SqlConnection" />.
        /// </summary>
        /// <returns></returns>
        public Int32? ExecuteNonQuery( [NotNull] String query, [CanBeNull] params SqlParameter[] parameters ) {
            if ( String.IsNullOrWhiteSpace( value: query ) ) {
                throw new ArgumentException( message: "Value cannot be null or whitespace.", paramName: nameof( query ) );
            }
            this.Sproc = query;

            try {
                using var connection = new SqlConnection( this.ConnectionString );

                using var command = new SqlCommand( query, connection ) {
                    CommandType = CommandType.StoredProcedure,
                    CommandTimeout = ( Int32 )this.CommandTimeout.TotalSeconds
                };

                if ( null != parameters ) {
                    foreach ( var parameter in parameters ) {
                        command.Parameters.Add( parameter );
                    }
                }

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
            this.Sproc = query ?? throw new ArgumentNullException( paramName: nameof( query ) );

            try {
                using ( var connection = new SqlConnection( this.ConnectionString ) ) {

                    using ( var command = new SqlCommand( query, connection ) {
                        CommandType = commandType,
                        CommandTimeout = ( Int32 )this.CommandTimeout.TotalSeconds
                    } ) {
                        if ( null != parameters ) {
                            foreach ( var parameter in parameters ) {
                                command.Parameters.Add( parameter );
                            }
                        }

                        await connection.OpenAsync( this.Token ).ConfigureAwait( false );

                        using ( var nonQueryAsync = command.ExecuteNonQueryAsync( this.Token ) ) {
                            return await nonQueryAsync.ConfigureAwait( false );
                        }
                    }
                }
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

        /// <summary>
        ///     Returns a <see cref="DataTable" />
        /// </summary>
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

                if ( null != parameters ) {
                    foreach ( var parameter in parameters ) {
                        command.Parameters.Add( parameter );
                    }
                }

                table.BeginLoadData();
                connection.Open();
                table.Load( command.ExecuteReader() );
                table.EndLoadData();

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
                throw new ArgumentException( message: "Value cannot be null or whitespace.", paramName: nameof( query ) );
            }
            this.Sproc = query;

            try {

                using var connection = new SqlConnection( this.ConnectionString );

                using var command = new SqlCommand( query, connection ) {
                    CommandType = commandType,
                    CommandTimeout = ( Int32 )this.CommandTimeout.TotalSeconds
                };

                if ( null != parameters ) {
                    foreach ( var parameter in parameters ) {
                        command.Parameters.Add( parameter );
                    }
                }

                await connection.OpenAsync( this.Token ).ConfigureAwait( false );
                using var readerAsync = await command.ExecuteReaderAsync( this.Token ).ConfigureAwait( false );
                using var table = readerAsync.ToDataTable();

                return table.CreateDataReader();
            }
            catch ( SqlException exception ) {
                exception.Log( Rebuild( query, parameters ) );
            }
            catch ( DbException exception ) {
                exception.Log( Rebuild( query, parameters ) );
            }

            return default;
        }

        /// <summary>
        ///     Returns a <see cref="DataTable" />
        /// </summary>
        /// <param name="query">      </param>
        /// <param name="commandType"></param>
        /// <param name="parameters"> </param>
        /// <returns></returns>
        [ItemNotNull]
        public async Task<DataTable> ExecuteReaderDataTableAsync( [NotNull] String query, CommandType commandType, [CanBeNull] params SqlParameter[] parameters ) {
            if ( String.IsNullOrWhiteSpace( value: query ) ) {
                throw new ArgumentException( message: "Value cannot be null or whitespace.", paramName: nameof( query ) );
            }
            this.Sproc = query;

            var table = new DataTable();

            try {
                using var connection = new SqlConnection( this.ConnectionString );

                using var command = new SqlCommand( query, connection ) {
                    CommandType = commandType,
                    CommandTimeout = ( Int32 )this.CommandTimeout.TotalSeconds
                };

                if ( null != parameters ) {
                    foreach ( var parameter in parameters ) {
                        command.Parameters.Add( parameter );
                    }
                }

                table.BeginLoadData();
                await connection.OpenAsync( this.Token ).ConfigureAwait( false );
                table.Load( await command.ExecuteReaderAsync( this.Token ).ConfigureAwait( false ) );
                table.EndLoadData();
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
                throw new ArgumentException( message: "Value cannot be null or whitespace.", paramName: nameof( query ) );
            }
            this.Sproc = query;

            try {
                using var connection = new SqlConnection( this.ConnectionString );

                using var command = new SqlCommand( query, connection ) {
                    CommandType = commandType,
                    CommandTimeout = ( Int32 )this.CommandTimeout.TotalSeconds
                };

                if ( null != parameters ) {
                    foreach ( var parameter in parameters ) {
                        command.Parameters.Add( parameter );
                    }
                }

                connection.Open();

                return command.ExecuteScalar().ConvertTo<T>();
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

                if ( null != parameters ) {
                    foreach ( var parameter in parameters ) {
                        command.Parameters.Add( parameter );
                    }
                }

                await connection.OpenAsync( this.Token ).ConfigureAwait( false );

                var scalar = await command.ExecuteScalarAsync( this.Token ).ConfigureAwait( false );

                return scalar.ConvertTo<T>();
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
        ///     Overwrites the <paramref name="table" /> contents with data from the <paramref name="sproc" />.
        ///     <para>Note: Include the parameters after the sproc.</para>
        ///     <para>Can throw exceptions on connecting or executing the sproc.</para>
        /// </summary>
        /// <param name="sproc"></param>
        /// <param name="table"></param>
        public async Task<Boolean> FillTableAsync( [NotNull] String sproc, [NotNull] DataTable table ) {
            if ( table == null ) {
                throw new ArgumentNullException( paramName: nameof( table ) );
            }
            this.Sproc = sproc;

            if ( String.IsNullOrWhiteSpace( value: sproc ) ) {
                throw new ArgumentException( message: "Value cannot be null or whitespace.", paramName: nameof( sproc ) );
            }

            table.Clear();

            using var connection = new SqlConnection( this.ConnectionString );

            using var dataAdapter = new SqlDataAdapter( sproc, connection ) {
                AcceptChangesDuringFill = false,
                FillLoadOption = LoadOption.OverwriteChanges,
                MissingMappingAction = MissingMappingAction.Passthrough,
                MissingSchemaAction = MissingSchemaAction.Add,
                SelectCommand = {
                    CommandTimeout = ( Int32 ) this.Timeout.TotalSeconds, CommandType = CommandType.StoredProcedure
                }
            };

            dataAdapter.SelectCommand?.Parameters.AddRange( this.ParameterSet.ToArray() );

            await connection.OpenAsync( this.Token ).ConfigureAwait( false );
            dataAdapter.Fill( table );

            return true;
        }

        /// <summary>
        ///     <para>Run a query, no rows expected to be read.</para>
        ///     <para>Does not catch any exceptions.</para>
        /// </summary>
        /// <param name="sproc"></param>
        public async Task NonQueryAsync( [NotNull] String sproc ) {
            if ( String.IsNullOrWhiteSpace( value: sproc ) ) {
                throw new ArgumentException( message: "Value cannot be null or whitespace.", paramName: nameof( sproc ) );
            }

            this.Sproc = $"Executing SQL command {sproc}.";

            using var connection = new SqlConnection( this.ConnectionString );

            using var command = new SqlCommand {
                Connection = connection,
                CommandType = CommandType.StoredProcedure,
                CommandTimeout = ( Int32 )this.Timeout.TotalSeconds,
                CommandText = sproc
            };

            command.Parameters.AddRange( this.ParameterSet.ToArray() );
            await connection.OpenAsync( this.Token ).ConfigureAwait( false );
            await command.ExecuteNonQueryAsync( this.Token ).ConfigureAwait( false );
        }

        /// <summary>
        ///     Make sure to include any parameters ( <see cref="Add{T}" />) to avoid sql injection attacks.
        /// </summary>
        /// <param name="sql"></param>
        [NotNull]
        public DataTableReader QueryAdHoc( [NotNull] String sql ) {
            if ( String.IsNullOrWhiteSpace( value: sql ) ) {
                throw new ArgumentException( message: "Value cannot be null or whitespace.", paramName: nameof( sql ) );
            }

            this.Sproc = $"Executing AdHoc SQL: {sql.DoubleQuote()}.";

            using var connection = new SqlConnection( this.ConnectionString );

            using var command = new SqlCommand {
                Connection = connection,
                CommandTimeout = ( Int32 )this.Timeout.TotalSeconds,
                CommandType = CommandType.Text,
                CommandText = sql
            };

            command.Parameters.AddRange( this.ParameterSet.ToArray() );
            connection.Open();

            using var reader = command.ExecuteReader();
            using var table = reader.ToDataTable();
            return table.CreateDataReader();
        }

        /// <summary>
        ///     Make sure to include any parameters ( <see cref="Add{T}" />) to avoid sql injection attacks.
        /// </summary>
        /// <param name="sql"></param>
        [NotNull]
        [ItemNotNull]
        public async Task<DataTableReader> QueryAdHocAsync( [NotNull] String sql ) {
            if ( String.IsNullOrWhiteSpace( value: sql ) ) {
                throw new ArgumentException( message: "Value cannot be null or whitespace.", paramName: nameof( sql ) );
            }

            this.Sproc = $"Executing AdHoc SQL: {sql.DoubleQuote()}.";

            using var connection = new SqlConnection( this.ConnectionString );

            using var command = new SqlCommand {
                Connection = connection,
                CommandTimeout = ( Int32 )this.Timeout.TotalSeconds,
                CommandType = CommandType.Text,
                CommandText = sql
            };

            command.Parameters.AddRange( this.ParameterSet.ToArray() );
            await connection.OpenAsync( this.Token ).ConfigureAwait( false );

            using var reader = await command.ExecuteReaderAsync( this.Token ).ConfigureAwait( false );
            using var table = reader.ToDataTable();
            return table.CreateDataReader();
        }

        /// <summary>
        ///     Simplest possible database connection.
        ///     <para>Connect and then run <paramref name="sproc" />.</para>
        /// </summary>
        /// <param name="sproc"></param>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        [NotNull]
        public async Task<SqlDataReader> QueryAsync( [NotNull] String sproc ) {
            if ( String.IsNullOrWhiteSpace( value: sproc ) ) {
                throw new ArgumentException( message: "Value cannot be null or whitespace.", paramName: nameof( sproc ) );
            }

            this.Sproc = sproc;

            using var connection = new SqlConnection( this.ConnectionString );

            using var command = new SqlCommand {
                Connection = connection,
                CommandType = CommandType.StoredProcedure,
                CommandTimeout = ( Int32 )this.Timeout.TotalSeconds,
                CommandText = sproc
            };

            command.Parameters.AddRange( this.ParameterSet.ToArray() );
            await connection.OpenAsync( this.Token ).ConfigureAwait( false );

            return await command.ExecuteReaderAsync( this.Token ).ConfigureAwait( false );
        }

        /// <summary>
        ///     Returns a <see cref="DataTable" />
        /// </summary>
        /// <param name="query">     </param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        [CanBeNull]
        [ItemCanBeNull]
        public IEnumerable<TResult> QueryList<TResult>( String query, [CanBeNull] params SqlParameter[] parameters ) {
            if ( query.IsNullOrWhiteSpace() ) {
                throw new ArgumentNullException( nameof( query ) );
            }
            this.Sproc = query;

            try {
                using var connection = new SqlConnection( this.ConnectionString );

                using var command = new SqlCommand( query, connection ) {
                    CommandType = CommandType.StoredProcedure,
                    CommandTimeout = ( Int32 )this.CommandTimeout.TotalSeconds
                };

                if ( null != parameters ) {
                    foreach ( var parameter in parameters ) {
                        command.Parameters.Add( parameter );
                    }
                }

                connection.Open();

                return GenericPopulatorExtensions.CreateList<TResult>( command.ExecuteReader() );
            }
            catch ( SqlException exception ) {
                exception.Log( Rebuild( query, parameters ) );
            }
            catch ( DbException exception ) {
                exception.Log( Rebuild( query, parameters ) );
            }

            return null;
        }

        public void UseDatabase( [NotNull] String dbName ) {
            if ( String.IsNullOrWhiteSpace( value: dbName ) ) {
                throw new ArgumentException( message: "Value cannot be null or whitespace.", paramName: nameof( dbName ) );
            }

            using ( var _ = this.QueryAdHoc( $"USE {dbName.Bracket()};" ) ) { }
        }

        public async Task UseDatabaseAsync( [NotNull] String dbName ) {
            if ( String.IsNullOrWhiteSpace( value: dbName ) ) {
                throw new ArgumentException( message: "Value cannot be null or whitespace.", paramName: nameof( dbName ) );
            }

            using ( var _ = await this.QueryAdHocAsync( $"USE {dbName.Bracket()};" ).ConfigureAwait( false ) ) { }
        }

        [NotNull]
        private String ConnectionString { get; }

        private CancellationToken Token { get; }

        /// <summary>
        ///     The parameter collection for this database connection.
        /// </summary>
        [NotNull]
        internal ConcurrentHashset<SqlParameter> ParameterSet { get; } = new ConcurrentHashset<SqlParameter>();

        /// <summary>
        ///     <para>Create a database object to the specified server.</para>
        /// </summary>
        public AsyncDatabaseServer( [NotNull] SqlConnectionStringBuilder builder, [CanBeNull] String useDatabase = null, CancellationToken? token = default ) {
            if ( builder == null ) {
                throw new ArgumentNullException( paramName: nameof( builder ) );
            }

            this.Token = token ?? CancellationToken.None;

            useDatabase = useDatabase.Trimmed();

            if ( !String.IsNullOrWhiteSpace( useDatabase ) ) {
                builder.InitialCatalog = useDatabase.Bracket();
            }

            this.ConnectionString = builder.ConnectionString;
        }

        /// <summary>
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="useDatabase"></param>
        /// <param name="token"></param>
        /// <exception cref="InvalidOperationException"></exception>
        public AsyncDatabaseServer( [NotNull] String connectionString, [CanBeNull] String useDatabase = null, CancellationToken? token = default ) {
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

#pragma warning disable CA1063 // Implement IDisposable Correctly
        ~AsyncDatabaseServer() {
#pragma warning restore CA1063 // Implement IDisposable Correctly
            $"We have an undisposed Database() connection somewhere. Query={this.Sproc.DoubleQuote()}".Break();
        }

        [DebuggerStepThrough]
        [NotNull]
        private static String Rebuild( [NotNull] String query, [CanBeNull] IEnumerable<SqlParameter> parameters = null ) {
            if ( String.IsNullOrWhiteSpace( value: query ) ) {
                throw new ArgumentException( message: "Value cannot be null or whitespace.", paramName: nameof( query ) );
            }

            if ( parameters == null ) {
                return $"exec {query}";
            }

            return
                $"exec {query} {parameters.Where( parameter => parameter != default ).Select( parameter => $"{parameter.ParameterName}={parameter.Value?.ToString().SingleQuote() ?? String.Empty}" ).ToStrings()}; ";
        }

        public static async Task<Boolean> CreateDatabase( [NotNull] String databaseName, [NotNull] String connectionString ) {
            databaseName = databaseName.Trimmed();

            if ( String.IsNullOrWhiteSpace( value: databaseName ) ) {
                throw new ArgumentException( message: "Value cannot be null or whitespace.", paramName: nameof( databaseName ) );
            }

            if ( String.IsNullOrWhiteSpace( value: connectionString ) ) {
                throw new ArgumentException( message: "Value cannot be null or whitespace.", paramName: nameof( connectionString ) );
            }

            try {
                using var db = new AsyncDatabaseServer( connectionString, useDatabase: "master" );

                await db.QueryAdHocAsync( $"create database {databaseName.Bracket()};" ).ConfigureAwait( false );

                return true;
            }
            catch ( SqlException exception ) {
                if ( !exception.SQLTimeout() ) {
                    exception.Log();
                }
            }

            return false;
        }

        /// <summary>
        ///     //TODO Make this better later.. just return any "working" connection for now.
        /// </summary>
        /// <param name="timeout"></param>
        /// <param name="credentials"></param>
        /// <returns></returns>
        [NotNull]
        public static IEnumerable<SqlServer> FindUsableServers( TimeSpan timeout, [NotNull] params Credentials[] credentials ) {
            if ( credentials == null ) {
                throw new ArgumentNullException( paramName: nameof( credentials ) );
            }

            try {
                var cts = new CancellationTokenSource( timeout );

                return credentials.Where( c => c != default ).SelectMany( credential => LookForAnyDatabaseServerWithThisCredential( credential, timeout ) )
                    .Select( builder => builder?.TryGetResponse( cts.Token ) ).Where( builder => builder != default && builder.Status == Status.Success );
            }
            catch ( Exception exception ) {
                exception.Log();
            }

            throw new InvalidOperationException();
        }

        [NotNull]
        public static IEnumerable<SqlConnectionStringBuilder> LookForAnyDatabaseServerWithThisCredential( [CanBeNull] Credentials credentials, TimeSpan connectTimeout ) {
            if ( SqlDataSourceEnumerator.Instance != null ) {
                return
                    from DataRow row in SqlDataSourceEnumerator.Instance.GetDataSources().Rows
                    let serverName = row?[ "ServerName" ].Trimmed()
                    let instanceName = row?[ "InstanceName" ].Trimmed()
                    where !String.IsNullOrWhiteSpace( serverName ) && !String.IsNullOrWhiteSpace( instanceName )
                    select PopulateConnectionStringBuilder( serverName ?? throw new InvalidOperationException(), instanceName ?? throw new InvalidOperationException(),
                        connectTimeout, credentials );
            }

            return Enumerable.Empty<SqlConnectionStringBuilder>();
        }

        [NotNull]
        public static SqlConnectionStringBuilder PopulateConnectionStringBuilder( [NotNull] String serverName, [NotNull] String instanceName, TimeSpan connectTimeout,
            [CanBeNull] Credentials credentials = default ) {
            if ( String.IsNullOrWhiteSpace( value: serverName ) ) {
                throw new ArgumentException( message: "Value cannot be null or whitespace.", paramName: nameof( serverName ) );
            }

            if ( String.IsNullOrWhiteSpace( value: instanceName ) ) {
                throw new ArgumentException( message: "Value cannot be null or whitespace.", paramName: nameof( instanceName ) );
            }

            var builder = new SqlConnectionStringBuilder {
                DataSource = $@"{serverName}\{instanceName}",
                AsynchronousProcessing = true,
                ApplicationIntent = ApplicationIntent.ReadWrite,
                ApplicationName = Application.ProductName,
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

        [NotNull]
        [DebuggerStepThrough]
        public static async Task StartAnySQLBrowsers( TimeSpan timeout ) {

            "Searching for any database servers...".Log();

            var machines = new ConcurrentHashset<String> {
                "Thor", Dns.GetHostName(), "127.0.0.1" //my development servers' names. Feel free to add your server.
            };

            var stopwatch = Stopwatch.StartNew();

            await Task.Run( () => {

                var services = new List<ServiceController>( machines.Where( s => !String.IsNullOrWhiteSpace( s ) ).Select( machineName => {
                    using var service = new ServiceController {
                        ServiceName = "SQLBrowser",
                        MachineName = machineName
                    };
                    if ( service.Status != ServiceControllerStatus.Running ) {
                        service.Start();
                    }
                    return service;
                } ) );

                $"Searching for database servers...".Log();
                while ( stopwatch.Elapsed < timeout ) {
                    var notRunningYet = services.Where( controller => controller != default ).Where( controller =>
                          controller.Status.In( ServiceControllerStatus.StartPending, ServiceControllerStatus.Running ) == false );

                    Parallel.ForEach( notRunningYet, controller => controller?.WaitForStatus( ServiceControllerStatus.Running, timeout ) );

                }
            } ).ConfigureAwait( false );
        }

    }

}