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
// File "DatabaseExtensions.cs" last touched on 2021-08-15 at 5:39 AM by Protiguous.

#nullable enable

namespace Librainian.Databases {

	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.ComponentModel;
	using System.Data;
	using System.Data.Common;
	using System.Diagnostics;
	using System.Diagnostics.CodeAnalysis;
	using System.Globalization;
	using System.Linq;
	using System.Management;
	using System.Reflection;
	using System.ServiceProcess;
	using System.Threading;
	using System.Threading.Tasks;
	using Exceptions;
	using JetBrains.Annotations;
	using Logging;
	using Maths;
	using Measurement.Time;
	using Microsoft.Data.SqlClient;
	using Microsoft.SqlServer.Management.Smo;
	using Parsing;
	using Persistence;
	using PooledAwait;
	using FieldDictionary = System.Collections.Generic.Dictionary<System.String, System.Int32>;

	public static class DatabaseExtensions {

		private static Dictionary<Type, IList<PropertyInfo>> TypeDictionary { get; } = new();

		/// <summary>
		///     Return a dictionary of fields and their index.
		/// </summary>
		/// <param name="reader"></param>
		private static FieldDictionary GetFieldNames( this IDataReader reader ) {
			var dictionary = new FieldDictionary( reader.FieldCount, StringComparer.OrdinalIgnoreCase );

			if ( !reader.IsClosed ) {
				for ( var i = 0; i < reader.FieldCount; i++ ) {
					dictionary.Add( reader.GetName( i ), i );
				}
			}

			return dictionary;
		}

		[return: System.Diagnostics.CodeAnalysis.NotNull]
		public static T? Adhoc<T>( this SqlConnectionStringBuilder builderToTest, String command, CancellationToken cancellationToken ) {
			if ( String.IsNullOrWhiteSpace( command ) ) {
				throw new ArgumentException( "Value cannot be null or whitespace.", nameof( command ) );
			}

			try {
				using var db = new DatabaseServer( builderToTest.ConnectionString, null, cancellationToken, cancellationToken );

				return db.ExecuteScalar<T>( command, CommandType.Text );
			}
			catch ( Exception exception ) {
				exception.Log();

				throw;
			}
		}

		public static async PooledValueTask<T?> AdhocAsync<T>(
			this SqlConnectionStringBuilder builderToTest,
			String query,
			CancellationToken connectCancellationToken,
			CancellationToken executeCancellationToken
		) {
			if ( String.IsNullOrWhiteSpace( query ) ) {
				throw new ArgumentException( "Value cannot be null or whitespace.", nameof( query ) );
			}

			try {
				using var db = new DatabaseServer( builderToTest.ConnectionString, null, connectCancellationToken, executeCancellationToken );

				return await db.ExecuteScalarAsync<T>( query, CommandType.Text, executeCancellationToken ).ConfigureAwait( false );
			}
			catch ( Exception exception ) {
				exception.Log();

				throw;
			}
		}

		/*

        /// <summary>
        /// Converts a DataTable to a list with generic objects
        /// </summary>
        /// <typeparam name="T">Generic object</typeparam>
        /// <param name="table">DataTable</param>
        /// <returns>List with generic objects</returns>
        [NotNull]
        public static List<T> DataTableToList<T>( [NotNull] this DataTable table ) where T : class, new() {
            if ( table is null ) {
                throw new ArgumentEmptyException( nameof( table ) );
            }

            var list = new List<T>( table.Rows.Count );

            try {
                foreach ( var row in table.AsEnumerable() ) {
                    var obj = new T();

                    foreach ( var prop in obj.GetType().GetProperties() ) {
                        try {
                            var propertyInfo = obj.GetType().GetProperty( prop.Name );
                            propertyInfo?.SetValue( obj, Convert.ChangeType( row[ prop.Name ], propertyInfo.PropertyType ), null );
                        }
                        catch { }
                    }

                    list.Add( obj );
                }

                list.TrimExcess();
            }
            catch { }

            return list;
        }
		*/

