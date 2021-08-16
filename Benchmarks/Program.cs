namespace Benchmarks {
	using System;
	using BenchmarkDotNet.Running;

	public class Program {

		public static void Main( String[] args ) => BenchmarkRunner.Run<BenchmarkParsings>();

	}

}
