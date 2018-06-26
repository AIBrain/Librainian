// Copyright © 1995-2018 to Rick@AIBrain.org and Protiguous. All Rights Reserved.
// 
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
// 
// This source code contained in "StopwatchStruct.cs" belongs to Rick@AIBrain.org and
// Protiguous@Protiguous.com unless otherwise specified or the original license has
// been overwritten by automatic formatting.
// (We try to avoid it from happening, but it does accidentally happen.)
// 
// Any unmodified portions of source code gleaned from other projects still retain their original
// license and our thanks goes to those Authors. If you find your code in this source code, please
// let us know so we can properly attribute you and include the proper license and/or copyright.
// 
// Donations, royalties from any software that uses any of our code, or license fees can be paid
// to us via bitcoin at the address 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2.
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
// For business inquiries, please contact me at Protiguous@Protiguous.com .
// 
// Our software can be found at "https://Protiguous.Software/"
// Our GitHub address is "https://github.com/Protiguous".
// Feel free to browse any source code we might have available.
// 
// ***  Project "Librainian"  ***
// File "StopwatchStruct.cs" was last formatted by Protiguous on 2018/06/04 at 4:16 PM.

namespace Librainian.Measurement.Time {

	using System;
	using System.Runtime.InteropServices;
	using System.Runtime.Versioning;

	/// <summary>
	///     A value-type implementation of Stopwatch. It allows for high precision benchmarking without needing to instantiate
	///     reference types on the managed heap.
	///     Because of native calls to QueryPerformanceCounter, this struct will only work on Windows.
	///     Note: because this is a struct, instances will be passed by value. This means every time you assign an instance to
	///     another variable, or pass it as a
	///     parameter, a copy is made. Calling methods like Stop() on one copy, will not have any effect on other copies.
	///     Unless you are comfortable with the
	///     implications of a pass-by-value mutable struct, it is highly recommended that you only assign each instance to one
	///     variable, and do not pass it to
	///     other methods.
	/// </summary>
	/// <copyright>bret@atlantisflight.org</copyright>
	/// <from>https://github.com/bretcope/PerformanceTypes/blob/master/PerformanceTypes/StopwatchStruct.cs</from>
	public struct StopwatchStruct {

		private Int64 _elapsedCounts;

		private Int64 _startTimestamp;

		/// <summary>
		///     The resolution of QueryPerformanceCounter (in "counts" per second).
		/// </summary>
		public static Double CountsPerMillisecond { get; }

		/// <summary>
		///     The resolution of QueryPerformanceCounter (in "counts" per second).
		/// </summary>
		public static Int64 CountsPerSecond { get; }

		public static Boolean IsHighResolution => true;

		/// <summary>
		///     Gets the total elapsed time that the stopwatch has run. This property is accurate, even while the stopwatch is
		///     running.
		/// </summary>
		public TimeSpan Elapsed => TimeSpan.FromMilliseconds( this.GetElapsedMilliseconds() );

		/// <summary>
		///     The number of "counts" that the stopwatch has run. This property is accurate, even while the stopwatch is running.
		/// </summary>
		public Int64 ElapsedCounts {
			get {
				if ( !this.IsRunning ) { return this._elapsedCounts; }

				QueryPerformanceCounter( out var timestamp );

				return this._elapsedCounts + ( timestamp - this._startTimestamp );
			}
		}

		/// <summary>
		///     True if the stopwatch is running. In other words, Start() has been called, but Stop() has not.
		/// </summary>
		public Boolean IsRunning { get; private set; }

		static StopwatchStruct() {
			if ( !QueryPerformanceFrequency( out var countsPerSecond ) ) { return; }

			CountsPerSecond = countsPerSecond;
			CountsPerMillisecond = countsPerSecond / 1000.0;
		}

		/// <summary>
		///     Calls the WinAPI QueryPerformanceCounter method (the Windows high-resolution time).
		///     See: https://msdn.microsoft.com/en-us/library/windows/desktop/ms644904(v=vs.85).aspx
		/// </summary>
		/// <param name="value">The current high-resolution time value in ticks.</param>
		/// <returns>True if the call succeeded.</returns>
		[DllImport( "kernel32.dll" )]
		[ResourceExposure( ResourceScope.None )]
		public static extern Boolean QueryPerformanceCounter( out Int64 value );

		/// <summary>
		///     Calls the WinAPI QueryPerformanceFrequency method. This method allows you to determine the resolution of
		///     QueryPerformanceCounter.
		///     See: https://msdn.microsoft.com/en-us/library/windows/desktop/ms644905(v=vs.85).aspx
		/// </summary>
		/// <param name="value">The number of ticks </param>
		/// <returns>True if the call succeeded.</returns>
		[DllImport( "kernel32.dll" )]
		[ResourceExposure( ResourceScope.None )]
		public static extern Boolean QueryPerformanceFrequency( out Int64 value );

		/// <summary>
		///     Returns the total number of milliseconds that the stopwatch has run. This property is accurate, even while the
		///     stopwatch is running.
		/// </summary>
		public Double GetElapsedMilliseconds() => this.ElapsedCounts / CountsPerMillisecond;

		/// <summary>
		///     Starts the stopwatch by marking the start time.
		/// </summary>
		public void Start() {
			if ( this.IsRunning ) { return; }

			this.IsRunning = true;
			QueryPerformanceCounter( out this._startTimestamp );
		}

		/// <summary>
		///     Stops the stopwatch, if it was running. Calculates the elapsed ticks since it was last started, and adds them to
		///     <see cref="ElapsedCounts" />.
		/// </summary>
		public void Stop() {
			if ( !this.IsRunning ) { return; }

			QueryPerformanceCounter( out var timestamp );
			this.IsRunning = false;
			this._elapsedCounts += timestamp - this._startTimestamp;
		}

	}

}