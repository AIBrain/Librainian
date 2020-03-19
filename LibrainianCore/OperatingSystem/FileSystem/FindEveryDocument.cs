// Copyright © 2020 Protiguous. All Rights Reserved.
// 
// This entire copyright notice and license must be retained and must be kept visible in any binaries, libraries, repositories, and source code (directly or derived)
// from our binaries, libraries, projects, or solutions.
// 
// This source code contained in "FindEveryDocument.cs" belongs to Protiguous@Protiguous.com unless otherwise specified or the original license has been overwritten
// by formatting. (We try to avoid it from happening, but it does accidentally happen.)
// 
// Any unmodified portions of source code gleaned from other projects still retain their original license and our thanks goes to those Authors.
// If you find your code in this source code, please let us know so we can properly attribute you and include the proper license and/or copyright.
// 
// If you want to use any of our code in a commercial project, you must contact Protiguous@Protiguous.com for permission and a quote.
// 
// Donations are accepted via bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2 and PayPal: Protiguous@Protiguous.com
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
// For business inquiries, please contact me at Protiguous@Protiguous.com.
// 
// Our website can be found at "https://Protiguous.com/"
// Our software can be found at "https://Protiguous.Software/"
// Our GitHub address is "https://github.com/Protiguous".
// Feel free to browse any source code we make available.
// 
// Project: "LibrainianCore", File: "FindEveryDocument.cs" was last formatted by Protiguous on 2020/03/16 at 3:09 PM.

namespace Librainian.OperatingSystem.FileSystem {

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Threading.Tasks.Dataflow;
    using ComputerSystem.Devices;
    using JetBrains.Annotations;
    using Parsing;
    using Utilities;

    public class FindEveryDocument : ABetterClassDispose {

        private BufferBlock<Document> DocumentsFound { get; }

        private ActionBlock<Disk> DrivesFound { get; set; }

        private ActionBlock<IFolder> FoldersFound { get; set; }

        private String Status { get; set; }

        public CancellationTokenSource CancellationTokenSource { get; } = new CancellationTokenSource();

        /// <summary>A list of drives that exist in the system.</summary>
        [NotNull]
        public List<Disk> PossibleDrives { get; } = new List<Disk>( capacity: ParsingConstants.EnglishAlphabetUppercase.Length );

        public IProgress<Single> Progress { get; }

        public FindEveryDocument( [NotNull] BufferBlock<Document> documentsFound, [NotNull] IProgress<Single> progress ) {
            this.DocumentsFound = documentsFound ?? throw new ArgumentNullException( paramName: nameof( documentsFound ) );
            this.Progress = progress ?? throw new ArgumentNullException( paramName: nameof( progress ) );
        }

        /// <summary>Dispose of any <see cref="IDisposable" /> (managed) fields or properties in this method.</summary>
        public override void DisposeManaged() { }

        [NotNull]
        public Task StartScanning() =>
            Task.Run( action: () => {

                foreach ( var ch in ParsingConstants.EnglishAlphabetUppercase ) {
                    var drive = new Disk( driveLetter: ch );

                    if ( drive.Exists() && drive.Info.IsReady ) {
                        this.PossibleDrives.Add( item: drive );
                    }
                }

                this.PossibleDrives.TrimExcess();

                Int64 counter = 0;

                this.DrivesFound = new ActionBlock<Disk>( action: disk => {
                    var root = new Folder( fullPath: disk.Info.RootDirectory.FullName );

                    Parallel.ForEach( source: root.BetterGetFolders().AsParallel(), body: folder => {
                        if ( this.CancellationTokenSource.IsCancellationRequested ) {
                            return;
                        }

                        this.Status = $"Found folder `{folder.FullPath}`.";
                        this.FoldersFound.Post( item: folder );
                        Interlocked.Increment( location: ref counter );
                        this.Progress.Report( value: counter );
                    } );
                } );

                this.FoldersFound = new ActionBlock<IFolder>( action: parent => {
                    Parallel.ForEach( source: parent.BetterGetFolders().AsParallel(), body: folder => {
                        if ( this.CancellationTokenSource.IsCancellationRequested ) {
                            return;
                        }

                        this.Status = $"Found folder `{folder.FullPath}`.";
                        this.FoldersFound.Post( item: folder );
                        Interlocked.Increment( location: ref counter );
                        this.Progress.Report( value: counter );
                    } );

                    Parallel.ForEach( source: parent.GetDocuments().AsParallel(), body: document => {
                        if ( this.CancellationTokenSource.IsCancellationRequested ) {
                            return;
                        }

                        this.Status = $"Found document `{document.FullPath}`.";
                        this.DocumentsFound.Post( item: document );
                        Interlocked.Increment( location: ref counter );

                        //progress.Report(counter);
                    } );
                } );
            }, cancellationToken: this.CancellationTokenSource.Token );

    }

}