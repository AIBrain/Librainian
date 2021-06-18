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
// File "DatabaseServer.cs" last touched on 2021-04-25 at 10:07 PM by Protiguous.

#nullable enable

namespace Librainian.Databases {

	using System;
	using System.Collections.Generic;
	using System.Data;
	using System.Data.Common;
	using System.Data.SqlTypes;
	using System.Diagnostics;
	using System.IO;
	using System.Linq;
	using System.Threading;
	using System.Threading.Tasks;
	using System.Xml;
	using Converters;
	using Exceptions;
	using Internet;
	using JetBrains.Annotations;
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

	public class DatabaseServer : ABetterClassDisposeReactive, IDatabase {

		public const Int32 DefaultRetries = 5;

		/// <summary>
		/// </summary>
		/// <param name="connectionString"> </param>
		/// <param name="useDatabase">      </param>
		/// <param name="openCancellationToken"></param>
		/// <param name="executeCancellationToken"></param>
		/// <exception cref="InvalidOperationException"></exception>
		public DatabaseServer(
			String connectionString,
			String? useDatabase,
			CancellationToken openCancellationToken,
			CancellationToken executeCancellationToken
		) {
			$"New {nameof( DatabaseServer )} on thread {Thread.CurrentThread.ManagedThreadId}.".Verbose();

			this._openCancellationToken = openCancellationToken;
			this._executeCancellationToken = executeCancellationToken;

			connectionString = connectionString.Trimmed() ?? throw new ArgumentEmptyException( nameof( connectionString ) );
			useDatabase = useDatabase.Trimmed();

			var builder = new SqlConnectionStringBuilder( connectionString ) {
				Pooling = true, IntegratedSecurity = true, MaxPoolSize = 32767 //wild guess
			};

			if ( !String.IsNullOrEmpty( useDatabase ) ) {
				builder.InitialCatalog = useDatabase;
			}

			this._connectionString = builder.ConnectionString;

			Debug.Assert( !String.IsNullOrWhiteSpace( this._connectionString ) );
		}

		/// <summary>
		///     The number of sql connections open across ALL threads.
		/// </summary>
		private Int64 ConnectionCounter { get; set; }

		private String? _connectionString { get; }

		private CancellationToken _openCancellationToken { get; }

		private CancellationToken _executeCancellationToken { get; }

		private Stopwatch TimeSinceLastConnectAttempt { get; } = Stopwatch.StartNew();

		/// <summary>
		///     Set to 1 minute by default.
		/// </summary>
		public TimeSpan CommandTimeout { get; set; } = Minutes.One;

		public String? Query { get; set; }

