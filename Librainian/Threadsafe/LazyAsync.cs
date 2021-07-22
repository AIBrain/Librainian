// Copyright � Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible in any binaries, libraries, repositories, or source code (directly or derived) from our binaries, libraries, projects, solutions, or applications.
//
// All source code belongs to Protiguous@Protiguous.com unless otherwise specified or the original license has been overwritten by formatting. (We try to avoid it from happening, but it does accidentally happen.)
//
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
// File "LazyAsync.cs" last touched on 2021-04-14 at 12:42 PM by Protiguous.

namespace Librainian.Threadsafe {

	using System;
	using System.Runtime.CompilerServices;
	using System.Threading.Tasks;

	/// <summary>
	///     Provides support for asynchronous lazy initialization. This type is fully threadsafe.
	/// </summary>
	/// <typeparam name="T">The type of object that is being asynchronously initialized.</typeparam>
	/// <code>
	/// private static readonly AsyncLazy&lt;MyResource&gt;
	///         myResource = new AsyncLazy
	///         &lt;MyResource&gt;
	///             ( () => new MyResource() );
	///             // or:
	///             async () => { var ret = new MyResource(); await ret.InitAsync(); return ret; }
	/// </code>
	public sealed class LazyAsync<T> {

		/// <summary>
		///     The underlying lazy task.
		/// </summary>
		private readonly Lazy<Task<T>> instance;

		/// <summary>
		///     Initializes a new instance of the <see cref="LazyAsync{T}" /> class.
		/// </summary>
		/// <param name="factory">The delegate that is invoked on a background thread to produce the value when it is needed.</param>
		public LazyAsync( Func<T> factory ) {
			this.instance = new Lazy<Task<T>>( () => Task.Run( factory ) );
		}

		/// <summary>
		///     Initializes a new instance of the <see cref="LazyAsync{T}" /> class.
		/// </summary>
		/// <param name="factory">
		///     The asynchronous delegate that is invoked on a background thread to produce the value when it is
		///     needed.
		/// </param>
		public LazyAsync( Func<Task<T>> factory ) {
			this.instance = new Lazy<Task<T>>( () => Task.Run( factory ) );
		}

		/// <summary>
		///     Asynchronous infrastructure support. This method permits instances of <see cref="LazyAsync{T}" /> to be await'ed.
		/// </summary>
		public TaskAwaiter<T> GetAwaiter() => this.instance.Value.GetAwaiter();

		/// <summary>
		///     Starts the asynchronous initialization, if it has not already started.
		/// </summary>
		public void Start() => _ = this.instance.Value;
	}

	public class TestLazyAsync {

		public async Task Yup() {
			LazyAsync<Int32> yahLazyAsync = new( this.GetHashCode );

			Console.WriteLine( await yahLazyAsync );
		}
	}
}