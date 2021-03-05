// Copyright © Protiguous. All Rights Reserved.
// This entire copyright notice and license must be retained and must be kept visible in any binaries, libraries, repositories, or source code (directly or derived) from our binaries, libraries, projects, solutions, or applications.
// All source code belongs to Protiguous@Protiguous.com unless otherwise specified or the original license has been overwritten by formatting. (We try to avoid it from happening, but it does accidentally happen.)
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
// File "ReferenceArray.cs" last formatted on 2020-08-14 at 8:47 PM.

#nullable enable

namespace Librainian.Threadsafe {

	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Runtime.CompilerServices;
	using System.Threading;
	using JetBrains.Annotations;

	/// <summary>A reference array that may be updated atomically</summary>
	public class ReferenceArray<T> where T : class {

		public ReferenceArray( Int32 length ) {
			if ( length <= 0 ) {
				throw new ArgumentOutOfRangeException( nameof( length ) );
			}

			this.Length = length;
			this.Array = new T[length];
		}

		/// <summary>
		///     Create a new <see cref="ReferenceArray{T}" /> with the same length as, and all elements copied from, the given
		///     array.
		/// </summary>
		/// <param name="array"></param>
		public ReferenceArray( [NotNull] IEnumerable<T> array ) => this.Array = array.ToArray();

		[NotNull]
		private T[] Array { get; }

		public Int32 Length { get; }

		/// <summary>Atomically set the value to the given updated value if the current value equals the comparand</summary>
		/// <param name="newValue"> The new value</param>
		/// <param name="comparand">The comparand (expected value)</param>
		/// <param name="index">    The index.</param>
		/// <returns>The original value</returns>
		public Boolean AtomicCompareExchange( Int32 index, [CanBeNull] T newValue, [CanBeNull] T comparand ) =>
			Interlocked.CompareExchange( ref this.Array[index], newValue, comparand ) == comparand;

		/// <summary>Atomically set the value to the given updated value</summary>
		/// <param name="newValue">The new value</param>
		/// <param name="index">   The index.</param>
		/// <returns>The original value</returns>
		public T? AtomicExchange( Int32 index, [CanBeNull] T newValue ) {
			var result = Interlocked.Exchange( ref this.Array[index], newValue );

			return result;
		}

		/// <summary>Read the value applying acquire fence semantic</summary>
		/// <param name="index">The element index</param>
		/// <returns>The current value</returns>
		[CanBeNull]
		public T ReadAcquireFence( Int32 index ) {
			var value = this.Array[index];
			Thread.MemoryBarrier();

			return value;
		}

		/// <summary>Read the value applying a compiler only fence, no CPU fence is applied</summary>
		/// <param name="index">The element index</param>
		/// <returns>The current value</returns>
		[MethodImpl( MethodImplOptions.NoOptimization )]
		[CanBeNull]
		public T ReadCompilerOnlyFence( Int32 index ) => this.Array[index];

		/// <summary>Read the value applying full fence semantic</summary>
		/// <param name="index">The element index</param>
		/// <returns>The current value</returns>
		[CanBeNull]
		public T ReadFullFence( Int32 index ) {
			var value = this.Array[index];
			Thread.MemoryBarrier();

			return value;
		}

		/// <summary>Read the value without applying any fence</summary>
		/// <param name="index">The index of the element.</param>
		/// <returns>The current value.</returns>
		[CanBeNull]
		public T ReadUnfenced( Int32 index ) => this.Array[index];

		/// <summary>Write the value applying a compiler fence only, no CPU fence is applied</summary>
		/// <param name="index">   The element index</param>
		/// <param name="newValue">The new value</param>
		[MethodImpl( MethodImplOptions.NoOptimization )]
		public void WriteCompilerOnlyFence( Int32 index, [CanBeNull] T newValue ) => this.Array[index] = newValue;

		/// <summary>Write the value applying full fence semantic</summary>
		/// <param name="index">   The element index</param>
		/// <param name="newValue">The new value</param>
		public void WriteFullFence( Int32 index, [CanBeNull] T newValue ) {
			this.Array[index] = newValue;
			Thread.MemoryBarrier();
		}

		/// <summary>Write the value applying release fence semantic</summary>
		/// <param name="index">   The element index</param>
		/// <param name="newValue">The new value</param>
		public void WriteReleaseFence( Int32 index, [CanBeNull] T newValue ) {
			this.Array[index] = newValue;
			Thread.MemoryBarrier();
		}

		/// <summary>Write without applying any fence</summary>
		/// <param name="index">   The index.</param>
		/// <param name="newValue">The new value</param>
		public void WriteUnfenced( Int32 index, [CanBeNull] T newValue ) => this.Array[index] = newValue;

	}

}