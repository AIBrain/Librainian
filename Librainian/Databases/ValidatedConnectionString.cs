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
// File "ValidatedConnectionString.cs" last touched on 2021-12-19 at 8:01 AM by Protiguous.

namespace Librainian.Databases;

using System;
using System.Collections.Generic;
using Configuration;
using Exceptions;
using LazyCache;
using Logging;
using Microsoft.Data.SqlClient;
using Parsing;

public record ValidatedConnectionString : IValidatedConnectionString {

	/// <summary>
	///     Uses the <paramref name="secretRevealer" /> to obtain a database connection string and validated it through a
	///     <see
	///         cref="SqlConnectionStringBuilder" />
	///     .
	/// </summary>
	/// <param name="secretRevealer"></param>
	/// <param name="applicationSetting"></param>
	/// <param name="cache"></param>
	/// <exception cref="KeyNotFoundException"></exception>
	/// <exception cref="FormatException"></exception>
	/// <exception cref="NullException"></exception>
	public ValidatedConnectionString( ISecretRevealer secretRevealer, IApplicationSetting applicationSetting, IAppCache cache ) {
		if ( secretRevealer is null ) {
			throw new NullException( nameof( secretRevealer ) );
		}

		if ( applicationSetting is null ) {
			throw new NullException( nameof( applicationSetting ) );
		}

		this.Cache = cache ?? throw new NullException( nameof( cache ) );

		var key = Common.ToKey( nameof( PullFromSecrectsAndRunThroughBuilder ), nameof( DatabaseServer ), nameof( ValidatedConnectionString ) );
		this.Value = this.Cache.GetOrAdd( key, PullFromSecrectsAndRunThroughBuilder ) ?? throw new NullException( nameof( this.Cache ) );

		return;	

		String PullFromSecrectsAndRunThroughBuilder() {
			//This is ran when the Value is currently not cached
			$"Caching new {nameof( DatabaseServer )} on thread {Environment.CurrentManagedThreadId} for app {applicationSetting.DoubleQuote()}.".Verbose();

			var trimmed = secretRevealer.GetDatabaseConnectionString().Trimmed() ?? throw new NullException( nameof( secretRevealer.GetDatabaseConnectionString ) );

			var builder = new SqlConnectionStringBuilder( trimmed ) {
				Pooling = true,
				Encrypt = false,
				MaxPoolSize = 2048,
				IntegratedSecurity = false,
				TrustServerCertificate = false,
				MultipleActiveResultSets = false,
				WorkstationID = Environment.MachineName,
				ApplicationIntent = ApplicationIntent.ReadWrite,
				ApplicationName = applicationSetting.GetProductName(),
				Authentication = SqlAuthenticationMethod.SqlPassword

				//ConnectTimeout = ( Int32 )DatabaseServer.DefaultConnectionTimeout.TotalSeconds,
				//CommandTimeout = ( Int32 )DatabaseServer.DefaultExecuteTimeout.TotalSeconds
			};

			//builder.IntegratedSecurity = String.IsNullOrWhiteSpace( builder.UserID ) || String.IsNullOrWhiteSpace( builder.Password );

			/*
			if ( !String.IsNullOrEmpty( useDatabase = useDatabase.Trimmed() ) ) {
				builder.InitialCatalog = useDatabase;
			}
			*/

			//builder.ConnectionString.Verbose();
			return builder.ConnectionString;
		}
	}

	private IAppCache Cache { get; }

	public String Value { get; }

}