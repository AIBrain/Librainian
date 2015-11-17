// Copyright 2015 Rick@AIBrain.org.
// 
// This notice must be kept visible in the source.
// 
// This section of source code belongs to Rick@AIBrain.Org unless otherwise specified, or the original license has been overwritten by the automatic formatting of this code.
// Any unmodified sections of source code borrowed from other projects retain their original license and thanks goes to the Authors.
// 
// Donations and Royalties can be paid via
// PayPal: paypal@aibrain.org
// bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
// litecoin: LeUxdU2w3o6pLZGVys5xpDZvvo8DUrjBp9
// 
// Usage of the source code or compiled binaries is AS-IS.I am not responsible for Anything You Do.
// 
// Contact me by email if you have any questions or helpful criticism.
//  
// "Librainian/BigInt.cs" was last cleaned by Rick on 2015/08/05 at 1:43 PM

namespace Librainian.Maths {

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Extensions;

    /// <summary>
    /// http://codereview.stackexchange.com/a/99085/26303
    /// </summary>
    [Immutable]
    public class BigInt {

        public BigInt( String number ) { this.Integer = CalculateBigInteger( number ); }

        public BigInt( List< Int32 > list ) { this.Integer = list; }

        public List< Int32 > Integer { get; }

        private static List< Int32 > CalculateBigInteger( String number ) { return number.Reverse().Select( chararcter => Int32.Parse( chararcter.ToString() ) ).ToList(); }

        private static Int32 NumberAdd( Int32 value1, Int32 value2, ref Int32 carryOver ) {
            var addResult = value1 + value2 + carryOver;
            carryOver = addResult / 10;
            var addValue = addResult % 10;
            return addValue;
        }

        public static BigInt Add( BigInt int1, BigInt int2 ) {
            var result = new List< Int32 >();

            var carryOver = 0;

            IEnumerator< Int32 > enumerator1 = int1.Integer.GetEnumerator();
            IEnumerator< Int32 > enumerator2 = int2.Integer.GetEnumerator();

            enumerator1.MoveNext();
            enumerator2.MoveNext();

            var hasNext1 = true;
            var hasNext2 = true;

            while ( hasNext1 || hasNext2 ) {
                var value = NumberAdd( enumerator1.Current, enumerator2.Current, ref carryOver );
                result.Add( value );

                hasNext1 = enumerator1.MoveNext();
                hasNext2 = enumerator2.MoveNext();
            }

            if ( carryOver != 0 ) {
                result.Add( carryOver );
            }

            return new BigInt( result );
        }

        public override String ToString() {
            var sb = new StringBuilder();
            foreach ( var number in this.Integer ) {
                sb.Append( number.ToString() );
            }

            var reverseString = sb.ToString().ToCharArray();
            Array.Reverse( reverseString );
            return new String( reverseString );
        }
    }

}
