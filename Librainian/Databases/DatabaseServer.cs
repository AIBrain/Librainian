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
// File "DatabaseServer.cs" last touched on 2021-08-16 at 2:01 PM by Protiguous.

#nullable enable

namespace Librainian.Databases {

	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Data;
	using System.Data.Common;
	using System.Diagnostics;
	using System.Linq;
	using System.Threading;
	using System.Threading.Tasks;
	using System.Xml;
	using Converters;
	using Exceptions;
	using Internet;
	using Logging;
	using Maths;
	using Measurement.Frequency;
	using Measurement.Time;
	using Microsoft.Data.SqlClient;
	using Microsoft.Data.SqlClient.Server;
	using Parsing;
	using Persistence;
	using PooledAwait;
	using Threading;
	using Utilities;
	using Utilities.Disposables;

	public class DatabaseServer : ABetterClassDisposeReactive, IDatabaseServer {

		/// <summary>
		///     Defaults to 5 attempts.
		/// </summary>
		public const Int32 DefaultRetries = 5;

		/// <summary>
		///     The number of sql connections open across ALL threads.
		/// </summary>
		private static Int64 ConnectionCounter;

		/// <summary>
		/// </summary>
		/// <param name="connectionString"> </param>
		/// <param name="useDatabase">      </param>
		/// <param name="applicationName"></param>
		/// <exception cref="InvalidOperationException"></exception>
		public DatabaseServer( String connectionString, String? useDatabase, String applicationName ) {
			this.ApplicationName = applicationName;
			$"New {nameof( DatabaseServer )} on thread {Environment.CurrentManagedThreadId} for app {applicationName.DoubleQuote()}.".Verbose();

			this.ConnectionTimeout = DefaultConnectionTimeout;

			connectionString = connectionString.Trimmed() ?? throw new ArgumentEmptyException( nameof( connectionString ) );

			var builder = new SqlConnectionStringBuilder( connectionString ) {
				Pooling = true,
				MaxPoolSize = 32767,
				IntegratedSecurity = true,
				MultipleActiveResultSets = true,
				WorkstationID = Environment.MachineName,
				ApplicationIntent = ApplicationIntent.ReadWrite,
				ApplicationName = applicationName,
				ConnectTimeout = ( Int32 )this.ConnectionTimeout.TotalSeconds,
				CommandTimeout = ( Int32 )this.CommandTimeout.TotalSeconds
			};

			builder.IntegratedSecurity = String.IsNullOrWhiteSpace( builder.UserID ) && String.IsNullOrWhiteSpace( builder.Password );

			if ( !String.IsNullOrEmpty( useDatabase = useDatabase.Trimmed() ) ) {
				builder.InitialCatalog = useDatabase;
			}

			this._connectionString = builder.ConnectionString;

			Debug.Assert( !String.IsNullOrWhiteSpace( this._connectionString ) );
		}

		public String ApplicationName { get; }

		/// <summary>
		///     Defaults to 30 seconds.
		/// </summary>
		public static TimeSpan DefaultConnectionTimeout => Seconds.Thirty;

		/// <summary>
		///     Defaults to 1 minute.
		/// </summary>
		public static TimeSpan DefaultCommandTimeout => Minutes.One;

		public TimeSpan ConnectionTimeout { get; set; }

		public static TimeSpan DefaultTimeBetweenRetries => Seconds.One;

		private String? _connectionString { get; }

		private Stopwatch TimeSinceLastConnectAttempt { get; } = Stopwatch.StartNew();

		/// <summary>
		///     A debugging aid. EACH database call will delay upon opening a connection.
		/// </summary>
		public static IQuantityOfTime? DatabaseDelay { get; set; } //= Milliseconds.FiveHundred;

		private SqlConnection? Connection { get; set; } = default( SqlConnection? );

		private SqlCommand? Command { get; set; } = default( SqlCommand? );

		/// <summary>
		///     Set to 1 minute by default.
		/// </summary>
		public TimeSpan CommandTimeout { get; set; } = DefaultCommandTimeout;

		public String? Query { get; set; }

		/*
		public Int32? ExecuteNonQuery( String query, Int32 retriesLeft, CommandType commandType, params SqlParameter[]? parameters ) {
			if ( String.IsNullOrWhiteSpace( query ) ) {
				throw new ArgumentException( "Value cannot be null or whitespace.", nameof( query ) );
			}

			this.Query = query;

		TryAgain:

			try {
				this.CreateCommand( commandType, parameters );

				return this.Command?.ExecuteNonQuery();
			}

			catch ( Exception exception ) {
				if ( exception.AttemptQueryAgain( ref retriesLeft ) ) {
					this.CloseConnection();
					await Task.Delay( DefaultTimeBetweenRetries );

					goto TryAgain;
				}

				exception.Log( Rebuild( query, parameters ) );
			}
			finally {
				this.CloseConnection();
			}

			return default( Int32? );
		}
		*/

		/*
		/// <summary>
		///     Opens a connection, runs the query, and returns the number of rows affected.
		/// </summary>
		/// <param name="query">      </param>
		/// <param name="commandType"></param>
		/// <param name="parameters"> </param>
		public Int32? ExecuteNonQuery( String query, CommandType commandType, params SqlParameter[]? parameters ) =>
			this.ExecuteNonQuery( query, 1, commandType, parameters );
		*/

