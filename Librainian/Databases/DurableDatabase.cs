// Copyright © Protiguous. All Rights Reserved.
// This entire copyright notice and license must be retained and must be kept visible in any binaries, libraries, repositories, or source code (directly or derived) from our binaries, libraries, projects, solutions, or applications.
// All source code belongs to Protiguous@Protiguous.com unless otherwise specified or the original license has been overwritten by formatting. (We try to avoid it from happening, but it does accidentally happen.)
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
// File "DurableDatabase.cs" last formatted on 2020-09-11 at 12:34 PM.

#nullable enable

namespace Librainian.Databases {
	using System;
	using System.Collections.Generic;
	using System.Data;
	using System.Data.Common;
	using System.Threading;
	using System.Threading.Tasks;
	using Extensions;
	using JetBrains.Annotations;
	using Logging;
	using Maths;
	using Microsoft.Data.SqlClient;
	using Utilities;

	public class DurableDatabase : ABetterClassDispose {

		/// <summary>A database connection attempts to stay connected in the event of an unwanted disconnect.</summary>
		/// <param name="connectionString"></param>
		/// <param name="retries">         </param>
		/// <exception cref="InvalidOperationException"></exception>
		/// <remarks>This has not been tested if it makes a noticable difference versus SQL Server connection pooling.</remarks>
		public DurableDatabase( [NotNull] String connectionString, UInt16 retries ) {
			if ( String.IsNullOrWhiteSpace( connectionString ) ) {
				throw new ArgumentException( "Value cannot be null or whitespace.", nameof( connectionString ) );
			}

			this.Retries = retries;
			this.ConnectionString = connectionString;

			this.SqlConnections = new ThreadLocal<SqlConnection>( () => {
				var connection = new SqlConnection( this.ConnectionString ) {StateChange += this.SqlConnection_StateChange};

				return connection;
			}, true );

			var test = this.OpenConnection(); //try/start the current thread's open;

			if ( null == test ) {
				var builder = new SqlConnectionStringBuilder( this.ConnectionString );

				throw new InvalidOperationException( $"Unable to connect to {builder.DataSource}" );
			}
		}

		[NotNull] private String ConnectionString { get; }

		private UInt16 Retries { get; }

		[NotNull] private ThreadLocal<SqlConnection> SqlConnections { get; }

		public CancellationTokenSource CancelConnection { get; } = new CancellationTokenSource();

		[CanBeNull]
		private SqlConnection? OpenConnection() {
			if ( this.SqlConnections.Value.State == ConnectionState.Open ) {
				return this.SqlConnections.Value;
			}

			try {
				this.SqlConnections.Value.Open();

				return this.SqlConnections.Value;
			}
			catch ( Exception exception ) {
				exception.Log();
			}

			return default;
		}

		/// <summary>Return true if connected.</summary>
		/// <param name="sender"></param>
		/// <returns></returns>
		private Boolean ReOpenConnection( [CanBeNull]
			Object? sender ) {
			if ( this.CancelConnection.IsCancellationRequested ) {
				return default;
			}

			if ( !( sender is SqlConnection connection ) ) {
				return default;
			}

			var retries = this.Retries;

			do {
				retries--;

				try {
					if ( this.CancelConnection.IsCancellationRequested ) {
						return default;
					}

					connection.Open();

					if ( connection.State == ConnectionState.Open ) {
						return true;
					}
				}
				catch ( SqlException exception ) {
					exception.Log();
				}
				catch ( DbException exception ) {
					exception.Log();
				}
			} while ( retries > 0 );

			return default;
		}

