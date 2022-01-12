namespace Benchmarks;

using System;
using BenchmarkDotNet.Running;

public class Program {

	public static void Main( String[] args ) => BenchmarkRunner.Run<CompareRightSubstringRangeSlice>();

	//public static void Main( String[] args ) => BenchmarkSwitcher.FromAssembly( typeof( Program ).Assembly ).Run( args, new DebugInProcessConfig() );

}