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
// Our software can be found at "https://Protiguous.com/Software/"
// Our GitHub address is "https://github.com/Protiguous".
// 
// File "DatabaseServer.cs" last formatted on 2022-12-22 at 5:15 PM by Protiguous.

#nullable enable

namespace Librainian.Databases;

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
using Configuration;
using Converters;
using Exceptions;
using Logging;
using Maths;
using Measurement.Time;
using Measurement.Time.Clocks;
using Microsoft.Data.SqlClient;
using Microsoft.Data.SqlClient.Server;
using Parsing;
using PooledAwait;
using Utilities;
using Utilities.Disposables;

public class DatabaseServer : ABetterClassDisposeAsync, IDatabaseServer {

	private const Int32 DefaultRetriesLeft = 10;

	/// <summary>The number of sql connections open across ALL threads.</summary>
	private static Int64 _connectionCounter;

	private Int32 _retriesLeft = DefaultRetriesLeft;

	private Int32 retriesLeft = DefaultRetriesLeft;

	static DatabaseServer() {
		var sqlRetryLogicOption = new SqlRetryLogicOption {
			NumberOfTries = DefaultRetriesLeft,
			DeltaTime = Minutes.One,
			MaxTimeInterval = Minutes.One
		};

		RetryProvider = SqlConfigurableRetryFactory.CreateIncrementalRetryProvider( sqlRetryLogicOption ) ??
		                throw new InvalidOperationException( $"{nameof( SqlConfigurableRetryFactory )} was null or invalid!" );
	}

	public DatabaseServer( IValidatedConnectionString validatedConnectionString, IApplicationSetting applicationSetting ) : base( applicationSetting.Info() ) {
		if ( validatedConnectionString is null ) {
			throw new ArgumentNullException( nameof( validatedConnectionString ) );
		}

		this.DefaultConnectionTimeout = Minutes.One;
		this.DefaultExecuteTimeout = Minutes.One;

		//TODO Implement a linear fallback instead of a flat timespan for DefaultTimeBetweenRetries.

		this.ConnectionString = validatedConnectionString.Value;
		this.ApplicationSetting = applicationSetting ?? throw new ArgumentNullException( nameof( applicationSetting ) );

		if ( String.IsNullOrWhiteSpace( this.ConnectionString ) ) {
			throw new NullException( nameof( this.ConnectionString ) );
		}

		this.EnteredCommandSemaphore = DatabaseCommandSemaphores.Wait( this.DefaultMaximumTimeout() );
		if ( !this.EnteredCommandSemaphore ) {
			throw new InvalidOperationException( "Timeout waiting to create SQL command object." );
		}

		this.Command = new SqlCommand();
		AppContext.SetSwitch( "Switch.Microsoft.Data.SqlClient.EnableRetryLogic", true );
	}

	/// <summary>Allow this many (1024 by default) concurrent async database Commands.</summary>
	private static SemaphoreSlim DatabaseCommandSemaphores { get; } = new(1024, 1024);

	/// <summary>Allow this many (1024 by default) concurrent async operations.</summary>
	private static SemaphoreSlim DatabaseConnectionSemaphores { get; } = new(1024, 1024);

	private static SqlRetryLogicBaseProvider RetryProvider { get; }

	private SqlCommand Command { get; }

	private SqlConnection? Connection { get; set; }

	private String? ConnectionString { get; init; }

	private Boolean EnteredCommandSemaphore { get; }

	private Boolean EnteredConnectionSemaphore { get; set; }

	private Stopwatch TimeSinceLastConnectAttempt { get; } = Stopwatch.StartNew();

	/// <summary>A debugging aid. EACH database call will delay upon opening a connection.</summary>
	public static IQuantityOfTime? ArtificialDatabaseDelay { get; set; }

	public IApplicationSetting ApplicationSetting { get; }

	/// <summary>Defaults to 1 minute.</summary>
	public TimeSpan DefaultConnectionTimeout { get; set; }

	/// <summary>Defaults to 1 minute.</summary>
	public TimeSpan DefaultExecuteTimeout { get; set; }

	public TimeSpan DefaultTimeBetweenRetries { get; set; } = Seconds.One;

	/// <summary>Defaults to 3 seconds.</summary>
	public TimeSpan? SlowQueriesTakeLongerThan { get; set; } = Seconds.Three;