		private void SqlConnection_StateChange( [CanBeNull]
			Object? sender, [NotNull] StateChangeEventArgs e ) {
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
			if ( !this.CancelConnection.IsCancellationRequested ) {
				this.CancelConnection.Cancel();
			}

			foreach ( var connection in this.SqlConnections.Values ) {
				if ( connection == null ) {
					throw new InvalidOperationException( $"{nameof( connection )} is null." );
				}

				// ReSharper disable once SwitchStatementHandlesSomeKnownEnumValuesWithDefault
				switch ( connection.State ) {
					case ConnectionState.Open: {
						connection.Close();

						break;
					}

					case ConnectionState.Closed: {
						break;
					}

					case ConnectionState.Connecting: {
						connection.Close();

						break;
					}

					case ConnectionState.Executing: {
						connection.Close();

						break;
					}

					case ConnectionState.Fetching: {
						connection.Close();

						break;
					}

					case ConnectionState.Broken: {
						connection.Close();

						break;
					}
				}
			}
		}

		/// <summary>Opens and then closes a <see cref="SqlConnection" />.</summary>
		/// <returns></returns>
		public Int32? ExecuteNonQuery( [NotNull] String query, [CanBeNull]
			params SqlParameter[]? parameters ) {
			if ( String.IsNullOrWhiteSpace( query ) ) {
				throw new ArgumentNullException( nameof( query ) );
			}

			try {
				using var command = new SqlCommand( query, this.OpenConnection() ) {
					CommandType = CommandType.Text
				};

				if ( null != parameters ) {
					command.Parameters?.AddRange( parameters );
				}

				return command.ExecuteNonQuery();
			}
			catch ( SqlException exception ) {
				exception.Log();
			}
			catch ( DbException exception ) {
				exception.Log();
			}
			catch ( Exception exception ) {
				exception.Log();
			}

			return default;
		}

		public Int32? ExecuteNonQuery( [NotNull] String query, Int32 retries, [CanBeNull]
			params SqlParameter[]? parameters ) {
			if ( String.IsNullOrWhiteSpace( query ) ) {
				throw new ArgumentNullException( nameof( query ) );
			}

			TryAgain:

			try {
				using var command = new SqlCommand( query, this.OpenConnection() ) {
					CommandType = CommandType.StoredProcedure
				};

				if ( null != parameters ) {
					command.Parameters?.AddRange( parameters );
				}

				return command.ExecuteNonQuery();
			}
			catch ( InvalidOperationException ) {
				//timeout probably
				retries--;

				if ( retries.Any() ) {
					goto TryAgain;
				}
			}
			catch ( SqlException exception ) {
				exception.Log();
			}
			catch ( DbException exception ) {
				exception.Log();
			}

			return default;
		}

		/// <summary></summary>
		/// <returns></returns>
		public Boolean ExecuteNonQuery( [NotNull] String query ) {
			if ( String.IsNullOrWhiteSpace( query ) ) {
				throw new ArgumentNullException( nameof( query ) );
			}

			try {
				using var sqlcommand = new SqlCommand( query, this.OpenConnection() ) {
					CommandType = CommandType.Text
				};

				sqlcommand.ExecuteNonQuery();

				return true;
			}
			catch ( SqlException exception ) {
				exception.Log();
			}
			catch ( DbException exception ) {
				exception.Log();
			}
			catch ( Exception exception ) {
				exception.Log();
			}

			return default;
		}

		[ItemCanBeNull]
		public async Task<Int32?> ExecuteNonQueryAsync( [NotNull] String query, CommandType commandType, [CanBeNull]
			params SqlParameter[]? parameters ) {
			if ( String.IsNullOrWhiteSpace( query ) ) {
				throw new ArgumentNullException( nameof( query ) );
			}

			try {
#if !NET48
				await
#endif
					using var command = new SqlCommand( query, this.OpenConnection() ) {
						CommandType = commandType
					};

				if ( null != parameters ) {
					command.Parameters?.AddRange( parameters );
				}

				return await command.ExecuteNonQueryAsync().ConfigureAwait( false );
			}
			catch ( SqlException exception ) {
				exception.Log();
			}
			catch ( DbException exception ) {
				exception.Log();
			}

			return default;
		}

