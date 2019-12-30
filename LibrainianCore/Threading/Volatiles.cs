﻿// Copyright © Rick@AIBrain.org and Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
//
// This source code contained in "Volatiles.cs" belongs to Protiguous@Protiguous.com and
// Rick@AIBrain.org unless otherwise specified or the original license has
// been overwritten by formatting.
// (We try to avoid it from happening, but it does accidentally happen.)
//
// Any unmodified portions of source code gleaned from other projects still retain their original
// license and our thanks goes to those Authors. If you find your code in this source code, please
// let us know so we can properly attribute you and include the proper license and/or copyright.
//
// If you want to use any of our code, you must contact Protiguous@Protiguous.com or
// Sales@AIBrain.org for permission and a quote.
//
// Donations are accepted (for now) via
//     bitcoin:1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//     PayPal:Protiguous@Protiguous.com
//     (We're always looking into other solutions.. Any ideas?)
//
// =========================================================
// Disclaimer:  Usage of the source code or binaries is AS-IS.
//    No warranties are expressed, implied, or given.
//    We are NOT responsible for Anything You Do With Our Code.
//    We are NOT responsible for Anything You Do With Our Executables.
//    We are NOT responsible for Anything You Do With Your Computer.
// =========================================================
//
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
// For business inquiries, please contact me at Protiguous@Protiguous.com
//
// Our website can be found at "https://Protiguous.com/"
// Our software can be found at "https://Protiguous.Software/"
// Our GitHub address is "https://github.com/Protiguous".
// Feel free to browse any source code we make available.
//
// Project: "Librainian", "Volatiles.cs" was last formatted by Protiguous on 2019/12/04 at 9:47 PM.

namespace LibrainianCore.Threading {

    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Threading;

    /// <summary>An integer value that may be updated atomically</summary>
    public struct Integer {

        private Int32 _value;

        /// <summary>Create a new <see cref="Integer" /> with the given initial value.</summary>
        /// <param name="value">Initial value</param>
        public Integer( Int32 value ) => this._value = value;

        /// <summary>Atomically add the given value to the current value and return the sum</summary>
        /// <param name="delta">The value to be added</param>
        /// <returns>The sum of the current value and the given value</returns>
        public Int32 AtomicAddAndGet( Int32 delta ) => Interlocked.Add( location1: ref this._value, delta );

        /// <summary>Atomically set the value to the given updated value if the current value equals the comparand</summary>
        /// <param name="newValue"> The new value</param>
        /// <param name="comparand">The comparand (expected value)</param>
        /// <returns></returns>
        public Boolean AtomicCompareExchange( Int32 newValue, Int32 comparand ) =>
            Interlocked.CompareExchange( location1: ref this._value, newValue, comparand: comparand ) == comparand;

        /// <summary>Atomically increment the current value and return the new value</summary>
        /// <returns>The decremented value.</returns>
        public Int32 AtomicDecrementAndGet() => Interlocked.Decrement( location: ref this._value );

        /// <summary>Atomically set the value to the given updated value</summary>
        /// <param name="newValue">The new value</param>
        /// <returns>The original value</returns>
        public Int32 AtomicExchange( Int32 newValue ) => Interlocked.Exchange( location1: ref this._value, newValue );

        /// <summary>Atomically increment the current value and return the new value</summary>
        /// <returns>The incremented value.</returns>
        public Int32 AtomicIncrementAndGet() => Interlocked.Increment( location: ref this._value );

        /// <summary>Read the value applying acquire fence semantic</summary>
        /// <returns>The current value</returns>
        public Int32 ReadAcquireFence() {
            var value = this._value;
            Thread.MemoryBarrier();

            return value;
        }

        /// <summary>Read the value applying a compiler only fence, no CPU fence is applied</summary>
        /// <returns>The current value</returns>
        [MethodImpl( methodImplOptions: MethodImplOptions.NoOptimization )]
        public Int32 ReadCompilerOnlyFence() => this._value;

        /// <summary>Read the value applying full fence semantic</summary>
        /// <returns>The current value</returns>
        public Int32 ReadFullFence() {
            var value = this._value;
            Thread.MemoryBarrier();

            return value;
        }

        /// <summary>Read the value without applying any fence</summary>
        /// <returns>The current value</returns>
        public Int32 ReadUnfenced() => this._value;

        /// <summary>Returns the String representation of the current value.</summary>
        /// <returns>the String representation of the current value.</returns>
        public override String ToString() {
            var value = this.ReadFullFence();

            return value.ToString();
        }

        /// <summary>Write the value applying a compiler fence only, no CPU fence is applied</summary>
        /// <param name="newValue">The new value</param>
        [MethodImpl( methodImplOptions: MethodImplOptions.NoOptimization )]
        public void WriteCompilerOnlyFence( Int32 newValue ) => this._value = newValue;

        /// <summary>Write the value applying full fence semantic</summary>
        /// <param name="newValue">The new value</param>
        public void WriteFullFence( Int32 newValue ) {
            this._value = newValue;
            Thread.MemoryBarrier();
        }

        /// <summary>Write the value applying release fence semantic</summary>
        /// <param name="newValue">The new value</param>
        public void WriteReleaseFence( Int32 newValue ) {
            this._value = newValue;
            Thread.MemoryBarrier();
        }

        /// <summary>Write without applying any fence</summary>
        /// <param name="newValue">The new value</param>
        public void WriteUnfenced( Int32 newValue ) => this._value = newValue;
    }

    /// <summary>A long value that may be updated atomically</summary>
    public struct Long {

