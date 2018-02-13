// Copyright 2017 Rick@AIBrain.org.
// 
// This notice must be kept visible in the source.
// 
// This section of source code belongs to Rick@AIBrain.Org unless otherwise specified, or the
// original license has been overwritten by the automatic formatting of this code. Any unmodified
// sections of source code borrowed from other projects retain their original license and thanks
// goes to the Authors.
// 
// Donations and royalties can be paid via
//  PayPal: paypal@aibrain.org
//  bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//  litecoin: LeUxdU2w3o6pLZGVys5xpDZvvo8DUrjBp9
// 
// Usage of the source code or compiled binaries is AS-IS. I am not responsible for Anything You Do.
// 
// Contact me by email if you have any questions or helpful criticism.
// 
// "Librainian/DatabaseExtensions.cs" was last cleaned by Rick on 2017/01/19 at 6:58 AM

namespace Librainian.Database {

	using System;
	using System.Collections.Generic;
	using System.Data;
	using System.Data.SqlClient;
	using System.Diagnostics;
	using System.Linq;
	using System.Management;
	using System.Media;
	using System.Reflection;
	using System.ServiceProcess;
	using System.Threading;
	using Extensions;
	using JetBrains.Annotations;
	using Maths;
	using Newtonsoft.Json;
	using NUnit.Framework;

	public static class DatabaseExtensions {

		private static readonly Dictionary<Type, IList<PropertyInfo>> TypeDictionary = new Dictionary<Type, IList<PropertyInfo>>();

		public static IList<PropertyInfo> GetPropertiesForType<T>() {
			var type = typeof( T );
			if ( !DatabaseExtensions.TypeDictionary.ContainsKey( typeof( T ) ) ) {
				DatabaseExtensions.TypeDictionary.Add( type, type.GetProperties().ToList() );
			}
			return DatabaseExtensions.TypeDictionary[ type ];
		}

		/// <summary>Convert our IList to a DataSet</summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="list"></param>
		/// <returns>DataSet</returns>
		/// <copyright>Based from http://codereview.stackexchange.com/q/40891</copyright>
		public static DataSet ToDataSet<T>( this IEnumerable<T> list ) {
			var ds = new DataSet();
			ds.Tables.Add( list.ToDataTable() );
			return ds;
		}

		/// <summary>
		///     <para>Warning: Untested and buggy.</para>
		///     Convert our IList to a DataTable
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="list"></param>
		/// <returns>DataTable</returns>
		/// <copyright>Based from http://codereview.stackexchange.com/q/40891</copyright>
		public static DataTable ToDataTable<T>( this IEnumerable<T> list ) {
			var elementType = typeof( T );

			var t = new DataTable();

			var properties = elementType.GetProperties();

			foreach ( var propInfo in properties ) {
				var propertyType = propInfo.PropertyType;
				var colType = Nullable.GetUnderlyingType( propertyType ) ?? propertyType;
				t.Columns.Add( propInfo.Name, colType );
			}

			foreach ( var item in list ) {
				foreach ( var propInfo in properties ) {
					var newRow = t.NewRow();

					// try { var ival = propInfo.GetValue( item );
					newRow[ propInfo.Name ] = item; // DBNull.Value; //ival ??

					// } catch ( Exception exception) { Debug.WriteLine( exception.Message ); }
					t.Rows.Add( newRow );
				}
			}
			return t;
		}

		public static DataTable ToDataTable( this SqlDataReader dataReader ) {
			var table = new DataTable();
			table.BeginLoadData();
			if ( dataReader != null ) {
				table.Load( dataReader, LoadOption.OverwriteChanges );
			}
			table.EndLoadData();
			return table;
		}

		public static IEnumerable<T> ToList<T>( this DataTable table ) {
			var properties = GetPropertiesForType<T>();
			IList<T> result = new List<T>();

			foreach ( var row in table.Rows ) {
				var item = DatabaseExtensions.CreateItemFromRow<T>( ( DataRow )row, properties );
				result.Add( item );
			}

			return result;
		}

		public static SqlParameter ToSqlParameter<TValue>( this TValue value, String parameterName ) => new SqlParameter( parameterName, value ) { Value = value };

		public static SqlParameter ToSqlParameter( this SqlDbType sqlDbType, String parameterName, Int32 size ) => new SqlParameter( parameterName, sqlDbType, size );