		/// <summary>
		/// </summary>
		/// <param name="query">      </param>
		/// <param name="commandType"></param>
		/// <param name="cancellationToken"></param>
		/// <param name="parameters"> </param>
		/// <exception cref="InvalidOperationException"></exception>
		/// <exception cref="SqlException"></exception>
		/// <exception cref="DbException"></exception>
		public async PooledValueTask<Int32?> ExecuteNonQueryAsync(
			String query,
			CommandType commandType,
			CancellationToken cancellationToken,
			params SqlParameter[]? parameters
		) {
			this.Query = query;

			var retries = DefaultRetries;

			TryAgain:
			try {
				await this.CreateCommand( commandType, cancellationToken, parameters ).ConfigureAwait( false );
				var command = this.Command;
				if ( command != null ) {
					return await command.ExecuteNonQueryAsync( cancellationToken )!.ConfigureAwait( false );
				}
			}
			catch ( Exception exception ) {
				if ( exception.AttemptQueryAgain( ref retries ) ) {
					await this.CloseConnection().ConfigureAwait( false );
					await Task.Delay( DefaultTimeBetweenRetries, cancellationToken ).ConfigureAwait( false );
					goto TryAgain;
				}

				exception.Log( Rebuild( query, parameters ) );
			}
			finally {
				await this.CloseConnection().ConfigureAwait( false );
			}

			return default( Int32? );
		}

		/// <summary>
		///     Execute the stored procedure " <paramref name="query" />" with the optional parameters
		///     <paramref name="parameters" />.
		/// </summary>
		/// <param name="query">     </param>
		/// <param name="cancellationToken"></param>
		/// <param name="parameters"></param>
		/// <exception cref="InvalidOperationException"></exception>
		/// <exception cref="SqlException"></exception>
		/// <exception cref="DbException"></exception>
		public async PooledValueTask<Int32?> ExecuteNonQueryAsync( String query, CancellationToken cancellationToken, params SqlParameter[]? parameters ) =>
			await this.ExecuteNonQueryAsync( query, CommandType.StoredProcedure, cancellationToken, parameters ).ConfigureAwait( false );

		/// <summary>
		///     Execute the stored procedure " <paramref name="query" />" with the optional <paramref name="parameters" />.
		/// </summary>
		/// <param name="query">     </param>
		/// <param name="cancellationToken"></param>
		/// <param name="parameters"></param>
		/// <exception cref="InvalidOperationException"></exception>
		/// <exception cref="SqlException"></exception>
		/// <exception cref="DbException"></exception>
		public async PooledValueTask<Int32?> RunSprocAsync( String query, CancellationToken cancellationToken, params SqlParameter[]? parameters ) =>
			await this.ExecuteNonQueryAsync( query, CommandType.StoredProcedure, cancellationToken, parameters ).ConfigureAwait( false );

		/*
		/// <summary>
		///     Returns a <see cref="DataTable" />
		/// </summary>
		/// <param name="query">      </param>
		/// <param name="commandType"></param>
		/// <param name="table">      </param>
		/// <param name="parameters"> </param>
		public Boolean ExecuteReader( String query, CommandType commandType, out DataTable table, params SqlParameter[]? parameters ) {
			if ( String.IsNullOrWhiteSpace( query ) ) {
				throw new ArgumentEmptyException( nameof( query ) );
			}

			this.Query = query;

			var retries = DefaultRetries;

		TryAgain:
			table = new DataTable();

			try {
				this.CreateCommand( commandType, parameters );

				using var dataReader = this.Command?.ExecuteReader();

				if ( dataReader is null ) {
					return false;
				}

				table.BeginLoadData();
				table.Load( dataReader );
				table.EndLoadData();

				return true;

			}
			catch ( Exception exception ) {
				if ( exception.AttemptQueryAgain( ref retries ) ) {
					this.CloseConnection();
					await Task.Delay( DefaultTimeBetweenRetries );
					goto TryAgain;
				}

				exception.Log( Rebuild( query, parameters ) );
			}
			finally {
				this.CloseConnection();
			}

			return false;
		}
		*/

		/// <param name="query">      </param>
		/// <param name="commandType"></param>
		/// <param name="cancellationToken"></param>
		/// <param name="parameters"> </param>
		[NeedsTesting]
		public async PooledValueTask<DataTableReader?> ExecuteReaderAsync(
			String query,
			CommandType commandType,
			CancellationToken cancellationToken,
			params SqlParameter[]? parameters
		) {
			if ( String.IsNullOrWhiteSpace( query ) ) {
				throw new ArgumentException( "Value cannot be null or whitespace.", nameof( query ) );
			}

			this.Query = query;

			var retries = DefaultRetries;

			TryAgain:

			try {
				await this.CreateCommand( commandType, cancellationToken, parameters ).ConfigureAwait( false );

				var command = this.Command;
				if ( command != null ) {
					var readerAsync = await command.ExecuteReaderAsync( cancellationToken ).ConfigureAwait( false );

					if ( readerAsync != null ) {
						using var table = readerAsync.ToDataTable();

						return table.CreateDataReader();
					}
				}
			}
			catch ( Exception exception ) {
				if ( exception.AttemptQueryAgain( ref retries ) ) {
					await this.CloseConnection().ConfigureAwait( false );
					await Task.Delay( DefaultTimeBetweenRetries, cancellationToken ).ConfigureAwait( false );
					goto TryAgain;
				}

				exception.Log( Rebuild( query, parameters ) );
			}
			finally {
				await this.CloseConnection().ConfigureAwait( false );
			}

			return default( DataTableReader? );
		}

