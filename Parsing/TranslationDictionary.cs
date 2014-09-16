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
// "Librainian/TranslationDictionary.cs" was last cleaned by Rick on 2014/08/11 at 12:40 AM
#endregion

namespace Librainian.Parsing {
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using Threading;

    public class TranslationDictionary {
        public enum Languages {
            English,

            Spanish,

            Russian,

            German
        }

        //TODO use a concurrentBag()
        private readonly StringCollection Lookupsinprogress = new StringCollection();

        /// <summary>
        ///     The language defined in this dictionary.
        /// </summary>
        public Languages Language = Languages.English;

        //public static AIBrain.Persist.PersistedHashTable RawDefinitions;
        //public static Persisted.PersistedPKPKTable Definitions;
        //public static Persisted.PersistedTable Synonyms;
        /// <summary>
        ///     A semi-static cache of definitions pulled from the internet (so we don't have to redownload all the time).
        /// </summary>
        /// <summary>
        ///     PK(Word,Index) { RecordDate, DictionaryID, PartOfSpeech, Definition }
        /// </summary>
        /// <summary>
        ///     Synonyms are different words (or sometimes phrases) with identical or very similar meanings.
        /// </summary>
        /// <summary>
        ///     Antonyms are words with opposite or nearly opposite meanings.
        /// </summary>
        //public static Persisted.PersistedTable Antonyms;
        private TranslationDictionary() {
            this.Language = Languages.English;
            this.LoadDefinitions();
        }

        public long LookupsInProgress {
            get {
                var syncRoot = this.Lookupsinprogress.SyncRoot;
                if ( syncRoot != null ) {
                    lock ( syncRoot ) {
                        return this.Lookupsinprogress.Count;
                    }
                }
                return 0;
            }
        }

        public List< String > GetDefinitions( String word ) {
            var results = new List< String >( 5 );
            return results;
        }

        public void LoadDefinitions() {
            //RawDefinitions = new Persist.PersistedHashTable( "Raw Dictionary Definitions for " + Language.ToString() );

            //Definitions = new Persisted.PersistedPKPKTable( Language + " Dictionary" );
            //if ( !Definitions.isLoaded ) {
            //    Definitions.AddPrimaryKeys( "Word", typeof( String ), "Index", typeof( uint ) );
            //    Definitions.AddField( "DictionaryID", typeof( Guid ), Common.Uniqueness.Unique );
            //    Definitions.AddField( "PartOfSpeech", typeof( PartOfSpeech ), Common.Uniqueness.NotUnique );
            //    Definitions.AddField( "RecordDate", typeof( DateTime ), Common.Uniqueness.NotUnique );
            //    Definitions.AddField( "Definition", typeof( String ), Common.Uniqueness.NotUnique );
            //    Definitions.Save();
            //}

            //Synonyms = new Persisted.PersistedTable( Language + " Synonyms" );
            //if ( !Synonyms.isLoaded ) {
            //    Synonyms.AddPrimaryKey( "DictionaryID", typeof( Guid ) );   //from Definitions
            //    Synonyms.AddField( "SynonymID", typeof( Guid ), Common.Uniqueness.Unique );
            //    Synonyms.AddField( "RecordDate", typeof( DateTime ), Common.Uniqueness.NotUnique );
            //    Synonyms.AddField( "List", typeof( List<Guid> ), Common.Uniqueness.NotUnique );
            //    Synonyms.Save();
            //}

            //Antonyms = new Persisted.PersistedTable( Language + " Antonyms" );
            //if ( !Antonyms.isLoaded ) {
            //    Antonyms.AddPrimaryKey( "DictionaryID", typeof( Guid ) );   //from Definitions
            //    Antonyms.AddField( "AntonymID", typeof( Guid ), Common.Uniqueness.Unique );
            //    Antonyms.AddField( "RecordDate", typeof( DateTime ), Common.Uniqueness.NotUnique );
            //    Antonyms.AddField( "List", typeof( List<Guid> ), Common.Uniqueness.NotUnique );
            //    Antonyms.Save();
            //}
        }

        public Boolean Save() {
            ////lock ( RawDefinitions.SynchRoot ) { if ( !RawDefinitions.Save() ) { AllGood = false; } }
            //lock ( Definitions.Rows.SyncRoot ) { if ( !Definitions.Save() ) { AllGood = false; } }
            //lock ( Synonyms.Rows.SyncRoot ) { if ( !Synonyms.Save() ) { AllGood = false; } }
            //lock ( Antonyms.Rows.SyncRoot ) { if ( !Antonyms.Save() ) { AllGood = false; } }
            var AllGood = Randem.NextBoolean();
            return AllGood;
        }

        //private static List<String> fileListIndex = DbFileHelper.GetIndexForType( DbPartOfSpeechType.All );
        //private static List<String> fileListData = DbFileHelper.GetDBaseForType( DbPartOfSpeechType.All );

