// Copyright © 1995-2018 to Rick@AIBrain.org and Protiguous.
// All Rights Reserved.
//
// This ENTIRE copyright notice and file header MUST BE KEPT
// VISIBLE in any source code derived from or used from our
// libraries and projects.
//
// =========================================================
// This section of source code, "Database.cs",
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
// "Librainian/Librainian/Database.cs" was last cleaned by Protiguous on 2018/05/15 at 10:39 PM.

namespace Librainian.Database {

    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Common;
    using System.Data.SqlClient;
    using System.Diagnostics.CodeAnalysis;
    using System.Threading.Tasks;
    using JetBrains.Annotations;
    using Magic;

    public sealed class Database : ABetterClassDispose, IDatabase {

        private readonly String _connectionString;

        /// <summary>
        /// </summary>
        /// <param name="connectionString"></param>
        /// <exception cref="InvalidOperationException"></exception>
        public Database( String connectionString ) => this._connectionString = connectionString;

        public override void DisposeManaged() { }

        /// <summary>
        ///     Opens and then closes a <see cref="SqlConnection" />.
        /// </summary>
        /// <returns></returns>
        [SuppressMessage( "Microsoft.Security", "CA2100:Review SQL queries for security vulnerabilities" )]
        public Boolean ExecuteNonQuery( String query, params SqlParameter[] parameters ) {
            if ( query.IsNullOrWhiteSpace() ) { throw new ArgumentNullException( nameof( query ) ); }

            try {
                using ( var connection = new SqlConnection( this._connectionString ) ) {
                    connection.Open();

                    using ( var command = new SqlCommand( query, connection ) { CommandType = CommandType.Text, CommandTimeout = 0 } ) {
                        if ( null != parameters ) { command.Parameters.AddRange( parameters ); }

                        command.ExecuteNonQuery();
                    }

                    return true;
                }
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
                using ( var connection = new SqlConnection( this._connectionString ) ) {
                    connection.Open();

                    using ( var command = new SqlCommand( query, connection ) { CommandType = CommandType.StoredProcedure } ) {
                        if ( null != parameters ) { command.Parameters.AddRange( parameters ); }

                        command.ExecuteNonQuery();

                        return true;
                    }
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

        [ItemCanBeNull]
        public async Task<Int32?> ExecuteNonQueryAsync( String query, CommandType commandType, params SqlParameter[] parameters ) {
            if ( query.IsNullOrWhiteSpace() ) { throw new ArgumentNullException( nameof( query ) ); }

            try {
                using ( var connection = new SqlConnection( this._connectionString ) ) {
                    connection.Open();

                    using ( var command = new SqlCommand( query, connection ) { CommandType = commandType, CommandTimeout = 0 } ) {
                        if ( null != parameters ) { command.Parameters.AddRange( parameters ); }

                        return await command.ExecuteNonQueryAsync().ConfigureAwait( false );
                    }
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
                using ( var connection = new SqlConnection( this._connectionString ) ) {
                    connection.Open();

                    using ( var command = new SqlCommand( query, connection ) { CommandType = commandType } ) {
                        if ( null != parameters ) { command.Parameters.AddRange( parameters ); }

                        table.BeginLoadData();

                        using ( var reader = command.ExecuteReader() ) { table.Load( reader ); }

                        table.EndLoadData();

                        return true;
                    }
                }
            }
            catch ( SqlException exception ) { exception.More(); }
            catch ( DbException exception ) { exception.More(); }
            catch ( Exception exception ) { exception.More(); }

            return false;
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
                using ( var connection = new SqlConnection( this._connectionString ) ) {
                    connection.Open();

                    using ( var command = new SqlCommand( query, connection ) { CommandType = commandType } ) {
                        if ( null != parameters ) { command.Parameters.AddRange( parameters ); }

                        return await command.ExecuteReaderAsync().ConfigureAwait( false );
                    }
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
                using ( var connection = new SqlConnection( this._connectionString ) ) {
                    connection.Open();

                    using ( var command = new SqlCommand( query, connection ) { CommandType = commandType } ) {
                        if ( null != parameters ) { command.Parameters.AddRange( parameters ); }

                        table.BeginLoadData();

                        using ( var reader = await command.ExecuteReaderAsync().ConfigureAwait( false ) ) { table.Load( reader ); }

                        table.EndLoadData();
                    }
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
                using ( var connection = new SqlConnection( this._connectionString ) ) {
                    connection.Open();

                    using ( var command = new SqlCommand( query, connection ) { CommandType = CommandType.StoredProcedure } ) {
                        if ( null != parameters ) { command.Parameters.AddRange( parameters ); }

                        var scalar = command.ExecuteScalar();

                        if ( null == scalar || Convert.IsDBNull( scalar ) ) { return default; }

                        if ( scalar is TResult executeScalar ) { return executeScalar; }

                        if ( scalar.TryCast<TResult>( out var result ) ) { return result; }

                        return ( TResult )Convert.ChangeType( scalar, typeof( TResult ) );
                    }
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
                using ( var connection = new SqlConnection( this._connectionString ) ) {
                    connection.Open();

                    using ( var command = new SqlCommand( query, connection ) { CommandType = commandType } ) {
                        if ( null != parameters ) { command.Parameters.AddRange( parameters ); }

                        var task = command.ExecuteScalarAsync().ConfigureAwait( false );

                        var result = await task;

                        if ( result == DBNull.Value ) { return default; }

                        return ( TResult )result;
                    }
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
                using ( var connection = new SqlConnection( this._connectionString ) ) {
                    connection.Open();

                    using ( var command = new SqlCommand( query, connection ) { CommandType = CommandType.StoredProcedure } ) {
                        if ( null != parameters ) { command.Parameters.AddRange( parameters ); }

                        using ( var reader = command.ExecuteReader() ) {
                            var data = GenericPopulator<TResult>.CreateList( reader );

                            return data;
                        }
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