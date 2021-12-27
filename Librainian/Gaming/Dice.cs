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
// Our software can be found at "https://Protiguous.Software/"
// Our GitHub address is "https://github.com/Protiguous".
// 
// File "Dice.cs" last touched on 2021-12-11 at 1:18 PM by Protiguous.

namespace Librainian.Gaming;

using System;
using Maths;

public record Dice : AbstractGameObject {

	/// <summary>A 6-sided dice.</summary>
	public static readonly Dice Cube = new(Guid.Empty, 6);

	public Dice( Guid identifier, UInt16 numberOfSides = 6 ) : base( identifier ) {
		this.NumberOfSides = numberOfSides;
		this.Roll();
	}

	public UInt16 CurrentSide { get; private set; }

	public UInt16 NumberOfSides { get; }

	/// <summary>
	///     <para>Rolls the dice to determine which side lands face-up.</para>
	/// </summary>
	/// <returns>The side which landed face-up</returns>
	public UInt16 Roll() {
		this.CurrentSide = ( UInt16 ) ( this.NumberOfSides.Next() + 1 );
		return this.CurrentSide;
	}

}