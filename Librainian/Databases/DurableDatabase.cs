﻿// Copyright © Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible in any binaries, libraries, repositories,
// or source code (directly or derived) from our binaries, libraries, projects, solutions, or applications.
//
// All source code belongs to Protiguous@Protiguous.com unless otherwise specified or the original license has been overwritten
// by formatting. (We try to avoid it from happening, but it does accidentally happen.)
//
// Any unmodified portions of source code gleaned from other sources still retain their original license and our thanks goes to
// those Authors. If you find your code unattributed in this source code, please let us know so we can properly attribute you
// and include the proper license and/or copyright(s). If you want to use any of our code in a commercial project, you must
// contact Protiguous@Protiguous.com for permission, license, and a quote.
//
// Donations, payments, and royalties are accepted via bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2 and PayPal: Protiguous@Protiguous.com
//
// ====================================================================
// Disclaimer:  Usage of the source code or binaries is AS-IS. No warranties are expressed, implied, or given. We are NOT
// responsible for Anything You Do With Our Code. We are NOT responsible for Anything You Do With Our Executables. We are NOT
// responsible for Anything You Do With Your Computer. ====================================================================
//
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
// For business inquiries, please contact me at Protiguous@Protiguous.com. Our software can be found at
// "https://Protiguous.com/Software/" Our GitHub address is "https://github.com/Protiguous".
//
// File "DurableDatabase.cs" last formatted on 2021-11-30 at 7:16 PM by Protiguous.

#nullable enable

namespace Librainian.Databases;

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using Exceptions;
using Extensions;
using Logging;
using Maths;
using Microsoft.Data.SqlClient;
using Utilities.Disposables;

public class DurableDatabase : ABetterClassDispose {

	/// <summary>A database connection attempts to stay connected in the event of an unwanted disconnect.</summary>
	/// <param name="connectionString"></param>
	/// <param name="retries"></param>
	/// <exception cref="InvalidOperationException"></exception>
	/// <remarks>
	/// This has not been tested if it makes a noticable difference versus SQL Server connection pooling. It probably doesn't
	/// help, as experience shows a database connection should be as short as possible.
	/// </remarks>
	public DurableDatabase( String connectionString, UInt16 retries ) : base( nameof( DurableDatabase ) ) {
		if ( String.IsNullOrWhiteSpace( connectionString ) ) {
			throw new NullException( nameof( connectionString ) );
		}

		this.Retries = retries;
		this.ConnectionString = connectionString;

		this.SqlConnections = new ThreadLocal<SqlConnection>( () => {
			var connection = new SqlConnection( this.ConnectionString );
			connection.StateChange += this.SqlConnection_StateChange;

			return connection;
		}, true );

		var test = this.OpenConnection(); //try/start the current thread's open;

		if ( test == null ) {
			var builder = new SqlConnectionStringBuilder( this.ConnectionString );

			throw new InvalidOperationException( $"Unable to connect to {builder.DataSource}" );
		}
	}

	private String ConnectionString { get; }

	private UInt16 Retries { get; }

	private ThreadLocal<SqlConnection> SqlConnections { get; }

	public CancellationTokenSource CancelConnection { get; } = new();

	private SqlConnection? OpenConnection() {
		var sqlConnectionsValue = this.SqlConnections.Value;
		if ( sqlConnectionsValue!.State == ConnectionState.Open ) {
			return this.SqlConnections.Value;
		}

		try {
			sqlConnectionsValue.Open();

			return sqlConnectionsValue;
		}
		catch ( Exception exception ) {
			exception.Log();
		}

		return default( SqlConnection? );
	}