	public Stopwatch? StopWatch { get; private set; }

	/// <summary>Set to 1 minute by default.</summary>
	public TimeSpan CommandTimeout { get; set; }

	public String? Query { get; set; }

	/// <summary>
	///     Execute the stored procedure " <paramref name="query" />" with the optional <paramref name="parameters" />.
	/// </summary>
	/// <param name="query"></param>
	/// <param name="cancellationToken"></param>
	/// <param name="parameters"></param>
	/// <exception cref="InvalidOperationException"></exception>
	/// <exception cref="SqlException"></exception>
	/// <exception cref="DbException"></exception>
	public async FireAndForget BeginQuery( String query, CancellationToken cancellationToken, params SqlParameter[]? parameters ) =>
		await this.ExecuteNonQueryAsync( query, CommandType.StoredProcedure, cancellationToken, parameters );

	/// <summary></summary>
	/// <param name="query"></param>
	/// <param name="commandType"></param>
	/// <param name="cancellationToken"></param>
	/// <param name="parameters"></param>
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
		$"ExecuteNonQueryAsync {this.Query.DoubleQuote()} starting..".Verbose();

		TryAgain:
		try {
			await this.PopulateCommandAndOpenConnection( commandType, cancellationToken, parameters ).ConfigureAwait( false );
			return await this.Command.ExecuteNonQueryAsync( cancellationToken )!.ConfigureAwait( false );
		}
		catch ( Exception exception ) {
			$"Query {this.Query.DoubleQuote()} timed out.".Verbose();
			if ( exception.AttemptQueryAgain( ref this.retriesLeft ) ) {
				this.CloseConnection();
				await Task.Delay( this.DefaultTimeBetweenRetries, cancellationToken ).ConfigureAwait( false );
				goto TryAgain;
			}

			exception.Log();
		}
		finally {
			$"ExecuteNonQueryAsync {this.Query.DoubleQuote()} done.".Verbose();
		}

