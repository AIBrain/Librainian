﻿// Copyright © Rick@AIBrain.org and Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
//
// This source code contained in "GenericPopulator.cs" belongs to Protiguous@Protiguous.com and
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
// Project: "Librainian", "GenericPopulator.cs" was last formatted by Protiguous on 2019/08/08 at 6:55 AM.

namespace Librainian.Databases {

    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;
    using System.Linq.Expressions;
    using JetBrains.Annotations;

    public static class GenericPopulator<T> {

        [NotNull]
        private static Func<SqlDataReader, T> GetReader( [NotNull] IDataRecord reader ) {
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

        [NotNull]
        public static List<T> CreateList( [NotNull] SqlDataReader reader ) {
            var results = new List<T>();
            var readRow = GetReader( reader );

            while ( reader.Read() ) {
                results.Add( readRow( reader ) );
            }

            return results;
        }
    }
}