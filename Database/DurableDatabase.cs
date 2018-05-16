// Copyright © 1995-2018 to Rick@AIBrain.org and Protiguous.
// All Rights Reserved.
//
// This ENTIRE copyright notice and file header MUST BE KEPT
// VISIBLE in any source code derived from or used from our
// libraries and projects.
//
// =========================================================
// This section of source code, "DurableDatabase.cs",
// belongs to Rick@AIBrain.org and Protiguous@Protiguous.com
// unless otherwise specified OR the original license has been
// overwritten by the automatic formatting.
//
// (We try to avoid that from happening, but it does happen.)
//
// Any unmodified portions of source code gleaned from other
// projects still retain their original license and our thanks
// goes to those Authors.
// =========================================================
//
// Donations (more please!), royalties from any software that
// uses any of our code, and license fees can be paid to us via
// bitcoin at the address 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2.
//
// =========================================================
// Usage of the source code or compiled binaries is AS-IS.
// No warranties are expressed or implied.
// I am NOT responsible for Anything You Do With Our Code.
// =========================================================
//
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
//
// "Librainian/Librainian/DurableDatabase.cs" was last cleaned by Protiguous on 2018/05/15 at 10:39 PM.

namespace Librainian.Database {

    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Common;
    using System.Data.SqlClient;
    using System.Diagnostics.CodeAnalysis;
    using System.Threading;
    using System.Threading.Tasks;
    using JetBrains.Annotations;
    using Magic;

    public class DurableDatabase : ABetterClassDispose, IDatabase {

        /// <summary>
        ///     A database connection attempts to stay connected in the event of an unwanted disconnect.
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="retries">         </param>
        /// <exception cref="InvalidOperationException"></exception>
        public DurableDatabase( String connectionString, UInt16 retries ) {
            this.Retries = retries;
            this.ConnectionString = connectionString;

            this.SqlConnections = new ThreadLocal<SqlConnection>( () => {

                // ReSharper disable once UseObjectOrCollectionInitializer
                var connection = new SqlConnection( this.ConnectionString );
                connection.StateChange += this.SqlConnection_StateChange;

                return connection;
            }, true );

            var test = this.OpenConnection(); //try/start the current thread's open;

            if ( null == test ) {
                var builder = new SqlConnectionStringBuilder( this.ConnectionString );

                throw new InvalidOperationException( $"Unable to connect to {builder.DataSource}" );
            }
        }

        private String ConnectionString { get; }

        private UInt16 Retries { get; }

        private ThreadLocal<SqlConnection> SqlConnections { get; }

        public CancellationTokenSource CancelConnection { get; } = new CancellationTokenSource();

        [CanBeNull]
        private SqlConnection OpenConnection() {
            if ( this.SqlConnections.Value.State == ConnectionState.Open ) { return this.SqlConnections.Value; }

            try {
                this.SqlConnections.Value.Open();

                return this.SqlConnections.Value;
            }
            catch ( Exception exception ) { exception.More(); }

            return null;
        }

        /// <summary>
        ///     Return true if connected.
        /// </summary>
        /// <param name="sender"></param>
        /// <returns></returns>
        private Boolean ReOpenConnection( Object sender ) {
            if ( this.CancelConnection.IsCancellationRequested ) { return false; }

            if ( !( sender is SqlConnection connection ) ) { return false; }

            var retries = this.Retries;

            do {
                retries--;

                try {
                    if ( this.CancelConnection.IsCancellationRequested ) { return false; }

                    connection.Open();

                    if ( connection.State == ConnectionState.Open ) { return true; }
                }
                catch ( SqlException exception ) { exception.More(); }
                catch ( DbException exception ) { exception.More(); }
            } while ( retries > 0 );

            return false;
        }

