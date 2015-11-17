// Copyright 2015 Rick@AIBrain.org.
// 
// This notice must be kept visible in the source.
// 
// This section of source code belongs to Rick@AIBrain.Org unless otherwise specified, or the
// original license has been overwritten by the automatic formatting of this code. Any unmodified
// sections of source code borrowed from other projects retain their original license and thanks
// goes to the Authors.
// 
// Donations and Royalties can be paid via
// PayPal: paypal@aibrain.org
// bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
// litecoin: LeUxdU2w3o6pLZGVys5xpDZvvo8DUrjBp9
// 
// Usage of the source code or compiled binaries is AS-IS. I am not responsible for Anything You Do.
// 
// Contact me by email if you have any questions or helpful criticism.
// 
// "Librainian/SpeechToText.cs" was last cleaned by Rick on 2015/06/12 at 3:13 PM

namespace Librainian.Speech {

    using System;
    using System.Linq;
    using System.Speech.Recognition;
    using System.Speech.Synthesis;

    public class SpeechToText {
        private readonly SpeechRecognitionEngine _recognitionEngine = new SpeechRecognitionEngine();
        private SpeechSynthesizer _speech = new SpeechSynthesizer();

        private static Grammar CreateGrammars(String[] phrases) {
            if ( phrases == null ) {
                throw new ArgumentNullException( nameof( phrases ) );
            }

            //if ( phrases == null ) { phrases = new[] { "left", "right", "up", "down" }; }
            var choices = new Choices( phrases );
            var builder = new GrammarBuilder( choices );
            return new Grammar( builder );
        }

        private void Initialize() {
            this._recognitionEngine.SetInputToDefaultAudioDevice();
            this._recognitionEngine.SpeechRecognized += (s, args) => {
                var command = args.Result.Words.Where( word => word.Confidence > 0.5f ).Aggregate( "", (current, word) => current + word.Text + " " ).Trim();
                throw new NotImplementedException( command );
            };

            this._recognitionEngine.UnloadAllGrammars();
            this._recognitionEngine.LoadGrammar( CreateGrammars( new[] { "left", "right", "up", "down" } ) );
            this._recognitionEngine.RecognizeAsync( RecognizeMode.Multiple );
        }
    }
}