		/// <summary>
		///     Returns a <see cref="DataTable" />
		/// </summary>
		/// <param name="query">      </param>
		/// <param name="commandType"></param>
		/// <param name="cancellationToken"></param>
		/// <param name="parameters"> </param>
		public async PooledValueTask<DataTable> ExecuteReaderDataTableAsync(
			String query,
			CommandType commandType,
			CancellationToken cancellationToken,
			params SqlParameter[]? parameters
		) {
			if ( String.IsNullOrWhiteSpace( query ) ) {
				throw new ArgumentException( "Value cannot be null or whitespace.", nameof( query ) );
			}

			this.Query = query;

			var retries = DefaultRetries;

			TryAgain:
			var table = new DataTable();

			try {
				await this.CreateCommand( commandType, cancellationToken, parameters ).ConfigureAwait( false );

				using var reader = this.Command?.ExecuteReaderAsync( cancellationToken );

				if ( reader != null ) {
					table.BeginLoadData();
					table.Load( await reader.ConfigureAwait( false ) );
					table.EndLoadData();
				}
			}
			catch ( Exception exception ) {
				if ( exception.AttemptQueryAgain( ref retries ) ) {
					await this.CloseConnection().ConfigureAwait( false );
					await Task.Delay( DefaultTimeBetweenRetries, cancellationToken ).ConfigureAwait( false );
					goto TryAgain;
				}

				exception.Log( Rebuild( query, parameters ) );
			}
			finally {
				await this.CloseConnection().ConfigureAwait( false );
			}

			return table;
		}

		/*
		/// <summary>
		///     <para>Returns the first column of the first row.</para>
		/// </summary>
		/// <param name="query">      </param>
		/// <param name="commandType"></param>
		/// <param name="parameters"> </param>
		/// <exception cref="SqlException"></exception>
		/// <exception cref="DbException"></exception>
		public T? ExecuteScalar<T>( String query, CommandType commandType, params SqlParameter[]? parameters ) {
			if ( String.IsNullOrWhiteSpace( query ) ) {
				throw new ArgumentException( "Value cannot be null or whitespace.", nameof( query ) );
			}

			this.Query = query;

			var retries = DefaultRetries;

		TryAgain:

			try {
				this.CreateCommand( commandType, parameters );

				var scalar = this.Command?.ExecuteScalar();

				return scalar is null ? default( T? ) : scalar.Cast<Object, T>();

			}
			catch ( Exception exception ) {
				if ( exception.AttemptQueryAgain( ref retries ) ) {
					this.CloseConnection();
					await Task.Delay( DefaultTimeBetweenRetries );
					goto TryAgain;
				}

				exception.Log( Rebuild( query, parameters ) );
			}
			finally {
				this.CloseConnection();
			}

			return default( T? );
		}
		*/

		/// <summary>
		///     <para>Returns the first column of the first row.</para>
		/// </summary>
		/// <param name="query">      </param>
		/// <param name="commandType"></param>
		/// <param name="cancellationToken"></param>
		/// <param name="parameters"> </param>
		public async PooledValueTask<T?> ExecuteScalarAsync<T>(
			String query,
			CommandType commandType,
			CancellationToken cancellationToken,
			params SqlParameter[]? parameters
		) {
			if ( String.IsNullOrWhiteSpace( query ) ) {
				throw new ArgumentEmptyException( nameof( query ) );
			}

			this.Query = query;

			var retries = DefaultRetries;

			TryAgain:

			try {
				await this.CreateCommand( commandType, cancellationToken, parameters ).ConfigureAwait( false );

				var result = await ( this.Command?.ExecuteScalarAsync( cancellationToken ) ).ConfigureAwait( false );

				return result is null ? default( T? ) : result.Cast<Object, T>();
			}
			catch ( InvalidCastException exception ) {
				//TIP: check for SQLServer returning a Double when you expect a Single (float in SQL).
				exception.Log( Rebuild( query, parameters ) );

				throw;
			}
			catch ( Exception exception ) {
				if ( exception.AttemptQueryAgain( ref retries ) ) {
					await this.CloseConnection().ConfigureAwait( false );
					await Task.Delay( DefaultTimeBetweenRetries, cancellationToken ).ConfigureAwait( false );
					goto TryAgain;
				}

				exception.Log( Rebuild( query, parameters ) );
			}
			finally {
				await this.CloseConnection().ConfigureAwait( false );
			}

			return default( T? );
		}