        private Int64 _value;

        /// <summary>Create a new <see cref="Long" /> with the given initial value.</summary>
        /// <param name="value">Initial value</param>
        public Long( Int64 value ) => this._value = value;

        /// <summary>Atomically add the given value to the current value and return the sum</summary>
        /// <param name="delta">The value to be added</param>
        /// <returns>The sum of the current value and the given value</returns>
        public Int64 AtomicAddAndGet( Int64 delta ) => Interlocked.Add( location1: ref this._value, delta );

        /// <summary>Atomically set the value to the given updated value if the current value equals the comparand</summary>
        /// <param name="newValue"> The new value</param>
        /// <param name="comparand">The comparand (expected value)</param>
        /// <returns></returns>
        public Boolean AtomicCompareExchange( Int64 newValue, Int64 comparand ) =>
            Interlocked.CompareExchange( location1: ref this._value, newValue, comparand: comparand ) == comparand;

        /// <summary>Atomically increment the current value and return the new value</summary>
        /// <returns>The decremented value.</returns>
        public Int64 AtomicDecrementAndGet() => Interlocked.Decrement( location: ref this._value );

        /// <summary>Atomically set the value to the given updated value</summary>
        /// <param name="newValue">The new value</param>
        /// <returns>The original value</returns>
        public Int64 AtomicExchange( Int64 newValue ) => Interlocked.Exchange( location1: ref this._value, newValue );

        /// <summary>Atomically increment the current value and return the new value</summary>
        /// <returns>The incremented value.</returns>
        public Int64 AtomicIncrementAndGet() => Interlocked.Increment( location: ref this._value );

        /// <summary>Read the value applying acquire fence semantic</summary>
        /// <returns>The current value</returns>
        public Int64 ReadAcquireFence() {
            var value = this._value;
            Thread.MemoryBarrier();

            return value;
        }

        /// <summary>Read the value applying a compiler only fence, no CPU fence is applied</summary>
        /// <returns>The current value</returns>
        [MethodImpl( methodImplOptions: MethodImplOptions.NoOptimization )]
        public Int64 ReadCompilerOnlyFence() => this._value;

        /// <summary>Read the value applying full fence semantic</summary>
        /// <returns>The current value</returns>
        public Int64 ReadFullFence() {
            Thread.MemoryBarrier();

            return this._value;
        }

        /// <summary>Read the value without applying any fence</summary>
        /// <returns>The current value</returns>
        public Int64 ReadUnfenced() => this._value;

        /// <summary>Returns the String representation of the current value.</summary>
        /// <returns>the String representation of the current value.</returns>
        public override String ToString() {
            var value = this.ReadFullFence();

            return value.ToString();
        }

        /// <summary>Write the value applying a compiler fence only, no CPU fence is applied</summary>
        /// <param name="newValue">The new value</param>
        [MethodImpl( methodImplOptions: MethodImplOptions.NoOptimization )]
        public void WriteCompilerOnlyFence( Int64 newValue ) => this._value = newValue;

        /// <summary>Write the value applying full fence semantic</summary>
        /// <param name="newValue">The new value</param>
        public void WriteFullFence( Int64 newValue ) {
            Thread.MemoryBarrier();
            this._value = newValue;
        }

        /// <summary>Write the value applying release fence semantic</summary>
        /// <param name="newValue">The new value</param>
        public void WriteReleaseFence( Int64 newValue ) {
            Thread.MemoryBarrier();
            this._value = newValue;
        }

        /// <summary>Write without applying any fence</summary>
        /// <param name="newValue">The new value</param>
        public void WriteUnfenced( Int64 newValue ) => this._value = newValue;
    }

    /// <summary>A boolean value that may be updated atomically and is guaranteed to live on its own cache line (to prevent false sharing)</summary>
    [StructLayout( layoutKind: LayoutKind.Explicit, Size = Volatiles.CacheLineSize * 2 )]
    public struct PaddedBoolean {

        // Boolean stored as an int, CAS not available on Boolean
        [FieldOffset( offset: Volatiles.CacheLineSize )]
        private Int32 _value;

        private const Byte False = 0;

        private const Byte True = 1;

        /// <summary>Create a new <see cref="PaddedBoolean" /> with the given initial value.</summary>
        /// <param name="value">Initial value</param>
        public PaddedBoolean( Boolean value ) => this._value = value ? True : False;

        /// <summary>Read the value without applying any fence</summary>
        /// <returns>The current value</returns>
        public Boolean ReadUnfenced() => ToBool( this._value );

        /// <summary>Read the value applying acquire fence semantic</summary>
        /// <returns>The current value</returns>
        public Boolean ReadAcquireFence() {
            var value = ToBool( this._value );
            Thread.MemoryBarrier();

            return value;
        }

        /// <summary>Read the value applying full fence semantic</summary>
        /// <returns>The current value</returns>
        public Boolean ReadFullFence() {
            var value = ToBool( this._value );
            Thread.MemoryBarrier();

            return value;
        }

        /// <summary>Read the value applying a compiler only fence, no CPU fence is applied</summary>
        /// <returns>The current value</returns>
        [MethodImpl( methodImplOptions: MethodImplOptions.NoOptimization )]
        public Boolean ReadCompilerOnlyFence() => ToBool( this._value );

        /// <summary>Write the value applying release fence semantic</summary>
        /// <param name="newValue">The new value</param>
        public void WriteReleaseFence( Boolean newValue ) {
            var newValueInt = ToInt( newValue );
            Thread.MemoryBarrier();
            this._value = newValueInt;
        }

