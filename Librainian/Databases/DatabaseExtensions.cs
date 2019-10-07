// Copyright © Rick@AIBrain.org and Protiguous. All Rights Reserved.
// 
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
// 
// This source code contained in "DatabaseExtensions.cs" belongs to Protiguous@Protiguous.com and
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
// Project: "Librainian", "DatabaseExtensions.cs" was last formatted by Protiguous on 2019/10/06 at 9:32 AM.

namespace Librainian.Databases {

    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Sql;
    using System.Data.SqlClient;
    using System.Diagnostics;
    using System.Linq;
    using System.Management;
    using System.Media;
    using System.Reflection;
    using System.Runtime.Serialization;
    using System.Threading;
    using System.Threading.Tasks;
    using Collections.Extensions;
    using Internet;
    using JetBrains.Annotations;
    using Logging;
    using Maths;
    using Measurement.Time;
    using Parsing;
    using Persistence;
    using Threading;
    using static Persistence.Cache;
    using Fields = System.Collections.Generic.Dictionary<System.String, System.Int32>;

    public static class DatabaseExtensions {

        private static Dictionary<Type, IList<PropertyInfo>> TypeDictionary { get; } = new Dictionary<Type, IList<PropertyInfo>>();

        [CanBeNull]
        private static T CreateItemFromRow<T>( [CanBeNull] DataRow row, [NotNull] IEnumerable<PropertyInfo> properties ) {
            if ( row == null ) {
                return default;
            }

            if ( properties == null ) {
                throw new ArgumentNullException( paramName: nameof( properties ) );
            }

            //T item = new T();
            //var item = Activator.CreateInstance<T>(); //TODO use the faster creation function
            //var t = Expression.Lambda<Func<T>>( Expression.New( typeof( T ) ) ).Compile();
            var item = ( T ) FormatterServices.GetUninitializedObject( typeof( T ) );

            foreach ( var property in properties ) {
                property.SetValue( item, row[ property.Name ], null );
            }

            return item;
        }

        /// <summary>
        ///     Cache the fields in this <paramref name="reader" /> for 1 minute.
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        [NotNull]
        private static Fields GetDict( [NotNull] this IDataReader reader ) {
            if ( reader == null ) {
                throw new ArgumentNullException( paramName: nameof( reader ) );
            }

            var key = reader.Key();

            if ( !( Recall( key ) is Fields fields ) ) {
                fields = reader.GetFieldNames();
                Remember( key, fields, Sliding.Minutes( 1 ) );
            }

            return fields;
        }

        /// <summary>
        ///     Return a dictionary of fields and their index.
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        [NotNull]
        private static Fields GetFieldNames( [NotNull] this IDataReader reader ) {
            if ( reader == null ) {
                throw new ArgumentNullException( paramName: nameof( reader ) );
            }

            var dictionary = new Fields( reader.FieldCount, StringComparer.OrdinalIgnoreCase );

            if ( !reader.IsClosed ) {
                for ( var i = 0; i < reader.FieldCount; i++ ) {
                    dictionary.Add( reader.GetName( i ), i );
                }
            }

            return dictionary;
        }

        /// <summary>
        ///     Get a key for this <paramref name="reader" />.
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        [NotNull]
        private static String Key( [NotNull] this IDataReader reader ) {
            if ( reader == null ) {
                throw new ArgumentNullException( paramName: nameof( reader ) );
            }

            return BuildKey( reader.GetHashCode(), reader.FieldCount.GetHashCode(), reader.Depth, reader.RecordsAffected, reader.FieldCount, reader.IsClosed );
        }

        public static async Task<(Status status, T response, TimeSpan responseTime)> AdhocCommand<T>( [NotNull] this SqlConnectionStringBuilder builderToTest,
            [NotNull] String command, CancellationToken? token = null ) {

            if ( builderToTest == default ) {
                throw new ArgumentNullException( paramName: nameof( builderToTest ) );
            }

            if ( String.IsNullOrWhiteSpace( value: command ) ) {
                throw new ArgumentException( paramName: nameof( command ), message: "Value cannot be null or whitespace." );
            }

            var stopwatch = Stopwatch.StartNew();

            try {
                using ( var db = new Database( builderToTest.ConnectionString, token ) ) {
                    var result = await db.ExecuteScalarAsync<T>( command, CommandType.Text ).ConfigureAwait( false );

                    return ( result.status, result.result, stopwatch.Elapsed );
                }
            }
            catch ( Exception exception ) {
                exception.Log();
            }

            return ( Status.Failure, default, stopwatch.Elapsed );
        }

