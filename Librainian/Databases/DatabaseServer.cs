// Copyright © Protiguous. All Rights Reserved.
// 
// This entire copyright notice and license must be retained and must be kept visible in any binaries, libraries, repositories, or source code (directly or derived) from our binaries, libraries, projects, solutions, or applications.
// 
// All source code belongs to Protiguous@Protiguous.com unless otherwise specified or the original license has been overwritten by formatting. (We try to avoid it from happening, but it does accidentally happen.)
// 
// Any unmodified portions of source code gleaned from other sources still retain their original license and our thanks goes to those Authors.
// If you find your code unattributed in this source code, please let us know so we can properly attribute you and include the proper license and/or copyright(s).
// If you want to use any of our code in a commercial project, you must contact Protiguous@Protiguous.com for permission, license, and a quote.
// 
// Donations, payments, and royalties are accepted via bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2 and PayPal: Protiguous@Protiguous.com
// 
// ====================================================================
// Disclaimer:  Usage of the source code or binaries is AS-IS.
// No warranties are expressed, implied, or given.
// We are NOT responsible for Anything You Do With Our Code.
// We are NOT responsible for Anything You Do With Our Executables.
// We are NOT responsible for Anything You Do With Your Computer.
// ====================================================================
// 
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
// For business inquiries, please contact me at Protiguous@Protiguous.com.
// Our software can be found at "https://Protiguous.Software/"
// Our GitHub address is "https://github.com/Protiguous".
// 
// File "DatabaseServer.cs" last formatted on 2021-01-01 at 9:38 AM.

#nullable enable

namespace Librainian.Databases {

	using System;
	using System.Collections.Generic;
	using System.Data;
	using System.Data.Common;
	using System.Data.SqlTypes;
	using System.Diagnostics;
	using System.Linq;
	using System.Threading;
	using System.Xml;
	using Collections.Sets;
	using Converters;
	using Exceptions.Warnings;
	using Internet;
	using JetBrains.Annotations;
	using Logging;
	using Maths;
	using Measurement.Time;
	using Microsoft.Data.SqlClient;
	using Microsoft.Data.SqlClient.Server;
	using Parsing;
	using PooledAwait;
	using Utilities;

	public class DatabaseServer : ABetterClassDispose, IDatabase {

		/// <summary>
		///     <para>Create a database object to the specified server.</para>
		/// </summary>
		public DatabaseServer( [NotNull] SqlConnectionStringBuilder builder, [CanBeNull] String? useDatabase = null, CancellationToken? token = default ) : this(
			builder.ConnectionString, useDatabase, token ) {
			if ( builder == null ) {
				throw new ArgumentNullException( nameof( builder ) );
			}
		}

		/// <summary></summary>
		/// <param name="connectionString"></param>
		/// <param name="useDatabase"></param>
		/// <param name="token"></param>
		/// <exception cref="InvalidOperationException"></exception>
		public DatabaseServer( String connectionString, [CanBeNull] String? useDatabase = null, CancellationToken? token = default ) {
			this.Token = token ?? CancellationToken.None;
			this.ConnectionString = connectionString ?? throw new ArgumentNullException( nameof( connectionString ) );

			useDatabase = useDatabase.Trimmed();

			if ( !String.IsNullOrWhiteSpace( useDatabase ) ) {
				var builder = new SqlConnectionStringBuilder( connectionString ) {
					InitialCatalog = useDatabase.SmartBraces()
				};

				this.ConnectionString = builder.ConnectionString;
			}
		}

		private SqlConnection Connection { get; set; }

		[NotNull]
		private String ConnectionString { get; }

		private CancellationToken Token { get; }

		/// <summary>The parameter collection for this database connection.</summary>
		[NotNull]
		internal ConcurrentHashset<SqlParameter> ParameterSet { get; } = new();

		/// <summary>
		///     Set to 1 minute by default.
		/// </summary>
		public TimeSpan CommandTimeout { get; set; } = Minutes.One;

		[CanBeNull]
		public String? Sproc { get; set; }