        /// <summary>Write the value applying full fence semantic</summary>
        /// <param name="newValue">The new value</param>
        public void WriteFullFence( Boolean newValue ) {
            var newValueInt = ToInt( newValue );
            Thread.MemoryBarrier();
            this._value = newValueInt;
        }

        /// <summary>Write the value applying a compiler fence only, no CPU fence is applied</summary>
        /// <param name="newValue">The new value</param>
        [MethodImpl( methodImplOptions: MethodImplOptions.NoOptimization )]
        public void WriteCompilerOnlyFence( Boolean newValue ) => this._value = ToInt( newValue );

        /// <summary>Write without applying any fence</summary>
        /// <param name="newValue">The new value</param>
        public void WriteUnfenced( Boolean newValue ) => this._value = ToInt( newValue );

        /// <summary>Atomically set the value to the given updated value if the current value equals the comparand</summary>
        /// <param name="newValue"> The new value</param>
        /// <param name="comparand">The comparand (expected value)</param>
        /// <returns></returns>
        public Boolean AtomicCompareExchange( Boolean newValue, Boolean comparand ) {
            var newValueInt = ToInt( newValue );
            var comparandInt = ToInt( comparand );

            return Interlocked.CompareExchange( location1: ref this._value, newValueInt, comparand: comparandInt ) == comparandInt;
        }

        /// <summary>Atomically set the value to the given updated value</summary>
        /// <param name="newValue">The new value</param>
        /// <returns>The original value</returns>
        public Boolean AtomicExchange( Boolean newValue ) {
            var newValueInt = ToInt( newValue );
            var originalValue = Interlocked.Exchange( location1: ref this._value, newValueInt );

            return ToBool( originalValue );
        }

        /// <summary>Returns the String representation of the current value.</summary>
        /// <returns>the String representation of the current value.</returns>
        public override String ToString() {
            var value = this.ReadFullFence();

            return value.ToString();
        }

        private static Boolean ToBool( Int32 value ) {
            if ( value != False && value != True ) {
                throw new ArgumentOutOfRangeException( nameof( value ) );
            }

            return value == True;
        }

        private static Int32 ToInt( Boolean value ) => value ? True : False;
    }

    /// <summary>An integer value that may be updated atomically and is guaranteed to live on its own cache line (to prevent false sharing)</summary>
    [StructLayout( layoutKind: LayoutKind.Explicit, Size = Volatiles.CacheLineSize * 2 )]
    public struct PaddedInteger {

        [FieldOffset( offset: Volatiles.CacheLineSize )]
        private Int32 _value;

        /// <summary>Create a new <see cref="PaddedInteger" /> with the given initial value.</summary>
        /// <param name="value">Initial value</param>
        public PaddedInteger( Int32 value ) => this._value = value;

        /// <summary>Read the value without applying any fence</summary>
        /// <returns>The current value</returns>
        public Int32 ReadUnfenced() => this._value;

        /// <summary>Read the value applying acquire fence semantic</summary>
        /// <returns>The current value</returns>
        public Int32 ReadAcquireFence() {
            var value = this._value;
            Thread.MemoryBarrier();

            return value;
        }

        /// <summary>Read the value applying full fence semantic</summary>
        /// <returns>The current value</returns>
        public Int32 ReadFullFence() {
            var value = this._value;
            Thread.MemoryBarrier();

            return value;
        }

        /// <summary>Read the value applying a compiler only fence, no CPU fence is applied</summary>
        /// <returns>The current value</returns>
        [MethodImpl( methodImplOptions: MethodImplOptions.NoOptimization )]
        public Int32 ReadCompilerOnlyFence() => this._value;

        /// <summary>Write the value applying release fence semantic</summary>
        /// <param name="newValue">The new value</param>
        public void WriteReleaseFence( Int32 newValue ) {
            this._value = newValue;
            Thread.MemoryBarrier();
        }

        /// <summary>Write the value applying full fence semantic</summary>
        /// <param name="newValue">The new value</param>
        public void WriteFullFence( Int32 newValue ) {
            this._value = newValue;
            Thread.MemoryBarrier();
        }

        /// <summary>Write the value applying a compiler fence only, no CPU fence is applied</summary>
        /// <param name="newValue">The new value</param>
        [MethodImpl( methodImplOptions: MethodImplOptions.NoOptimization )]
        public void WriteCompilerOnlyFence( Int32 newValue ) => this._value = newValue;

        /// <summary>Write without applying any fence</summary>
        /// <param name="newValue">The new value</param>
        public void WriteUnfenced( Int32 newValue ) => this._value = newValue;

        /// <summary>Atomically set the value to the given updated value if the current value equals the comparand</summary>
        /// <param name="newValue"> The new value</param>
        /// <param name="comparand">The comparand (expected value)</param>
        /// <returns></returns>
        public Boolean AtomicCompareExchange( Int32 newValue, Int32 comparand ) =>
            Interlocked.CompareExchange( location1: ref this._value, newValue, comparand: comparand ) == comparand;

        /// <summary>Atomically set the value to the given updated value</summary>
        /// <param name="newValue">The new value</param>
        /// <returns>The original value</returns>
        public Int32 AtomicExchange( Int32 newValue ) => Interlocked.Exchange( location1: ref this._value, newValue );

        /// <summary>Atomically add the given value to the current value and return the sum</summary>
        /// <param name="delta">The value to be added</param>
        /// <returns>The sum of the current value and the given value</returns>
        public Int32 AtomicAddAndGet( Int32 delta ) => Interlocked.Add( location1: ref this._value, delta );

