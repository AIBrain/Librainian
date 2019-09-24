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
// Project: "Librainian", "IDatabase.cs" was last formatted by Protiguous on 2019/09/12 at 10:38 AM.

namespace Librainian.Databases {

    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;
    using System.Threading.Tasks;
    using JetBrains.Annotations;

    public interface IDatabase {

        /// <summary>
        ///     Opens and then closes a <see cref="SqlConnection" />.
        /// </summary>
        /// <returns></returns>
        Int32? ExecuteNonQuery( String query, params SqlParameter[] parameters );

        Int32? ExecuteNonQuery( String query, Int32 retries, params SqlParameter[] parameters );

        Task<Int32?> ExecuteNonQueryAsync( String query, CommandType commandType, params SqlParameter[] parameters );

        /// <summary>
        ///     Returns a <see cref="DataTable" />
        /// </summary>
        /// <param name="query">      </param>
        /// <param name="commandType"></param>
        /// <param name="table">      </param>
        /// <param name="parameters"> </param>
        /// <returns></returns>
        Boolean ExecuteReader( [NotNull] String query, CommandType commandType, out DataTable table, params SqlParameter[] parameters );

        /// <summary>
        /// </summary>
        /// <param name="query">      </param>
        /// <param name="commandType"></param>
        /// <param name="parameters"> </param>
        /// <returns></returns>
        Task<SqlDataReader> ExecuteReaderAsyncDataReader( String query, CommandType commandType, params SqlParameter[] parameters );

        /// <summary>
        ///     Returns a <see cref="DataTable" />
        /// </summary>
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
        (Status status, TResult result) ExecuteScalar<TResult>( [NotNull] String query, CommandType commandType, params SqlParameter[] parameters );

        /// <summary>
        ///     <para>Returns the first column of the first row.</para>
        /// </summary>
        /// <param name="query">      </param>
        /// <param name="commandType"></param>
        /// <param name="parameters"> </param>
        /// <returns></returns>
        Task<(Status status, TResult result)> ExecuteScalarAsync<TResult>( [NotNull] String query, CommandType commandType, params SqlParameter[] parameters );

        /// <summary>
        ///     Returns a <see cref="IEnumerable{T}" />
        /// </summary>
        /// <param name="query">     </param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        IEnumerable<TResult> QueryList<TResult>( [NotNull] String query, params SqlParameter[] parameters );
    }
}