		/*
                private static List<T> MapList<T>( DataTable dt ) {
                    List<T> list = new List<T>();

                    FieldInfo[] fields = typeof( T ).GetFields();
                    T t = Activator.CreateInstance<T>();

                    foreach ( DataRow dr in dt.Rows ) {
                        foreach ( FieldInfo fi in fields )
                            fi.SetValueDirect( __makeref(t), dr[ fi.Name ] );

                        list.Add( t );
                    }

                    return list;
                }
        */
		/*

                /// <summary>
                /// <para>
                /// "Attempting below to get a fluent Object to DTO builder - which fails when a
                /// property is missed."
                /// </para>
                /// </summary>
                /// <typeparam name="T1"></typeparam>
                /// <typeparam name="T2"></typeparam>
                /// <param name="obj"></param>
                /// <param name="items"></param>
                /// <returns></returns>
                /// <copyright>Based from http://codereview.stackexchange.com/q/69359</copyright>
                public static T2 ToDto<T1, T2>( this T1 obj, params Expression<Func<T1, dynamic>>[] items ) where T1 : class {
                    var eo = new ExpandoObject();
                    var props = eo as IDictionary<String, object>;

                    foreach ( var item in items ) {
                        var member = item.Body as MemberExpression;
                        var unary = item.Body as UnaryExpression;
                        var body = member ?? unary?.Operand as MemberExpression;

                        if ( member != null && body.Member is PropertyInfo ) {
                            var property = body.Member as PropertyInfo;
                            props[ property.Name ] = obj.GetType().GetProperty( property.Name ).GetValue( obj, null );
                        }
                        else if ( unary != null ) {
                            var ubody = ( UnaryExpression )item.Body;
                            var property = ubody.Operand as MemberExpression;
                            if ( property != null ) {
                                props[ property.Member.Name ] = obj.GetType()
                                    .GetProperty( property.Member.Name )
                                    .GetValue( obj, null );
                            }
                            else // full expression with number funcs
                            {
                                var compiled = item.Compile();
                                var result = ( KeyValuePair<string, object> )compiled.Invoke( obj );
                                props[ result.Key ] = result.Value;
                            }
                        }
                    }

                    var json = JsonConvert.SerializeObject( eo );
                    var anon = JsonConvert.DeserializeAnonymousType<object>( json, Activator.CreateInstance<T2>() );
                    return ( ( JObject )anon ).ToObject<T2>();
                }
        */

		/*

                /// <summary>
                /// Returns the total time taken for a simple query. (connect + execute + fetch...)
                /// </summary>
                /// <returns></returns>
                [Obsolete( "No access to a local Server atm." )]
                public static TimeSpan EasyPing( SQLQuery db ) {
                    var stopwatch = Stopwatch.StartNew();
                    try {
                        var stack = new Stack<Object>();
                        db.Params.AddWithValue( "@when", DateTime.Now ).DbType = DbType.DateTime;
                        using ( var reader = db.Query( "[dbo].[HalloWrold]" ) ) {
                            while ( reader.Read() ) {
                                for ( var i = 0 ; i < reader.FieldCount ; i++ ) {
                                    stack.Push( reader.GetFieldValue<Object>( i ) );
                                }

                                //DateTime wesaid;
                                //DateTime theysaid;
                                //if ( DateTime.TryParse( bob[ 0 ].ToString(), out wesaid ) && DateTime.TryParse( bob[ 1 ].ToString(), out theysaid ) ) {
                                //    var differ = TimeSpan.FromTicks( Math.Abs( theysaid.Ticks - wesaid.Ticks ) );
                                //    //Generic.Report( String.Format( "Database ping replied: {0}.", differ.Simple() ) );
                                //}
                            }
                        }
                    }
                    catch ( Exception exception ) {
                        exception.Error();
                    }

                    //Generic.Report( String.Format( "Database ping actual: {0}.", stopwatch.Elapsed.Simple() ) );
                    return stopwatch.Elapsed;
                }
        */

		public static void TryPlayFile( this String fileName ) {
			try {
				using ( var player = new SoundPlayer() ) {
					player.SoundLocation = fileName;
					player.Load();
					player.Play();
				}
			}
			catch ( Exception exception ) {
				exception.More();
			}
		}

		private static T CreateItemFromRow<T>( DataRow row, IEnumerable<PropertyInfo> properties ) {
			//T item = new T();
			var item = Activator.CreateInstance<T>();
			foreach ( var property in properties ) {
				property.SetValue( item, row[ property.Name ], null );
			}
			return item;
		}

