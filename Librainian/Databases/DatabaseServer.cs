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
// File "DatabaseServer.cs" last touched on 2021-08-28 at 10:25 AM by Protiguous.

#nullable enable

namespace Librainian.Databases {

	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Data;
	using System.Data.Common;
	using System.Diagnostics;
	using System.Diagnostics.Contracts;
	using System.Linq;
	using System.Threading;
	using System.Threading.Tasks;
	using System.Xml;
	using Configuration;
	using Converters;
	using Exceptions;
	using Logging;
	using Maths;
	using Measurement.Frequency;
	using Measurement.Time;
	using Microsoft.Data.SqlClient;
	using Microsoft.Data.SqlClient.Server;
	using Parsing;
	using PooledAwait;
	using Threading;
	using Utilities;
	using Utilities.Disposables;

	public class DatabaseServer : ABetterClassDisposeAsync, IDatabaseServer {

		/// <summary>
		///     The number of sql connections open across ALL threads.
		/// </summary>
		private static Int64 ConnectionCounter;

		/// <summary>
		///     Defaults to 10 attempts.
		/// </summary>
		public const Int32 DefaultRetries = 10;

		private static SqlRetryLogicBaseProvider RetryProvider { get; } = SqlConfigurableRetryFactory.CreateIncrementalRetryProvider( new SqlRetryLogicOption {
			NumberOfTries = DefaultRetries,
			DeltaTime = TimeSpan.Parse( "00:00:01" ),
			MaxTimeInterval = DefaultMaximumTimeout
		} );

		private SqlCommand Command { get; }

		private SqlConnection? Connection { get; set; } = default( SqlConnection? );

		private String? ConnectionString { get; init; }

		private Stopwatch TimeSinceLastConnectAttempt { get; } = Stopwatch.StartNew();

		/// <summary>
		///     A debugging aid. EACH database call will delay upon opening a connection.
		/// </summary>
		public static IQuantityOfTime? ArtificialDatabaseDelay { get; set; }

		/// <summary>
		///     Defaults to 1 minute.
		/// </summary>
		public static TimeSpan DefaultCommandTimeout => Minutes.One;

		/// <summary>
		///     Defaults to 30 seconds.
		/// </summary>
		public static TimeSpan DefaultConnectionTimeout => Seconds.Thirty;

		public static TimeSpan DefaultMaximumTimeout => Minutes.One;

		public static TimeSpan DefaultTimeBetweenRetries => Seconds.One;

		public IApplicationSetting ApplicationSetting { get; }

		/// <summary>
		///     Set to 1 minute by default.
		/// </summary>
		public TimeSpan CommandTimeout { get; set; } = DefaultCommandTimeout;

		public String? Query { get; set; }

		public TimeSpan SlowQueriesTakeLongerThan { get; set; } = Seconds.Five;

		//= Milliseconds.FiveHundred;
		public Stopwatch? StopWatch { get; private set; }

		/// <summary>
		///     When true, queries taking longer than <see cref="SlowQueriesTakeLongerThan" /> (defaults to 5 seconds) will call
		///     Debugger.Break();
		/// </summary>
		public Boolean TimeQueries { get; set; }

		/// <summary>
		/// </summary>
		/// <param name="validatedConnectionString"> </param>
		/// <param name="applicationSetting"></param>
		/// <param name="reportQueryTiming"></param>
		/// <exception cref="InvalidOperationException"></exception>
		public DatabaseServer(
			IValidatedConnectionString validatedConnectionString,
			IApplicationSetting applicationSetting,
			QueryTiming reportQueryTiming = QueryTiming.ReportTiming
		) {
			if ( validatedConnectionString == null ) {
				throw new ArgumentNullException( nameof( validatedConnectionString ) );
			}

			this.ConnectionString = validatedConnectionString.Value;
			this.ApplicationSetting = applicationSetting ?? throw new ArgumentNullException( nameof( applicationSetting ) );

			this.TimeQueries = reportQueryTiming == QueryTiming.ReportTiming;

			if ( String.IsNullOrWhiteSpace( this.ConnectionString ) ) {
				throw new NullException( nameof( this.ConnectionString ) );
			}

			this.Command = new SqlCommand();
			AppContext.SetSwitch( "Switch.Microsoft.Data.SqlClient.EnableRetryLogic", true );
		}

		~DatabaseServer() =>
					$"Warning: We have an undisposed DatabaseServer() connection somewhere. This could cause a memory leak. Query={this.Query.DoubleQuote()}".Log(
						BreakOrDontBreak.Break );

