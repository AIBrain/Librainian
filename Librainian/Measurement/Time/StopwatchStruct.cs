// Copyright © Protiguous. All Rights Reserved.
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
//
// Disclaimer:  Usage of the source code or binaries is AS-IS.
// No warranties are expressed, implied, or given.
// We are NOT responsible for Anything You Do With Our Code.
// We are NOT responsible for Anything You Do With Our Executables.
// We are NOT responsible for Anything You Do With Your Computer.
//
//
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
// For business inquiries, please contact me at Protiguous@Protiguous.com.
// Our software can be found at "https://Protiguous.com/Software/"
// Our GitHub address is "https://github.com/Protiguous".
//
// File "StopwatchStruct.cs" last formatted on 2022-12-22 at 5:18 PM by Protiguous.

namespace Librainian.Measurement.Time;

using System;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using OperatingSystem;

/// <summary>
///     A value-type implementation of Stopwatch. It allows for high precision benchmarking without needing to instantiate
///     reference types on the managed heap. Because of native
///     calls to QueryPerformanceCounter, this struct will only work on Windows. Note: because this is a struct, instances
///     will be passed by value. This means every time you assign an
///     instance to another variable, or pass it as a parameter, a copy is made. Calling methods like Stop() on one copy,
///     will not have any effect on other copies. Unless you are
///     comfortable with the implications of a pass-by-value mutable struct, it is highly recommended that you only assign
///     each instance to one variable, and do not pass it to other
///     methods.
/// </summary>
/// <copyright>bret@atlantisflight.org</copyright>
/// <from>https://github.com/bretcope/PerformanceTypes/blob/master/PerformanceTypes/StopwatchStruct.cs</from>
public struct StopwatchStruct {

	private Int64 _elapsedCounts;

	private Int64 _startTimestamp;

	static StopwatchStruct() {
		if ( !StopwatchDLLs.QueryPerformanceFrequency( out var countsPerSecond ) ) {
			return;
		}

		CountsPerSecond = countsPerSecond;
		CountsPerMillisecond = countsPerSecond / 1000.0;
	}

	/// <summary>The resolution of QueryPerformanceCounter (in "counts" per second).</summary>
	public static Double CountsPerMillisecond { get; }

	/// <summary>The resolution of QueryPerformanceCounter (in "counts" per second).</summary>
	public static Int64 CountsPerSecond { get; }

	public static Boolean IsHighResolution => true;

	/// <summary>
	///     Gets the total elapsed time that the stopwatch has run. This property is accurate, even while the stopwatch is
	///     running.
	/// </summary>
	public TimeSpan Elapsed => TimeSpan.FromMilliseconds( this.GetElapsedMilliseconds() );

	/// <summary>
	///     The number of "counts" that the stopwatch has run. This property is accurate, even while the stopwatch is
	///     running.
	/// </summary>
	public Int64 ElapsedCounts {
		get {
			if ( !this.IsRunning ) {
				return this._elapsedCounts;
			}

			StopwatchDLLs.QueryPerformanceCounter( out var timestamp );

			return this._elapsedCounts + ( timestamp - this._startTimestamp );
		}
	}

	/// <summary>True if the stopwatch is running. In other words, Start() has been called, but Stop() has not.</summary>
	public Boolean IsRunning { get; private set; }

	/// <summary>
	///     Returns the total number of milliseconds that the stopwatch has run. This property is accurate, even while the
	///     stopwatch is running.
	/// </summary>
	public Double GetElapsedMilliseconds() => this.ElapsedCounts / CountsPerMillisecond;

	/// <summary>Starts the stopwatch by marking the start time.</summary>
	public void Start() {
		if ( this.IsRunning ) {
			return;
		}

		this.IsRunning = true;
		StopwatchDLLs.QueryPerformanceCounter( out this._startTimestamp );
	}

	/// <summary>
	///     Stops the stopwatch, if it was running. Calculates the elapsed ticks since it was last started, and adds them
	///     to <see cref="ElapsedCounts" />.
	/// </summary>
	public void Stop() {
		if ( !this.IsRunning ) {
			return;
		}

		StopwatchDLLs.QueryPerformanceCounter( out var timestamp );
		this.IsRunning = false;
		this._elapsedCounts += timestamp - this._startTimestamp;
	}
}

public static class StopwatchDLLs {

	/// <summary>
	///     Calls the WinAPI QueryPerformanceCounter method (the Windows high-resolution time). See:
	///     https://msdn.microsoft.com/en-us/library/windows/desktop/ms644904(v=vs.85).aspx
	/// </summary>
	/// <param name="lpPerformanceCount">The current high-resolution time value in ticks.</param>
	/// <returns>True if the call succeeded.</returns>
	[ResourceExposure( ResourceScope.None )]
	[DllImport( ExternDll.Kernel32, ThrowOnUnmappableChar = true, CharSet = CharSet.Auto, BestFitMapping = false, ExactSpelling = false )]
	internal static extern Boolean QueryPerformanceCounter( out Int64 lpPerformanceCount );

	/// <summary>
	///     Calls the WinAPI QueryPerformanceFrequency method. This method allows you to determine the resolution of
	///     QueryPerformanceCounter. See:
	///     https://msdn.microsoft.com/en-us/library/windows/desktop/ms644905(v=vs.85).aspx
	/// </summary>
	/// <param name="lpFrequency">The number of ticks </param>
	/// <returns>True if the call succeeded.</returns>
	[ResourceExposure( ResourceScope.None )]
	[DllImport( ExternDll.Kernel32, ThrowOnUnmappableChar = true, CharSet = CharSet.Auto, BestFitMapping = false, ExactSpelling = false )]
	internal static extern Boolean QueryPerformanceFrequency( out Int64 lpFrequency );
}