	/// <summary>Return true if connected.</summary>
	/// <param name="sender"></param>
	private Boolean ReOpenConnection( Object? sender ) {
		if ( this.CancelConnection.IsCancellationRequested ) {
			return false;
		}

		if ( sender is not SqlConnection connection ) {
			return false;
		}

		var retries = this.Retries;

		do {
			retries--;

			try {
				if ( this.CancelConnection.IsCancellationRequested ) {
					return false;
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

		return false;
	}

	private void SqlConnection_StateChange( Object? sender, StateChangeEventArgs e ) {
		switch ( e.CurrentState ) {
			case ConnectionState.Closed:
				this.ReOpenConnection( sender );

				break;

			case ConnectionState.Open:
				break; //do nothing

			case ConnectionState.Connecting:
				Thread.SpinWait( 99 ); //TODO pooa. then test.

				break;

			case ConnectionState.Executing:
				break; //do nothing

			case ConnectionState.Fetching:
				break; //do nothing

			case ConnectionState.Broken:
				this.ReOpenConnection( sender );

				break;

			default:
				throw new ArgumentOutOfRangeException();
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
	public Int32? ExecuteNonQuery( String query, params SqlParameter[]? parameters ) {
		if ( String.IsNullOrWhiteSpace( query ) ) {
			throw new NullException( nameof( query ) );
		}

		try {
			var open = this.OpenConnection();
			if ( open != null ) {
				using var command = new SqlCommand( query, open ) {
					CommandType = CommandType.Text
				};

				if ( parameters != null ) {
					command.Parameters?.AddRange( parameters );
				}

				return command.ExecuteNonQuery();
			}
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

		return default( Int32? );
	}

	public Int32? ExecuteNonQuery( String query, Int32 retries, params SqlParameter[]? parameters ) {
		if ( String.IsNullOrWhiteSpace( query ) ) {
			throw new NullException( nameof( query ) );
		}

		TryAgain:

		try {
			using var command = new SqlCommand( query, this.OpenConnection() ) {
				CommandType = CommandType.StoredProcedure
			};

			if ( parameters != null ) {
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

		return default( Int32? );
	}

	public Boolean ExecuteNonQuery( String query ) {
		if ( String.IsNullOrWhiteSpace( query ) ) {
			throw new NullException( nameof( query ) );
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

		return false;
	}

	public async Task<Int32?> ExecuteNonQueryAsync( String query, CommandType commandType, params SqlParameter[]? parameters ) {
		if ( String.IsNullOrWhiteSpace( query ) ) {
			throw new NullException( nameof( query ) );
		}

		try {
			var command = new SqlCommand( query, this.OpenConnection() ) {
				CommandType = commandType
			};
			await using var asyncDisposable = command.ConfigureAwait( false );

			if ( parameters != null ) {
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

		return default( Int32? );
	}

	/// <summary>Returns a <see cref="DataTable" /></summary>
	/// <param name="query"></param>
	/// <param name="commandType"></param>
	/// <param name="table"></param>
	/// <param name="parameters"></param>
	public Boolean ExecuteReader( String query, CommandType commandType, out DataTable table, params SqlParameter[]? parameters ) {
		if ( String.IsNullOrWhiteSpace( query ) ) {
			throw new NullException( nameof( query ) );
		}

		using DataTable local = new();

		table = local;

		try {
			using var command = new SqlCommand( query, this.OpenConnection() ) {
				CommandType = commandType
			};

			if ( parameters != null ) {
				command.Parameters?.AddRange( parameters );
			}

			table.BeginLoadData();

			using ( var reader = command.ExecuteReader() ) {
				if ( reader != null ) {
					table.Load( reader );
				}
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

		return false;
	}

	/// <summary>Returns a <see cref="DataTable" /></summary>
	/// <param name="query"></param>
	/// <param name="commandType"></param>
	/// <param name="parameters"></param>
	public DataTable ExecuteReader( String query, CommandType commandType, params SqlParameter[]? parameters ) {
		if ( String.IsNullOrWhiteSpace( query ) ) {
			throw new NullException( nameof( query ) );
		}

		var table = new DataTable();

		try {
			using var command = new SqlCommand( query, this.OpenConnection() ) {
				CommandType = commandType
			};

			if ( parameters != null ) {
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

	/// <param name="query"></param>
	/// <param name="commandType"></param>
	/// <param name="parameters"></param>
	public async Task<DataTableReader?> ExecuteReaderAsyncDataReader( String? query, CommandType commandType, params SqlParameter[]? parameters ) {
		if ( String.IsNullOrWhiteSpace( query ) ) {
			throw new NullException( nameof( query ) );
		}

		try {
			DataTable table;

			var command = new SqlCommand( query, this.OpenConnection() ) {
				CommandType = commandType
			};
			await using ( command.ConfigureAwait( false ) ) {
				if ( parameters != null ) {
					command.Parameters?.AddRange( parameters );
				}

				var reader = await command.ExecuteReaderAsync().ConfigureAwait( false );
				await using var _ = reader.ConfigureAwait( false );
				table = reader.ToDataTable();
			}

			return table.CreateDataReader();
		}
		catch ( SqlException exception ) {
			exception.Log();
		}

		return default( DataTableReader );
	}

	/// <summary>Returns a <see cref="DataTable" /></summary>
	/// <param name="query"></param>
	/// <param name="commandType"></param>
	/// <param name="parameters"></param>
	public async Task<DataTable> ExecuteReaderDataTableAsync( String query, CommandType commandType, params SqlParameter[]? parameters ) {
		using var table = new DataTable();

		try {
			var command = new SqlCommand( query, this.OpenConnection() ) {
				CommandType = commandType
			};
			await using var _ = command.ConfigureAwait( false );

			if ( parameters != null ) {
				command.Parameters?.AddRange( parameters );
			}

			table.BeginLoadData();

			var reader = await command.ExecuteReaderAsync( this.CancelConnection.Token ).ConfigureAwait( false );
			await using ( reader.ConfigureAwait( false ) ) {
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
	/// <para>Returns the first column of the first row.</para>
	/// </summary>
	/// <param name="query"></param>
	/// <param name="commandType"></param>
	/// <param name="parameters"></param>
	public (Status status, TResult result) ExecuteScalar<TResult>( String query, CommandType commandType, params SqlParameter[]? parameters ) {
		try {
			using var command = new SqlCommand( query, this.OpenConnection() ) {
				CommandType = commandType
			};

			if ( parameters != null ) {
				command.Parameters?.AddRange( parameters );
			}

			var scalar = command.ExecuteScalar();

			if ( scalar == null || scalar == DBNull.Value || Convert.IsDBNull( scalar ) ) {
				return (Status.Success, default( TResult ));
			}

			if ( scalar is TResult result1 ) {
				return (Status.Success, result1);
			}

			if ( scalar.TryCast<TResult>( out var result ) ) {
				return (Status.Success, result);
			}

			return (Status.Success, ( TResult )Convert.ChangeType( scalar, typeof( TResult ) ));
		}
		catch ( SqlException exception ) {
			exception.Log();
		}
		catch ( DbException exception ) {
			exception.Log();
		}

		return default( (Status status, TResult result) );
	}

	/// <summary>
	/// <para>Returns the first column of the first row.</para>
	/// </summary>
	/// <param name="query"></param>
	/// <param name="commandType"></param>
	/// <param name="parameters"></param>
	public async Task<(Status status, TResult result)> ExecuteScalarAsync<TResult>( String query, CommandType commandType, params SqlParameter[]? parameters ) {
		if ( String.IsNullOrWhiteSpace( query ) ) {
			throw new NullException( nameof( query ) );
		}

		try {
			var command = new SqlCommand( query, this.OpenConnection() ) {
				CommandType = commandType,
				CommandTimeout = 0
			};
			await using var _ = command.ConfigureAwait( false );

			if ( parameters != null ) {
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

			if ( scalar == null || scalar == DBNull.Value || Convert.IsDBNull( scalar ) ) {
				return (Status.Success, default( TResult ));
			}

			if ( scalar is TResult scalarAsync ) {
				return (Status.Success, scalarAsync);
			}

			if ( scalar.TryCast<TResult>( out var result ) ) {
				return (Status.Success, result);
			}

			return (Status.Success, ( TResult )Convert.ChangeType( scalar, typeof( TResult ) ));
		}
		catch ( InvalidCastException exception ) {

			//TIP: check for SQLServer returning a Double when you expect a Single (float in SQL).
			exception.Log();
		}
		catch ( SqlException exception ) {
			exception.Log();
		}

		return default( (Status status, TResult result) );
	}

	/// <summary>Returns a <see cref="DataTable" /></summary>
	/// <param name="query"></param>
	/// <param name="parameters"></param>
	public IEnumerable<TResult?>? QueryList<TResult>( String query, params SqlParameter[]? parameters ) {
		try {
			using var command = new SqlCommand( query, this.OpenConnection() ) {
				CommandType = CommandType.StoredProcedure
			};

			if ( parameters != null ) {
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

		return default( IEnumerable<TResult> );
	}
}