		private static void ConnectionOnInfoMessage( Object sender, SqlInfoMessageEventArgs e ) =>
					$"{nameof( SqlInfoMessageEventArgs )} {nameof( e.Message )}={e.Message}".Verbose();

		[DebuggerStepThrough]
		private static String Rebuild( String query, IEnumerable<SqlParameter>? parameters = null ) {
			if ( String.IsNullOrWhiteSpace( query ) ) {
				throw new ArgumentEmptyException( nameof( query ) );
			}

			return parameters is null ? $"exec {query}" :
				$"exec {query} {parameters.Select( parameter => $"{parameter.ParameterName}={parameter.Value?.ToString().SingleQuote() ?? String.Empty}" ).ToStrings( ", " )}; ";
		}

		private void BreakOnSlowQueries() {
			var stopWatch = this.StopWatch;
			if ( stopWatch is null ) {
				return;
			}

			stopWatch.Stop();
			if ( this.TimeQueries && Debugger.IsAttached && stopWatch.Elapsed >= this.SlowQueriesTakeLongerThan ) {
				"Slow query detection.".BreakIfDebug( $"Query {this.Query.DoubleQuote()} took {stopWatch.Elapsed.Simpler()}." );
			}
			else {
				if ( stopWatch.Elapsed > Milliseconds.FiveHundred ) {
					$"Query {this.Query.DoubleQuote()} took {stopWatch.Elapsed.Simpler()} to execute.".Verbose();
				}
			}
		}

		private void CloseConnection() {
			var connection = this.Connection;
			if ( connection is null ) {
				return;
			}

			try {
				using ( connection ) {
					if ( connection.State == ConnectionState.Open ) {
						connection.Close();
					}
				}
			}
			catch ( InvalidOperationException ) {
				/* swallow */
			}
			catch ( SqlException exception ) {
				exception.Log( BreakOrDontBreak.Break );
			}
			finally {
				Interlocked.Decrement( ref ConnectionCounter );
				this.Connection = default( SqlConnection? );
			}
		}

		private void OnRetrying( Object? sender, SqlRetryingEventArgs e ) {
			$"Retrying SQL {this.Query.DoubleQuote()}. Retry count={e.RetryCount}.".Verbose();

			e.Cancel = this.IsDisposed;
		}

