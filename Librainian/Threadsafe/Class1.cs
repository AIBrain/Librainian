#nullable enable

namespace Librainian.Threadsafe {
	using System;
	using System.Diagnostics;

	public static class Program {

		public static void Main() {

			var start = Stopwatch.StartNew();

			static DateTime Time() => Threadsafer<DateTime>.Get()!.Invoke();

			var prev = Time();
			while ( start.Elapsed < TimeSpan.FromSeconds( 10 ) ) {
				var now = Time();
				if ( now != prev ) {
					Console.WriteLine( now );
					prev = now;
				}

			}

		}
	}
}
