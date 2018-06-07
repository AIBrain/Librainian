// Copyright © 1995-2018 to Rick@AIBrain.org and Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
//
// This source code contained in "DatabaseExtensions.cs" belongs to Rick@AIBrain.org and
// Protiguous@Protiguous.com unless otherwise specified or the original license has
// been overwritten by automatic formatting.
// (We try to avoid it from happening, but it does accidentally happen.)
//
// Any unmodified portions of source code gleaned from other projects still retain their original
// license and our thanks goes to those Authors. If you find your code in this source code, please
// let us know so we can properly attribute you and include the proper license and/or copyright.
//
// Donations, royalties from any software that uses any of our code, or license fees can be paid
// to us via bitcoin at the address 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2.
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
// For business inquiries, please contact me at Protiguous@Protiguous.com .
//
// Our software can be found at "https://Protiguous.Software/"
// Our GitHub address is "https://github.com/Protiguous".
// Feel free to browse any source code we might have available.
//
// ***  Project "Librainian"  ***
// File "DatabaseExtensions.cs" was last formatted by Protiguous on 2018/06/04 at 3:49 PM.

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
	using Newtonsoft.Json;
	using NUnit.Framework;
	using Parsing;

	[DebuggerDisplay( "{" + nameof( ToString ) + "(),nq}" )]
	[JsonObject]
	[Immutable]
	public struct SqlServerInstance {

		public String ConnectToThis => $"{this.MachineName}\\{this.InstanceName}";

		public String Edition { get; set; }

		public String InstanceName { get; set; }

		public String MachineName { get; set; }

		public String ServiceName { get; set; }

		public String Version { get; set; }

		public override String ToString() => $"{this.ServiceName} {this.InstanceName} {this.Version} {this.Edition}";
	}

	public static class DatabaseExtensions {

		private static Dictionary<Type, IList<PropertyInfo>> TypeDictionary { get; } = new Dictionary<Type, IList<PropertyInfo>>();

		[CanBeNull]
		private static T CreateItemFromRow<T>( DataRow row, [NotNull] IEnumerable<PropertyInfo> properties ) {
			if ( row is null ) {
				return default;
			}

			if ( properties is null ) { throw new ArgumentNullException( paramName: nameof( properties ) ); }

			//T item = new T();
			var item = Activator.CreateInstance<T>();

			foreach ( var property in properties ) { property.SetValue( item, row[property.Name], null ); }

			return item;
		}

		/// <summary>
		///     Enumerates all SQL Server instances on the machine.
		/// </summary>
		/// <returns></returns>
		public static IEnumerable<SqlServerInstance> EnumerateSqlInstances() {
			const String query = "select * from SqlServiceAdvancedProperty where SQLServiceType = 1 and PropertyName = \'instanceID\'";

			foreach ( var correctNamespace in GetCorrectWmiNameSpaces() ) {

				var getSqlEngine = new ManagementObjectSearcher( correctNamespace, query );

				try {
					if ( !getSqlEngine.Get().Count.Any() ) { yield break; }
				}
				catch ( ManagementException ) { yield break; }

				//Console.WriteLine( "SQL Server database instances discovered :" );
				//Console.WriteLine( "Instance Name \t ServiceName \t Edition \t Version \t" );
				foreach ( var o in getSqlEngine.Get() ) {
					if ( !( o is ManagementObject sqlEngine ) ) { continue; }

					var serviceName = sqlEngine["ServiceName"].ToString();
					var instanceName = GetInstanceNameFromServiceName( serviceName );
					var version = GetWmiPropertyValueForEngineService( serviceName, correctNamespace, "Version" );
					var edition = GetWmiPropertyValueForEngineService( serviceName, correctNamespace, "SKUNAME" );

					yield return new SqlServerInstance { InstanceName = instanceName, ServiceName = serviceName, Version = version, Edition = edition, MachineName = Environment.MachineName };
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
				namespaces.AddRange( nsClass.GetInstances().OfType<ManagementObject>().Select( ns => ns["Name"].ToString() ) );
			}
			catch ( ManagementException exception ) { exception.More(); }

			foreach ( var ns in namespaces.Where( s => s.StartsWith( "ComputerManagement" ) ) ) { yield return root + "\\" + ns; }
		}

		/// <summary>
		///     method extracts the instance name from the service name
		/// </summary>
		/// <param name="serviceName"></param>
		/// <returns></returns>
		public static String GetInstanceNameFromServiceName( String serviceName ) {
			if ( String.IsNullOrEmpty( serviceName ) ) { return String.Empty; }

			if ( serviceName.Like( "MSSQLSERVER" ) ) { return serviceName; }

			return serviceName.Substring( serviceName.IndexOf( '$' ) + 1, serviceName.Length - serviceName.IndexOf( '$' ) - 1 );
		}

		public static IList<PropertyInfo> GetPropertiesForType<T>() {
			var type = typeof( T );

			if ( !TypeDictionary.ContainsKey( typeof( T ) ) ) { TypeDictionary.Add( type, type.GetProperties().ToList() ); }

			return TypeDictionary[type];
		}

		/// <summary>
		///     Returns the WMI property value for a given property name for a particular SQL Server service Name
		/// </summary>
		/// <param name="serviceName"> The service name for the SQL Server engine serivce to query for</param>
		/// <param name="wmiNamespace">The wmi namespace to connect to</param>
		/// <param name="propertyName">The property name whose value is required</param>
		/// <returns></returns>
		public static String GetWmiPropertyValueForEngineService( [NotNull] String serviceName, [NotNull] String wmiNamespace, [NotNull] String propertyName ) {
			if ( serviceName is null ) { throw new ArgumentNullException( nameof( serviceName ) ); }

			if ( wmiNamespace is null ) { throw new ArgumentNullException( nameof( wmiNamespace ) ); }

			if ( propertyName is null ) { throw new ArgumentNullException( nameof( propertyName ) ); }

			var query = $"select * from SqlServiceAdvancedProperty where SQLServiceType = 1 and PropertyName = '{propertyName}' and ServiceName = '{serviceName}'";
			var propertySearcher = new ManagementObjectSearcher( wmiNamespace, query );

			foreach ( var o in propertySearcher.Get() ) {
				if ( o is ManagementObject managementObject ) { return managementObject["PropertyStrValue"].ToString(); }
			}

			return String.Empty;
		}

		[Test]
		public static void ListInstances() {
			foreach ( var instance in EnumerateSqlInstances() ) { Console.WriteLine( instance ); }
		}

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
							catch ( Exception e ) { Console.WriteLine( e.Message ); }

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
							catch ( Exception e ) { Console.WriteLine( e.Message ); }

							break;
					}
				}
				catch ( Exception e ) { Console.WriteLine( e.Message ); }
			}
		}

		/// <summary>
		///     Convert our IList to a DataSet
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="list"></param>
		/// <returns>DataSet</returns>
		/// <copyright>
		///     Based from http://codereview.stackexchange.com/q/40891
		/// </copyright>
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
		/// <copyright>
		///     Based from http://codereview.stackexchange.com/q/40891
		/// </copyright>
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
					newRow[propInfo.Name] = item; // DBNull.Value; //ival ??

					// } catch ( Exception exception) { Debug.WriteLine( exception.Message ); }
					t.Rows.Add( newRow );
				}
			}

			return t;
		}

		public static DataTable ToDataTable( this SqlDataReader dataReader ) {
			var table = new DataTable();
			table.BeginLoadData();

			if ( dataReader != null ) { table.Load( dataReader, LoadOption.OverwriteChanges ); }

			table.EndLoadData();

			return table;
		}

		public static IEnumerable<T> ToList<T>( this DataTable table ) {
			var properties = GetPropertiesForType<T>();
			IList<T> result = new List<T>();

			foreach ( var row in table.Rows ) {
				var item = CreateItemFromRow<T>( row as DataRow, properties );
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
                /// <para>"Attempting below to get a fluent Object to DTO builder - which fails when a property is missed."</para>
                /// </summary>
                /// <typeparam name="T1"></typeparam>
                /// <typeparam name="T2"></typeparam>
                /// <param name="obj">  </param>
                /// <param name="items"></param>
                /// <returns></returns>
                /// <copyright>
                ///     Based from http://codereview.stackexchange.com/q/69359
                /// </copyright>
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
			catch ( Exception exception ) { exception.More(); }
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
	}
}