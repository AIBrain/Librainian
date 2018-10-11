// Copyright © Rick@AIBrain.org and Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
//
// This source code contained in "FactDatabase.cs" belongs to Protiguous@Protiguous.com and
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
//     paypal@AIBrain.Org
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
// Project: "Librainian", "FactDatabase.cs" was last formatted by Protiguous on 2018/07/10 at 9:12 PM.

namespace Librainian.Knowledge
{

    using ComputerSystem.FileSystem;
    using JetBrains.Annotations;
    using Maths;
    using Newtonsoft.Json;
    using Parsing;
    using System;
    using System.Collections.Concurrent;
    using System.ComponentModel;
    using System.IO;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Windows.Forms;
    using Threading;

    [JsonObject]
    public class FactDatabase
    {

        [JsonProperty]
        [NotNull]
        public readonly ConcurrentBag<Document> KnbFiles = new ConcurrentBag<Document>();

        [JsonProperty]
        public Int32 FilesFound;

        public Int32 AddFile([NotNull] Document dataFile, [CanBeNull] ProgressChangedEventHandler feedback = null)
        {
            if (dataFile == null) { throw new ArgumentNullException(nameof(dataFile)); }

            if (!dataFile.Extension().Like(".knb")) { return 0; }

            Interlocked.Increment(ref this.FilesFound);
            feedback?.Invoke(this, new ProgressChangedEventArgs(this.FilesFound, $"Found data file {dataFile.FileName()}"));

            if (!this.KnbFiles.Contains(dataFile)) { this.KnbFiles.Add(dataFile); }

            //TODO text, xml, csv, html, etc...

            return 0;
        }

        public async Task ReadRandomFact([CanBeNull] Action<String> action)
        {
            if (null == action) { return; }

            await Task.Run(() =>
            {

                //pick random line from random file
                var file = this.KnbFiles.OrderBy(o => Randem.Next()).FirstOrDefault();

                if (null == file) { return; }

                try
                {

                    //pick random line
                    var line = File.ReadLines(file.FullPathWithFileName).Where(s => !String.IsNullOrWhiteSpace(s)).Where(s => Char.IsLetter(s[0])).OrderBy(o => Randem.Next()).FirstOrDefault();
                    action(line);
                }
                catch (Exception exception) { exception.Log(); }
            });
        }

        public String SearchForFactFiles(SimpleCancel cancellation)
        {
            Logging.Enter();

            try
            {
                var searchPatterns = new[] {
                    "*.knb"
                };

                var folder = new Folder(Path.Combine(Path.GetDirectoryName(Application.ExecutablePath)));

                folder.Info.FindFiles(fileSearchPatterns: searchPatterns, cancellation: cancellation, onFindFile: file => this.AddFile(dataFile: new Document(file)), onEachDirectory: null,
                    searchStyle: SearchStyle.FilesFirst);

                if (!this.KnbFiles.Any())
                {
                    folder = new Folder(Environment.SpecialFolder.CommonDocuments);

                    folder.Info.FindFiles(fileSearchPatterns: searchPatterns, cancellation: cancellation, onFindFile: file => this.AddFile(dataFile: new Document(file)), onEachDirectory: null,
                        searchStyle: SearchStyle.FilesFirst);
                }

                if (!this.KnbFiles.Any()) { searchPatterns.SearchAllDrives(onFindFile: file => this.AddFile(dataFile: new Document(file)), cancellation: cancellation); }

                return $"Found {this.KnbFiles.Count} KNB files";
            }
            finally { Logging.Exit(); }
        }
    }
}