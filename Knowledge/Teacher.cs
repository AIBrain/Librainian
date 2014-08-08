#region License & Information
// This notice must be kept visible in the source.
// 
// This section of source code belongs to Rick@AIBrain.Org unless otherwise specified,
// or the original license has been overwritten by the automatic formatting of this code.
// Any unmodified sections of source code borrowed from other projects retain their original license and thanks goes to the Authors.
// 
// Donations and Royalties can be paid via
// PayPal: paypal@aibrain.org
// bitcoin:1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
// bitcoin:1NzEsF7eegeEWDr5Vr9sSSgtUC4aL6axJu
// litecoin:LeUxdU2w3o6pLZGVys5xpDZvvo8DUrjBp9
// 
// Usage of the source code or compiled binaries is AS-IS.
// I am not responsible for Anything You Do.
// 
// "Librainian2/Teacher.cs" was last cleaned by Rick on 2014/08/08 at 2:27 PM
#endregion

namespace Librainian.Knowledge {
    using System;
    using System.Runtime.Serialization;
    using Annotations;
    using Collections;
    using Extensions;
    using Maths;
    using Persistence;

    [DataContract( IsReference = true )]
    public class Factoid {
        //TODO

        public Factoid( [NotNull] String description, Percentage truthiness ) {
            if ( description == null ) {
                throw new ArgumentNullException( "description" );
            }
            this.Description = description;
            this.Truthiness = truthiness;
        }

        [DataMember]
        public String Description { get; private set; }

        [DataMember]
        public Percentage Truthiness { get; private set; }
    }

    [DataContract( IsReference = true )]
    public class Teacher : Persistable {
        [DataMember] public ThreadSafeList< Topic > Topics = new ThreadSafeList< Topic >();

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
}