		/// <summary>
		///     Overwrites the <paramref name="table" /> contents with data from the <paramref name="query" />.
		///     <para>Note: Include the parameters after the query.</para>
		///     <para>Can throw exceptions on connecting or executing the query.</para>
		/// </summary>
		/// <param name="query">      </param>
		/// <param name="commandType"></param>
		/// <param name="table">      </param>
		/// <param name="cancellationToken"></param>
		/// <param name="parameters"> </param>
		public async PooledValueTask<Boolean> FillTableAsync(
			String query,
			CommandType commandType,
			DataTable table,
			CancellationToken cancellationToken,
			params SqlParameter[]? parameters
		) {
			if ( table is null ) {
				throw new ArgumentEmptyException( nameof( table ) );
			}

			this.Query = query;

			var retries = DefaultRetries;

			TryAgain:
			table.Clear();

			try {
				await this.OpenConnectionAsync( cancellationToken ).ConfigureAwait( false );

				var connection = this.Connection;
				if ( connection is null ) {
					return false;
				}
				else {
					using var dataAdapter = new SqlDataAdapter( query, connection ) {
						AcceptChangesDuringFill = false,
						FillLoadOption = LoadOption.OverwriteChanges,
						MissingMappingAction = MissingMappingAction.Passthrough,
						MissingSchemaAction = MissingSchemaAction.Add,
						SelectCommand = {
							CommandTimeout = ( Int32 )this.CommandTimeout.TotalSeconds, CommandType = commandType
						}
					};

					dataAdapter.Fill( table );
				}
			}
			catch ( Exception exception ) {
				if ( exception.AttemptQueryAgain( ref retries ) ) {
					await this.CloseConnection().ConfigureAwait( false );
					await Task.Delay( DefaultTimeBetweenRetries, cancellationToken ).ConfigureAwait( false );
					goto TryAgain;
				}

				exception.Log( Rebuild( query, parameters ) );
			}
			finally {
				await this.CloseConnection().ConfigureAwait( false );
			}

			return true;
		}

		/*
		public DataTableReader? QueryAdHoc( String query, params SqlParameter[]? parameters ) {
			if ( String.IsNullOrWhiteSpace( query ) ) {
				throw new ArgumentException( "Value cannot be null or whitespace.", nameof( query ) );
			}

			this.Query = query;
			var retries = DefaultRetries;

		TryAgain:
			try {
				this.CreateCommand( CommandType.Text, parameters );

				using var table = this.Command?.ExecuteReader()?.ToDataTable();
				return table?.CreateDataReader();
			}
			catch ( Exception exception ) {
				if ( exception.AttemptQueryAgain( ref retries ) ) {
					this.CloseConnection();
					await Task.Delay( DefaultTimeBetweenRetries );
					goto TryAgain;
				}

				exception.Log( Rebuild( query, parameters ) );
			}
			finally {
				this.CloseConnection();
			}

			return default( DataTableReader? );
		}
		*/

		public async PooledValueTask<DataTableReader?> QueryAdhocReaderAsync( String query, CancellationToken cancellationToken, params SqlParameter[]? parameters ) {
			if ( String.IsNullOrWhiteSpace( query ) ) {
				throw new ArgumentException( "Value cannot be null or whitespace.", nameof( query ) );
			}

			this.Query = query;
			var retries = DefaultRetries;

			TryAgain:

			try {
				await this.CreateCommand( CommandType.Text, cancellationToken, parameters ).ConfigureAwait( false );

				var reader = await ( this.Command?.ExecuteReaderAsync( cancellationToken ) ).ConfigureAwait( false );

				using var table = reader.ToDataTable();

				return table.CreateDataReader();
			}
			catch ( Exception exception ) {
				if ( exception.AttemptQueryAgain( ref retries ) ) {
					await this.CloseConnection().ConfigureAwait( false );
					await Task.Delay( DefaultTimeBetweenRetries, cancellationToken ).ConfigureAwait( false );
					goto TryAgain;
				}

				exception.Log( Rebuild( query, parameters ) );
			}
			finally {
				await this.CloseConnection().ConfigureAwait( false );
			}

			return default( DataTableReader? );
		}

		public async PooledValueTask<DatabaseServer> QueryAdhocAsync( String query, CancellationToken cancellationToken, params SqlParameter[]? parameters ) {
			if ( String.IsNullOrWhiteSpace( query ) ) {
				throw new ArgumentException( "Value cannot be null or whitespace.", nameof( query ) );
			}

			this.Query = query;
			var retries = DefaultRetries;

			TryAgain:

			try {
				await this.CreateCommand( CommandType.Text, cancellationToken, parameters ).ConfigureAwait( false );

				var command = this.Command;
				if ( command != null ) {
					await command.ExecuteNonQueryAsync( cancellationToken ).ConfigureAwait( false );
				}
			}
			catch ( Exception exception ) {
				if ( exception.AttemptQueryAgain( ref retries ) ) {
					await this.CloseConnection().ConfigureAwait( false );
					await Task.Delay( DefaultTimeBetweenRetries, cancellationToken ).ConfigureAwait( false );
					goto TryAgain;
				}

				exception.Log( Rebuild( query, parameters ) );
			}
			finally {
				await this.CloseConnection().ConfigureAwait( false );
			}

			return this;
		}

		/// <summary>
		///     Simplest possible database connection.
		///     <para>Connect and then run <paramref name="query" />.</para>
		/// </summary>
		/// <param name="query">      </param>
		/// <param name="commandType"></param>
		/// <param name="cancellationToken"></param>
		/// <param name="parameters"> </param>
		/// <exception cref="ArgumentException"></exception>
		/// <exception cref="InvalidOperationException"></exception>
		public async PooledValueTask<SqlDataReader?> QueryAsync(
			String query,
			CommandType commandType,
			CancellationToken cancellationToken,
			params SqlParameter[]? parameters
		) {
			if ( String.IsNullOrWhiteSpace( query ) ) {
				throw new ArgumentException( "Value cannot be null or whitespace.", nameof( query ) );
			}

			this.Query = query;
			var retries = DefaultRetries;

			TryAgain:

			try {
				await this.CreateCommand( commandType, cancellationToken, parameters ).ConfigureAwait( false );

				var command = this.Command;
				if ( command != null ) {
					return await command.ExecuteReaderAsync( cancellationToken ).ConfigureAwait( false );
				}
			}
			catch ( Exception exception ) {
				if ( exception.AttemptQueryAgain( ref retries ) ) {
					await this.CloseConnection().ConfigureAwait( false );
					await Task.Delay( DefaultTimeBetweenRetries, cancellationToken ).ConfigureAwait( false );
					goto TryAgain;
				}

				exception.Log( Rebuild( query, parameters ) );
			}
			finally {
				await this.CloseConnection().ConfigureAwait( false );
			}

			return default( SqlDataReader? );
		}

