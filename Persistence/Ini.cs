namespace Librainian.Persistence {

    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Runtime.Serialization;
    using System.Threading.Tasks;
    using JetBrains.Annotations;
    using Maths;
    using NUnit.Framework;
    using OperatingSystem.FileSystem;

    public static class IniTests {

        public const String ini_test_data = @"
[ Section 1  ]
;This is a comment
data1=value1
data2 =value2
data3= value3
data4 = value4
data5   =   value5


[ Section 2  ]
//This is also a comment
data11=value11
data22 = value22
data33   =   value33
data44 =value44
data55= value55
";


        public static Ini Ini1;
        public static Ini Ini2;
        public static Ini Ini3;

        [OneTimeSetUp]
        public static void Setup() {
            Ini1 = new Ini( );
            Ini2 = new Ini( );
            Ini3 = new Ini( );
        }

        [Test]
        public static void test_load_from_string() {
            Ini1 = new Ini( ini_test_data );
        }

        [Test]
        public static void test_load_from_file() {
            var tempDoc = Document.GetTempDocument();
            Ini1 = new Ini( tempDoc );
        }

        [Test]
        public static void test_load_from_file_by_static() {
            Ini1 = new Ini( ini_test_data );
        }

    }

    /// <summary>
    /// </summary>
    [DataContract( IsReference = true )]
    [Serializable]
    public class Ini {

        public Ini( String data ) {
            //TODO cheat: write out to temp file, read in, then delete temp file
            throw new NotImplementedException();
        }

        public Ini() {
            throw new NotImplementedException();
        }

        public Ini( Document document ) {

            throw new NotImplementedException();
        }

        public const String SectionBegin = "[";

        public const String SectionEnd = "]";

        public const String PairSeparator = "=";

        [DataMember]
        [NotNull]
        private ConcurrentDictionary<String, ConcurrentDictionary<String, String>> Data {
            get;
        } = new ConcurrentDictionary<String, ConcurrentDictionary<String, String>>();

        public static String EncodePair( KeyValuePair<String, String> pair ) {
            return $"{pair.Key}{PairSeparator}{pair.Value ?? String.Empty}";
        }

        public static String EncodeSection( String section ) {
            if ( section == null ) {
                throw new ArgumentNullException( nameof( section ) );
            }
            return $"{SectionBegin}{section.Trim()}{SectionEnd}\r\n";
        }

        public Boolean Add( String section, KeyValuePair< String, String > kvp ) {
            if ( String.IsNullOrWhiteSpace( section ) ) {
                throw new ArgumentNullException( nameof( section ) );
            }
            section = section.Trim();

            var retries = 10;
            TryAgain:
            ConcurrentDictionary< String, String > dict;
            if ( !this.Data.TryGetValue( section, out dict ) ) {
                this.Data[ section ] = new ConcurrentDictionary< String, String >(); //BUG chance to overwrite entire section here
            }
            try {
                this.Data[ section ][ kvp.Key.Trim() ] = kvp.Value.Trim();
                return true;
            }
            catch ( KeyNotFoundException exception ) {
                retries--;
                if ( retries.Any() ) {
                    goto TryAgain;
                }
                exception.More();
            }
            return false;
        }

        /// <summary>
        /// Removes all data from all sections.
        /// </summary>
        /// <returns></returns>
        public Boolean Clear() {
            Parallel.ForEach( this.Data.Keys, section => { Clear( section ); } );
            return !this.Data.Keys.Any();
        }

        public Boolean Clear( String section ) {
            if ( section == null ) {
                throw new ArgumentNullException( nameof( section ) );
            }
            ConcurrentDictionary<String, String> dict;
            return this.Data.TryRemove( section, out dict );
        }

        public Boolean TryLoad( Document document ) {

            if ( document == null ) {
                throw new ArgumentNullException( nameof( document ) );
            }
            if ( !document.Exists() ) {
                return false;
            }

            var currentSection = String.Empty;
            var lines = File.ReadLines( document.FullPathWithFileName );
            foreach ( var aline in lines ) {
                var line = aline.Trim();
                if ( line.StartsWith( SectionBegin) && line.EndsWith( SectionEnd ) ) {
                    currentSection = line.Substring( 1, line.Length - 2 );
                    continue;
                }
                if ( line.Contains( PairSeparator ) ) {
                    var pos = line.IndexOf( PairSeparator, StringComparison.Ordinal );
                    var key = line.Substring( 0, pos );
                    var value = line.Substring( pos + PairSeparator.Length + 1 );
                    Add( currentSection, new KeyValuePair< String, String >( key, value ) );
                    continue;
                }
            }

            return false;
        }

        /// <summary>
        /// Save the data to the specified document, overwriting it by default.
        /// </summary>
        /// <param name="document"></param>
        /// <param name="overwrite"></param>
        /// <returns></returns>
        public Boolean Save( Document document, Boolean overwrite = true ) {
            if ( document == null ) {
                throw new ArgumentNullException( nameof( document ) );
            }

            if ( document.Exists() ) {
                if ( overwrite ) {
                    document.Delete();
                }
                else {
                    return false;
                }
            }

            foreach ( var section in this.Data.Keys ) {
                WriteSection( document, section );
            }

            return false;
        }

        private Boolean WriteSection( Document document, String section ) {
            if ( document == null ) {
                throw new ArgumentNullException( nameof( document ) );
            }
            if ( section == null ) {
                throw new ArgumentNullException( nameof( section ) );
            }

            ConcurrentDictionary<String, String> dict;
            if ( !this.Data.TryGetValue( section, out dict ) ) {
                return false; //section not found
            }

            try {
                using ( var writer = File.AppendText( document.FullPathWithFileName ) ) {
                    writer.Write( EncodeSection( section ) );
                    foreach ( var pair in dict ) {
                        writer.Write( EncodePair( pair ) );
                    }

                    writer.Flush();
                }

                return true;
            }
            catch ( Exception exception ) {
                exception.More();
            }

            return false;
        }
    }
}