		/// <summary>
		///     Enumerates all SQL Server instances on the machine.
		/// </summary>
		public static IEnumerable<SqlServerInstance> EnumerateSqlInstances() {
			const String query = @"select * from SqlServiceAdvancedProperty where SQLServiceType = 1 and PropertyName = 'instanceID'";

			foreach ( var correctNamespace in GetCorrectWmiNameSpaces() ) {
				using var getSqlEngine = new ManagementObjectSearcher( correctNamespace, query );

				try {
					if ( !getSqlEngine.Get().Count.Any() ) {
						yield break;
					}
				}
				catch ( ManagementException ) {
					yield break;
				}

				foreach ( var serviceName in getSqlEngine.Get().Cast<ManagementObject>().Select( sqlEngine => sqlEngine["ServiceName"]?.ToString() ) ) {
					if ( !String.IsNullOrWhiteSpace( serviceName ) ) {
						var instance = new SqlServerInstance {
							ServiceName = serviceName,
							MachineName = Environment.MachineName,
							InstanceName = GetInstanceNameFromServiceName( serviceName ),
							Edition = GetWmiPropertyValueForEngineService( serviceName, correctNamespace, "SKUNAME" )
						};

						if ( Version.TryParse( GetWmiPropertyValueForEngineService( serviceName, correctNamespace, "Version" ), out var version ) ) {
							instance.Version = version;
						}

						yield return instance;
					}
				}
			}
		}

		/// <summary>
		///     Add the <paramref name="connectionString" /> to the <paramref name="file" /> under the given
		///     <paramref name="key" />.
		///     <para>Returns the key.</para>
		/// </summary>
		/// <param name="file">            </param>
		/// <param name="connectionString"></param>
		/// <param name="key">             </param>
		public static async Task<String> FileSet( this ConcurrentDictionaryFile<String, String> file, String connectionString, String key = Words.PrimeConnectionString ) {
			if ( file is null ) {
				throw new ArgumentEmptyException( nameof( file ) );
			}

			if ( String.IsNullOrWhiteSpace( key ) ) {
				throw new ArgumentException( "Value cannot be null or whitespace.", nameof( key ) );
			}

			if ( String.IsNullOrWhiteSpace( connectionString ) ) {
				throw new ArgumentException( "Value cannot be null or whitespace.", nameof( connectionString ) );
			}

			file[key] = connectionString;
			await file.Flush( CancellationToken.None ).ConfigureAwait( false );

			Debug.Assert( file[key].Like( connectionString ) );

			return key;
		}

		public static async Task<Boolean> FileSet(
			this ConcurrentDictionaryFile<String, String> file,
			SqlServerInfo sqlServerInfo,
			String key = Words.PrimeConnectionString
		) {
			if ( file is null ) {
				throw new ArgumentEmptyException( nameof( file ) );
			}

			if ( sqlServerInfo is null ) {
				throw new ArgumentEmptyException( nameof( sqlServerInfo ) );
			}

			if ( sqlServerInfo.Status != Status.Success ) {
				return false;
			}

			var connectionStringBuilder = sqlServerInfo.ConnectionStringBuilder;

			if ( connectionStringBuilder == null ) {
				return false;
			}

			var connectionString = connectionStringBuilder.ConnectionString;

			await file.FileSet( connectionString, key ).ConfigureAwait( false );

			return file.Get( key ).Like( connectionString );
		}

		public static String? Get( this ConcurrentDictionaryFile<String, String> file, String key = Words.PrimeConnectionString, Boolean throwIfNotFound = true ) {
			if ( file is null ) {
				throw new ArgumentEmptyException( nameof( file ) );
			}

			if ( String.IsNullOrWhiteSpace( key ) ) {
				throw new ArgumentException( "Value cannot be null or whitespace.", nameof( key ) );
			}

			if ( file.TryGetValue( key, out var connection ) ) {
				if ( !String.IsNullOrWhiteSpace( connection ) ) {
					return connection;
				}
			}

			if ( throwIfNotFound ) {
				throw new InvalidOperationException( "No usable connection string." );
			}

			return default( String? );
		}

