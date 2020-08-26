// Copyright © Protiguous. All Rights Reserved.
// This entire copyright notice and license must be retained and must be kept visible in any binaries, libraries, repositories, or source code (directly or derived) from our binaries, libraries, projects, solutions, or applications.
// All source code belongs to Protiguous@Protiguous.com unless otherwise specified or the original license has been overwritten by formatting. (We try to avoid it from happening, but it does accidentally happen.)
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
// File "DataRowSetExtensions.cs" last formatted on 2020-08-14 at 8:32 PM.

#nullable enable

namespace Librainian.Databases {

	using System;
	using System.Collections.Generic;
	using System.Data;
	using System.Diagnostics;
	using System.Reflection;
	using System.Runtime.Serialization;
	using JetBrains.Annotations;
	using Logging;

	public static class DataRowSetExtensions {

		public static void DisplayTable( [NotNull] this DataTable table ) {
			foreach ( DataRow row in table.Rows ) {
				foreach ( DataColumn dataColumn in table.Columns ) {
					Debug.WriteLine( "{0} = {1}", dataColumn.ColumnName, row[dataColumn] );
				}

				Debug.WriteLine( String.Empty );
			}
		}

		[NotNull]
		public static T CreateItemFromRow<T>( [NotNull] this DataRow row, [NotNull] [ItemNotNull] IEnumerable<PropertyInfo> properties ) {
			//TODO benchmark which is the *fastest*: new, Activator.CreateInstance, Expression.New, or FormatterServices.GetUninitializedObject
			//T item = new T();
			//var item = Activator.CreateInstance<T>();
			//var t = Expression.Lambda<Func<T>>( Expression.New( typeof( T ) ) ).Compile();

			var item = ( T )FormatterServices.GetUninitializedObject( typeof( T ) );

			foreach ( var property in properties ) {
				property.SetValue( item, row[property.Name], null );
			}

			return item;
		}

		public static Boolean Add<T>( [NotNull] this DataSet dataSet, [NotNull] IEnumerable<T> list ) {
			try {
				dataSet.Tables.Add( list.ToDataTable() );

				return true;
			}
			catch ( ArgumentNullException exception ) {
				exception.Log();

				throw;
			}
			catch ( ArgumentException exception ) {
				exception.Log();

				throw;
			}
			catch ( DuplicateNameException exception ) {
				exception.Log();

				throw;
			}
		}

	}

}