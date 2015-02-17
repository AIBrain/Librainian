#region License & Information

// This notice must be kept visible in the source.
//
// This section of source code belongs to Rick@AIBrain.Org unless otherwise specified,
// or the original license has been overwritten by the automatic formatting of this code.
// Any unmodified sections of source code borrowed from other projects retain their original license and thanks goes to the Authors.
//
// Donations and Royalties can be paid via
// PayPal: paypal@aibrain.org
// bitcoin:1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
// bitcoin:1NzEsF7eegeEWDr5Vr9sSSgtUC4aL6axJu
// litecoin:LeUxdU2w3o6pLZGVys5xpDZvvo8DUrjBp9
//
// Usage of the source code or compiled binaries is AS-IS.
// I am not responsible for Anything You Do.
//
// Contact me by email if you have any questions or helpful criticism.
//
// "Librainian 2015/Database.cs" was last cleaned by Rick on 2015/02/16 at 8:50 AM

#endregion License & Information

namespace Librainian.Database {

	using System;
	using System.Data;
	using System.Data.Common;
	using System.Data.Linq;
	using System.Data.SqlClient;
	using System.Diagnostics;
	using System.Net.NetworkInformation;
	using System.Threading;
	using System.Threading.Tasks;
	using System.Windows.Forms;
	using JetBrains.Annotations;
	using Measurement.Time;
	using Threading;

	public sealed class Database : IDisposable {
		//public static readonly ConcurrentDictionary< String, TimeSpan > QueryAverages = new ConcurrentDictionary< String, TimeSpan >();

		//public static ThreadLocal<SQLQuery> Queries = new ThreadLocal<SQLQuery>( () => new SQLQuery( library: "dbmssocn",server: "127.0.0.1",username: "Anonymous",password: "Anonymous" ) );
		//Server=myServerName\myInstanceName;Database=myDataBase;User Id=myUsername;Password=myPassword;

		//public SqlConnection Connection = new SqlConnection();
		public Stopwatch SinceConnected { get; }
		= Stopwatch.StartNew();

		/// <summary>
		///     Create a database object to MainServer
		/// </summary>
		public Database( [NotNull] String library, [NotNull] String server, [ NotNull ] String catalog ,[NotNull] String username, [NotNull] String password, Seconds timeout ) {
			if ( library == null ) {
				throw new ArgumentNullException( nameof( library ) );
			}
			if ( server == null ) {
				throw new ArgumentNullException( nameof( server ) );
			}
			if ( catalog == null ) {
				throw new ArgumentNullException( nameof( catalog ) );
			}
			if ( username == null ) {
				throw new ArgumentNullException( nameof( username ) );
			}
			if ( password == null ) {
				throw new ArgumentNullException( nameof( password ) );
			}
			this.Library = library;
			this.Server = server;
			this.Catalog = catalog;
			this.UserName = username;
			this.Password = password;

			var scsb = new SqlConnectionStringBuilder {
				ApplicationIntent = ApplicationIntent.ReadWrite,
				ApplicationName = Application.ProductName,
				AsynchronousProcessing = true,
				ConnectRetryCount = 10,
				ConnectTimeout = ( int )timeout.Value,
				DataSource = this.Server,
				//InitialCatalog = "master",
				Password = this.Password,
				Pooling = true,
				MultipleActiveResultSets = true,
				NetworkLibrary = library,
				UserID = this.UserName, 
			};

			this.Connection = new SqlConnection( scsb.ConnectionString );
#if DEBUG
			this.Connection.InfoMessage += ( sender, args ) => args.Message.Info();
			this.Connection.StateChange += ( sender, args ) => String.Format( "sql state changed from {0} to {1}", args.OriginalState, args.CurrentState ).Info();
#endif
			this.Connect().Wait();

			DbConnection bob = new SqlConnection( scsb.ConnectionString );
			//bob.ChangeDatabase

			//DataContext bob = new DataContext( this.Connection.ConnectionString );
			//if ( !bob.DatabaseExists() ) {
			//	bob.CreateDatabase();
			//}

		}

		public Task Connect() {
			try {
				if ( this.Connection.State == ConnectionState.Open ) {
					this.Connection.Close();
				}
				return this.Connection.OpenAsync();
			}
			finally {
				this.SinceConnected.Restart();
			}
		}

		public SqlConnection Connection { get; }

		private string Library { get; }

		private string Password { get; }

		private string Server { get; }
		public string Catalog { get; set; }

		private string UserName { get; }

