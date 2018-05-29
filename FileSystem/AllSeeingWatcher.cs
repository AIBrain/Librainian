// Copyright 2018 Rick@AIBrain.org.
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
// "Librainian/AllSeeingWatcher.cs" was last cleaned by Rick on 2018/01/31 at 7:08 PM

namespace Librainian.FileSystem {
    using System;
    using System.Collections.Concurrent;
    using System.IO;
    using Collections;
    using JetBrains.Annotations;
    using Magic;

    /// <summary>
    /// </summary>
    public class AllSeeingWatcher : ABetterClassDispose {
        [ItemNotNull]
        [NotNull]
        private readonly ConcurrentList<FileSystemWatcher> FileWatcher = new ConcurrentList<FileSystemWatcher>();

        public ConcurrentDictionary<DateTime, FileSystemEventArgs> Created { get; } = new ConcurrentDictionary<DateTime, FileSystemEventArgs>( concurrencyLevel: Environment.ProcessorCount, capacity: 123 );

        public ConcurrentDictionary<DateTime, FileSystemEventArgs> Changed { get; } = new ConcurrentDictionary<DateTime, FileSystemEventArgs>( concurrencyLevel: Environment.ProcessorCount, capacity: 123 );

        public ConcurrentDictionary<DateTime, RenamedEventArgs> Renamed { get; } = new ConcurrentDictionary<DateTime, RenamedEventArgs>( concurrencyLevel: Environment.ProcessorCount, capacity: 123 );

        public ConcurrentDictionary<DateTime, FileSystemEventArgs> Deleted { get; } = new ConcurrentDictionary<DateTime, FileSystemEventArgs>( concurrencyLevel: Environment.ProcessorCount, capacity: 123 );


        public void Start() {
            var drives = DriveInfo.GetDrives();

            foreach ( var drive in drives ) {
                var watcher = new FileSystemWatcher( path: drive.RootDirectory.FullName ) { IncludeSubdirectories = true, NotifyFilter = NotifyFilters.CreationTime | NotifyFilters.Size | NotifyFilters.LastAccess | NotifyFilters.LastWrite, InternalBufferSize = UInt16.MaxValue };

                watcher.Created += this.OnCreated;
                watcher.Changed += this.OnChanged;
                watcher.Renamed += this.OnRenamed;
                watcher.Deleted += this.OnDeleted;

                this.FileWatcher.Add( item: watcher );
                ( "Added " + nameof( AllSeeingWatcher ) + " to drive " + drive.RootDirectory ).Info();

                watcher.EnableRaisingEvents = true;
            }
        }

        public void Stop() {
            foreach ( var watcher in this.FileWatcher ) {
                using ( watcher ) { }
            }
        }

        private void OnCreated( Object sender, FileSystemEventArgs args ) => this.Created[key: DateTime.UtcNow] = args;

        private void OnChanged( Object sender, FileSystemEventArgs args ) => this.Changed[key: DateTime.UtcNow] = args;

        private void OnRenamed( Object sender, RenamedEventArgs args ) => this.Renamed[key: DateTime.UtcNow] = args;

        private void OnDeleted( Object sender, FileSystemEventArgs args ) => this.Deleted[key: DateTime.UtcNow] = args;

        protected override void DisposeManaged() {
            this.Stop();
            base.DisposeManaged();
        }
    }
}