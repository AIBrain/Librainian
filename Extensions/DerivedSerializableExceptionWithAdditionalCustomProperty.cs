// Copyright 2018 Protiguous.
//
// This notice must be kept visible in the source.
//
// This section of source code belongs to Protiguous@Protiguous.com unless otherwise specified or the original license has been overwritten by the automatic formatting of this code.
//
// Any unmodified sections of source code borrowed from other projects retain their original license and thanks goes to the Authors.
//
// Donations, royalties, and licenses can be paid via bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//
// Usage of the source code or compiled binaries is AS-IS. I am not responsible for Anything You Do.
//
// Contact me by email if you have any questions or helpful criticism.
//
// "Librainian/DerivedSerializableExceptionWithAdditionalCustomProperty.cs" was last cleaned by Protiguous on 2018/05/12 at 1:22 AM

namespace Librainian.Extensions {

    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Runtime.Serialization;
    using System.Runtime.Serialization.Formatters.Binary;
    using System.Security.Permissions;
    using Newtonsoft.Json;
    using NUnit.Framework;

    [JsonObject]
    [Serializable]
    public sealed class DerivedSerializableExceptionWithAdditionalCustomProperty : SerializableExceptionWithCustomProperties {

        [SecurityPermission( SecurityAction.Demand, SerializationFormatter = true )]

        // Serialization constructor is private, as this class is sealed
        private DerivedSerializableExceptionWithAdditionalCustomProperty( SerializationInfo info, StreamingContext context ) : base( info, context ) => this.Username = info.GetString( "Username" );

        public DerivedSerializableExceptionWithAdditionalCustomProperty() { }

        public DerivedSerializableExceptionWithAdditionalCustomProperty( String message ) : base( message ) { }

        public DerivedSerializableExceptionWithAdditionalCustomProperty( String message, Exception innerException ) : base( message, innerException ) { }

        public DerivedSerializableExceptionWithAdditionalCustomProperty( String message, String username, String resourceName, IList<String> validationErrors ) : base( message, resourceName, validationErrors ) =>
            this.Username = username;

        public DerivedSerializableExceptionWithAdditionalCustomProperty( String message, String username, String resourceName, IList<String> validationErrors, Exception innerException ) : base( message, resourceName,
            validationErrors, innerException ) =>
            this.Username = username;

        public String Username { get; }

        public override void GetObjectData( SerializationInfo info, StreamingContext context ) {
            if ( info is null ) {
                throw new ArgumentNullException( nameof( info ) );
            }

            info.AddValue( "Username", this.Username );
            base.GetObjectData( info, context );
        }
    }

    public class UnitTests {

        private const String Message = "The widget has unavoidably blooped out.";

        private const String ResourceName = "Resource-A";

        private const String Username = "Barry";

        private const String ValidationError1 = "You forgot to set the whizz bang flag.";

        private const String ValidationError2 = "Wally cannot operate in zero gravity.";

        private readonly List<String> _validationErrors = new List<String>();

        public UnitTests() {
            this._validationErrors.Add( ValidationError1 );
            this._validationErrors.Add( ValidationError2 );
        }

        [Test]
        public void TestDerivedSerializableExceptionWithAdditionalCustomProperty() {
            var ex = new DerivedSerializableExceptionWithAdditionalCustomProperty( Message, Username, ResourceName, this._validationErrors );

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
            var ex = new SerializableExceptionWithCustomProperties( Message, ResourceName, this._validationErrors );

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