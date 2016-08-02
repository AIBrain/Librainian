// Copyright 2016 Rick@AIBrain.org.
//
// This notice must be kept visible in the source.
//
// This section of source code belongs to Rick@AIBrain.Org unless otherwise specified, or the
// original license has been overwritten by the automatic formatting of this code. Any unmodified
// sections of source code borrowed from other projects retain their original license and thanks
// goes to the Authors.
//
// Donations and royalties can be paid via
//  PayPal: paypal@aibrain.org
//  bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//  litecoin: LeUxdU2w3o6pLZGVys5xpDZvvo8DUrjBp9
//
// Usage of the source code or compiled binaries is AS-IS. I am not responsible for Anything You Do.
//
// Contact me by email if you have any questions or helpful criticism.
//
// "Librainian/Database.cs" was last cleaned by Rick on 2016/06/18 at 10:50 PM

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
    using JetBrains.Annotations;
    using Magic;
    using Maths;
    using Measurement.Frequency;
    using Measurement.Time;
    using Parsing;

    public sealed class Database : BetterDisposableClass {
        private readonly String _connectionString;

        /// <summary>
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="intent"></param>
        /// <exception cref="InvalidOperationException"></exception>
        public Database( String connectionString, ApplicationIntent intent = ApplicationIntent.ReadWrite ) {
            this._connectionString = connectionString;
        }

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
        ///     Opens and then closes a <see cref="SqlConnection" />.
        /// </summary>
        /// <returns></returns>
        public Boolean OpenAndCloseConnection( String query = null ) {
            try {
                using ( var conn = new SqlConnection( this._connectionString ) ) {
                    conn.Open();
                    if ( !query.IsNullOrEmpty() ) {
                        using ( var sqlcommand = new SqlCommand( query, conn ) { CommandType = CommandType.Text, CommandTimeout = 0 } ) {
                            sqlcommand.ExecuteNonQuery();
                        }
                    }
                    return true;
                }
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

            return false;
        }

        /// <summary>
        ///     Returns a <see cref="DataTable" />
        /// </summary>
        /// <param name="query"></param>
        /// <param name="commandType"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        [NotNull]
        public DataTable Query( [NotNull] String query, CommandType commandType, params SqlParameter[] parameters ) {
            if ( query == null ) {
                throw new ArgumentNullException( nameof( query ) );
            }

            var table = new DataTable();

            try {
                using ( var conn = new SqlConnection( this._connectionString ) ) {
                    conn.Open();
                    using ( var command = new SqlCommand( query, conn ) { CommandType = commandType, CommandTimeout = 0 } ) {
                        if ( null != parameters ) {
                            command.Parameters.AddRange( parameters );
                        }

                        table.BeginLoadData();

                        using ( var reader = command.ExecuteReader() ) {
                            table.Load( reader, LoadOption.OverwriteChanges );
                        }

                        table.EndLoadData();
                    }
                }
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

            return table;
        }

        /// <summary>
        ///     Returns a <see cref="DataTable" />
        /// </summary>
        /// <param name="query"></param>
        /// <param name="commandType"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        [NotNull]
        public async Task<DataTable> QueryAsync( [NotNull] String query, CommandType commandType, params SqlParameter[] parameters ) {
            if ( query == null ) {
                throw new ArgumentNullException( nameof( query ) );
            }

            var table = new DataTable();

            try {
                using ( var conn = new SqlConnection( this._connectionString ) ) {
                    conn.Open();
                    using ( var command = new SqlCommand( query, conn ) { CommandType = commandType, CommandTimeout = 0 } ) {
                        if ( null != parameters ) {
                            command.Parameters.AddRange( parameters );
                        }

                        table.BeginLoadData();

                        using ( var reader = await command.ExecuteReaderAsync().ConfigureAwait( false ) ) {
                            table.Load( reader, LoadOption.OverwriteChanges );
                        }

                        table.EndLoadData();
                    }
                }
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
            return table;
        }

        /// <summary>
        ///     Returns a <see cref="DataTable" />
        /// </summary>
        /// <param name="query"></param>
        /// <param name="commandType"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        [CanBeNull]
        public IEnumerable<TResult> QueryList<TResult>( [NotNull] String query, CommandType commandType, params SqlParameter[] parameters ) {
            if ( query == null ) {
                throw new ArgumentNullException( nameof( query ) );
            }

            try {
                using ( var conn = new SqlConnection( this._connectionString ) ) {
                    conn.Open();
                    using ( var command = new SqlCommand( query, conn ) { CommandType = commandType, CommandTimeout = 0 } ) {
                        if ( null != parameters ) {
                            command.Parameters.AddRange( parameters );
                        }

                        using ( var reader = command.ExecuteReader() ) {
                            var data = new GenericPopulator<TResult>().CreateList( reader );
                            return data;
                        }
                    }
                }
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

            return null;
        }

        /// <summary>
        ///     <para>Returns the first column of the first row.</para>
        /// </summary>
        /// <param name="query"></param>
        /// <param name="commandType"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        [CanBeNull]
        public TResult QueryScalar<TResult>( [NotNull] String query, CommandType commandType, params SqlParameter[] parameters ) {
            if ( query == null ) {
                throw new ArgumentNullException( nameof( query ) );
            }

            try {
                using ( var conn = new SqlConnection( this._connectionString ) ) {
                    conn.Open();
                    using ( var command = new SqlCommand( query, conn ) { CommandType = commandType, CommandTimeout = 0 } ) {
                        if ( null != parameters ) {
                            command.Parameters.AddRange( parameters );
                        }

                        var scalar = command.ExecuteScalar();
                        if ( null == scalar || Convert.IsDBNull( scalar ) ) {
                            return default( TResult );
                        }
                        if ( scalar is TResult ) {
                            return ( TResult )scalar;
                        }
                        TResult result;
                        if ( scalar.TryCast( out result ) ) {
                            return result;
                        }
                        return ( TResult )Convert.ChangeType( scalar, typeof( TResult ) );
                    }
                }
            }
            catch ( SqlException exception ) {
                exception.More();
            }
            catch ( DbException exception ) {
                exception.More();
            }
            return default( TResult );
        }

        /// <summary>
        ///     <para>Returns the first column of the first row.</para>
        /// </summary>
        /// <param name="query"></param>
        /// <param name="commandType"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        [CanBeNull]
        public async Task<TResult> QueryScalarAsync<TResult>( [NotNull] String query, CommandType commandType, params SqlParameter[] parameters ) {
            if ( query == null ) {
                throw new ArgumentNullException( nameof( query ) );
            }

            try {
                using ( var conn = new SqlConnection( this._connectionString ) ) {
                    conn.Open();
                    using ( var command = new SqlCommand( query, conn ) { CommandType = commandType, CommandTimeout = 0 } ) {
                        if ( null != parameters ) {
                            command.Parameters.AddRange( parameters );
                        }

                        var task = command.ExecuteScalarAsync().ConfigureAwait( false );

                        var result = await task;
                        if ( result == DBNull.Value ) {
                            return default( TResult );
                        }
                        return ( TResult )result;
                    }
                }
            }
            catch ( InvalidCastException exception ) {

                //TIP: check for SQLServer returning a Double when you expect a Single (float in SQL).
                exception.More();
            }
            catch ( SqlException exception ) {
                exception.More();
            }
            return default( TResult );
        }

        public Boolean QueryWithNoResult( String query, CommandType commandType, Int32 retries, params SqlParameter[] parameters ) {
            TryAgain:
            try {
                using ( var conn = new SqlConnection( this._connectionString ) ) {
                    conn.Open();
                    using ( var command = new SqlCommand( query, conn ) { CommandType = commandType, CommandTimeout = 0 } ) {
                        if ( null != parameters ) {
                            command.Parameters.AddRange( parameters );
                        }

                        command.ExecuteNonQuery();
                        return true;
                    }
                }
            }
            catch ( InvalidOperationException ) {

                //timeout probably
                retries--;
                if ( retries.Any() ) {
                    goto TryAgain;
                }
            }
            catch ( SqlException exception ) {
                exception.More();
            }
            catch ( DbException exception ) {
                exception.More();
            }
            return false;
        }

        [NotNull]
        public async Task<Int32?> QueryWithNoResultAsync( String query, CommandType commandType, params SqlParameter[] parameters ) {
            try {
                using ( var conn = new SqlConnection( this._connectionString ) ) {
                    conn.Open();
                    using ( var command = new SqlCommand( query, conn ) { CommandType = commandType, CommandTimeout = 0 } ) {
                        if ( null != parameters ) {
                            command.Parameters.AddRange( parameters );
                        }

                        return await command.ExecuteNonQueryAsync().ConfigureAwait( false );
                    }
                }
            }
            catch ( SqlException exception ) {
                exception.More();
            }
            catch ( DbException exception ) {
                exception.More();
            }
            return null;
        }

        /// <summary>
        /// </summary>
        /// <param name="query"></param>
        /// <param name="commandType"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        [CanBeNull]
        public async Task<SqlDataReader> QueryWithResultAsync( String query, CommandType commandType, params SqlParameter[] parameters ) {
            try {
                using ( var conn = new SqlConnection( this._connectionString ) ) {
                    conn.Open();
                    using ( var command = new SqlCommand( query, conn ) { CommandType = commandType, CommandTimeout = 0 } ) {
                        if ( null != parameters ) {
                            command.Parameters.AddRange( parameters );
                        }
                        return await command.ExecuteReaderAsync().ConfigureAwait( false );
                    }
                }
            }
            catch ( SqlException exception ) {
                exception.More();
            }
            return null;
        }
    }
}