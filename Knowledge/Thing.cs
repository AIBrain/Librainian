// Copyright © 1995-2018 to Rick@AIBrain.org and Protiguous. All Rights Reserved.
// 
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
// 
// This source code contained in "Thing.cs" belongs to Rick@AIBrain.org and
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
// File "Thing.cs" was last formatted by Protiguous on 2018/06/04 at 4:01 PM.

namespace Librainian.Knowledge {

	using System;
	using System.Collections.Concurrent;
	using JetBrains.Annotations;
	using Maths.Numbers;

	/// <summary>new Thing("Morris", "casually talking with the user Rick")</summary>
	/// <example>an object exists, and it is called Morris.</example>
	public class Thing {

		public Domain Domain { get; }

		public ConcurrentDictionary<TypeOrClass, Percentage> HasTheseSubClasses { get; private set; }

		public String Label { get; }

		/// <summary>
		///     This <see cref="Thing" /> is a subClass of what <see cref="TypeOrClass" /> with a
		///     percentage of Trueness (determined so far, updated when we have new info)
		/// </summary>
		public ConcurrentDictionary<TypeOrClass, Percentage> SubClassesOf { get; }

		public Thing( [NotNull] String label, [NotNull] Domain domain ) {
			this.Label = label ?? throw new ArgumentNullException( nameof( label ) );
			this.Domain = domain ?? throw new ArgumentNullException( nameof( domain ) );
			this.SubClassesOf = new ConcurrentDictionary<TypeOrClass, Percentage>();
		}

	}

}