		public void Dispose() {
			try {
				//TODO
			}
			catch ( InvalidOperationException exception ) {
				exception.More();
			}

			try {
				using (this.Connection) {
					if ( this.Connection.State != ConnectionState.Closed ) {
						this.Connection.Close();
					}
				}
			}
			catch ( InvalidOperationException exception ) {
				exception.More();
			}
		}

		///// <summary>
		/////     The parameter collection for this database connection
		///// </summary>
		//public SqlParameterCollection Params {
		//    get {
		//        this.Command.Should().NotBeNull();
		//        return this.Command.Parameters;
		//    }
		//}

		public static Boolean IsNetworkConnected( int retries = 3 ) {
			var counter = retries;
			while ( !NetworkInterface.GetIsNetworkAvailable() && counter > 0 ) {
				--counter;
				String.Format( "Network disconnected. Waiting {0}. {1} retries...", Seconds.One, counter ).WriteLine();
				Thread.Sleep( 1000 );
			}
			return NetworkInterface.GetIsNetworkAvailable();
		}

		/*
                public void NonQuery( String cmdText ) {
                TryAgain:
                    try {
                        //var stopwatch = Stopwatch.StartNew();
                        if ( this.GetConnection() ) {
                            var command = new SqlCommand( cmdText );
                            command.ExecuteNonQuery();
                        }

                        //QueryAverages.AddOrUpdate( key: sproc, addValue: stopwatch.Elapsed, updateValueFactory: ( s, span ) => new Milliseconds( (Decimal ) ( QueryAverages[ sproc ].Add( stopwatch.Elapsed ).TotalMilliseconds/2.0 ) ) );
                        //foreach ( var pair in QueryAverages.Where( pair => pair.Value >= Seconds.One ) ) {
                        //    String.Format( "[{0}] average time is {1}", pair.Key, pair.Value.Simpler() ).TimeDebug();
                        //    TimeSpan value;
                        //    QueryAverages.TryRemove( pair.Key, out value );
                        //}

                        //if ( sproc.Contains( "Blink" ) ) { Generic.Report( String.Format( "Blink time average is {0}", QueryAverages[sproc].Simple() ) ); }
                    }
                    catch ( Exception exception ) {
                        var lower = exception.Message.ToLower();

                        if ( lower.Contains( "deadlocked" ) ) {
                            "deadlock.wav".TryPlayFile();
                            goto TryAgain;
                        }
                        if ( lower.Contains( "transport-level error" ) ) {
                            "lostconnection.wav".TryPlayFile();
                            goto TryAgain;
                        }
                        if ( lower.Contains( "timeout" ) ) {
                            "timeout.wav".TryPlayFile();
                            goto TryAgain;
                        }
                        throw;
                    }
                }
        */

		[CanBeNull]
		public SqlConnection GetConnection() {
			var retries = 10;
TryAgain:
			try {
				switch ( this.Connection.State ) {
					case ConnectionState.Open:
						return this.Connection;

					case ConnectionState.Executing:
						return this.Connection;

					case ConnectionState.Fetching:
						return this.Connection;

					case ConnectionState.Closed:
						this.Connect().Wait();
						if ( retries > 0 ) {
							--retries;
							goto TryAgain;
						}
						break;

					case ConnectionState.Connecting:
						while ( this.Connection.State == ConnectionState.Connecting ) {
							Task.Delay( Milliseconds.TwoHundredEleven ).Wait();
						}
						return GetConnection();

					case ConnectionState.Broken:
						this.Connect().Wait();
						if ( retries > 0 ) {
							--retries;
							goto TryAgain;
						}
						break;

					default:
						throw new ArgumentOutOfRangeException();
				}

				return this.Connection;
			}
			catch ( SqlException exception ) {
				if ( !IsNetworkConnected() ) {
					Task.Delay( Seconds.One ).Wait();
					goto TryAgain;
				}
				exception.More();
			}
			catch ( InvalidOperationException exception ) {
				exception.More();
			}
			return null;
		}

		//[CanBeNull]
		//public DataTableReader Query( String sproc ) {
		//TryAgain:
		//    try {
		//        var stopwatch = Stopwatch.StartNew();

		//        if ( this.GetConnection() ) {
		//            this.Command.CommandText = sproc;

		//            var table = new DataTable();
		//            table.BeginLoadData();
		//            using ( var reader = this.Command.ExecuteReader() ) {
		//                table.Load( reader, LoadOption.OverwriteChanges );
		//            }
		//            table.EndLoadData();

