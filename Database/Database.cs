// Copyright 2015 Rick@AIBrain.org.
// 
// This notice must be kept visible in the source.
// 
// This section of source code belongs to Rick@AIBrain.Org unless otherwise specified, or the original license has been overwritten by the automatic formatting of this code.
// Any unmodified sections of source code borrowed from other projects retain their original license and thanks goes to the Authors.
// 
// Donations and royalties can be paid via
// PayPal: paypal@aibrain.org
// bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
// litecoin: LeUxdU2w3o6pLZGVys5xpDZvvo8DUrjBp9
// 
// Usage of the source code or compiled binaries is AS-IS.
// I am not responsible for Anything You Do.
// 
// Contact me by email if you have any questions or helpful criticism.
//  
// "Librainian/Database.cs" was last cleaned by Rick on 2015/10/26 at 10:15 AM

namespace Librainian.Database {

    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Common;
    using System.Data.SqlClient;
    using System.Net.NetworkInformation;
    using System.Threading;
    using System.Threading.Tasks;
    using Extensions;
    using FluentAssertions;
    using JetBrains.Annotations;
    using Magic;
    using Measurement.Frequency;
    using Measurement.Time;
    using Properties;

    public sealed class Database : BetterDisposableClass {

        //[NotNull]private readonly SqlConnectionStringBuilder _connectionStringBuilder;

        private readonly String _connectionString;

        /// <summary>
        ///     We want one connection per thread..??
        /// </summary>
        [NotNull]
        private static readonly ThreadLocal<SqlConnection> Connections = new ThreadLocal<SqlConnection>( true );

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