        /// <summary>Atomically increment the current value and return the new value</summary>
        /// <returns>The incremented value.</returns>
        public Int32 AtomicIncrementAndGet() => Interlocked.Increment( location: ref this._value );

        /// <summary>Atomically increment the current value and return the new value</summary>
        /// <returns>The decremented value.</returns>
        public Int32 AtomicDecrementAndGet() => Interlocked.Decrement( location: ref this._value );

        /// <summary>Returns the String representation of the current value.</summary>
        /// <returns>the String representation of the current value.</returns>
        public override String ToString() {
            var value = this.ReadFullFence();

            return value.ToString();
        }
    }

    /// <summary>A long value that may be updated atomically and is guaranteed to live on its own cache line (to prevent false sharing)</summary>
    [StructLayout( layoutKind: LayoutKind.Explicit, Size = Volatiles.CacheLineSize * 2 )]
    public struct PaddedLong {

        [FieldOffset( offset: Volatiles.CacheLineSize )]
        private Int64 _value;

        /// <summary>Create a new <see cref="PaddedLong" /> with the given initial value.</summary>
        /// <param name="value">Initial value</param>
        public PaddedLong( Int64 value ) => this._value = value;

        /// <summary>Read the value without applying any fence</summary>
        /// <returns>The current value</returns>
        public Int64 ReadUnfenced() => this._value;

        /// <summary>Read the value applying acquire fence semantic</summary>
        /// <returns>The current value</returns>
        public Int64 ReadAcquireFence() {
            var value = this._value;
            Thread.MemoryBarrier();

            return value;
        }

        /// <summary>Read the value applying full fence semantic</summary>
        /// <returns>The current value</returns>
        public Int64 ReadFullFence() {
            Thread.MemoryBarrier();

            return this._value;
        }

        /// <summary>Read the value applying a compiler only fence, no CPU fence is applied</summary>
        /// <returns>The current value</returns>
        [MethodImpl( methodImplOptions: MethodImplOptions.NoOptimization )]
        public Int64 ReadCompilerOnlyFence() => this._value;

        /// <summary>Write the value applying release fence semantic</summary>
        /// <param name="newValue">The new value</param>
        public void WriteReleaseFence( Int64 newValue ) {
            Thread.MemoryBarrier();
            this._value = newValue;
        }

        /// <summary>Write the value applying full fence semantic</summary>
        /// <param name="newValue">The new value</param>
        public void WriteFullFence( Int64 newValue ) {
            Thread.MemoryBarrier();
            this._value = newValue;
        }

        /// <summary>Write the value applying a compiler fence only, no CPU fence is applied</summary>
        /// <param name="newValue">The new value</param>
        [MethodImpl( methodImplOptions: MethodImplOptions.NoOptimization )]
        public void WriteCompilerOnlyFence( Int64 newValue ) => this._value = newValue;

        /// <summary>Write without applying any fence</summary>
        /// <param name="newValue">The new value</param>
        public void WriteUnfenced( Int64 newValue ) => this._value = newValue;

        /// <summary>Atomically set the value to the given updated value if the current value equals the comparand</summary>
        /// <param name="newValue"> The new value</param>
        /// <param name="comparand">The comparand (expected value)</param>
        /// <returns></returns>
        public Boolean AtomicCompareExchange( Int64 newValue, Int64 comparand ) =>
            Interlocked.CompareExchange( location1: ref this._value, newValue, comparand: comparand ) == comparand;

        /// <summary>Atomically set the value to the given updated value</summary>
        /// <param name="newValue">The new value</param>
        /// <returns>The original value</returns>
        public Int64 AtomicExchange( Int64 newValue ) => Interlocked.Exchange( location1: ref this._value, newValue );

        /// <summary>Atomically add the given value to the current value and return the sum</summary>
        /// <param name="delta">The value to be added</param>
        /// <returns>The sum of the current value and the given value</returns>
        public Int64 AtomicAddAndGet( Int64 delta ) => Interlocked.Add( location1: ref this._value, delta );

        /// <summary>Atomically increment the current value and return the new value</summary>
        /// <returns>The incremented value.</returns>
        public Int64 AtomicIncrementAndGet() => Interlocked.Increment( location: ref this._value );

        /// <summary>Atomically increment the current value and return the new value</summary>
        /// <returns>The decremented value.</returns>
        public Int64 AtomicDecrementAndGet() => Interlocked.Decrement( location: ref this._value );

        /// <summary>Returns the String representation of the current value.</summary>
        /// <returns>the String representation of the current value.</returns>
        public override String ToString() {
            var value = this.ReadFullFence();

            return value.ToString();
        }
    }

    /// <summary>A reference that may be updated atomically</summary>
    public struct Reference<T> where T : class {

        private T _value;

        /// <summary>Create a new <see cref="Reference{T}" /> with the given initial value.</summary>
        /// <param name="value">Initial value</param>
        public Reference( [CanBeNull] T value ) => this._value = value;

        /// <summary>Atomically set the value to the given updated value if the current value equals the comparand</summary>
        /// <param name="newValue"> The new value</param>
        /// <param name="comparand">The comparand (expected value)</param>
        /// <returns></returns>
        public Boolean AtomicCompareExchange( [CanBeNull] T newValue, [CanBeNull] T comparand ) =>
            Interlocked.CompareExchange( location1: ref this._value, newValue, comparand: comparand ) == comparand;