		/// <summary>
		///     Method returns the correct SQL namespace to use to detect SQL Server instances.
		/// </summary>
		/// <returns>namespace to use to detect SQL Server instances</returns>
		public static IEnumerable<String> GetCorrectWmiNameSpaces() {
			const String root = "root\\Microsoft\\sqlserver";

			var namespaces = new List<String>();

			try {
				// Enumerate all WMI instances of __namespace WMI class.
				var objectGetOptions = new ObjectGetOptions {
					Timeout = Seconds.Ten
				};

				using var nsClass = new ManagementClass( new ManagementScope( root ), new ManagementPath( "__namespace" ), objectGetOptions );

				var items = nsClass.GetInstances().OfType<ManagementObject>().Select( ns => ns["Name"]?.ToString() );

				foreach ( var item in items ) {
					if ( !String.IsNullOrWhiteSpace( item ) ) {
						namespaces.Add( item );
					}
				}
			}
			catch ( ManagementException exception ) {
				exception.Log();
			}

			foreach ( var ns in namespaces.Where( s => s.StartsWith( "ComputerManagement", true, CultureInfo.CurrentCulture ) ) ) {
				yield return $@"{root}\{ns}";
			}
		}

		public static dynamic? GetDataSources() {
			//var services = ServiceController.GetServices().Where( service => service.ServiceName.StartsWith( "MSSQL$" ) );
			//return services;

			var availableSqlServers = SmoApplication.EnumAvailableSqlServers();

			return availableSqlServers;
		}

		/// <summary>
		///     method extracts the instance name from the service name
		/// </summary>
		/// <param name="serviceName"></param>
		public static String GetInstanceNameFromServiceName( String? serviceName ) {
			if ( String.IsNullOrEmpty( serviceName ) ) {
				return String.Empty;
			}

			if ( serviceName.Like( "MSSQLSERVER" ) ) {
				return serviceName;
			}

			var dollar = serviceName.IndexOf( '$' );
			return serviceName.Substring( dollar + 1, serviceName.Length - dollar - 1 );
		}

		public static IList<PropertyInfo> GetPropertiesForType<T>() {
			var type = typeof( T );

			if ( !TypeDictionary.ContainsKey( typeof( T ) ) ) {
				TypeDictionary.Add( type, type.GetProperties() );
			}

			return TypeDictionary[type];
		}

		/// <summary>
		///     Returns the WMI property value for a given property name for a particular SQL Server service Name
		/// </summary>
		/// <param name="serviceName"> The service name for the SQL Server engine serivce to query for</param>
		/// <param name="wmiNamespace">The wmi namespace to connect to</param>
		/// <param name="propertyName">The property name whose value is required</param>
		public static String GetWmiPropertyValueForEngineService( String serviceName, String wmiNamespace, String propertyName ) {
			if ( serviceName is null ) {
				throw new ArgumentEmptyException( nameof( serviceName ) );
			}

			if ( wmiNamespace is null ) {
				throw new ArgumentEmptyException( nameof( wmiNamespace ) );
			}

			if ( propertyName is null ) {
				throw new ArgumentEmptyException( nameof( propertyName ) );
			}

			var query = $"select * from SqlServiceAdvancedProperty where SQLServiceType = 1 and PropertyName = '{propertyName}' and ServiceName = '{serviceName}'";
			using var propertySearcher = new ManagementObjectSearcher( wmiNamespace, query );

			foreach ( var o in propertySearcher.Get() ) {
				if ( o is ManagementObject managementObject ) {
					return managementObject["PropertyStrValue"].ToString();
				}
			}

			return String.Empty;
		}


		public static Boolean AttemptQueryAgain<T>( this T exception, ref Int32 retriesLeft ) where T : Exception {
			if ( !retriesLeft.Any() || exception is TaskCanceledException ) {
				return false;
			}

			--retriesLeft;

			switch ( exception ) {
				case SqlException
				{
					IsTransient: true
				}:
					return true;
				case DbException
				{
					IsTransient: true
				}:
					return true;
				case SqlException
				{
					Errors: { }
				} sqlxException: {
						foreach ( SqlError error in sqlxException.Errors ) {
							if ( error.Message?.Contains( "Execution Timeout Expired", StringComparison.CurrentCultureIgnoreCase ) == true ) {
								return true;
							}
						}

						break;
					}
			}

			if ( exception.Message.Contains( "deadlocked", StringComparison.CurrentCultureIgnoreCase ) ) {
				if ( exception.Message.Contains( "Rerun the transaction", StringComparison.CurrentCultureIgnoreCase ) ) {
					return true;
				}
			}

			return exception.Message.Contains( "server was not found", StringComparison.CurrentCultureIgnoreCase ) ||
				   exception.Message.Contains( "was not accessible", StringComparison.CurrentCultureIgnoreCase ) ||
				   exception.Message.Contains( "timed out", StringComparison.CurrentCultureIgnoreCase ) ||
				   exception.Message.Contains( "requires an open and available Connection", StringComparison.CurrentCultureIgnoreCase ) || exception.HResult == -2146233079;
		}