		throw new InvalidOperationException( $"Database call {nameof( ExecuteNonQueryAsync )} failed." );
	}

	/// <summary>
	///     Execute the stored procedure " <paramref name="query" />" with the optional parameters
	///     <paramref name="parameters" />.
	/// </summary>
	/// <param name="query"></param>
	/// <param name="cancellationToken"></param>
	/// <param name="parameters"></param>
	/// <exception cref="InvalidOperationException"></exception>
	/// <exception cref="SqlException"></exception>
	/// <exception cref="DbException"></exception>
	public PooledValueTask<Int32?> ExecuteNonQueryAsync( String query, CancellationToken cancellationToken, params SqlParameter[] parameters ) =>
		this.ExecuteNonQueryAsync( query, CommandType.StoredProcedure, cancellationToken, parameters );

	/// <summary></summary>
	/// <param name="query"></param>
	/// <param name="commandType"></param>
	/// <param name="cancellationToken"></param>
	/// <param name="parameters"></param>
	[NeedsTesting]
	public async Task<DataTableReader?> ExecuteReaderAsync( String query, CommandType commandType, CancellationToken cancellationToken, params SqlParameter[]? parameters ) {
		if ( String.IsNullOrWhiteSpace( query ) ) {
			throw new NullException( nameof( query ) );
		}

		this.Query = query;

		TryAgain:

		try {
			await this.PopulateCommandAndOpenConnection( commandType, cancellationToken, parameters ).ConfigureAwait( false );

			await using var readerAsync = await this.Command.ExecuteReaderAsync( cancellationToken )!.ConfigureAwait( false );

			if ( readerAsync != null ) {
				using var table = readerAsync.ToDataTable();

				return table.CreateDataReader();
			}
		}
		catch ( SqlException exception ) {
			if ( exception.AttemptQueryAgain( ref this.retriesLeft ) ) {
				this.CloseConnection();
				await Task.Delay( this.DefaultTimeBetweenRetries, cancellationToken ).ConfigureAwait( false );
				goto TryAgain;
			}

			exception.Log();
		}
		catch ( Exception exception ) {
			if ( exception.AttemptQueryAgain( ref this.retriesLeft ) ) {
				this.CloseConnection();
				await Task.Delay( this.DefaultTimeBetweenRetries, cancellationToken ).ConfigureAwait( false );
				goto TryAgain;
			}

			exception.Log();
		}

		return default( DataTableReader? );
	}

	/// <summary>Returns a <see cref="DataTable" /></summary>
	/// <param name="query"></param>
	/// <param name="commandType"></param>
	/// <param name="cancellationToken"></param>
	/// <param name="parameters"></param>
	public async Task<DataTable> ExecuteReaderDataTableAsync( String query, CommandType commandType, CancellationToken cancellationToken, params SqlParameter[]? parameters ) {
		if ( String.IsNullOrWhiteSpace( query ) ) {
			throw new NullException( nameof( query ) );
		}

		this.Query = query;

		TryAgain:
		var table = new DataTable();

		try {
			await this.PopulateCommandAndOpenConnection( commandType, cancellationToken, parameters ).ConfigureAwait( false );

			using var reader = this.Command.ExecuteReaderAsync( cancellationToken );

			if ( reader != null ) {
				table.BeginLoadData();
				table.Load( await reader.ConfigureAwait( false ) );
				table.EndLoadData();
			}
		}
		catch ( Exception exception ) {
			if ( exception.AttemptQueryAgain( ref this.retriesLeft ) ) {
				this.CloseConnection();
				await Task.Delay( this.DefaultTimeBetweenRetries, cancellationToken ).ConfigureAwait( false );
				goto TryAgain;
			}

			exception.Log();
		}

		return table;
	}

	/// <summary>
	///     <para>Returns the first column of the first row.</para>
	/// </summary>
	/// <param name="query"></param>
	/// <param name="commandType"></param>
	/// <param name="cancellationToken"></param>
	/// <param name="parameters"></param>
	public async Task<T?> ExecuteScalarAsync<T>( String query, CommandType commandType, CancellationToken cancellationToken, params SqlParameter[]? parameters ) {
		if ( String.IsNullOrWhiteSpace( query ) ) {
			throw new NullException( nameof( query ) );
		}

		this.Query = query;

		TryAgain:

		try {
			await this.PopulateCommandAndOpenConnection( commandType, cancellationToken, parameters ).ConfigureAwait( false );

			var result = await this.Command.ExecuteScalarAsync( cancellationToken )!.ConfigureAwait( false );

			return result is null ? default( T? ) : result.Cast<Object, T>();
		}
		catch ( InvalidCastException exception ) {
			//TIP: check for SQLServer returning a Double when you expect a Single (float in SQL).
			exception.Log();

			throw;
		}
		catch ( Exception exception ) {
			if ( exception.AttemptQueryAgain( ref this.retriesLeft ) ) {
				this.CloseConnection();
				await Task.Delay( this.DefaultTimeBetweenRetries, cancellationToken ).ConfigureAwait( false );
				goto TryAgain;
			}

			exception.Log();
		}

		return default( T? );
	}

	/// <summary>
	///     Overwrites the <paramref name="table" /> contents with data from the <paramref name="query" />.
	///     <para>Note: Include the parameters after the query.</para>
	///     <para>Can throw exceptions on connecting or executing the query.</para>
	/// </summary>
	/// <param name="query"></param>
	/// <param name="commandType"></param>
	/// <param name="table"></param>
	/// <param name="cancellationToken"></param>
	/// <param name="progress"></param>
	/// <param name="parameters"></param>
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

		TryAgain:
		table.Clear();

		try {
			var status = await this.OpenConnectionAsync( cancellationToken ).ConfigureAwait( false );
			if ( status.IsBad() ) {
				return false;
			}

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
					CommandTimeout = ( Int32 ) this.CommandTimeout.TotalSeconds,
					CommandType = commandType
				}
			};

			if ( parameters != null ) {
				dataAdapter.SelectCommand?.Parameters?.AddRange( parameters );
			}

			dataAdapter.Fill( table );
		}
		catch ( Exception exception ) {
			if ( exception.AttemptQueryAgain( ref this.retriesLeft ) ) {
				this.CloseConnection();
				await Task.Delay( this.DefaultTimeBetweenRetries, cancellationToken ).ConfigureAwait( false );
				goto TryAgain;
			}

			exception.Log();
		}

		return true;
	}

	public async Task<DatabaseServer> QueryAdhocAsync( String query, CancellationToken cancellationToken, params SqlParameter[]? parameters ) {
		if ( String.IsNullOrWhiteSpace( query ) ) {
			throw new NullException( nameof( query ) );
		}

		this.Query = query;

		TryAgain:

		try {
			await this.PopulateCommandAndOpenConnection( CommandType.Text, cancellationToken, parameters ).ConfigureAwait( false );

			var task = this.Command.ExecuteNonQueryAsync( cancellationToken );
			if ( task != null ) {
				_ = await task.ConfigureAwait( false );
			}
		}
		catch ( Exception exception ) {
			if ( exception.AttemptQueryAgain( ref this.retriesLeft ) ) {
				this.CloseConnection();
				await Task.Delay( this.DefaultTimeBetweenRetries, cancellationToken ).ConfigureAwait( false );
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

		TryAgain:

		try {
			await this.PopulateCommandAndOpenConnection( CommandType.Text, cancellationToken, parameters ).ConfigureAwait( false );

			await using var reader = await this.Command.ExecuteReaderAsync( cancellationToken )!.ConfigureAwait( false );

			using var table = reader.ToDataTable();

			return table.CreateDataReader();
		}
		catch ( Exception exception ) {
			if ( exception.AttemptQueryAgain( ref this.retriesLeft ) ) {
				this.CloseConnection();
				await Task.Delay( this.DefaultTimeBetweenRetries, cancellationToken ).ConfigureAwait( false );
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
	/// <param name="query"></param>
	/// <param name="commandType"></param>
	/// <param name="cancellationToken"></param>
	/// <param name="parameters"></param>
	/// <exception cref="NullException"></exception>
	/// <exception cref="InvalidOperationException"></exception>
	/// <exception cref="InvalidCastException"></exception>
	public async Task<SqlDataReader?> QueryAsync( String query, CommandType commandType, CancellationToken cancellationToken, params SqlParameter[]? parameters ) {
		if ( String.IsNullOrWhiteSpace( query ) ) {
			throw new NullException( nameof( query ) );
		}

		this.Query = query;

		TryAgain:

		try {
			await this.PopulateCommandAndOpenConnection( commandType, cancellationToken, parameters ).ConfigureAwait( false );

			await using var result = await this.Command.ExecuteReaderAsync( cancellationToken )!.ConfigureAwait( false );

			return result;
		}
		catch ( Exception exception ) {
			if ( exception.AttemptQueryAgain( ref this.retriesLeft ) ) {
				this.CloseConnection();
				await Task.Delay( this.DefaultTimeBetweenRetries, cancellationToken ).ConfigureAwait( false );
				goto TryAgain;
			}

			exception.Log();
		}

		return default( SqlDataReader? );
	}

	/// <summary>Returns a <see cref="DataTable" /></summary>
	/// <param name="query"></param>
	/// <param name="commandType"></param>
	/// <param name="cancellationToken"></param>
	/// <param name="parameters"></param>
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

		TryAgain:

		try {
			await this.PopulateCommandAndOpenConnection( commandType, cancellationToken, parameters ).ConfigureAwait( false );

			var reader = await this.Command.ExecuteReaderAsync( cancellationToken )!.ConfigureAwait( false );

			if ( reader != null ) {
				return GenericPopulatorExtensions.CreateList<TResult>( reader );
			}
		}
		catch ( Exception exception ) {
			if ( exception.AttemptQueryAgain( ref this.retriesLeft ) ) {
				this.CloseConnection();
				await Task.Delay( this.DefaultTimeBetweenRetries, cancellationToken ).ConfigureAwait( false );
				goto TryAgain;
			}

			exception.Log();
		}

		return default( IEnumerable<TResult>? );
	}

	/// <summary>Execute the stored procedure <paramref name="query" /> with the optional <paramref name="parameters" />.</summary>
	/// <param name="query"></param>
	/// <param name="cancellationToken"></param>
	/// <param name="parameters"></param>
	/// <exception cref="InvalidOperationException"></exception>
	/// <exception cref="SqlException"></exception>
	/// <exception cref="DbException"></exception>
	public PooledValueTask<Int32?> RunSprocAsync( String query, CancellationToken cancellationToken, params SqlParameter[]? parameters ) =>
		this.ExecuteNonQueryAsync( query, CommandType.StoredProcedure, cancellationToken, parameters );

	public async Task UseDatabaseAsync( String databaseName, CancellationToken cancellationToken ) {
		if ( String.IsNullOrWhiteSpace( databaseName ) ) {
			throw new NullException( nameof( databaseName ) );
		}

		await using var adhoc = await this.QueryAdhocAsync( $"USE {databaseName.SmartBrackets()};", cancellationToken ).ConfigureAwait( false );
	}

	~DatabaseServer() {
		new InvalidOperationException(
			$"Warning: We have an undisposed {nameof( DatabaseServer )} connection somewhere. This could cause a memory leak. Query={this.Query.DoubleQuote()}" ).Log(
			BreakOrDontBreak.Break );
		this.DisposeAsync().AsTask().Wait( this.DefaultMaximumTimeout() );
	}

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
		if ( Debugger.IsAttached && this.SlowQueriesTakeLongerThan is not null && stopWatch.Elapsed >= this.SlowQueriesTakeLongerThan ) {
			$"Query {this.Query.DoubleQuote()} took {stopWatch.Elapsed.Simpler()}.".DebugLine();
		}
	}

	private void CloseConnection() {
		try {
			using ( this.Connection ) {
				try {
					this.Connection?.Close();
				}
				catch ( SqlException ) { }
			}

			if ( this.EnteredConnectionSemaphore ) {
				DatabaseConnectionSemaphores.Release();
			}

			if ( this.EnteredCommandSemaphore ) {
				DatabaseCommandSemaphores.Release();
			}
		}
		catch ( InvalidOperationException ) {
			/* swallow */
		}
		catch ( SqlException exception ) {
			exception.Log( BreakOrDontBreak.Break );
		}
		finally {
			Interlocked.Decrement( ref _connectionCounter );
			this.Connection = default( SqlConnection? );
		}
	}

	/// <summary>Return true if any retries are available.</summary>
	/// <returns></returns>
	private Boolean DecrementRetries() => ( --this._retriesLeft ).Any();

	private void OnRetrying( Object? sender, SqlRetryingEventArgs e ) {
		$"Retrying SQL {this.Query.DoubleQuote()}. Retry count={e.RetryCount}.".Verbose();

		e.Cancel = this.IsDisposed;
	}

	/// <summary>Create a sql server database connection via async.</summary>
	[NeedsTesting]
	private async PooledValueTask<Status> OpenConnectionAsync( CancellationToken cancellationToken ) {
		Debug.Assert( !String.IsNullOrWhiteSpace( this.ConnectionString ) );
		if ( String.IsNullOrWhiteSpace( this.ConnectionString ) ) {
			throw new NullException( nameof( this.ConnectionString ) );
		}

		try {
			if ( !this.retriesLeft.Any() ) {
				return Status.Stop;
			}

			this.EnteredConnectionSemaphore = await DatabaseConnectionSemaphores.WaitAsync( this.DefaultConnectionTimeout, cancellationToken ).ConfigureAwait( false );
			if ( !this.EnteredConnectionSemaphore ) {
				return Status.Timeout;
			}

			if ( this.SlowQueriesTakeLongerThan is not null ) {
				this.StopWatch = Stopwatch.StartNew();
			}

			AttachInfoMessage();

			AttachRetryLogic();

			await IntroduceArtificialDelay().ConfigureAwait( false );

			if ( cancellationToken.IsCancellationRequested ) {
				return Status.Cancel;
			}

			//FluentTimer? timer = null;

			//if ( progress is not null ) {
			//var stopwatch = Stopwatch.StartNew();
			//var sqlConnection = this.Connection ?? throw new NullException( nameof( this.Connection ) );
			//timer = FluentTimer.Create( Fps.Thirty, () => progress.Report( (stopwatch.Elapsed, sqlConnection.State) ) ).AutoReset( true );
			//}

			var connection = this.Connection;
			if ( connection == null ) {
				throw new NullException( nameof( this.Connection ) );
			}

			try {
				this.TimeSinceLastConnectAttempt.Restart();

				await connection.OpenAsync( cancellationToken )!.ConfigureAwait( false );

				var counter = Interlocked.Increment( ref _connectionCounter );
				$"{counter} active database connections.".Verbose();

				this.Command.Connection = this.Connection;

				return Status.Continue;
			}
			catch ( InvalidOperationException exception ) {
				return await InCaseOfException( exception ).ConfigureAwait( false );
			}
			catch ( SqlException exception ) {
				return await InCaseOfException( exception ).ConfigureAwait( false );
			}
			catch ( Exception exception ) {
				return await InCaseOfException( exception ).ConfigureAwait( false );
			}
		}
		catch ( InvalidOperationException exception ) {
			return await InCaseOfException( exception ).ConfigureAwait( false );
		}
		catch ( SqlException exception ) {
			return await InCaseOfException( exception ).ConfigureAwait( false );
		}
		catch ( DbException exception ) {
			return await InCaseOfException( exception ).ConfigureAwait( false );
		}
		catch ( TaskCanceledException cancelled ) {
			$"Open database connection was {nameof( cancelled ).DoubleQuote()}.".Verbose();
		}

		return Status.Unknown;

		void AttachInfoMessage() {
			if ( this.Connection is null ) {
				this.Connection = new SqlConnection( this.ConnectionString );
				this.Connection.InfoMessage += ConnectionOnInfoMessage;
			}
		}

		void AttachRetryLogic() {
			var sqlConnection = this.Connection;
			if ( sqlConnection is {RetryLogicProvider: null} ) {
				sqlConnection.RetryLogicProvider = RetryProvider;
				sqlConnection.RetryLogicProvider.Retrying = this.OnRetrying;
			}
		}

		async Task IntroduceArtificialDelay() {
			if ( ArtificialDatabaseDelay is not null ) {
				await Task.Delay( ArtificialDatabaseDelay.ToSeconds(), cancellationToken ).ConfigureAwait( false );
			}
		}

		async PooledValueTask<Status> InCaseOfException<T>( T exception ) where T : Exception {
			if ( exception.AttemptQueryAgain( ref this.retriesLeft ) ) {
				await Task.Delay( this.DefaultTimeBetweenRetries, cancellationToken ).ConfigureAwait( false );

				var status = await this.OpenConnectionAsync( cancellationToken ).ConfigureAwait( false );

				if ( status.IsGood() ) {
					return status;
				}
			}

			throw exception.Log();
		}
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
			TimeClock => SqlDbType.Time,
			DateTimeOffset => SqlDbType.DateTimeOffset,
			var _ => SqlDbType.Variant
		};
	}

	public async Task<Boolean> CreateDatabase( String databaseName, IValidatedConnectionString connectionString, CancellationToken cancellationToken ) {
		databaseName = databaseName.Trimmed() ?? throw new ArgumentEmptyException( nameof( databaseName ) );

		TryAgain:

		try {
			await using var database = new DatabaseServer( connectionString, this.ApplicationSetting );

			await using var adhoc = await database.QueryAdhocAsync( $"create database {databaseName.SmartBrackets()};", cancellationToken ).ConfigureAwait( false );

			return true;
		}
		catch ( Exception exception ) {
			if ( exception.AttemptQueryAgain( ref this.retriesLeft ) ) {
				this.CloseConnection();
				await Task.Delay( this.DefaultTimeBetweenRetries, cancellationToken ).ConfigureAwait( false );
				goto TryAgain;
			}

			exception.Log( BreakOrDontBreak.Break );
		}

		return false;
	}

	public TimeSpan DefaultMaximumTimeout() => this.DefaultConnectionTimeout + this.DefaultExecuteTimeout;

	public override ValueTask DisposeManagedAsync() {
		this.BreakOnSlowQueries();
		this.CloseConnection();
		using ( this.Command ) { }

		return ValueTask.CompletedTask;
	}

	/// <summary>
	///     <para>Returns the first column of the first row.</para>
	/// </summary>
	/// <param name="query"></param>
	/// <param name="cancellationToken"></param>
	/// <param name="parameters"></param>
	public async Task<T?> ExecuteScalarAsync<T>( String query, CancellationToken cancellationToken, params SqlParameter[]? parameters ) =>
		await this.ExecuteScalarAsync<T?>( query, CommandType.StoredProcedure, cancellationToken, parameters ).ConfigureAwait( false );

	public String? GetConnectionString() => this.ConnectionString;

	public async Task<IDictionary?> GetStats( CancellationToken cancellationToken ) {
		try {
			var status = await this.OpenConnectionAsync( cancellationToken ).ConfigureAwait( false );
			if ( status.IsBad() ) {
				return default( IDictionary? );
			}

			return this.Connection?.RetrieveStatistics();
		}
		catch ( Exception exception ) {
			exception.Log();
		}
		finally {
			await this.DisposeManagedAsync().ConfigureAwait( false );
		}

		return default( IDictionary? );
	}

	[NeedsTesting]
	public async PooledValueTask<Status> PopulateCommandAndOpenConnection( CommandType commandType, CancellationToken cancellationToken, params SqlParameter[]? parameters ) {
		if ( String.IsNullOrWhiteSpace( this.Query ) ) {
			throw new NullException( nameof( this.Query ) );
		}

		//$"Setting command value for query {this.Query.DoubleQuote()}..".Verbose();
		this.Command.CommandType = commandType;
		this.Command.CommandText = this.Query;
		this.Command.CommandTimeout = ( Int32 ) this.CommandTimeout.TotalSeconds;

		var commandParameters = this.Command.Parameters;
		if ( commandParameters != null ) {
			if ( parameters != null ) {
				foreach ( var parameter in parameters ) {
					commandParameters.Add( parameter );
				}

				Debug.Assert( parameters.Length == commandParameters.Count );
			}
		}

		var status = await this.OpenConnectionAsync( cancellationToken ).ConfigureAwait( false );

		if ( status.IsBad() ) {
			throw new InvalidOperationException( "Error connecting to database server." );
		}

		return status;
	}

	/// <summary>
	///     Simplest possible database connection.
	///     <para>Connect and then run <paramref name="query" />.</para>
	/// </summary>
	/// <param name="query"></param>
	/// <param name="cancellationToken"></param>
	/// <param name="parameters"></param>
	/// <exception cref="NullException"></exception>
	/// <exception cref="InvalidOperationException"></exception>
	public async Task<SqlDataReader?> QueryAsync( String query, CancellationToken cancellationToken, params SqlParameter[]? parameters ) {
		if ( String.IsNullOrWhiteSpace( query ) ) {
			throw new NullException( nameof( query ) );
		}

		this.Query = query;

		TryAgain:

		try {
			await this.PopulateCommandAndOpenConnection( CommandType.StoredProcedure, cancellationToken, parameters ).ConfigureAwait( false );

			return await this.Command.ExecuteReaderAsync( cancellationToken ).ConfigureAwait( false );
		}
		catch ( InvalidCastException exception ) {
			//TIP: check for SQLServer returning a Double when you expect a Single (float in SQL).
			exception.Log();

			throw;
		}
		catch ( Exception exception ) {
			if ( exception.AttemptQueryAgain( ref this.retriesLeft ) ) {
				this.CloseConnection();
				await Task.Delay( this.DefaultTimeBetweenRetries, cancellationToken ).ConfigureAwait( false );
				goto TryAgain;
			}

			exception.Log();
		}

		return default( SqlDataReader? );
	}

	/// <summary>
	///     Execute the stored procedure " <paramref name="query" />" with the optional parameters
	///     <paramref name="parameters" />.
	/// </summary>
	/// <param name="query"></param>
	/// <param name="cancellationToken"></param>
	/// <param name="parameters"></param>
	/// <exception cref="InvalidOperationException"></exception>
	/// <exception cref="SqlException"></exception>
	/// <exception cref="DbException"></exception>
	public async Task<Int32?> RunStoredProcedureAsync( String query, CancellationToken cancellationToken, params SqlParameter[]? parameters ) {
		this.Query = query;

		TryAgain:

		try {
			await this.PopulateCommandAndOpenConnection( CommandType.StoredProcedure, cancellationToken, parameters ).ConfigureAwait( false );

			return await this.Command.ExecuteNonQueryAsync( cancellationToken )!.ConfigureAwait( false );
		}
		catch ( Exception exception ) {
			if ( exception.AttemptQueryAgain( ref this.retriesLeft ) ) {
				this.CloseConnection();
				await Task.Delay( this.DefaultTimeBetweenRetries, cancellationToken ).ConfigureAwait( false );
				goto TryAgain;
			}

			exception.Log();
		}

		return default( Int32? );
	}

	/// <summary>Defaults to 10 attempts.</summary>
	public void SetRetriesLeft( Int32 value ) {
		this._retriesLeft = value;
	}

}