		/// <summary>
		///     Returns a <see cref="DataTable" />
		/// </summary>
		/// <param name="query">      </param>
		/// <param name="commandType"></param>
		/// <param name="cancellationToken"></param>
		/// <param name="parameters"> </param>
		public async PooledValueTask<IEnumerable<TResult>?> QueryListAsync<TResult>(
			String query,
			CommandType commandType,
			CancellationToken cancellationToken,
			params SqlParameter[]? parameters
		) {
			if ( String.IsNullOrWhiteSpace( query ) ) {
				throw new ArgumentEmptyException( nameof( query ) );
			}

			this.Query = query;
			var retries = DefaultRetries;

			TryAgain:

			try {
				await this.CreateCommand( commandType, cancellationToken, parameters ).ConfigureAwait( false );

				var reader = await this.Command.ExecuteReaderAsync( cancellationToken ).ConfigureAwait( false );

				if ( reader != null ) {
					return GenericPopulatorExtensions.CreateList<TResult>( reader );
				}
			}
			catch ( Exception exception ) {
				if ( exception.AttemptQueryAgain( ref retries ) ) {
					await this.CloseConnection().ConfigureAwait( false );
					await Task.Delay( DefaultTimeBetweenRetries, cancellationToken ).ConfigureAwait( false );
					goto TryAgain;
				}

				exception.Log( Rebuild( query, parameters ) );
			}
			finally {
				await this.CloseConnection().ConfigureAwait( false );
			}

			return default( IEnumerable<TResult>? );
		}

		/*
		public void UseDatabase( String databaseName ) {
			if ( String.IsNullOrWhiteSpace( databaseName ) ) {
				throw new ArgumentException( "Value cannot be null or whitespace.", nameof( databaseName ) );
			}

			using var _ = this.QueryAdHoc( $"USE {databaseName.SmartBraces()};" );
		}
		*/

		public async PooledValueTask UseDatabaseAsync( String databaseName, CancellationToken cancellationToken ) {
			if ( String.IsNullOrWhiteSpace( databaseName ) ) {
				throw new ArgumentException( "Value cannot be null or whitespace.", nameof( databaseName ) );
			}

			await using var _ = await this.QueryAdhocReaderAsync( $"USE {databaseName.SmartBraces()};", cancellationToken ).ConfigureAwait( false );
		}

		[NeedsTesting]
		public async PooledValueTask CreateCommand( CommandType commandType, CancellationToken cancellationToken, params SqlParameter[]? parameters ) {
			if ( String.IsNullOrWhiteSpace( this.Query ) ) {
				throw new NullException( nameof( this.Query ) );
			}

			await this.OpenConnectionAsync( cancellationToken ).ConfigureAwait( false );

			this.Command = new SqlCommand( this.Query, this.Connection ) {
				CommandTimeout = ( Int32 )this.CommandTimeout.TotalSeconds, CommandType = commandType
			};

			if ( this.Command.Parameters is null || parameters is null ) {
				return;
			}

			foreach ( var parameter in parameters.Distinct() ) {
				// This cloning is an attempt to get rid of the stupid "feature" of sqlparameters being "assigned" already.
				// Needs tested asap!!
				var serialized = parameter.Serialize();
				var newParameter = serialized.Deserialize<SqlParameter>();
				if ( newParameter != null ) {
					this.Command.Parameters.Add( newParameter );
				}
			}
		}

		/// <summary>
		///     Simplest possible database connection.
		///     <para>Connect and then run <paramref name="query" />.</para>
		/// </summary>
		/// <param name="query">     </param>
		/// <param name="cancellationToken"></param>
		/// <param name="parameters"></param>
		/// <exception cref="ArgumentException"></exception>
		/// <exception cref="InvalidOperationException"></exception>
		public async PooledValueTask<SqlDataReader?> QueryAsync( String query, CancellationToken cancellationToken, params SqlParameter[]? parameters ) {
			if ( String.IsNullOrWhiteSpace( query ) ) {
				throw new ArgumentException( "Value cannot be null or whitespace.", nameof( query ) );
			}

			this.Query = query;
			var retries = DefaultRetries;

			TryAgain:

			try {
				await this.CreateCommand( CommandType.StoredProcedure, cancellationToken, parameters ).ConfigureAwait( false );

				var command = this.Command;
				if ( command != null ) {
					return await command.ExecuteReaderAsync( cancellationToken ).ConfigureAwait( false );
				}
			}
			catch ( InvalidCastException exception ) {
				//TIP: check for SQLServer returning a Double when you expect a Single (float in SQL).
				exception.Log( Rebuild( query, parameters ) );

				throw;
			}
			catch ( Exception exception ) {
				if ( exception.AttemptQueryAgain( ref retries ) ) {
					await this.CloseConnection().ConfigureAwait( false );
					await Task.Delay( DefaultTimeBetweenRetries, cancellationToken ).ConfigureAwait( false );
					goto TryAgain;
				}

				exception.Log( Rebuild( query, parameters ) );
			}
			finally {
				await this.CloseConnection().ConfigureAwait( false );
			}

			return default( SqlDataReader? );
		}

