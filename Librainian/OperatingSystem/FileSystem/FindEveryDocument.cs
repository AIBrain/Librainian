// Copyright © Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible in any binaries, libraries, repositories, or source code (directly or derived) from our binaries, libraries, projects, solutions, or applications.
//
// All source code belongs to Protiguous@Protiguous.com unless otherwise specified or the original license has been overwritten by formatting. (We try to avoid it from happening, but it does accidentally happen.)
//
// Any unmodified portions of source code gleaned from other sources still retain their original license and our thanks goes to those Authors.
// If you find your code unattributed in this source code, please let us know so we can properly attribute you and include the proper license and/or copyright(s).
//
// If you want to use any of our code in a commercial project, you must contact Protiguous@Protiguous.com for permission, license, and a quote.
//
// Donations, payments, and royalties are accepted via bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2 and PayPal: Protiguous@Protiguous.com
//
// ====================================================================
// Disclaimer:  Usage of the source code or binaries is AS-IS.
//     No warranties are expressed, implied, or given.
//     We are NOT responsible for Anything You Do With Our Code.
//     We are NOT responsible for Anything You Do With Our Executables.
//     We are NOT responsible for Anything You Do With Your Computer.
// ====================================================================
//
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
// For business inquiries, please contact me at Protiguous@Protiguous.com.
//
// Our software can be found at "https://Protiguous.Software/"
// Our GitHub address is "https://github.com/Protiguous".

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
        public List<Disk> PossibleDrives { get; } = new List<Disk>( ParsingConstants.English.AlphabetUppercase.Length );

        public IProgress<Single> Progress { get; }

        public FindEveryDocument( [NotNull] BufferBlock<Document> documentsFound, [NotNull] IProgress<Single> progress ) {
            this.DocumentsFound = documentsFound ?? throw new ArgumentNullException( nameof( documentsFound ) );
            this.Progress = progress ?? throw new ArgumentNullException( nameof( progress ) );
        }

        /// <summary>Dispose of any <see cref="IDisposable" /> (managed) fields or properties in this method.</summary>
        public override void DisposeManaged() { }

        [NotNull]
        public Task StartScanning() =>
            Task.Run( () => {

                foreach ( var ch in ParsingConstants.English.AlphabetUppercase ) {
                    var drive = new Disk( ch );

                    if ( drive.Exists() && drive.Info.IsReady ) {
                        this.PossibleDrives.Add( drive );
                    }
                }

                this.PossibleDrives.TrimExcess();

                Int64 counter = 0;

                this.DrivesFound = new ActionBlock<Disk>( disk => {
                    var root = new Folder( disk.Info.RootDirectory.FullName );

                    Parallel.ForEach( root.BetterGetFolders().AsParallel(), folder => {
                        if ( this.CancellationTokenSource.IsCancellationRequested ) {
                            return;
                        }

                        this.Status = $"Found folder `{folder.FullPath}`.";
                        this.FoldersFound.Post( folder );
                        Interlocked.Increment( ref counter );
                        this.Progress.Report( counter );
                    } );
                } );

                this.FoldersFound = new ActionBlock<IFolder>( parent => {
                    Parallel.ForEach( parent.BetterGetFolders().AsParallel(), folder => {
                        if ( this.CancellationTokenSource.IsCancellationRequested ) {
                            return;
                        }

                        this.Status = $"Found folder `{folder.FullPath}`.";
                        this.FoldersFound.Post( folder );
                        Interlocked.Increment( ref counter );
                        this.Progress.Report( counter );
                    } );

                    Parallel.ForEach( parent.GetDocuments().AsParallel(), document => {
                        if ( this.CancellationTokenSource.IsCancellationRequested ) {
                            return;
                        }

                        this.Status = $"Found document `{document.FullPath}`.";
                        this.DocumentsFound.Post( document );
                        Interlocked.Increment( ref counter );

                        //progress.Report(counter);
                    } );
                } );
            }, this.CancellationTokenSource.Token );

    }

}