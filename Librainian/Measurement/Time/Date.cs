// Copyright © Protiguous. All Rights Reserved.
// This entire copyright notice and license must be retained and must be kept visible in any binaries, libraries, repositories, or source code (directly or derived) from our binaries, libraries, projects, solutions, or applications.
// All source code belongs to Protiguous@Protiguous.com unless otherwise specified or the original license has been overwritten by formatting.
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
//
// Our software can be found at "https://Protiguous.com/Software"
// Our GitHub address is "https://github.com/Protiguous".
//
// File "Date.cs" last formatted on 2021-02-03 at 3:18 PM.

namespace Librainian.Measurement.Time {

	using System;
	using System.Numerics;
	using Clocks;
	using Extensions;
	using Newtonsoft.Json;

	/// <summary>
	///     <see cref="Year" />, <see cref="Month" />, and <see cref="Day" />.
	/// </summary>
	[Immutable]
	[JsonObject]
	public record Date( Year Year, Month Month, Day Day ) {
		public static readonly Date Zero = new( Year.Zero, Month.MinimumValue, Day.MinimumValue );

		public Date( BigInteger year, Byte month, Byte day ) : this( new Year( year ), new Month( month ), new Day( day ) ) {
			while ( day > Day.Maximum ) {
				day -= Day.MaximumValue;
				month++;

				while ( month > Month.MaximumValue ) {
					month -= Month.MaximumValue;
					year++;
				}
			}

			this.Day = new Day( day );

			while ( month > Month.MaximumValue ) {
				month -= Month.MaximumValue;
				year++;
			}

			this.Month = new Month( month );

			this.Year = new Year( year );
		}

		public Date( DateTime dateTime ) : this( dateTime.Year, ( Byte )dateTime.Month, ( Byte )dateTime.Day ) { }

		public Date( SpanOfTime spanOfTime ) : this( new Year( spanOfTime.GetWholeYears() ), new Month( ( Byte )(UInt32)spanOfTime.Months.Value ),
			new Day( ( Byte )(UInt32)spanOfTime.Days.Value ) ) { }

		public static Date Now => new( DateTime.Now );

		public static Date UtcNow => new( DateTime.UtcNow );

		public static implicit operator DateTime?( Date date ) => TimeExtensions.TryConvertToDateTime( date, out var dateTime ) ? dateTime : default( DateTime? );

		public static Boolean operator <( Date left, Date right ) => left.ToSpanOfTime().CalcTotalPlanckTimes() < right.ToSpanOfTime().CalcTotalPlanckTimes();

		public static Boolean operator <=( Date left, Date right ) => left.ToSpanOfTime().CalcTotalPlanckTimes() <= right.ToSpanOfTime().CalcTotalPlanckTimes();

		public static Boolean operator >( Date left, Date right ) => left.ToSpanOfTime().CalcTotalPlanckTimes() > right.ToSpanOfTime().CalcTotalPlanckTimes();

		public static Boolean operator >=( Date left, Date right ) => left.ToSpanOfTime().CalcTotalPlanckTimes() >= right.ToSpanOfTime().CalcTotalPlanckTimes();

		public Boolean TryConvertToDateTime( out DateTime? dateTime ) => TimeExtensions.TryConvertToDateTime( this, out dateTime );
	}
}