		public static async ValueTask<(Status, String?)> TestDatabaseConnectionString( String connectionString, CancellationToken cancellationToken ) {
			if ( String.IsNullOrWhiteSpace( connectionString ) ) {
				throw new ArgumentException( "Value cannot be null or whitespace.", nameof( connectionString ) );
			}

			var builder = new SqlConnectionStringBuilder( connectionString );

			var retriesLeft = DatabaseServer.DefaultRetries;
		TryAgain:
			try {
				var sqlServer = await builder.TryGetResponse( cancellationToken, cancellationToken ).ConfigureAwait( false );

				if ( sqlServer?.Status.IsGood() == true ) {
					if ( sqlServer.ConnectionStringBuilder != null ) {
						return (sqlServer.Status, sqlServer.ConnectionStringBuilder.ConnectionString);
					}
				}
			}
			catch ( Exception exception ) {
				if ( exception.AttemptQueryAgain( ref retriesLeft ) ) {
					await Task.Delay( Seconds.One, cancellationToken ).ConfigureAwait( false );
					goto TryAgain;
				}

				exception.Log();
			}

			return (Status.Failure, default( String? ));
		}

		/// <summary>
		///     Convert our IList to a DataSet
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="list">      </param>
		/// <param name="exampleSet"></param>
		/// <returns>DataSet</returns>
		/// <copyright>
		///     Based from http://codereview.stackexchange.com/q/40891
		/// </copyright>
		public static DataSet ToDataSet<T>( this IEnumerable<T> list, DataSet? exampleSet = null ) {
			if ( list is null ) {
				throw new ArgumentEmptyException( nameof( list ) );
			}

			var dataSet = new DataSet {
				Tables = {
					list.ToDataTable()
				},
				Namespace = exampleSet?.Namespace,
				CaseSensitive = exampleSet?.CaseSensitive == true,
				EnforceConstraints = exampleSet?.EnforceConstraints == true,
				DataSetName = exampleSet?.DataSetName ?? list.GetHashCode().ToString(),
				Locale = exampleSet?.Locale ?? Thread.CurrentThread.CurrentCulture,
				Prefix = exampleSet?.Prefix,
				RemotingFormat = exampleSet?.RemotingFormat switch {
					SerializationFormat.Binary => SerializationFormat.Binary,
					SerializationFormat.Xml => SerializationFormat.Xml,
					null => SerializationFormat.Xml,
					var _ => throw new ArgumentOutOfRangeException( $"Invalid {nameof( exampleSet.RemotingFormat )}." )
				},
				SchemaSerializationMode = exampleSet?.SchemaSerializationMode switch {
					SchemaSerializationMode.ExcludeSchema => SchemaSerializationMode.ExcludeSchema,
					SchemaSerializationMode.IncludeSchema => SchemaSerializationMode.IncludeSchema,
					null => SchemaSerializationMode.IncludeSchema,
					var _ => throw new ArgumentOutOfRangeException( $"Invalid {nameof( exampleSet.SchemaSerializationMode )}." )
				},
				Site = exampleSet?.Site
			};

			if ( exampleSet is null ) {
				return dataSet;
			}

			if ( exampleSet.Container is not null && dataSet.Container is not null ) {
				foreach ( IComponent? o in exampleSet.Container.Components ) {
					dataSet.Container.Add( o );
				}
			}

			foreach ( DictionaryEntry entry in exampleSet.ExtendedProperties ) {
				dataSet.ExtendedProperties.Add( entry.Key, entry.Value );
			}

			foreach ( DataRelation relation in exampleSet.Relations ) {
				dataSet.Relations.Add( relation );
			}

			var i = 0;

			foreach ( DataViewSetting dataViewSetting in exampleSet.DefaultViewManager.DataViewSettings ) {
				dataSet.DefaultViewManager.DataViewSettings[i++] = dataViewSetting;
			}

			return dataSet;
		}

