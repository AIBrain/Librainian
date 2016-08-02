// Copyright 2016 Rick@AIBrain.org.
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
// "Librainian/DatabaseExtensions.cs" was last cleaned by Rick on 2016/06/18 at 10:50 PM

namespace Librainian.Database {

    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;
    using System.Linq;
    using System.Media;
    using System.Reflection;

    public static class DatabaseExtensions {
        private static readonly Dictionary<Type, IList<PropertyInfo>> typeDictionary = new Dictionary<Type, IList<PropertyInfo>>();

        public static IList<PropertyInfo> GetPropertiesForType<T>() {
            var type = typeof( T );
            if ( !typeDictionary.ContainsKey( typeof( T ) ) ) {
                typeDictionary.Add( type, type.GetProperties().ToList() );
            }
            return typeDictionary[ type ];
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

        public static IList<T> ToList<T>( this DataTable table ) {
            var properties = GetPropertiesForType<T>();
            IList<T> result = new List<T>();

            foreach ( var row in table.Rows ) {
                var item = CreateItemFromRow<T>( ( DataRow )row, properties );
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

        private static T CreateItemFromRow<T>( DataRow row, IList<PropertyInfo> properties ) {

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
    }
}