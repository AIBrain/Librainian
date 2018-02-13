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
//  PayPal: paypal@aibrain.org
//  bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//  litecoin: LeUxdU2w3o6pLZGVys5xpDZvvo8DUrjBp9
//
// Usage of the source code or compiled binaries is AS-IS. I am not responsible for Anything You Do.
//
// Contact me by email if you have any questions or helpful criticism.
//
// "Librainian/SpeechInput.cs" was last cleaned by Rick on 2016/06/18 at 10:57 PM

namespace Librainian.Speech {

    using System;
    using System.Globalization;
    using System.Linq;
    using System.Speech.Recognition;
    using Collections;
    using Parsing;

    public class SpeechInput {

		public SpeechInput() => this.RecognitionEngine = new Lazy<SpeechRecognitionEngine>( () => {
			var speechRecognitionEngine = new SpeechRecognitionEngine( CultureInfo.CurrentCulture );

			try {
				speechRecognitionEngine.LoadGrammar( this.Grammar.Value );
			}
			catch ( InvalidOperationException ) { }

			try {
				speechRecognitionEngine.SetInputToDefaultAudioDevice();
			}
			catch ( InvalidOperationException ) {
				"Warning: No microphone found.".Warning();
			}

			try {
				speechRecognitionEngine.RecognizeAsync( RecognizeMode.Multiple );
			}
			catch ( InvalidOperationException ) { }

			return speechRecognitionEngine;
		}, isThreadSafe: true );

		public Lazy<Grammar> Grammar {
            get;
        } = new Lazy<Grammar>( () => {
            var grammar = new DictationGrammar { Enabled = true };
            return grammar;
        } );

        public Lazy<SpeechRecognitionEngine> RecognitionEngine {
            get;
        }

        public static Grammar CreateGrammars( params String[] phrases ) {
            if ( phrases == null ) {
                throw new ArgumentNullException( nameof( phrases ) );
            }

            var choices = new Choices( phrases );
            var builder = new GrammarBuilder( choices );
            return new Grammar( builder );
        }

        public void AttachEvent( Action<AudioLevelUpdatedEventArgs> audioLevelUpdated = null, Action<AudioSignalProblemOccurredEventArgs> audioSignalProblemOccurred = null, Action<AudioStateChangedEventArgs> audioStateChanged = null, Action<LoadGrammarCompletedEventArgs> loadGrammarCompleted = null, Action<RecognizeCompletedEventArgs> recognizeCompleted = null, Action<RecognizerUpdateReachedEventArgs> recognizerUpdateReached = null, Action<SpeechDetectedEventArgs> speechDetected = null, Action<SpeechHypothesizedEventArgs> speechHypothesized = null, Action<SpeechRecognitionRejectedEventArgs> speechRecognitionRejected = null, Action<SpeechRecognizedEventArgs> speechRecognized = null ) {
            this.RecognitionEngine.Value.AudioLevelUpdated += ( sender, args ) => audioLevelUpdated?.Invoke( args );
            this.RecognitionEngine.Value.AudioSignalProblemOccurred += ( sender, args ) => audioSignalProblemOccurred?.Invoke( args );
            this.RecognitionEngine.Value.AudioStateChanged += ( sender, args ) => audioStateChanged?.Invoke( args );
            this.RecognitionEngine.Value.LoadGrammarCompleted += ( sender, args ) => loadGrammarCompleted?.Invoke( args );
            this.RecognitionEngine.Value.RecognizeCompleted += ( sender, args ) => recognizeCompleted?.Invoke( args );
            this.RecognitionEngine.Value.RecognizerUpdateReached += ( sender, args ) => recognizerUpdateReached?.Invoke( args );
            this.RecognitionEngine.Value.SpeechDetected += ( sender, args ) => speechDetected?.Invoke( args );
            this.RecognitionEngine.Value.SpeechHypothesized += ( sender, args ) => speechHypothesized?.Invoke( args );
            this.RecognitionEngine.Value.SpeechRecognitionRejected += ( sender, args ) => speechRecognitionRejected?.Invoke( args );
            this.RecognitionEngine.Value.SpeechRecognized += ( sender, args ) => speechRecognized?.Invoke( args );
        }

		/// <summary>
		///     <seealso cref="AttachEvent" />
		/// </summary>
		/// <param name="action"></param>
		public void OnRecognizeSentence( Action<String> action ) => this.RecognitionEngine.Value.SpeechRecognized += ( s, args ) => {
			var words = args.Result.Words.Select( unit => unit.Text ).ToList();
			var sentence = words.ToStrings( ParsingExtensions.Singlespace, "." );
			action( sentence );
		};

		public void Stop() => this.RecognitionEngine.Value.RecognizeAsyncCancel();
    }
}