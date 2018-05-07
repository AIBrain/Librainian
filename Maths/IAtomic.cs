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
// "Librainian/IAtomic.cs" was last cleaned by Protiguous on 2016/06/18 at 10:53 PM

namespace Librainian.Maths {

    using System;

    /// <summary>Provide atomic access to an instance of <typeparamref name="T" />.</summary>
    /// <typeparam name="T">The type of the instance to be updated atomically.</typeparam>
    /// <author>Kenneth Xu</author>
    internal interface IAtomic<T> {

        /// <summary>Gets and sets the current value.</summary>
        T Value {
            get; set;
        }

        /// <summary>
        ///     Atomically sets the value to the <paramref name="newValue" /> if the current value
        ///     equals the <paramref name="expectedValue" />.
        /// </summary>
        /// <param name="expectedValue">The expected value</param>
        /// <param name="newValue">
        ///     The new value to use of the current value equals the expected value.
        /// </param>
        /// <returns>
        ///     <c>true</c> if the current value equaled the expected value, <c>false</c> otherwise.
        /// </returns>
        Boolean CompareAndSet( T expectedValue, T newValue );

        /// <summary>Atomically sets to the given value and returns the previous value.</summary>
        /// <param name="newValue">The new value for the instance.</param>
        /// <returns>the previous value of the instance.</returns>
        T Exchange( T newValue );

        /// <summary>Eventually sets to the given value.</summary>
        /// <param name="newValue">the new value</param>
        void LazySet( T newValue );

        /// <summary>Returns the String representation of the current value.</summary>
        /// <returns>The String representation of the current value.</returns>
        String ToString();

        /// <summary>
        ///     Atomically sets the value to the <paramref name="newValue" /> if the current value
        ///     equals the <paramref name="expectedValue" />. May fail spuriously.
        /// </summary>
        /// <param name="expectedValue">The expected value</param>
        /// <param name="newValue">
        ///     The new value to use of the current value equals the expected value.
        /// </param>
        /// <returns>
        ///     <c>true</c> if the current value equaled the expected value, <c>false</c> otherwise.
        /// </returns>
        Boolean WeakCompareAndSet( T expectedValue, T newValue );
    }
}