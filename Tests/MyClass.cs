// Copyright 2016 Rick@AIBrain.org.
// 
// This notice must be kept visible in the source.
// 
// This section of source code belongs to Rick@AIBrain.Org unless otherwise specified, or the
// original license has been overwritten by the automatic formatting of this code. Any unmodified
// sections of source code borrowed from other projects retain their original license and thanks
// goes to the Authors.
// 
// Donations and royalties can be paid via
//  PayPal: paypal@aibrain.org
//  bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//  litecoin: LeUxdU2w3o6pLZGVys5xpDZvvo8DUrjBp9
// 
// Usage of the source code or compiled binaries is AS-IS. I am not responsible for Anything You Do.
// 
// Contact me by email if you have any questions or helpful criticism.
// 
// "Librainian Tests/MyClass.cs" was last cleaned by Rick on 2016/02/02 at 10:10 PM

namespace LibrainianTests {
    using System;
    using System.IO;
    using System.Runtime.CompilerServices;
    using System.Runtime.Serialization;
    using System.Runtime.Serialization.Formatters.Binary;
    using NUnit.Framework;

    [ TestFixture ]
    [ Serializable ]
    public class MyClass : ISerializable {

        public MyClass() => this.Trace();

        protected MyClass( SerializationInfo info, StreamingContext context ) => this.Trace();

        /// <summary>
        ///     Populates a <see cref="T:System.Runtime.Serialization.SerializationInfo" /> with the data needed to serialize the
        ///     target object.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo" /> to populate with data. </param>
        /// <param name="context">
        ///     The destination (see <see cref="T:System.Runtime.Serialization.StreamingContext" />) for this
        ///     serialization.
        /// </param>
        /// <exception cref="T:System.Security.SecurityException">The caller does not have the required permission. </exception>
        public void GetObjectData( SerializationInfo info, StreamingContext context ) => this.Trace();

        private void Trace( [CallerMemberName] String caller = null ) => Console.WriteLine( $"Trace {caller}: {this.GetType().Name}" );

        [OnDeserializing]
        internal void OnDeserializingMethod( StreamingContext context ) => this.Trace();

        [OnDeserialized]
        internal void OnDeserializedMethod( StreamingContext context ) => this.Trace();

        [OnSerializing]
        internal void OnSerializingMethod( StreamingContext context ) => this.Trace();

        [OnSerialized]
        internal void OnSerializedMethod( StreamingContext context ) => this.Trace();

        [ Test ]
        public static void SerializeAndDeserializeTest() {
            using ( var ms = new MemoryStream() ) {

                var orig = new MyClass();

                var ser = new BinaryFormatter();
                Console.WriteLine( "before serializing" );
                ser.Serialize( ms, orig );
                Console.WriteLine( "after serializing" );

                Console.WriteLine(  );
                ms.Position = 0;
                Console.WriteLine( "before deserializing" );
                ser.Deserialize( ms );
                Console.WriteLine( "after deserializing" );
            }
        }

    }

}
