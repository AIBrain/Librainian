// Copyright © Protiguous. All Rights Reserved.
// This entire copyright notice and license must be retained and must be kept visible in any binaries, libraries, repositories, or source code (directly or derived) from our binaries, libraries, projects, solutions, or applications.
// All source code belongs to Protiguous@Protiguous.com unless otherwise specified or the original license has been overwritten by formatting.
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
//
// Our software can be found at "https://Protiguous.com/Software"
// Our GitHub address is "https://github.com/Protiguous".
//
// File "LocalDB.cs" last formatted on 2021-02-03 at 5:04 PM.

#nullable enable

namespace Librainian.Databases {

	using System;
	using System.Data;
	using System.Data.Common;
	using System.Reflection;
	using System.Threading;
	using System.Threading.Tasks;
	using FileSystem;
	using FileSystem.Pri.LongPath;
	using JetBrains.Annotations;
	using Logging;
	using Measurement.Time;
	using Microsoft.Data.SqlClient;
	using PooledAwait;
	using Utilities;

	public class LocalDb : ABetterClassDispose {

		public LocalDb( [NotNull] String databaseName, [CanBeNull] Folder? databaseLocation = null, TimeSpan? timeoutForReads = null, TimeSpan? timeoutForWrites = null ) {

			//TODO Check for [] around databaseName.
			//TODO Check for spaces in databaseName.

			if ( String.IsNullOrWhiteSpace( databaseName ) ) {
				throw new ArgumentNullException( nameof( databaseName ) );
			}

			databaseLocation ??= new Folder( Environment.SpecialFolder.LocalApplicationData, Assembly.GetEntryAssembly()?.Location.GetDirectoryName() ?? nameof( LocalDb ) );

			this.ReadTimeout = timeoutForReads.GetValueOrDefault( Seconds.Thirty );
			this.WriteTimeout = timeoutForWrites.GetValueOrDefault( this.ReadTimeout + Seconds.Thirty );

			this.DatabaseName = databaseName;
			this.DatabaseLocation = databaseLocation;

			this.DatabaseLocation.Create( new CancellationTokenSource( Seconds.Ten ).Token ).AsValueTask().AsTask().Wait( Seconds.Ten );

			"Building SQL connection string...".Info();

			this.DatabaseMdf = new Document( this.DatabaseLocation, $"{this.DatabaseName}.mdf" );
			this.DatabaseLog = new Document( this.DatabaseLocation, $"{this.DatabaseName}_log.ldf" ); //TODO does localdb even use a log file?

			this.ConnectionString = @"Data Source=(localdb)\MSSQLLocalDB;Integrated Security=True;Initial Catalog=master;Integrated Security=True;";

			var _ = this.Initialize( new CancellationTokenSource( Minutes.One ).Token );
		}

		private async FireAndForget Initialize( CancellationToken cancellationToken ) {
			var exists = await this.DatabaseMdf.Exists( cancellationToken ).ConfigureAwait( false );

			if ( !exists ) {
				await using var connection = new SqlConnection( this.ConnectionString );

				await connection.OpenAsync( cancellationToken ).ConfigureAwait( false );
				var command = connection.CreateCommand();

				if ( command is null ) {
					throw new InvalidOperationException( $"Error creating command object for {nameof( LocalDb )}." );
				}

				command.CommandText = String.Format( "CREATE DATABASE {0} ON (NAME = N'{0}', FILENAME = '{1}')", this.DatabaseName, this.DatabaseMdf.FullPath );

				await command.ExecuteNonQueryAsync( cancellationToken ).ConfigureAwait( false );
			}

			this.ConnectionString = $@"Data Source=(localdb)\MSSQLLocalDB;Integrated Security=True;Initial Catalog={this.DatabaseName};AttachDBFileName={this.DatabaseMdf.FullPath};";

			this.Connection = new SqlConnection( this.ConnectionString );
			this.Connection.InfoMessage += ( _, args ) => args?.Message.Info();
			this.Connection.StateChange += ( _, args ) => $"{args.OriginalState} -> {args.CurrentState}".Info();
			this.Connection.Disposed += ( _, args ) => $"Disposing SQL connection {args}".Info();

			$"Attempting connection to {this.DatabaseMdf}...".Info();
			await this.Connection.OpenAsync( cancellationToken ).ConfigureAwait( false );
			this.Connection.ServerVersion.Info();
			this.Connection.Close();
		}

		[NotNull]
		public SqlConnection Connection { get; set; }

		[NotNull]
		public String ConnectionString { get; set; }

		[NotNull]
		public Folder DatabaseLocation { get; }

		[NotNull]
		public Document DatabaseLog { get; }

		[NotNull]
		public Document DatabaseMdf { get; }

		[NotNull]
		public String DatabaseName { get; }

		public TimeSpan ReadTimeout { get; }

		public TimeSpan WriteTimeout { get; }

		public async Task DetachDatabaseAsync() {
			try {
				if ( this.Connection.State == ConnectionState.Closed ) {
					await this.Connection.OpenAsync().ConfigureAwait( false );
				}

#if NET48
				using var cmd = this.Connection.CreateCommand();
#else
				await using var cmd = this.Connection.CreateCommand();
#endif

				if ( cmd != null ) {
					cmd.CommandText = String.Format( "ALTER DATABASE {0} SET SINGLE_USER WITH ROLLBACK IMMEDIATE; exec sp_detach_db N'{0}'", this.DatabaseName );
					await cmd.ExecuteNonQueryAsync().ConfigureAwait( false );
				}
			}
			catch ( SqlException exception ) {
				exception.Log();
			}
			catch ( DbException exception ) {
				exception.Log();
			}
		}

		public override void DisposeManaged() => this.DetachDatabaseAsync().Wait( this.ReadTimeout + this.WriteTimeout );
	}
}