		/// <summary>Returns a <see cref="DataTable" /></summary>
		/// <param name="query">      </param>
		/// <param name="commandType"></param>
		/// <param name="table">      </param>
		/// <param name="parameters"> </param>
		/// <returns></returns>
		public Boolean ExecuteReader( [NotNull] String query, CommandType commandType, [NotNull] out DataTable table, [CanBeNull]
			params SqlParameter[]? parameters ) {
			if ( String.IsNullOrWhiteSpace( query ) ) {
				throw new ArgumentNullException( nameof( query ) );
			}

			table = new DataTable();

			try {
				using var command = new SqlCommand( query, this.OpenConnection() ) {
					CommandType = commandType
				};

				if ( null != parameters ) {
					command.Parameters?.AddRange( parameters );
				}

				table.BeginLoadData();

				using ( var reader = command.ExecuteReader() ) {
					table.Load( reader );
				}

				table.EndLoadData();

				return true;
			}
			catch ( SqlException exception ) {
				exception.Log();
			}
			catch ( DbException exception ) {
				exception.Log();
			}
			catch ( Exception exception ) {
				exception.Log();
			}

			return default;
		}

		/// <summary>Returns a <see cref="DataTable" /></summary>
		/// <param name="query">      </param>
		/// <param name="commandType"></param>
		/// <param name="parameters"> </param>
		/// <returns></returns>
		[NotNull]
		public DataTable ExecuteReader( [NotNull] String query, CommandType commandType, [CanBeNull]
			params SqlParameter[]? parameters ) {
			if ( String.IsNullOrWhiteSpace( query ) ) {
				throw new ArgumentNullException( nameof( query ) );
			}

			var table = new DataTable();

			try {
				using var command = new SqlCommand( query, this.OpenConnection() ) {
					CommandType = commandType
				};

				if ( null != parameters ) {
					command.Parameters?.AddRange( parameters );
				}

				table.BeginLoadData();

				using ( var reader = command.ExecuteReader() ) {
					table.Load( reader );
				}

				table.EndLoadData();
			}
			catch ( SqlException exception ) {
				exception.Log();
			}
			catch ( DbException exception ) {
				exception.Log();
			}
			catch ( Exception exception ) {
				exception.Log();
			}

			return table;
		}

		/// <summary></summary>
		/// <param name="query">      </param>
		/// <param name="commandType"></param>
		/// <param name="parameters"> </param>
		/// <returns></returns>
		[ItemCanBeNull]
		public async Task<DataTableReader> ExecuteReaderAsyncDataReader( [CanBeNull]
			String? query, CommandType commandType, [CanBeNull]
			params SqlParameter[]? parameters ) {
			if ( String.IsNullOrWhiteSpace( query ) ) {
				throw new ArgumentNullException( nameof( query ) );
			}

			try {
				DataTable table;

#if !NET48
				await
#endif
					using ( var command = new SqlCommand( query, this.OpenConnection() ) {
						CommandType = commandType
					} ) {
					if ( null != parameters ) {
						command.Parameters?.AddRange( parameters );
					}

#if !NET48
					await
#endif
						using var reader = await command.ExecuteReaderAsync().ConfigureAwait( false );
					table = reader.ToDataTable();
				}

				return table.CreateDataReader();
			}
			catch ( SqlException exception ) {
				exception.Log();
			}

			return default;
		}