		/// <summary>
		///     Execute the stored procedure " <paramref name="query" />" with the optional <paramref name="parameters" />.
		/// </summary>
		/// <param name="query">     </param>
		/// <param name="cancellationToken"></param>
		/// <param name="parameters"></param>
		/// <exception cref="InvalidOperationException"></exception>
		/// <exception cref="SqlException"></exception>
		/// <exception cref="DbException"></exception>
		public async FireAndForget FireOffQuery( String query, CancellationToken cancellationToken, params SqlParameter[]? parameters ) =>
			await this.ExecuteNonQueryAsync( query, CommandType.StoredProcedure, cancellationToken, parameters ).ConfigureAwait( false );

		/// <summary>
		///     Create a sql server database connection via async.
		/// </summary>
		private async ValueTask<Status> OpenConnectionAsync(
			CancellationToken cancellationToken,
			Int32 retriesLeft = DefaultRetries,
			IProgress<(TimeSpan Elapsed, ConnectionState State)>? progress = null
		) {
			Debug.Assert( !String.IsNullOrWhiteSpace( this._connectionString ) );
			if ( String.IsNullOrWhiteSpace( this._connectionString ) ) {
				throw new NullException( nameof( this._connectionString ) );
			}

			if ( !retriesLeft.Any() ) {
				return Status.Stop;
			}

			--retriesLeft;

			this.Connection = new SqlConnection( this._connectionString );

			try {
				if ( DatabaseDelay is Seconds delay ) {
					await Task.Delay( delay, cancellationToken ).ConfigureAwait( false );
				}

				FluentTimer? timer = null;

				if ( progress is not null ) {
					var stopwatch = Stopwatch.StartNew();
					var sqlConnection = this.Connection;
					timer = Fps.Thirty.CreateTimer( () => progress.Report( ( stopwatch.Elapsed, sqlConnection.State ) ) );
				}

				try {
					this.TimeSinceLastConnectAttempt.Restart();
					var cancels = cancellationToken;
					await this.Connection.OpenAsync( cancels ).ConfigureAwait( false );

					if ( this.Connection.State == ConnectionState.Open ) {
						return Status.Continue;
					}
				}
				catch ( InvalidOperationException exception ) {
					if ( exception.AttemptQueryAgain( ref retriesLeft ) ) {
						await Task.Delay( DefaultTimeBetweenRetries, cancellationToken ).ConfigureAwait( false );
						await this.OpenConnectionAsync( cancellationToken, retriesLeft, progress ).ConfigureAwait( false );
					}

					exception.Log();
				}
				catch ( Exception exception ) {
					if ( exception.AttemptQueryAgain( ref retriesLeft ) ) {
						await Task.Delay( DefaultTimeBetweenRetries, cancellationToken ).ConfigureAwait( false );
						await this.OpenConnectionAsync( cancellationToken, retriesLeft, progress ).ConfigureAwait( false );
					}

					exception.Log();
				}

				using ( timer ) { }
			}
			catch ( InvalidOperationException exception ) {
				if ( exception.AttemptQueryAgain( ref retriesLeft ) ) {
					await Task.Delay( DefaultTimeBetweenRetries, cancellationToken ).ConfigureAwait( false );
					await this.OpenConnectionAsync( cancellationToken, retriesLeft, progress ).ConfigureAwait( false );
				}

				exception.Log();
			}
			catch ( SqlException exception ) {
				if ( exception.AttemptQueryAgain( ref retriesLeft ) ) {
					await Task.Delay( DefaultTimeBetweenRetries, cancellationToken ).ConfigureAwait( false );
					await this.OpenConnectionAsync( cancellationToken, retriesLeft, progress ).ConfigureAwait( false );
				}

				exception.Log();
			}
			catch ( DbException exception ) {
				if ( exception.AttemptQueryAgain( ref retriesLeft ) ) {
					await Task.Delay( DefaultTimeBetweenRetries, cancellationToken ).ConfigureAwait( false );
					await this.OpenConnectionAsync( cancellationToken, retriesLeft, progress ).ConfigureAwait( false );
				}

				exception.Log();
			}
			catch ( TaskCanceledException cancelled ) {
				"Open database connection was cancelled.".Verbose();
				//cancelled.Log( BreakOrDontBreak.DontBreak );
			}

			return Status.Unknown;
		}

		private void ValueChangedHandler( AsyncLocalValueChangedArgs<SqlConnection?> obj ) {
			$"Thread {Environment.CurrentManagedThreadId}: was {( obj.PreviousValue?.ToString() ).SmartQuote()} to {( obj.CurrentValue?.ToString() ).SmartQuote()}."
				.TraceLine();
		}

		public async Task<IDictionary?> GetStats( CancellationToken cancellationToken ) {
			try {
				await this.OpenConnectionAsync( cancellationToken ).ConfigureAwait( false );
				return this.Connection?.RetrieveStatistics();
			}
			catch ( Exception exception ) {
				exception.Log();
			}

			return default( IDictionary? );
		}

