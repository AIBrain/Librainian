// Copyright 2016 Protiguous.
//
// This notice must be kept visible in the source.
//
// This section of source code belongs to Protiguous@Protiguous.com unless otherwise specified, or the
// original license has been overwritten by the automatic formatting of this code. Any unmodified
// sections of source code borrowed from other projects retain their original license and thanks
// goes to the Authors.
//
// Donations and royalties can be paid via
//  
//  bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//  
//
// Usage of the source code or compiled binaries is AS-IS. I am not responsible for Anything You Do.
//
// Contact me by email if you have any questions or helpful criticism.
//
// "Librainian/GenericPopulator.cs" was last cleaned by Protiguous on 2016/06/18 at 10:50 PM

namespace Librainian.Database {

    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;
    using System.Linq.Expressions;

    public static class GenericPopulator<T> {

        public static List<T> CreateList( SqlDataReader reader ) {
            var results = new List<T>();
            var readRow = GetReader( reader );

            while ( reader.Read() ) {
                results.Add( readRow( reader ) );
            }

            return results;
        }

        private static Func<SqlDataReader, T> GetReader( IDataRecord reader ) {
            var readerColumns = new List<String>();
            for ( var index = 0; index < reader.FieldCount; index++ ) {
                readerColumns.Add( reader.GetName( index ) );
            }

            // determine the information about the reader
            var readerParam = Expression.Parameter( typeof( SqlDataReader ), "reader" );
            var readerGetValue = typeof( SqlDataReader ).GetMethod( "GetValue" );

            // create a Constant expression of DBNull.Value to compare values to in reader
            var dbNullValue = typeof( DBNull ).GetField( "Value" );
            var dbNullExp = Expression.Field( Expression.Parameter( typeof( DBNull ), "System.DBNull" ), dbNullValue );

            // loop through the properties and create MemberBinding expressions for each property
            var memberBindings = new List<MemberBinding>();
            foreach ( var prop in typeof( T ).GetProperties() ) {

                // determine the default value of the property
                Object defaultValue = null;
                if ( prop.PropertyType.IsValueType ) {
                    defaultValue = Activator.CreateInstance( prop.PropertyType );
                }
                else if ( prop.PropertyType.Name.ToLower().Equals( "string" ) ) {
                    defaultValue = String.Empty;
                }

                if ( readerColumns.Contains( prop.Name ) ) {

                    // build the Call expression to retrieve the data value from the reader
                    var indexExpression = Expression.Constant( reader.GetOrdinal( prop.Name ) );
                    var getValueExp = Expression.Call( readerParam, readerGetValue, indexExpression );

                    // create the conditional expression to make sure the reader value != DBNull.Value
                    var testExp = Expression.NotEqual( dbNullExp, getValueExp );
                    var ifTrue = Expression.Convert( getValueExp, prop.PropertyType );
                    var ifFalse = Expression.Convert( Expression.Constant( defaultValue ), prop.PropertyType );

                    // create the actual Bind expression to bind the value from the reader to the property value
                    var mi = typeof( T ).GetMember( prop.Name )[ 0 ];
                    MemberBinding mb = Expression.Bind( mi, Expression.Condition( testExp, ifTrue, ifFalse ) );
                    memberBindings.Add( mb );
                }
            }

            // create a MemberInit expression for the item with the member bindings
            var newItem = Expression.New( typeof( T ) );
            var memberInit = Expression.MemberInit( newItem, memberBindings );

            var lambda = Expression.Lambda<Func<SqlDataReader, T>>( memberInit, readerParam );
            Delegate resDelegate = lambda.Compile();

            return ( Func<SqlDataReader, T> )resDelegate;
        }
    }
}