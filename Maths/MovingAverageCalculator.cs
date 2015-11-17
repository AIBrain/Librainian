// Copyright 2015 Rick@AIBrain.org.
// 
// This notice must be kept visible in the source.
// 
// This section of source code belongs to Rick@AIBrain.Org unless otherwise specified, or the
// original license has been overwritten by the automatic formatting of this code. Any unmodified
// sections of source code borrowed from other projects retain their original license and thanks
// goes to the Authors.
// 
// Donations and Royalties can be paid via
// PayPal: paypal@aibrain.org
// bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
// litecoin: LeUxdU2w3o6pLZGVys5xpDZvvo8DUrjBp9
// 
// Usage of the source code or compiled binaries is AS-IS. I am not responsible for Anything You Do.
// 
// Contact me by email if you have any questions or helpful criticism.
// 
// "Librainian/MovingAverageCalculator.cs" was last cleaned by Rick on 2015/06/12 at 3:00 PM

namespace Librainian.Maths {

    using System;

    /// <summary>
    /// Calculates a moving average value over a specified window. The window size must be specified
    /// upon creation of this object.
    /// </summary>
    /// <remarks>
    /// Authored by Drew Noakes, February 2005. Use freely, though keep this message intact and
    /// report any bugs to me. I also appreciate seeing extensions, or simply hearing that you're
    /// using these classes. You may not copyright this work, though may use it in
    /// commercial/copyrighted works. Happy coding. Updated 29 March 2007. Added a Reset() method.
    /// </remarks>
    /// <seealso cref="http://drewnoakes.com/code/util/MovingAverageCalculator.html" />
    public sealed class MovingAverageCalculator {
        private readonly Single[] _values;
        private readonly Int32 _windowSize;
        private Int32 _nextValueIndex;
        private Single _sum;
        private Int32 _valuesIn;

        /// <summary>
        /// Gets a value indicating whether enough values have been provided to fill the speicified
        /// window size. Values returned from NextValue may still be used prior to IsMature
        /// returning true, however such values are not subject to the intended smoothing effect of
        /// the moving average's window size.
        /// </summary>
        public Boolean IsMature => this._valuesIn == this._windowSize;

        /// <summary>Create a new moving average calculator.</summary>
        /// <param name="windowSize">
        /// The maximum number of values to be considered by this moving average calculation.
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">If windowSize less than one.</exception>
        public MovingAverageCalculator(Int32 windowSize) {
            if ( windowSize < 1 ) {
                throw new ArgumentOutOfRangeException( nameof( windowSize ), windowSize, "Window size must be greater than zero." );
            }

            this._windowSize = windowSize;
            this._values = new Single[ this._windowSize ];

            this.Reset();
        }

        /// <summary>
        /// Updates the moving average with its next value, and returns the updated average value.
        /// When IsMature is true and NextValue is called, a previous value will 'fall out' of the
        /// moving average.
        /// </summary>
        /// <param name="nextValue">The next value to be considered within the moving average.</param>
        /// <returns>The updated moving average value.</returns>
        /// <exception cref="ArgumentOutOfRangeException">If nextValue is equal to float.NaN.</exception>
        public Single NextValue(Single nextValue) {
            if ( Single.IsNaN( nextValue ) ) {
                throw new ArgumentOutOfRangeException( nameof( nextValue ), "NaN may not be provided as the next value.  It would corrupt the state of the calculation." );
            }

            // add new value to the sum
            this._sum += nextValue;

            if ( this._valuesIn < this._windowSize ) {

                // we haven't yet filled our window
                this._valuesIn++;
            }
            else {

                // remove oldest value from sum
                this._sum -= this._values[ this._nextValueIndex ];
            }

            // store the value
            this._values[ this._nextValueIndex ] = nextValue;

            // progress the next value index pointer
            this._nextValueIndex++;
            if ( this._nextValueIndex == this._windowSize ) {
                this._nextValueIndex = 0;
            }

            return this._sum / this._valuesIn;
        }

        /// <summary>
        /// Clears any accumulated state and resets the calculator to its initial configuration.
        /// Calling this method is the equivalent of creating a new instance.
        /// </summary>
        public void Reset() {
            this._nextValueIndex = 0;
            this._sum = 0;
            this._valuesIn = 0;
        }
    }
}