		/*
                [Obsolete( "No access to a local Server atm." )]
                public static TimeSpan PingAverage() {
                    var stopwatch = Stopwatch.StartNew();
                    var db = new SQLQuery();
                    var bag = new ConcurrentBag< TimeSpan >();
                    do {
                        bag.Add( EasyPing( db ) ); //hack
                    } while ( stopwatch.Elapsed < Second.Three || bag.Count < 10 );

                    //var list = new List<TimeSpan>( bag.Distinct() );
                    var average = bag.Average( timeSpan => timeSpan.TotalMilliseconds );
                    return TimeSpan.FromMilliseconds( average );
                }
        */

		/*

                /// <summary>
                /// Returns the total time taken for a simple query. (connect + execute + fetch...)
                /// </summary>
                /// <returns></returns>
                [Obsolete( "No access to a local Server atm." )]
                public static TimeSpan Ping() {
                    var stopwatch = Stopwatch.StartNew();
                    try {
                        using ( var query = new SQLQuery( ) ) {
                            query.Params.AddWithValue( "@when", DateTime.Now )
                                 .DbType = DbType.DateTime;
                            using ( var reader = query.Query( "[dbo].[HalloWrold]" ) ) {
                                if ( reader.Read() ) {
                                    DateTime wesaid;
                                    DateTime theysaid;
                                    if ( DateTime.TryParse( reader[ 0 ].ToString(), out wesaid ) && DateTime.TryParse( reader[ 1 ].ToString(), out theysaid ) ) {
                                        var differ = TimeSpan.FromTicks( Math.Abs( theysaid.Ticks - wesaid.Ticks ) );

                                        //Generic.Report( String.Format( "Database ping replied: {0}.", differ.Simple() ) );
                                    }
                                }
                            }
                        }
                    }
                    catch ( Exception exception ) {
                        exception.Error();
                    }

                    //Generic.Report( String.Format( "Database ping actual: {0}.", stopwatch.Elapsed.Simple() ) );
                    return stopwatch.Elapsed;
                }
        */

		public static void StartSqlBrowserService( IEnumerable<String> activeMachines ) {
			var myService = new ServiceController { ServiceName = "SQLBrowser" };

			foreach ( var machine in activeMachines ) {
				try {
					myService.MachineName = machine;
					var svcStatus = myService.Status.ToString();
					switch ( svcStatus ) {
						case "ContinuePending":
							Console.WriteLine( "Service is attempting to continue." );
							break;

						case "Paused":
							Console.WriteLine( "Service is paused." );
							Console.WriteLine( "Attempting to continue the service." );
							myService.Continue();
							break;

						case "PausePending":
							Console.WriteLine( "Service is pausing." );
							Thread.Sleep( 1000 );
							try {
								Console.WriteLine( "Attempting to continue the service." );
								myService.Start();
							}
							catch ( Exception e ) {
								Console.WriteLine( e.Message );
							}
							break;

						case "Running":
							Console.WriteLine( "Service is already running." );
							break;

						case "StartPending":
							Console.WriteLine( "Service is starting." );
							break;

						case "Stopped":
							Console.WriteLine( "Service is stopped." );
							Console.WriteLine( "Attempting to start service." );
							myService.Start();
							break;

						case "StopPending":
							Console.WriteLine( "Service is stopping." );
							Thread.Sleep( 1000 );
							try {
								Console.WriteLine( "Attempting to restart service." );
								myService.Start();
							}
							catch ( Exception e ) {
								Console.WriteLine( e.Message );
							}
							break;
					}
				}
				catch ( Exception e ) {
					Console.WriteLine( e.Message );
				}
			}
		}

		[DebuggerDisplay( "{" + nameof( ToString ) + "(),nq}" )]
		[JsonObject]
		[Immutable]
		public struct SQLServerInstance {

			public String MachineName { get; set; }
			public String ServiceName { get; set; }
			public String InstanceName { get; set; }
			public String Version { get; set; }
			public String Edition { get; set; }

			public String ConnectToThis => $"{this.MachineName}\\{this.InstanceName}";

			public override String ToString() => $"{this.ServiceName} {this.InstanceName} {this.Version} {this.Edition}";

		}

		[Test]
		public static void ListInstances() {
			foreach ( var instance in EnumerateSQLInstances() ) {
				Console.WriteLine( instance );
			}
		}

