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
// "Librainian2/SpeechToText.cs" was last cleaned by Rick on 2014/08/08 at 2:31 PM
#endregion

namespace Librainian.Speech {
    using System;
    using System.Linq;
    using System.Speech.Recognition;
    using System.Speech.Synthesis;

    public class SpeechToText {
        private readonly SpeechRecognitionEngine _recognitionEngine = new SpeechRecognitionEngine();

        private SpeechSynthesizer _speech = new SpeechSynthesizer();

        private void Initialize() {
            this._recognitionEngine.SetInputToDefaultAudioDevice();
            this._recognitionEngine.SpeechRecognized += ( s, args ) => {
                                                            var command = args.Result.Words.Where( word => word.Confidence > 0.5f ).Aggregate( "", ( current, word ) => current + ( word.Text + " " ) ).Trim();
                                                            throw new NotImplementedException( command );
                                                        };

            this._recognitionEngine.UnloadAllGrammars();
            this._recognitionEngine.LoadGrammar( CreateGrammars( new[] { "left", "right", "up", "down" } ) );
            this._recognitionEngine.RecognizeAsync( RecognizeMode.Multiple );
        }

        private static Grammar CreateGrammars( String[] phrases ) {
            if ( phrases == null ) {
                throw new ArgumentNullException( "phrases" );
            }
            //if ( phrases == null ) { phrases = new[] { "left", "right", "up", "down" }; }
            var choices = new Choices( phrases );
            var builder = new GrammarBuilder( choices );
            return new Grammar( builder );
        }
    }
}
