#region License & Information

// This notice must be kept visible in the source.
// 
// This section of source code belongs to Rick@AIBrain.Org unless otherwise specified, or the
// original license has been overwritten by the automatic formatting of this code. Any unmodified
// sections of source code borrowed from other projects retain their original license and thanks
// goes to the Authors.
// 
// Donations and Royalties can be paid via
// PayPal: paypal@aibrain.org
// bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
// bitcoin: 1NzEsF7eegeEWDr5Vr9sSSgtUC4aL6axJu
// litecoin: LeUxdU2w3o6pLZGVys5xpDZvvo8DUrjBp9
// 
// Usage of the source code or compiled binaries is AS-IS. I am not responsible for Anything You Do.
// 
// Contact me by email if you have any questions or helpful criticism.
// 
// "Librainian 2015/Database.cs" was last cleaned by aibra_000 on 2015/04/20 at 6:53 PM

#endregion License & Information

namespace Librainian.Database {
    using System;
    using System.Data;
    using System.Data.Common;
    using System.Data.SqlClient;
    using System.Diagnostics;
    using System.Net.NetworkInformation;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Windows.Forms;
    using FluentAssertions;
    using JetBrains.Annotations;
    using Measurement.Frequency;
    using Measurement.Time;
    using Properties;
    using Threading;

    public sealed class Database {

        // ReSharper disable once MemberCanBePrivate.Global
        public static Boolean IsNetworkConnected( int retries = 3 ) {
            var counter = retries;
            while ( !NetworkInterface.GetIsNetworkAvailable() && counter > 0 ) {
                --counter;
                $"Network disconnected. Waiting {Seconds.One}. {counter} retries left...".WriteLine();
                Thread.Sleep( Hertz.One );
            }
            return NetworkInterface.GetIsNetworkAvailable();
        }

        [NotNull]
        private readonly SqlConnectionStringBuilder _connectionStringBuilder;

        /*
                [CanBeNull]
                public static readonly ThreadLocal<String> ConnectionStrings = new ThreadLocal<string>( ( ) => {
                    try {
                        ConnectionStringBuilder.Should().NotBeNull( $"{nameof( ConnectionStringBuilder )} has not been built yet." );
                        ConnectionStringBuilder.ConnectionString.Should().BeNullOrEmpty( $"{nameof( ConnectionStringBuilder.ConnectionString )} has not been built yet." );
                        return ConnectionStringBuilder.ConnectionString;
                    }
                    catch ( Exception exception ) {
                        exception.More();
                        return null;
                    }
                } );
        */

        public Database( [NotNull] String library, [NotNull] String server, [CanBeNull] String catalog, [CanBeNull] String username, [CanBeNull] String password, int timeout = 3, ApplicationIntent intent = ApplicationIntent.ReadWrite, int retries = 2 ) {

            if ( library == null ) {
                throw new ArgumentNullException( nameof( library ) );
            }
            if ( server == null ) {
                throw new ArgumentNullException( nameof( server ) );
            }
            if ( catalog == null ) {
                throw new ArgumentNullException( nameof( catalog ) );
            }
            if ( username == null ) {
                throw new ArgumentNullException( nameof( username ) );
            }
            if ( password == null ) {
                throw new ArgumentNullException( nameof( password ) );
            }
            if ( timeout < 1 ) {
                throw new ArgumentOutOfRangeException( nameof( timeout ), timeout, Resources.Database_GetConnection_Timeout_is_less_than_one_ );
            }

            try {
                this._connectionStringBuilder = new SqlConnectionStringBuilder {
                    ApplicationIntent = intent,
                    ApplicationName = Application.ProductName,
                    AsynchronousProcessing = true,
                    ConnectRetryCount = retries,
                    ConnectTimeout = timeout,
                    DataSource = server,
                    InitialCatalog = catalog,
                    Password = password,
                    Pooling = true,
                    MultipleActiveResultSets = true,
                    NetworkLibrary = library,
                    UserID = username
                };
                this._connectionStringBuilder.Should().NotBeNull();
                return;
            }
            catch ( ArgumentException exception ) {
                exception.More();
            }
            catch ( SqlException exception ) {
                exception.More();
            }
            catch ( DbException exception ) {
                exception.More();
            }
            catch ( Exception exception ) {
                exception.More();
            }
            throw new InvalidOperationException();
        }

        /// <summary>
        /// We want one connection per thread..??
        /// </summary>
        [NotNull]
        private static readonly ThreadLocal<SqlConnection> Connections = new ThreadLocal<SqlConnection>( true );

