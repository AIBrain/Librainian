// Copyright © 1995-2018 to Rick@AIBrain.org and Protiguous.
// All Rights Reserved.
//
// This ENTIRE copyright notice and file header MUST BE KEPT
// VISIBLE in any source code derived from or used from our
// libraries and projects.
//
// =========================================================
// This section of source code, "AllSeeingWatcher.cs",
// belongs to Rick@AIBrain.org and Protiguous@Protiguous.com
// unless otherwise specified OR the original license has been
// overwritten by the automatic formatting.
//
// (We try to avoid that from happening, but it does happen.)
//
// Any unmodified portions of source code gleaned from other
// projects still retain their original license and our thanks
// goes to those Authors.
// =========================================================
//
// Donations (more please!), royalties from any software that
// uses any of our code, and license fees can be paid to us via
// bitcoin at the address 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2.
//
// =========================================================
// Usage of the source code or compiled binaries is AS-IS.
// No warranties are expressed or implied.
// I am NOT responsible for Anything You Do With Our Code.
// =========================================================
//
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
//
// "Librainian/Librainian/AllSeeingWatcher.cs" was last cleaned by Protiguous on 2018/05/15 at 10:41 PM.

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
        private ConcurrentList<FileSystemWatcher> FileWatchers { get; } = new ConcurrentList<FileSystemWatcher>();

        public ConcurrentDictionary<DateTime, FileSystemEventArgs> Changed { get; } = new ConcurrentDictionary<DateTime, FileSystemEventArgs>( concurrencyLevel: Environment.ProcessorCount, capacity: 123 );

        public ConcurrentDictionary<DateTime, FileSystemEventArgs> Created { get; } = new ConcurrentDictionary<DateTime, FileSystemEventArgs>( concurrencyLevel: Environment.ProcessorCount, capacity: 123 );

        public ConcurrentDictionary<DateTime, FileSystemEventArgs> Deleted { get; } = new ConcurrentDictionary<DateTime, FileSystemEventArgs>( concurrencyLevel: Environment.ProcessorCount, capacity: 123 );

        public ConcurrentDictionary<DateTime, RenamedEventArgs> Renamed { get; } = new ConcurrentDictionary<DateTime, RenamedEventArgs>( concurrencyLevel: Environment.ProcessorCount, capacity: 123 );

        private void OnChanged( Object sender, FileSystemEventArgs args ) => this.Changed[DateTime.UtcNow] = args;

        private void OnCreated( Object sender, FileSystemEventArgs args ) => this.Created[DateTime.UtcNow] = args;

        private void OnDeleted( Object sender, FileSystemEventArgs args ) => this.Deleted[DateTime.UtcNow] = args;

        private void OnRenamed( Object sender, RenamedEventArgs args ) => this.Renamed[DateTime.UtcNow] = args;

        public override void DisposeManaged() {
            this.Stop();
            base.DisposeManaged();
        }

        public void Start() {
            var drives = DriveInfo.GetDrives();

            foreach ( var drive in drives ) {
                var watcher = new FileSystemWatcher( drive.RootDirectory.FullName ) {
                    IncludeSubdirectories = true,
                    NotifyFilter = NotifyFilters.CreationTime | NotifyFilters.Size | NotifyFilters.LastAccess | NotifyFilters.LastWrite,
                    InternalBufferSize = UInt16.MaxValue
                };

                watcher.Created += this.OnCreated;
                watcher.Changed += this.OnChanged;
                watcher.Renamed += this.OnRenamed;
                watcher.Deleted += this.OnDeleted;

                this.FileWatchers.Add( item: watcher );
                ( "Added " + nameof( AllSeeingWatcher ) + " to drive " + drive.RootDirectory ).Info();

                watcher.EnableRaisingEvents = true;
            }
        }

        public void Stop() {
            foreach ( var watcher in this.FileWatchers ) {
                using ( watcher ) { }
            }
        }
    }
}