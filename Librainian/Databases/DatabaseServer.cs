// Copyright � Protiguous. All Rights Reserved.
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
// File "DatabaseServer.cs" last touched on 2021-03-07 at 1:49 PM by Protiguous.

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
	using System.Threading.Tasks;
	using System.Xml;
	using Collections.Extensions;
	using Converters;
	using Exceptions;
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

	public class DatabaseServer : ABetterClassDisposeReactive, IDatabase {

		/// <summary>
		///     The number of sql connections open across ALL threads.
		/// </summary>
		private static Int64 ConnectionCounter;

		/// <summary>
		/// </summary>
		/// <param name="connectionString"> </param>
		/// <param name="useDatabase">      </param>
		/// <param name="cancellationToken"></param>
		/// <exception cref="InvalidOperationException"></exception>
		public DatabaseServer( String connectionString, [CanBeNull] String? useDatabase, CancellationToken cancellationToken ) {
			$"New {nameof( DatabaseServer )} on thread {Thread.CurrentThread.ManagedThreadId}!".Verbose();

			this.CancellationToken = cancellationToken;

			connectionString = connectionString.Trimmed() ?? throw new ArgumentNullException( nameof( connectionString ) );
			useDatabase = useDatabase.Trimmed();

			var builder = new SqlConnectionStringBuilder( connectionString ) {
				Pooling = true, IntegratedSecurity = true, MaxPoolSize = 1024 //wild guess
			};

			if ( !String.IsNullOrEmpty( useDatabase ) ) {
				builder.InitialCatalog = useDatabase;
			}

			this.ConnectionString = builder.ConnectionString;

			Debug.Assert( !String.IsNullOrWhiteSpace( this.ConnectionString ) );
		}

		[CanBeNull]
		private AsyncLocal<SqlConnection?>? Connection { get; set; }

		[CanBeNull]
		private String? ConnectionString { get; }

		private CancellationToken CancellationToken { get; }

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
				using var connection = this.OpenConnection();
				try {
					using var command = new SqlCommand( query, connection ) {
						CommandTimeout = ( Int32 )this.CommandTimeout.TotalSeconds, CommandType = commandType
					};

					return command.PopulateParameters( parameters ).ExecuteNonQuery();
				}
				finally {
					CloseConnection( connection );
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

			return default( Int32? );
		}

		/// <summary>
		///     Opens a connection, runs the query, and returns the number of rows affected.
		/// </summary>
		/// <param name="query">      </param>
		/// <param name="commandType"></param>
		/// <param name="parameters"> </param>
		/// <returns></returns>
		public Int32? ExecuteNonQuery( [NotNull] String query, CommandType commandType, [CanBeNull] params SqlParameter?[]? parameters ) =>
			this.ExecuteNonQuery( query, 1, commandType, parameters );

		/// <summary>
		/// </summary>
		/// <param name="sproc">      </param>
		/// <param name="commandType"></param>
		/// <param name="parameters"> </param>
		/// <returns></returns>
		/// <exception cref="InvalidOperationException"></exception>
		/// <exception cref="SqlException"></exception>
		/// <exception cref="DbException"></exception>
		public async PooledValueTask<Int32?> ExecuteNonQueryAsync( [NotNull] String sproc, CommandType commandType, [CanBeNull] params SqlParameter?[]? parameters ) {
			this.Sproc = sproc ?? throw new ArgumentNullException( nameof( sproc ) );

			try {
				await this.CreateAsyncConnection().ConfigureAwait( false );

				await using var command = new SqlCommand( sproc, this.Connection.Value ) {
					CommandType = commandType, CommandTimeout = ( Int32 )this.CommandTimeout.TotalSeconds
				};

				var result = await command.PopulateParameters( parameters ).ExecuteNonQueryAsync( this.CancellationToken ).ConfigureAwait( false );

				return result;
			}
			catch ( SqlException exception ) {
				if ( !exception.PossibleTimeout() ) {
					exception.Log( Rebuild( sproc, parameters ) );
				}
			}
			catch ( DbException exception ) {
				exception.Log( Rebuild( sproc, parameters ) );
			}
			finally {
				this.CloseAsyncConnection();
			}

			return default( Int32? );
		}

		/// <summary>
		///     Execute the stored procedure " <paramref name="sproc" />" with the optional parameters
		///     <paramref name="parameters" />.
		/// </summary>
		/// <param name="sproc">     </param>
		/// <param name="parameters"></param>
		/// <returns></returns>
		/// <exception cref="InvalidOperationException"></exception>
		/// <exception cref="SqlException"></exception>
		/// <exception cref="DbException"></exception>
		public async PooledValueTask<Int32?> ExecuteNonQueryAsync( [NotNull] String sproc, [CanBeNull] params SqlParameter?[]? parameters ) =>
			await this.ExecuteNonQueryAsync( sproc, CommandType.StoredProcedure, parameters ).ConfigureAwait( false );

		/// <summary>
		///     Execute the stored procedure " <paramref name="sproc" />" with the optional parameters
		///     <paramref name="parameters" />.
		/// </summary>
		/// <param name="sproc">     </param>
		/// <param name="parameters"></param>
		/// <returns></returns>
		/// <exception cref="InvalidOperationException"></exception>
		/// <exception cref="SqlException"></exception>
		/// <exception cref="DbException"></exception>
		public async PooledValueTask<Int32?> RunSprocAsync( [NotNull] String sproc, [CanBeNull] params SqlParameter?[]? parameters ) =>
			await this.ExecuteNonQueryAsync( sproc, CommandType.StoredProcedure, parameters ).ConfigureAwait( false );

		/// <summary>
		///     Returns a <see cref="DataTable" />
		/// </summary>
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
				using var connection = this.OpenConnection();
				try {
					using var command = new SqlCommand( query, connection ) {
						CommandTimeout = ( Int32 )this.CommandTimeout.TotalSeconds, CommandType = commandType
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
				finally {
					CloseConnection( connection );
				}
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
		public async PooledValueTask<DataTableReader?> ExecuteReaderAsync( [NotNull] String query, CommandType commandType, [CanBeNull] params SqlParameter?[]? parameters ) {
			if ( String.IsNullOrWhiteSpace( query ) ) {
				throw new ArgumentException( "Value cannot be null or whitespace.", nameof( query ) );
			}

			this.Sproc = query;

			try {
				await this.CreateAsyncConnection().ConfigureAwait( false );

				await using var command = new SqlCommand( query, this.Connection.Value ) {
					CommandType = commandType, CommandTimeout = ( Int32 )this.CommandTimeout.TotalSeconds
				};

				using var reader = command.PopulateParameters( parameters ).ExecuteReaderAsync( this.CancellationToken );

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
			finally {
				this.CloseAsyncConnection();
			}

			return default( DataTableReader? );
		}

		/// <summary>
		///     Returns a <see cref="DataTable" />
		/// </summary>
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
				await this.CreateAsyncConnection().ConfigureAwait( false );

				await using var command = new SqlCommand( query, this.Connection.Value ) {
					CommandType = commandType, CommandTimeout = ( Int32 )this.CommandTimeout.TotalSeconds
				};

				using var reader = command.PopulateParameters( parameters ).ExecuteReaderAsync( this.CancellationToken );

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
			finally {
				this.CloseAsyncConnection();
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
				using var connection = this.OpenConnection();
				try {
					using var command = new SqlCommand( query, connection ) {
						CommandTimeout = ( Int32 )this.CommandTimeout.TotalSeconds, CommandType = commandType
					};

					var scalar = command.PopulateParameters( parameters ).ExecuteScalar();

					return scalar is null ? default( T? ) : scalar.Cast<Object, T>();
				}
				finally {
					CloseConnection( connection );
				}
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
				await this.CreateAsyncConnection().ConfigureAwait( false );

				await using var command = new SqlCommand( query, this.Connection.Value ) {
					CommandType = commandType, CommandTimeout = ( Int32 )this.CommandTimeout.TotalSeconds
				};

				var result = await command.PopulateParameters( parameters ).ExecuteScalarAsync( this.CancellationToken ).ConfigureAwait( false );

				return result is null ? default( T? ) : result.Cast<Object, T>();
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
			finally {
				this.CloseAsyncConnection();
			}
		}

		/// <summary>
		///     Overwrites the <paramref name="table" /> contents with data from the <paramref name="sproc" />.
		///     <para>Note: Include the parameters after the sproc.</para>
		///     <para>Can throw exceptions on connecting or executing the sproc.</para>
		/// </summary>
		/// <param name="sproc">      </param>
		/// <param name="commandType"></param>
		/// <param name="table">      </param>
		/// <param name="parameters"> </param>
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

			try {
				await this.CreateAsyncConnection().ConfigureAwait( false );

				using var dataAdapter = new SqlDataAdapter( sproc, this.Connection.Value ) {
					AcceptChangesDuringFill = false,
					FillLoadOption = LoadOption.OverwriteChanges,
					MissingMappingAction = MissingMappingAction.Passthrough,
					MissingSchemaAction = MissingSchemaAction.Add,
					SelectCommand = {
						CommandTimeout = ( Int32 )this.CommandTimeout.TotalSeconds, CommandType = commandType
					}
				};

				var _ = dataAdapter.SelectCommand?.PopulateParameters( parameters );

				dataAdapter.Fill( table );
			}
			finally {
				this.CloseAsyncConnection();
			}

			return true;
		}

		[NotNull]
		public DataTableReader? QueryAdHoc( [NotNull] String sql, [CanBeNull] params SqlParameter[] parameters ) {
			if ( String.IsNullOrWhiteSpace( sql ) ) {
				throw new ArgumentException( "Value cannot be null or whitespace.", nameof( sql ) );
			}

			this.Sproc = $"Executing AdHoc SQL: {sql.DoubleQuote()}.";

			try {
				using var connection = this.OpenConnection();

				try {
					using var command = new SqlCommand( sql, connection ) {
						CommandTimeout = ( Int32 )this.CommandTimeout.TotalSeconds, CommandType = CommandType.Text
					};

					using var table = command.PopulateParameters( parameters ).ExecuteReader().ToDataTable();
					return table.CreateDataReader();
				}
				finally {
					CloseConnection( connection );
				}
			}
			catch ( InvalidOperationException exception ) {
				exception.Log( Rebuild( sql, parameters ) );
			}
			catch ( SqlException exception ) {
				exception.Log( Rebuild( sql, parameters ) );
			}
			catch ( DbException exception ) {
				exception.Log( Rebuild( sql, parameters ) );
			}

			return default( DataTableReader? );
		}

		public async PooledValueTask<DataTableReader?> QueryAdhocReaderAsync( [NotNull] String sql, [CanBeNull] params SqlParameter[] parameters ) {
			if ( String.IsNullOrWhiteSpace( sql ) ) {
				throw new ArgumentException( "Value cannot be null or whitespace.", nameof( sql ) );
			}

			this.Sproc = $"Executing AdHoc SQL: {sql.DoubleQuote()}.";

			try {
				await this.CreateAsyncConnection().ConfigureAwait( false );

				await using var command = new SqlCommand( sql, this.Connection.Value ) {
					CommandTimeout = ( Int32 )this.CommandTimeout.TotalSeconds, CommandType = CommandType.Text
				};

				await using var reader = await command.PopulateParameters( parameters ).ExecuteReaderAsync( this.CancellationToken ).ConfigureAwait( false );

				using var table = reader.ToDataTable();

				return table.CreateDataReader();
			}
			catch ( SqlException exception ) {
				if ( !exception.PossibleTimeout() ) {
					exception.Log( Rebuild( sql, parameters ) );
				}
			}
			catch ( DbException exception ) {
				exception.Log( Rebuild( sql, parameters ) );
			}
			finally {
				this.CloseAsyncConnection();
			}

			return default( DataTableReader? );
		}

		public async PooledValueTask<DatabaseServer> QueryAdhocAsync( [NotNull] String sql, [CanBeNull] params SqlParameter?[]? parameters ) {
			if ( String.IsNullOrWhiteSpace( sql ) ) {
				throw new ArgumentException( "Value cannot be null or whitespace.", nameof( sql ) );
			}

			this.Sproc = $"Executing AdHoc SQL: {sql.DoubleQuote()}.";

			try {
				await this.CreateAsyncConnection().ConfigureAwait( false );

				await using var command = new SqlCommand( sql, this.Connection.Value ) {
					CommandTimeout = ( Int32 )this.CommandTimeout.TotalSeconds, CommandType = CommandType.Text
				};

				await command.PopulateParameters( parameters ).ExecuteNonQueryAsync( this.CancellationToken ).ConfigureAwait( false );
			}
			catch ( SqlException exception ) {
				if ( !exception.PossibleTimeout() ) {
					exception.Log( Rebuild( sql, parameters ) );
				}
			}
			catch ( DbException exception ) {
				exception.Log( Rebuild( sql, parameters ) );
			}
			finally {
				this.CloseAsyncConnection();
			}

			return this;
		}

		/// <summary>
		///     Simplest possible database connection.
		///     <para>Connect and then run <paramref name="sproc" />.</para>
		/// </summary>
		/// <param name="sproc">      </param>
		/// <param name="commandType"></param>
		/// <param name="parameters"> </param>
		/// <exception cref="ArgumentException"></exception>
		/// <exception cref="InvalidOperationException"></exception>
		public async PooledValueTask<SqlDataReader?> QueryAsync( [NotNull] String sproc, CommandType commandType, [CanBeNull] params SqlParameter?[]? parameters ) {
			if ( String.IsNullOrWhiteSpace( sproc ) ) {
				throw new ArgumentException( "Value cannot be null or whitespace.", nameof( sproc ) );
			}

			this.Sproc = sproc;

			try {
				await this.CreateAsyncConnection().ConfigureAwait( false );

				await using var command = new SqlCommand( sproc, this.Connection.Value ) {
					CommandType = commandType, CommandTimeout = ( Int32 )this.CommandTimeout.TotalSeconds
				};

				return await command.PopulateParameters( parameters ).ExecuteReaderAsync( this.CancellationToken ).ConfigureAwait( false );
			}
			catch ( SqlException exception ) {
				if ( !exception.PossibleTimeout() ) {
					exception.Log( Rebuild( sproc, parameters ) );
				}
			}
			catch ( DbException exception ) {
				exception.Log( Rebuild( sproc, parameters ) );
			}
			finally {
				this.CloseAsyncConnection();
			}

			return default( SqlDataReader? );
		}

		/// <summary>
		///     Simplest possible database connection.
		///     <para>Connect and then run <paramref name="sproc" />.</para>
		/// </summary>
		/// <param name="sproc">     </param>
		/// <param name="parameters"></param>
		/// <exception cref="ArgumentException"></exception>
		/// <exception cref="InvalidOperationException"></exception>
		public async PooledValueTask<SqlDataReader?> QueryAsync( [NotNull] String sproc, [CanBeNull] params SqlParameter?[]? parameters ) {
			if ( String.IsNullOrWhiteSpace( sproc ) ) {
				throw new ArgumentException( "Value cannot be null or whitespace.", nameof( sproc ) );
			}

			this.Sproc = sproc;

			try {
				await this.CreateAsyncConnection().ConfigureAwait( false );

				await using var command = new SqlCommand( sproc, this.Connection.Value ) {
					CommandType = CommandType.StoredProcedure, CommandTimeout = ( Int32 )this.CommandTimeout.TotalSeconds
				};

				return await command.PopulateParameters( parameters ).ExecuteReaderAsync( this.CancellationToken ).ConfigureAwait( false );
			}
			catch ( InvalidCastException exception ) {
				//TIP: check for SQLServer returning a Double when you expect a Single (float in SQL).
				exception.Log( Rebuild( sproc, parameters ) );

				throw;
			}
			catch ( SqlException exception ) {
				exception.Log( Rebuild( sproc, parameters ) );

				throw;
			}
			catch ( DbException exception ) {
				exception.Log( Rebuild( sproc, parameters ) );

				throw;
			}
			finally {
				this.CloseAsyncConnection();
			}
		}

		/// <summary>
		///     Returns a <see cref="DataTable" />
		/// </summary>
		/// <param name="query">      </param>
		/// <param name="commandType"></param>
		/// <param name="parameters"> </param>
		/// <returns></returns>
		[CanBeNull]
		[ItemCanBeNull]
		public IEnumerable<TResult>? QueryList<TResult>( String query, CommandType commandType, [CanBeNull] params SqlParameter[] parameters ) {
			if ( String.IsNullOrWhiteSpace( query ) ) {
				throw new ArgumentNullException( nameof( query ) );
			}

			this.Sproc = query;

			try {
				using var connection = this.OpenConnection();

				try {
					using var command = new SqlCommand( query, connection ) {
						CommandType = commandType, CommandTimeout = ( Int32 )this.CommandTimeout.TotalSeconds
					};

					var reader = command.PopulateParameters( parameters ).ExecuteReader();

					if ( reader != null ) {
						return GenericPopulatorExtensions.CreateList<TResult>( reader );
					}
				}
				finally {
					CloseConnection( connection );
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

		public void UseDatabase( [NotNull] String databaseName ) {
			if ( String.IsNullOrWhiteSpace( databaseName ) ) {
				throw new ArgumentException( "Value cannot be null or whitespace.", nameof( databaseName ) );
			}

			using var _ = this.QueryAdHoc( $"USE {databaseName.SmartBraces()};" );
		}

		public async PooledValueTask UseDatabaseAsync( [NotNull] String databaseName ) {
			if ( String.IsNullOrWhiteSpace( databaseName ) ) {
				throw new ArgumentException( "Value cannot be null or whitespace.", nameof( databaseName ) );
			}

			await using var _ = await this.QueryAdhocReaderAsync( $"USE {databaseName.SmartBraces()};" ).ConfigureAwait( false );
		}

		private async Task CreateAsyncConnection() {
			this.Connection = new AsyncLocal<SqlConnection?> {
				Value = await this.OpenConnectionAsync().ConfigureAwait( false )
			};
		}

		/// <summary>
		///     A count of every SQL connection that has had Open() called.
		///     <para>Each call to Close() will decrement this counter.</para>
		/// </summary>
		/// <returns></returns>
		public static Int64 GetConnectionCounter() => Interlocked.Read( ref ConnectionCounter );

		private void CloseAsyncConnection() {
			this.Connection?.Value?.Close();
			this.Connection = default( AsyncLocal<SqlConnection?> );

			Interlocked.Decrement( ref ConnectionCounter );
		}

		/// <summary>
		///     Opens a connection, runs the <paramref name="sproc" />, and returns the number of rows affected.
		/// </summary>
		/// <param name="sproc">     </param>
		/// <param name="parameters"></param>
		/// <returns></returns>
		public Int32? ExecuteNonQuery( [NotNull] String sproc, [CanBeNull] params SqlParameter?[]? parameters ) =>
			this.ExecuteNonQuery( sproc, CommandType.StoredProcedure, parameters );

		/// <summary>
		///     Execute the stored procedure " <paramref name="sproc" />" with the optional parameters
		///     <paramref name="parameters" />.
		/// </summary>
		/// <param name="sproc">     </param>
		/// <param name="parameters"></param>
		/// <returns></returns>
		/// <exception cref="InvalidOperationException"></exception>
		/// <exception cref="SqlException"></exception>
		/// <exception cref="DbException"></exception>
		public Int32? RunStoredProcedure( [NotNull] String sproc, [CanBeNull] params SqlParameter?[]? parameters ) {
			try {
				this.Sproc = sproc ?? throw new ArgumentNullException( nameof( sproc ) );

				using var connection = this.OpenConnection();
				try {
					using var command = new SqlCommand( sproc, connection ) {
						CommandType = CommandType.StoredProcedure, CommandTimeout = ( Int32 )this.CommandTimeout.TotalSeconds
					};

					return command.PopulateParameters( parameters ).ExecuteNonQuery();
				}
				finally {
					CloseConnection( connection );
				}
			}
			catch ( SqlException exception ) {
				if ( !exception.PossibleTimeout() ) {
					exception.Log( Rebuild( sproc!, parameters ) );
				}
			}
			catch ( DbException exception ) {
				exception.Log( Rebuild( sproc!, parameters ) );
			}

			return default( Int32? );
		}

		private static void CloseConnection( IDbConnection connection ) {
			connection.Close();
			Interlocked.Decrement( ref ConnectionCounter );
		}

		[Pure]
		private SqlConnection OpenConnection() {
			Debug.Assert( !String.IsNullOrWhiteSpace( this.ConnectionString ) );

			var connection = new SqlConnection( this.ConnectionString );
			connection.Open();
			Interlocked.Increment( ref ConnectionCounter );

			return connection;
		}

		/// <summary>
		///     Create a sql server database connection, and then call OpenAsync and store it in an AynsLocal
		/// </summary>
		/// <returns></returns>
		private async Task<SqlConnection> OpenConnectionAsync() {
			Debug.Assert( !String.IsNullOrWhiteSpace( this.ConnectionString ) );
			Debug.Assert( this.Connection?.Value is null );

			this.Connection.Value = new SqlConnection( this.ConnectionString );

			if ( this.Connection.Value.State.In( ConnectionState.Connecting, ConnectionState.Open ) ) {
				return this.Connection.Value;
			}

			await this.Connection.Value.OpenAsync( this.CancellationToken ).ConfigureAwait( false );

			if ( this.Connection.Value.State.In( ConnectionState.Connecting, ConnectionState.Open, ConnectionState.Executing, ConnectionState.Fetching ) ) {
				Interlocked.Increment( ref ConnectionCounter );
			}

			return this.Connection.Value;
		}

		/// <summary>
		///     <para>Returns the first column of the first row.</para>
		/// </summary>
		/// <param name="query">     </param>
		/// <param name="parameters"></param>
		/// <returns></returns>
		public async PooledValueTask<T?> ExecuteScalarAsync<T>( String query, [CanBeNull] params SqlParameter[] parameters ) =>
			await this.ExecuteScalarAsync<T?>( query, CommandType.StoredProcedure, parameters ).ConfigureAwait( false );

		[CanBeNull]
		public DataTableReader? ExecuteDataReader( [NotNull] String query, CommandType commandType, [CanBeNull] params SqlParameter?[]? parameters ) {
			if ( String.IsNullOrWhiteSpace( query ) ) {
				throw new ArgumentException( "Value cannot be null or whitespace.", nameof( query ) );
			}

			this.Sproc = query;

			try {
				using var connection = this.OpenConnection();

				try {
					using var command = new SqlCommand( query, connection ) {
						CommandType = commandType, CommandTimeout = ( Int32 )this.CommandTimeout.TotalSeconds
					};

					using var table = command.PopulateParameters( parameters ).ExecuteReader( CommandBehavior.CloseConnection ).ToDataTable();

					return table.CreateDataReader();
				}
				finally {
					CloseConnection( connection );
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

		~DatabaseServer() =>
			$"Warning: We have an undisposed DatabaseServer() connection somewhere. This could cause a memory leak. Query={this.Sproc.DoubleQuote()}".Log(
				BreakOrDontBreak.Break );

#endif

		/// <summary>
		///     Dispose of any <see cref="IDisposable" /> (managed) fields or properties in this method.
		/// </summary>
		public override void DisposeManaged() {
			var connectionsStillOpen = Interlocked.Read( ref ConnectionCounter );

			if ( connectionsStillOpen.Any() ) {
				$"Warning: We have an unclosed DatabaseServer() connection somewhere. Last Query={this.Sproc.DoubleQuote()}".Log( BreakOrDontBreak.Break );
			}
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
				using var db = new DatabaseServer( connectionString, "master", new CancellationTokenSource( Seconds.Thirty ).Token );

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
		/// </summary>
		/// <typeparam name="T">The type of the data in <paramref name="rows" />.</typeparam>
		/// <param name="sproc">                The fully qualified name of the stored procedure to execute.</param>
		/// <param name="useDataTable">         </param>
		/// <param name="columnName">           </param>
		/// <param name="tableVariableTypeName">Example: dbo.PageViewTableType</param>
		/// <param name="rows">                 </param>
		public void ExecuteProcedure<T>( String sproc, Boolean useDataTable, [NotNull] String columnName, [NotNull] String tableVariableTypeName, IEnumerable<T> rows ) {
			if ( String.IsNullOrWhiteSpace( columnName ) ) {
				throw new ArgumentEmptyException( nameof( columnName ) );
			}

			if ( String.IsNullOrWhiteSpace( tableVariableTypeName ) ) {
				throw new ArgumentException( "Value cannot be null or whitespace.", nameof( tableVariableTypeName ) );
			}

			using var connection = this.OpenConnection();

			using var command = new SqlCommand( sproc, connection ) {
				CommandType = CommandType.StoredProcedure, CommandTimeout = ( Int32 )this.CommandTimeout.TotalSeconds
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
				throw new SqlTypeException( $"Unknown data type. Unable to create {nameof( SqlParameter )}." );
			}

			parameter.SqlDbType = SqlDbType.Structured;
			parameter.TypeName = tableVariableTypeName;

			command.ExecuteNonQuery();
		}

		/// <summary>
		///     Try a best guess for the <see cref="SqlDbType" /> of <paramref name="type" />.
		///     <para>
		///         <remarks>Try <paramref name="type" /> ?? SqlDbType.Variant</remarks>
		///     </para>
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
			var metaData = new SqlMetaData[ 1 ];
			metaData[ 0 ] = new SqlMetaData( columnName, sqlDbType );
			SqlDataRecord record = new(metaData);
			foreach ( var row in rows ) {
				if ( row is not null ) {
					record.SetValue( 0, row );
				}

				yield return record;
			}
		}

	}

}