		public Int32? ExecuteNonQuery( String query, Int32 retries, CommandType commandType, params SqlParameter?[]? parameters ) {
			if ( String.IsNullOrWhiteSpace( query ) ) {
				throw new ArgumentException( "Value cannot be null or whitespace.", nameof( query ) );
			}

			this.Query = query;
			var connection = default( SqlConnection? );

			TryAgain:
			try {
				connection = this.OpenConnection();
				if ( connection is not null ) {
					using var command = new SqlCommand( query, connection ) {
						CommandTimeout = ( Int32 )this.CommandTimeout.TotalSeconds, CommandType = commandType
					};

					_ = command.PopulateParameters( parameters );

					IHateDeadlocks:
					try {
						return command.ExecuteNonQuery();
					}
					catch ( SqlException exception ) {
						if ( exception.Message.Contains( "deadlocked", StringComparison.OrdinalIgnoreCase ) ) {
							if ( exception.Message.Contains( "Rerun the transaction", StringComparison.OrdinalIgnoreCase ) ) {
								$"Query {this.Query.DoubleQuote()} deadlocked. Rerunning..".DebugLine();
								Thread.Yield();
								goto IHateDeadlocks;
							}
						}
					}
				}
			}
			
			catch ( Exception exception ) {
				if ( exception.AttemptQueryAgain() && retries.Any() ) {
					--retries;
					this.CloseConnection( ref connection );
					Thread.Sleep( Seconds.One );
					goto TryAgain;
				}

				exception.Log( Rebuild( query, parameters ) );
			}
			finally {
				this.CloseConnection( ref connection );
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
		public Int32? ExecuteNonQuery( String query, CommandType commandType, params SqlParameter?[]? parameters ) =>
			this.ExecuteNonQuery( query, 1, commandType, parameters );

		/// <summary>
		/// </summary>
		/// <param name="query">      </param>
		/// <param name="commandType"></param>
		/// <param name="cancellationToken"></param>
		/// <param name="parameters"> </param>
		/// <returns></returns>
		/// <exception cref="InvalidOperationException"></exception>
		/// <exception cref="SqlException"></exception>
		/// <exception cref="DbException"></exception>
		public async PooledValueTask<Int32?> ExecuteNonQueryAsync(
			String query,
			CommandType commandType,
			CancellationToken cancellationToken,
			params SqlParameter?[]? parameters
		) {
			this.Query = query;

			var connection = default( SqlConnection? );

			var retries = DefaultRetries;

			TryAgain:
			try {
				connection = await this.OpenConnectionAsync( cancellationToken ).ConfigureAwait( false );

				if ( connection is not null ) {
					await using var command = new SqlCommand( query, connection ) {
						CommandType = commandType, CommandTimeout = ( Int32 )this.CommandTimeout.TotalSeconds
					};

					return await command.PopulateParameters( parameters ).ExecuteNonQueryAsync( this._executeCancellationToken ).ConfigureAwait( false );
				}
			}
			catch ( Exception exception ) {
				if ( exception.AttemptQueryAgain() && retries.Any() ) {
					--retries;
					this.CloseConnection( ref connection );
					Thread.Sleep( Seconds.One );
					goto TryAgain;
				}

				exception.Log( Rebuild( query, parameters ) );
			}
			finally {
				this.CloseAsyncConnection( ref connection );
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
		/// <returns></returns>
		/// <exception cref="InvalidOperationException"></exception>
		/// <exception cref="SqlException"></exception>
		/// <exception cref="DbException"></exception>
		public async PooledValueTask<Int32?> ExecuteNonQueryAsync(
			String query,
			CancellationToken cancellationToken,
			params SqlParameter?[]? parameters
		) =>
			await this.ExecuteNonQueryAsync( query, CommandType.StoredProcedure, cancellationToken, parameters ).ConfigureAwait( false );

		/// <summary>
		///     Execute the stored procedure " <paramref name="query" />" with the optional <paramref name="parameters" />.
		/// </summary>
		/// <param name="query">     </param>
		/// <param name="cancellationToken"></param>
		/// <param name="parameters"></param>
		/// <returns></returns>
		/// <exception cref="InvalidOperationException"></exception>
		/// <exception cref="SqlException"></exception>
		/// <exception cref="DbException"></exception>
		public async PooledValueTask<Int32?> RunSprocAsync( String query, CancellationToken cancellationToken, params SqlParameter?[]? parameters ) =>
			await this.ExecuteNonQueryAsync( query, CommandType.StoredProcedure, cancellationToken, parameters ).ConfigureAwait( false );

		/// <summary>
		///     Returns a <see cref="DataTable" />
		/// </summary>
		/// <param name="query">      </param>
		/// <param name="commandType"></param>
		/// <param name="table">      </param>
		/// <param name="parameters"> </param>
		/// <returns></returns>
		public Boolean ExecuteReader( String query, CommandType commandType, out DataTable table, params SqlParameter?[]? parameters ) {
			if ( String.IsNullOrWhiteSpace( query ) ) {
				throw new ArgumentEmptyException( nameof( query ) );
			}

			this.Query = query;

			var retries = DefaultRetries;

			TryAgain:
			table = new DataTable();

			var connection = default( SqlConnection? );
			try {
				connection = this.OpenConnection();

				if ( connection != null ) {
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
			}
			catch ( Exception exception ) {
				if ( exception.AttemptQueryAgain() && retries.Any() ) {
					--retries;
					this.CloseConnection( ref connection );
					Thread.Sleep( Seconds.One );
					goto TryAgain;
				}

				exception.Log( Rebuild( query, parameters ) );
			}
			finally {
				this.CloseConnection( ref connection );
			}

			return false;
		}

		/// <param name="query">      </param>
		/// <param name="commandType"></param>
		/// <param name="cancellationToken"></param>
		/// <param name="parameters"> </param>
		public async PooledValueTask<DataTableReader?> ExecuteReaderAsync(
			String query,
			CommandType commandType,
			CancellationToken cancellationToken,
			params SqlParameter?[]? parameters
		) {
			if ( String.IsNullOrWhiteSpace( query ) ) {
				throw new ArgumentException( "Value cannot be null or whitespace.", nameof( query ) );
			}

			this.Query = query;

			var retries = DefaultRetries;

			TryAgain:
			var connection = default( SqlConnection? );
			try {
				connection = await this.OpenConnectionAsync( this._openCancellationToken.LinkTo( cancellationToken ) ).ConfigureAwait( false );

				if ( connection != null ) {
					await using var command = new SqlCommand( query, connection ) {
						CommandType = commandType, CommandTimeout = ( Int32 )this.CommandTimeout.TotalSeconds
					};

					await using var readerAsync = await command.PopulateParameters( parameters )
					                                           .ExecuteReaderAsync( this._executeCancellationToken.LinkTo( cancellationToken ) )
					                                           .ConfigureAwait( false );

					using var table = readerAsync.ToDataTable();

					return table.CreateDataReader();
				}
			}
			catch ( Exception exception ) {
				if ( exception.AttemptQueryAgain() && retries.Any() ) {
					--retries;
					this.CloseConnection( ref connection );
					Thread.Sleep( Seconds.One );
					goto TryAgain;
				}

				exception.Log( Rebuild( query, parameters ) );
			}
			finally {
				this.CloseAsyncConnection( ref connection );
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
		/// <returns></returns>
		public async PooledValueTask<DataTable> ExecuteReaderDataTableAsync(
			String query,
			CommandType commandType,
			CancellationToken cancellationToken,
			params SqlParameter?[]? parameters
		) {
			if ( String.IsNullOrWhiteSpace( query ) ) {
				throw new ArgumentException( "Value cannot be null or whitespace.", nameof( query ) );
			}

			this.Query = query;

			var retries = DefaultRetries;

			TryAgain:
			var table = new DataTable();

			var connection = default( SqlConnection? );
			try {
				connection = await this.OpenConnectionAsync( cancellationToken ).ConfigureAwait( false );

				if ( connection != null ) {
					await using var command = new SqlCommand( query, connection ) {
						CommandType = commandType, CommandTimeout = ( Int32 )this.CommandTimeout.TotalSeconds
					};

					using var reader = command.PopulateParameters( parameters ).ExecuteReaderAsync( this._executeCancellationToken.LinkTo( cancellationToken ) );

					if ( reader != null ) {
						table.BeginLoadData();
						table.Load( await reader.ConfigureAwait( false ) );
						table.EndLoadData();
					}
				}
			}
			catch ( Exception exception ) {
				if ( retries.Any() && exception.AttemptQueryAgain() ) {
					--retries;
					this.CloseConnection( ref connection );
					Thread.Sleep( Seconds.One );
					goto TryAgain;
				}

				exception.Log( Rebuild( query, parameters ) );
			}
			finally {
				this.CloseAsyncConnection( ref connection );
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
		public T? ExecuteScalar<T>( String query, CommandType commandType, params SqlParameter[]? parameters ) {
			if ( String.IsNullOrWhiteSpace( query ) ) {
				throw new ArgumentException( "Value cannot be null or whitespace.", nameof( query ) );
			}

			this.Query = query;

			var retries = DefaultRetries;

			TryAgain:
			var connection = default( SqlConnection? );
			try {
				connection = this.OpenConnection();

				if ( connection != null ) {
					using var command = new SqlCommand( query, connection ) {
						CommandTimeout = ( Int32 )this.CommandTimeout.TotalSeconds, CommandType = commandType
					};

					var scalar = command.PopulateParameters( parameters ).ExecuteScalar();

					return scalar is null ? default( T? ) : scalar.Cast<Object, T>();
				}
			}
			catch ( Exception exception ) {
				if ( retries.Any() && exception.AttemptQueryAgain() ) {
					--retries;
					this.CloseConnection( ref connection );
					Thread.Sleep( Seconds.One );
					goto TryAgain;
				}

				exception.Log( Rebuild( query, parameters ) );
			}
			finally {
				this.CloseConnection( ref connection );
			}

			return default( T? );
		}

		/// <summary>
		///     <para>Returns the first column of the first row.</para>
		/// </summary>
		/// <param name="query">      </param>
		/// <param name="commandType"></param>
		/// <param name="cancellationToken"></param>
		/// <param name="parameters"> </param>
		/// <returns></returns>
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
			var connection = default( SqlConnection? );
			try {
				connection = await this.OpenConnectionAsync( cancellationToken ).ConfigureAwait( false );

				if ( connection != null ) {
					await using var command = new SqlCommand( query, connection ) {
						CommandType = commandType, CommandTimeout = ( Int32 )this.CommandTimeout.TotalSeconds
					};

					var task = command.PopulateParameters( parameters ).ExecuteScalarAsync( this._executeCancellationToken.LinkTo( cancellationToken ) );
					if ( task is not null ) {
						var result = await task.ConfigureAwait( false );

						return result is null ? default( T? ) : result.Cast<Object, T>();
					}
				}
			}
			catch ( InvalidCastException exception ) {
				//TIP: check for SQLServer returning a Double when you expect a Single (float in SQL).
				exception.Log( Rebuild( query, parameters ) );

				throw;
			}
			catch ( Exception exception ) {
				if ( retries.Any() && exception.AttemptQueryAgain() ) {
					--retries;
					this.CloseConnection( ref connection );
					Thread.Sleep( Seconds.One );
					goto TryAgain;
				}

				exception.Log( Rebuild( query, parameters ) );
			}
			finally {
				this.CloseAsyncConnection( ref connection );
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
			params SqlParameter?[]? parameters
		) {
			if ( table is null ) {
				throw new ArgumentEmptyException( nameof( table ) );
			}

			this.Query = query;
			var retries = DefaultRetries;

			if ( String.IsNullOrWhiteSpace( query ) ) {
				throw new ArgumentException( "Value cannot be null or whitespace.", nameof( query ) );
			}

			TryAgain:
			table.Clear();

			var connection = default( SqlConnection? );
			try {
				connection = await this.OpenConnectionAsync( cancellationToken ).ConfigureAwait( false );

				if ( connection != null ) {
					using var dataAdapter = new SqlDataAdapter( query, connection ) {
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
			}
			catch ( Exception exception ) {
				if ( retries.Any() && exception.AttemptQueryAgain() ) {
					--retries;
					this.CloseConnection( ref connection );
					Thread.Sleep( Seconds.One );
					goto TryAgain;
				}

				exception.Log( Rebuild( query, parameters ) );
			}
			finally {
				this.CloseAsyncConnection( ref connection );
			}

			return true;
		}

		public DataTableReader QueryAdHoc( String query, params SqlParameter[]? parameters ) {
			if ( String.IsNullOrWhiteSpace( query ) ) {
				throw new ArgumentException( "Value cannot be null or whitespace.", nameof( query ) );
			}

			this.Query = query;
			var retries = DefaultRetries;

			TryAgain:
			var connection = default( SqlConnection? );
			try {
				connection = this.OpenConnection();

				try {
					if ( connection != null ) {
						using var command = new SqlCommand( query, connection ) {
							CommandTimeout = ( Int32 )this.CommandTimeout.TotalSeconds, CommandType = CommandType.Text
						};

						var task = command.PopulateParameters( parameters ).ExecuteReader();
						if ( task is not null ) {
							using var table = task.ToDataTable();
							return table.CreateDataReader();
						}
					}
				}
				finally {
					this.CloseConnection( ref connection );
				}
			}
			catch ( Exception exception ) {
				if ( retries.Any() && exception.AttemptQueryAgain() ) {
					--retries;
					this.CloseConnection( ref connection );
					Thread.Sleep( Seconds.One );
					goto TryAgain;
				}

				exception.Log( Rebuild( query, parameters ) );
			}

			return default( DataTableReader? );
		}

		public async PooledValueTask<DataTableReader?> QueryAdhocReaderAsync(
			String query,
			CancellationToken cancellationToken,
			params SqlParameter[]? parameters
		) {
			if ( String.IsNullOrWhiteSpace( query ) ) {
				throw new ArgumentException( "Value cannot be null or whitespace.", nameof( query ) );
			}

			this.Query = query;
			var retries = DefaultRetries;

			TryAgain:
			var connection = default( SqlConnection? );
			try {
				connection = await this.OpenConnectionAsync( cancellationToken ).ConfigureAwait( false );

				if ( connection != null ) {
					await using var command = new SqlCommand( query, connection ) {
						CommandTimeout = ( Int32 )this.CommandTimeout.TotalSeconds, CommandType = CommandType.Text
					};

					var task = command.PopulateParameters( parameters ).ExecuteReaderAsync( this._executeCancellationToken.LinkTo( cancellationToken ) );
					if ( task is not null ) {
						await using var reader = await task.ConfigureAwait( false );

						using var table = reader.ToDataTable();

						return table.CreateDataReader();
					}
				}
			}
			catch ( Exception exception ) {
				if ( retries.Any() && exception.AttemptQueryAgain() ) {
					--retries;
					this.CloseConnection( ref connection );
					await Task.Delay( Seconds.One, cancellationToken ).ConfigureAwait( false );

					goto TryAgain;
				}

				exception.Log( Rebuild( query, parameters ) );
			}
			finally {
				this.CloseAsyncConnection( ref connection );
			}

			return default( DataTableReader? );
		}

		public async PooledValueTask<DatabaseServer> QueryAdhocAsync(
			String query,
			CancellationToken cancellationToken,
			params SqlParameter?[]? parameters
		) {
			if ( String.IsNullOrWhiteSpace( query ) ) {
				throw new ArgumentException( "Value cannot be null or whitespace.", nameof( query ) );
			}

			this.Query = query;
			var retries = DefaultRetries;

			TryAgain:
			var connection = default( SqlConnection? );
			try {
				connection = await this.OpenConnectionAsync( cancellationToken ).ConfigureAwait( false );

				if ( connection != null ) {
					await using var command = new SqlCommand( query, connection ) {
						CommandTimeout = ( Int32 )this.CommandTimeout.TotalSeconds, CommandType = CommandType.Text
					};

					var task = command.PopulateParameters( parameters ).ExecuteNonQueryAsync( this._executeCancellationToken.LinkTo( cancellationToken ) );
					if ( task is not null ) {
						await task.ConfigureAwait( false );
					}
				}
			}
			catch ( Exception exception ) {
				if ( retries.Any() && exception.AttemptQueryAgain() ) {
					--retries;
					this.CloseConnection( ref connection );
					await Task.Delay( Seconds.One, cancellationToken ).ConfigureAwait( false );

					goto TryAgain;
				}

				exception.Log( Rebuild( query, parameters ) );
			}
			finally {
				this.CloseAsyncConnection( ref connection );
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
			params SqlParameter?[]? parameters
		) {
			if ( String.IsNullOrWhiteSpace( query ) ) {
				throw new ArgumentException( "Value cannot be null or whitespace.", nameof( query ) );
			}

			this.Query = query;
			var retries = DefaultRetries;

			TryAgain:

			var connection = default( SqlConnection? );
			try {
				connection = await this.OpenConnectionAsync( cancellationToken ).ConfigureAwait( false );

				if ( connection != null ) {
					await using var command = new SqlCommand( query, connection ) {
						CommandType = commandType, CommandTimeout = ( Int32 )this.CommandTimeout.TotalSeconds
					};

					var task = command.PopulateParameters( parameters ).ExecuteReaderAsync( this._executeCancellationToken.LinkTo( cancellationToken ) );
					if ( task is not null ) {
						return await task.ConfigureAwait( false );
					}
				}
			}
			catch ( Exception exception ) {
				if ( retries.Any() && exception.AttemptQueryAgain() ) {
					--retries;
					this.CloseConnection( ref connection );
					await Task.Delay( Seconds.One, cancellationToken ).ConfigureAwait( false );

					goto TryAgain;
				}

				exception.Log( Rebuild( query, parameters ) );
			}
			finally {
				this.CloseAsyncConnection( ref connection );
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
		/// <exception cref="ArgumentException"></exception>
		/// <exception cref="InvalidOperationException"></exception>
		public async PooledValueTask<SqlDataReader?> QueryAsync(
			String query,
			CancellationToken cancellationToken,
			params SqlParameter?[]? parameters
		) {
			if ( String.IsNullOrWhiteSpace( query ) ) {
				throw new ArgumentException( "Value cannot be null or whitespace.", nameof( query ) );
			}

			this.Query = query;
			var retries = DefaultRetries;

			TryAgain:
			var connection = default( SqlConnection? );
			try {
				connection = await this.OpenConnectionAsync( cancellationToken ).ConfigureAwait( false );

				if ( connection != null ) {
					await using var command = new SqlCommand( query, connection ) {
						CommandType = CommandType.StoredProcedure, CommandTimeout = ( Int32 )this.CommandTimeout.TotalSeconds
					};

					return await command.PopulateParameters( parameters )
					                    .ExecuteReaderAsync( this._executeCancellationToken.LinkTo( cancellationToken ) )
					                    .ConfigureAwait( false );
				}
			}
			catch ( InvalidCastException exception ) {
				//TIP: check for SQLServer returning a Double when you expect a Single (float in SQL).
				exception.Log( Rebuild( query, parameters ) );

				throw;
			}
			catch ( Exception exception ) {
				if ( retries.Any() && exception.AttemptQueryAgain() ) {
					--retries;
					this.CloseConnection( ref connection );
					await Task.Delay( Seconds.One, cancellationToken ).ConfigureAwait( false );

					goto TryAgain;
				}

				exception.Log( Rebuild( query, parameters ) );
			}
			finally {
				this.CloseAsyncConnection( ref connection );
			}

			return default( SqlDataReader? );
		}

		/// <summary>
		///     Returns a <see cref="DataTable" />
		/// </summary>
		/// <param name="query">      </param>
		/// <param name="commandType"></param>
		/// <param name="parameters"> </param>
		/// <returns></returns>
		public IEnumerable<TResult?>? QueryList<TResult>( String query, CommandType commandType, params SqlParameter[]? parameters ) {
			if ( String.IsNullOrWhiteSpace( query ) ) {
				throw new ArgumentEmptyException( nameof( query ) );
			}

			this.Query = query;
			var retries = DefaultRetries;

			TryAgain:
			var connection = default( SqlConnection? );
			try {
				connection = this.OpenConnection();

				if ( connection != null ) {
					using var command = new SqlCommand( query, connection ) {
						CommandType = commandType, CommandTimeout = ( Int32 )this.CommandTimeout.TotalSeconds
					};

					var reader = command.PopulateParameters( parameters ).ExecuteReader();

					if ( reader != null ) {
						return GenericPopulatorExtensions.CreateList<TResult>( reader );
					}
				}
			}
			catch ( Exception exception ) {
				if ( retries.Any() && exception.AttemptQueryAgain() ) {
					--retries;
					this.CloseConnection( ref connection );
					Thread.Sleep( Seconds.One );

					goto TryAgain;
				}

				exception.Log( Rebuild( query, parameters ) );
			}
			finally {
				this.CloseConnection( ref connection );
			}

			return default( IEnumerable<TResult>? );
		}

		public void UseDatabase( String databaseName ) {
			if ( String.IsNullOrWhiteSpace( databaseName ) ) {
				throw new ArgumentException( "Value cannot be null or whitespace.", nameof( databaseName ) );
			}

			using var _ = this.QueryAdHoc( $"USE {databaseName.SmartBraces()};" );
		}

		public async PooledValueTask UseDatabaseAsync( String databaseName, CancellationToken cancellationToken ) {
			if ( String.IsNullOrWhiteSpace( databaseName ) ) {
				throw new ArgumentException( "Value cannot be null or whitespace.", nameof( databaseName ) );
			}

			await using var _ = await this.QueryAdhocReaderAsync( $"USE {databaseName.SmartBraces()};", cancellationToken ).ConfigureAwait( false );
		}

		/// <summary>
		///     Execute the stored procedure " <paramref name="query" />" with the optional <paramref name="parameters" />.
		/// </summary>
		/// <param name="query">     </param>
		/// <param name="cancellationToken"></param>
		/// <param name="parameters"></param>
		/// <returns></returns>
		/// <exception cref="InvalidOperationException"></exception>
		/// <exception cref="SqlException"></exception>
		/// <exception cref="DbException"></exception>
		public async FireAndForget FireOffQuery( String query, CancellationToken cancellationToken, params SqlParameter?[]? parameters ) {
			await this.ExecuteNonQueryAsync( query, CommandType.StoredProcedure, cancellationToken, parameters ).ConfigureAwait(false);
		}

		/// <summary>
		/// A debugging aid. EACH database call will delay upon opening a connection.
		/// </summary>
		public static IQuantityOfTime? DatabaseDelay { get; set; } //= Milliseconds.FiveHundred;

		/// <summary>
		///     Create a sql server database connection, and then call OpenAsync and store it in an AynsLocal
		/// </summary>
		/// <returns></returns>
		private async Task<SqlConnection?> OpenConnectionAsync( CancellationToken cancellationToken, IProgress<(TimeSpan Elapsed, ConnectionState State)>? progress = null ) {
			Debug.Assert( !String.IsNullOrWhiteSpace( this._connectionString ) );

			var connection = new SqlConnection( this._connectionString );

			var retriesleft = DefaultRetries;

			TryAgain:

			try {
				if ( DatabaseDelay is Seconds delay ) {
					await Task.Delay( delay, cancellationToken ).ConfigureAwait( false );
				}

				FluentTimer? timer = null;

				if ( progress is not null ) {
					var stopwatch = Stopwatch.StartNew();
					timer = Fps.Thirty.CreateTimer( () => progress.Report( ( stopwatch.Elapsed, connection.State ) ) );
				}

				try {
					this.TimeSinceLastConnectAttempt.Restart();
					await connection.OpenAsync( this._openCancellationToken.LinkTo( cancellationToken ) )!.ConfigureAwait( false );
				}
				catch ( InvalidOperationException exception ) {
					if ( exception.AttemptQueryAgain() ) {
						await Task.Delay( Seconds.One, cancellationToken ).ConfigureAwait( false );
						goto TryAgain;
					}

					exception.Log();
				}
				catch ( Exception exception ) {
					if ( exception.AttemptQueryAgain() ) {
						await Task.Delay( Seconds.One, cancellationToken ).ConfigureAwait( false );
						goto TryAgain;
					}

					exception.Log();
				}

				using ( timer ) { }
			}
			catch ( InvalidOperationException exception ) {
				if ( retriesleft.Any() ) {
					--retriesleft;
					--this.ConnectionCounter;
					"SQL Pool exhaustion. Trying open connection again in 1 second..".Verbose();
					Thread.Yield();
					await Task.Delay( Seconds.One, this._openCancellationToken ).ConfigureAwait( false );
					goto TryAgain;
				}

				exception.Log( BreakOrDontBreak.Break );
			}
			catch ( SqlException exception ) {
				if ( exception.IsTransient && retriesleft.Any() ) {
					$"Transient {nameof( SqlException )}. Trying open connection again..".Verbose();
					--retriesleft;
					--this.ConnectionCounter;
					goto TryAgain;
				}

				exception.Log( BreakOrDontBreak.Break );
			}
			catch ( DbException exception ) {
				if ( retriesleft.Any() ) {
					if ( exception.IsTransient || exception.AttemptQueryAgain() ) {
						$"Transient {nameof( SqlException )}. Trying open connection again..".Verbose();
						--retriesleft;
						--this.ConnectionCounter;
						goto TryAgain;
					}
				}

				exception.Log( BreakOrDontBreak.Break );
			}
			catch ( TaskCanceledException cancelled ) {
				"Open database connection was cancelled.".Verbose();
				cancelled.Log( BreakOrDontBreak.DontBreak );
			}

			return connection;
		}

		private void ValueChangedHandler( AsyncLocalValueChangedArgs<SqlConnection?> obj ) {
			$"Thread {Thread.CurrentThread.ManagedThreadId}: was {( obj.PreviousValue?.ToString() ).SmartQuote()} to {( obj.CurrentValue?.ToString() ).SmartQuote()}."
				.TraceLine();
		}

		/// <summary>
		///     A count of every SQL connection that has had Open() called per thread.
		///     <para>Each call to Close() will decrement this counter.</para>
		/// </summary>
		/// <returns></returns>
		public Int64 GetConnectionCounter() => this.ConnectionCounter;

		private void CloseAsyncConnection( ref SqlConnection? connection ) {
			if ( connection == null ) {
				new ArgumentEmptyException( nameof( connection ) ).Log( BreakOrDontBreak.Break );
				return;
			}

			if ( this.GetConnectionCounter().Any() ) {
				connection.Close();
				--this.ConnectionCounter;
			}
		}

		/// <summary>
		///     Opens a connection, runs the <paramref name="query" />, and returns the number of rows affected.
		/// </summary>
		/// <param name="query">     </param>
		/// <param name="parameters"></param>
		/// <returns></returns>
		public Int32? ExecuteNonQuery( String query, params SqlParameter?[]? parameters ) =>
			this.ExecuteNonQuery( query, DefaultRetries, CommandType.StoredProcedure, parameters );

		/// <summary>
		///     Execute the stored procedure " <paramref name="query" />" with the optional parameters
		///     <paramref name="parameters" />.
		/// </summary>
		/// <param name="query">     </param>
		/// <param name="parameters"></param>
		/// <returns></returns>
		/// <exception cref="InvalidOperationException"></exception>
		/// <exception cref="SqlException"></exception>
		/// <exception cref="DbException"></exception>
		public Int32? RunStoredProcedure( String query, params SqlParameter?[]? parameters ) {
			this.Query = query;
			var retries = DefaultRetries;

			TryAgain:

			var connection = this.OpenConnection();
			try {
				if ( connection != null ) {
					using var command = new SqlCommand( query, connection ) {
						CommandType = CommandType.StoredProcedure, CommandTimeout = ( Int32 )this.CommandTimeout.TotalSeconds
					};

					return command.PopulateParameters( parameters ).ExecuteNonQuery();
				}
			}
			catch ( Exception exception ) {
				if ( exception.AttemptQueryAgain() && retries.Any() ) {
					--retries;
					this.CloseConnection( ref connection );
					Thread.Sleep( Seconds.One );
					goto TryAgain;
				}

				exception.Log( Rebuild( query, parameters ) );
			}
			finally {
				this.CloseConnection( ref connection );
			}

			return default( Int32? );
		}

		private void CloseConnection( ref SqlConnection? connection ) {
			if ( connection is null ) {
				return;
			}

			using ( connection ) {
				connection.Close();
				--this.ConnectionCounter;
			}
		}

		[Pure]
		private SqlConnection? OpenConnection() {
			Debug.Assert( !String.IsNullOrWhiteSpace( this._connectionString ) );

			var retriesleft = DefaultRetries;

			TryAgain:

			try {
				if ( DatabaseDelay is Seconds delay ) {
					Thread.Sleep( delay );
				}


				//$"Attempting to open sql connection. {Retry} attempts remaining..".Verbose();

				var connection = new SqlConnection( this._connectionString );
				connection.Open();
				++this.ConnectionCounter;

				return connection;
			}
			catch ( InvalidOperationException exception ) {
				if ( retriesleft.Any() ) {
					--retriesleft;
					--this.ConnectionCounter;
					"SQL Pool exhaustion. Trying open connection again in 1 second..".Verbose();
					Thread.Yield();
					Thread.Sleep( Seconds.One );
					goto TryAgain;
				}

				exception.Log( BreakOrDontBreak.Break );
			}
			catch ( SqlException exception ) {
				if ( exception.IsTransient && retriesleft.Any() ) {
					$"Transient {nameof( SqlException )}. Trying open connection again..".Verbose();
					--retriesleft;
					--this.ConnectionCounter;
					goto TryAgain;
				}

				exception.Log( BreakOrDontBreak.Break );
			}
			catch ( DbException exception ) {
				if ( retriesleft.Any() ) {
					if ( exception.IsTransient || exception.AttemptQueryAgain() ) {
						$"Transient {nameof( SqlException )}. Trying open connection again..".Verbose();
						--retriesleft;
						--this.ConnectionCounter;
						goto TryAgain;
					}
				}

				exception.Log( BreakOrDontBreak.Break );
			}

			return default( SqlConnection? );
		}

		/// <summary>
		///     <para>Returns the first column of the first row.</para>
		/// </summary>
		/// <param name="query">     </param>
		/// <param name="cancellationToken"></param>
		/// <param name="parameters"></param>
		/// <returns></returns>
		public async PooledValueTask<T?> ExecuteScalarAsync<T>( String query, CancellationToken cancellationToken, params SqlParameter[]? parameters ) =>
			await this.ExecuteScalarAsync<T?>( query, CommandType.StoredProcedure, cancellationToken, parameters ).ConfigureAwait( false );

		public DataTableReader? ExecuteDataReader( String query, CommandType commandType, params SqlParameter?[]? parameters ) {
			if ( String.IsNullOrWhiteSpace( query ) ) {
				throw new ArgumentException( "Value cannot be null or whitespace.", nameof( query ) );
			}

			this.Query = query;
			var retries = DefaultRetries;

			TryAgain:
			var connection = default( SqlConnection? );
			try {
				connection = this.OpenConnection();

				if ( connection != null ) {
					using var command = new SqlCommand( query, connection ) {
						CommandType = commandType, CommandTimeout = ( Int32 )this.CommandTimeout.TotalSeconds
					};

					var task = command.PopulateParameters( parameters ).ExecuteReader( CommandBehavior.CloseConnection );
					if ( task is not null ) {
						using var table = task.ToDataTable();

						return table.CreateDataReader();
					}
				}
			}
			catch ( Exception exception ) {
				if ( exception.AttemptQueryAgain() && retries.Any() ) {
					--retries;
					this.CloseConnection( ref connection );
					Thread.Sleep( Seconds.One );
					goto TryAgain;
				}

				exception.Log( Rebuild( query, parameters ) );
			}
			finally {
				this.CloseConnection( ref connection );
			}

			return default( DataTableReader? );
		}

#if VERBOSE

		~DatabaseServer() =>
			$"Warning: We have an undisposed DatabaseServer() connection somewhere. This could cause a memory leak. Query={this.Query.DoubleQuote()}".Log(
				BreakOrDontBreak.Break );

#endif

		/// <summary>
		///     Dispose of any <see cref="IDisposable" /> (managed) fields or properties in this method.
		/// </summary>
		public override void DisposeManaged() {
			if ( this.ConnectionCounter.Any() ) {
				$"Warning: We have an unclosed DatabaseServer() connection somewhere. Last Query={this.Query.DoubleQuote()}".Log( BreakOrDontBreak.Break );
			}
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
				using var db = new DatabaseServer( connectionString, "master", new CancellationTokenSource( Seconds.Ten ).Token,
					new CancellationTokenSource( Seconds.Thirty ).Token );

				await db.QueryAdhocReaderAsync( $"create database {databaseName.SmartBraces()};", cancellationToken ).ConfigureAwait( false );

				return true;
			}
			catch ( Exception exception ) {
				if ( exception.AttemptQueryAgain() && retries.Any() ) {
					--retries;
					Thread.Sleep( Seconds.One );
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

			using var connection = this.OpenConnection();

			if ( connection == null ) {
				return;
			}

			using var command = new SqlCommand( query, connection ) {
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

		public String? GetConnectionString() => this._connectionString;

	}

}