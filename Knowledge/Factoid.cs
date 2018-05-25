// Copyright © 1995-2018 to Rick@AIBrain.org and Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
//
// This source code contained in "Factoid.cs" belongs to Rick@AIBrain.org and
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
// Usage of the source code or binaries is AS-IS.
// No warranties are expressed, implied, or given.
// We are NOT responsible for Anything You Do With Our Code.
// =========================================================
//
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
// For business inquiries, please contact me at Protiguous@Protiguous.com
//
// "Librainian/Librainian/Factoid.cs" was last formatted by Protiguous on 2018/05/24 at 7:17 PM.

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