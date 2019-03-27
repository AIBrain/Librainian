// Copyright © 1995-2018 to Rick@AIBrain.org and Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
//
// This source code contained in "DerivedSerializableExceptionWithAdditionalCustomPropertyTests.cs" belongs to Rick@AIBrain.org and
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
//
// "Librainian/Librainian/DerivedSerializableExceptionWithAdditionalCustomPropertyTests.cs" was last formatted by Protiguous on 2018/05/22 at 5:53 PM.

namespace LibrainianTests.Persistence {

    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Runtime.Serialization.Formatters.Binary;
    using Librainian.Extensions;
    using NUnit.Framework;

    public class DerivedSerializableExceptionWithAdditionalCustomPropertyTests {

        private const String Message = "The widget has unavoidably blooped out.";

        private const String ResourceName = "Resource-A";

        private const String Username = "Barry";

        private const String ValidationError1 = "You forgot to set the whizz bang flag.";

        private const String ValidationError2 = "Wally cannot operate in zero gravity.";

        private List<String> ValidationErrors { get; } = new List<String>();

        public DerivedSerializableExceptionWithAdditionalCustomPropertyTests() {
            this.ValidationErrors.Add( ValidationError1 );
            this.ValidationErrors.Add( ValidationError2 );
        }

        [Test]
        public void TestDerivedSerializableExceptionWithAdditionalCustomProperty() {
            var ex = new DerivedSerializableExceptionWithAdditionalCustomProperty( Message, Username, ResourceName, this.ValidationErrors );

            // Sanity check: Make sure custom properties are set before serialization
            Assert.AreEqual( Message, ex.Message, "Message" );
            Assert.AreEqual( ResourceName, ex.ResourceName, "ex.ResourceName" );
            Assert.AreEqual( 2, ex.ValidationErrors.Count, "ex.ValidationErrors.Count" );
            Assert.AreEqual( ValidationError1, ex.ValidationErrors[0], "ex.ValidationErrors[0]" );
            Assert.AreEqual( ValidationError2, ex.ValidationErrors[1], "ex.ValidationErrors[1]" );
            Assert.AreEqual( Username, ex.Username );

            // Save the full ToString() value, including the exception message and stack trace.
            var exceptionToString = ex.ToString();

            // Round-trip the exception: Serialize and de-serialize with a BinaryFormatter
            var bf = new BinaryFormatter();

            using ( var ms = new MemoryStream() ) {

                // "Save" object state
                bf.Serialize( ms, ex );

                // Re-use the same stream for de-serialization
                ms.Seek( 0, 0 );

                // Replace the original exception with de-serialized one
                ex = ( DerivedSerializableExceptionWithAdditionalCustomProperty )bf.Deserialize( ms );
            }

            // Make sure custom properties are preserved after serialization
            Assert.AreEqual( Message, ex.Message, "Message" );
            Assert.AreEqual( ResourceName, ex.ResourceName, "ex.ResourceName" );
            Assert.AreEqual( 2, ex.ValidationErrors.Count, "ex.ValidationErrors.Count" );
            Assert.AreEqual( ValidationError1, ex.ValidationErrors[0], "ex.ValidationErrors[0]" );
            Assert.AreEqual( ValidationError2, ex.ValidationErrors[1], "ex.ValidationErrors[1]" );
            Assert.AreEqual( Username, ex.Username );

            // Double-check that the exception message and stack trace (owned by the base Exception) are preserved
            Assert.AreEqual( exceptionToString, ex.ToString(), "ex.ToString()" );
        }

        [Test]
        public void TestSerializableExceptionWithCustomProperties() {
            var ex = new SerializableExceptionWithCustomProperties( Message, ResourceName, this.ValidationErrors );

            // Sanity check: Make sure custom properties are set before serialization
            Assert.AreEqual( Message, ex.Message, "Message" );
            Assert.AreEqual( ResourceName, ex.ResourceName, "ex.ResourceName" );
            Assert.AreEqual( 2, ex.ValidationErrors.Count, "ex.ValidationErrors.Count" );
            Assert.AreEqual( ValidationError1, ex.ValidationErrors[0], "ex.ValidationErrors[0]" );
            Assert.AreEqual( ValidationError2, ex.ValidationErrors[1], "ex.ValidationErrors[1]" );

            // Save the full ToString() value, including the exception message and stack trace.
            var exceptionToString = ex.ToString();

            // Round-trip the exception: Serialize and de-serialize with a BinaryFormatter
            var bf = new BinaryFormatter();

            using ( var ms = new MemoryStream() ) {

                // "Save" object state
                bf.Serialize( ms, ex );

                // Re-use the same stream for de-serialization
                ms.Seek( 0, 0 );

                // Replace the original exception with de-serialized one
                ex = ( SerializableExceptionWithCustomProperties )bf.Deserialize( ms );
            }

            // Make sure custom properties are preserved after serialization
            Assert.AreEqual( Message, ex.Message, "Message" );
            Assert.AreEqual( ResourceName, ex.ResourceName, "ex.ResourceName" );
            Assert.AreEqual( 2, ex.ValidationErrors.Count, "ex.ValidationErrors.Count" );
            Assert.AreEqual( ValidationError1, ex.ValidationErrors[0], "ex.ValidationErrors[0]" );
            Assert.AreEqual( ValidationError2, ex.ValidationErrors[1], "ex.ValidationErrors[1]" );

            // Double-check that the exception message and stack trace (owned by the base Exception) are preserved
            Assert.AreEqual( exceptionToString, ex.ToString(), "ex.ToString()" );
        }

        [Test]
        public void TestSerializableExceptionWithoutCustomProperties() {
            Exception ex = new SerializableExceptionWithoutCustomProperties( "Message", new Exception( "Inner exception." ) );

            // Save the full ToString() value, including the exception message and stack trace.
            var exceptionToString = ex.ToString();

            // Round-trip the exception: Serialize and de-serialize with a BinaryFormatter
            var bf = new BinaryFormatter();

            using ( var ms = new MemoryStream() ) {

                // "Save" object state
                bf.Serialize( ms, ex );

                // Re-use the same stream for de-serialization
                ms.Seek( 0, 0 );

                // Replace the original exception with de-serialized one
                ex = ( SerializableExceptionWithoutCustomProperties )bf.Deserialize( ms );
            }

            // Double-check that the exception message and stack trace (owned by the base Exception) are preserved
            Assert.AreEqual( exceptionToString, ex.ToString(), "ex.ToString()" );
        }
    }
}