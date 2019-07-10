// Copyright © Rick@AIBrain.org and Protiguous. All Rights Reserved.
// 
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
// 
// This source code contained in "Database.cs" belongs to Protiguous@Protiguous.com and
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
// Project: "Librainian", "Database.cs" was last formatted by Protiguous on 2019/06/23 at 1:43 PM.

namespace Librainian.Database {

    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Common;
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
    using Extensions;
    using Internet;
    using JetBrains.Annotations;
    using Logging;
    using Magic;
    using Maths;
    using Parsing;

    public sealed class Database : ABetterClassDispose, IDatabase {

        /// <summary>
        ///     Opens and then closes a <see cref="SqlConnection" />.
        /// </summary>
        /// <returns></returns>
        public Boolean ExecuteNonQuery( String query, [CanBeNull] params SqlParameter[] parameters ) {
            if ( query.IsNullOrWhiteSpace() ) {
                throw new ArgumentNullException( nameof( query ) );
            }

            try {
                using ( var connection = new SqlConnection( this._connectionString ) ) {
                    connection.Open();

                    using ( var command = new SqlCommand( query, connection ) {
                        CommandType = CommandType.Text, CommandTimeout = ( Int32 ) this.CommandTimeout.TotalSeconds
                    } ) {
                        if ( null != parameters ) {
                            command.Parameters.AddRange( parameters.Where( parameter => parameter != null ).ToArray() );
                        }

                        command.ExecuteNonQuery();
                    }

                    return true;
                }
            }
            catch ( SqlException exception ) {
                exception.Log( Combine( query, parameters ) );
            }
            catch ( DbException exception ) {
                exception.Log( Combine( query, parameters ) );
            }
            catch ( Exception exception ) {
                exception.Log( Combine( query, parameters ) );
            }

            return false;
        }

        public Boolean ExecuteNonQuery( String query, Int32 retries, [CanBeNull] params SqlParameter[] parameters ) {
            if ( query.IsNullOrWhiteSpace() ) {
                throw new ArgumentNullException( nameof( query ) );
            }

            TryAgain:
            --retries;

            try {
                using ( var connection = new SqlConnection( this._connectionString ) ) {
                    connection.Open();

                    using ( var command = new SqlCommand( query, connection ) {
                        CommandType = CommandType.StoredProcedure, CommandTimeout = ( Int32 ) this.CommandTimeout.TotalSeconds
                    } ) {
                        if ( null != parameters ) {
                            command.Parameters.AddRange( parameters.Where( parameter => parameter != null ).ToArray() );
                        }

                        command.ExecuteNonQuery();

                        return true;
                    }
                }
            }
            catch ( InvalidOperationException exception ) {
                exception.Log( Combine( query, parameters ) );

                if ( retries.Any() ) {
                    goto TryAgain;
                }
            }
            catch ( SqlException exception ) {
                exception.Log( Combine( query, parameters ) );

                if ( retries.Any() ) {
                    goto TryAgain;
                }
            }
            catch ( DbException exception ) {
                exception.Log( Combine( query, parameters ) );

                if ( retries.Any() ) {
                    goto TryAgain;
                }
            }

            return false;
        }

