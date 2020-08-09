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
					Debug.WriteLine( "{0} = {1}", dataColumn.ColumnName, row[ dataColumn ] );
				}

				Debug.WriteLine( String.Empty );
			}
		}

		[NotNull]
		public static T CreateItemFromRow<T>( [NotNull] this DataRow row, [NotNull][ItemNotNull] IEnumerable<PropertyInfo> properties ) {

			//TODO benchmark which is the *fastest*: new, Activator.CreateInstance, Expression.New, or FormatterServices.GetUninitializedObject
			//T item = new T();
			//var item = Activator.CreateInstance<T>();
			//var t = Expression.Lambda<Func<T>>( Expression.New( typeof( T ) ) ).Compile();

			var item = ( T )FormatterServices.GetUninitializedObject( typeof( T ) );

			foreach ( var property in properties ) {
				property.SetValue( item, row[ property.Name ], null );
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