// Copyright 2018 Protiguous.
//
// This notice must be kept visible in the source.
//
// This section of source code belongs to Protiguous@Protiguous.com unless otherwise specified or the original license has been overwritten by the automatic formatting of this code.
//
// Any unmodified sections of source code borrowed from other projects retain their original license and thanks goes to the Authors.
//
// Donations, royalties, and licenses can be paid via bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//
// Usage of the source code or compiled binaries is AS-IS. I am not responsible for Anything You Do.
//
// Contact me by email if you have any questions or helpful criticism.
//
// "Librainian/IDatabase.cs" was last cleaned by Protiguous on 2018/05/12 at 1:22 AM

namespace Librainian.Database {

    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;
    using System.Threading.Tasks;
    using JetBrains.Annotations;

    public interface IDatabase {

        /// <summary>
        /// Opens and then closes a <see cref="SqlConnection"/>.
        /// </summary>
        /// <returns></returns>
        Boolean ExecuteNonQuery( String query, params SqlParameter[] parameters );

        Boolean ExecuteNonQuery( String query, Int32 retries, params SqlParameter[] parameters );

        Task<Int32?> ExecuteNonQueryAsync( String query, CommandType commandType, params SqlParameter[] parameters );

        /// <summary>
        /// Returns a <see cref="DataTable"/>
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
        /// Returns a <see cref="DataTable"/>
        /// </summary>
        /// <param name="query">      </param>
        /// <param name="commandType"></param>
        /// <param name="parameters"> </param>
        /// <returns></returns>
        Task<DataTable> ExecuteReaderAsyncDataTable( String query, CommandType commandType, params SqlParameter[] parameters );

        /// <summary>
        /// <para>Returns the first column of the first row.</para>
        /// </summary>
        /// <param name="query">      </param>
        /// <param name="commandType"></param>
        /// <param name="parameters"> </param>
        /// <returns></returns>
        TResult ExecuteScalar<TResult>( [NotNull] String query, CommandType commandType, params SqlParameter[] parameters );

        /// <summary>
        /// <para>Returns the first column of the first row.</para>
        /// </summary>
        /// <param name="query">      </param>
        /// <param name="commandType"></param>
        /// <param name="parameters"> </param>
        /// <returns></returns>
        Task<TResult> ExecuteScalarAsync<TResult>( [NotNull] String query, CommandType commandType, params SqlParameter[] parameters );

        /// <summary>
        /// Returns a <see cref="DataTable"/>
        /// </summary>
        /// <param name="query">     </param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        IEnumerable<TResult> QueryList<TResult>( [NotNull] String query, params SqlParameter[] parameters );
    }
}