        private void SqlConnection_StateChange( Object sender, StateChangeEventArgs e ) {
            switch ( e.CurrentState ) {
                case ConnectionState.Closed:
                    this.ReOpenConnection( sender );

                    break;

                case ConnectionState.Open: break; //do nothing

                case ConnectionState.Connecting:
                    Thread.SpinWait( 99 ); //TODO pooa.

                    break;

                case ConnectionState.Executing: break; //do nothing

                case ConnectionState.Fetching: break; //do nothing

                case ConnectionState.Broken:
                    this.ReOpenConnection( sender );

                    break;

                default: throw new ArgumentOutOfRangeException();
            }
        }

        public override void DisposeManaged() {
            if ( !this.CancelConnection.IsCancellationRequested ) { this.CancelConnection.Cancel(); }

            foreach ( var connection in this.SqlConnections.Values ) {
                switch ( connection.State ) {
                    case ConnectionState.Open:
                        connection.Close();

                        break;

                    case ConnectionState.Closed: break;

                    case ConnectionState.Connecting:
                        connection.Close();

                        break;

                    case ConnectionState.Executing:
                        connection.Close();

                        break;

                    case ConnectionState.Fetching:
                        connection.Close();

                        break;

                    case ConnectionState.Broken:
                        connection.Close();

                        break;

                    default: throw new ArgumentOutOfRangeException();
                }
            }
        }

        /// <summary>
        ///     Opens and then closes a <see cref="SqlConnection" />.
        /// </summary>
        /// <returns></returns>
        [SuppressMessage( "Microsoft.Security", "CA2100:Review SQL queries for security vulnerabilities" )]
        public Boolean ExecuteNonQuery( String query, params SqlParameter[] parameters ) {
            if ( query.IsNullOrWhiteSpace() ) { throw new ArgumentNullException( nameof( query ) ); }

            try {
                using ( var command = new SqlCommand( query, this.OpenConnection() ) { CommandType = CommandType.Text } ) {
                    if ( null != parameters ) { command.Parameters.AddRange( parameters ); }

                    command.ExecuteNonQuery();
                }

                return true;
            }
            catch ( SqlException exception ) { exception.More(); }
            catch ( DbException exception ) { exception.More(); }
            catch ( Exception exception ) { exception.More(); }

            return false;
        }

        [SuppressMessage( "Microsoft.Security", "CA2100:Review SQL queries for security vulnerabilities", Justification = "StoredProcedure" )]
        public Boolean ExecuteNonQuery( String query, Int32 retries, params SqlParameter[] parameters ) {
            if ( query.IsNullOrWhiteSpace() ) { throw new ArgumentNullException( nameof( query ) ); }

            TryAgain:

            try {
                using ( var command = new SqlCommand( query, this.OpenConnection() ) { CommandType = CommandType.StoredProcedure } ) {
                    if ( null != parameters ) { command.Parameters.AddRange( parameters ); }

                    command.ExecuteNonQuery();

                    return true;
                }
            }
            catch ( InvalidOperationException ) {

                //timeout probably
                retries--;

                if ( retries.Any() ) { goto TryAgain; }
            }
            catch ( SqlException exception ) { exception.More(); }
            catch ( DbException exception ) { exception.More(); }

            return false;
        }

        /// <summary>
        /// </summary>
        /// <returns></returns>
        [SuppressMessage( "Microsoft.Security", "CA2100:Review SQL queries for security vulnerabilities" )]
        public Boolean ExecuteNonQuery( String query ) {
            if ( query.IsNullOrWhiteSpace() ) { throw new ArgumentNullException( nameof( query ) ); }

            try {
                using ( var sqlcommand = new SqlCommand( query, this.OpenConnection() ) { CommandType = CommandType.Text } ) {
                    sqlcommand.ExecuteNonQuery();

                    return true;
                }
            }
            catch ( SqlException exception ) { exception.More(); }
            catch ( DbException exception ) { exception.More(); }
            catch ( Exception exception ) { exception.More(); }

            return false;
        }

        [ItemCanBeNull]
        public async Task<Int32?> ExecuteNonQueryAsync( String query, CommandType commandType, params SqlParameter[] parameters ) {
            if ( query.IsNullOrWhiteSpace() ) { throw new ArgumentNullException( nameof( query ) ); }

            try {
                using ( var command = new SqlCommand( query, this.OpenConnection() ) { CommandType = commandType } ) {
                    if ( null != parameters ) { command.Parameters.AddRange( parameters ); }

                    return await command.ExecuteNonQueryAsync().ConfigureAwait( false );
                }
            }
            catch ( SqlException exception ) { exception.More(); }
            catch ( DbException exception ) { exception.More(); }

            return null;
        }

