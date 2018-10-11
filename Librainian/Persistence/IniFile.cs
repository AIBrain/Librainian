// Copyright © Rick@AIBrain.org and Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
//
// This source code contained in "IniFile.cs" belongs to Protiguous@Protiguous.com and
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
// Project: "Librainian", "IniFile.cs" was last formatted by Protiguous on 2018/09/24 at 4:28 AM.

namespace Librainian.Persistence {

    using ComputerSystem.FileSystem;
    using Extensions;
    using JetBrains.Annotations;
    using Newtonsoft.Json;
    using Parsing;
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;

    /// <summary>
    ///     A text <see cref="Document" /> with <see cref="KeyValuePair{TKey,TValue}" /> under common Sections.
    /// </summary>
    [JsonObject]
    public class IniFile {

        private const String PairSeparator = "=";

        private const String SectionBegin = "[";

        private const String SectionEnd = "]";

        [JsonProperty]
        [NotNull]
        private ConcurrentDictionary<String, ConcurrentDictionary<String, String>> Data { [DebuggerStepThrough] get; } = new ConcurrentDictionary<String, ConcurrentDictionary<String, String>>();

        /// <summary>
        ///     <para>WARNING: Set this value AFTER <see cref="Add(Document)" />.</para>
        ///     <para>If <see cref="AutoSaveDocument" /> is set, the entire dictionary/text is saved on each change.</para>
        /// </summary>
        [JsonProperty]
        public Document AutoSaveDocument { get; set; }

        [NotNull]
        public IEnumerable<String> Sections => this.Data.Keys;

        [CanBeNull]
        public IReadOnlyDictionary<String, String> this[[CanBeNull] String section] {
            [DebuggerStepThrough]
            [CanBeNull]
            get {
                if (String.IsNullOrEmpty(section)) { return null; }

                if (!this.Data.ContainsKey(section)) { return null; }

                return this.Data.TryGetValue(section, out var result) ? result : null;
            }
        }

        /// <summary>
        ///     If <see cref="AutoSaveDocument" /> is set, the entire dictionary/text is saved on each change.
        /// </summary>
        /// <param name="section"></param>
        /// <param name="key">    </param>
        /// <returns></returns>
        [CanBeNull]
        public String this[[CanBeNull] String section, [CanBeNull] String key] {
            [DebuggerStepThrough]
            [CanBeNull]
            get {
                if (String.IsNullOrEmpty(section)) { return null; }

                if (String.IsNullOrEmpty(key)) { return null; }

                if (!this.Data.ContainsKey(section)) { return null; }

                return this.Data[section].TryGetValue(key, out var value) ? value : null;
            }

            [DebuggerStepThrough]
            set {
                if (String.IsNullOrEmpty(section)) { return; }

                if (String.IsNullOrEmpty(key)) { return; }

                this.Add(section, new KeyValuePair<String, String>(key, value));

                if (null != this.AutoSaveDocument) { this.Save(this.AutoSaveDocument); }
            }
        }

        public IniFile(String data) {

            //cheat: write out to temp file, read in, then delete temp file
            var document = Document.GetTempDocument();
            document.AppendText(data);
            this.Add(document);
            this.AutoSaveDocument = null;
            document.Delete();
        }

        /// <summary>
        ///     Entire document dictionary is saved on any change.
        /// </summary>
        /// <param name="autoSaveDocument"></param>
        public IniFile([NotNull] Document autoSaveDocument) : this() {
            this.AutoSaveDocument = autoSaveDocument;
            this.Add(autoSaveDocument);
        }

        public IniFile() { }

        private Boolean WriteSection([NotNull] Document document, [NotNull] String section) {
            if (document == null) { throw new ArgumentNullException(nameof(document)); }

            if (section == null) { throw new ArgumentNullException(nameof(section)); }

            if (!this.Data.TryGetValue(section, out var dict)) {
                return false; //section not found
            }

            try {
                using (var writer = File.AppendText(document.FullPathWithFileName)) {
                    writer.Write(EncodeSection(section));

                    foreach (var pair in dict.OrderBy(pair => pair.Key)) { writer.WriteLine(EncodePair(pair)); }

                    writer.Write(Environment.NewLine);
                    writer.Flush();
                }

                return true;
            }
            catch (Exception exception) { exception.Log(); }

            return false;
        }

        private async Task<Boolean> WriteSectionAsync([NotNull] Document document, [NotNull] String section) {
            if (document == null) { throw new ArgumentNullException(nameof(document)); }

            if (section == null) { throw new ArgumentNullException(nameof(section)); }

            try {
                if (!this.Data.TryGetValue(section, out var dict)) {
                    return false; //section not found
                }

                using (var writer = File.AppendText(document.FullPathWithFileName)) {
                    writer.Write(EncodeSection(section));

                    foreach (var pair in dict.OrderBy(pair => pair.Key)) { await writer.WriteAsync(EncodePair(pair)); }

                    await writer.WriteLineAsync();
                    await writer.FlushAsync();
                }

                return true;
            }
            catch (Exception exception) { exception.Log(); }

            return false;
        }

        private Boolean WriteSectionJSON(Document document, [NotNull] String section) {
            if (!this.Data.TryGetValue(section, out var dict)) {
                return false; //section not found
            }

            try { return true; }
            catch (Exception exception) { exception.Log(); }

            return false;
        }

        [NotNull]
        [DebuggerStepThrough]
        public static String EncodePair(KeyValuePair<String, String> pair) => $"{pair.Key}{PairSeparator}{pair.Value ?? String.Empty}";

