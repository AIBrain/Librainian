// Copyright 2016 Protiguous.
//
// This notice must be kept visible in the source.
//
// This section of source code belongs to Protiguous@Protiguous.com unless otherwise specified, or the
// original license has been overwritten by the automatic formatting of this code. Any unmodified
// sections of source code borrowed from other projects retain their original license and thanks
// goes to the Authors.
//
// Donations and royalties can be paid via
//  PayPal: paypal@Protiguous.com
//  bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//  
//
// Usage of the source code or compiled binaries is AS-IS. I am not responsible for Anything You Do.
//
// Contact me by email if you have any questions or helpful criticism.
//
// "Librainian/UDC.cs" was last cleaned by Protiguous on 2016/08/26 at 10:30 AM

namespace Librainian.Linguistics {

    using System;

    /// <summary>Universal Decimal Classification</summary>
    /// <seealso cref="http://wikipedia.org/wiki/Universal_Decimal_Classification"></seealso>
    /// <example>
    ///     539. 120 Theoretical problems of elementary particles physics. Theories and models of
    ///     fundamental interactions.
    ///     539. 120.2 Symmetries of quantum physics
    ///     539. 120.224 Reflection in time and space
    /// </example>
    public class UDC {

        public UDC( String notation ) => this.Notation = notation;

	    public static UDC Unknown { get; } = new UDC( String.Empty );

        public String Notation {
            get;
        }

        /// <summary>Serves as the default hash function. </summary>
        /// <returns>A hash code for the current object.</returns>
        public override Int32 GetHashCode() => this.Notation.GetHashCode();

    }
}
