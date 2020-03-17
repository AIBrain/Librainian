// Copyright © 2020 Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible in any binaries, libraries, repositories, and source code (directly or derived)
// from our binaries, libraries, projects, or solutions.
//
// This source code contained in "IDatabase.cs" belongs to Protiguous@Protiguous.com unless otherwise specified or the original license has been overwritten
// by formatting. (We try to avoid it from happening, but it does accidentally happen.)
//
// Any unmodified portions of source code gleaned from other projects still retain their original license and our thanks goes to those Authors.
// If you find your code in this source code, please let us know so we can properly attribute you and include the proper license and/or copyright.
//
// If you want to use any of our code in a commercial project, you must contact Protiguous@Protiguous.com for permission and a quote.
//
// Donations are accepted via bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2 and PayPal: Protiguous@Protiguous.com
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
// For business inquiries, please contact me at Protiguous@Protiguous.com.
//
// Our website can be found at "https://Protiguous.com/"
// Our software can be found at "https://Protiguous.Software/"
// Our GitHub address is "https://github.com/Protiguous".
// Feel free to browse any source code we make available.
//
// Project: "Librainian", File: "IDatabase.cs" was last formatted by Protiguous on 2020/03/16 at 2:54 PM.

namespace Librainian.Databases {

    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Threading.Tasks;
    using JetBrains.Annotations;
    using Microsoft.Data.SqlClient;

    public interface IDatabase {

        TimeSpan CommandTimeout { get; set; }

        String Sproc { get; set; }

        //void Add<T>( [NotNull] String name, SqlDbType type, [CanBeNull] T value );

        /// <summary>Opens and then closes a <see cref="SqlConnection" />.</summary>
        /// <returns></returns>
        Int32? ExecuteNonQuery( String query, CommandType commandType, params SqlParameter[] parameters );

        Int32? ExecuteNonQuery( String query, Int32 retries, CommandType commandType, params SqlParameter[] parameters );

        Task<Int32?> ExecuteNonQueryAsync( String query, CommandType commandType, params SqlParameter[] parameters );

        /// <summary>Returns a <see cref="DataTable" /></summary>
        /// <param name="query">      </param>
        /// <param name="commandType"></param>
        /// <param name="table">      </param>
        /// <param name="parameters"> </param>
        /// <returns></returns>
        Boolean ExecuteReader( [NotNull] String query, CommandType commandType, out DataTable table, params SqlParameter[] parameters );

        /// <summary></summary>
        /// <param name="query">      </param>
        /// <param name="commandType"></param>
        /// <param name="parameters"> </param>
        /// <returns></returns>
        Task<DataTableReader> ExecuteReaderAsyncDataReader( String query, CommandType commandType, params SqlParameter[] parameters );

        /// <summary>Returns a <see cref="DataTable" /></summary>
        /// <param name="query">      </param>
        /// <param name="commandType"></param>
        /// <param name="parameters"> </param>
        /// <returns></returns>
        Task<DataTable> ExecuteReaderDataTableAsync( String query, CommandType commandType, params SqlParameter[] parameters );

        /// <summary>
        ///     <para>Returns the first column of the first row.</para>
        /// </summary>
        /// <param name="query">      </param>
        /// <param name="commandType"></param>
        /// <param name="parameters"> </param>
        /// <returns></returns>
        TResult ExecuteScalar<TResult>( [NotNull] String query, CommandType commandType, params SqlParameter[] parameters );

        /// <summary>
        ///     <para>Returns the first column of the first row.</para>
        /// </summary>
        /// <param name="query">      </param>
        /// <param name="commandType"></param>
        /// <param name="parameters"> </param>
        /// <returns></returns>
        Task<TResult> ExecuteScalarAsync<TResult>( [NotNull] String query, CommandType commandType, params SqlParameter[] parameters );

        /// <summary>Overwrites the <paramref name="table" /> contents with data from the <paramref name="sproc" />.
        /// <para>Note: Include the parameters after the sproc.</para>
        /// <para>Can throw exceptions on connecting or executing the sproc.</para>
        /// </summary>
        /// <param name="sproc"></param>
        /// <param name="commandType"></param>
        /// <param name="table"></param>
        /// <param name="parameters"></param>
        Task<Boolean> FillTableAsync( [NotNull] String sproc, CommandType commandType, [NotNull] DataTable table, params SqlParameter[] parameters );

        /// <summary>
        ///     <para>Run a query, no rows expected to be read.</para>
        ///     <para>Does not catch any exceptions.</para>
        /// </summary>
        /// <param name="sproc"></param>
        /// <param name="commandType"></param>
        /// <param name="parameters"></param>
        Task NonQueryAsync( [NotNull] String sproc, CommandType commandType, params SqlParameter[] parameters );

        DataTableReader QueryAdHoc( [NotNull] String sql, params SqlParameter[] parameters );

        Task<DataTableReader> QueryAdHocAsync( [NotNull] String sql, params SqlParameter[] parameters );

        /// <summary>
        ///     <para>Connect and then run <paramref name="sproc" />.</para>
        /// </summary>
        /// <param name="sproc"></param>
        /// <param name="commandType"></param>
        /// <param name="parameters"></param>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        Task<SqlDataReader> QueryAsync( [NotNull] String sproc, CommandType commandType, params SqlParameter[] parameters );

        /// <summary>Returns a <see cref="IEnumerable{T}" /></summary>
        /// <param name="query">     </param>
        /// <param name="commandType"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        IEnumerable<TResult> QueryList<TResult>( [NotNull] String query, CommandType commandType, params SqlParameter[] parameters );

        void UseDatabase( [NotNull] String dbName );

        Task UseDatabaseAsync( [NotNull] String dbName );
    }
}