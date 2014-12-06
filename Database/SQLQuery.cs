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
// "Librainian/SQLQuery.cs" was last cleaned by Rick on 2014/11/26 at 1:45 PM

#endregion License & Information

namespace Librainian.Database {

    using System;
    using System.Data;
    using System.Data.SqlClient;
    using System.Diagnostics;
    using System.Net.NetworkInformation;
    using System.Threading;
    using System.Threading.Tasks;
    using Annotations;
    using Measurement.Time;
    using Threading;

    public sealed class SQLQuery : IDisposable {

        /*
                private static readonly String SQLConnectionString = new SqlConnectionStringBuilder {
                    ApplicationIntent = ApplicationIntent.ReadWrite,
                    ApplicationName = Application.ProductName,
                    AsynchronousProcessing = true,
                    ConnectTimeout = ( int )Parameters.Databases.Timeout.TotalSeconds,
                    DataSource = Parameters.Databases.MainServer.Server,
                    Pooling = true,
                    MaxPoolSize = 1024,
                    MinPoolSize = 10,
                    InitialCatalog = Parameters.EngineName,
                    MultipleActiveResultSets = true,
                    NetworkLibrary = Parameters.Databases.MainServer.Library,
                    UserID = Parameters.Databases.MainServer.UserName,
                    Password = Parameters.Databases.MainServer.Password
                }.ConnectionString;
        */

        //public static readonly ConcurrentDictionary< String, TimeSpan > QueryAverages = new ConcurrentDictionary< String, TimeSpan >();

        //public readonly Cache Cache = new Cache(); //TODO

        //public readonly SqlCommand Command = new SqlCommand();

        public static ThreadLocal<SQLQuery> Queries = new ThreadLocal<SQLQuery>( () => new SQLQuery( "dbmssocn", "127.0.0.1", "Anonymous", "Anonymous" ) );

        public SqlConnection Connection = new SqlConnection();
        internal readonly String Library;
        internal readonly String Password;
        internal readonly String Server;
        internal readonly String UserName;
        internal Stopwatch SinceOpened = Stopwatch.StartNew();

        /// <summary>
        ///     Create a database object to MainServer
        /// </summary>
        public SQLQuery( [NotNull] String library, [NotNull] String server, [NotNull] String username, [NotNull] String password ) {
            if ( library == null ) {
                throw new ArgumentNullException( "library" );
            }
            if ( server == null ) {
                throw new ArgumentNullException( "server" );
            }
            if ( username == null ) {
                throw new ArgumentNullException( "username" );
            }
            if ( password == null ) {
                throw new ArgumentNullException( "password" );
            }
            this.Library = library;
            this.Server = server;
            this.UserName = username;
            this.Password = password;

            //this.Command.CommandType = CommandType.StoredProcedure;
            //this.Command.CommandTimeout = ( int ) Seconds.Thirty.Value;
        }

        ///// <summary>
        /////     The parameter collection for this database connection
        ///// </summary>
        //public SqlParameterCollection Params {
        //    get {
        //        this.Command.Should().NotBeNull();
        //        return this.Command.Parameters;
        //    }
        //}

        public static DataTable Convert( SqlDataReader dataReader ) {
            var table = new DataTable();
            table.BeginLoadData();
            if ( dataReader != null ) {
                table.Load( dataReader, LoadOption.OverwriteChanges );
            }
            table.EndLoadData();
            return table;
        }

        public static Boolean IsNetworkConnected( int retries = 3 ) {
            var counter = retries;
            while ( !NetworkInterface.GetIsNetworkAvailable() && counter > 0 ) {
                --counter;
                String.Format( "Network disconnected. Waiting {0}. {1} retries...", Seconds.One, counter ).WriteLine();
                Thread.Sleep( 1000 );
            }
            return NetworkInterface.GetIsNetworkAvailable();
        }