        /// <summary>
        /// <para>Creates a <see cref="SqlConnection"/> if needed from the first available <see cref="_connectionStringBuilder"/>.</para>
        /// </summary>
        private void CreateConnection( ) {
            if ( !Connections.IsValueCreated ) {
                Connections.Value = new SqlConnection( this._connectionStringBuilder.ToString() );
#if DEBUG
                Connections.Value.InfoMessage += ( sender, args ) => args.Message.Info();
                Connections.Value.StateChange += ( sender, args ) => $"sql state changed from {args.OriginalState} to {args.CurrentState}".Info();
#endif
            }

            Connections.Should().NotBeNull( $"{nameof( Connections )} not connected on thread {Thread.CurrentThread.ManagedThreadId}" );
        }

        //public void Dispose( ) {
        //    try {
        //        //TODO
        //    }
        //    catch ( InvalidOperationException exception ) {
        //        exception.More();
        //    }

        //    try {
        //        using (this.Connection) {
        //            if ( this.Connection.State != ConnectionState.Closed ) {
        //                this.Connection.Close();
        //            }
        //        }
        //    }
        //    catch ( InvalidOperationException exception ) {
        //        exception.More();
        //    }
        //}

        ///// <summary>
        /////     The parameter collection for this database connection
        ///// </summary>
        //public SqlParameterCollection Params {
        //    get {
        //        this.Command.Should().NotBeNull();
        //        return this.Command.Parameters;
        //    }
        //}


        /*
                public void NonQuery( String cmdText ) {
                TryAgain:
                    try {

                        //var stopwatch = Stopwatch.StartNew();
                        if ( this.GetConnection() ) {
                            var command = new SqlCommand( cmdText );
                            command.ExecuteNonQuery();
                        }

                        //QueryAverages.AddOrUpdate( key: sproc, addValue: stopwatch.Elapsed, updateValueFactory: ( s, span ) => new Milliseconds( (Decimal ) ( QueryAverages[ sproc ].Add( stopwatch.Elapsed ).TotalMilliseconds/2.0 ) ) );
                        //foreach ( var pair in QueryAverages.Where( pair => pair.Value >= Seconds.One ) ) {
                        //    String.Format( "[{0}] average time is {1}", pair.Key, pair.Value.Simpler() ).TimeDebug();
                        //    TimeSpan value;
                        //    QueryAverages.TryRemove( pair.Key, out value );
                        //}

                        //if ( sproc.Contains( "Blink" ) ) { Generic.Report( String.Format( "Blink time average is {0}", QueryAverages[sproc].Simple() ) ); }
                    }
                    catch ( Exception exception ) {
                        var lower = exception.Message.ToLower();

                        if ( lower.Contains( "deadlocked" ) ) {
                            "deadlock.wav".TryPlayFile();
                            goto TryAgain;
                        }
                        if ( lower.Contains( "transport-level error" ) ) {
                            "lostconnection.wav".TryPlayFile();
                            goto TryAgain;
                        }
                        if ( lower.Contains( "timeout" ) ) {
                            "timeout.wav".TryPlayFile();
                            goto TryAgain;
                        }
                        throw;
                    }
                }
        */

        [CanBeNull]
        public SqlConnection OpenConnection( ) {

            CreateConnection();

            var retries = 10;
            TryAgain:
            try {
                switch ( Connections.Value.State ) {
                    case ConnectionState.Open:
                        return Connections.Value;

                    case ConnectionState.Executing:
                        return Connections.Value;

                    case ConnectionState.Fetching:
                        return Connections.Value;

                    case ConnectionState.Closed:
                        bool ret;
                        try {
                            Connections.Value.Should().NotBeNull();

                            if ( Connections.Value.State == ConnectionState.Open ) {
                                Connections.Value.Close();
                            }
                            Connections.Value.Open();

                            ret = true;
                        }
                        catch ( SqlException exception ) {
                            exception.More();
                            ret = false;
                        }
                        if ( ret ) {
                            return Connections.Value;
                        }
                        if ( retries > 0 ) {
                            --retries;
                            goto TryAgain;
                        }
                        break;

                    case ConnectionState.Connecting:
                        while ( Connections.Value.State == ConnectionState.Connecting ) {
                            Task.Delay( Milliseconds.TwoHundredEleven ).Wait();
                        }
                        return this.OpenConnection();

                    case ConnectionState.Broken:
                        bool ret1;
                        try {
                            Connections.Should().NotBeNull();

                            if ( Connections.Value.State == ConnectionState.Open ) {
                                Connections.Value.Close();
                            }
                            Connections.Value.Open();

                            ret1 = true;
                        }
                        catch ( SqlException exception ) {
                            exception.More();
                            ret1 = false;
                        }
                        if ( ret1 ) {
                            return Connections.Value;
                        }
                        if ( retries > 0 ) {
                            --retries;
                            goto TryAgain;
                        }
                        break;

                    default:
                        throw new ArgumentOutOfRangeException();
                }

                return Connections.Value;
            }
            catch ( SqlException exception ) {
                if ( !IsNetworkConnected() ) {
                    Task.Delay( Seconds.One ).Wait();
                    goto TryAgain;
                }
                exception.More();
            }
            catch ( InvalidOperationException exception ) {
                exception.More();
            }
            return null;
        }