		/*
		/// <summary>
		///     Opens a connection, runs the <paramref name="query" />, and returns the number of rows affected.
		/// </summary>
		/// <param name="query">     </param>
		/// <param name="parameters"></param>
		public Int32? ExecuteNonQuery( String query, params SqlParameter[]? parameters ) => this.ExecuteNonQuery( query, DefaultRetries, CommandType.StoredProcedure, parameters );
		*/

		/// <summary>
		///     Execute the stored procedure " <paramref name="query" />" with the optional parameters
		///     <paramref name="parameters" />.
		/// </summary>
		/// <param name="query">     </param>
		/// <param name="cancellationToken"></param>
		/// <param name="parameters"></param>
		/// <exception cref="InvalidOperationException"></exception>
		/// <exception cref="SqlException"></exception>
		/// <exception cref="DbException"></exception>
		public async Task<Int32?> RunStoredProcedureAsync( String query, CancellationToken cancellationToken, params SqlParameter[]? parameters ) {
			this.Query = query;
			var retries = DefaultRetries;

			TryAgain:

			try {
				await this.CreateCommand( CommandType.StoredProcedure, cancellationToken, parameters ).ConfigureAwait( false );

				return await this.Command.ExecuteNonQueryAsync( cancellationToken ).ConfigureAwait( false );
			}
			catch ( Exception exception ) {
				if ( exception.AttemptQueryAgain( ref retries ) ) {
					await this.CloseConnection().ConfigureAwait( false );
					await Task.Delay( DefaultTimeBetweenRetries, cancellationToken ).ConfigureAwait( false );
					goto TryAgain;
				}

				exception.Log( Rebuild( query, parameters ) );
			}
			finally {
				await this.CloseConnection().ConfigureAwait( false );
			}

			return default( Int32? );
		}

		private async PooledValueTask CloseConnection() {
			var connection = this.Connection;
			if ( connection is null ) {
				return;
			}

			await using ( connection.ConfigureAwait( false ) ) {
				try {
					await connection.CloseAsync().ConfigureAwait( false );
				}
				catch ( SqlException exception ) {
					exception.Log( BreakOrDontBreak.Break );
				}
				finally {
					Interlocked.Decrement( ref ConnectionCounter );
				}

				this.Connection = default( SqlConnection? );
			}
		}

		/*
		[Pure]
		private SqlConnection OpenSQLConnection() {
			Debug.Assert( !String.IsNullOrWhiteSpace( this._connectionString ) );

			var retriesleft = DefaultRetries;

		TryAgain:

			try {
				if ( DatabaseDelay is Seconds delay ) {
					Thread.Sleep( delay );
				}

				$"Attempting to open sql connection. {retriesleft} attempts remaining..".Verbose();

				var connection = new SqlConnection( this._connectionString );
				connection.Open();
				Interlocked.Increment( ref ConnectionCounter );

				return connection;
			}
			catch ( InvalidOperationException exception ) {
				if ( exception.AttemptQueryAgain( ref retriesleft ) ) {
					SleepyTime();
					goto TryAgain;
				}

				exception.Log( BreakOrDontBreak.Break );
			}
			catch ( SqlException exception ) {
				if ( exception.IsTransient && exception.AttemptQueryAgain( ref retriesleft ) ) {
					SleepyTime();
					goto TryAgain;
				}

				exception.Log( BreakOrDontBreak.Break );
			}
			catch ( DbException exception ) {
				if ( exception.IsTransient && exception.AttemptQueryAgain( ref retriesleft ) ) {
					SleepyTime();
					goto TryAgain;
				}

				exception.Log( BreakOrDontBreak.Break );
			}

			throw new NullException( nameof( this.OpenSQLConnection ) );

			static void SleepyTime() {
				$"SQL Pool exhaustion. {Interlocked.Read( ref ConnectionCounter )} open connections already. Trying open connection again in 1 second..".Verbose();
				Interlocked.Decrement( ref ConnectionCounter );
				await Task.Delay( DefaultTimeBetweenRetries );
			}
		}
		*/

		/// <summary>
		///     <para>Returns the first column of the first row.</para>
		/// </summary>
		/// <param name="query">     </param>
		/// <param name="cancellationToken"></param>
		/// <param name="parameters"></param>
		public async PooledValueTask<T?> ExecuteScalarAsync<T>( String query, CancellationToken cancellationToken, params SqlParameter[]? parameters ) =>
			await this.ExecuteScalarAsync<T?>( query, CommandType.StoredProcedure, cancellationToken, parameters ).ConfigureAwait( false );