        /// <summary>Atomically set the value to the given updated value</summary>
        /// <param name="newValue">The new value</param>
        /// <returns>The original value</returns>
        [CanBeNull]
        public T AtomicExchange( [CanBeNull] T newValue ) => Interlocked.Exchange( location1: ref this._value, newValue );

        /// <summary>Read the value applying acquire fence semantic</summary>
        /// <returns>The current value</returns>
        [CanBeNull]
        public T ReadAcquireFence() {
            var value = this._value;
            Thread.MemoryBarrier();

            return value;
        }

        /// <summary>Read the value applying a compiler only fence, no CPU fence is applied</summary>
        /// <returns>The current value</returns>
        [MethodImpl( methodImplOptions: MethodImplOptions.NoOptimization )]
        [CanBeNull]
        public T ReadCompilerOnlyFence() => this._value;

        /// <summary>Read the value applying full fence semantic</summary>
        /// <returns>The current value</returns>
        [CanBeNull]
        public T ReadFullFence() {
            var value = this._value;
            Thread.MemoryBarrier();

            return value;
        }

        /// <summary>Read the value without applying any fence</summary>
        /// <returns>The current value</returns>
        [CanBeNull]
        public T ReadUnfenced() => this._value;

        /// <summary>Returns the String representation of the current value.</summary>
        /// <returns>the String representation of the current value.</returns>
        public override String ToString() {
            var value = this.ReadFullFence();

            return value.ToString();
        }

        /// <summary>Write the value applying a compiler fence only, no CPU fence is applied</summary>
        /// <param name="newValue">The new value</param>
        [MethodImpl( methodImplOptions: MethodImplOptions.NoOptimization )]
        public void WriteCompilerOnlyFence( [CanBeNull] T newValue ) => this._value = newValue;

        /// <summary>Write the value applying full fence semantic</summary>
        /// <param name="newValue">The new value</param>
        public void WriteFullFence( [CanBeNull] T newValue ) {
            Thread.MemoryBarrier();
            this._value = newValue;
        }

        /// <summary>Write the value applying release fence semantic</summary>
        /// <param name="newValue">The new value</param>
        public void WriteReleaseFence( [CanBeNull] T newValue ) {
            Thread.MemoryBarrier();
            this._value = newValue;
        }

        /// <summary>Write without applying any fence</summary>
        /// <param name="newValue">The new value</param>
        public void WriteUnfenced( [CanBeNull] T newValue ) => this._value = newValue;
    }

    /// <summary>A small toolkit of classes that support lock-free thread-safe programming on single variables and arrays</summary>
    /// <see cref="http://github.com/disruptor-net/Disruptor-net/blob/master/Atomic/Volatile.cs" date="Dec 30, 2011" />
    public static class Volatiles {

        /// <summary>Size of a cache line in bytes</summary>
        public const Int32 CacheLineSize = 64;
    }

    /// <summary>A <see cref="Boolean" /> array that may be updated atomically</summary>
    public class BooleanArray {

        private const Int32 False = 0;

        private const Int32 True = 1;

        [NotNull]
        private Int32[] _array { get; }

        /// <summary>Length of the array</summary>
        public Int32 Length => this._array.Length;

        /// <summary>Create a new <see cref="BooleanArray" /> of a given length</summary>
        /// <param name="length">Length of the array</param>
        public BooleanArray( Int32 length ) {
            if ( length < 1 ) {
                throw new ArgumentOutOfRangeException( nameof( length ) );
            }

            this._array = new Int32[ length ];
        }

        /// <summary>Create a new <see cref="BooleanArray" /> with the same length as, and all elements copied from, the given array.</summary>
        /// <param name="array"></param>
        public BooleanArray( [NotNull] IEnumerable<Boolean> array ) => this._array = array.Select( selector: ToInt ).ToArray();

        private static Boolean ToBool( Int32 value ) {
            if ( value != False && value != True ) {
                throw new ArgumentOutOfRangeException( nameof( value ) );
            }

            return value == True;
        }

        private static Int32 ToInt( Boolean value ) => value ? True : False;

        /// <summary>Atomically set the value to the given updated value if the current value equals the comparand</summary>
        /// <param name="newValue"> The new value</param>
        /// <param name="comparand">The comparand (expected value)</param>
        /// <param name="index">    The index.</param>
        /// <returns>The original value</returns>
        public Boolean AtomicCompareExchange( Int32 index, Boolean newValue, Boolean comparand ) {
            var newValueInt = ToInt( newValue );
            var comparandInt = ToInt( comparand );

            return Interlocked.CompareExchange( location1: ref this._array[ index ], newValueInt, comparand: comparandInt ) == comparandInt;
        }

        /// <summary>Atomically set the value to the given updated value</summary>
        /// <param name="newValue">The new value</param>
        /// <param name="index">   The index.</param>
        /// <returns>The original value</returns>
        public Boolean AtomicExchange( Int32 index, Boolean newValue ) {
            var result = Interlocked.Exchange( location1: ref this._array[ index ], ToInt( newValue ) );

            return ToBool( result );
        }

        /// <summary>Read the value applying acquire fence semantic</summary>
        /// <param name="index">The element index</param>
        /// <returns>The current value</returns>
        public Boolean ReadAcquireFence( Int32 index ) {
            var value = this._array[ index ];
            Thread.MemoryBarrier();

            return ToBool( value );
        }

        /// <summary>Read the value applying a compiler only fence, no CPU fence is applied</summary>
        /// <param name="index">The element index</param>
        /// <returns>The current value</returns>
        [MethodImpl( methodImplOptions: MethodImplOptions.NoOptimization )]
        public Boolean ReadCompilerOnlyFence( Int32 index ) => ToBool( this._array[ index ] );