        public void Dispose() {
            try {
            }
            catch ( InvalidOperationException exception ) {
                exception.Error();
            }

            try {
                if ( null != this.Connection ) {
                    using ( Connection ) {
                        if ( Connection.State != ConnectionState.Closed ) {
                            Connection.Close();
                        }
                    }
                }
            }
            catch ( InvalidOperationException exception ) {
                exception.Error();
            }
            this.Connection = null;
        }

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
        public SqlConnection GetConnection() {
            var retries = 10;
        TryAgain:
            try {
                if ( String.IsNullOrWhiteSpace( this.Connection.ConnectionString ) ) {

                    //this.Connection.ConnectionString = SQLConnectionString;
                    this.Connection.InfoMessage += ( sender, sqlInfoMessageEventArgs ) => {
                                                       String.Format( "[{0}] {1}", this.Server, sqlInfoMessageEventArgs.Message ).WriteLine();
                                                   };
                }

                //if ( this.SinceOpened.Elapsed > timeout && this.Connection.State == ConnectionState.Open ) {
                //    this.Connection.Close();
                //}

                switch ( this.Connection.State ) {
                    case ConnectionState.Open:
                        break;

                    case ConnectionState.Executing:
                        break;

                    case ConnectionState.Fetching:
                        break;

                    case ConnectionState.Closed:
                        this.Connection.Open();
                        this.SinceOpened = Stopwatch.StartNew();
                        --retries;
                        if ( retries > 0 ) {
                            goto TryAgain;
                        }
                        break;

                    case ConnectionState.Connecting:
                        while ( this.Connection.State == ConnectionState.Connecting ) {
                            Task.Delay( Milliseconds.TwoHundredEleven ).Wait();
                        }
                        return GetConnection();

                    case ConnectionState.Broken:
                        this.Connection.Open();
                        this.SinceOpened = Stopwatch.StartNew();
                        --retries;
                        if ( retries > 0 ) {
                            goto TryAgain;
                        }
                        break;

                    default:
                        throw new ArgumentOutOfRangeException();
                }

                return this.Connection;
            }
            catch ( SqlException exception ) {
                if ( !IsNetworkConnected() ) {
                    Task.Delay( Seconds.One ).Wait();
                    goto TryAgain;
                }
                exception.Error();
            }
            catch ( InvalidOperationException exception ) {
                exception.Error();
            }
            return null;
        }

        //[CanBeNull]
        //public DataTableReader Query( String sproc ) {
        //TryAgain:
        //    try {
        //        var stopwatch = Stopwatch.StartNew();

        //        if ( this.GetConnection() ) {
        //            this.Command.CommandText = sproc;

        //            var table = new DataTable();
        //            table.BeginLoadData();
        //            using ( var reader = this.Command.ExecuteReader() ) {
        //                table.Load( reader, LoadOption.OverwriteChanges );
        //            }
        //            table.EndLoadData();

        //            QueryAverages.AddOrUpdate( key: sproc, addValue: stopwatch.Elapsed, updateValueFactory: ( s, span ) => new Milliseconds( ( Decimal )( QueryAverages[ sproc ].Add( stopwatch.Elapsed ).TotalMilliseconds / 2.0 ) ) );

        //            return table.CreateDataReader();
        //        }
        //    }
        //    catch ( Exception exception ) {
        //        var lower = exception.Message.ToLower();
        //        if ( lower.Contains( "deadlocked" ) ) {
        //            "deadlock.wav".TryPlayFile();
        //            goto TryAgain;
        //        }
        //        if ( lower.Contains( "transport-level error" ) ) {
        //            "lostconnection.wav".TryPlayFile();
        //            goto TryAgain;
        //        }
        //        if ( lower.Contains( "timeout" ) ) {
        //            "timeout.wav".TryPlayFile();
        //            goto TryAgain;
        //        }
        //        exception.Error();
        //        throw;
        //    }
        //    return null;
        //}

        /// <summary>
        /// Returns the first column of the first row.
        /// </summary>
        /// <param name="query"></param>
        /// <param name="commandType"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        [ CanBeNull ]
        public async Task<Object> QueryScalarAsync( String query, CommandType commandType, params SqlParameter[] parameters ) {
        TryAgain:
            try {
                var command = new SqlCommand( query, GetConnection() ) {
                    CommandType = commandType
                };
                if ( null != parameters ) {
                    command.Parameters.AddRange( parameters );
                }
                return await command.ExecuteScalarAsync();
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

        public async Task QueryWithNoResultAsync( String query, CommandType commandType, params SqlParameter[] parameters ) {
        TryAgain:
            try {
                var command = new SqlCommand( query, GetConnection() ) {
                    CommandType = commandType
                };
                if ( null != parameters ) {
                    command.Parameters.AddRange( parameters );
                }
                await command.ExecuteNonQueryAsync();
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

        /// <summary>
        /// </summary>
        /// <param name="query"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public async Task<SqlDataReader> QueryWithResultAsync( String query, CommandType commandType, params SqlParameter[] parameters ) {
        TryAgain:
            try {
                var command = new SqlCommand( query, GetConnection() ) {
                    CommandType = commandType
                };
                if ( null != parameters ) {
                    command.Parameters.AddRange( parameters );
                }
                return await command.ExecuteReaderAsync();
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
    }
}