﻿// Copyright © Protiguous. All Rights Reserved.
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
// File "CompositeFormatter.cs" last formatted on 2021-02-03 at 4:06 PM.

#nullable enable

namespace Librainian.Parsing {

	using System;
	using System.Collections.Generic;
	using System.Globalization;
	using System.Linq;
	using JetBrains.Annotations;

	/// <summary>
	///     Binds multiple formatters together.
	/// </summary>
	/// <seealso cref="Formatter" />
	/// <remarks>From the Vanara.PInvoke project @ https://github.com/dahall/Vanara</remarks>
	internal sealed class CompositeFormatter : Formatter {

		[NotNull]
		[ItemNotNull]
		private List<Formatter> _formatters { get; }

		/// <summary>
		///     Initializes a new instance of the <see cref="CompositeFormatter" /> class.
		/// </summary>
		/// <param name="culture">   The culture.</param>
		/// <param name="formatters">The formatters.</param>
		public CompositeFormatter( [CanBeNull] CultureInfo? culture = null, [NotNull] params Formatter[] formatters ) : base( culture ) =>
			this._formatters = new List<Formatter>( formatters );

		/// <summary>
		///     Adds the specified formatter.
		/// </summary>
		/// <param name="formatter">The formatter.</param>
		public void Add( [NotNull] Formatter formatter ) {
			if ( formatter == null ) {
				throw new ArgumentNullException( nameof( formatter ) );
			}

			this._formatters.Add( formatter );
		}

		/// <summary>
		///     Converts the value of a specified object to an equivalent string representation using specified format and
		///     culture-specific formatting information.
		/// </summary>
		/// <param name="format">        A format string containing formatting specifications.</param>
		/// <param name="arg">           An object to format.</param>
		/// <param name="formatProvider">An object that supplies format information about the current instance.</param>
		/// <returns>
		///     The string representation of the value of <paramref name="arg" />, formatted as specified by
		///     <paramref name="format" /> and <paramref name="formatProvider" />.
		/// </returns>
		public override String Format( [CanBeNull] String? format, [CanBeNull] Object? arg, [CanBeNull] IFormatProvider? formatProvider ) =>
			this._formatters.Select( formatter => formatter.Format( format, arg, formatProvider ) ).First();
	}
}