		/// <summary>
		///     Create a sql server database connection via async.
		/// </summary>
		[Pure]
		private async PooledValueTask<Status> OpenConnectionAsync(
			CancellationToken cancellationToken,
			Int32 retriesLeft = DefaultRetries,
			IProgress<(TimeSpan Elapsed, ConnectionState State)>? progress = null
		) {
			Debug.Assert( !String.IsNullOrWhiteSpace( this.ConnectionString ) );
			if ( String.IsNullOrWhiteSpace( this.ConnectionString ) ) {
				throw new NullException( nameof( this.ConnectionString ) );
			}

			try {
				if ( !retriesLeft.Any() ) {
					return Status.Stop;
				}

				--retriesLeft;

				if ( this.TimeQueries ) {
					this.StopWatch = Stopwatch.StartNew();
				}

				if ( this.Connection is null ) {
					this.Connection = new SqlConnection( this.ConnectionString );
					this.Connection.InfoMessage += ConnectionOnInfoMessage;
				}

				if ( this.Connection.RetryLogicProvider is null ) {
					this.Connection.RetryLogicProvider = RetryProvider;
					this.Connection.RetryLogicProvider.Retrying += this.OnRetrying;
				}

				if ( ArtificialDatabaseDelay is not null ) {
					await Task.Delay( ArtificialDatabaseDelay.ToSeconds(), cancellationToken ).ConfigureAwait( false );
				}

				if ( cancellationToken.IsCancellationRequested ) {
					return Status.Cancel;
				}

				FluentTimer? timer = null;

				if ( progress is not null ) {
					var stopwatch = Stopwatch.StartNew();
					var sqlConnection = this.Connection;
					timer = Fps.Thirty.CreateTimer( () => progress.Report( (stopwatch.Elapsed, sqlConnection.State) ) );
				}

				try {
					this.TimeSinceLastConnectAttempt.Restart();

					//$"Opening connection for {this.Query}..".Verbose();
					await this.Connection.OpenAsync( cancellationToken ).ConfigureAwait( false );

					while ( this.Connection?.State == ConnectionState.Connecting ) {
						"awaiting for connection state to become open..".Verbose();
						await Task.Delay( Milliseconds.One, cancellationToken ).ConfigureAwait( false );
					}

					Interlocked.Increment( ref ConnectionCounter );
					return Status.Continue;
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
			var metaData = new SqlMetaData[1];
			metaData[0] = new SqlMetaData( columnName, sqlDbType );
			SqlDataRecord record = new( metaData );
			foreach ( var row in rows ) {
				if ( row is not null ) {
					record.SetValue( 0, row );
				}

				yield return record;
			}
		}

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

		public async Task<Boolean> CreateDatabase( String databaseName, IValidatedConnectionString connectionString, CancellationToken cancellationToken ) {
			databaseName = databaseName.Trimmed() ?? throw new ArgumentEmptyException( nameof( databaseName ) );

			var retries = DefaultRetries;

		TryAgain:

			try {
				await using var db = new DatabaseServer( connectionString, this.ApplicationSetting, QueryTiming.ReportTiming );

				await db.QueryAdhocReaderAsync( $"create database {databaseName.SmartBraces()};", cancellationToken ).ConfigureAwait( false );

				return true;
			}
			catch ( Exception exception ) {
				if ( exception.AttemptQueryAgain( ref retries ) ) {
					this.CloseConnection();
					await Task.Delay( DefaultTimeBetweenRetries, cancellationToken ).ConfigureAwait( false );
					goto TryAgain;
				}

				exception.Log( BreakOrDontBreak.Break );
			}

			return false;
		}

		public override ValueTask DisposeManagedAsync() {
			this.BreakOnSlowQueries();
			this.CloseConnection();
			using ( this.Command ) { }

			return ValueTask.CompletedTask;
		}

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
				await this.PopulateCommand( commandType, cancellationToken, parameters ).ConfigureAwait( false );
				return await this.Command.ExecuteNonQueryAsync( cancellationToken )!.ConfigureAwait( false );
			}
			catch ( Exception exception ) {
				if ( exception.AttemptQueryAgain( ref retries ) ) {
					this.CloseConnection();
					await Task.Delay( DefaultTimeBetweenRetries, cancellationToken ).ConfigureAwait( false );
					goto TryAgain;
				}

				exception.Log();
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
		public async Task<Int32?> ExecuteNonQueryAsync( String query, CancellationToken cancellationToken, params SqlParameter[]? parameters ) =>
			await this.ExecuteNonQueryAsync( query, CommandType.StoredProcedure, cancellationToken, parameters ).ConfigureAwait( false );

		/// <param name="query">      </param>
		/// <param name="commandType"></param>
		/// <param name="cancellationToken"></param>
		/// <param name="parameters"> </param>
		[NeedsTesting]
		public async Task<DataTableReader?> ExecuteReaderAsync(
			String query,
			CommandType commandType,
			CancellationToken cancellationToken,
			params SqlParameter[]? parameters
		) {
			if ( String.IsNullOrWhiteSpace( query ) ) {
				throw new NullException( nameof( query ) );
			}

			this.Query = query;

			var retries = DefaultRetries;

		TryAgain:

			try {
				await this.PopulateCommand( commandType, cancellationToken, parameters ).ConfigureAwait( false );

				await using var readerAsync = await this.Command.ExecuteReaderAsync( cancellationToken ).ConfigureAwait( false );

				if ( readerAsync != null ) {
					using var table = readerAsync.ToDataTable();

					return table.CreateDataReader();
				}
			}
			catch ( Exception exception ) {
				if ( exception.AttemptQueryAgain( ref retries ) ) {
					this.CloseConnection();
					await Task.Delay( DefaultTimeBetweenRetries, cancellationToken ).ConfigureAwait( false );
					goto TryAgain;
				}

				exception.Log();
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
		public async Task<DataTable> ExecuteReaderDataTableAsync(
			String query,
			CommandType commandType,
			CancellationToken cancellationToken,
			params SqlParameter[]? parameters
		) {
			if ( String.IsNullOrWhiteSpace( query ) ) {
				throw new NullException( nameof( query ) );
			}

			this.Query = query;

			var retries = DefaultRetries;

		TryAgain:
			var table = new DataTable();

			try {
				await this.PopulateCommand( commandType, cancellationToken, parameters ).ConfigureAwait( false );

				using var reader = this.Command.ExecuteReaderAsync( cancellationToken );

				if ( reader != null ) {
					table.BeginLoadData();
					table.Load( await reader.ConfigureAwait( false ) );
					table.EndLoadData();
				}
			}
			catch ( Exception exception ) {
				if ( exception.AttemptQueryAgain( ref retries ) ) {
					this.CloseConnection();
					await Task.Delay( DefaultTimeBetweenRetries, cancellationToken ).ConfigureAwait( false );
					goto TryAgain;
				}

				exception.Log();
			}

			return table;
		}

		/// <summary>
		///     <para>Returns the first column of the first row.</para>
		/// </summary>
		/// <param name="query">      </param>
		/// <param name="commandType"></param>
		/// <param name="cancellationToken"></param>
		/// <param name="parameters"> </param>
		public async Task<T?> ExecuteScalarAsync<T>( String query, CommandType commandType, CancellationToken cancellationToken, params SqlParameter[]? parameters ) {
			if ( String.IsNullOrWhiteSpace( query ) ) {
				throw new NullException( nameof( query ) );
			}

			this.Query = query;

			var retries = DefaultRetries;

		TryAgain:

			try {
				await this.PopulateCommand( commandType, cancellationToken, parameters ).ConfigureAwait( false );

				var result = await this.Command.ExecuteScalarAsync( cancellationToken ).ConfigureAwait( false );

				return result is null ? default( T? ) : result.Cast<Object, T>();
			}
			catch ( InvalidCastException exception ) {

				//TIP: check for SQLServer returning a Double when you expect a Single (float in SQL).
				exception.Log();

				throw;
			}
			catch ( Exception exception ) {
				if ( exception.AttemptQueryAgain( ref retries ) ) {
					this.CloseConnection();
					await Task.Delay( DefaultTimeBetweenRetries, cancellationToken ).ConfigureAwait( false );
					goto TryAgain;
				}

				exception.Log();
			}

			return default( T? );
		}

		/// <summary>
		///     <para>Returns the first column of the first row.</para>
		/// </summary>
		/// <param name="query">     </param>
		/// <param name="cancellationToken"></param>
		/// <param name="parameters"></param>
		public async Task<T?> ExecuteScalarAsync<T>( String query, CancellationToken cancellationToken, params SqlParameter[]? parameters ) =>
			await this.ExecuteScalarAsync<T?>( query, CommandType.StoredProcedure, cancellationToken, parameters ).ConfigureAwait( false );

		/// <summary>
		///     Overwrites the <paramref name="table" /> contents with data from the <paramref name="query" />.
		///     <para>Note: Include the parameters after the query.</para>
		///     <para>Can throw exceptions on connecting or executing the query.</para>
		/// </summary>
		/// <param name="query">      </param>
		/// <param name="commandType"></param>
		/// <param name="table">      </param>
		/// <param name="cancellationToken"></param>
		/// <param name="progress"></param>
		/// <param name="parameters"> </param>
		public async Task<Boolean> FillTableAsync(
			String query,
			CommandType commandType,
			DataTable table,
			CancellationToken cancellationToken,
			IProgress<(TimeSpan Elapsed, ConnectionState State)>? progress = null,
			params SqlParameter[]? parameters
		) {
			if ( table is null ) {
				throw new NullException( nameof( table ) );
			}

			this.Query = query;

			var retries = DefaultRetries;

		TryAgain:
			table.Clear();

			try {
				await this.OpenConnectionAsync( cancellationToken, retries, progress ).ConfigureAwait( false );

				var connection = this.Connection;
				if ( connection is null ) {
					return false;
				}

				using var dataAdapter = new SqlDataAdapter( query, connection ) {
					AcceptChangesDuringFill = false,
					FillLoadOption = LoadOption.OverwriteChanges,
					MissingMappingAction = MissingMappingAction.Passthrough,
					MissingSchemaAction = MissingSchemaAction.Add,
					SelectCommand = {
						CommandTimeout = ( Int32 )this.CommandTimeout.TotalSeconds, CommandType = commandType
					}
				};

				if ( parameters != null ) {
					dataAdapter.SelectCommand?.Parameters?.AddRange( parameters );
				}

				dataAdapter.Fill( table );
			}
			catch ( Exception exception ) {
				if ( exception.AttemptQueryAgain( ref retries ) ) {
					this.CloseConnection();
					await Task.Delay( DefaultTimeBetweenRetries, cancellationToken ).ConfigureAwait( false );
					goto TryAgain;
				}

				exception.Log();
			}

			return true;
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

		public String? GetConnectionString() => this.ConnectionString;

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

		[NeedsTesting]
		public async ValueTask PopulateCommand( CommandType commandType, CancellationToken cancellationToken, params SqlParameter[]? parameters ) {
			if ( String.IsNullOrWhiteSpace( this.Query ) ) {
				throw new NullException( nameof( this.Query ) );
			}

			await this.OpenConnectionAsync( cancellationToken ).ConfigureAwait( false );

			//$"Setting command value for query {this.Query.DoubleQuote()}..".Verbose();
			this.Command.CommandType = commandType;
			this.Command.CommandText = this.Query;
			this.Command.Connection = this.Connection;
			this.Command.CommandTimeout = ( Int32 )this.CommandTimeout.TotalSeconds;

			if ( this.Command.Parameters is null || parameters?.Length.Any() != true || parameters.Length == this.Command.Parameters.Count ) {
				return;
			}

			foreach ( var parameter in parameters ) {

				//$"Adding parameter {parameter.ParameterName} to command object..".Verbose();
				this.Command.Parameters.Add( parameter );
			}
		}

		public async Task<DatabaseServer> QueryAdhocAsync( String query, CancellationToken cancellationToken, params SqlParameter[]? parameters ) {
			if ( String.IsNullOrWhiteSpace( query ) ) {
				throw new NullException( nameof( query ) );
			}

			this.Query = query;
			var retries = DefaultRetries;

		TryAgain:

			try {
				await this.PopulateCommand( CommandType.Text, cancellationToken, parameters ).ConfigureAwait( false );

				await this.Command.ExecuteNonQueryAsync( cancellationToken ).ConfigureAwait( false );
			}
			catch ( Exception exception ) {
				if ( exception.AttemptQueryAgain( ref retries ) ) {
					this.CloseConnection();
					await Task.Delay( DefaultTimeBetweenRetries, cancellationToken ).ConfigureAwait( false );
					goto TryAgain;
				}

				exception.Log();
			}

			return this;
		}

		public async Task<DataTableReader?> QueryAdhocReaderAsync( String query, CancellationToken cancellationToken, params SqlParameter[]? parameters ) {
			if ( String.IsNullOrWhiteSpace( query ) ) {
				throw new NullException( nameof( query ) );
			}

			this.Query = query;
			var retries = DefaultRetries;

		TryAgain:

			try {
				await this.PopulateCommand( CommandType.Text, cancellationToken, parameters ).ConfigureAwait( false );

				await using var reader = await this.Command.ExecuteReaderAsync( cancellationToken ).ConfigureAwait( false );

				using var table = reader.ToDataTable();

				return table.CreateDataReader();
			}
			catch ( Exception exception ) {
				if ( exception.AttemptQueryAgain( ref retries ) ) {
					this.CloseConnection();
					await Task.Delay( DefaultTimeBetweenRetries, cancellationToken ).ConfigureAwait( false );
					goto TryAgain;
				}

				exception.Log();
			}

			return default( DataTableReader? );
		}

		/// <summary>
		///     Simplest possible database connection.
		///     <para>Connect and then run <paramref name="query" />.</para>
		/// </summary>
		/// <param name="query">      </param>
		/// <param name="commandType"></param>
		/// <param name="cancellationToken"></param>
		/// <param name="parameters"> </param>
		/// <exception cref="NullException"></exception>
		/// <exception cref="InvalidOperationException"></exception>
		/// <exception cref="InvalidCastException"></exception>
		public async Task<SqlDataReader?> QueryAsync( String query, CommandType commandType, CancellationToken cancellationToken, params SqlParameter[]? parameters ) {
			if ( String.IsNullOrWhiteSpace( query ) ) {
				throw new NullException( nameof( query ) );
			}

			this.Query = query;
			var retries = DefaultRetries;

		TryAgain:

			try {
				await this.PopulateCommand( commandType, cancellationToken, parameters ).ConfigureAwait( false );

				await using var result = await this.Command.ExecuteReaderAsync( cancellationToken ).ConfigureAwait( false );

				return result;
			}
			catch ( Exception exception ) {
				if ( exception.AttemptQueryAgain( ref retries ) ) {
					this.CloseConnection();
					await Task.Delay( DefaultTimeBetweenRetries, cancellationToken ).ConfigureAwait( false );
					goto TryAgain;
				}

				exception.Log();
			}

			return default( SqlDataReader? );
		}

		/// <summary>
		///     Simplest possible database connection.
		///     <para>Connect and then run <paramref name="query" />.</para>
		/// </summary>
		/// <param name="query">     </param>
		/// <param name="cancellationToken"></param>
		/// <param name="parameters"></param>
		/// <exception cref="NullException"></exception>
		/// <exception cref="InvalidOperationException"></exception>
		public async Task<SqlDataReader?> QueryAsync( String query, CancellationToken cancellationToken, params SqlParameter[]? parameters ) {
			if ( String.IsNullOrWhiteSpace( query ) ) {
				throw new NullException( nameof( query ) );
			}

			this.Query = query;
			var retries = DefaultRetries;

		TryAgain:

			try {
				await this.PopulateCommand( CommandType.StoredProcedure, cancellationToken, parameters ).ConfigureAwait( false );

				return await this.Command.ExecuteReaderAsync( cancellationToken ).ConfigureAwait( false );
			}
			catch ( InvalidCastException exception ) {

				//TIP: check for SQLServer returning a Double when you expect a Single (float in SQL).
				exception.Log();

				throw;
			}
			catch ( Exception exception ) {
				if ( exception.AttemptQueryAgain( ref retries ) ) {
					this.CloseConnection();
					await Task.Delay( DefaultTimeBetweenRetries, cancellationToken ).ConfigureAwait( false );
					goto TryAgain;
				}

				exception.Log();
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
		public async Task<IEnumerable<TResult>?> QueryListAsync<TResult>(
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
				await this.PopulateCommand( commandType, cancellationToken, parameters ).ConfigureAwait( false );

				var reader = await this.Command.ExecuteReaderAsync( cancellationToken ).ConfigureAwait( false );

				if ( reader != null ) {
					return GenericPopulatorExtensions.CreateList<TResult>( reader );
				}
			}
			catch ( Exception exception ) {
				if ( exception.AttemptQueryAgain( ref retries ) ) {
					this.CloseConnection();
					await Task.Delay( DefaultTimeBetweenRetries, cancellationToken ).ConfigureAwait( false );
					goto TryAgain;
				}

				exception.Log();
			}

			return default( IEnumerable<TResult>? );
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

				using var dataReader = this.Command.ExecuteReader();

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

				exception.Log(  );
			}
			finally {
				this.CloseConnection();
			}

			return false;
		}
		*/
		/*
		public void UseDatabase( String databaseName ) {
			if ( String.IsNullOrWhiteSpace( databaseName ) ) {
				throw new NullException( "Value cannot be null or whitespace.", nameof( databaseName ) );
			}

			using var _ = this.QueryAdHoc( $"USE {databaseName.SmartBraces()};" );
		}
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
				await this.PopulateCommand( CommandType.StoredProcedure, cancellationToken, parameters ).ConfigureAwait( false );

				return await this.Command.ExecuteNonQueryAsync( cancellationToken ).ConfigureAwait( false );
			}
			catch ( Exception exception ) {
				if ( exception.AttemptQueryAgain( ref retries ) ) {
					this.CloseConnection();
					await Task.Delay( DefaultTimeBetweenRetries, cancellationToken ).ConfigureAwait( false );
					goto TryAgain;
				}

				exception.Log();
			}

			return default( Int32? );
		}

		public async Task UseDatabaseAsync( String databaseName, CancellationToken cancellationToken ) {
			if ( String.IsNullOrWhiteSpace( databaseName ) ) {
				throw new NullException( nameof( databaseName ) );
			}

			await using var _ = await this.QueryAdhocReaderAsync( $"USE {databaseName.SmartBraces()};", cancellationToken ).ConfigureAwait( false );
		}

		/*
		private static void ValueChangedHandler( AsyncLocalValueChangedArgs<SqlConnection?> obj ) {
			$"Thread {Environment.CurrentManagedThreadId}: was {( obj.PreviousValue?.ToString() ).SmartQuote()} to {( obj.CurrentValue?.ToString() ).SmartQuote()}."
				.Verbose();
		}
		*/
		/*

		/// <summary>
		///     Opens a connection, runs the <paramref name="query" />, and returns the number of rows affected.
		/// </summary>
		/// <param name="query">     </param>
		/// <param name="parameters"></param>
		public Int32? ExecuteNonQuery( String query, params SqlParameter[]? parameters ) => this.ExecuteNonQuery( query, DefaultRetries, CommandType.StoredProcedure, parameters );
		*/
	}
}