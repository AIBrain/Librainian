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
// Contact me by email if you have any questions or helpful criticism.
//
// "Librainian/PersistTable.cs" was last cleaned by Rick on 2014/08/24 at 4:50 AM

#endregion License & Information

namespace Librainian.Persistence {

    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Runtime.Serialization;
    using System.Runtime.Serialization.Formatters;
    using System.Text;
    using System.Threading;
    using Annotations;
    using FluentAssertions;
    using IO;
    using Librainian.Extensions;
    using Microsoft.Isam.Esent.Collections.Generic;
    using Ninject;
    using Parsing;
    using Threading;

    /// <summary>
    ///     <para>A little wrapper over the PersistentDictionary class.</para>
    /// </summary>
    [DataContract( IsReference = true )]
    [DebuggerDisplay( "{DebuggerDisplay,nq}" )]
    [Serializable]
    public class PersistTable<TKey, TValue> : IInitializable, IPersistTable
        where TKey : /*struct,*/ IComparable<TKey>
        where TValue : class {

        [NotNull]
        public readonly PersistentDictionary<TKey, String> Dictionary;

        public PersistTable( [NotNull] Folder folder ) {
            if ( folder == null ) {
                throw new ArgumentNullException( "folder" );
            }
            this.Folder = folder;
            var directory = this.Folder.FullName;

            this.Folder.Create();

            if ( !this.Folder.Exists ) {
                throw new DirectoryNotFoundException( String.Format( "Unable to find or create the folder `{0}`.", this.Folder.FullName ) );
            }

            this.Dictionary = new PersistentDictionary<TKey, String>( directory );

            this.TestForReadWriteAccess();
        }


        /// <summary>
        /// <para>Here is where we interject NetDataContractSerializer to serialize to and from a String so the PersistentDictionary has no trouble with it.</para>
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        [ CanBeNull ]
        public TValue this[ TKey key ] {
            get {
                String storedValue;
                if ( !this.Dictionary.TryGetValue( key, out storedValue ) ) {
                    return null;
                }
                var deSerialized = storedValue.DeSerialize<TValue>();
                return deSerialized;
            }
            set {
                var obj = value;
                var valueToStore = obj.Serialize() ?? String.Empty;
                this.Dictionary[ key ] = valueToStore;
            }
        }


        public PersistTable( [NotNull] String fullpath )
            : this( new Folder( fullpath ) ) {
        }

        [UsedImplicitly]
        private String DebuggerDisplay {
            get {
                return this.Dictionary.ToString();
            }
        }


        /// <summary>
        /// No path given? Use the programdata\thisapp.exe type of path.
        /// </summary>
        public PersistTable() {
            throw new NotImplementedException();
            var name = Types.GetPropertyName( () => this );
            //TODO
        }

        [NotNull]
        public Folder Folder {
            get;
            private set;
        }

        public void Initialize() {
            Report.Enter();
            this.Dictionary.Database.Should().NotBeNullOrWhiteSpace();
            if ( this.Dictionary.Database.IsNullOrWhiteSpace() ) {
                throw new DirectoryNotFoundException( String.Format( "Unable to find or create the folder `{0}`.", this.Folder.FullName ) );
            }
            Report.Exit();
        }

/*
        private dynamic ToExpando( IEnumerable<KeyValuePair<TKey, TValue>> dictionary ) {
            var expandoObject = new ExpandoObject() as IDictionary<TKey, TValue>;

            if ( null != expandoObject ) {
                foreach ( var keyValuePair in dictionary ) {
                    expandoObject[ keyValuePair.Key ] = keyValuePair.Value;
                }
            }

            return expandoObject;
        }
*/

        /*
                /// <summary>
                ///     check if we have a storage folder.
                ///     if we don't, popup a dialog to ask.
                ///     Settings.
                /// </summary>
                /// <returns></returns>
                public void ValidateStorageFolder() {
                    try {

                    Again:
                        if ( null == this.MainStoragePath ) {
                            this.AskUserForStorageFolder();
                            if ( null == this.MainStoragePath ) {
                                goto Again;
                            }
                        }

                        this.MainStoragePath.Refresh();
                        if ( !this.MainStoragePath.Exists ) {
                            this.AskUserForStorageFolder();
                        }

                        if ( null == this.MainStoragePath ) {
                            return;
                        }

                        if ( null == this.MainStoragePath.Ensure( requestReadAccess: true, requestWriteAccess: true ) ) {
                            goto Again;
                        }

                        if ( !this.MainStoragePath.Exists ) {
                            var dialogResult = MessageBox.Show( String.Format( "Unable to access storage folder [{0}]. Retry?", this.MainStoragePath.FullName ), "Folder Not Found", MessageBoxButtons.RetryCancel, MessageBoxIcon.Error );
                            switch ( dialogResult ) {
                                case DialogResult.Retry:
                                    goto Again;
                                case DialogResult.Cancel:
                                    return;
                            }
                        }

                        try {
                            this.TestForReadWriteAccess();
                        }
                        catch ( Exception ) {
                            var dialogResult = MessageBox.Show( String.Format( "Unable to write to storage folder [{0}]. Retry?", this.MainStoragePath ), "No Access", MessageBoxButtons.RetryCancel );
                            switch ( dialogResult ) {
                                case DialogResult.Retry:
                                    goto Again;
                                case DialogResult.Cancel:
                                    return;
                            }
                        }
                    }
                    finally {
                        String.Format( "Using storage folder `{0}`.", this.MainStoragePath ).TimeDebug();
                    }
                }
        */

        /// <summary>
        ///     Return true if we can read/write in the <see cref="Folder" />.
        /// </summary>
        /// <returns></returns>
        private Boolean TestForReadWriteAccess() {
            try {
                Document document;
                if ( this.Folder.TryGetTempDocument( out document ) ) {
                    var text = Randem.NextString( length: 1024, lowers: true, uppers: true, numbers: true, symbols: true );
                    document.AppendText( text );
                    document.TryDeleting();
                    return true;
                }
            }
            catch ( Exception ) {

            }
            return false;
        }
    }

    public static class SerializationExtensions {
        internal static readonly ThreadLocal<StreamingContext> StreamingContexts = new ThreadLocal<StreamingContext>( () => new StreamingContext( StreamingContextStates.All ) );

        internal static readonly ThreadLocal<NetDataContractSerializer> Serializers = new ThreadLocal<NetDataContractSerializer>( () => new NetDataContractSerializer( context: StreamingContexts.Value, maxItemsInObjectGraph: Int32.MaxValue, ignoreExtensionDataObject: false, assemblyFormat: FormatterAssemblyStyle.Simple, surrogateSelector: null ) );

        [CanBeNull]
        public static String Serialize<TType>( this TType obj ) where TType : class {
            try {
                using ( var stream = new MemoryStream() ) {
                    var serializer = Serializers.Value;
                    serializer.WriteObject( stream, obj );
                    return stream.ReadToEnd();
                }
            }
            catch ( SerializationException exception ) {
                exception.Error();
            }
            return null;
        }

        [CanBeNull]
        public static TType DeSerialize<TType>( this String storedAsString ) where TType : class {
            try {
                var byteArray = Encoding.Unicode.GetBytes( storedAsString );

                using ( var ms = new MemoryStream( byteArray ) ) {
                    ms.Position = 0;
                    var serializer = Serializers.Value;
                    var deSerialized = serializer.ReadObject( ms ) as TType;
                    return deSerialized;
                }
            }
            catch ( SerializationException exception ) {
                exception.Error();
            }
            return null;
        }
    }
}