        /// <summary>
        /// Returns a <see cref="DataTable"/>
        /// </summary>
        /// <param name="query"></param>
        /// <param name="commandType"></param>
        /// <param name="useTransaction"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        [NotNull]
        public DataTable Query( [NotNull] String query, CommandType commandType, Boolean useTransaction = true, params SqlParameter[] parameters ) {
            if ( query == null ) {
                throw new ArgumentNullException( nameof( query ) );
            }

            var table = new DataTable();

            SqlTransaction transaction = null;
            try {
                var command = new SqlCommand( query, this.OpenConnection() ) {
                    CommandType = commandType
                };

                if ( null != parameters ) {
                    command.Parameters.AddRange( parameters );
                }

                if ( useTransaction ) {
                    transaction = Connections.Value.BeginTransaction(); // Start a local transaction.
                    command.Transaction = transaction;
                }


                table.BeginLoadData();
                try {
                    using ( var reader = command.ExecuteReader() ) { table.Load( reader, LoadOption.OverwriteChanges ); }
                }
                finally {
                    transaction?.Commit(); // Attempt to commit the transaction.
                }
                table.EndLoadData();
           }
            catch ( SqlException exception) {
                exception.More();
                
                // Attempt to roll back the transaction.
                try {
                    transaction?.Rollback();
                }
                catch ( Exception exception2 ) {

                    // This catch block will handle any errors that may have occurred on the server
                    // that would cause the rollback to fail, such as a closed connection.
                    exception2.More();
                }
            }
            catch ( DbException exception) {
                exception.More();
            }
            catch ( Exception exception) {
                exception.More();
            }
            return table;
        }

        /// <summary>
        /// <para>Returns the first column of the first row.</para>
        /// </summary>
        /// <param name="query"></param>
        /// <param name="commandType"></param>
        /// <param name="useTransaction"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        [CanBeNull]
        public async Task<TResult> QueryScalarAsync<TResult>( [NotNull] String query, CommandType commandType, Boolean useTransaction = true, params SqlParameter[] parameters ) {
            if ( query == null ) {
                throw new ArgumentNullException( nameof( query ) );
            }
            SqlTransaction transaction = null;

            try {
                var command = new SqlCommand( query, this.OpenConnection() ) {
                    CommandType = commandType
                };

                if ( null != parameters ) {
                    command.Parameters.AddRange( parameters );
                }

                if ( useTransaction ) {
                    transaction = Connections.Value.BeginTransaction(); // Start a local transaction.
                    command.Transaction = transaction;
                }
                try {
                    var bob = await command.ExecuteScalarAsync();
                    return ( TResult )bob;
                }
                finally {
                    transaction?.Commit(); // Attempt to commit the transaction.
                }
            }
            catch ( SqlException exception ) {
                exception.More();

                // Attempt to roll back the transaction.
                try {
                    transaction?.Rollback();
                }
                catch ( Exception exception2 ) {

                    // This catch block will handle any errors that may have occurred on the server
                    // that would cause the rollback to fail, such as a closed connection.
                    exception2.More();
                }
            }
            return default(TResult);
        }