		/// <summary>Returns a <see cref="DataTable" /></summary>
		/// <param name="query">      </param>
		/// <param name="commandType"></param>
		/// <param name="parameters"> </param>
		/// <returns></returns>
		[ItemNotNull]
		public async Task<DataTable> ExecuteReaderDataTableAsync( [NotNull] String query, CommandType commandType, [CanBeNull]
			params SqlParameter[]? parameters ) {
			var table = new DataTable();

			try {
#if !NET48
				await
#endif
					using var command = new SqlCommand( query, this.OpenConnection() ) {
						CommandType = commandType
					};

				if ( null != parameters ) {
					command.Parameters?.AddRange( parameters );
				}

				table.BeginLoadData();

#if !NET48
				await
#endif
					using ( var reader = await command.ExecuteReaderAsync( this.CancelConnection.Token ).ConfigureAwait( false ) ) {
					table.Load( reader );
				}

				table.EndLoadData();
			}
			catch ( SqlException exception ) {
				exception.Log();
			}
			catch ( DbException exception ) {
				exception.Log();
			}
			catch ( Exception exception ) {
				exception.Log();
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
		public (Status status, TResult result) ExecuteScalar<TResult>( [NotNull] String query, CommandType commandType, [CanBeNull]
			params SqlParameter[]? parameters ) {
			try {
				using var command = new SqlCommand( query, this.OpenConnection() ) {
					CommandType = commandType
				};

				if ( null != parameters ) {
					command.Parameters?.AddRange( parameters );
				}

				var scalar = command.ExecuteScalar();

				if ( null == scalar || scalar == DBNull.Value || Convert.IsDBNull( scalar ) ) {
					return ( Status.Success, default )!;
				}

				if ( scalar is TResult result1 ) {
					return ( Status.Success, result1 );
				}

				if ( scalar.TryCast<TResult>( out var result ) ) {
					return ( Status.Success, result );
				}

				return ( Status.Success, ( TResult )Convert.ChangeType( scalar, typeof( TResult ) ) );
			}
			catch ( SqlException exception ) {
				exception.Log();
			}
			catch ( DbException exception ) {
				exception.Log();
			}

			return default;
		}

		/// <summary>
		///     <para>Returns the first column of the first row.</para>
		/// </summary>
		/// <param name="query">      </param>
		/// <param name="commandType"></param>
		/// <param name="parameters"> </param>
		/// <returns></returns>
		public async Task<(Status status, TResult result)> ExecuteScalarAsync<TResult>( [NotNull] String query, CommandType commandType, [CanBeNull]
			params SqlParameter[]? parameters ) {
			if ( String.IsNullOrWhiteSpace( query ) ) {
				throw new ArgumentNullException( nameof( query ) );
			}

			try {
#if !NET48
				await
#endif
					using var command = new SqlCommand( query, this.OpenConnection() ) {
						CommandType = commandType, CommandTimeout = 0
					};

				if ( null != parameters ) {
					command.Parameters?.AddRange( parameters );
				}

				TryAgain:
				Object scalar;

				try {
					scalar = await command.ExecuteScalarAsync().ConfigureAwait( false );
				}
				catch ( SqlException exception ) {
					if ( exception.Number == DatabaseErrors.Deadlock ) {
						goto TryAgain;
					}

					throw;
				}

				if ( null == scalar || scalar == DBNull.Value || Convert.IsDBNull( scalar ) ) {
					return ( Status.Success, default )!;
				}

				if ( scalar is TResult scalarAsync ) {
					return ( Status.Success, scalarAsync );
				}

				if ( scalar.TryCast<TResult>( out var result ) ) {
					return ( Status.Success, result );
				}

				return ( Status.Success, ( TResult )Convert.ChangeType( scalar, typeof( TResult ) ) );
			}
			catch ( InvalidCastException exception ) {
				//TIP: check for SQLServer returning a Double when you expect a Single (float in SQL).
				exception.Log();
			}
			catch ( SqlException exception ) {
				exception.Log();
			}

			return default;
		}

		/// <summary>Returns a <see cref="DataTable" /></summary>
		/// <param name="query">     </param>
		/// <param name="parameters"></param>
		/// <returns></returns>
		[CanBeNull]
		[ItemCanBeNull]
		public IEnumerable<TResult> QueryList<TResult>( [NotNull] String query, [CanBeNull]
			params SqlParameter[]? parameters ) {
			try {
				using var command = new SqlCommand( query, this.OpenConnection() ) {
					CommandType = CommandType.StoredProcedure
				};

				if ( null != parameters ) {
					command.Parameters?.AddRange( parameters );
				}

				using var reader = command.ExecuteReader();

				var data = GenericPopulatorExtensions.CreateList<TResult>( reader );

				return data;
			}
			catch ( SqlException exception ) {
				exception.Log();
			}
			catch ( DbException exception ) {
				exception.Log();
			}
			catch ( Exception exception ) {
				exception.Log();
			}

			return default;
		}

	}
}