        /// <summary>
        ///     Converts a DataTable to a list with generic objects
        /// </summary>
        /// <typeparam name="T">Generic object</typeparam>
        /// <param name="table">DataTable</param>
        /// <returns>List with generic objects</returns>
        [NotNull]
        public static List<T> DataTableToList<T>( this DataTable table ) where T : class, new() {
            try {
                var list = new List<T>( table.Rows.Count );

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

                return list;
            }
            catch {
                return null;
            }
        }

        public static void DisplayTable( [NotNull] this DataTable table ) {
            if ( table == null ) {
                throw new ArgumentNullException( paramName: nameof( table ) );
            }

            foreach ( DataRow row in table.Rows ) {
                foreach ( DataColumn dataColumn in table.Columns ) {
                    Debug.WriteLine( "{0} = {1}", dataColumn.ColumnName, row[ dataColumn ] );
                }

                Debug.WriteLine( String.Empty );
            }
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
                    var instanceName = GetInstanceNameFromServiceName( serviceName );
                    var version = GetWmiPropertyValueForEngineService( serviceName, correctNamespace, "Version" );
                    var edition = GetWmiPropertyValueForEngineService( serviceName, correctNamespace, "SKUNAME" );

                    yield return new SqlServerInstance {
                        InstanceName = instanceName, ServiceName = serviceName, Version = version, Edition = edition, MachineName = Environment.MachineName
                    };
                }
            }
        }

        /// <summary>
        ///     //TODO Make this better later on.. just return any working connection for now..
        /// </summary>
        /// <param name="timeout"></param>
        /// <param name="credentials"></param>
        /// <returns></returns>
        [ItemCanBeNull]
        public static async Task<IEnumerable<SqlServer>> FindUsableServers( this TimeSpan timeout, [NotNull] params Credentials[] credentials ) {
            if ( credentials == null ) {
                throw new ArgumentNullException( paramName: nameof( credentials ) );
            }

            if ( credentials.Any( c => c == default ) ) {
                throw new ArgumentNullException( paramName: nameof( credentials ) );
            }

            try {
                var token = new CancellationTokenSource( timeout ).Token;

                var list = credentials.Where( c => c != default ).SelectMany( credential => credential.LookForAnyDatabases( timeout ) )
                    .Select( builder => builder.TryGetResponse( token ) ).ToList();

                await Task.WhenAny( list ).ConfigureAwait( false );

                return list.Where( builder => builder.IsDone() && builder.Result.Status == Status.Success ).Select( task => task.Result );
            }
            catch ( Exception exception ) {
                exception.Log();
            }

            return default;
        }

        [CanBeNull]
        public static String Get( [NotNull] this ConcurrentDictionaryFile<String, String> file, [NotNull] String key = Words.PrimeConnectionString,
            Boolean throwIfNotFound = false ) {
            if ( file == null ) {
                throw new ArgumentNullException( paramName: nameof( file ) );
            }

            if ( String.IsNullOrWhiteSpace( value: key ) ) {
                throw new ArgumentException( message: "Value cannot be null or whitespace.", paramName: nameof( key ) );
            }

            if ( file.TryGetValue( key, out var connection ) ) {
                if ( !String.IsNullOrWhiteSpace( connection ) ) {
                    return connection;
                }
            }

            if ( throwIfNotFound ) {
                throw new InvalidOperationException( "No usable connection string." );
            }

            return default;
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
                exception.Log();
            }

            foreach ( var ns in namespaces.Where( s => s.StartsWith( "ComputerManagement" ) ) ) {
                yield return $@"{root}\{ns}";
            }
        }

        /// <summary>
        ///     method extracts the instance name from the service name
        /// </summary>
        /// <param name="serviceName"></param>
        /// <returns></returns>
        [NotNull]
        public static String GetInstanceNameFromServiceName( [CanBeNull] String serviceName ) {
            if ( String.IsNullOrEmpty( serviceName ) ) {
                return String.Empty;
            }

            if ( serviceName.Like( "MSSQLSERVER" ) ) {
                return serviceName;
            }

            return serviceName.Substring( serviceName.IndexOf( '$' ) + 1, serviceName.Length - serviceName.IndexOf( '$' ) - 1 );
        }

        public static IList<PropertyInfo> GetPropertiesForType<T>() {
            var type = typeof( T );

            if ( !TypeDictionary.ContainsKey( typeof( T ) ) ) {
                TypeDictionary.Add( type, type.GetProperties().ToList() );
            }

            return TypeDictionary[ type ];
        }

        /// <summary>
        ///     Returns the WMI property value for a given property name for a particular SQL Server service Name
        /// </summary>
        /// <param name="serviceName"> The service name for the SQL Server engine serivce to query for</param>
        /// <param name="wmiNamespace">The wmi namespace to connect to</param>
        /// <param name="propertyName">The property name whose value is required</param>
        /// <returns></returns>
        [NotNull]
        public static String GetWmiPropertyValueForEngineService( [NotNull] String serviceName, [NotNull] String wmiNamespace, [NotNull] String propertyName ) {
            if ( serviceName == null ) {
                throw new ArgumentNullException( nameof( serviceName ) );
            }

            if ( wmiNamespace == null ) {
                throw new ArgumentNullException( nameof( wmiNamespace ) );
            }

            if ( propertyName == null ) {
                throw new ArgumentNullException( nameof( propertyName ) );
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

        public static async Task<Status> InitializeDatabaseConnection( [NotNull] this ConcurrentDictionaryFile<String, String> file, Credentials credentials,
            CancellationToken token ) {
            if ( file == null ) {
                throw new ArgumentNullException( paramName: nameof( file ) );
            }

            try {

                var timeout = ( TimeSpan ) Seconds.Thirty;

                TryAgain:
                var startSQLBrowsersTask = Database.StartAnySQLBrowsers( timeout );

                var connectionString = file.Get();

                if ( !String.IsNullOrWhiteSpace( connectionString ) ) {
                    var builder = new SqlConnectionStringBuilder( connectionString );

                    var sqlServer = await builder.TryGetResponse( token ).ConfigureAwait( false );

                    if ( sqlServer != null && sqlServer.Status == Status.Success && sqlServer.ConnectionStringBuilder != default ) {

                        file.Set( sqlServer.ConnectionStringBuilder.ToString() );

                        return sqlServer.Status;
                    }
                }

                await startSQLBrowsersTask.ConfigureAwait( false );

                var servers = await timeout.FindUsableServers( credentials ).ConfigureAwait( false );

                if ( servers != null ) {
                    if ( servers.Any( sqlServer => file.Set( sqlServer ) ) ) {
                        return Status.Success;
                    }

                    goto TryAgain;
                }
            }
            catch ( Exception exception ) {
                exception.Log();
            }

            return Status.Failure;
        }

        [NotNull]
        public static IEnumerable<SqlConnectionStringBuilder> LookForAnyDatabases( [CanBeNull] this Credentials credentials, TimeSpan connectTimeout ) =>
            from DataRow row in SqlDataSourceEnumerator.Instance.GetDataSources().Rows
            let serverName = row[ "ServerName" ].Trimmed()
            let instanceName = row[ "InstanceName" ].Trimmed()
            where serverName != null && instanceName != null
            select Database.OurConnectionStringBuilder( serverName, instanceName, connectTimeout, credentials );

        /// <summary>
        ///     Returns the ordinal or null.
        /// </summary>
        /// <param name="reader">    </param>
        /// <param name="columnName"></param>
        /// <returns></returns>
        public static Int32? Ordinal( [NotNull] this SqlDataReader reader, [NotNull] String columnName ) {
            if ( reader == default ) {
                throw new ArgumentNullException( paramName: nameof( reader ) );
            }

            if ( String.IsNullOrEmpty( value: columnName ) ) {
                throw new ArgumentException( message: "Value cannot be null or empty.", paramName: nameof( columnName ) );
            }

            var dictionary = reader.GetDict();

            if ( dictionary.TryGetValue( columnName, out var result ) ) {
                return result;
            }

            return default;
        }

        /// <summary>
        ///     Add the <paramref name="connectionString" /> to the <paramref name="file" /> under the given
        ///     <paramref name="key" />.
        ///     <para>Returns the key.</para>
        /// </summary>
        /// <param name="file"></param>
        /// <param name="connectionString"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        [NotNull]
        public static String Set( [NotNull] this ConcurrentDictionaryFile<String, String> file, [NotNull] String connectionString,
            [NotNull] String key = Words.PrimeConnectionString ) {
            if ( file == null ) {
                throw new ArgumentNullException( paramName: nameof( file ) );
            }

            if ( String.IsNullOrWhiteSpace( value: key ) ) {
                throw new ArgumentException( message: "Value cannot be null or whitespace.", paramName: nameof( key ) );
            }

            if ( String.IsNullOrWhiteSpace( value: connectionString ) ) {
                throw new ArgumentException( message: "Value cannot be null or whitespace.", paramName: nameof( connectionString ) );
            }

            file[ key ] = connectionString;
            file.Flush();

            Debug.Assert( file[ key ].Like( connectionString ) );

            return key;
        }

        public static Boolean Set( [NotNull] this ConcurrentDictionaryFile<String, String> file, [NotNull] SqlServer sqlServer,
            [NotNull] String key = Words.PrimeConnectionString ) {
            if ( file == null ) {
                throw new ArgumentNullException( paramName: nameof( file ) );
            }

            if ( sqlServer == null ) {
                throw new ArgumentNullException( paramName: nameof( sqlServer ) );
            }

            if ( sqlServer.Status != Status.Success ) {
                return false;
            }

            var connectionStringBuilder = sqlServer.ConnectionStringBuilder;

            if ( connectionStringBuilder == default ) {
                return false;
            }

            var connectionString = connectionStringBuilder.ConnectionString;

            file.Set( connectionString, key );

            return file.Get( key ).Like( connectionString );
        }

        public static Boolean SQLTimeout( this SqlException exception ) {
            var message = exception.Message;

            if ( message.Contains( "The server was not found or was not accessible" ) ) {
                return true;
            }

            return false;
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
        [NotNull]
        public static DataSet ToDataSet<T>( [NotNull] this IEnumerable<T> list ) {
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
        [NotNull]
        public static DataTable ToDataTable<T>( [NotNull] this IEnumerable<T> list ) {
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

        [NotNull]
        public static DataTable ToDataTable( [CanBeNull] this SqlDataReader dataReader ) {
            var table = new DataTable();
            table.BeginLoadData();

            if ( dataReader != null ) {
                table.Load( dataReader, LoadOption.OverwriteChanges );
            }

            table.EndLoadData();

            return table;
        }

        [NotNull]
        public static IEnumerable<T> ToList<T>( [NotNull] this DataTable table ) {
            var properties = GetPropertiesForType<T>();
            IList<T> result = new List<T>();

            foreach ( var row in table.Rows ) {
                var item = CreateItemFromRow<T>( row as DataRow, properties );
                result.Add( item );
            }

            return result;
        }

        [NotNull]
        public static SqlParameter ToSqlParameter<TValue>( this TValue value, String parameterName ) =>
            new SqlParameter( parameterName, value ) {
                Value = value
            };

        [NotNull]
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

        /// <summary>
        ///     Performs two selects on the database.
        ///     <code>select @@VERSION;" and "select SYSUTCDATETIME();</code>
        /// </summary>
        /// <param name="test"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        [ItemCanBeNull]
        public static async Task<SqlServer> TryGetResponse( [NotNull] this SqlConnectionStringBuilder test, CancellationToken token ) {

            if ( test == default ) {
                throw new ArgumentNullException( paramName: nameof( test ) );
            }

            try {

                var version = await test.AdhocCommand<String>( "select @@VERSION;", token ).ConfigureAwait( false );

                if ( version.status != Status.Success ) {
                    $"Failed connecting to server {test.DataSource}.".Break();

                    return default;
                }

                var getdate = await test.AdhocCommand<DateTime>( "select SYSUTCDATETIME();", token ).ConfigureAwait( false );

                if ( getdate.status != Status.Success ) {
                    $"Failed connecting to server {test.DataSource}.".Break();

                    return default;
                }

                var serverDateTime = getdate.response; //should already be utc.
                var now = DateTime.UtcNow; //get this computer's utc

                if ( serverDateTime.Date == now.Date ) {
                    ( $"Opened a connection to {test.DataSource}!" + $"{Environment.NewLine}Server Version:{version}" +
                      $"{Environment.NewLine}Server time is {serverDateTime.ToLocalTime()}" ).Log();

                    var connectionStringBuilder = new SqlConnectionStringBuilder( test.ConnectionString );

                    return new SqlServer {
                        Status = Status.Success,
                        ConnectionStringBuilder = connectionStringBuilder,
                        Version = version.response,
                        UTCDateTime = serverDateTime,
                        ResponseTime = TimeSpan.FromTicks( ( version.responseTime.Ticks + getdate.responseTime.Ticks ) / 2 ) /*avg*/
                    };
                }
            }
            catch ( Exception exception ) {
                exception.Log();
            }

            "Failed connecting to server.".Break();

            return default;
        }

        public static void TryPlayFile( this String fileName ) {
            try {
                using ( var player = new SoundPlayer() ) {
                    player.SoundLocation = fileName;
                    player.Load();
                    player.Play();
                }
            }
            catch ( Exception exception ) {
                exception.Log();
            }
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