		/// <summary>
		///     Enumerates all SQL Server instances on the machine.
		/// </summary>
		/// <returns></returns>
		public static IEnumerable<SQLServerInstance> EnumerateSQLInstances() {
			const String query = "select * from SqlServiceAdvancedProperty where SQLServiceType = 1 and PropertyName = \'instanceID\'";
			foreach ( var correctNamespace in GetCorrectWmiNameSpaces() ) {

				var getSqlEngine = new ManagementObjectSearcher( correctNamespace, query );
				try {
					if ( !getSqlEngine.Get().Count.Any() ) {
						yield break;
					}
				}
				catch ( ManagementException ) {
					yield break;
				}
				//Console.WriteLine( "SQL Server database instances discovered :" );
				//Console.WriteLine( "Instance Name \t ServiceName \t Edition \t Version \t" );
				foreach ( var o in getSqlEngine.Get() ) {
					if ( !( o is ManagementObject sqlEngine ) ) {
						continue;
					}
					var serviceName = sqlEngine[ "ServiceName" ].ToString();
					var instanceName = DatabaseExtensions.GetInstanceNameFromServiceName( serviceName );
					var version = DatabaseExtensions.GetWmiPropertyValueForEngineService( serviceName, correctNamespace, "Version" );
					var edition = DatabaseExtensions.GetWmiPropertyValueForEngineService( serviceName, correctNamespace, "SKUNAME" );
					yield return new SQLServerInstance { InstanceName = instanceName, ServiceName = serviceName, Version = version, Edition = edition, MachineName = Environment.MachineName };
				}
			}
		}

		/// <summary>
		///     Method returns the correct SQL namespace to use to detect SQL Server instances.
		/// </summary>
		/// <returns>namespace to use to detect SQL Server instances</returns>
		[ItemNotNull]
		public static IEnumerable<String> GetCorrectWmiNameSpaces() {
			const String root = "root\\Microsoft\\sqlserver";
			var namespaces = new List<String>();
			try {
				// Enumerate all WMI instances of __namespace WMI class.
				var nsClass = new ManagementClass( new ManagementScope( root ), new ManagementPath( "__namespace" ), null );
				namespaces.AddRange( nsClass.GetInstances().OfType<ManagementObject>().Select( ns => ns[ "Name" ].ToString() ) );
			}
			catch ( ManagementException exception ) {
				exception.More();
			}
			foreach ( var ns in namespaces.Where( s => s.StartsWith( "ComputerManagement" ) ) ) {
				yield return root +  "\\" + ns;
			}
		}

		/// <summary>
		/// method extracts the instance name from the service name
		/// </summary>
		/// <param name="serviceName"></param>
		/// <returns></returns>
		public static String GetInstanceNameFromServiceName( String serviceName ) {
			if ( String.IsNullOrEmpty( serviceName ) ) {
				return String.Empty;
			}
			if ( String.Equals( serviceName, "MSSQLSERVER", StringComparison.OrdinalIgnoreCase ) ) {
				return serviceName;
			}
			return serviceName.Substring( serviceName.IndexOf( '$' ) + 1, serviceName.Length - serviceName.IndexOf( '$' ) - 1 );
		}

		/// <summary>
		/// Returns the WMI property value for a given property name for a particular SQL Server service Name
		/// </summary>
		/// <param name="serviceName">The service name for the SQL Server engine serivce to query for</param>
		/// <param name="wmiNamespace">The wmi namespace to connect to </param>
		/// <param name="propertyName">The property name whose value is required</param>
		/// <returns></returns>
		public static String GetWmiPropertyValueForEngineService( [NotNull] String serviceName, [NotNull] String wmiNamespace, [NotNull] String propertyName ) {
			if ( serviceName == null ) {
				throw new ArgumentNullException( paramName: nameof( serviceName ) );
			}
			if ( wmiNamespace == null ) {
				throw new ArgumentNullException( paramName: nameof( wmiNamespace ) );
			}
			if ( propertyName == null ) {
				throw new ArgumentNullException( paramName: nameof( propertyName ) );
			}
			var query = $"select * from SqlServiceAdvancedProperty where SQLServiceType = 1 and PropertyName = '{propertyName}' and ServiceName = '{serviceName}'";
			var propertySearcher = new ManagementObjectSearcher( wmiNamespace, query );
			foreach ( var o in propertySearcher.Get() ) {
				if ( o is ManagementObject managementObject ) {
					return managementObject[ "PropertyStrValue" ].ToString();
				}
			}
			return String.Empty;
		}
	}

}
