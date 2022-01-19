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
// File "IniSection.cs" last formatted on 2022-12-22 at 5:20 PM by Protiguous.

#nullable enable

namespace Librainian.Persistence.InIFiles;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Maths;
using Newtonsoft.Json;
using Parsing;

[JsonObject]
public class IniSection : IEnumerable<IniLine> {

	[JsonProperty]
	private List<IniLine> lines { get; } = new();

	/// <summary>Gets the number of elements in the collection.</summary>
	/// <returns>The number of elements in the collection. </returns>
	public Int32 Count => this.lines.Count;

	/// <summary>Gets the element at the specified index in the read-only list.</summary>
	/// <param name="index">The zero-based index of the element to get. </param>
	/// <returns>The element at the specified index in the read-only list.</returns>
	public IniLine this[ Int32 index ] {
		get {
			if ( index <= 0 || index > this.Count ) {
				throw new ArgumentOutOfRangeException( nameof( index ) );
			}

			return this.lines[ index ];
		}
	}

	public IEnumerator<IniLine> GetEnumerator() => this.lines.GetEnumerator();

	IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

	public Boolean Add( String key, String? value ) {
		if ( String.IsNullOrEmpty( key ) ) {
			throw new ArgumentException( "Value cannot be null or empty.", nameof( key ) );
		}

		this.lines.Add( new IniLine( key, value ) );

		return true;
	}

	public Boolean Exists( String key ) => !String.IsNullOrEmpty( key ) && this.lines.Any( pair => pair.Key.Like( key ) );

	public Boolean Remove( String key ) => this.lines.RemoveAll( pair => pair.Key.Like( key ) ).Any();

}