namespace Librainian.Maths {
    using System;
    using System.Numerics;
    using Extensions;
    using Properties;

    /// <summary>
    /// Valid numbers are 0, 1, 2, 3, 4, 5, 6, 7, 8, 9
    /// </summary>
    /// <remarks>All functions should be atomic.</remarks>
    [Immutable]
    public struct Digit : IComparable<Digit> {

        public const Byte Minimum = 0;
        public const Byte Maximum = 9;

        // ReSharper disable InconsistentNaming
        public static readonly Digit zero = new Digit( 0 );
        public static readonly Digit one = new Digit( 1 );
        public static readonly Digit two = new Digit( 2 );
        public static readonly Digit three = new Digit( 3 );
        public static readonly Digit four = new Digit( 4 );
        public static readonly Digit five = new Digit( 5 );
        public static readonly Digit six = new Digit( 6 );
        public static readonly Digit seven = new Digit( 7 );
        public static readonly Digit eight = new Digit( 8 );
        public static readonly Digit nine = new Digit( 9 );
        // ReSharper restore InconsistentNaming

        public Byte Value { get; }

        public static implicit operator Byte( Digit digit ) => digit.Value;

        public Digit( BigInteger value ) {
            if ( value < Minimum || value > Maximum ) {
                throw new ArgumentOutOfRangeException( nameof( value ), Resources.Out_of_range );
            }
            this.Value = ( Byte )value;
        }

        /// <summary>
        /// Compares the current object with another object of the same type.
        /// </summary>
        /// <returns>
        /// A value that indicates the relative order of the objects being compared. The return value has the following meanings: Value Meaning Less than zero This object is less than the <paramref name="other"/> parameter.Zero This object is equal to <paramref name="other"/>. Greater than zero This object is greater than <paramref name="other"/>. 
        /// </returns>
        /// <param name="other">An object to compare with this object.</param>
        public int CompareTo( Digit other ) => this.Value.CompareTo( other.Value );

        public override string ToString() {
            switch ( this.Value ) {
                case 0: return nameof( zero );
                case 1: return nameof( one );
                case 2: return nameof( two );
                case 3: return nameof( three );
                case 4: return nameof( four );
                case 5: return nameof( five );
                case 6: return nameof( six );
                case 7: return nameof( seven );
                case 8: return nameof( eight );
                case 9: return nameof( nine );
                default: return String.Empty;
            }
        }

        public string ToNumber() => this.Value.ToString();

        public static bool operator <( Digit left, Digit right ) => left.Value < right.Value;

        public static bool operator >( Digit left, Digit right ) => left.Value > right.Value;

        public static bool operator <=( Digit left, Digit right ) => left.Value <= right.Value;

        public static bool operator >=( Digit left, Digit right ) => left.Value >= right.Value;
    }
}