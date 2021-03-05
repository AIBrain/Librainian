#nullable enable

namespace Librainian.Threadsafe {
	using System;
	using System.Diagnostics;
	using System.Threading;
	using JetBrains.Annotations;

	public class Threadsafer<T> {

		private ThreadLocal<T> Instances { get; }

		public Threadsafer( [NotNull] Func<T> func ) {
			if ( func == null ) {
				throw new ArgumentNullException( nameof( func ) );
			}

			this.Instances = new ThreadLocal<T>( func );
		}

		public T? Get() => this.Instances.Value;
	}

	public static class Program {

		public static void Main() {

			var start = Stopwatch.StartNew();

			var bob = new Threadsafer<DateTime>( Func  );
			var prev = bob.Get();
			while ( start.Elapsed < TimeSpan.FromSeconds( 10 ) ) {
				var now = bob.Get();
				if ( now != prev ) {
					Console.WriteLine( now );
					prev = now;
				}

			}

		}

		private static DateTime Func() {
			return DateTime.UtcNow;
		}
	}
}