        /// <summary>Read the value applying full fence semantic</summary>
        /// <param name="index">The element index</param>
        /// <returns>The current value</returns>
        public Boolean ReadFullFence( Int32 index ) {
            var value = this._array[ index ];
            Thread.MemoryBarrier();

            return ToBool( value );
        }

        /// <summary>Read the value without applying any fence</summary>
        /// <param name="index">The index of the element.</param>
        /// <returns>The current value.</returns>
        public Boolean ReadUnfenced( Int32 index ) => ToBool( this._array[ index ] );

        /// <summary>Write the value applying a compiler fence only, no CPU fence is applied</summary>
        /// <param name="index">   The element index</param>
        /// <param name="newValue">The new value</param>
        [MethodImpl( methodImplOptions: MethodImplOptions.NoOptimization )]
        public void WriteCompilerOnlyFence( Int32 index, Boolean newValue ) => this._array[ index ] = ToInt( newValue );

        /// <summary>Write the value applying full fence semantic</summary>
        /// <param name="index">   The element index</param>
        /// <param name="newValue">The new value</param>
        public void WriteFullFence( Int32 index, Boolean newValue ) {
            this._array[ index ] = ToInt( newValue );
            Thread.MemoryBarrier();
        }

        /// <summary>Write the value applying release fence semantic</summary>
        /// <param name="index">   The element index</param>
        /// <param name="newValue">The new value</param>
        public void WriteReleaseFence( Int32 index, Boolean newValue ) {
            this._array[ index ] = ToInt( newValue );
            Thread.MemoryBarrier();
        }

        /// <summary>Write without applying any fence</summary>
        /// <param name="index">   The index.</param>
        /// <param name="newValue">The new value</param>
        public void WriteUnfenced( Int32 index, Boolean newValue ) => this._array[ index ] = ToInt( newValue );
    }

    /// <summary>An <see cref="Int32" /> array that may be updated atomically</summary>
    public class IntegerArray {

        [NotNull]
        private Int32[] _array { get; }

        /// <summary>Length of the array</summary>
        public Int32 Length => this._array.Length;

        /// <summary>Create a new <see cref="IntegerArray" /> of a given length</summary>
        /// <param name="length">Length of the array</param>
        public IntegerArray( Int32 length ) {
            if ( length <= 0 ) {
                throw new ArgumentOutOfRangeException( nameof( length ) );
            }

            this._array = new Int32[ length ];
        }

        /// <summary>Create a new AtomicIntegerArray with the same length as, and all elements copied from, the given array.</summary>
        /// <param name="array"></param>
        public IntegerArray( [NotNull] Int32[] array ) {
            if ( array is null ) {
                throw new ArgumentNullException( nameof( array ) );
            }

            this._array = new Int32[ array.Length ];
            array.CopyTo( array: this._array, index: 0 );
        }

        /// <summary>Atomically add the given value to the current value and return the sum</summary>
        /// <param name="delta">The value to be added</param>
        /// <param name="index">The index.</param>
        /// <returns>The sum of the current value and the given value</returns>
        public Int32 AtomicAddAndGet( Int32 index, Int32 delta ) => Interlocked.Add( location1: ref this._array[ index ], delta );

        /// <summary>Atomically set the value to the given updated value if the current value equals the comparand</summary>
        /// <param name="newValue"> The new value</param>
        /// <param name="comparand">The comparand (expected value)</param>
        /// <param name="index">    The index.</param>
        /// <returns>The original value</returns>
        public Boolean AtomicCompareExchange( Int32 index, Int32 newValue, Int32 comparand ) =>
            Interlocked.CompareExchange( location1: ref this._array[ index ], newValue, comparand: comparand ) == comparand;

        /// <summary>Atomically increment the current value and return the new value</summary>
        /// <param name="index">The index.</param>
        /// <returns>The decremented value.</returns>
        public Int32 AtomicDecrementAndGet( Int32 index ) => Interlocked.Decrement( location: ref this._array[ index ] );

        /// <summary>Atomically set the value to the given updated value</summary>
        /// <param name="newValue">The new value</param>
        /// <param name="index">   The index.</param>
        /// <returns>The original value</returns>
        public Int32 AtomicExchange( Int32 index, Int32 newValue ) => Interlocked.Exchange( location1: ref this._array[ index ], newValue );

        /// <summary>Atomically increment the current value and return the new value</summary>
        /// <param name="index">The index.</param>
        /// <returns>The incremented value.</returns>
        public Int32 AtomicIncrementAndGet( Int32 index ) => Interlocked.Increment( location: ref this._array[ index ] );

        /// <summary>Read the value applying acquire fence semantic</summary>
        /// <param name="index">The element index</param>
        /// <returns>The current value</returns>
        public Int32 ReadAcquireFence( Int32 index ) {
            var value = this._array[ index ];
            Thread.MemoryBarrier();

            return value;
        }

        /// <summary>Read the value applying a compiler only fence, no CPU fence is applied</summary>
        /// <param name="index">The element index</param>
        /// <returns>The current value</returns>
        [MethodImpl( methodImplOptions: MethodImplOptions.NoOptimization )]
        public Int32 ReadCompilerOnlyFence( Int32 index ) => this._array[ index ];