        /// <summary>
        /// <para>Returns the first column of the first row.</para>
        /// </summary>
        /// <param name="query"></param>
        /// <param name="commandType"></param>
        /// <param name="useTransaction"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        [CanBeNull]
        public TResult QueryScalar<TResult>( [NotNull] String query, CommandType commandType, Boolean useTransaction = true, params SqlParameter[] parameters ) {
            if ( query == null ) {
                throw new ArgumentNullException( nameof( query ) );
            }
            SqlTransaction transaction = null;

            try {
                var command = new SqlCommand( query, this.OpenConnection() ) {
                    CommandType = commandType
                };

                if ( null != parameters ) {
                    command.Parameters.AddRange( parameters );
                }

                if ( useTransaction ) {
                    transaction = Connections.Value.BeginTransaction(); // Start a local transaction.
                    command.Transaction = transaction;
                }
                try {
                    var bob = command.ExecuteScalar();
                    return ( TResult )bob;
                }
                finally {
                    transaction?.Commit(); // Attempt to commit the transaction.
                }
            }
            catch ( SqlException exception ) {
                exception.More();

                // Attempt to roll back the transaction.
                try {
                    transaction?.Rollback();
                }
                catch ( Exception exception2 ) {

                    // This catch block will handle any errors that may have occurred on the server
                    // that would cause the rollback to fail, such as a closed connection.
                    exception2.More();
                }
            }
            catch ( DbException exception ) {
                exception.More();

                // Attempt to roll back the transaction.
                try {
                    transaction?.Rollback();
                }
                catch ( Exception exception2 ) {

                    // This catch block will handle any errors that may have occurred on the server
                    // that would cause the rollback to fail, such as a closed connection.
                    exception2.More();
                }
            }
            return default(TResult);
        }

        [CanBeNull]
        public async Task<int?> QueryWithNoResultAsync( String query, CommandType commandType, Boolean useTransaction = true, params SqlParameter[] parameters ) {
            SqlTransaction transaction = null;
            try {
                var command = new SqlCommand( query, this.OpenConnection() ) {
                    CommandType = commandType
                };

                if ( null != parameters ) {
                    command.Parameters.AddRange( parameters );
                }

                if ( useTransaction ) {
                    transaction = Connections.Value.BeginTransaction(); // Start a local transaction.
                    command.Transaction = transaction;
                }

                try {
                    var bob = await command.ExecuteNonQueryAsync();
                    return bob;
                }
                finally {
                    transaction?.Commit(); // Attempt to commit the transaction.
                }
            }
            catch ( SqlException exception ) {
                exception.More();
                try {
                    transaction?.Rollback();
                }
                catch ( Exception exception2 ) {
                    exception2.More(); // This catch block will handle any errors that may have occurred on the server that would cause the rollback to fail, such as a closed connection.
                }
            }
            catch ( DbException exception ) {
                exception.More();
                try {
                    transaction?.Rollback();
                }
                catch ( Exception exception2 ) {

                    // This catch block will handle any errors that may have occurred on the server
                    // that would cause the rollback to fail, such as a closed connection.
                    exception2.More();
                }
            }
            return null;
        }

        public Boolean QueryWithNoResult( String query, CommandType commandType, Boolean useTransaction = true, params SqlParameter[] parameters ) {
            SqlTransaction transaction = null;
            try {
                var command = new SqlCommand( query, this.OpenConnection() ) {
                    CommandType = commandType
                };

                if ( null != parameters ) {
                    command.Parameters.AddRange( parameters );
                }

                if ( useTransaction ) {
                    transaction = Connections.Value.BeginTransaction(); // Start a local transaction.
                    command.Transaction = transaction;
                }

                try {
                    command.ExecuteNonQuery();
                    return true;
                }
                finally {
                    transaction?.Commit(); // Attempt to commit the transaction.
                }
            }
            catch ( SqlException exception ) {
                exception.More();
                try {
                    transaction?.Rollback();
                }
                catch ( Exception exception2 ) {
                    exception2.More(); // This catch block will handle any errors that may have occurred on the server that would cause the rollback to fail, such as a closed connection.
                }
            }
            catch ( DbException exception ) {
                exception.More();
                try {
                    transaction?.Rollback();
                }
                catch ( Exception exception2 ) {

                    // This catch block will handle any errors that may have occurred on the server
                    // that would cause the rollback to fail, such as a closed connection.
                    exception2.More();
                }
            }
            return false;
        }

        /// <summary>
        /// </summary>
        /// <param name="query"></param>
        /// <param name="commandType"></param>
        /// <param name="useTransaction"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        [CanBeNull]
        public async Task<SqlDataReader> QueryWithResultAsync( String query, CommandType commandType, Boolean useTransaction = true, params SqlParameter[] parameters ) {

            //TryAgain:
            try {
                var command = new SqlCommand( query, this.OpenConnection() ) {
                    CommandType = commandType
                };
                if ( null != parameters ) {
                    command.Parameters.AddRange( parameters );
                }
                return await command.ExecuteReaderAsync();
            }
            catch ( SqlException exception ) {
                exception.More();
            }
            return null;
        }

    }
}