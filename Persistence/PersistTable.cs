namespace Librainian.Persistence {
    using System;
    using System.IO;
    using System.Windows.Forms;
    using Annotations;
    using CodeFluent.Runtime.BinaryServices;
    using Controls;
    using FluentAssertions;
    using IO;
    using Librainian.Extensions;
    using Microsoft.Isam.Esent.Collections.Generic;
    using Ninject;
    using Parsing;
    using Threading;

    /// <summary>
    ///    <para>A little wrapper over the PersistentDictionary class.</para>
    /// </summary>
    public class PersistTable<TKey, TValue> : IInitializable where TKey : IComparable<TKey> {

        [NotNull]
        public Folder Folder { get; private set; }

        [NotNull]
        public readonly PersistentDictionary<TKey, TValue> Dictionary;

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

            this.Dictionary = new PersistentDictionary<TKey, TValue>( directory );
        }

        public PersistTable( [NotNull] String fullpath ) : this( new Folder( fullpath ) ) { }


        public void Initialize() {
            Threads.Report.Enter();
            this.Dictionary.Database.Should().NotBeNullOrWhiteSpace();
            if ( this.Dictionary.Database.IsNullOrWhiteSpace() ) {
                throw new DirectoryNotFoundException( String.Format( "Unable to find or create the folder `{0}`.", this.Folder.FullName ) );
            }
            Threads.Report.Exit();
        }


        /*
                /// <summary>
                ///     check if we have a storage folder.
                ///     if we don't, popup a dialog to ask.
                ///     Settings.
                /// </summary>
                /// <returns></returns>
                public void ValidateStorageFolder() {
                    try {
                    //TODO recheck all this logic some other day
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

        private void TestForReadWriteAccess() {

            var randomFile = Path.GetTempFileName();
            File.Delete( randomFile );

            var randomFileName = Path.Combine( this.Folder.FullName, Path.GetFileName( randomFile ) );
            try {
                var temp = Path.Combine( this.MainStoragePath.FullName, String.Format( "{0}", randomFileName ) );
                NtfsAlternateStream.WriteAllText( temp, text: Randem.NextString( 144, true, true, true, true ) );
                NtfsAlternateStream.Delete( temp );
            }
            finally {
                File.Delete( randomFileName );
            }
        }

        /// <summary>
        ///     ask user for folder/network path where to store AIBrain
        /// </summary>
        public void AskUserForStorageFolder() {
            var folderBrowserDialog = new FolderBrowserDialog {
                ShowNewFolderButton = true,
                Description = "Please direct me to a folder where I can store my memory.",
                RootFolder = Environment.SpecialFolder.MyComputer
            };

            var owner = WindowWrapper.CreateWindowWrapper( Threads.CurrentProcess.MainWindowHandle );

            var dialog = folderBrowserDialog.ShowDialog( owner );

            if ( dialog != DialogResult.OK || folderBrowserDialog.SelectedPath.IsNullOrWhiteSpace() ) {
                return;
            }
            this.MainStoragePath = new DirectoryInfo( folderBrowserDialog.SelectedPath );
        }
    }
}