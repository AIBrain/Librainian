namespace Librainian.Extensions {

    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Runtime.Serialization.Formatters.Binary;
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