		//            QueryAverages.AddOrUpdate( key: sproc, addValue: stopwatch.Elapsed, updateValueFactory: ( s, span ) => new Milliseconds( ( Decimal )( QueryAverages[ sproc ].Add( stopwatch.Elapsed ).TotalMilliseconds / 2.0 ) ) );

		//            return table.CreateDataReader();
		//        }
		//    }
		//    catch ( Exception exception ) {
		//        var lower = exception.Message.ToLower();
		//        if ( lower.Contains( "deadlocked" ) ) {
		//            "deadlock.wav".TryPlayFile();
		//            goto TryAgain;
		//        }
		//        if ( lower.Contains( "transport-level error" ) ) {
		//            "lostconnection.wav".TryPlayFile();
		//            goto TryAgain;
		//        }
		//        if ( lower.Contains( "timeout" ) ) {
		//            "timeout.wav".TryPlayFile();
		//            goto TryAgain;
		//        }
		//        exception.Error();
		//        throw;
		//    }
		//    return null;
		//}

		/// <summary>
		///     Returns the first column of the first row.
		/// </summary>
		/// <param name="query"></param>
		/// <param name="commandType"></param>
		/// <param name="parameters"></param>
		/// <returns></returns>
		[CanBeNull]
		public async Task<TResult> QueryScalarAsync<TResult>( String query, CommandType commandType, params SqlParameter[] parameters ) {
			//TryAgain:
			try {
				var command = new SqlCommand( query, GetConnection() ) {
					CommandType = commandType
				};
				if ( null != parameters ) {
					command.Parameters.AddRange( parameters );
				}
				var bob = await command.ExecuteScalarAsync();
				return ( TResult )bob;
			}
			catch ( SqlException exception ) {
				exception.More();
				//var lower = exception.Message.ToLower();

				//            if ( lower.Contains( "deadlocked" ) ) {
				//                "deadlock.wav".TryPlayFile();
				//                goto TryAgain;
				//            }
				//            if ( lower.Contains( "transport-level error" ) ) {
				//                "lostconnection.wav".TryPlayFile();
				//                goto TryAgain;
				//            }
				//            if ( lower.Contains( "timeout" ) ) {
				//                "timeout.wav".TryPlayFile();
				//                goto TryAgain;
				//            }
				//            throw;
			}
			return default(TResult);
		}

		[NotNull]
		public async Task QueryWithNoResultAsync( String query, CommandType commandType, params SqlParameter[] parameters ) {
			//TryAgain:
			try {
				var command = new SqlCommand( query, GetConnection() ) {
					CommandType = commandType
				};
				if ( null != parameters ) {
					command.Parameters.AddRange( parameters );
				}
				await command.ExecuteNonQueryAsync();
			}
			catch ( SqlException exception ) {
				exception.More();
				//var lower = exception.Message.ToLower();

				//            if ( lower.Contains( "deadlocked" ) ) {
				//                "deadlock.wav".TryPlayFile();
				//                goto TryAgain;
				//            }
				//            if ( lower.Contains( "transport-level error" ) ) {
				//                "lostconnection.wav".TryPlayFile();
				//                goto TryAgain;
				//            }
				//            if ( lower.Contains( "timeout" ) ) {
				//                "timeout.wav".TryPlayFile();
				//                goto TryAgain;
				//            }
				//            throw;
			}
		}

		/// <summary>
		/// </summary>
		/// <param name="query"></param>
		/// <param name="commandType"></param>
		/// <param name="parameters"></param>
		/// <returns></returns>
		[CanBeNull]
		public async Task<SqlDataReader> QueryWithResultAsync( String query, CommandType commandType, params SqlParameter[] parameters ) {
			//TryAgain:
			try {
				var command = new SqlCommand( query, GetConnection() ) {
					CommandType = commandType
				};
				if ( null != parameters ) {
					command.Parameters.AddRange( parameters );
				}
				return await command.ExecuteReaderAsync();
			}
			catch ( SqlException exception ) {
				exception.More();
				//var lower = exception.Message.ToLower();

				//            if ( lower.Contains( "deadlocked" ) ) {
				//                "deadlock.wav".TryPlayFile();
				//                goto TryAgain;
				//            }
				//            if ( lower.Contains( "transport-level error" ) ) {
				//                "lostconnection.wav".TryPlayFile();
				//                goto TryAgain;
				//            }
				//            if ( lower.Contains( "timeout" ) ) {
				//                "timeout.wav".TryPlayFile();
				//                goto TryAgain;
				//            }
				//            throw;
			}
			return null;
		}
	}
}