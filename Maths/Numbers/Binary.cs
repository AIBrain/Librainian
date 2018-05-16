// Copyright © 1995-2018 to Rick@AIBrain.org and Protiguous.
// All Rights Reserved.
//
// This ENTIRE copyright notice and file header MUST BE KEPT
// VISIBLE in any source code derived from or used from our
// libraries and projects.
//
// =========================================================
// This section of source code, "Binary.cs",
// belongs to Rick@AIBrain.org and Protiguous@Protiguous.com
// unless otherwise specified OR the original license has been
// overwritten by the automatic formatting.
//
// (We try to avoid that from happening, but it does happen.)
//
// Any unmodified portions of source code gleaned from other
// projects still retain their original license and our thanks
// goes to those Authors.
// =========================================================
//
// Donations (more please!), royalties from any software that
// uses any of our code, and license fees can be paid to us via
// bitcoin at the address 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2.
//
// =========================================================
// Usage of the source code or compiled binaries is AS-IS.
// No warranties are expressed or implied.
// I am NOT responsible for Anything You Do With Our Code.
// =========================================================
//
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
//
// "Librainian/Librainian/Binary.cs" was last cleaned by Protiguous on 2018/05/15 at 10:45 PM.

namespace Librainian.Maths.Numbers {

    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Text;
    using JetBrains.Annotations;

    /// <summary>
    ///     Based from Hamming code found at http://maciejlis.com/hamming-code-algorithm-c-sharp/
    /// </summary>
    [DebuggerDisplay( "{" + nameof( ToString ) + "()}" )]
    public class Binary : IEnumerable<Boolean> {

        public Binary( IReadOnlyCollection<Boolean> booleans ) {
            this.Booleans = booleans.ToList();
            this.Booleans.Capacity = this.Booleans.Count;
        }

        public Binary( IEnumerable<Boolean> binary ) : this( ( IReadOnlyCollection<Boolean> )binary ) { }

        public Binary( Int32 value ) : this( ConvertToBinary( value ) ) { }

        public Binary( Int32 value, Int32 minSize ) : this( ConvertToBinary( value, minSize ) ) { }

        [NotNull]
        public List<Boolean> Booleans { get; }

        public Int32 Length => this.Booleans.Count;

        public Boolean this[Int32 index] {
            get => this.Booleans[index];

            set => this.Booleans[index] = value;
        }

        public static Binary Concatenate( Binary a, Binary b ) {
            var result = new Binary( new Boolean[a.Length + b.Length] );
            var n = 0;

            foreach ( var bit in a ) {
                result[n] = bit;
                n++;
            }

            foreach ( var bit in b ) {
                result[n] = bit;
                n++;
            }

            return result;
        }

        public static IEnumerable<Boolean> ConvertToBinary( Int32 value ) {
            var binaryString = Convert.ToString( value, 2 );

            return binaryString.Select( c => c == '1' );
        }

        public static IEnumerable<Boolean> ConvertToBinary( Int32 value, Int32 minSize ) {
            var toBinary = new List<Boolean>( ConvertToBinary( value ) );

            while ( toBinary.Count != minSize ) { toBinary.Insert( 0, false ); }

            return toBinary;
        }

        public static Binary operator &( Binary a, Binary b ) {
            if ( a.Length != b.Length ) { throw new ArgumentException( "Binaries must have same length" ); }

            var result = new Boolean[a.Length];

            for ( var i = 0; i < a.Length; i++ ) { result[i] = a[i] & b[i]; }

            return new Binary( result );
        }

        public static Binary operator ^( Binary a, Binary b ) {
            if ( a.Length != b.Length ) { throw new ArgumentException( "Binaries must have same length" ); }

            var result = new Boolean[a.Length];

            for ( var i = 0; i < a.Length; i++ ) { result[i] = a[i] ^ b[i]; }

            return new Binary( result );
        }

        public Int32 CountOnes() => this.Booleans.Count( bit => bit );

        public Int32 CountZeroes() => this.Booleans.Count( bit => !bit );

        public IEnumerator<Boolean> GetEnumerator() => this.Booleans.GetEnumerator();

        public override String ToString() {
            var stringBuilder = new StringBuilder( this.Length );

            foreach ( var bit in this.Booleans ) { stringBuilder.Append( bit ? '1' : '0' ); }

            return stringBuilder.ToString();
        }

        IEnumerator IEnumerable.GetEnumerator() => this.Booleans.GetEnumerator();
    }
}