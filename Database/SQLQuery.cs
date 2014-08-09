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
// "Librainian2/SQLQuery.cs" was last cleaned by Rick on 2014/08/08 at 2:26 PM

#endregion License & Information

namespace Librainian.Database {

    using System;
    using System.Collections.Concurrent;
    using System.Data;
    using System.Data.SqlClient;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web.Caching;
    using Annotations;
    using FluentAssertions;
    using Measurement.Time;
    using Threading;

    [Obsolete( "No access to a local server atm." )]
    public sealed class SQLQuery : IDisposable {

        //[Obsolete( "No access to a local server atm." )]
        //private static readonly string SQLConnectionString = new SqlConnectionStringBuilder {
        //    ApplicationIntent = ApplicationIntent.ReadWrite,
        //    ApplicationName = Parameters.EngineName,
        //    AsynchronousProcessing = true,
        //    ConnectTimeout = ( int )Parameters.Databases.Timeout.TotalSeconds,
        //    DataSource = Parameters.Databases.MainServer.Server,
        //    Pooling = true,
        //    MaxPoolSize = 1024,
        //    MinPoolSize = 10,
        //    InitialCatalog = Parameters.EngineName,
        //    MultipleActiveResultSets = true,
        //    NetworkLibrary = Parameters.Databases.MainServer.Library,
        //    UserID = Parameters.Databases.MainServer.UserName,
        //    Password = Parameters.Databases.MainServer.Password
        //}.ConnectionString;

        public static readonly ConcurrentDictionary<String, TimeSpan> QueryAverages = new ConcurrentDictionary<String, TimeSpan>();

        public readonly Cache Cash = new Cache(); //TODO

        public readonly SqlCommand Command = new SqlCommand();

        public readonly SqlConnection Connection = new SqlConnection();

        internal readonly String Server;
        internal String Library;

        internal String Password;

        internal Stopwatch SinceOpened = Stopwatch.StartNew();

        internal String UserName;

        /// <summary>
        /// Create a database object to MainServer
        /// </summary>
        [Obsolete( "No access to a local server atm." )]
        public SQLQuery( [NotNull] String library, [NotNull] String server, [NotNull] String username, [NotNull] String password, [NotNull] String sproc ) {
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
            if ( sproc == null ) {
                throw new ArgumentNullException( "sproc" );
            }
            this.Library = library;
            this.Server = server;
            this.UserName = username;
            this.Password = password;
            this.Command.CommandType = CommandType.StoredProcedure;

            //this.Command.CommandTimeout = (Minute.One).;

            //Utilities.IO.ExtensionMethods.

            this.Cash.Should().NotBeNull();
        }

        /// <summary>
        /// The parameter collection for this database connection
        /// </summary>
        public SqlParameterCollection Params {
            get {
                this.Command.Should().NotBeNull();
                return this.Command.Parameters;
            }
        }

        private Task ExecuteNonQueryAsyncTask { get; set; }

        public void Dispose() {
            if ( null != this.ExecuteNonQueryAsyncTask ) {
                return;
            }
            try {
                if ( null != this.Command ) {
                    this.Command.Dispose();
                }
            }
            catch ( InvalidOperationException exception ) {
                exception.Log();
            }

            try {
                if ( null != this.Connection ) {
                    this.Connection.Dispose();
                }
            }
            catch ( InvalidOperationException exception ) {
                exception.Log();
            }
        }

