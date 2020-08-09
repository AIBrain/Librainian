namespace Librainian.Measurement.Time {

	using System;
	using System.Diagnostics;
	using System.Numerics;
	using Extensions;
	using Newtonsoft.Json;

	/// <summary>
	///     <para>Absolute universal date and time.</para>
	///     <para><see cref="PlanckTimes" /> since the big bang of <i>this</i> universe.</para>
	/// </summary>
	/// <see cref="http://wikipedia.org/wiki/Lol" />
	[Immutable]
	[JsonObject]
	[DebuggerDisplay( "ToString()" )]
	public struct UniversalDateTime : IComparable<UniversalDateTime> {

		public static UniversalDateTime Now => new UniversalDateTime( DateTime.UtcNow );

		/// <summary>
		///     <para>1 planck times</para>
		/// </summary>
		public static UniversalDateTime One { get; } = new UniversalDateTime( BigInteger.One );

		/// <summary>
		///     <para>The value of this constant is equivalent to 00:00:00.0000000, January 1, 0001.</para>
		///     <para>430,000,000,000,000,000 seconds</para>
		/// </summary>
		public static PlanckTimes PlancksUpToMinDateTime { get; } = new PlanckTimes( new Seconds( 4.3E17m ) );

		/// <summary>
		///     <para>0 planck times</para>
		/// </summary>
		public static UniversalDateTime TheBeginning { get; } = new UniversalDateTime( BigInteger.Zero );

		public static UniversalDateTime Unix { get; } = new UniversalDateTime( Epochs.Unix );

		/// <summary></summary>
		[JsonProperty]
		public Date Date { get; }

		/// <summary></summary>
		[JsonProperty]
		public Time Time { get; }

		/// <summary>
		///     <para><see cref="PlanckTimes" /> since the big bang of <i>this</i> universe.</para>
		/// </summary>
		[JsonProperty]
		public BigInteger Value { get; }

		public UniversalDateTime( BigInteger planckTimesSinceBigBang ) {
			this.Value = planckTimesSinceBigBang;
			var span = new SpanOfTime( this.Value );

			//TODO
			this.Date = new Date( span );
			this.Time = new Time( span );
		}

		public UniversalDateTime( DateTime dateTime ) {
			var span = CalcSpanSince( dateTime );

			this.Value = span.CalcTotalPlanckTimes().Value;
			this.Date = new Date( span ); //we can use span here because the values have been normalized. Should()Have()Been()?
			this.Time = new Time( span );

			//this.Time = new Time();
		}

		private static UniversalDateTime Combine( UniversalDateTime left, BigInteger value ) => new UniversalDateTime( left.Value + value );

		private static UniversalDateTime Combine( UniversalDateTime left, UniversalDateTime right ) => Combine( left, right.Value );

		/// <summary>Given a <see cref="DateTime" />, calculate the <see cref="SpanOfTime" />.</summary>
		/// <param name="dateTime"></param>
		/// <returns></returns>
		public static SpanOfTime CalcSpanSince( DateTime dateTime ) {
			var sinceThen = new SpanOfTime( dateTime - DateTime.MinValue );
			var plancksSinceThen = sinceThen.CalcTotalPlanckTimes();
			var span = new SpanOfTime( PlancksUpToMinDateTime.Value + plancksSinceThen.Value );

			return span;
		}

		public static UniversalDateTime operator -( UniversalDateTime left, UniversalDateTime right ) => Combine( left, -right );

		public static UniversalDateTime operator -( UniversalDateTime universalDateTime ) => new UniversalDateTime( universalDateTime.Value * -1 );

		public static Boolean operator <( UniversalDateTime left, UniversalDateTime right ) => left.Value < right.Value;

		public static Boolean operator >( UniversalDateTime left, UniversalDateTime right ) => left.Value > right.Value;

		public Int32 CompareTo( UniversalDateTime other ) => this.Value.CompareTo( other.Value );

		public override String ToString() => this.Value.ToString();

	}

}