		public static DataSet ToDataSet<T>( this IEnumerable<T> collection ) {
			var dts = new DataSet();

			var properties = TypeDescriptor.GetProperties( typeof( T ) );
			DataTable table = new();
			foreach ( PropertyDescriptor prop in properties ) {
				table.Columns.Add( prop.Name, Nullable.GetUnderlyingType( prop.PropertyType ) ?? prop.PropertyType );
			}

			foreach ( var item in collection ) {
				if ( item is null or DBNull ) {
					continue;
				}

				var row = table.NewRow();
				foreach ( PropertyDescriptor prop in properties ) {
					row[prop.Name] = prop.GetValue( item ) ?? DBNull.Value;
				}

				table.Rows.Add( row );
			}

			dts.Tables.Add( table );

			return dts;
		}

		public static DataTable ToDataTable<T>( this IEnumerable<T> collection ) {
			DataTable table = new();

			var properties = TypeDescriptor.GetProperties( typeof( T ) );
			foreach ( PropertyDescriptor prop in properties ) {
				table.Columns.Add( prop.Name, Nullable.GetUnderlyingType( prop.PropertyType ) ?? prop.PropertyType );
			}

			foreach ( var item in collection ) {
				if ( item is null or DBNull ) {
					continue;
				}

				var row = table.NewRow();
				foreach ( PropertyDescriptor prop in properties ) {
					row[prop.Name] = prop.GetValue( item ) ?? DBNull.Value;
				}

				table.Rows.Add( row );
			}

			return table;
		}

		/// <summary>
		///     To allow disconnecting the <see cref="SqlDataReader" /> as soon as possible.
		/// </summary>
		/// <param name="dataReader"></param>
		public static DataTable ToDataTable( this SqlDataReader dataReader ) {
			using var table = new DataTable();
			table.BeginLoadData();
			table.Load( dataReader, LoadOption.OverwriteChanges, ( _, _ ) => $"Error reading {nameof( dataReader )}.".Log() );
			table.EndLoadData();

			return table;
		}

		/// <summary>
		///     <para>Warning: Untested and possibly buggy.</para>
		///     Convert our <paramref name="list" /> to a <see cref="DataTable" />.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="list"></param>
		/// <returns>DataTable</returns>
		/// <copyright>
		///     Based from http://codereview.stackexchange.com/q/40891
		/// </copyright>
		public static DataTable ToDataTableBuggy<T>( this IEnumerable<T> list ) {
			if ( list is null ) {
				throw new ArgumentEmptyException( nameof( list ) );
			}

			using var table = new DataTable();

			var properties = list.GetType().GetProperties();

			foreach ( var propInfo in properties ) {
				var propertyType = propInfo.PropertyType;
				var colType = Nullable.GetUnderlyingType( propertyType ) ?? propertyType;
				table.Columns.Add( propInfo.Name, colType );
			}

			foreach ( var item in list ) {
				var newRow = table.NewRow();

				foreach ( var propInfo in properties ) {
					newRow[propInfo.Name] = item; //BUG This does not look right??
				}

				table.Rows.Add( newRow );
			}

			return table;
		}

		/// <summary>
		///     Warning: Untested.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="list"></param>
		public static DataTable ToDataTable3<T>( this IEnumerable<T> list ) {
			"Untested code!".Break();
			using var table = new DataTable();

			var columns = list.GetType().GetProperties();

			foreach ( var getProperty in columns ) {
				var icolType = getProperty.PropertyType;

				if ( icolType.IsGenericType && icolType.GetGenericTypeDefinition() == typeof( Nullable<> ) ) {
					icolType = icolType.GetGenericArguments()[0];
				}

				table.Columns.Add( new DataColumn( getProperty.Name, icolType ) );
			}

			foreach ( var record in list.Where( record => record is not null ) ) {
				var dr = table.NewRow();

				foreach ( var p in columns ) {
					dr[p.Name] = p.GetValue( record, default( Object?[]? ) ) ?? DBNull.Value;
				}

				table.Rows.Add( dr );
			}

			return table;
		}

