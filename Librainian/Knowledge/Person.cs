// Copyright © Rick@AIBrain.org and Protiguous. All Rights Reserved.
// 
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
// 
// This source code contained in "Person.cs" belongs to Protiguous@Protiguous.com and
// Rick@AIBrain.org unless otherwise specified or the original license has
// been overwritten by formatting.
// (We try to avoid it from happening, but it does accidentally happen.)
// 
// Any unmodified portions of source code gleaned from other projects still retain their original
// license and our thanks goes to those Authors. If you find your code in this source code, please
// let us know so we can properly attribute you and include the proper license and/or copyright.
// 
// If you want to use any of our code, you must contact Protiguous@Protiguous.com or
// Sales@AIBrain.org for permission and a quote.
// 
// Donations are accepted (for now) via
//     bitcoin:1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//     paypal@AIBrain.Org
//     (We're still looking into other solutions! Any ideas?)
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
// For business inquiries, please contact me at Protiguous@Protiguous.com
// 
// Our website can be found at "https://Protiguous.com/"
// Our software can be found at "https://Protiguous.Software/"
// Our GitHub address is "https://github.com/Protiguous".
// Feel free to browse any source code we *might* make available.
// 
// Project: "Librainian", "Person.cs" was last formatted by Protiguous on 2018/11/24 at 12:57 AM.

namespace Librainian.Knowledge {

	using System;
	using System.Collections.Generic;
	using Collections;
	using JetBrains.Annotations;
	using Measurement.Time;
	using Newtonsoft.Json;
	using ReactiveUI.Fody.Helpers;

	/// <summary>
	/// Just a common class used in examples to run test code on.
	/// </summary>
	public interface IPerson {

		[JsonProperty]
		Date BirthDate { get; set; }

		[JsonProperty]
		Time BirthTime { get; set; }

		[JsonProperty]
		IList<IPerson> Children { get; }

		[JsonProperty]
		String FirstName { get; set; }

		[JsonProperty]
		String LastName { get; set; }

		[JsonProperty]
		String MiddleName { get; set; }

		[JsonProperty]
		IList<IPerson> Parents { get; }

		[JsonIgnore]
		String FullName { get; }

	}

	/// <summary>
	/// Just a common class used in examples to run test code on.
	/// </summary>
	[JsonObject]
	public class Person : IPerson {

		[Reactive]
		[JsonProperty]
		public UInt64 ID { get; set; }

		[Reactive]
		[JsonProperty]
		public Date BirthDate { get; set; }

		[Reactive]
		[JsonProperty]
		public Time BirthTime { get; set; }

		[JsonProperty]
		public IList<IPerson> Children { get; } = new List<IPerson>();

		[Reactive]
		[JsonProperty]
		public String FirstName { get; set; }

		[Reactive]
		[JsonProperty]
		public String LastName { get; set; }

		[Reactive]
		[JsonProperty]
		public String MiddleName { get; set; }

		[JsonProperty]
		public IList<IPerson> Parents { get; } = new List<IPerson>();

		[JsonIgnore]
		[NotNull]
		public String FullName {
			get {
				var arr = new [] {
					this.FirstName, this.MiddleName, this.LastName
				};

				var fullName = $"{arr.ToStrings(" " )}";

				return fullName;
			}
		}

	}

}