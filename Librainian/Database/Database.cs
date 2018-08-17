// Copyright © Rick@AIBrain.org and Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
//
// This source code contained in "Database.cs" belongs to Protiguous@Protiguous.com and
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
//     paypal@AIBrain.Org
//     (We're still looking into other solutions! Any ideas?)
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
// Feel free to browse any source code we *might* make available.
//
// Project: "Librainian", "Database.cs" was last formatted by Protiguous on 2018/07/10 at 8:58 PM.

namespace Librainian.Database {

	using System;
	using System.Collections.Generic;
	using System.Data;
	using System.Data.Common;
	using System.Data.SqlClient;
	using System.Diagnostics.CodeAnalysis;
	using System.Threading.Tasks;
	using Extensions;
	using JetBrains.Annotations;
	using Magic;
	using Parsing;
	using Threading;

	public sealed class Database : ABetterClassDispose, IDatabase {

		/// <summary>
		///     Opens and then closes a <see cref="SqlConnection" />.
		/// </summary>
		/// <returns></returns>
		public Boolean ExecuteNonQuery( String query, [CanBeNull] params SqlParameter[] parameters ) {
			if ( query.IsNullOrWhiteSpace() ) { throw new ArgumentNullException( nameof( query ) ); }

			try {
				using ( var connection = new SqlConnection( this._connectionString ) ) {
					connection.Open();

					using ( var command = new SqlCommand( query, connection ) {
						CommandType = CommandType.Text,
						CommandTimeout = 0
					} ) {
						if ( null != parameters ) { command.Parameters.AddRange( parameters ); }

						command.ExecuteNonQuery();
					}

					return true;
				}
			}
			catch ( SqlException exception ) { exception.Log(); }
			catch ( DbException exception ) { exception.Log(); }
			catch ( Exception exception ) { exception.Log(); }

			return false;
		}

		[SuppressMessage( "Microsoft.Security", "CA2100:Review SQL queries for security vulnerabilities", Justification = "StoredProcedure" )]
		public Boolean ExecuteNonQuery( String query, Int32 retries, [CanBeNull] params SqlParameter[] parameters ) {
			if ( query.IsNullOrWhiteSpace() ) { throw new ArgumentNullException( nameof( query ) ); }

			TryAgain:

			try {
				using ( var connection = new SqlConnection( this._connectionString ) ) {
					connection.Open();

					using ( var command = new SqlCommand( query, connection ) {
						CommandType = CommandType.StoredProcedure
					} ) {
						if ( null != parameters ) { command.Parameters.AddRange( parameters ); }

						command.ExecuteNonQuery();

						return true;
					}
				}
			}
			catch ( InvalidOperationException ) {

				//timeout probably
				retries--;

				if ( retries.Any() ) { goto TryAgain; }
			}
			catch ( SqlException exception ) { exception.Log(); }
			catch ( DbException exception ) { exception.Log(); }

			return false;
		}

		[ItemCanBeNull]
		public async Task<Int32?> ExecuteNonQueryAsync( String query, CommandType commandType, [CanBeNull] params SqlParameter[] parameters ) {
			if ( query.IsNullOrWhiteSpace() ) { throw new ArgumentNullException( nameof( query ) ); }

			try {
				using ( var connection = new SqlConnection( this._connectionString ) ) {
					connection.Open();

					using ( var command = new SqlCommand( query, connection ) {
						CommandType = commandType,
						CommandTimeout = 0
					} ) {
						if ( null != parameters ) { command.Parameters.AddRange( parameters ); }

						return await command.ExecuteNonQueryAsync().NoUI();
					}
				}
			}
			catch ( SqlException exception ) { exception.Log(); }
			catch ( DbException exception ) { exception.Log(); }

			return null;
		}

		/// <summary>
		///     Returns a <see cref="DataTable" />
		/// </summary>
		/// <param name="query">      </param>
		/// <param name="commandType"></param>
		/// <param name="table">      </param>
		/// <param name="parameters"> </param>
		/// <returns></returns>
		public Boolean ExecuteReader( String query, CommandType commandType, [NotNull] out DataTable table, [CanBeNull] params SqlParameter[] parameters ) {
			if ( query.IsNullOrWhiteSpace() ) { throw new ArgumentNullException( nameof( query ) ); }

			table = new DataTable();

			try {
				using ( var connection = new SqlConnection( this._connectionString ) ) {
					connection.Open();

					using ( var command = new SqlCommand( query, connection ) {
						CommandType = commandType
					} ) {
						if ( null != parameters ) { command.Parameters.AddRange( parameters ); }

						table.BeginLoadData();

						using ( var reader = command.ExecuteReader() ) { table.Load( reader ); }

						table.EndLoadData();

						return true;
					}
				}
			}
			catch ( SqlException exception ) { exception.Log(); }
			catch ( DbException exception ) { exception.Log(); }
			catch ( Exception exception ) { exception.Log(); }

			return false;
		}

		/// <summary>
		/// </summary>
		/// <param name="query">      </param>
		/// <param name="commandType"></param>
		/// <param name="parameters"> </param>
		/// <returns></returns>
		[ItemCanBeNull]
		public async Task<SqlDataReader> ExecuteReaderAsyncDataReader( String query, CommandType commandType, [CanBeNull] params SqlParameter[] parameters ) {
			if ( query.IsNullOrWhiteSpace() ) { throw new ArgumentNullException( nameof( query ) ); }

			try {
				using ( var connection = new SqlConnection( this._connectionString ) ) {
					connection.Open();

					using ( var command = new SqlCommand( query, connection ) {
						CommandType = commandType
					} ) {
						if ( null != parameters ) { command.Parameters.AddRange( parameters ); }

						return await command.ExecuteReaderAsync().NoUI();
					}
				}
			}
			catch ( SqlException exception ) { exception.Log(); }

			return null;
		}

