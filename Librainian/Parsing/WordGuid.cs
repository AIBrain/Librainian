// Copyright 2016 Rick@AIBrain.org.
//
// This notice must be kept visible in the source.
//
// This section of source code belongs to Rick@AIBrain.Org unless otherwise specified, or the
// original license has been overwritten by the automatic formatting of this code. Any unmodified
// sections of source code borrowed from other projects retain their original license and thanks
// goes to the Authors.
//
// Donations and royalties can be paid via
//  PayPal: Protiguous@Protiguous.com
//  bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//  litecoin: LeUxdU2w3o6pLZGVys5xpDZvvo8DUrjBp9
//
// Usage of the source code or compiled binaries is AS-IS. I am not responsible for Anything You Do.
//
// Contact me by email if you have any questions or helpful criticism.
//
// "Librainian/WordGuid.cs" was last cleaned by Rick on 2016/06/18 at 10:56 PM

namespace Librainian.Parsing {
    /*
        [Obsolete]
        public class WordGuid {
            private readonly Action< String > Info = obj => obj.WriteLine();

            private readonly WordToGuidAndGuidToWord WGGW;

            public WordGuid( Action< String > info ) {
                this.Info = null;
                if ( null != info ) {
                    this.Info += info;
                }
                this.WGGW = new WordToGuidAndGuidToWord( "WordsGuids", "xml" );
            }

            public Boolean Load() {
                this.Info( "Loading word guid database." );
                return this.WGGW.Load();
            }

            public Boolean Save() {
                this.Info( "Saving word guid database." );
                return this.WGGW.Save();
            }

            public int Count => this.WGGW.Count;

            public void Erase() {
                try {
                    this.WGGW.Clear();
                    this.Info( "All words in the word guid database have been erased." );
                }
                catch ( Exception exception ) {
                    exception.Log( );
                }
            }

            /// <summary>Adds and returns the guid for the word.</summary>
            /// <param name="word"></param>
            /// <returns></returns>
            public Guid Get( String word ) {
                try {
                    if ( String.IsNullOrEmpty( word ) ) {
                        return Guid.Empty;
                    }

                    if ( !this.WGGW.Contains( word ) ) {
                        this.WGGW[ word ] = Guid.NewGuid();
                        String.Format( "Word {0} has guid {1}.", word, this.WGGW[ word ] ).WriteLine();
                    }

                    return this.WGGW[ word ];
                }
                catch ( Exception exception ) {
                    exception.Log();
                }
                return Guid.Empty;
            }

            /// <summary>Returns the word for this guid or String.Empty;</summary>
            /// <param name="guid"></param>
            /// <returns></returns>
            public String Get( Guid guid ) {
                try {
                    if ( Guid.Empty == guid ) {
                        return String.Empty;
                    }
                    return !this.WGGW.Contains( guid ) ? String.Empty : this.WGGW[ guid ];
                }
                catch ( Exception exception ) {
                    exception.Log();
                }
                return String.Empty;
            }
        }
    */
}