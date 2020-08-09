#nullable enable

namespace Librainian.Threadsafe {

	using System;

	/// <summary>A small toolkit of classes that support lock-free thread-safe programming on single variables and arrays</summary>
	/// <see cref="http://github.com/disruptor-net/Disruptor-net/blob/master/Atomic/Volatile.cs" />
	public static class Volatiles {

		/// <summary>Size of a cache line in bytes</summary>
		public const Int32 CacheLineSize = 64;

	}

}