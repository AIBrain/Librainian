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
// Project: "Librainian", "Database.cs" was last formatted by Protiguous on 2019/09/12 at 10:38 AM.

namespace Librainian.Databases {

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
    using Xunit;

    public sealed class Database : ABetterClassDispose, IDatabase {

        private readonly String _connectionString;

        public static ConcurrentHashset<SqlConnectionStringBuilder> ConnectiontringBuilders { get; } = new ConcurrentHashset<SqlConnectionStringBuilder>();

        public TimeSpan CommandTimeout { get; set; } = TimeSpan.FromMinutes( 2 );

        public CancellationToken Token { get; }

        /// <summary>
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="token"></param>
        /// <exception cref="InvalidOperationException"></exception>
        public Database( String connectionString, CancellationToken? token = default ) {
            this.Token = token ?? CancellationToken.None;
            this._connectionString = connectionString;
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

            return $"exec {query} {parameters.Select( parameter => $"{parameter.ParameterName}={parameter.Value?.ToString().SingleQuote() ?? String.Empty}" ).ToStrings()}; ";
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
                    exception.Log( Rebuild( databaseName ) );
                }
            }

            return false;
        }

        [NotNull]
        [ItemNotNull]
        [DebuggerStepThrough]
        public static async Task<ConcurrentDictionary<String, ServiceControllerStatus>> FindAndStartSqlBrowserServices( [NotNull] IEnumerable<String> activeMachines,
                    TimeSpan timeout ) {

            "Searching for any database servers...".Log();

            var machines = new ConcurrentHashset<String>( activeMachines );

            var common = new[] {
                "127.0.0.1", Dns.GetHostName()
            };

            foreach ( var s in common ) {
                machines.Add( s );
            }

            var status = new ConcurrentDictionary<String, ServiceControllerStatus>();

            var tasks = new List<Task>( machines.Count );

            tasks.AddRange( machines.Select( machine => Task.Run( () => {
                try {
                    $"Searching for database server on {machine}...".Log();

                    try {
                        var service = new ServiceController {
                            ServiceName = "SQLBrowser", MachineName = machine
                        };

                        if ( service.Status != ServiceControllerStatus.Running ) {
                            service.Start();
                        }

                        service.WaitForStatus( ServiceControllerStatus.Running, timeout );
                        status[ machine ] = service.Status;
                    }
                    catch ( InvalidOperationException exception ) {
                        exception.Log();
                        status.TryRemove( machine, out _ );
                    }

                }
                catch ( Exception exception ) {
                    exception.Log();
                }
            }, new CancellationTokenSource( timeout ).Token ) ) );

            await Task.WhenAny(tasks).ConfigureAwait( false );

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
                ConnectTimeout = ( Int32 )connectTimeout.TotalSeconds,
                ConnectRetryInterval = 1,
                PacketSize = 8060,
                Pooling = true
            };

            //security
            if ( !String.IsNullOrWhiteSpace( credentials?.Username ) ) {
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



        public static async Task StartAnySQLBrowsers( TimeSpan searchTimeout ) =>
            await FindAndStartSqlBrowserServices( new[] {
                "Thor" //my development server's name. Feel free to remove or add your own.
            }, searchTimeout ).ConfigureAwait( false );

        /// <summary>
        ///     Opens and then closes a <see cref="SqlConnection" />.
        /// </summary>
        /// <returns></returns>
        public Int32? ExecuteNonQuery( String query, [CanBeNull] params SqlParameter[] parameters ) {
            if ( query.IsNullOrWhiteSpace() ) {
                throw new ArgumentNullException( nameof( query ) );
            }

            try {
                using ( var connection = new SqlConnection( this._connectionString ) ) {
                    connection.Open();

                    using ( var command = new SqlCommand( query, connection ) {
                        CommandType = CommandType.StoredProcedure,
                        CommandTimeout = ( Int32 )this.CommandTimeout.TotalSeconds
                    } ) {
                        if ( null != parameters ) {
                            foreach ( var parameter in parameters ) {
                                Assert.StrictEqual( parameter, command.Parameters.Add( parameter ) );
                            }
                        }

                        return command.ExecuteNonQuery();
                    }
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

        public Int32? ExecuteNonQuery( String query, Int32 retries, [CanBeNull] params SqlParameter[] parameters ) {
            if ( query.IsNullOrWhiteSpace() ) {
                throw new ArgumentNullException( nameof( query ) );
            }

            TryAgain:
            --retries;

            try {
                using ( var connection = new SqlConnection( this._connectionString ) ) {
                    connection.Open();

                    using ( var command = new SqlCommand( query, connection ) {
                        CommandType = CommandType.StoredProcedure,
                        CommandTimeout = ( Int32 )this.CommandTimeout.TotalSeconds
                    } ) {
                        if ( null != parameters ) {
                            foreach ( var parameter in parameters ) {
                                Assert.StrictEqual( parameter, command.Parameters.Add( parameter ) );
                            }
                        }

                        return command.ExecuteNonQuery();
                    }
                }
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

        [ItemCanBeNull]
        public async Task<Int32?> ExecuteNonQueryAsync( String query, CommandType commandType, [CanBeNull] params SqlParameter[] parameters ) {
            if ( query.IsNullOrWhiteSpace() ) {
                throw new ArgumentNullException( nameof( query ) );
            }

            try {
                using ( var connection = new SqlConnection( this._connectionString ) ) {
                    try {
                        await connection.OpenAsync( this.Token ).ConfigureAwait( false );
                    }
                    catch ( SqlException exception ) {
                        exception.Log();

                        return default;
                    }

                    using ( var command = new SqlCommand( query, connection ) {
                        CommandType = commandType,
                        CommandTimeout = ( Int32 )this.CommandTimeout.TotalSeconds
                    } ) {
                        if ( null != parameters ) {
                            foreach ( var parameter in parameters ) {
                                Assert.StrictEqual( parameter, command.Parameters.Add( parameter ) );
                            }
                        }

                        try {
                            return await command.ExecuteNonQueryAsync( this.Token ).ConfigureAwait( false );
                        }
                        catch ( SqlException exception ) {
                            exception.Log();

                            return default;
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

            table = new DataTable();

            try {

                using ( var connection = new SqlConnection( this._connectionString ) ) {
                    connection.Open();

                    using ( var command = new SqlCommand( query, connection ) {
                        CommandType = commandType,
                        CommandTimeout = ( Int32 )this.CommandTimeout.TotalSeconds
                    } ) {
                        if ( null != parameters ) {
                            foreach ( var parameter in parameters ) {
                                Assert.StrictEqual( parameter, command.Parameters.Add( parameter ) );
                            }
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
        public async Task<SqlDataReader> ExecuteReaderAsyncDataReader( String query, CommandType commandType, [CanBeNull] params SqlParameter[] parameters ) {
            if ( query.IsNullOrWhiteSpace() ) {
                throw new ArgumentNullException( nameof( query ) );
            }

            try {
                using ( var connection = new SqlConnection( this._connectionString ) ) {
                    await connection.OpenAsync( this.Token ).ConfigureAwait( false );

                    using ( var command = new SqlCommand( query, connection ) {
                        CommandType = commandType,
                        CommandTimeout = ( Int32 )this.CommandTimeout.TotalSeconds
                    } ) {
                        if ( null != parameters ) {
                            foreach ( var parameter in parameters ) {
                                Assert.StrictEqual( parameter, command.Parameters.Add( parameter ) );
                            }
                        }

                        return await command.ExecuteReaderAsync( this.Token ).ConfigureAwait( false );
                    }
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

        /// <summary>
        ///     Returns a <see cref="DataTable" />
        /// </summary>
        /// <param name="query">      </param>
        /// <param name="commandType"></param>
        /// <param name="parameters"> </param>
        /// <returns></returns>
        [ItemNotNull]
        public async Task<DataTable> ExecuteReaderDataTableAsync( String query, CommandType commandType, [CanBeNull] params SqlParameter[] parameters ) {
            if ( query.IsNullOrWhiteSpace() ) {
                throw new ArgumentNullException( nameof( query ) );
            }

            var table = new DataTable();

            try {
                using ( var connection = new SqlConnection( this._connectionString ) ) {
                    await connection.OpenAsync( this.Token ).ConfigureAwait( false );

                    using ( var command = new SqlCommand( query, connection ) {
                        CommandType = commandType,
                        CommandTimeout = ( Int32 )this.CommandTimeout.TotalSeconds
                    } ) {
                        if ( null != parameters ) {
                            foreach ( var parameter in parameters ) {
                                Assert.StrictEqual( parameter, command.Parameters.Add( parameter ) );
                            }
                        }

                        table.BeginLoadData();

                        using ( var reader = await command.ExecuteReaderAsync( this.Token ).ConfigureAwait( false ) ) {
                            table.Load( reader );
                        }

                        table.EndLoadData();
                    }
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
        public (Status status, T result) ExecuteScalar<T>( String query, CommandType commandType, [CanBeNull] params SqlParameter[] parameters ) {
            if ( query.IsNullOrWhiteSpace() ) {
                throw new ArgumentNullException( nameof( query ) );
            }

            try {
                using ( var connection = new SqlConnection( this._connectionString ) ) {
                    connection.Open();

                    using ( var command = new SqlCommand( query, connection ) {
                        CommandType = commandType,
                        CommandTimeout = ( Int32 )this.CommandTimeout.TotalSeconds
                    } ) {
                        if ( null != parameters ) {
                            foreach ( var parameter in parameters ) {
                                Assert.StrictEqual( parameter, command.Parameters.Add( parameter ) );
                            }
                        }

                        var scalar = command.ExecuteScalar();

                        if ( null == scalar || Convert.IsDBNull( scalar ) || Convert.IsDBNull( scalar ) ) {
                            return (default, default);
                        }

                        if ( scalar is T executeScalar ) {
                            return (Status.Success, executeScalar);
                        }

                        if ( scalar.TryCast<T>( out var result ) ) {
                            return (Status.Success, result);
                        }

                        try {
                            result = ( T )Convert.ChangeType( scalar, typeof( T ) );
                        }
                        catch ( InvalidCastException ) {
                            return default;
                        }
                        catch ( FormatException ) {
                            return default;
                        }
                        catch ( OverflowException ) {
                            return default;
                        }
                        catch ( ArgumentNullException ) {
                            return default;
                        }

                        return (Status.Success, result);
                    }
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

        /// <summary>
        ///     <para>Returns the first column of the first row.</para>
        /// </summary>
        /// <param name="query">      </param>
        /// <param name="commandType"></param>
        /// <param name="parameters"> </param>
        /// <returns></returns>
        public async Task<(Status status, T result)> ExecuteScalarAsync<T>( String query, CommandType commandType, [CanBeNull] params SqlParameter[] parameters ) {
            if ( query.IsNullOrWhiteSpace() ) {
                throw new ArgumentNullException( nameof( query ) );
            }

            try {
                using ( var connection = new SqlConnection( this._connectionString ) ) {
                    try {
                        await connection.OpenAsync( this.Token ).ConfigureAwait( false );
                    }
                    catch ( SqlException ) {
                        return (Status.Failure, default); //login most likely failed.
                    }
                    catch ( Exception ) {
                        return (Status.Failure, default);
                    }

                    using ( var command = new SqlCommand( query, connection ) {
                        CommandType = commandType,
                        CommandTimeout = ( Int32 )this.CommandTimeout.TotalSeconds
                    } ) {
                        if ( null != parameters ) {
                            foreach ( var parameter in parameters ) {
                                Assert.StrictEqual( parameter, command.Parameters.Add( parameter ) );
                            }
                        }

                        var scalar = await command.ExecuteScalarAsync( this.Token ).ConfigureAwait( false );

                        if ( scalar == null || scalar == DBNull.Value || Convert.IsDBNull( scalar ) ) {
                            return (Status.Success, default);
                        }

                        if ( scalar is T executeScalar ) {
                            return (Status.Success, executeScalar);
                        }

                        if ( scalar.TryCast<T>( out var result ) ) {
                            return (Status.Success, result);
                        }

                        try {
                            return (Status.Success, ( T )Convert.ChangeType( scalar, typeof( T ) ));
                        }
                        catch ( InvalidCastException ) {
                            return default;
                        }
                        catch ( FormatException ) {
                            return default;
                        }
                        catch ( OverflowException ) {
                            return default;
                        }
                        catch ( ArgumentNullException ) {
                            return default;
                        }
                    }
                }
            }
            catch ( InvalidCastException exception ) {

                //TIP: check for SQLServer returning a Double when you expect a Single (float in SQL).
                exception.Log( Rebuild( query, parameters ) );
            }
            catch ( SqlException exception ) {
                exception.Log( Rebuild( query, parameters ) );
            }
            catch ( DbException exception ) {
                exception.Log( Rebuild( query, parameters ) );
            }

            return (Status.Failure, default);
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
                        CommandType = CommandType.StoredProcedure,
                        CommandTimeout = ( Int32 )this.CommandTimeout.TotalSeconds
                    } ) {
                        if ( null != parameters ) {
                            foreach ( var parameter in parameters ) {
                                Assert.StrictEqual( parameter, command.Parameters.Add( parameter ) );
                            }
                        }

                        using ( var reader = command.ExecuteReader() ) {
                            return GenericPopulator<TResult>.CreateList( reader );
                        }
                    }
                }
            }
            catch ( SqlException exception ) {
                exception.Log( Rebuild( query, parameters ) );
            }
            catch ( DbException exception ) {
                exception.Log( Rebuild( query, parameters ) );
            }

            return null;
        }

        /*
        private static (String query, IEnumerable<SqlParameter> parameters) Rebuild( [NotNull] String query, [NotNull] SqlParameterCollection parameters ) {
            if ( String.IsNullOrWhiteSpace( value: query ) ) {
                throw new ArgumentException( message: "Value cannot be null or whitespace.", paramName: nameof( query ) );
            }

            return ( $"{query.SingleQuote()};", parameters.Cast<SqlParameter>() );
        }
        */

    }
}