        [NotNull]
        [DebuggerStepThrough]
        public static String EncodeSection([NotNull] String section) {
            if (section == null) { throw new ArgumentNullException(nameof(section)); }

            return $"{SectionBegin}{section.Trim()}{SectionEnd}{Environment.NewLine}";
        }

        /// <summary>
        ///     (Trims whitespaces from section, key, and value.)
        /// </summary>
        /// <param name="section"></param>
        /// <param name="kvp">    </param>
        /// <returns></returns>
        public Boolean Add(String section, KeyValuePair<String, String> kvp) {
            if (String.IsNullOrWhiteSpace(section)) { throw new ArgumentException("Argument == null or whitespace", nameof(section)); }

            section = section.Trim();

            var retries = 10;
            TryAgain:

            lock (this.Data) {
                if (!this.Data.ContainsKey(section)) { this.Data[section] = new ConcurrentDictionary<String, String>(); }
            }

            try {
                this.Data[section][kvp.Key.Trim()] = kvp.Value.Trim();

                return null == this.AutoSaveDocument || this.Save(this.AutoSaveDocument);
            }
            catch (KeyNotFoundException exception) {
                retries--;

                if (retries.Any()) { goto TryAgain; }

                exception.Log();
            }

            return false;
        }

        public Boolean Add([NotNull] Document document) {
            if (document == null) { throw new ArgumentNullException(nameof(document)); }

            if (!document.Exists()) { return false; }

            try {
                var lines = File.ReadLines(document.FullPathWithFileName).Where(line => !String.IsNullOrWhiteSpace(line));

                //.ToList();

                return this.Add(lines);
            }
            catch (IOException exception) {

                //file in use by another app
                exception.Log();

                return false;
            }
            catch (OutOfMemoryException exception) {

                //file is huge
                exception.Log();

                return false;
            }
        }

        public Boolean Add(String text) {
            text = text.Replace(Environment.NewLine, "\n");

            var lines = text.Split(new[] {
                '\n'
            }, StringSplitOptions.RemoveEmptyEntries);

            return this.Add(lines);
        }

        public Boolean Add([NotNull] IEnumerable<String> lines) {
            if (lines == null) { throw new ArgumentNullException(nameof(lines)); }

            var counter = 0;
            var section = String.Empty;

            foreach (var line in lines.Where(s => !s.IsNullOrEmpty()).Select(aline => aline.Trim()).Where(line => !line.IsNullOrWhiteSpace())) {
                if (line.StartsWith(SectionBegin) && line.EndsWith(SectionEnd)) {
                    section = line.Substring(SectionBegin.Length, line.Length - (SectionBegin.Length + SectionEnd.Length)).Trim();

                    continue;
                }

                if (line.Contains(PairSeparator)) {
                    var pos = line.IndexOf(PairSeparator, StringComparison.Ordinal);
                    var key = line.Substring(0, pos).Trim();
                    var value = line.Substring(pos + PairSeparator.Length);

                    if (this.Add(section, new KeyValuePair<String, String>(key, value))) { counter++; }
                }
            }

            return counter.Any();
        }

        /// <summary>
        ///     Return the entire structure as a JSON formatted String.
        /// </summary>
        /// <returns></returns>
        [NotNull]
        public String AsJSON() {
            var tempDocument = Document.GetTempDocument();

            var writer = File.CreateText(tempDocument.FullPathWithFileName);

            using (JsonWriter jw = new JsonTextWriter(writer)) {
                jw.Formatting = Formatting.Indented;
                var serializer = new JsonSerializer();
                serializer.Serialize(jw, this.Data);
            }

            var text = File.ReadAllText(tempDocument.FullPathWithFileName);

            return text;
        }

        /// <summary>
        ///     Removes all data from all sections.
        /// </summary>
        /// <returns></returns>
        public Boolean Clear() {
            Parallel.ForEach(this.Data.Keys, section => this.TryRemove(section));

            return !this.Data.Keys.Any();
        }

        /// <summary>
        ///     Save the data to the specified document, overwriting it by default.
        /// </summary>
        /// <param name="document"> </param>
        /// <param name="overwrite"></param>
        /// <returns></returns>
        public Boolean Save([NotNull] Document document, Boolean overwrite = true) {
            if (document == null) { throw new ArgumentNullException(nameof(document)); }

            if (document.Exists()) {
                if (overwrite) { document.Delete(); }
                else { return false; }
            }

            foreach (var section in this.Data.Keys.OrderBy(section => section)) { this.WriteSection(document, section); }

            return true;
        }

        /// <summary>
        ///     Save the data to the specified document, overwriting it by default.
        /// </summary>
        /// <param name="document"> </param>
        /// <param name="overwrite"></param>
        /// <returns></returns>
        public async Task<Boolean> SaveAsync([NotNull] Document document, Boolean overwrite = true) {
            if (document == null) { throw new ArgumentNullException(nameof(document)); }

            if (document.Exists()) {
                if (overwrite) { document.Delete(); }
                else { return false; }
            }

            foreach (var section in this.Data.Keys.OrderBy(section => section)) { await this.WriteSectionAsync(document, section); }

            return false;
        }

        [DebuggerStepThrough]
        public Boolean TryRemove([NotNull] String section) {
            if (section == null) { throw new ArgumentNullException(nameof(section)); }

            return this.Data.TryRemove(section, out var dict);
        }

        [DebuggerStepThrough]
        public Boolean TryRemove([NotNull] String section, String key) {
            if (section == null) { throw new ArgumentNullException(nameof(section)); }

            if (!this.Data.ContainsKey(section)) { return false; }

            return this.Data[section].TryRemove(key, out var value);
        }
    }
}