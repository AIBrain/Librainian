namespace Librainian.Collections.Arrays {

	using System;
	using JetBrains.Annotations;

	public class LongArrayTraverse {

		[NotNull]
		private Int32[] MaxLengths { get; }

		[NotNull]
		public Int64[] Position { get; }

		public LongArrayTraverse( [NotNull] Array array ) {
			this.MaxLengths = new Int32[ array.Rank ];

			for ( var dimension = 0; dimension < array.Rank; ++dimension ) {
				this.MaxLengths[ dimension ] = array.GetLength( dimension ) - 1;	//will GetLongLength ever be needed here?
			}

			this.Position = new Int64[ array.Rank ];
		}

		public Boolean Step() {
			for ( var i = 0; i < this.Position.LongLength; ++i ) {
				if ( this.Position[ i ] >= this.MaxLengths[ i ] ) {
					continue;
				}

				++this.Position[ i ];

				for ( var j = 0; j < i; j++ ) {
					this.Position[ j ] = 0;
				}

				return true;
			}

			return false;
		}
	}

}