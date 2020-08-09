#nullable enable

namespace Librainian.Threadsafe {

	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Threading;
	using JetBrains.Annotations;

	public class ThreadSafeEnumerable<T> : IEnumerable<T> {

		public ThreadSafeEnumerable( [NotNull] IEnumerable<T> original ) => this._Original = original ?? throw new ArgumentNullException( nameof( original ) );

		[NotNull]
		private IEnumerable<T> _Original { get; }

		[NotNull]
		public IEnumerator<T> GetEnumerator() => new ThreadSafeEnumerator( this._Original.GetEnumerator() );

		[NotNull]
		IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

		private sealed class ThreadSafeEnumerator : IEnumerator<T> {

			internal ThreadSafeEnumerator( [NotNull] IEnumerator<T> original ) => this.original = original ?? throw new ArgumentNullException( nameof( original ) );

			[NotNull]
			private ThreadLocal<T> current { get; } = new ThreadLocal<T>();

			[NotNull]
			private IEnumerator<T> original { get; }

			[NotNull]
			private Object padlock { get; } = new Object();

			[CanBeNull]
			public T Current => this.current.Value;

			[CanBeNull]
			Object? IEnumerator.Current => this.Current;

			public void Dispose() {
				using ( this.original ) { }

				using ( this.current ) { }
			}

			public Boolean MoveNext() {
				lock ( this.padlock ) {
					var next = this.original.MoveNext();

					if ( next ) {
						this.current.Value = this.original.Current;
					}

					return next;
				}
			}

			public void Reset() => throw new NotSupportedException();

		}

	}

}