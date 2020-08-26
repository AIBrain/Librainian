﻿// Copyright © Protiguous. All Rights Reserved.
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
// File "Formatter.cs" last formatted on 2020-08-14 at 8:41 PM.

namespace Librainian.Parsing {

	using System;
	using System.Globalization;
	using JetBrains.Annotations;

	/// <summary>Base class for expandable formatters.</summary>
	/// <remarks>From the Vanara.PInvoke project @ https://github.com/dahall/Vanara </remarks>
	public abstract class Formatter : IFormatProvider, ICustomFormatter {

		/// <summary>Initializes a new instance of the <see cref="Formatter" /> class.</summary>
		/// <param name="culture">The culture.</param>
		protected Formatter( [CanBeNull] CultureInfo culture = null ) => this.Culture = culture ?? CultureInfo.CurrentCulture;

		/// <summary>Gets the culture.</summary>
		/// <value>The culture.</value>
		[NotNull]
		public CultureInfo Culture { get; }

		/// <summary>
		///     Converts the value of a specified object to an equivalent string representation using specified format and
		///     culture-specific formatting information.
		/// </summary>
		/// <param name="format">A format string containing formatting specifications.</param>
		/// <param name="arg">An object to format.</param>
		/// <param name="formatProvider">An object that supplies format information about the current instance.</param>
		/// <returns>
		///     The string representation of the value of <paramref name="arg" />, formatted as specified by
		///     <paramref name="format" /> and <paramref name="formatProvider" />.
		/// </returns>
		public abstract String Format( [CanBeNull] String? format, [CanBeNull] Object? arg, [CanBeNull] IFormatProvider? formatProvider );

		/// <summary>Returns an object that provides formatting services for the specified type.</summary>
		/// <param name="formatType">An object that specifies the type of format object to return.</param>
		/// <returns>
		///     An instance of the object specified by <paramref name="formatType" />, if the IFormatProvider implementation can
		///     supply that type of object; otherwise,
		///     <see langword="null" />.
		/// </returns>
		public virtual Object GetFormat( [CanBeNull] Type? formatType ) => formatType == typeof( ICustomFormatter ) ? this : null;

		/// <summary>Helper method that can be used inside the Format method to handle unrecognized formats.</summary>
		/// <param name="format">A format string containing formatting specifications.</param>
		/// <param name="arg">An object to format.</param>
		/// <returns>
		///     The string representation of the value of <paramref name="arg" />, formatted as specified by
		///     <paramref name="format" />.
		/// </returns>
		[NotNull]
		protected String HandleOtherFormats( [CanBeNull] String? format, [CanBeNull] Object? arg ) =>
			( arg as IFormattable )?.ToString( format, this.Culture ) ?? arg?.ToString() ?? String.Empty;

		/// <summary>Gets a default instance of a composite formatter.</summary>
		/// <param name="culture">The culture.</param>
		/// <returns>A composite formatter.</returns>
		[NotNull]
		public static Formatter Default( [CanBeNull] CultureInfo culture = null ) => new CompositeFormatter( culture );

	}

}