		/*
        public static DataTable ToDataTable<T>(this IEnumerable<T> source)
        {
            DataTable table = new DataTable();

            //// get properties of T
            var binding = BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetProperty;
            var options = PropertyReflectionOptions.IgnoreEnumerable | PropertyReflectionOptions.IgnoreIndexer;

            var properties = ReflectionExtensions.GetProperties<T>(binding, options).ToList();

            //// create table schema based on properties
            foreach (var property in properties)
            {
                table.Columns.Add(property.Name, property.PropertyType);
            }

            //// create table data from T instances
            object[] values = new object[properties.Count];

            foreach (T item in source)
            {
                for (int i = 0; i < properties.Count; i++)
                {
                    values[i] = properties[i].GetValue(item, null);
                }

                table.Rows.Add(values);
            }

            return table;
        }
        */

		/*
		[NotNull]
		public static IEnumerable<T> ToList<T>( [NotNull] this DataTable table ) {
			if ( table is null ) {
				throw new ArgumentEmptyException( nameof( table ) );
			}

			var properties = GetPropertiesForType<T>();

			foreach ( var row in table.Rows.Cast<DataRow>() ) {
				if ( row != null ) {
					yield return row.CreateItemFromRow<T>( properties );
				}
			}
		}
		*/

		public static SqlParameter ToSqlParameter<TValue>( [DisallowNull] this TValue value, String parameterName ) {
			if ( value is null ) {
				throw new ArgumentEmptyException( nameof( value ) );
			}

			if ( String.IsNullOrEmpty( parameterName ) ) {
				throw new ArgumentException( "Value cannot be null or empty.", nameof( parameterName ) );
			}

			return new SqlParameter( parameterName, value ) {
				Value = value
			};
		}

		public static SqlParameter ToSqlParameter( this SqlDbType sqlDbType, String parameterName, Int32 size ) {
			if ( String.IsNullOrWhiteSpace( parameterName ) ) {
				throw new ArgumentException( "Value cannot be null or whitespace.", nameof( parameterName ) );
			}

			return new( parameterName, sqlDbType, size );
		}

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

		/// <summary>
		///     Performs two adhoc selects on the database.
		///     <code>select @@VERSION;" and "select SYSUTCDATETIME();</code>
		/// </summary>
		/// <param name="test">             </param>
		/// <param name="connectCancellationToken"></param>
		/// <param name="executeCancellationToken"></param>
		public static async ValueTask<SqlServerInfo?> TryGetResponse(
			this SqlConnectionStringBuilder test,
			CancellationToken connectCancellationToken,
			CancellationToken executeCancellationToken
		) {
			if ( test is null ) {
				throw new ArgumentEmptyException( nameof( test ) );
			}

			try {
				var version = await test.AdhocAsync<String>( "select @@version;", connectCancellationToken, executeCancellationToken ).ConfigureAwait( false );

				if ( String.IsNullOrWhiteSpace( version ) ) {
					$"Failed connecting to server {test.DataSource}.".Verbose();

					return default( SqlServerInfo? );
				}

				var getdate = await test.AdhocAsync<DateTime?>( "select sysutcdatetime();", connectCancellationToken, executeCancellationToken ).ConfigureAwait( false );

				if ( !getdate.HasValue ) {
					$"Failed connecting to server {test.DataSource}.".Verbose();

					return default( SqlServerInfo? );
				}

				var serverDateTime = getdate.Value; //should already be utc.
				var now = DateTime.UtcNow; //get this computer's utc

				if ( serverDateTime.Date == now.Date ) {
					( $"Opened a connection to {test.DataSource}!" + $"{Environment.NewLine}Server Version:{version}" +
					  $"{Environment.NewLine}Server time is {serverDateTime.ToLocalTime()}" ).Log();

					var connectionStringBuilder = new SqlConnectionStringBuilder( test.ConnectionString );

					return new SqlServerInfo {
						Status = Status.Success,
						ConnectionStringBuilder = connectionStringBuilder,
						Version = version,
						UTCDateTime = serverDateTime
					};
				}
			}
			catch ( TaskCanceledException exception ) {
				exception.Log( BreakOrDontBreak.DontBreak );
			}

			"Failed connecting to server.".Break();

			return default( SqlServerInfo? );
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