        /// <summary>
        ///     Returns a <see cref="DataTable" />
        /// </summary>
        /// <param name="query">      </param>
        /// <param name="commandType"></param>
        /// <param name="table">      </param>
        /// <param name="parameters"> </param>
        /// <returns></returns>
        [SuppressMessage( "Microsoft.Security", "CA2100:Review SQL queries for security vulnerabilities" )]
        public Boolean ExecuteReader( String query, CommandType commandType, out DataTable table, params SqlParameter[] parameters ) {
            if ( query.IsNullOrWhiteSpace() ) { throw new ArgumentNullException( nameof( query ) ); }

            table = new DataTable();

            try {
                using ( var command = new SqlCommand( query, this.OpenConnection() ) { CommandType = commandType } ) {
                    if ( null != parameters ) { command.Parameters.AddRange( parameters ); }

                    table.BeginLoadData();

                    using ( var reader = command.ExecuteReader() ) { table.Load( reader ); }

                    table.EndLoadData();

                    return true;
                }
            }
            catch ( SqlException exception ) { exception.More(); }
            catch ( DbException exception ) { exception.More(); }
            catch ( Exception exception ) { exception.More(); }

            return false;
        }

        /// <summary>
        ///     Returns a <see cref="DataTable" />
        /// </summary>
        /// <param name="query">      </param>
        /// <param name="commandType"></param>
        /// <param name="parameters"> </param>
        /// <returns></returns>
        [SuppressMessage( "Microsoft.Security", "CA2100:Review SQL queries for security vulnerabilities" )]
        [NotNull]
        public DataTable ExecuteReader( String query, CommandType commandType, params SqlParameter[] parameters ) {
            if ( query.IsNullOrWhiteSpace() ) { throw new ArgumentNullException( nameof( query ) ); }

            var table = new DataTable();

            try {
                using ( var command = new SqlCommand( query, this.OpenConnection() ) { CommandType = commandType } ) {
                    if ( null != parameters ) { command.Parameters.AddRange( parameters ); }

                    table.BeginLoadData();

                    using ( var reader = command.ExecuteReader() ) { table.Load( reader ); }

                    table.EndLoadData();
                }
            }
            catch ( SqlException exception ) { exception.More(); }
            catch ( DbException exception ) { exception.More(); }
            catch ( Exception exception ) { exception.More(); }

            return table;
        }

        /// <summary>
        /// </summary>
        /// <param name="query">      </param>
        /// <param name="commandType"></param>
        /// <param name="parameters"> </param>
        /// <returns></returns>
        [ItemCanBeNull]
        public async Task<SqlDataReader> ExecuteReaderAsyncDataReader( String query, CommandType commandType, params SqlParameter[] parameters ) {
            if ( query.IsNullOrWhiteSpace() ) { throw new ArgumentNullException( nameof( query ) ); }

            try {
                using ( var command = new SqlCommand( query, this.OpenConnection() ) { CommandType = commandType } ) {
                    if ( null != parameters ) { command.Parameters.AddRange( parameters ); }

                    return await command.ExecuteReaderAsync().ConfigureAwait( false );
                }
            }
            catch ( SqlException exception ) { exception.More(); }

            return null;
        }

