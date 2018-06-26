// Copyright © 1995-2018 to Rick@AIBrain.org and Protiguous. All Rights Reserved.
// 
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
// 
// This source code contained in "Premise.cs" belongs to Rick@AIBrain.org and
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
// Disclaimer:  Usage of the source code or binaries is AS-IS.
//    No warranties are expressed, implied, or given.
//    We are NOT responsible for Anything You Do With Our Code.
//    We are NOT responsible for Anything You Do With Our Executables.
//    We are NOT responsible for Anything You Do With Your Computer.
// =========================================================
// 
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
// For business inquiries, please contact me at Protiguous@Protiguous.com .
// 
// Our software can be found at "https://Protiguous.Software/"
// Our GitHub address is "https://github.com/Protiguous".
// Feel free to browse any source code we might have available.
// 
// ***  Project "Librainian"  ***
// File "Premise.cs" was last formatted by Protiguous on 2018/06/04 at 4:01 PM.

namespace Librainian.Knowledge.Logic {

	using System;

	/// <summary>
	///     In logic, an argument is a set of one or more declarative sentences (or "propositions")
	///     known as the premises along with another declarative sentence (or "proposition") known as
	///     the conclusion. Aristotle held that any logical argument could be reduced to two premises
	///     and a conclusion.
	/// </summary>
	/// <remarks>http: //wikipedia.org/wiki/Premise</remarks>
	/// <example>
	///     Premises are sometimes left unstated in which case they are called missing premises, for
	///     example: Socrates is mortal, since all men are mortal. It is evident that a tacitly
	///     understood claim is that Socrates is a man. The fully expressed reasoning is thus:
	///     Since all men are mortal and Socrates is a man, it follows that Socrates is mortal.
	///     In this example, the first two independent clauses preceding the comma (namely,
	///     "all men are mortal" and "Socrates is a man") are the premises, while "Socrates is
	///     mortal" is the conclusion. The proof of a conclusion depends on both the truth of
	///     the premises and the validity of the argument
	/// </example>
	public class Premise {

		public Statement FirstStatement { get; private set; }

		public Statement SecondStatement { get; private set; }

		public Double ProbabilityOfBeingTrue;

	}

}