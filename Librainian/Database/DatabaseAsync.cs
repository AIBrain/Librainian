// Copyright © Rick@AIBrain.org and Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
//
// This source code contained in "DatabaseAsync.cs" belongs to Protiguous@Protiguous.com and
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
// Project: "Librainian", "DatabaseAsync.cs" was last formatted by Protiguous on 2019/08/08 at 6:53 AM.

namespace Librainian.Database {

    using System;
    using System.Data;
    using System.Data.SqlClient;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Collections.Extensions;
    using Collections.Sets;
    using JetBrains.Annotations;
    using Logging;
    using Magic;

    public class DatabaseAsync : ABetterClassDispose {

        private DateTime WhenCreated { get; }

        /// <summary>
        ///     The parameter collection for this database connection.
        /// </summary>
        [NotNull]
        internal ConcurrentHashset<SqlParameter> ParameterSet { get; } = new ConcurrentHashset<SqlParameter>();

        [CanBeNull]
        public CancellationTokenSource ConnectCTS { get; set; }

        [CanBeNull]
        public SqlConnection Connection { get; set; }

        public SqlConnectionStringBuilder ConnectionBuilder { get; }

        [CanBeNull]
        public CancellationTokenSource ExecuteCTS { get; set; }

        [NotNull]
        public String FullQualifiedSproc => $"{this.ConnectionBuilder.DataSource}.{this.Sproc}";

        [CanBeNull]
        public String Sproc { get; set; }

        public TimeSpan Timeout { get; set; }

        /// <summary>
        ///     <para>Create a database object to the specified server.</para>
        /// </summary>
        public DatabaseAsync( [NotNull] SqlConnectionStringBuilder builder ) {
            this.ConnectionBuilder = builder ?? throw new ArgumentNullException( paramName: nameof( builder ) );
            this.WhenCreated = DateTime.Now;
            this.Timeout = TimeSpan.FromMinutes( 1 );
        }

        ~DatabaseAsync() {
            $"We have an undisposed Database() connection somewhere. Created connection at {this.WhenCreated.ToLongTimeString()}. Hint={this.FullQualifiedSproc}".Break();
        }

        public void Add<T>( String name, SqlDbType type, T value ) =>
            this.ParameterSet.Add( new SqlParameter( name, type ) {
                Value = value
            } );

        /// <summary>
        ///     Connect async.
        ///     <para>Doesn't catch any exceptions.</para>
        /// </summary>
        public Task ConnectAsync() {
            this.Connection = new SqlConnection( this.ConnectionBuilder.ConnectionString );

            return this.Connection.OpenAsync( this.ConnectCTS?.Token ?? CancellationToken.None );
        }

        /// <summary>
        ///     Overwrites the <paramref name="table" /> contents with data from the <paramref name="sproc" />.
        ///     <para>Note: Include the parameters after the sproc.</para>
        ///     <para>Can throw exceptions on connecting or executing the sproc.</para>
        /// </summary>
        /// <param name="sproc"></param>
        /// <param name="table"></param>
        /// <param name="preClearTable"></param>
        public async Task<Boolean> FillTable( [NotNull] String sproc, [NotNull] DataTable table, Boolean preClearTable = true ) {
            if ( table == null ) {
                throw new ArgumentNullException( paramName: nameof( table ) );
            }

            if ( String.IsNullOrWhiteSpace( value: sproc ) ) {
                throw new ArgumentException( message: "Value cannot be null or whitespace.", paramName: nameof( sproc ) );
            }

            if ( preClearTable ) {
                table.Clear();
                table.AcceptChanges();
            }

            try {
                await this.ConnectAsync().ConfigureAwait( false );

                using ( var dataAdapter = new SqlDataAdapter( sproc, this.Connection ) ) {
                    dataAdapter.SelectCommand.CommandTimeout = ( Int32 ) this.Timeout.TotalSeconds;
                    dataAdapter.SelectCommand.CommandType = CommandType.StoredProcedure;
                    dataAdapter.SelectCommand.Parameters.AddRange( this.ParameterSet.ToArray() );

                    dataAdapter.AcceptChangesDuringFill = false;
                    dataAdapter.FillLoadOption = LoadOption.OverwriteChanges;
                    dataAdapter.MissingMappingAction = MissingMappingAction.Passthrough;
                    dataAdapter.MissingSchemaAction = MissingSchemaAction.Add;

                    dataAdapter.Fill( table );

                    return true;
                }
            }
            catch ( InvalidOperationException exception ) {
                exception.Log();

                return false;
            }
            catch ( SqlException exception ) {
                exception.Log();

                return false;
            }
            finally {
                table.AcceptChanges();
            }
        }