        /// <summary>
        ///     Returns a <see cref="DataTable" />
        /// </summary>
        /// <param name="query">      </param>
        /// <param name="commandType"></param>
        /// <param name="parameters"> </param>
        /// <returns></returns>
        public async Task<DataTable> ExecuteReaderAsyncDataTable( String query, CommandType commandType, params SqlParameter[] parameters ) {
            if ( query.IsNullOrWhiteSpace() ) { throw new ArgumentNullException( nameof( query ) ); }

            var table = new DataTable();

            try {
                using ( var command = new SqlCommand( query, this.OpenConnection() ) { CommandType = commandType } ) {
                    if ( null != parameters ) { command.Parameters.AddRange( parameters ); }

                    table.BeginLoadData();

                    using ( var reader = await command.ExecuteReaderAsync( this.CancelConnection.Token ).ConfigureAwait( false ) ) { table.Load( reader ); }

                    table.EndLoadData();
                }
            }
            catch ( SqlException exception ) { exception.More(); }
            catch ( DbException exception ) { exception.More(); }
            catch ( Exception exception ) { exception.More(); }

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
        [SuppressMessage( "Microsoft.Security", "CA2100:Review SQL queries for security vulnerabilities" )]
        public TResult ExecuteScalar<TResult>( String query, CommandType commandType, params SqlParameter[] parameters ) {
            if ( query.IsNullOrWhiteSpace() ) { throw new ArgumentNullException( nameof( query ) ); }

            try {
                using ( var command = new SqlCommand( query, this.OpenConnection() ) { CommandType = commandType } ) {
                    if ( null != parameters ) { command.Parameters.AddRange( parameters ); }

                    var scalar = command.ExecuteScalar();

                    if ( null == scalar || Convert.IsDBNull( scalar ) ) { return default; }

                    if ( scalar is TResult result1 ) { return result1; }

                    if ( scalar.TryCast<TResult>( out var result ) ) { return result; }

                    return ( TResult )Convert.ChangeType( scalar, typeof( TResult ) );
                }
            }
            catch ( SqlException exception ) { exception.More(); }
            catch ( DbException exception ) { exception.More(); }

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
        public async Task<TResult> ExecuteScalarAsync<TResult>( String query, CommandType commandType, params SqlParameter[] parameters ) {
            if ( query.IsNullOrWhiteSpace() ) { throw new ArgumentNullException( nameof( query ) ); }

            try {
                using ( var command = new SqlCommand( query, this.OpenConnection() ) { CommandType = commandType, CommandTimeout = 0 } ) {
                    if ( null != parameters ) { command.Parameters.AddRange( parameters ); }

                    TryAgain:
                    Object scalar;

                    try { scalar = await command.ExecuteScalarAsync().ConfigureAwait( false ); }
                    catch ( SqlException exception ) {
                        if ( exception.Number == DatabaseErrors.Deadlock ) { goto TryAgain; }

                        throw;
                    }

                    if ( null == scalar || Convert.IsDBNull( scalar ) ) { return default; }

                    if ( scalar is TResult scalarAsync ) { return scalarAsync; }

                    if ( scalar.TryCast<TResult>( out var result ) ) { return result; }

                    return ( TResult )Convert.ChangeType( scalar, typeof( TResult ) );
                }
            }
            catch ( InvalidCastException exception ) {

                //TIP: check for SQLServer returning a Double when you expect a Single (float in SQL).
                exception.More();
            }
            catch ( SqlException exception ) { exception.More(); }

            return default;
        }

        /// <summary>
        ///     Returns a <see cref="DataTable" />
        /// </summary>
        /// <param name="query">     </param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        [SuppressMessage( "Microsoft.Security", "CA2100:Review SQL queries for security vulnerabilities" )]
        [ItemCanBeNull]
        public IEnumerable<TResult> QueryList<TResult>( String query, params SqlParameter[] parameters ) {
            if ( query.IsNullOrWhiteSpace() ) { throw new ArgumentNullException( nameof( query ) ); }

            try {
                using ( var command = new SqlCommand( query, this.OpenConnection() ) { CommandType = CommandType.StoredProcedure } ) {
                    if ( null != parameters ) { command.Parameters.AddRange( parameters ); }

                    using ( var reader = command.ExecuteReader() ) {
                        var data = GenericPopulator<TResult>.CreateList( reader );

                        return data;
                    }
                }
            }
            catch ( SqlException exception ) { exception.More(); }
            catch ( DbException exception ) { exception.More(); }
            catch ( Exception exception ) { exception.More(); }

            return null;
        }
    }
}