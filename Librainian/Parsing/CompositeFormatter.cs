namespace Librainian.Parsing {

	using System;
	using System.Collections.Generic;
	using System.Globalization;
	using JetBrains.Annotations;

	/// <summary>Binds multiple formatters together.</summary>
	/// <seealso cref="Formatter"/>
	/// <remarks>From the Vanara.PInvoke project @ https://github.com/dahall/Vanara </remarks>
	internal sealed class CompositeFormatter : Formatter {
		private readonly List<Formatter> _formatters;

		/// <summary>Initializes a new instance of the <see cref="CompositeFormatter"/> class.</summary>
		/// <param name="culture">The culture.</param>
		/// <param name="formatters">The formatters.</param>
		public CompositeFormatter( [CanBeNull] CultureInfo culture = null, [NotNull] params Formatter[] formatters ) : base( culture ) => this._formatters = new List<Formatter>( formatters );

		/// <summary>Adds the specified formatter.</summary>
		/// <param name="formatter">The formatter.</param>
		public void Add( Formatter formatter ) => this._formatters.Add( formatter );

		/// <summary>
		/// Converts the value of a specified object to an equivalent string representation using specified format and culture-specific
		/// formatting information.
		/// </summary>
		/// <param name="format">A format string containing formatting specifications.</param>
		/// <param name="arg">An object to format.</param>
		/// <param name="formatProvider">An object that supplies format information about the current instance.</param>
		/// <returns>
		/// The string representation of the value of <paramref name="arg"/>, formatted as specified by <paramref name="format"/> and
		/// <paramref name="formatProvider"/>.
		/// </returns>
		[CanBeNull]
		public override String Format( String format, Object arg, IFormatProvider formatProvider ) {
			foreach ( var formatter in this._formatters ) {
				var result = formatter.Format( format, arg, formatProvider );
				if ( result != null ) {
					return result;
				}
			}
			return null;
		}
	}
}