        /// <summary>
        ///     <para>Run a query, no rows expected to be read.</para>
        ///     <para>Does not catch any exceptions.</para>
        /// </summary>
        /// <param name="sproc"></param>
        public void NonQuery( [NotNull] String sproc ) {
            if ( String.IsNullOrWhiteSpace( value: sproc ) ) {
                throw new ArgumentException( message: "Value cannot be null or whitespace.", paramName: nameof( sproc ) );
            }

            try {
                using ( var connection = new SqlConnection( this.ConnectionBuilder.ConnectionString ) ) {
                    connection.Open();

                    var command = new SqlCommand {
                        Connection = connection, CommandType = CommandType.StoredProcedure, CommandTimeout = ( Int32 ) this.Timeout.TotalSeconds, CommandText = sproc
                    };

                    command.Parameters.AddRange( this.ParameterSet.ToArray() );

                    this.Sproc = $"Executing SQL command {command.CommandText}.";
                    command.ExecuteNonQuery();
                }
            }
            catch ( Exception exception ) {
                exception.Log();
            }
        }

        /// <summary>
        ///     Simplest possible database connection.
        ///     <para>Connect and then run <paramref name="sproc" />.</para>
        /// </summary>
        /// <param name="sproc"></param>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        [NotNull]
        public SqlDataReader Query( String sproc ) {
            if ( sproc.IsEmpty() ) {
                throw new ArgumentException( message: "Sproc cannot be null or whitespace.", paramName: nameof( sproc ) );
            }

            using ( var connection = new SqlConnection( this.ConnectionBuilder.ConnectionString ) ) {
                connection.Open();

                var command = new SqlCommand {
                    Connection = connection, CommandType = CommandType.StoredProcedure, CommandTimeout = ( Int32 ) this.Timeout.TotalSeconds, CommandText = sproc
                };

                command.Parameters.AddRange( this.ParameterSet.ToArray() );

                this.Sproc = sproc;

                return command.ExecuteReader();
            }
        }

        /// <summary>
        ///     Make sure to include any parameters ( <see cref="Add{T}" />) to avoid sql injection attacks.
        /// </summary>
        /// <param name="sql"></param>
        [NotNull]
        public SqlDataReader QueryAdHoc( String sql ) {

            this.Sproc = nameof( this.QueryAdHoc );

            using ( var connection = new SqlConnection( this.ConnectionBuilder.ConnectionString ) ) {
                connection.Open();

                var command = new SqlCommand {
                    Connection = connection, CommandTimeout = ( Int32 ) this.Timeout.TotalSeconds, CommandType = CommandType.Text, CommandText = sql
                };

                command.Parameters.AddRange( this.ParameterSet.ToArray() );

                this.Sproc = $"Executing AdHoc SQL {command.CommandText}.";

                return command.ExecuteReader();
            }
        }

        /// <summary>
        ///     Call the async methods.
        ///     <para>Does not catch any exceptions.</para>
        /// </summary>
        /// <param name="sproc"></param>
        [ItemNotNull]
        public async Task<SqlDataReader> QueryAsync( [NotNull] String sproc ) {
            if ( String.IsNullOrWhiteSpace( value: sproc ) ) {
                throw new ArgumentException( "Value cannot be null or whitespace.", paramName: nameof( sproc ) );
            }

            this.Sproc = sproc;

            await this.ConnectAsync().ConfigureAwait( false );

            var command = new SqlCommand {
                Connection = this.Connection, CommandType = CommandType.StoredProcedure, CommandTimeout = ( Int32 ) this.Timeout.TotalSeconds, CommandText = sproc
            };

            command.Parameters.AddRange( this.ParameterSet.ToArray() );

            return await command.ExecuteReaderAsync().ConfigureAwait( false );
        }
    }
}