        [ItemCanBeNull]
        public async Task<Int32?> ExecuteNonQueryAsync( String query, CommandType commandType, [CanBeNull] params SqlParameter[] parameters ) {
            if ( query.IsNullOrWhiteSpace() ) {
                throw new ArgumentNullException( nameof( query ) );
            }

            try {
                using ( var connection = new SqlConnection( this._connectionString ) ) {
                    try {
                        await connection.OpenAsync().ConfigureAwait( false );
                    }
                    catch ( SqlException exception) {
                        exception.Log();

                        return default;
                    }

                    using ( var command = new SqlCommand( query, connection ) {
                        CommandType = commandType, CommandTimeout = ( Int32 ) this.CommandTimeout.TotalSeconds
                    } ) {
                        if ( null != parameters ) {
                            command.Parameters.AddRange( parameters.Where( parameter => parameter != null ).ToArray() );
                        }

                        try {
                            return await command.ExecuteNonQueryAsync().ConfigureAwait( false );
                        }
                        catch ( SqlException exception ) {
                            exception.Log();
                            return default;
                        }
                    }
                }
            }
            catch ( SqlException exception ) {
                if ( !exception.SQLTimeout( delayFor: TimeSpan.FromSeconds( 1 ) ) ) {
                    exception.Log( Combine( query, parameters ) );
                }
            }
            catch ( DbException exception ) {
                exception.Log( Combine( query, parameters ) );
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

            table = new DataTable();

            try {

                using ( var connection = new SqlConnection( this._connectionString ) ) {
                    connection.Open();

                    using ( var command = new SqlCommand( query, connection ) {
                        CommandType = commandType, CommandTimeout = ( Int32 ) this.CommandTimeout.TotalSeconds
                    } ) {
                        if ( null != parameters ) {
                            command.Parameters.AddRange( parameters.Where( parameter => parameter != null ).ToArray() );
                        }

                        table.BeginLoadData();

                        using ( var reader = command.ExecuteReader() ) {
                            table.Load( reader );
                        }

                        table.EndLoadData();

                        return true;
                    }
                }
            }
            catch ( SqlException exception ) {
                exception.Log( Combine( query, parameters ) );
            }
            catch ( DbException exception ) {
                exception.Log( Combine( query, parameters ) );
            }

            return false;
        }

        /// <param name="query">      </param>
        /// <param name="commandType"></param>
        /// <param name="parameters"> </param>
        [ItemCanBeNull]
        public async Task<SqlDataReader> ExecuteReaderAsyncDataReader( String query, CommandType commandType, [CanBeNull] params SqlParameter[] parameters ) {
            if ( query.IsNullOrWhiteSpace() ) {
                throw new ArgumentNullException( nameof( query ) );
            }

            try {
                using ( var connection = new SqlConnection( this._connectionString ) ) {
                    await connection.OpenAsync().ConfigureAwait( false );

                    using ( var command = new SqlCommand( query, connection ) {
                        CommandType = commandType, CommandTimeout = ( Int32 ) this.CommandTimeout.TotalSeconds
                    } ) {
                        if ( null != parameters ) {
                            command.Parameters.AddRange( parameters.Where( parameter => parameter != null ).ToArray() );
                        }

                        return await command.ExecuteReaderAsync().ConfigureAwait( false );
                    }
                }
            }
            catch ( SqlException exception ) {
                exception.Log( Combine( query, parameters ) );
            }
            catch ( DbException exception ) {
                exception.Log( Combine( query, parameters ) );
            }

            return null;
        }

        /// <summary>
        ///     Returns a <see cref="DataTable" />
        /// </summary>
        /// <param name="query">      </param>
        /// <param name="commandType"></param>
        /// <param name="parameters"> </param>
        /// <returns></returns>
        [ItemNotNull]
        public async Task<DataTable> ExecuteReaderAsyncDataTable( String query, CommandType commandType, [CanBeNull] params SqlParameter[] parameters ) {
            if ( query.IsNullOrWhiteSpace() ) {
                throw new ArgumentNullException( nameof( query ) );
            }

            var table = new DataTable();

            try {
                using ( var connection = new SqlConnection( this._connectionString ) ) {
                    await connection.OpenAsync().ConfigureAwait( false );

                    using ( var command = new SqlCommand( query, connection ) {
                        CommandType = commandType, CommandTimeout = ( Int32 ) this.CommandTimeout.TotalSeconds
                    } ) {
                        if ( null != parameters ) {
                            command.Parameters.AddRange( parameters.Where( parameter => parameter != null ).ToArray() );
                        }

                        table.BeginLoadData();

                        using ( var reader = await command.ExecuteReaderAsync().ConfigureAwait( false ) ) {
                            table.Load( reader );
                        }

                        table.EndLoadData();
                    }
                }
            }
            catch ( SqlException exception ) {
                exception.Log( Combine( query, parameters ) );
            }
            catch ( DbException exception ) {
                exception.Log( Combine( query, parameters ) );
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
        public TResult ExecuteScalar<TResult>( String query, CommandType commandType, [CanBeNull] params SqlParameter[] parameters ) {
            if ( query.IsNullOrWhiteSpace() ) {
                throw new ArgumentNullException( nameof( query ) );
            }

            try {
                using ( var connection = new SqlConnection( this._connectionString ) ) {
                    connection.Open();

                    using ( var command = new SqlCommand( query, connection ) {
                        CommandType = commandType, CommandTimeout = ( Int32 ) this.CommandTimeout.TotalSeconds
                    } ) {
                        if ( null != parameters ) {
                            command.Parameters.AddRange( parameters.Where( parameter => parameter != null ).ToArray() );
                        }

                        var scalar = command.ExecuteScalar();

                        if ( null == scalar || Convert.IsDBNull( scalar ) ) {
                            return default;
                        }

                        if ( scalar is TResult executeScalar ) {
                            return executeScalar;
                        }

                        if ( scalar.TryCast<TResult>( out var result ) ) {
                            return result;
                        }

                        return ( TResult ) Convert.ChangeType( scalar, typeof( TResult ) );
                    }
                }
            }
            catch ( SqlException exception ) {
                exception.Log( Combine( query, parameters ) );
            }
            catch ( DbException exception ) {
                exception.Log( Combine( query, parameters ) );
            }

            return default;
        }

        /// <summary>
        ///     <para>Returns the first column of the first row.</para>
        /// </summary>
        /// <param name="query">      </param>
        /// <param name="commandType"></param>
        /// <param name="parameters"> </param>
        /// <returns></returns>
        [ItemCanBeNull]
        public async Task<TResult> ExecuteScalarAsync<TResult>( String query, CommandType commandType, [CanBeNull] params SqlParameter[] parameters ) {
            if ( query.IsNullOrWhiteSpace() ) {
                throw new ArgumentNullException( nameof( query ) );
            }

            try {
                using ( var connection = new SqlConnection( this._connectionString ) ) {
                    await connection.OpenAsync().ConfigureAwait( false );

                    using ( var command = new SqlCommand( query, connection ) {
                        CommandType = commandType, CommandTimeout = ( Int32 ) this.CommandTimeout.TotalSeconds
                    } ) {
                        if ( null != parameters ) {
                            command.Parameters.AddRange( parameters.Where( parameter => parameter != null ).ToArray() );
                        }

                        var result = await command.ExecuteScalarAsync().ConfigureAwait( false );

                        if ( result == DBNull.Value ) {
                            return default;
                        }

                        return ( TResult ) result;
                    }
                }
            }
            catch ( InvalidCastException exception ) {

                //TIP: check for SQLServer returning a Double when you expect a Single (float in SQL).
                exception.Log( Combine( query, parameters ) );
            }
            catch ( SqlException exception ) {
                exception.Log( Combine( query, parameters ) );
            }
            catch ( DbException exception ) {
                exception.Log( Combine( query, parameters ) );
            }

            return default;
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

            try {
                using ( var connection = new SqlConnection( this._connectionString ) ) {
                    connection.Open();

                    using ( var command = new SqlCommand( query, connection ) {
                        CommandType = CommandType.StoredProcedure, CommandTimeout = ( Int32 ) this.CommandTimeout.TotalSeconds
                    } ) {
                        if ( null != parameters ) {
                            command.Parameters.AddRange( parameters.Where( parameter => parameter != null ).ToArray() );
                        }

                        using ( var reader = command.ExecuteReader() ) {
                            return GenericPopulator<TResult>.CreateList( reader );
                        }
                    }
                }
            }
            catch ( SqlException exception ) {
                exception.Log( Combine( query, parameters ) );
            }
            catch ( DbException exception ) {
                exception.Log( Combine( query, parameters ) );
            }

            return null;
        }

        private readonly String _connectionString;

        public static ConcurrentHashset<SqlConnectionStringBuilder> ConnectiontringBuilders { get; } = new ConcurrentHashset<SqlConnectionStringBuilder>();

        public TimeSpan CommandTimeout { get; set; } = TimeSpan.FromMinutes( 1 );

        /// <summary>
        /// </summary>
        /// <param name="connectionString"></param>
        /// <exception cref="InvalidOperationException"></exception>
        public Database( String connectionString ) => this._connectionString = connectionString;

        private static (String query, IEnumerable<SqlParameter> parameters) Combine( [NotNull] String query, [NotNull] SqlParameterCollection parameters ) {
            if ( String.IsNullOrWhiteSpace( value: query ) ) {
                throw new ArgumentException( message: "Value cannot be null or whitespace.", paramName: nameof( query ) );
            }

            return ( $"{query.SingleQuote()};", parameters.Cast<SqlParameter>() );
        }

        private static (String query, IEnumerable<SqlParameter> parameters) Combine( [NotNull] String query, [CanBeNull] IEnumerable<SqlParameter> parameters ) {
            if ( String.IsNullOrWhiteSpace( value: query ) ) {
                throw new ArgumentException( message: "Value cannot be null or whitespace.", paramName: nameof( query ) );
            }

            return ( $"{query.SingleQuote()};", parameters );
        }

        public static Boolean CreateDatabaseIfNotExist( [NotNull] String databaseName, String connectionString ) {
            try {
                databaseName = databaseName.Trim();

                if ( String.IsNullOrWhiteSpace( value: databaseName ) ) {
                    throw new ArgumentException( message: "Value cannot be null or whitespace.", paramName: nameof( databaseName ) );
                }

                var builder = new SqlConnectionStringBuilder( connectionString ) {
                    InitialCatalog = "[master]"
                };

                using ( var db = new DatabaseAsync( builder ) ) {
                    var sql = $"create database [{databaseName}];";
                    var result = db.QueryAdHoc( sql );

                    return true;
                }
            }
            catch ( SqlException exception ) {
                if ( !exception.SQLTimeout() ) {
                    exception.Log( Combine( databaseName, ( IEnumerable<SqlParameter> ) null ) );
                }
            }

            return false;
        }

        [ItemNotNull]
        public static async Task<ConcurrentDictionary<String, ServiceControllerStatus>> FindAndStartSqlBrowserServices( [NotNull] IEnumerable<String> activeMachines,
            TimeSpan timeout ) {
            var service = new ServiceController {
                ServiceName = "SQLBrowser"
            };

            var machines = activeMachines.ToList();

            var thisMachine = Dns.GetHostName();

            if ( !machines.Contains( thisMachine ) ) {
                machines.Add( thisMachine );
            }

            var status = new ConcurrentDictionary<String, ServiceControllerStatus>();

            await Task.Run( () => {
                Parallel.ForEach( machines, machine => {

                    try {
                        var stopwatch = Stopwatch.StartNew();

                        service.MachineName = machine;

                        try {
                            if ( service.Status.In( ServiceControllerStatus.Running ) ) {
                                /*do nothing*/
                            }

                            if ( service.Status.In( ServiceControllerStatus.ContinuePending ) ) {
                                do {
                                    Thread.Yield();
                                } while ( service.Status.In( ServiceControllerStatus.ContinuePending ) && stopwatch.Elapsed < timeout );
                            }

                            if ( service.Status.In( ServiceControllerStatus.Paused ) ) {
                                service.Continue();

                                do {
                                    Thread.Yield();
                                } while ( service.Status.In( ServiceControllerStatus.Paused ) && stopwatch.Elapsed < timeout );
                            }

                            if ( service.Status.In( ServiceControllerStatus.PausePending, ServiceControllerStatus.Stopped, ServiceControllerStatus.StopPending ) ) {
                                service.Start();

                                do {
                                    Thread.Yield();
                                } while ( service.Status.In( ServiceControllerStatus.PausePending, ServiceControllerStatus.Stopped, ServiceControllerStatus.StopPending ) &&
                                          stopwatch.Elapsed < timeout );
                            }

                            if ( service.Status.In( ServiceControllerStatus.StartPending ) ) {
                                do {
                                    Thread.Yield();
                                } while ( service.Status.In( ServiceControllerStatus.StartPending ) && stopwatch.Elapsed < timeout );
                            }

                            status[ machine ] = service.Status;
                        }
                        catch ( InvalidOperationException exception ) {
                            exception.Log();
                        }
                    }
                    catch ( Exception e ) {
                        Debug.WriteLine( e.Message );
                    }
                } );
            } ).ConfigureAwait( false );

            return status;
        }

        [NotNull]
        public static SqlConnectionStringBuilder OurConnectionStringBuilder( [NotNull] String serverName, [NotNull] String instanceName, TimeSpan connectTimeout,
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
                ConnectTimeout = ( Int32 ) connectTimeout.TotalSeconds,
                ConnectRetryInterval = 1,
                PacketSize = 8000,
                Pooling = true
            };

            //security
            if ( credentials != default ) {
                builder.Remove( "Integrated Security" );
                builder.Remove( "Authentication" );
                builder.Encrypt = false;
                builder.UserID = credentials.UserID;
                builder.Password = credentials.Password;
            }
            else {
                builder.Remove( nameof( builder.UserID ) );
                builder.Remove( nameof( builder.Password ) );
                builder.Encrypt = false;
                builder.IntegratedSecurity = true;

                //builder.Authentication = SqlAuthenticationMethod.NotSpecified;
            }

            ConnectiontringBuilders.Add( builder );

            return builder;
        }

        public static async Task StartAnySQLBrowers( TimeSpan searchTimeout ) {
            "Searching for any database servers...".Log();

            await FindAndStartSqlBrowserServices( new[] {
                Dns.GetHostName()
            }, searchTimeout ).ConfigureAwait( false );
        }

    }

}