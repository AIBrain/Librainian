#nullable enable

namespace Librainian.Collections.Extensions {

	using System;
	using Arrays;
	using JetBrains.Annotations;

	public static class ArrayExtensions {

		public static void ForEach( [NotNull] this Array array, [NotNull] Action<Array, Int32[]> action ) {
			if ( array.Length == 0 ) {
				return;
			}

			var walker = new ArrayTraverse( array );

			do {
				action( array, walker.Position );
			} while ( walker.Step() );
		}

		public static void ForEach( [NotNull] this Array array, [NotNull] Action<Array, Int64[]> action ) {
			if ( array.LongLength == 0 ) {
				return;
			}

			var walker = new LongArrayTraverse( array );

			do {
				action( array, walker.Position );
			} while ( walker.Step() );
		}

	}

}