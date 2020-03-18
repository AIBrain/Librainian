// Copyright © 2020 Protiguous. All Rights Reserved.
// 
// This entire copyright notice and license must be retained and must be kept visible in any binaries, libraries, repositories, and source code (directly or derived)
// from our binaries, libraries, projects, or solutions.
// 
// This source code contained in "GenericPopulator.cs" belongs to Protiguous@Protiguous.com unless otherwise specified or the original license has been overwritten
// by formatting. (We try to avoid it from happening, but it does accidentally happen.)
// 
// Any unmodified portions of source code gleaned from other projects still retain their original license and our thanks goes to those Authors.
// If you find your code in this source code, please let us know so we can properly attribute you and include the proper license and/or copyright.
// 
// If you want to use any of our code in a commercial project, you must contact Protiguous@Protiguous.com for permission and a quote.
// 
// Donations are accepted via bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2 and PayPal: Protiguous@Protiguous.com
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
// For business inquiries, please contact me at Protiguous@Protiguous.com.
// 
// Our website can be found at "https://Protiguous.com/"
// Our software can be found at "https://Protiguous.Software/"
// Our GitHub address is "https://github.com/Protiguous".
// Feel free to browse any source code we make available.
// 
// Project: "Librainian", File: "GenericPopulator.cs" was last formatted by Protiguous on 2020/03/16 at 9:33 PM.

namespace Librainian.Databases {

    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq.Expressions;
    using JetBrains.Annotations;
    using Microsoft.Data.SqlClient;
    using Parsing;

    public static class GenericPopulator<T> {

        [CanBeNull]
        public static Func<SqlDataReader, T> GetReader( [NotNull] IDataRecord reader ) {
            if ( reader == null ) {
                throw new ArgumentNullException( paramName: nameof( reader ) );
            }

            var readerColumns = new List<String>();

            for ( var index = 0; index < reader.FieldCount; index++ ) {
                readerColumns.Add( item: reader.GetName( i: index ) );
            }

            // determine the information about the reader
            var readerParam = Expression.Parameter( type: typeof( SqlDataReader ), name: "reader" );
            var readerGetValue = typeof( SqlDataReader ).GetMethod( name: "GetValue" );

            if ( readerGetValue is null ) {
                return null;
            }

            // create a Constant expression of DBNull.Value to compare values to in reader
            var dbNullValue = typeof( DBNull ).GetField( name: "Value" );
            var dbNullExp = Expression.Field( expression: Expression.Parameter( type: typeof( DBNull ), name: "System.DBNull" ), field: dbNullValue );

            // loop through the properties and create MemberBinding expressions for each property
            var memberBindings = new List<MemberBinding>();

            foreach ( var prop in typeof( T ).GetProperties() ) {

                // determine the default value of the property
                Object defaultValue = null;

                if ( prop.PropertyType.IsValueType ) {
                    defaultValue = Activator.CreateInstance( type: prop.PropertyType );
                }
                else if ( prop.PropertyType.Name.Like( right: "string" ) ) {
                    defaultValue = String.Empty;
                }

                if ( readerColumns.Contains( item: prop.Name ) ) {

                    // build the Call expression to retrieve the data value from the reader
                    var indexExpression = Expression.Constant( value: reader.GetOrdinal( name: prop.Name ) );
                    var getValueExp = Expression.Call( instance: readerParam, method: readerGetValue, indexExpression );

                    // create the conditional expression to make sure the reader value != DBNull.Value
                    var testExp = Expression.NotEqual( left: dbNullExp, right: getValueExp );
                    var ifTrue = Expression.Convert( expression: getValueExp, type: prop.PropertyType );
                    var ifFalse = Expression.Convert( expression: Expression.Constant( value: defaultValue ), type: prop.PropertyType );

                    // create the actual Bind expression to bind the value from the reader to the property value
                    var mi = typeof( T ).GetMember( name: prop.Name )[ 0 ];
                    MemberBinding mb = Expression.Bind( member: mi, expression: Expression.Condition( test: testExp, ifTrue: ifTrue, ifFalse: ifFalse ) );
                    memberBindings.Add( item: mb );
                }
            }

            // create a MemberInit expression for the item with the member bindings
            var newItem = Expression.New( type: typeof( T ) );
            var memberInit = Expression.MemberInit( newExpression: newItem, bindings: memberBindings );

            var lambda = Expression.Lambda<Func<SqlDataReader, T>>( body: memberInit, readerParam );
            Delegate resDelegate = lambda.Compile();

            return ( Func<SqlDataReader, T> ) resDelegate;
        }

    }

    public static class GenericPopulatorExtensions {

        [NotNull]
        public static List<T> CreateList<T>( [NotNull] SqlDataReader reader ) {
            var results = new List<T>();
            var readRow = GenericPopulator<T>.GetReader( reader: reader );

            while ( reader.Read() ) {
                results.Add( item: readRow( arg: reader ) );
            }

            return results;
        }

    }

}