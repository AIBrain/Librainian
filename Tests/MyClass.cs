// Copyright © Rick@AIBrain.org and Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
//
// This source code contained in "MyClass.cs" belongs to Protiguous@Protiguous.com and
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
//     PayPal:Protiguous@Protiguous.com
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
// Project: "LibrainianTests", "MyClass.cs" was last formatted by Protiguous on 2019/03/17 at 11:06 AM.

namespace LibrainianTests {

    using System;
    using System.IO;
    using System.Runtime.CompilerServices;
    using System.Runtime.Serialization;
    using System.Runtime.Serialization.Formatters.Binary;
    using JetBrains.Annotations;
    using Xunit;

    [Serializable]
    public class MyClass : ISerializable {

        protected MyClass( SerializationInfo info, StreamingContext context ) => this.Trace();

        public MyClass() => this.Trace();

        private void Trace( [CanBeNull] [CallerMemberName] String caller = null ) => Console.WriteLine( $"Trace {caller}: {this.GetType().Name}" );

        [OnDeserialized]
        internal void OnDeserializedMethod( StreamingContext context ) => this.Trace();

        [OnDeserializing]
        internal void OnDeserializingMethod( StreamingContext context ) => this.Trace();

        [OnSerialized]
        internal void OnSerializedMethod( StreamingContext context ) => this.Trace();

        [OnSerializing]
        internal void OnSerializingMethod( StreamingContext context ) => this.Trace();

        [Fact]
        public static void SerializeAndDeserializeTest() {
            using ( var ms = new MemoryStream() ) {

                var orig = new MyClass();

                var ser = new BinaryFormatter();
                Console.WriteLine( "before serializing" );
                ser.Serialize( ms, orig );
                Console.WriteLine( "after serializing" );

                Console.WriteLine();
                ms.Position = 0;
                Console.WriteLine( "before deserializing" );
                ser.Deserialize( ms );
                Console.WriteLine( "after deserializing" );
            }
        }

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
    }
}