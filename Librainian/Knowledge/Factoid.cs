// Copyright � Rick@AIBrain.org and Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
//
// This source code contained in "Factoid.cs" belongs to Protiguous@Protiguous.com and
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
// Project: "Librainian", "Factoid.cs" was last formatted by Protiguous on 2018/07/10 at 9:12 PM.

namespace Librainian.Knowledge {

	using System;
	using JetBrains.Annotations;
	using Maths.Numbers;
	using Newtonsoft.Json;

	[JsonObject]
	public class Factoid {

		[JsonProperty]
		public String Description { get; private set; }

		//TODO
		[JsonProperty]
		public Percentage Truthiness { get; private set; }

		public Factoid( [NotNull] String description, Percentage truthiness ) {
			this.Description = description ?? throw new ArgumentNullException( nameof( description ) );
			this.Truthiness = truthiness;
		}
	}

	/*
        [JsonObject]
        public class Teacher  {
            [JsonProperty] public ThreadSafeList< Topic > Topics = new ThreadSafeList< Topic >();

            public void AddTopic( String topic ) {

                //TODO uh....... where was I going with this ?
                this.Topics.Add( new Topic() );
            }

            public override void Deserialize() {
                try {
                    this.IsDeserializing = true;
                    if ( String.IsNullOrWhiteSpace( this.PersistPath ) ) {
                        throw new ArgumentException( "PersistFileName needs to be set." );
                    }
                    this.PersistPath.Loader< Teacher >( onLoad: database => database.DeepClone( destination: this ) );
                }
                finally {
                    this.IsDeserializing = false;
                }
            }

            public override void Serialize() {
                try {
                    this.IsSerializing = true;
                    if ( String.IsNullOrWhiteSpace( this.PersistPath ) ) {
                        throw new ArgumentException( "PersistFileName needs to be set." );
                    }
                    this.Saver( this.PersistPath );
                }
                finally {
                    this.IsSerializing = false;
                }
            }
        }
    */
}