        /// <summary>Read the value applying full fence semantic</summary>
        /// <param name="index">The element index</param>
        /// <returns>The current value</returns>
        public Int32 ReadFullFence( Int32 index ) {
            var value = this._array[ index ];
            Thread.MemoryBarrier();

            return value;
        }

        /// <summary>Read the value without applying any fence</summary>
        /// <param name="index">The index of the element.</param>
        /// <returns>The current value.</returns>
        public Int32 ReadUnfenced( Int32 index ) => this._array[ index ];

        /// <summary>Write the value applying a compiler fence only, no CPU fence is applied</summary>
        /// <param name="index">   The element index</param>
        /// <param name="newValue">The new value</param>
        [MethodImpl( methodImplOptions: MethodImplOptions.NoOptimization )]
        public void WriteCompilerOnlyFence( Int32 index, Int32 newValue ) => this._array[ index ] = newValue;

        /// <summary>Write the value applying full fence semantic</summary>
        /// <param name="index">   The element index</param>
        /// <param name="newValue">The new value</param>
        public void WriteFullFence( Int32 index, Int32 newValue ) {
            this._array[ index ] = newValue;
            Thread.MemoryBarrier();
        }

        /// <summary>Write the value applying release fence semantic</summary>
        /// <param name="index">   The element index</param>
        /// <param name="newValue">The new value</param>
        public void WriteReleaseFence( Int32 index, Int32 newValue ) {
            this._array[ index ] = newValue;
            Thread.MemoryBarrier();
        }

        /// <summary>Write without applying any fence</summary>
        /// <param name="index">   The index.</param>
        /// <param name="newValue">The new value</param>
        public void WriteUnfenced( Int32 index, Int32 newValue ) => this._array[ index ] = newValue;
    }

    /// <summary>A <see cref="Int64" /> array that may be updated atomically</summary>
    public class LongArray {

        [NotNull]
        private Int64[] _array { get; }

        /// <summary>Length of the array</summary>
        public Int32 Length => this._array.Length;

        /// <summary>Create a new <see cref="LongArray" /> of a given length</summary>
        /// <param name="length">Length of the array</param>
        public LongArray( Int32 length ) {
            if ( length <= 0 ) {
                throw new ArgumentOutOfRangeException( nameof( length ) );
            }

            this._array = new Int64[ length ];
        }

        /// <summary>Create a new <see cref="LongArray" /> with the same length as, and all elements copied from, the given array.</summary>
        /// <param name="array"></param>
        public LongArray( [NotNull] Int64[] array ) {
            if ( array is null ) {
                throw new ArgumentNullException( nameof( array ) );
            }

            this._array = new Int64[ array.Length ];
            array.CopyTo( array: this._array, index: 0 );
        }

        /// <summary>Atomically add the given value to the current value and return the sum</summary>
        /// <param name="delta">The value to be added</param>
        /// <param name="index">The index.</param>
        /// <returns>The sum of the current value and the given value</returns>
        public Int64 AtomicAddAndGet( Int32 index, Int64 delta ) => Interlocked.Add( location1: ref this._array[ index ], delta );

        /// <summary>Atomically set the value to the given updated value if the current value equals the comparand</summary>
        /// <param name="newValue"> The new value</param>
        /// <param name="comparand">The comparand (expected value)</param>
        /// <param name="index">    The index.</param>
        /// <returns>The original value</returns>
        public Boolean AtomicCompareExchange( Int32 index, Int64 newValue, Int64 comparand ) =>
            Interlocked.CompareExchange( location1: ref this._array[ index ], newValue, comparand: comparand ) == comparand;

        /// <summary>Atomically increment the current value and return the new value</summary>
        /// <param name="index">The index.</param>
        /// <returns>The decremented value.</returns>
        public Int64 AtomicDecrementAndGet( Int32 index ) => Interlocked.Decrement( location: ref this._array[ index ] );

        /// <summary>Atomically set the value to the given updated value</summary>
        /// <param name="newValue">The new value</param>
        /// <param name="index">   The index.</param>
        /// <returns>The original value</returns>
        public Int64 AtomicExchange( Int32 index, Int64 newValue ) => Interlocked.Exchange( location1: ref this._array[ index ], newValue );

        /// <summary>Atomically increment the current value and return the new value</summary>
        /// <param name="index">The index.</param>
        /// <returns>The incremented value.</returns>
        public Int64 AtomicIncrementAndGet( Int32 index ) => Interlocked.Increment( location: ref this._array[ index ] );

        /// <summary>Read the value applying acquire fence semantic</summary>
        /// <param name="index">The element index</param>
        /// <returns>The current value</returns>
        public Int64 ReadAcquireFence( Int32 index ) {
            var value = this._array[ index ];
            Thread.MemoryBarrier();

            return value;
        }

        /// <summary>Read the value applying a compiler only fence, no CPU fence is applied</summary>
        /// <param name="index">The element index</param>
        /// <returns>The current value</returns>
        [MethodImpl( methodImplOptions: MethodImplOptions.NoOptimization )]
        public Int64 ReadCompilerOnlyFence( Int32 index ) => this._array[ index ];

        /// <summary>Read the value applying full fence semantic</summary>
        /// <param name="index">The element index</param>
        /// <returns>The current value</returns>
        public Int64 ReadFullFence( Int32 index ) {
            var value = this._array[ index ];
            Thread.MemoryBarrier();

            return value;
        }

        /// <summary>Read the value without applying any fence</summary>
        /// <param name="index">The index of the element.</param>
        /// <returns>The current value.</returns>
        public Int64 ReadUnfenced( Int32 index ) => this._array[ index ];

