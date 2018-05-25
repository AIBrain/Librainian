// Copyright © 1995-2018 to Rick@AIBrain.org and Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
//
// This source code contained in "IAtomic.cs" belongs to Rick@AIBrain.org and
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
// Usage of the source code or binaries is AS-IS.
// No warranties are expressed, implied, or given.
// We are NOT responsible for Anything You Do With Our Code.
// =========================================================
//
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
// For business inquiries, please contact me at Protiguous@Protiguous.com
//
// "Librainian/Librainian/IAtomic.cs" was last formatted by Protiguous on 2018/05/24 at 7:21 PM.

namespace Librainian.Maths {

    using System;

    /// <summary>Provide atomic access to an instance of <typeparamref name="T" />.</summary>
    /// <typeparam name="T">The type of the instance to be updated atomically.</typeparam>
    /// <author>Kenneth Xu</author>
    internal interface IAtomic<T> {

        /// <summary>Gets and sets the current value.</summary>
        T Value { get; set; }

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