        public void NonQuery( String sproc ) {
        TryAgain:
            try {
                var stopwatch = Stopwatch.StartNew();
                if ( this.Open() ) {
                    this.Command.CommandText = sproc;
                    this.Command.ExecuteNonQuery();
                }
                QueryAverages.AddOrUpdate( key: sproc, addValue: stopwatch.Elapsed, updateValueFactory: ( s, span ) => new Milliseconds( ( decimal )( QueryAverages[ sproc ].Add( stopwatch.Elapsed ).TotalMilliseconds / 2.0 ) ) );
                foreach ( var pair in QueryAverages.Where( pair => pair.Value >= Seconds.One ) ) {
                    String.Format( "[{0}] average time is {1}", pair.Key, pair.Value.Simpler() ).TimeDebug();
                    TimeSpan value;
                    QueryAverages.TryRemove( pair.Key, out value );
                }

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

        [Obsolete]
        public async void NonQueryAsync( String sproc ) {
        TryAgain:
            try {
                if ( this.Open() ) {
                    if ( null != this.ExecuteNonQueryAsyncTask ) {
                        await this.ExecuteNonQueryAsyncTask;
                    }

                    this.Command.CommandText = sproc;
                    this.ExecuteNonQueryAsyncTask = this.Command.ExecuteNonQueryAsync();

                    //this.ExecuteNonQueryAsyncTask.ContinueWith( task => {
                    //    var command = this.Command;
                    //    if ( command != null ) {
                    //        command.Dispose();
                    //    }
                    //    var connection = this.Connection;
                    //    if ( connection != null ) {
                    //        connection.Dispose();
                    //    }
                    //}, TaskContinuationOptions.ExecuteSynchronously );
                }
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

        public DataTableReader Query( String sproc ) {
        TryAgain:
            try {
                var stopwatch = Stopwatch.StartNew();

                if ( this.Open() ) {
                    this.Command.CommandText = sproc;

                    var table = new DataTable();
                    table.BeginLoadData();
                    using ( var reader = this.Command.ExecuteReader() ) {
                        table.Load( reader, LoadOption.OverwriteChanges );
                    }
                    table.EndLoadData();

                    QueryAverages.AddOrUpdate( key: sproc, addValue: stopwatch.Elapsed, updateValueFactory: ( s, span ) => new Milliseconds( ( decimal )( QueryAverages[ sproc ].Add( stopwatch.Elapsed ).TotalMilliseconds / 2.0 ) ) );

                    return table.CreateDataReader();
                }
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
                exception.Log();
                throw;
            }
            return null;
        }

        internal Boolean Open( TimeSpan? timeout = null ) {
        TryAgain:
            try {
                if ( String.IsNullOrWhiteSpace( this.Connection.ConnectionString ) ) {

                    //this.Connection.ConnectionString = SQLConnectionString;
                    this.Connection.InfoMessage += ( sender, sqlInfoMessageEventArgs ) => String.Format( "[{0}] {1}", this.Server, sqlInfoMessageEventArgs.Message ).TimeDebug();
                }

                if ( this.SinceOpened.Elapsed > timeout ) {
                    if ( this.Connection.State == ConnectionState.Open ) {
                        this.Connection.Close();
                    }
                }

                if ( this.Connection.State == ConnectionState.Closed ) {
                    this.Connection.Open();
                    this.SinceOpened = Stopwatch.StartNew();
                }
                if ( null == this.Command.Connection ) {
                    this.Command.Connection = this.Connection;
                }
                return true;
            }
            catch ( SqlException exception ) {
                if ( !SQLDatabaseExtensions.IsNetworkConnected() ) {
                    Task.Delay( Seconds.One ).Wait();
                    goto TryAgain;
                }
                exception.Log();
            }
            catch ( InvalidOperationException exception ) {
                exception.Log();
            }
            return false;
        }

        //public async Task<SqlDataReader> QueryAsync( String sproc ) {
        //TryAgain:
        //    try {
        //        if ( null != SqlDataReadTask ) {
        //            await SqlDataReadTask;
        //        }

        //        if ( this.Open() ) {
        //            this.Command.CommandText = sproc;
        //            this.SqlDataReadTask = this.Command.ExecuteReaderAsync();
        //            return await this.SqlDataReadTask;
        //        }
        //    }
        //    catch ( Exception exception ) {
        //        var lower = exception.Message.ToLower();
        //        if ( lower.Contains( "deadlocked" ) ) {
        //            SQLDatabaseExtensions.PlayFile( "deadlock.wav" );
        //            goto TryAgain;
        //        }
        //        if ( lower.Contains( "transport-level error" ) ) {
        //            SQLDatabaseExtensions.PlayFile( "lostconnection.wav" );
        //            goto TryAgain;
        //        }
        //        if ( lower.Contains( "timeout" ) ) {
        //            SQLDatabaseExtensions.PlayFile( "timeout.wav" );
        //            goto TryAgain;
        //        }
        //        exception.Log();
        //        throw;
        //    }
        //    return null;
        //}

        //protected Task< SqlDataReader > SqlDataReadTask { get; set; }
        /*
                [Obsolete]
                public void PushNonQuery( String sproc ) {
                    TryAgain:
                    try {
                        if ( this.Open() ) {

                            //Generic.Report( String.Format( "{0}", Thread.CurrentThread.ManagedThreadId ) );
                            this.Command.CommandText = sproc;
                            this.Command.BeginExecuteNonQuery( ar => {

                                                                   //Generic.Report( String.Format( "{0}", Thread.CurrentThread.ManagedThreadId ) );
                                                                   this.Command.EndExecuteNonQuery( ar );
                                                               },
                                                               null );
                        }
                    }
                    catch ( Exception exception ) {
                        var lower = exception.Message.ToLower();

                        if ( lower.Contains( "deadlocked" ) ) {
                            SQLDatabaseExtensions.PlayFile( "deadlock.wav" );
                            goto TryAgain;
                        }
                        if ( lower.Contains( "transport-level error" ) ) {
                            SQLDatabaseExtensions.PlayFile( "lostconnection.wav" );
                            goto TryAgain;
                        }
                        if ( lower.Contains( "timeout" ) ) {
                            SQLDatabaseExtensions.PlayFile( "timeout.wav" );
                            goto TryAgain;
                        }
                        throw;
                    }
                }
        */
    }
}