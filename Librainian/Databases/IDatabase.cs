// Copyright © Rick@AIBrain.org and Protiguous. All Rights Reserved.
// 
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
// 
// This source code contained in "IDatabase.cs" belongs to Protiguous@Protiguous.com and
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
//     PayPal:Protiguous@Protiguous.com
//     (We're always looking into other solutions.. Any ideas?)
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
// Feel free to browse any source code we make available.
// 
// Project: "Librainian", "IDatabase.cs" was last formatted by Protiguous on 2019/11/19 at 6:16 AM.

namespace Librainian.Databases {

    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;
    using System.Threading.Tasks;
    using JetBrains.Annotations;

    public interface IDatabase {

        TimeSpan CommandTimeout { get; set; }

        String Sproc { get; set; }

        TimeSpan Timeout { get; set; }

        void Add<T>( [NotNull] String name, SqlDbType type, [CanBeNull] T value );

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
        Task<Boolean> FillTableAsync( [NotNull] String sproc, CommandType commandType, [NotNull] DataTable table );

        /// <summary>
        ///     <para>Run a query, no rows expected to be read.</para>
        ///     <para>Does not catch any exceptions.</para>
        /// </summary>
        /// <param name="sproc"></param>
        /// <param name="commandType"></param>
        Task NonQueryAsync( [NotNull] String sproc, CommandType commandType );

        /// <summary>Make sure to include any parameters ( <see cref="DatabaseServer.Add{T}" />) to avoid sql injection attacks.</summary>
        /// <param name="sql"></param>
        DataTableReader QueryAdHoc( [NotNull] String sql );

        /// <summary>Make sure to include any parameters ( <see cref="DatabaseServer.Add{T}" />) to avoid sql injection attacks.</summary>
        /// <param name="sql"></param>
        Task<DataTableReader> QueryAdHocAsync( [NotNull] String sql );

        /// <summary>Simplest possible database connection.
        /// <para>Connect and then run <paramref name="sproc" />.</para>
        /// </summary>
        /// <param name="sproc"></param>
        /// <param name="commandType"></param>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        Task<SqlDataReader> QueryAsync( [NotNull] String sproc, CommandType commandType );

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