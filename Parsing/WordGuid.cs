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
// "Librainian/WordGuid.cs" was last cleaned by Rick on 2014/08/11 at 12:40 AM
#endregion

namespace Librainian.Parsing {
    using System;
    using Collections;
    using Threading;

    [Obsolete]
    public class WordGuid {
        private readonly Action< String > Info = obj => obj.TimeDebug();

        private readonly WordToGuidAndGuidToWord WGGW;

        public WordGuid( Action< String > info ) {
            this.Info = null;
            if ( null != info ) {
                this.Info += info;
            }
            this.WGGW = new WordToGuidAndGuidToWord( "WordsGuids", "xml" );
        }

        #region ILoadable Members
        public Boolean Load() {
            this.Info( "Loading word guid database." );
            return this.WGGW.Load();
        }
        #endregion ILoadable Members

        #region ISavable Members
        public Boolean Save() {
            this.Info( "Saving word guid database." );
            return this.WGGW.Save();
        }
        #endregion ISavable Members

        public int Count { get { return this.WGGW.Count; } }

        public void Erase() {
            try {
                this.WGGW.Clear();
                this.Info( "All words in the word guid database have been erased." );
            }
            catch ( Exception exception ) {
                exception.Error( message: "Unable to erase the word guid database." );
            }
        }

        /// <summary>
        ///     Adds and returns the guid for the word.
        /// </summary>
        /// <param name="word"></param>
        /// <returns></returns>
        public Guid Get( String word ) {
            try {
                if ( String.IsNullOrEmpty( word ) ) {
                    return Guid.Empty;
                }

                if ( !this.WGGW.Contains( word ) ) {
                    this.WGGW[ word ] = Guid.NewGuid();
                    String.Format( "Word {0} has guid {1}.", word, this.WGGW[ word ] ).TimeDebug();
                }

                return this.WGGW[ word ];
            }
            catch ( Exception exception ) {
                exception.Error();
            }
            return Guid.Empty;
        }

        /// <summary>
        ///     Returns the word for this guid or String.Empty;
        /// </summary>
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
                exception.Error();
            }
            return String.Empty;
        }
    }
}
