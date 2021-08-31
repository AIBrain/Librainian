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
// File "IDatabaseServer.cs" last touched on 2021-08-27 at 10:31 AM by Protiguous.

#nullable enable

namespace Librainian.Databases {

	using System;
	using System.Collections.Generic;
	using System.Data;
	using System.Threading;
	using System.Threading.Tasks;
	using Exceptions;
	using Microsoft.Data.SqlClient;
	using PooledAwait;

	public interface IDatabaseServer {

		TimeSpan CommandTimeout { get; set; }

		String? Query { get; set; }

		Task<Int32?> ExecuteNonQueryAsync( String query, CancellationToken cancellationToken, params SqlParameter[] parameters );

		PooledValueTask<Int32?> ExecuteNonQueryAsync( String query, CommandType commandType, CancellationToken cancellationToken, params SqlParameter[] parameters );

		/// <summary>
		/// </summary>
		/// <param name="query">      </param>
		/// <param name="commandType"></param>
		/// <param name="cancellationToken"></param>
		/// <param name="parameters"> </param>
		Task<DataTableReader?> ExecuteReaderAsync( String query, CommandType commandType, CancellationToken cancellationToken, params SqlParameter[] parameters );

		/// <summary>
		///     Returns a <see cref="DataTable" />
		/// </summary>
		/// <param name="query">      </param>
		/// <param name="commandType"></param>
		/// <param name="cancellationToken"></param>
		/// <param name="parameters"> </param>
		Task<DataTable> ExecuteReaderDataTableAsync( String query, CommandType commandType, CancellationToken cancellationToken, params SqlParameter[] parameters );

		/// <summary>
		///     <para>Returns the first column of the first row.</para>
		/// </summary>
		/// <param name="query">      </param>
		/// <param name="commandType"></param>
		/// <param name="cancellationToken"></param>
		/// <param name="parameters"> </param>
		Task<TResult?> ExecuteScalarAsync<TResult>( String query, CommandType commandType, CancellationToken cancellationToken, params SqlParameter[] parameters );

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
		Task<Boolean> FillTableAsync( String query, CommandType commandType, DataTable table, CancellationToken cancellationToken, IProgress<(TimeSpan Elapsed, ConnectionState State)>? progress = null, params SqlParameter[] parameters );

		Task<DatabaseServer> QueryAdhocAsync( String query, CancellationToken cancellationToken, params SqlParameter?[]? parameters );

		Task<DataTableReader?> QueryAdhocReaderAsync( String query, CancellationToken cancellationToken, params SqlParameter[] parameters );

		/// <summary>
		///     <para>Connect and then run <paramref name="query" />.</para>
		/// </summary>
		/// <param name="query">      </param>
		/// <param name="commandType"></param>
		/// <param name="cancellationToken"></param>
		/// <param name="parameters"> </param>
		/// <exception cref="NullException"></exception>
		/// <exception cref="InvalidOperationException"></exception>
		Task<SqlDataReader?> QueryAsync( String query, CommandType commandType, CancellationToken cancellationToken, params SqlParameter[] parameters );

		/// <summary>
		///     Returns a <see cref="IEnumerable{T}" />
		/// </summary>
		/// <param name="query">      </param>
		/// <param name="commandType"></param>
		/// <param name="cancellationToken"></param>
		/// <param name="parameters"> </param>
		/// <returns></returns>
		Task<IEnumerable<TResult>?> QueryListAsync<TResult>( String query, CommandType commandType, CancellationToken cancellationToken, params SqlParameter[] parameters );

		PooledValueTask<Int32?> RunSprocAsync( String query, CancellationToken cancellationToken, params SqlParameter[] parameters );

		Task UseDatabaseAsync( String databaseName, CancellationToken cancellationToken );
	}
}