		/*
		public DataTableReader? ExecuteDataReader( String query, CommandType commandType, params SqlParameter[]? parameters ) {
			if ( String.IsNullOrWhiteSpace( query ) ) {
				throw new ArgumentException( "Value cannot be null or whitespace.", nameof( query ) );
			}

			this.Query = query;
			var retries = DefaultRetries;

		TryAgain:
			var connection = default( SqlConnection? );
			try {
				connection = this.OpenSQLConnection();

				if ( connection != null ) {
					using var command = new SqlCommand( query, connection ) {
						CommandType = commandType,
						CommandTimeout = ( Int32 )this.CommandTimeout.TotalSeconds
					};

					using var withParameters = command.PopulateParameters( parameters );
					var task = withParameters.ExecuteReader( CommandBehavior.CloseConnection );
					if ( task is not null ) {
						using ( var table = task.ToDataTable() ) {
							return table.CreateDataReader();
						}
					}
				}
			}
			catch ( Exception exception ) {
				if ( exception.AttemptQueryAgain() && retries.Any() ) {
					--retries;
					CloseConnection( ref connection );
					Thread.Sleep( DefaultTimeBetweenRetries );
					goto TryAgain;
				}

				exception.Log( Rebuild( query, parameters ) );
			}
			finally {
				CloseConnection( ref connection );
			}

			return default( DataTableReader? );
		}
		*/

		~DatabaseServer() =>
			$"Warning: We have an undisposed DatabaseServer() connection somewhere. This could cause a memory leak. Query={this.Query.DoubleQuote()}".Log(
				BreakOrDontBreak.Break );

		public override void DisposeManaged() {
			using ( this.Command ) {
				this.Command = default( SqlCommand? );
			}

			this.CloseConnection();
		}

		[DebuggerStepThrough]
		private static String Rebuild( String query, IEnumerable<SqlParameter?>? parameters = null ) {
			if ( String.IsNullOrWhiteSpace( query ) ) {
				throw new ArgumentEmptyException( nameof( query ) );
			}

			return parameters is null ? $"exec {query}" :
				$"exec {query} {parameters.Where( parameter => parameter is not null ).Select( parameter => $"{parameter!.ParameterName}={parameter.Value?.ToString().SingleQuote() ?? String.Empty}" ).ToStrings( ", " )}; ";
		}

		public async PooledValueTask<Boolean> CreateDatabase( String databaseName, String connectionString, CancellationToken cancellationToken ) {
			databaseName = databaseName.Trimmed() ?? throw new ArgumentEmptyException( nameof( databaseName ) );
			connectionString = connectionString.Trimmed() ?? throw new ArgumentEmptyException( nameof( connectionString ) );

			var retries = DefaultRetries;

			TryAgain:

			try {
				using var db = new DatabaseServer( connectionString, "master", this.ApplicationName );

				await db.QueryAdhocReaderAsync( $"create database {databaseName.SmartBraces()};", cancellationToken ).ConfigureAwait( false );

				return true;
			}
			catch ( Exception exception ) {
				if ( exception.AttemptQueryAgain( ref retries ) ) {
					await this.CloseConnection().ConfigureAwait( false );
					await Task.Delay( DefaultTimeBetweenRetries, cancellationToken ).ConfigureAwait( false );
					goto TryAgain;
				}

				exception.Log( BreakOrDontBreak.Break );
			}

			return false;
		}

		public static SqlConnectionStringBuilder PopulateConnectionStringBuilder(
			String serverName,
			String instanceName,
			TimeSpan connectTimeout,
			Credentials? credentials = default,
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

		/*
		/// <summary>
		/// </summary>
		/// <typeparam name="T">The type of the data in <paramref name="rows" />.</typeparam>
		/// <param name="query">                The fully qualified name of the stored procedure to execute.</param>
		/// <param name="useDataTable">         </param>
		/// <param name="columnName">           </param>
		/// <param name="tableVariableTypeName">Example: dbo.PageViewTableType</param>
		/// <param name="rows">                 </param>
		public void ExecuteProcedure<T>( String query, Boolean useDataTable, String columnName, String tableVariableTypeName, IEnumerable<T> rows ) {
			if ( String.IsNullOrWhiteSpace( columnName ) ) {
				throw new ArgumentEmptyException( nameof( columnName ) );
			}

			if ( String.IsNullOrWhiteSpace( tableVariableTypeName ) ) {
				throw new ArgumentException( "Value cannot be null or whitespace.", nameof( tableVariableTypeName ) );
			}

			using var connection = this.OpenSQLConnection();

			if ( connection == null ) {
				return;
			}

			using var command = new SqlCommand( query, connection ) {
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
				throw new SqlTypeException( $"Unknown data type. Unable to create {nameof( SqlParameter )}." );
			}

			parameter.SqlDbType = SqlDbType.Structured;
			parameter.TypeName = tableVariableTypeName;

			try {
				command.ExecuteNonQuery();
			}
			catch ( InvalidCastException exception ) {
				exception.Log( BreakOrDontBreak.Break );
			}
			catch ( SqlException exception ) {
				exception.Log( BreakOrDontBreak.Break );
			}
			catch ( IOException exception ) {
				exception.Log( BreakOrDontBreak.Break );
			}
			catch ( ObjectDisposedException exception ) {
				exception.Log( BreakOrDontBreak.Break );
			}
			catch ( InvalidOperationException exception ) {
				exception.Log( BreakOrDontBreak.Break );
			}
		}
		*/

		/// <summary>
		///     Try a best guess for the <see cref="SqlDbType" /> of <paramref name="type" />.
		///     <para>
		///         <remarks>Try <paramref name="type" /> ?? SqlDbType.Variant</remarks>
		///     </para>
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="type"></param>
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

		internal static DataTable CreateDataTable<T>( String columnName, IEnumerable<T> rows ) {
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

		internal static IEnumerable<SqlDataRecord> CreateSqlDataRecords<T>( String columnName, SqlDbType sqlDbType, IEnumerable<T> rows ) {
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