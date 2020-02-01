// Copyright © Protiguous. All Rights Reserved.
// 
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
// 
// This source code contained in "CompositeFormatter.cs" belongs to Protiguous@Protiguous.com
// unless otherwise specified or the original license has been overwritten by formatting.
// (We try to avoid it from happening, but it does accidentally happen.)
// 
// Any unmodified portions of source code gleaned from other projects still retain their original
// license and our thanks goes to those Authors. If you find your code in this source code, please
// let us know so we can properly attribute you and include the proper license and/or copyright.
// 
// If you want to use any of our code in a commercial project, you must contact
// Protiguous@Protiguous.com for permission and a quote.
// 
// Donations are accepted (for now) via
//     bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//     PayPal: Protiguous@Protiguous.com
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
// For business inquiries, please contact me at Protiguous@Protiguous.com.
// 
// Our website can be found at "https://Protiguous.com/"
// Our software can be found at "https://Protiguous.Software/"
// Our GitHub address is "https://github.com/Protiguous".
// Feel free to browse any source code we make available.
// 
// Project: "Librainian", "CompositeFormatter.cs" was last formatted by Protiguous on 2020/01/31 at 12:28 AM.

namespace Librainian.Parsing {

    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using JetBrains.Annotations;

    /// <summary>Binds multiple formatters together.</summary>
    /// <seealso cref="Formatter" />
    /// <remarks>From the Vanara.PInvoke project @ https://github.com/dahall/Vanara </remarks>
    internal sealed class CompositeFormatter : Formatter {

        [NotNull]
        private List<Formatter> _formatters { get; }

        /// <summary>Initializes a new instance of the <see cref="CompositeFormatter" /> class.</summary>
        /// <param name="culture">The culture.</param>
        /// <param name="formatters">The formatters.</param>
        public CompositeFormatter( [CanBeNull] CultureInfo culture = null, [NotNull] params Formatter[] formatters ) : base( culture ) =>
            this._formatters = new List<Formatter>( formatters );

        /// <summary>Adds the specified formatter.</summary>
        /// <param name="formatter">The formatter.</param>
        public void Add( [CanBeNull] Formatter formatter ) => this._formatters.Add( formatter );

        /// <summary>Converts the value of a specified object to an equivalent string representation using specified format and culture-specific formatting information.</summary>
        /// <param name="format">A format string containing formatting specifications.</param>
        /// <param name="arg">An object to format.</param>
        /// <param name="formatProvider">An object that supplies format information about the current instance.</param>
        /// <returns>The string representation of the value of <paramref name="arg" />, formatted as specified by <paramref name="format" /> and <paramref name="formatProvider" />.</returns>
        [CanBeNull]
        public override String Format( [CanBeNull] String format, [CanBeNull] Object arg, [CanBeNull] IFormatProvider formatProvider ) {
            return this._formatters.Select( formatter => formatter.Format( format, arg, formatProvider ) ).FirstOrDefault( result => result != null );
        }

    }

}