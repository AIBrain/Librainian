// Copyright © Protiguous. All Rights Reserved.
// 
// This entire copyright notice and license must be retained and must be kept visible in any binaries, libraries, repositories, or source code (directly or derived) from our binaries, libraries, projects, solutions, or applications.
// 
// All source code belongs to Protiguous@Protiguous.com unless otherwise specified or the original license has been overwritten by formatting. (We try to avoid it from happening, but it does accidentally happen.)
// 
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
// Our software can be found at "https://Protiguous.com/Software/"
// Our GitHub address is "https://github.com/Protiguous".
// 
// File "HumanCalculator.cs" last touched on 2021-10-13 at 4:27 PM by Protiguous.

namespace Librainian.Maths;

using System;
using System.Linq;
using System.Numerics;
using Numbers;

/// <summary>Challenge: Do math the way we (me) were taught in school.</summary>
public static class HumanCalculator {

	/// <summary>Add classroom-style (the challenge: avoid using BigInteger+BigInteger operation or reversing the strings).</summary>
	/// <param name="whom"></param>
	/// <param name="nombre"></param>
	/// <see cref="http://wikipedia.org/wiki/Addition#Notation_and_terminology" />
	public static BigInteger Add( this BigInteger whom, BigInteger nombre ) {
		var resultant = BigInteger.Zero;

		//TODO
		return resultant;
	}

	/// <summary>Add classroom-style (the challenge: avoid using BigInteger+BigInteger operation or reversing the strings).</summary>
	/// <param name="addends"></param>
	public static BigInteger Add( params BigInteger[] addends ) {
		var total = BigInteger.Zero;

		foreach ( var local in addends.Select( term => term.ToString() ) ) {
			var term = local;

			// total
			//+ term
			//______
			//result

			var s = total.ToString();
			var result = String.Empty;

			if ( s.Length < term.Length ) {
				s = s.PadLeft( term.Length, '0' );
			}
			else if ( term.Length < s.Length ) {
				term = term.PadLeft( s.Length, '0' );
			}

			while ( term.Any() ) {
				if ( s is null ) {
					//BUG This won't work.
					break;
				}

				if ( !Digit.TryParse( s.Last().ToString(), out var l ) ) {
					continue;
				}

				s = s[ ..^1 ]; //s = s.Substring(0, s.Length - 1);

				//TODO Totally not finished.

				/*
				var m = Byte.Parse( term.Last().ToString() );
				term = term[ ..^1 ];

				var t = ( l + m ).ToString();
				var c = Byte.Parse( t.Last().ToString() );

				if ( t.Length == 2 ) {
					result = "1" + c;
				}
				else {
					result += c;
				}
				*/
			}

			total += BigInteger.Parse( result );
		}

		return total;
	}

	public static BigInteger Divide( BigInteger[] terms ) => throw new NotImplementedException();

	public static BigInteger Multiply( BigInteger[] terms ) => throw new NotImplementedException();

	public static BigInteger Operate( MathOperation operation, params BigInteger[]? terms ) {
		return operation switch {
			MathOperation.Addition => Add( terms ),
			MathOperation.Subtraction => Subtract( terms ),
			MathOperation.Multiplication => Multiply( terms ),
			MathOperation.Division => Divide( terms ),
			var _ => throw new ArgumentOutOfRangeException( nameof( operation ), operation, $"Unknown operation {operation}" )
		};
	}

	public static BigInteger Subtract( BigInteger[] terms ) => throw new NotImplementedException();

}

public enum MathOperation {

	/// <summary>https://en.wikipedia.org/wiki/Addition</summary>
	/// <see cref="http://wikipedia.org/wiki/Addition" />
	Addition,

	Subtraction,

	Multiplication,

	Division

}