        /*
        /// <summary>
        /// Starts a pull of definitions for each word in sentence (if not already in raw).
        /// </summary>
        /// <param name="BaseWord"></param>
        /// <returns></returns>
        public static void Overlook( String sentence ) {
            return; //TODO anything here?

            foreach ( var word in AIBrain.Parsing.Sentence.ToWords( sentence ).Distinct() ) {
                if ( GetDefinitions( word ).Count() < 1 ) {
                    lock ( _lookupsinprogress.SyncRoot ) {
                        if ( !_lookupsinprogress.Contains( word ) ) {
                            Dictionary<String, List<Definition>> retVal = new Dictionary<String, List<Definition>>();

                            for ( int i = 0; i < fileListIndex.Count; i++ ) {
                                long offset = FileParser.FastSearch( word.ToLower(), fileListIndex[ i ] );
                                if ( offset > 0 ) {
                                    Index idx = FileParser.ParseIndex( offset, fileListIndex[ i ] );
                                    foreach ( long synSetOffset in idx.SynSetsOffsets ) {
                                        try {
                                            Definition def = FileParser.ParseDefinition( synSetOffset, fileListData[ i ], word );
                                            String wordKey = String.Join( ", ", def.Words.ToArray() );
                                            if ( !retVal.ContainsKey( wordKey ) ) { retVal.Add( wordKey, new List<Definition>() ); }

                                            retVal[ wordKey ].Add( def );
                                        }
                                        catch ( Exception Error ) {
                                            Utility.LogException( Error );
                                        }
                                    }
                                }
                            }

                            Console.WriteLine( retVal.ToString() );
                        }
                        else {
                            Debug.WriteLine( String.Format( "Skipping '{0}': already being looked up." ) );
                        }
                    }
                }
            }
        }
        */

        /*
        private static void DictionaryService_DefineInDictCompleted( object sender, DefineInDictCompletedEventArgs e ) {
            if ( e.Cancelled ) { return; }
            if ( null != e.Error ) {
                Utility.LogException( e.Error, e.Error.Message );
                return;
            }

            lock ( _lookupsinprogress ) {
                _lookupsinprogress.Remove( e.Result.Word );
            }

            foreach ( var item in e.Result.Definitions ) {
                lock ( RawDefinitions.SynchRoot ) {
                    if ( !RawDefinitions.Collection.ContainsKey( item.Word ) ) { RawDefinitions.isDirty = true; }
                    if ( RawDefinitions.Collection[ item.Word ] is String ) {
                        String currentValue = RawDefinitions.Collection[ item.Word ] as String;
                        if ( !currentValue.Equals( item.WordDefinition ) ) { RawDefinitions.isDirty = true; }
                    }
                    RawDefinitions.Collection[ item.Word ] = item.WordDefinition;
                }
            }
            lock ( RawDefinitions.SynchRoot ) {
                RawDefinitions.Save();
            }

            foreach ( var item in e.Result.Definitions ) { Grok( item.Word ); }
        }
        */

        //private static ReaderWriterLockSlim UnoGrokker = new ReaderWriterLockSlim( System.Threading.LockRecursionPolicy.SupportsRecursion );

        /*
        public static void Grok( String DaWord ) {
            try {
                UnoGrokker.EnterWriteLock();

                if ( String.IsNullOrEmpty( DaWord ) ) { return; }

                String rawdefinitions = String.Empty;

                lock ( RawDefinitions.SynchRoot ) {
                    if ( RawDefinitions[ DaWord ] is String ) {
                        rawdefinitions = RawDefinitions[ DaWord ] as String;
                    }
                }

                if ( String.IsNullOrEmpty( rawdefinitions ) ) { return; }

                String firstword = rawdefinitions.FirstWord();
                if ( !firstword.Equals( DaWord ) ) { return; }

                rawdefinitions = rawdefinitions.Remove( 0, DaWord.Length + 2 ).Trim();
                //rawdefinitions += "\r\n" + rawdefinitions;

                // "n 1: an expression of greeting; \"every morning they exchanged\n          polite hellos\" [syn: {hello}, {hullo}, {howdy}, {how-do-you-do}]\n     2: a state in the United States in the central Pacific on the\n        Hawaiian Islands [syn: {Hawaii}, {Aloha State}]"

                String[] rawparts = Regex.Split( rawdefinitions, @"(?=[nv]\s[1-9]+:\s\w+)", RegexOptions.Singleline ); //magic.
                foreach ( var rawdef in rawparts ) {
                    if ( String.IsNullOrEmpty( rawdef.Trim() ) ) { continue; }
                    String pos = rawdef.Substring( 0, 2 ).Trim();
                }

                //AIBrain.Brain.BlackBoxClass.Echo( String.Format( "{0} has a PoS {1} and a definition of {2}.", item.Word, item.WordDefinition, item.Dictionary.Id ) );
            }
            finally {
                UnoGrokker.ExitWriteLock();
            }
        }
        */
    }
}