		public Int32? ExecuteNonQuery( String query, Int32 retries, CommandType commandType, params SqlParameter?[]? parameters ) {
			if ( String.IsNullOrWhiteSpace( query ) ) {
				throw new ArgumentException( "Value cannot be null or whitespace.", nameof( query ) );
			}

			this.Sproc = query;

			TryAgain:
			--retries;

			try {
				using var command = new SqlCommand {
					Connection = this.OpenConnection(),
					CommandTimeout = ( Int32 )this.CommandTimeout.TotalSeconds,
					CommandType = commandType,
					CommandText = query
				};

				return command.PopulateParameters( parameters ).ExecuteNonQuery();
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

			return default( Int32? );
		}

		/// <summary>
		///     Opens a connection, runs the query, and returns the number of rows affected.
		/// </summary>
		/// <param name="query"></param>
		/// <param name="commandType"></param>
		/// <param name="parameters"></param>
		/// <returns></returns>
		public Int32? ExecuteNonQuery( [NotNull] String query, CommandType commandType, [CanBeNull] params SqlParameter?[]? parameters ) {
			if ( String.IsNullOrWhiteSpace( query ) ) {
				throw new ArgumentException( "Value cannot be null or whitespace.", nameof( query ) );
			}

			this.Sproc = query;

			try {
				using var command = new SqlCommand( query, this.OpenConnection() ) {
					CommandType = commandType,
					CommandTimeout = ( Int32 )this.CommandTimeout.TotalSeconds
				};

				return command.PopulateParameters( parameters ).ExecuteNonQuery();
			}
			catch ( SqlException exception ) {
				exception.Log( Rebuild( query, parameters ) );
			}
			catch ( DbException exception ) {
				exception.Log( Rebuild( query, parameters ) );
			}

			return default( Int32? );
		}

		/// <summary>
		/// </summary>
		/// <param name="sproc"></param>
		/// <param name="commandType"></param>
		/// <param name="parameters"></param>
		/// <returns></returns>
		/// <exception cref="InvalidOperationException"></exception>
		/// <exception cref="SqlException"></exception>
		/// <exception cref="DbException"></exception>
		public async PooledValueTask<Int32?> ExecuteNonQueryAsync( [NotNull] String sproc, CommandType commandType, [CanBeNull] params SqlParameter?[]? parameters ) {
			this.Sproc = sproc ?? throw new ArgumentNullException( nameof( sproc ) );

			try {
				await using var command = new SqlCommand( sproc, await this.OpenConnectionAsync().ConfigureAwait( false ) ) {
					CommandType = commandType,
					CommandTimeout = ( Int32 )this.CommandTimeout.TotalSeconds
				};

				var task = command.PopulateParameters( parameters ).ExecuteNonQueryAsync( this.Token );

				if ( task is null ) {
					throw new InvalidOperationException( $"Error executing database query {sproc.DoubleQuote()}." );
				}

				return await task.ConfigureAwait( false );
			}
			catch ( SqlException exception ) {
				if ( !exception.PossibleTimeout() ) {
					exception.Log( Rebuild( sproc, parameters ) );
				}
			}
			catch ( DbException exception ) {
				exception.Log( Rebuild( sproc, parameters ) );
			}

			return default( Int32? );
		}

		/// <summary>
		///     Execute the stored procedure "<paramref name="sproc" />" with the optional parameters
		///     <paramref name="parameters" />.
		/// </summary>
		/// <param name="sproc"></param>
		/// <param name="parameters"></param>
		/// <returns></returns>
		/// <exception cref="InvalidOperationException"></exception>
		/// <exception cref="SqlException"></exception>
		/// <exception cref="DbException"></exception>
		public async PooledValueTask<Int32?> ExecuteNonQueryAsync( [NotNull] String sproc, [CanBeNull] params SqlParameter?[]? parameters ) {
			this.Sproc = sproc ?? throw new ArgumentNullException( nameof( sproc ) );

			try {
				await using var command = new SqlCommand( sproc, await this.OpenConnectionAsync().ConfigureAwait( false ) ) {
					CommandType = CommandType.StoredProcedure,
					CommandTimeout = ( Int32 )this.CommandTimeout.TotalSeconds
				};

				var task = command.PopulateParameters( parameters ).ExecuteNonQueryAsync( this.Token );

				if ( task is null ) {
					throw new InvalidOperationException( $"Error executing database query {sproc.DoubleQuote()}." );
				}

				return await task.ConfigureAwait( false );
			}
			catch ( SqlException exception ) {
				if ( !exception.PossibleTimeout() ) {
					exception.Log( Rebuild( sproc, parameters ) );
				}
			}
			catch ( DbException exception ) {
				exception.Log( Rebuild( sproc, parameters ) );
			}

			return default( Int32? );
		}

		/// <summary>
		///     Execute the stored procedure "<paramref name="sproc" />" with the optional parameters
		///     <paramref name="parameters" />.
		/// </summary>
		/// <param name="sproc"></param>
		/// <param name="parameters"></param>
		/// <returns></returns>
		/// <exception cref="InvalidOperationException"></exception>
		/// <exception cref="SqlException"></exception>
		/// <exception cref="DbException"></exception>
		public async PooledValueTask<Int32?> RunSprocAsync( [NotNull] String sproc, [CanBeNull] params SqlParameter?[]? parameters ) {
			this.Sproc = sproc ?? throw new ArgumentNullException( nameof( sproc ) );

			try {
				await using var command = new SqlCommand( sproc, await this.OpenConnectionAsync().ConfigureAwait( false ) ) {
					CommandType = CommandType.StoredProcedure,
					CommandTimeout = ( Int32 )this.CommandTimeout.TotalSeconds
				};

				var task = command.PopulateParameters( parameters ).ExecuteNonQueryAsync( this.Token );

				if ( task is null ) {
					throw new InvalidOperationException( $"Error executing database query {sproc.DoubleQuote()}." );
				}

				return await task.ConfigureAwait( false );
			}
			catch ( SqlException exception ) {
				if ( !exception.PossibleTimeout() ) {
					exception.Log( Rebuild( sproc, parameters ) );
				}
			}
			catch ( DbException exception ) {
				exception.Log( Rebuild( sproc, parameters ) );
			}

			return default( Int32? );
		}

		/// <summary>Returns a <see cref="DataTable" /></summary>
		/// <param name="query">      </param>
		/// <param name="commandType"></param>
		/// <param name="table">      </param>
		/// <param name="parameters"> </param>
		/// <returns></returns>
		public Boolean ExecuteReader( String query, CommandType commandType, [NotNull] out DataTable table, [CanBeNull] params SqlParameter?[]? parameters ) {
			if ( String.IsNullOrWhiteSpace( query ) ) {
				throw new ArgumentNullException( nameof( query ) );
			}

			this.Sproc = query;

			table = new DataTable();

			try {
				using var command = new SqlCommand( query, this.OpenConnection() ) {
					CommandType = commandType,
					CommandTimeout = ( Int32 )this.CommandTimeout.TotalSeconds
				};

				using var bob = command.PopulateParameters( parameters ).ExecuteReader();

				if ( bob is null ) {
					return false;
				}

				table.BeginLoadData();
				table.Load( bob );
				table.EndLoadData();

				return true;
			}
			catch ( SqlException exception ) {
				exception.Log( Rebuild( query, parameters ) );
			}
			catch ( DbException exception ) {
				exception.Log( Rebuild( query, parameters ) );
			}

			return false;
		}

		/// <param name="query">      </param>
		/// <param name="commandType"></param>
		/// <param name="parameters"> </param>
		public async PooledValueTask<DataTableReader?> ExecuteReaderAsyncDataReader(
			[NotNull] String query,
			CommandType commandType,
			[CanBeNull] params SqlParameter?[]? parameters
		) {
			if ( String.IsNullOrWhiteSpace( query ) ) {
				throw new ArgumentException( "Value cannot be null or whitespace.", nameof( query ) );
			}

			this.Sproc = query;

			try {
				await using var command = new SqlCommand( query, await this.OpenConnectionAsync().ConfigureAwait( false ) ) {
					CommandType = commandType,
					CommandTimeout = ( Int32 )this.CommandTimeout.TotalSeconds
				};

				using var reader = command.PopulateParameters( parameters ).ExecuteReaderAsync( this.Token );

				if ( reader != null ) {
					await using var readerAsync = await reader.ConfigureAwait( false );
					using var table = readerAsync.ToDataTable();

					return table.CreateDataReader();
				}
			}
			catch ( SqlException exception ) {
				exception.Log( Rebuild( query, parameters ) );
			}
			catch ( DbException exception ) {
				exception.Log( Rebuild( query, parameters ) );
			}

			return default( DataTableReader? );
		}

		/// <summary>Returns a <see cref="DataTable" /></summary>
		/// <param name="query">      </param>
		/// <param name="commandType"></param>
		/// <param name="parameters"> </param>
		/// <returns></returns>
		public async PooledValueTask<DataTable> ExecuteReaderDataTableAsync(
			[NotNull] String query,
			CommandType commandType,
			[CanBeNull] params SqlParameter?[]? parameters
		) {
			if ( String.IsNullOrWhiteSpace( query ) ) {
				throw new ArgumentException( "Value cannot be null or whitespace.", nameof( query ) );
			}

			this.Sproc = query;

			var table = new DataTable();

			try {
				await using var command = new SqlCommand( query, await this.OpenConnectionAsync().ConfigureAwait( false ) ) {
					CommandType = commandType,
					CommandTimeout = ( Int32 )this.CommandTimeout.TotalSeconds
				};

				using var reader = command.PopulateParameters( parameters ).ExecuteReaderAsync( this.Token );

				if ( reader != null ) {
					table.BeginLoadData();
					table.Load( await reader.ConfigureAwait( false ) );
					table.EndLoadData();
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
		/// <exception cref="SqlException"></exception>
		/// <exception cref="DbException"></exception>
		[CanBeNull]
		public T? ExecuteScalar<T>( String query, CommandType commandType, [CanBeNull] params SqlParameter[] parameters ) {
			if ( String.IsNullOrWhiteSpace( query ) ) {
				throw new ArgumentException( "Value cannot be null or whitespace.", nameof( query ) );
			}

			this.Sproc = query;

			try {
				using var command = new SqlCommand( query, this.OpenConnection() ) {
					CommandType = commandType,
					CommandTimeout = ( Int32 )this.CommandTimeout.TotalSeconds
				};

				var sqlCommand = command.PopulateParameters( parameters );
				var scalar = sqlCommand.ExecuteScalar();

				if ( scalar is null ) {
					return default( T? );
				}

				return scalar.Cast<Object, T>();
			}
			catch ( SqlException exception ) {
				exception.Log( Rebuild( query, parameters ) );

				throw;
			}
			catch ( DbException exception ) {
				exception.Log( Rebuild( query, parameters ) );

				throw;
			}
		}

		/// <summary>
		///     <para>Returns the first column of the first row.</para>
		/// </summary>
		/// <param name="query">      </param>
		/// <param name="commandType"></param>
		/// <param name="parameters"> </param>
		/// <returns></returns>
		public async PooledValueTask<T?> ExecuteScalarAsync<T>( String query, CommandType commandType, [CanBeNull] params SqlParameter[] parameters ) {
			if ( String.IsNullOrWhiteSpace( query ) ) {
				throw new ArgumentNullException( nameof( query ) );
			}

			this.Sproc = query;

			try {
				await using var command = new SqlCommand( query, await this.OpenConnectionAsync().ConfigureAwait( false ) ) {
					CommandType = commandType,
					CommandTimeout = ( Int32 )this.CommandTimeout.TotalSeconds
				};

				using var scalar = command.PopulateParameters( parameters ).ExecuteScalarAsync( this.Token );

				if ( scalar is null ) {
					throw new InvalidOperationException(
						$"Exception calling {nameof( command.ExecuteScalarAsync ).DoubleQuote()} on command {command.CommandText.DoubleQuote()}." );
				}

				var result = await scalar.ConfigureAwait( false );

				var cast = result.Cast<Object, T>();

				return cast;
			}
			catch ( InvalidCastException exception ) {
				//TIP: check for SQLServer returning a Double when you expect a Single (float in SQL).
				exception.Log( Rebuild( query, parameters ) );

				throw;
			}
			catch ( SqlException exception ) {
				exception.Log( Rebuild( query, parameters ) );

				throw;
			}
			catch ( DbException exception ) {
				exception.Log( Rebuild( query, parameters ) );

				throw;
			}
		}

		/// <summary>
		///     Overwrites the <paramref name="table" /> contents with data from the <paramref name="sproc" />.
		///     <para>Note: Include the parameters after the sproc.</para>
		///     <para>Can throw exceptions on connecting or executing the sproc.</para>
		/// </summary>
		/// <param name="sproc"></param>
		/// <param name="commandType"></param>
		/// <param name="table"></param>
		/// <param name="parameters"></param>
		public async PooledValueTask<Boolean> FillTableAsync(
			[NotNull] String sproc,
			CommandType commandType,
			[NotNull] DataTable table,
			[CanBeNull] params SqlParameter?[]? parameters
		) {
			if ( table is null ) {
				throw new ArgumentNullException( nameof( table ) );
			}

			this.Sproc = sproc;

			if ( String.IsNullOrWhiteSpace( sproc ) ) {
				throw new ArgumentException( "Value cannot be null or whitespace.", nameof( sproc ) );
			}

			table.Clear();

			using var dataAdapter = new SqlDataAdapter( sproc, await this.OpenConnectionAsync().ConfigureAwait( false ) ) {
				AcceptChangesDuringFill = false,
				FillLoadOption = LoadOption.OverwriteChanges,
				MissingMappingAction = MissingMappingAction.Passthrough,
				MissingSchemaAction = MissingSchemaAction.Add,
				SelectCommand = {
					CommandTimeout = ( Int32 ) this.CommandTimeout.TotalSeconds, CommandType = commandType
				}
			};

			var _ = dataAdapter.SelectCommand?.PopulateParameters( parameters );

			dataAdapter.Fill( table );

			return true;
		}

		/// <summary>
		///     <para>Run a query, no rows expected to be read.</para>
		///     <para>Does not catch any exceptions.</para>
		/// </summary>
		/// <param name="sproc"></param>
		/// <param name="commandType"></param>
		/// <param name="parameters"></param>
		public async PooledValueTask NonQueryAsync( [NotNull] String sproc, CommandType commandType, [CanBeNull] params SqlParameter?[]? parameters ) {
			if ( String.IsNullOrWhiteSpace( sproc ) ) {
				throw new ArgumentException( "Value cannot be null or whitespace.", nameof( sproc ) );
			}

			this.Sproc = $"Executing SQL command {sproc}.";

			await using var command = new SqlCommand {
				Connection = await this.OpenConnectionAsync().ConfigureAwait( false ),
				CommandType = commandType,
				CommandTimeout = ( Int32 )this.CommandTimeout.TotalSeconds,
				CommandText = sproc
			};

			var task = command.PopulateParameters( parameters ).ExecuteNonQueryAsync( this.Token );

			if ( task == null ) {
				throw new InvalidOperationException(
					$"Exception calling {nameof( command.ExecuteNonQueryAsync ).DoubleQuote()} on command {command.CommandText.DoubleQuote()}." );
			}

			await task.ConfigureAwait( false );
		}

		[NotNull]
		public DataTableReader QueryAdHoc( [NotNull] String sql, [CanBeNull] params SqlParameter?[]? parameters ) {
			if ( String.IsNullOrWhiteSpace( sql ) ) {
				throw new ArgumentException( "Value cannot be null or whitespace.", nameof( sql ) );
			}

			this.Sproc = $"Executing AdHoc SQL: {sql.DoubleQuote()}.";

			using var command = new SqlCommand {
				Connection = this.OpenConnection(),
				CommandTimeout = ( Int32 )this.CommandTimeout.TotalSeconds,
				CommandType = CommandType.Text,
				CommandText = sql
			};

			using var reader = command.PopulateParameters( parameters ).ExecuteReader();
			using var table = ( reader ?? throw new InvalidOperationException() ).ToDataTable();

			return table.CreateDataReader();
		}

		public async PooledValueTask<DataTableReader> QueryAdhocReaderAsync( [NotNull] String sql, [CanBeNull] params SqlParameter?[]? parameters ) {
			if ( String.IsNullOrWhiteSpace( sql ) ) {
				throw new ArgumentException( "Value cannot be null or whitespace.", nameof( sql ) );
			}

			this.Sproc = $"Executing AdHoc SQL: {sql.DoubleQuote()}.";

			await using var command = new SqlCommand {
				Connection = await this.OpenConnectionAsync().ConfigureAwait( false ),
				CommandTimeout = ( Int32 )this.CommandTimeout.TotalSeconds,
				CommandType = CommandType.Text,
				CommandText = sql
			};

			var execute = command.PopulateParameters( parameters ).ExecuteReaderAsync( this.Token );

			if ( execute != null ) {
				await using var reader = await execute.ConfigureAwait( false );
				using var table = reader.ToDataTable();

				return table.CreateDataReader();
			}

			return new DataTableReader( new DataTable() );
		}

		public async PooledValueTask<DatabaseServer> QueryAdhocAsync( [NotNull] String sql, [CanBeNull] params SqlParameter?[]? parameters ) {
			if ( String.IsNullOrWhiteSpace( sql ) ) {
				throw new ArgumentException( "Value cannot be null or whitespace.", nameof( sql ) );
			}

			this.Sproc = $"Executing AdHoc SQL: {sql.DoubleQuote()}.";

			await using var command = new SqlCommand {
				Connection = await this.OpenConnectionAsync().ConfigureAwait( false ),
				CommandTimeout = ( Int32 )this.CommandTimeout.TotalSeconds,
				CommandType = CommandType.Text,
				CommandText = sql
			};

			var task = command.PopulateParameters( parameters ).ExecuteNonQueryAsync( this.Token );

			//TODO throw if null task?

			if ( task != null ) {
				await task.ConfigureAwait( false );
			}

			return this;
		}

		/// <summary>
		///     Simplest possible database connection.
		///     <para>Connect and then run <paramref name="sproc" />.</para>
		/// </summary>
		/// <param name="sproc"></param>
		/// <param name="commandType"></param>
		/// <param name="parameters"></param>
		/// <exception cref="ArgumentException"></exception>
		/// <exception cref="InvalidOperationException"></exception>
		public async PooledValueTask<SqlDataReader?> QueryAsync( [NotNull] String sproc, CommandType commandType, [CanBeNull] params SqlParameter?[]? parameters ) {
			if ( String.IsNullOrWhiteSpace( sproc ) ) {
				throw new ArgumentException( "Value cannot be null or whitespace.", nameof( sproc ) );
			}

			this.Sproc = sproc;

			await using var command = new SqlCommand {
				Connection = await this.OpenConnectionAsync().ConfigureAwait( false ),
				CommandType = commandType,
				CommandTimeout = ( Int32 )this.CommandTimeout.TotalSeconds,
				CommandText = sproc
			};

			using var reader = command.PopulateParameters( parameters ).ExecuteReaderAsync( this.Token );

			return reader != null ? await reader.ConfigureAwait( false ) : default( SqlDataReader? );
		}

		/// <summary>
		///     Simplest possible database connection.
		///     <para>Connect and then run <paramref name="sproc" />.</para>
		/// </summary>
		/// <param name="sproc"></param>
		/// <param name="parameters"></param>
		/// <exception cref="ArgumentException"></exception>
		/// <exception cref="InvalidOperationException"></exception>
		public async PooledValueTask<SqlDataReader?> QueryAsync( [NotNull] String sproc, [CanBeNull] params SqlParameter?[]? parameters ) {
			if ( String.IsNullOrWhiteSpace( sproc ) ) {
				throw new ArgumentException( "Value cannot be null or whitespace.", nameof( sproc ) );
			}

			this.Sproc = sproc;

			await using var command = new SqlCommand {
				Connection = await this.OpenConnectionAsync().ConfigureAwait( false ),
				CommandType = CommandType.StoredProcedure,
				CommandTimeout = ( Int32 )this.CommandTimeout.TotalSeconds,
				CommandText = sproc
			};

			using var reader = command.PopulateParameters( parameters ).ExecuteReaderAsync( this.Token );

			return reader != null ? await reader.ConfigureAwait( false ) : default( SqlDataReader? );
		}

		/// <summary>Returns a <see cref="DataTable" /></summary>
		/// <param name="query">     </param>
		/// <param name="commandType"></param>
		/// <param name="parameters"></param>
		/// <returns></returns>
		[CanBeNull]
		[ItemCanBeNull]
		public IEnumerable<TResult>? QueryList<TResult>( String query, CommandType commandType, [CanBeNull] params SqlParameter[] parameters ) {
			if ( String.IsNullOrWhiteSpace( query ) ) {
				throw new ArgumentNullException( nameof( query ) );
			}

			this.Sproc = query;

			try {
				using var command = new SqlCommand( query, this.OpenConnection() ) {
					CommandType = commandType,
					CommandTimeout = ( Int32 )this.CommandTimeout.TotalSeconds
				};

				var reader = command.PopulateParameters( parameters ).ExecuteReader();

				if ( reader != null ) {
					return GenericPopulatorExtensions.CreateList<TResult>( reader );
				}
			}
			catch ( SqlException exception ) {
				exception.Log( Rebuild( query, parameters ) );
			}
			catch ( DbException exception ) {
				exception.Log( Rebuild( query, parameters ) );
			}

			return default( IEnumerable<TResult>? );
		}

		public void UseDatabase( [NotNull] String dbName ) {
			if ( String.IsNullOrWhiteSpace( dbName ) ) {
				throw new ArgumentException( "Value cannot be null or whitespace.", nameof( dbName ) );
			}

			using var _ = this.QueryAdHoc( $"USE {dbName.SmartBraces()};" );
		}

		public async PooledValueTask UseDatabaseAsync( [NotNull] String dbName ) {
			if ( String.IsNullOrWhiteSpace( dbName ) ) {
				throw new ArgumentException( "Value cannot be null or whitespace.", nameof( dbName ) );
			}

			await using var _ = await this.QueryAdhocReaderAsync( $"USE {dbName.SmartBraces()};" ).ConfigureAwait( false );
		}

		/// <summary>
		///     Opens a connection, runs the <paramref name="sproc" />, and returns the number of rows affected.
		/// </summary>
		/// <param name="sproc"></param>
		/// <param name="parameters"></param>
		/// <returns></returns>
		public Int32? ExecuteNonQuery( [NotNull] String sproc, [CanBeNull] params SqlParameter?[]? parameters ) =>
			this.ExecuteNonQuery( sproc, CommandType.StoredProcedure, parameters );

		/// <summary>
		///     Execute the stored procedure "<paramref name="sproc" />" with the optional parameters
		///     <paramref name="parameters" />.
		/// </summary>
		/// <param name="sproc"></param>
		/// <param name="parameters"></param>
		/// <returns></returns>
		/// <exception cref="InvalidOperationException"></exception>
		/// <exception cref="SqlException"></exception>
		/// <exception cref="DbException"></exception>
		public Int32? RunStoredProcedure( [NotNull] String sproc, [CanBeNull] params SqlParameter?[]? parameters ) {
			try {
				this.Sproc = sproc ?? throw new ArgumentNullException( nameof( sproc ) );

				using var command = new SqlCommand( sproc, this.OpenConnection() ) {
					CommandType = CommandType.StoredProcedure,
					CommandTimeout = ( Int32 )this.CommandTimeout.TotalSeconds
				};

				return command.PopulateParameters( parameters ).ExecuteNonQuery();
			}
			catch ( SqlException exception ) {
				if ( !exception.PossibleTimeout() ) {
					exception.Log( Rebuild( sproc, parameters ) );
				}
			}
			catch ( DbException exception ) {
				exception.Log( Rebuild( sproc, parameters ) );
			}

			return default( Int32? );
		}

		[Pure]
		private SqlConnection OpenConnection() {
			this.Connection = new SqlConnection( this.ConnectionString );

			this.Connection.Open();

			return this.Connection;
		}

		[Pure]
		private async PooledValueTask<SqlConnection> OpenConnectionAsync() {
			this.Connection = new SqlConnection( this.ConnectionString );

			using var open = this.Connection.OpenAsync( this.Token );

			if ( open is null ) {
				throw new InvalidOperationException( $"Error opening connection to {this.Connection.DataSource.DoubleQuote()}." );
			}

			await open.ConfigureAwait( false );

			return this.Connection;
		}

		/// <summary>
		///     <para>Returns the first column of the first row.</para>
		/// </summary>
		/// <param name="query">      </param>
		/// <param name="parameters"> </param>
		/// <returns></returns>
		public async PooledValueTask<T?> ExecuteScalarAsync<T>( String query, [CanBeNull] params SqlParameter[] parameters ) {
			if ( String.IsNullOrWhiteSpace( query ) ) {
				throw new ArgumentNullException( nameof( query ) );
			}

			this.Sproc = query;

			try {
				await using var command = new SqlCommand( query, await this.OpenConnectionAsync().ConfigureAwait( false ) ) {
					CommandType = CommandType.StoredProcedure,
					CommandTimeout = ( Int32 )this.CommandTimeout.TotalSeconds
				};

				using var scalar = command.PopulateParameters( parameters ).ExecuteScalarAsync( this.Token );

				if ( scalar is null ) {
					throw new InvalidOperationException(
						$"Exception calling {nameof( command.ExecuteScalarAsync ).DoubleQuote()} on command {command.CommandText.DoubleQuote()}." );
				}

				var result = await scalar.ConfigureAwait( false );

				var cast = result.Cast<Object, T>();

				return cast;
			}
			catch ( InvalidCastException exception ) {
				//TIP: check for SQLServer returning a Double when you expect a Single (float in SQL).
				exception.Log( Rebuild( query, parameters ) );

				throw;
			}
			catch ( SqlException exception ) {
				exception.Log( Rebuild( query, parameters ) );

				throw;
			}
			catch ( DbException exception ) {
				exception.Log( Rebuild( query, parameters ) );

				throw;
			}
		}

		[CanBeNull]
		public DataTableReader? ExecuteDataReader( [NotNull] String query, CommandType commandType, [CanBeNull] params SqlParameter?[]? parameters ) {
			if ( String.IsNullOrWhiteSpace( query ) ) {
				throw new ArgumentException( "Value cannot be null or whitespace.", nameof( query ) );
			}

			this.Sproc = query;

			try {
				using var command = new SqlCommand( query, this.OpenConnection() ) {
					CommandType = commandType,
					CommandTimeout = ( Int32 )this.CommandTimeout.TotalSeconds
				};

				using var reader = command.PopulateParameters( parameters ).ExecuteReader( CommandBehavior.CloseConnection );

				if ( reader != null ) {
					using var table = reader.ToDataTable();

					return table.CreateDataReader();
				}
			}
			catch ( SqlException exception ) {
				exception.Log( Rebuild( query, parameters ) );
			}
			catch ( DbException exception ) {
				exception.Log( Rebuild( query, parameters ) );
			}

			return default( DataTableReader? );
		}

#if VERBOSE

		~DatabaseServer() => $"Warning: We have an undisposed Database() connection somewhere. This could cause a memory leak. Query={this.Sproc.DoubleQuote()}".Log();

#endif

		/// <summary>Dispose of any <see cref="IDisposable" /> (managed) fields or properties in this method.</summary>
		public override void DisposeManaged() {
			using ( this.Connection ) { }
		}

		[DebuggerStepThrough]
		[NotNull]
		private static String Rebuild( [NotNull] String query, [CanBeNull] IEnumerable<SqlParameter?>? parameters = null ) {
			if ( String.IsNullOrWhiteSpace( query ) ) {
				throw new ArgumentException( "Value cannot be null or whitespace.", nameof( query ) );
			}

			if ( parameters is null ) {
				return $"exec {query}";
			}

			return
				$"exec {query} {parameters.Where( parameter => parameter != null ).Select( parameter => $"{parameter!.ParameterName}={parameter.Value?.ToString().SingleQuote() ?? String.Empty}" ).ToStrings( "," )}; ";
		}

		public static async PooledValueTask<Boolean> CreateDatabase( [NotNull] String databaseName, [NotNull] String connectionString ) {
			databaseName = databaseName.Trimmed() ?? throw new ArgumentEmptyException( nameof( databaseName ) );
			connectionString = connectionString.Trimmed() ?? throw new ArgumentEmptyException( nameof( connectionString ) );

			try {
				using var db = new DatabaseServer( connectionString, "master" );

				await db.QueryAdhocReaderAsync( $"create database {databaseName.SmartBraces()};" ).ConfigureAwait( false );

				return true;
			}
			catch ( SqlException exception ) {
				if ( !exception.PossibleTimeout() ) {
					exception.Log();
				}
			}

			return false;
		}

		[NotNull]
		public static SqlConnectionStringBuilder PopulateConnectionStringBuilder(
			[NotNull] String serverName,
			[NotNull] String instanceName,
			TimeSpan connectTimeout,
			[CanBeNull] Credentials? credentials = default,
			Int32 packetSize = 4096
		) {
			if ( String.IsNullOrWhiteSpace( serverName ) ) {
				throw new ArgumentException( "Value cannot be null or whitespace.", nameof( serverName ) );
			}

			if ( String.IsNullOrWhiteSpace( instanceName ) ) {
				throw new ArgumentException( "Value cannot be null or whitespace.", nameof( instanceName ) );
			}

			var builder = new SqlConnectionStringBuilder {
				DataSource = $@"{serverName}\{instanceName}",

				//AsynchronousProcessing = true,
				ApplicationIntent = ApplicationIntent.ReadWrite,
				ConnectRetryCount = 3,
				ConnectTimeout = ( Int32 )connectTimeout.TotalSeconds,
				ConnectRetryInterval = 1,
				PacketSize = packetSize,
				Pooling = true
			};

			//security
			if ( String.IsNullOrWhiteSpace( credentials?.Username ) ) {
				builder.Remove( nameof( builder.UserID ) );
				builder.Remove( nameof( builder.Password ) );
				builder.IntegratedSecurity = true;
			}
			else {
				builder.Remove( "Integrated Security" );
				builder.Remove( "Authentication" );
				builder.IntegratedSecurity = false;
				builder.UserID = credentials.UserID;
				builder.Password = credentials.Password;
			}

			return builder;
		}

		/*
        [NotNull]
        [DebuggerStepThrough]
        public static async PooledValueTask StartAnySQLBrowsers( TimeSpan timeout ) {
            await PooledValueTask.Run( () => {
                "Searching for any database servers...".Log();

                var machines = new ConcurrentHashset<String> {
                    "Thor", Dns.GetHostName(), "127.0.0.1" //my development servers' names. Feel free to add your server.
                };

                var stopwatch = Stopwatch.StartNew();

                var services = machines.Where( s => !String.IsNullOrWhiteSpace( s ) ).Select( machineName => {
                    var service = new ServiceController {
                        ServiceName = "SQLBrowser", MachineName = machineName
                    };

                    if ( service.Status != ServiceControllerStatus.Running ) {
                        service.Start();
                    }

                    return service;
                } ).ToList();

                while ( stopwatch.Elapsed < timeout ) {
                    var notRunningYet = services.Where( controller =>
                        controller?.Status.In( ServiceControllerStatus.StartPending, ServiceControllerStatus.Running ) == false );

                    Parallel.ForEach( notRunningYet, controller => controller?.WaitForStatus( ServiceControllerStatus.Running, timeout ) );

                    if ( services.Any( controller => controller?.Status.In( ServiceControllerStatus.Running ) == true ) ) {
                        return;
                    }
                }
            } ).ConfigureAwait( false );
        }
        */

		/// <summary>
		/// 
		/// </summary>
		/// <typeparam name="T">The type of the data in <paramref name="rows"/>.</typeparam>
		/// <param name="sproc">The fully qualified name of the stored procedure to execute.</param>
		/// <param name="useDataTable"></param>
		/// <param name="columnName"></param>
		/// <param name="tableVariableTypeName">Example: dbo.PageViewTableType</param>
		/// <param name="rows"></param>
		public void ExecuteProcedure<T>( String sproc, Boolean useDataTable, [NotNull] String columnName, [NotNull] String tableVariableTypeName, IEnumerable<T> rows ) {
			if ( String.IsNullOrWhiteSpace( columnName ) ) {
				throw new ArgumentEmptyException( nameof( columnName ) );
			}

			if ( String.IsNullOrWhiteSpace( tableVariableTypeName ) ) {
				throw new ArgumentException( "Value cannot be null or whitespace.", nameof( tableVariableTypeName ) );
			}

			using var command = new SqlCommand( sproc, this.OpenConnection() ) {
				CommandType = CommandType.StoredProcedure,
				CommandTimeout = ( Int32 )this.CommandTimeout.TotalSeconds
			};

			if ( columnName.Left( 1 ) != "@" ) {
				columnName = $"@{columnName}";
			}

			SqlParameter? parameter;
			if ( useDataTable ) {
				parameter = command.Parameters?.AddWithValue( columnName, CreateDataTable( columnName, rows ) );
			}
			else {
				var dbType = TranslateToSqlDbType( typeof( T ) );
				parameter = command.Parameters?.AddWithValue( columnName, CreateSqlDataRecords( columnName, dbType, rows ) );
			}

			if ( parameter is null ) {
				throw new SqlTypeException( $"Unknown data type. Unable to create {nameof(SqlParameter)}." );
			}

			parameter.SqlDbType = SqlDbType.Structured;
			parameter.TypeName = tableVariableTypeName;

			command.ExecuteNonQuery();
		}

		/// <summary>
		/// Try a best guess for the <see cref="SqlDbType"/> of <paramref name="type"/>.
		/// <para>
		/// <remarks>Try <paramref name="type"/> ?? SqlDbType.Variant</remarks>
		/// </para>
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="type"></param>
		/// <returns></returns>
		public static SqlDbType TranslateToSqlDbType<T>( T type ) {
			return type switch {
				String s => s.Length.Between( 1, 4000 ) ? SqlDbType.NVarChar : SqlDbType.NText,
				UInt64 => SqlDbType.BigInt,
				Int64 => SqlDbType.BigInt,
				Int32 => SqlDbType.Int,
				UInt32 => SqlDbType.Int,
				Byte[] bytes => bytes.Length.Between( 1, 8000 ) ? SqlDbType.VarBinary : SqlDbType.Image,
				Boolean => SqlDbType.Bit,
				Char => SqlDbType.NChar,
				DateTime => SqlDbType.DateTime2,
				Decimal => SqlDbType.Decimal,
				Single => SqlDbType.Float,
				Guid => SqlDbType.UniqueIdentifier,
				Byte => SqlDbType.TinyInt,
				Int16 => SqlDbType.SmallInt,
				XmlDocument => SqlDbType.Xml,
				Date => SqlDbType.Date,
				TimeSpan => SqlDbType.Time,
				Time => SqlDbType.Time,
				DateTimeOffset => SqlDbType.DateTimeOffset,
				var _ => SqlDbType.Variant
			};
		}

		internal static DataTable CreateDataTable<T>( [NotNull] String columnName, IEnumerable<T> rows ) {
			if ( String.IsNullOrWhiteSpace( columnName ) ) {
				throw new ArgumentEmptyException( nameof( columnName ) );
			}

			DataTable table = new();
			table.Columns.Add( columnName, typeof( T ) );
			foreach ( var row in rows ) {
				table.Rows.Add( row );
			}

			return table;
		}

		internal static IEnumerable<SqlDataRecord> CreateSqlDataRecords<T>( [NotNull] String columnName, SqlDbType sqlDbType, IEnumerable<T> rows ) {
			SqlMetaData[] metaData = new SqlMetaData[ 1 ];
			metaData[ 0 ] = new SqlMetaData( columnName, sqlDbType );
			SqlDataRecord record = new( metaData );
			foreach ( var row in rows ) {
				if ( row is not null ) {
					record.SetValue( 0, row );
				}

				yield return record;
			}
		}
	}

}