        /// <summary>Write the value applying a compiler fence only, no CPU fence is applied</summary>
        /// <param name="index">   The element index</param>
        /// <param name="newValue">The new value</param>
        [MethodImpl( methodImplOptions: MethodImplOptions.NoOptimization )]
        public void WriteCompilerOnlyFence( Int32 index, Int64 newValue ) => this._array[ index ] = newValue;

        /// <summary>Write the value applying full fence semantic</summary>
        /// <param name="index">   The element index</param>
        /// <param name="newValue">The new value</param>
        public void WriteFullFence( Int32 index, Int64 newValue ) {
            this._array[ index ] = newValue;
            Thread.MemoryBarrier();
        }

        /// <summary>Write the value applying release fence semantic</summary>
        /// <param name="index">   The element index</param>
        /// <param name="newValue">The new value</param>
        public void WriteReleaseFence( Int32 index, Int64 newValue ) {
            this._array[ index ] = newValue;
            Thread.MemoryBarrier();
        }

        /// <summary>Write without applying any fence</summary>
        /// <param name="index">   The index.</param>
        /// <param name="newValue">The new value</param>
        public void WriteUnfenced( Int32 index, Int64 newValue ) => this._array[ index ] = newValue;
    }

    /// <summary>A reference array that may be updated atomically</summary>
    public class ReferenceArray<T> where T : class {

        [NotNull]
        private T[] _array { get; }

        /// <summary>Length of the array</summary>
        public Int32 Length => this._array.Length;

        /// <summary>Create a new <see cref="ReferenceArray{T}" /> of a given length</summary>
        /// <param name="length">Length of the array</param>
        public ReferenceArray( Int32 length ) {
            if ( length <= 0 ) {
                throw new ArgumentOutOfRangeException( nameof( length ) );
            }

            this._array = new T[ length ];
        }

        /// <summary>Create a new <see cref="ReferenceArray{T}" /> with the same length as, and all elements copied from, the given array.</summary>
        /// <param name="array"></param>
        public ReferenceArray( [NotNull] IEnumerable<T> array ) => this._array = array.ToArray();

        /// <summary>Atomically set the value to the given updated value if the current value equals the comparand</summary>
        /// <param name="newValue"> The new value</param>
        /// <param name="comparand">The comparand (expected value)</param>
        /// <param name="index">    The index.</param>
        /// <returns>The original value</returns>
        public Boolean AtomicCompareExchange( Int32 index, [CanBeNull] T newValue, [CanBeNull] T comparand ) =>
            Interlocked.CompareExchange( location1: ref this._array[ index ], newValue, comparand: comparand ) == comparand;

        /// <summary>Atomically set the value to the given updated value</summary>
        /// <param name="newValue">The new value</param>
        /// <param name="index">   The index.</param>
        /// <returns>The original value</returns>
        [CanBeNull]
        public T AtomicExchange( Int32 index, [CanBeNull] T newValue ) {
            var result = Interlocked.Exchange( location1: ref this._array[ index ], newValue );

            return result;
        }

        /// <summary>Read the value applying acquire fence semantic</summary>
        /// <param name="index">The element index</param>
        /// <returns>The current value</returns>
        [CanBeNull]
        public T ReadAcquireFence( Int32 index ) {
            var value = this._array[ index ];
            Thread.MemoryBarrier();

            return value;
        }

        /// <summary>Read the value applying a compiler only fence, no CPU fence is applied</summary>
        /// <param name="index">The element index</param>
        /// <returns>The current value</returns>
        [MethodImpl( methodImplOptions: MethodImplOptions.NoOptimization )]
        [CanBeNull]
        public T ReadCompilerOnlyFence( Int32 index ) => this._array[ index ];

        /// <summary>Read the value applying full fence semantic</summary>
        /// <param name="index">The element index</param>
        /// <returns>The current value</returns>
        [CanBeNull]
        public T ReadFullFence( Int32 index ) {
            var value = this._array[ index ];
            Thread.MemoryBarrier();

            return value;
        }

        /// <summary>Read the value without applying any fence</summary>
        /// <param name="index">The index of the element.</param>
        /// <returns>The current value.</returns>
        [CanBeNull]
        public T ReadUnfenced( Int32 index ) => this._array[ index ];

        /// <summary>Write the value applying a compiler fence only, no CPU fence is applied</summary>
        /// <param name="index">   The element index</param>
        /// <param name="newValue">The new value</param>
        [MethodImpl( methodImplOptions: MethodImplOptions.NoOptimization )]
        public void WriteCompilerOnlyFence( Int32 index, [CanBeNull] T newValue ) => this._array[ index ] = newValue;

        /// <summary>Write the value applying full fence semantic</summary>
        /// <param name="index">   The element index</param>
        /// <param name="newValue">The new value</param>
        public void WriteFullFence( Int32 index, [CanBeNull] T newValue ) {
            this._array[ index ] = newValue;
            Thread.MemoryBarrier();
        }

        /// <summary>Write the value applying release fence semantic</summary>
        /// <param name="index">   The element index</param>
        /// <param name="newValue">The new value</param>
        public void WriteReleaseFence( Int32 index, [CanBeNull] T newValue ) {
            this._array[ index ] = newValue;
            Thread.MemoryBarrier();
        }

        /// <summary>Write without applying any fence</summary>
        /// <param name="index">   The index.</param>
        /// <param name="newValue">The new value</param>
        public void WriteUnfenced( Int32 index, [CanBeNull] T newValue ) => this._array[ index ] = newValue;
    }
}