		/// <summary>
		///     Returns a <see cref="DataTable" />
		/// </summary>
		/// <param name="query">      </param>
		/// <param name="commandType"></param>
		/// <param name="parameters"> </param>
		/// <returns></returns>
		[ItemNotNull]
		public async Task<DataTable> ExecuteReaderAsyncDataTable( String query, CommandType commandType, [CanBeNull] params SqlParameter[] parameters ) {
			if ( query.IsNullOrWhiteSpace() ) { throw new ArgumentNullException( nameof( query ) ); }

			var table = new DataTable();

			try {
				using ( var connection = new SqlConnection( this._connectionString ) ) {
					connection.Open();

					using ( var command = new SqlCommand( query, connection ) {
						CommandType = commandType
					} ) {
						if ( null != parameters ) { command.Parameters.AddRange( parameters ); }

						table.BeginLoadData();

						using ( var reader = await command.ExecuteReaderAsync().NoUI() ) { table.Load( reader ); }

						table.EndLoadData();
					}
				}
			}
			catch ( SqlException exception ) { exception.Log(); }
			catch ( DbException exception ) { exception.Log(); }
			catch ( Exception exception ) { exception.Log(); }

			return table;
		}

		/// <summary>
		///     <para>Returns the first column of the first row.</para>
		/// </summary>
		/// <param name="query">      </param>
		/// <param name="commandType"></param>
		/// <param name="parameters"> </param>
		/// <returns></returns>
		[CanBeNull]
		public TResult ExecuteScalar<TResult>( String query, CommandType commandType, [CanBeNull] params SqlParameter[] parameters ) {
			if ( query.IsNullOrWhiteSpace() ) { throw new ArgumentNullException( nameof( query ) ); }

			try {
				using ( var connection = new SqlConnection( this._connectionString ) ) {
					connection.Open();

					using ( var command = new SqlCommand( query, connection ) {
						CommandType = CommandType.StoredProcedure
					} ) {
						if ( null != parameters ) { command.Parameters.AddRange( parameters ); }

						var scalar = command.ExecuteScalar();

						if ( null == scalar || Convert.IsDBNull( scalar ) ) { return default; }

						if ( scalar is TResult executeScalar ) { return executeScalar; }

						if ( scalar.TryCast<TResult>( out var result ) ) { return result; }

						return ( TResult ) Convert.ChangeType( scalar, typeof( TResult ) );
					}
				}
			}
			catch ( SqlException exception ) { exception.Log(); }
			catch ( DbException exception ) { exception.Log(); }

			return default;
		}

		/// <summary>
		///     <para>Returns the first column of the first row.</para>
		/// </summary>
		/// <param name="query">      </param>
		/// <param name="commandType"></param>
		/// <param name="parameters"> </param>
		/// <returns></returns>
		[ItemCanBeNull]
		public async Task<TResult> ExecuteScalarAsync<TResult>( String query, CommandType commandType, [CanBeNull] params SqlParameter[] parameters ) {
			if ( query.IsNullOrWhiteSpace() ) { throw new ArgumentNullException( nameof( query ) ); }

			try {
				using ( var connection = new SqlConnection( this._connectionString ) ) {
					connection.Open();

					using ( var command = new SqlCommand( query, connection ) {
						CommandType = commandType
					} ) {
						if ( null != parameters ) { command.Parameters.AddRange( parameters ); }

						var task = command.ExecuteScalarAsync().NoUI();

						var result = await task;

						if ( result == DBNull.Value ) { return default; }

						return ( TResult ) result;
					}
				}
			}
			catch ( InvalidCastException exception ) {

				//TIP: check for SQLServer returning a Double when you expect a Single (float in SQL).
				exception.Log();
			}
			catch ( SqlException exception ) { exception.Log(); }

			return default;
		}

		/// <summary>
		///     Returns a <see cref="DataTable" />
		/// </summary>
		/// <param name="query">     </param>
		/// <param name="parameters"></param>
		/// <returns></returns>
		[ItemCanBeNull]
		public IEnumerable<TResult> QueryList<TResult>( String query, [CanBeNull] params SqlParameter[] parameters ) {
			if ( query.IsNullOrWhiteSpace() ) { throw new ArgumentNullException( nameof( query ) ); }

			try {
				using ( var connection = new SqlConnection( this._connectionString ) ) {
					connection.Open();

					using ( var command = new SqlCommand( query, connection ) {
						CommandType = CommandType.StoredProcedure
					} ) {
						if ( null != parameters ) { command.Parameters.AddRange( parameters ); }

						using ( var reader = command.ExecuteReader() ) {
							var data = GenericPopulator<TResult>.CreateList( reader );

							return data;
						}
					}
				}
			}
			catch ( SqlException exception ) { exception.Log(); }
			catch ( DbException exception ) { exception.Log(); }
			catch ( Exception exception ) { exception.Log(); }

			return null;
		}

		private readonly String _connectionString;

		/// <summary>
		/// </summary>
		/// <param name="connectionString"></param>
		/// <exception cref="InvalidOperationException"></exception>
		public Database( String connectionString ) => this._connectionString = connectionString;

		public override void DisposeManaged() { }
	}
}