        //public struct ConnectionInfo {
        //    public String Library {
        //        get; set;
        //    }
        //    public String Server {
        //        get; set;
        //    }
        //    public String Catalog {
        //        get; set;
        //    }
        //    public String Username {
        //        get; set;
        //    }
        //    public String Password {
        //        get; set;
        //    }
        //}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="timeout"></param>
        /// <param name="intent"></param>
        /// <param name="retries"></param>
        /// <exception cref="InvalidOperationException"></exception>
        public Database( String connectionString, Int32 timeout = 5, ApplicationIntent intent = ApplicationIntent.ReadWrite, Int32 retries = 5 ) {
            //if ( connectionInfo.Library == null ) {
            //    throw new ArgumentNullException( nameof( connectionInfo.Library ) );
            //}
            //if ( connectionInfo.Server == null ) {
            //    throw new ArgumentNullException( nameof( connectionInfo.Server ) );
            //}
            //if ( connectionInfo.Catalog == null ) {
            //    throw new ArgumentNullException( nameof( connectionInfo.Catalog ) );
            //}
            //if ( connectionInfo.Username == null ) {
            //    throw new ArgumentNullException( nameof( connectionInfo.Username ) );
            //}
            //if ( connectionInfo.Password == null ) {
            //    throw new ArgumentNullException( nameof( connectionInfo.Password ) );
            //}
            if ( timeout < 1 ) {
                throw new ArgumentOutOfRangeException( nameof( timeout ), timeout, Resources.Database_GetConnection_Timeout_is_less_than_one_ );
            }

            try {
                //var connectionStringBuilder = new SqlConnectionStringBuilder( connectionString );

                //{
                //    ApplicationIntent = intent,
                //    ApplicationName = Application.ProductName,
                //    AsynchronousProcessing = true,
                //    ConnectRetryCount = retries,
                //    ConnectTimeout = timeout,
                //    DataSource = connectionInfo.Server,
                //    InitialCatalog = connectionInfo.Catalog,
                //    Password = connectionInfo.Password,
                //    Pooling = true,
                //    MultipleActiveResultSets = true,
                //    //NetworkLibrary = connectionInfo.Library,
                //    UserID = connectionInfo.Username,
                //};
                //connectionStringBuilder.Should().NotBeNull();
                this._connectionString = connectionString;
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

        protected override void CleanUpManagedResources() {
            //if ( Connections.Values != null ) {
            //    foreach ( var sqlConnection in Connections.Values ) {
            //        if ( !Connections.IsValueCreated ) {
            //            continue;
            //        }
            //        if ( sqlConnection.State != ConnectionState.Closed ) {
            //            try {
            //                sqlConnection.Close();
            //            }
            //            catch ( Exception exception ) {
            //                exception.More();
            //            }
            //        }
            //    }
            //}
            base.CleanUpManagedResources();
        }

        // ReSharper disable once MemberCanBePrivate.Global
        public static Boolean IsNetworkConnected( Int32 retries = 3 ) {
            var counter = retries;
            while ( !NetworkInterface.GetIsNetworkAvailable() && ( counter > 0 ) ) {
                --counter;
                $"Network disconnected. Waiting {Seconds.One}. {counter} retries left...".WriteLine();
                Thread.Sleep( Hertz.One );
            }
            return NetworkInterface.GetIsNetworkAvailable();
        }

        /// <summary>
        ///     <para>Creates a <see cref="SqlConnection" /> if needed, using <see cref="_connectionString" />.</para>
        /// </summary>
        private void CreateConnection() {
            if ( !Connections.IsValueCreated ) {
                // ReSharper disable once UseObjectOrCollectionInitializer
                Connections.Value = new SqlConnection( this._connectionString );
#if DEBUG
                Connections.Value.InfoMessage += ( sender, args ) => args.Message.Info();
                Connections.Value.StateChange += ( sender, args ) => $"sql state changed from {args.OriginalState} to {args.CurrentState}".Info();
#endif
            }

            Connections.Should()
                       .NotBeNull( $"{nameof( Connections )} not connected on thread {Thread.CurrentThread.ManagedThreadId}" );
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
        public SqlConnection OpenConnection() {
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
                        Boolean ret;
                        try {
                            Connections.Value.Should()
                                       .NotBeNull();

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
                            Task.Delay( Milliseconds.TwoHundredEleven )
                                .Wait();
                        }
                        return this.OpenConnection();

                    case ConnectionState.Broken:
                        Boolean ret1;
                        try {
                            Connections.Should()
                                       .NotBeNull();

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
                    Task.Delay( Seconds.One )
                        .Wait();
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
        ///     Returns a <see cref="DataTable" />
        /// </summary>
        /// <param name="query"></param>
        /// <param name="commandType"></param>
        /// <param name="useTransaction"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        [NotNull]
        public DataTable Query( [NotNull] String query, CommandType commandType, Boolean useTransaction = false, params SqlParameter[] parameters ) {
            if ( query == null ) {
                throw new ArgumentNullException( nameof( query ) );
            }

            var table = new DataTable();

            SqlTransaction transaction = null;
            try {
                var command = new SqlCommand( query, this.OpenConnection() ) { CommandType = commandType };

                if ( null != parameters ) {
                    command.Parameters.AddRange( parameters );
                }

                if ( useTransaction ) {
                    transaction = Connections.Value.BeginTransaction(); // Start a local transaction.
                    command.Transaction = transaction;
                }

                table.BeginLoadData();
                try {
                    using ( var reader = command.ExecuteReader() ) {
                        table.Load( reader, LoadOption.OverwriteChanges );
                    }
                }
                finally {
                    transaction?.Commit(); // Attempt to commit the transaction.
                }
                table.EndLoadData();
            }
            catch ( SqlException exception ) {
                exception.More();

                // Attempt to roll back the transaction.
                try {
                    transaction?.Rollback();
                }
                catch ( Exception exception2 ) {
                    // This catch block will handle any errors that may have occurred on the Server
                    // that would cause the rollback to fail, such as a closed connection.
                    exception2.More();
                }
            }
            catch ( DbException exception ) {
                exception.More();
            }
            catch ( Exception exception ) {
                exception.More();
            }
            return table;
        }

        /// <summary>
        ///     Returns a <see cref="DataTable" />
        /// </summary>
        /// <param name="query"></param>
        /// <param name="commandType"></param>
        /// <param name="useTransaction"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        [NotNull]
        public IEnumerable<TResult> QueryList<TResult>( [NotNull] String query, CommandType commandType, Boolean useTransaction = false, params SqlParameter[] parameters ) {
            if ( query == null ) {
                throw new ArgumentNullException( nameof( query ) );
            }

            //var table = new DataTable();
            var list = new List< TResult >();

            SqlTransaction transaction = null;
            try {
                var command = new SqlCommand( query, this.OpenConnection() ) { CommandType = commandType };

                if ( null != parameters ) {
                    command.Parameters.AddRange( parameters );
                }

                if ( useTransaction ) {
                    transaction = Connections.Value.BeginTransaction(); // Start a local transaction.
                    command.Transaction = transaction;
                }

                //table.BeginLoadData();
                try {
                    using ( var reader = command.ExecuteReader() ) {
                        //table.Load( reader, LoadOption.OverwriteChanges );
                        var data = new GenericPopulator< TResult >().CreateList( reader );
                    }
                }
                finally {
                    transaction?.Commit(); // Attempt to commit the transaction.
                }
                //table.EndLoadData();
            }
            catch ( SqlException exception ) {
                exception.More();

                // Attempt to roll back the transaction.
                try {
                    transaction?.Rollback();
                }
                catch ( Exception exception2 ) {
                    // This catch block will handle any errors that may have occurred on the Server
                    // that would cause the rollback to fail, such as a closed connection.
                    exception2.More();
                }
            }
            catch ( DbException exception ) {
                exception.More();
            }
            catch ( Exception exception ) {
                exception.More();
            }

            /* erroring out
            foreach ( DataRowCollection rowCollection in table.Rows ) {
                if ( rowCollection[ 0 ][ 0 ] is TResult ) {
                    yield return ( TResult ) rowCollection[ 0 ][ 0 ];
                }
            }
            */

            return list;

            //var bob = table.ToList< TResult >();


            //foreach ( var row in bob ) {
            //    yield return row;
            //}

            //foreach ( DataRow row in table.Rows ) {
            //    //if ( row[ 0 ] is TResult ) {
            //        yield return ( TResult ) row[ 0 ];
            //    //}
            //}
        }

      

        ///// <summary>
        ///// Converts a DataTable to a list with generic objects
        ///// </summary>
        ///// <typeparam name="T">Generic object</typeparam>
        ///// <param name="table">DataTable</param>
        ///// <returns>List with generic objects</returns>
        //public static List<T> DataTableToList<T>( DataTable table ) {
        //    try {
        //        List<T> list = new List<T>();

        //        foreach ( var row in table.AsEnumerable() ) {
        //            Object source = row[ 0 ];
        //            //T obj = new T();
        //            Object obj = Activator.CreateInstance<T>();

        //            source.DeepClone( obj );
        //            //foreach ( var prop in obj.GetType().GetProperties() ) {
        //            //    try {
        //            //        PropertyInfo propertyInfo = obj.GetType().GetProperty( prop.Name );
        //            //        propertyInfo.SetValue( obj, Convert.ChangeType( row[ prop.Name ], propertyInfo.PropertyType ), null );
        //            //    }
        //            //    catch { }
        //            //}

        //            list.Add( ( T )obj );
        //        }

        //        return list;
        //    }
        //    catch {
        //        return null;
        //    }
        //}

        //public static List<T> ToListof<T>( DataTable dt ) {
        //    const BindingFlags flags = BindingFlags.Public | BindingFlags.Instance;
        //    var columnNames = dt.Columns.Cast<DataColumn>()
        //        .Select( c => c.ColumnName )
        //        .ToList();
        //    var objectProperties = typeof( T ).GetProperties( flags );
        //    var targetList = dt.AsEnumerable().Select( dataRow => {
        //        var instanceOfT = Activator.CreateInstance<T>();

        //        foreach ( var properties in objectProperties.Where( properties => columnNames.Contains( properties.Name ) && dataRow[ properties.Name ] != DBNull.Value ) ) {
        //            properties.SetValue( instanceOfT, dataRow[ properties.Name ], null );
        //        }
        //        return instanceOfT;
        //    } ).ToList();

        //    return targetList;
        //}

        /// <summary>
        ///     <para>Returns the first column of the first row.</para>
        /// </summary>
        /// <param name="query"></param>
        /// <param name="commandType"></param>
        /// <param name="useTransaction"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        [CanBeNull]
        public async Task<TResult> QueryScalarAsync<TResult>( [NotNull] String query, CommandType commandType, Boolean useTransaction = false, params SqlParameter[] parameters ) {
            if ( query == null ) {
                throw new ArgumentNullException( nameof( query ) );
            }
            SqlTransaction transaction = null;

            try {
                var command = new SqlCommand( query, this.OpenConnection() ) { CommandType = commandType };

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
                    // This catch block will handle any errors that may have occurred on the Server
                    // that would cause the rollback to fail, such as a closed connection.
                    exception2.More();
                }
            }
            return default( TResult );
        }

        /// <summary>
        ///     <para>Returns the first column of the first row.</para>
        /// </summary>
        /// <param name="query"></param>
        /// <param name="commandType"></param>
        /// <param name="useTransaction"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        [CanBeNull]
        public TResult QueryScalar<TResult>( [NotNull] String query, CommandType commandType, Boolean useTransaction = false, params SqlParameter[] parameters ) {
            if ( query == null ) {
                throw new ArgumentNullException( nameof( query ) );
            }
            SqlTransaction transaction = null;

            try {
                var command = new SqlCommand( query, this.OpenConnection() ) { CommandType = commandType };

                if ( null != parameters ) {
                    command.Parameters.AddRange( parameters );
                }

                if ( useTransaction ) {
                    transaction = Connections.Value.BeginTransaction(); // Start a local transaction.
                    command.Transaction = transaction;
                }
                try {
                    var scalar = command.ExecuteScalar();
                    if ( ( null == scalar ) || Convert.IsDBNull( scalar ) ) {
                        return default( TResult );
                    }
                    else {
                        if ( scalar is TResult ) {
                            return ( TResult )scalar;
                        }
                        else {
                            TResult result;
                            if ( scalar.TryCast( out result ) ) {
                                return result;
                            }
                            return ( TResult )Convert.ChangeType( scalar, typeof( TResult ) );
                        }
                    }
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
                    // This catch block will handle any errors that may have occurred on the Server
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
                    // This catch block will handle any errors that may have occurred on the Server
                    // that would cause the rollback to fail, such as a closed connection.
                    exception2.More();
                }
            }
            return default( TResult );
        }

        [NotNull]
        public async Task<Int32?> QueryWithNoResultAsync( String query, CommandType commandType, Boolean useTransaction = false, params SqlParameter[] parameters ) {
            SqlTransaction transaction = null;
            try {
                var command = new SqlCommand( query, this.OpenConnection() ) { CommandType = commandType };

                if ( null != parameters ) {
                    command.Parameters.AddRange( parameters );
                }

                if ( useTransaction ) {
                    transaction = Connections.Value.BeginTransaction(); // Start a local transaction.
                    command.Transaction = transaction;
                }

                try {
                    return await command.ExecuteNonQueryAsync();
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
                    exception2.More(); // This catch block will handle any errors that may have occurred on the Server that would cause the rollback to fail, such as a closed connection.
                }
            }
            catch ( DbException exception ) {
                exception.More();
                try {
                    transaction?.Rollback();
                }
                catch ( Exception exception2 ) {
                    // This catch block will handle any errors that may have occurred on the Server
                    // that would cause the rollback to fail, such as a closed connection.
                    exception2.More();
                }
            }
            return null;
        }

        public Boolean QueryWithNoResult( String query, CommandType commandType, Boolean useTransaction = false, params SqlParameter[] parameters ) {
            SqlTransaction transaction = null;
            try {
                var command = new SqlCommand( query, this.OpenConnection() ) { CommandType = commandType };

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
                    exception2.More(); // This catch block will handle any errors that may have occurred on the Server that would cause the rollback to fail, such as a closed connection.
                }
            }
            catch ( DbException exception ) {
                exception.More();
                try {
                    transaction?.Rollback();
                }
                catch ( Exception exception2 ) {
                    // This catch block will handle any errors that may have occurred on the Server
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
        public async Task<SqlDataReader> QueryWithResultAsync( String query, CommandType commandType, Boolean useTransaction = false, params SqlParameter[] parameters ) {
            //TryAgain:
            try {
                var command = new SqlCommand( query, this.OpenConnection() ) { CommandType = commandType };
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
