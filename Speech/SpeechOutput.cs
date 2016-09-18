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
// "Librainian/SpeechOutput.cs" was last cleaned by Rick on 2016/06/18 at 10:57 PM

using System;

[assembly: CLSCompliant( false )]
namespace Librainian.Speech {

    using System;
    using System.Collections.Generic;
    using System.Speech.Synthesis;
    using System.Threading;
    using System.Windows.Forms;
    using JetBrains.Annotations;

    public class SpeechOutput {

        public SpeechOutput( VoiceGender gender = VoiceGender.Female, VoiceAge age = VoiceAge.Teen ) {
            this.Synthesizer.Value.SelectVoiceByHints( gender, age );
        }

        [NotNull]
        public Lazy<SpeechSynthesizer> Synthesizer {
            get;
        } = new Lazy<SpeechSynthesizer>( () => {
            var synthesizer = new SpeechSynthesizer();
            return synthesizer;
        }, isThreadSafe: true );

        public void AttachEvents( Action<EventArgs> speechFeedbackEvent = null ) {
            try {
                if ( null == speechFeedbackEvent ) {
                    return;
                }

                if ( Synthesizer.Value == null ) {
                    return;
                }

                Synthesizer.Value.SpeakStarted += ( sender, e ) => speechFeedbackEvent( e );
                Synthesizer.Value.SpeakStarted += ( sender, e ) => speechFeedbackEvent( e );
                Synthesizer.Value.SpeakProgress += ( sender, e ) => speechFeedbackEvent( e );
                Synthesizer.Value.PhonemeReached += ( sender, e ) => speechFeedbackEvent( e );
                Synthesizer.Value.SpeakCompleted += ( sender, e ) => speechFeedbackEvent( e );
                Synthesizer.Value.StateChanged += ( sender, e ) => speechFeedbackEvent( e );
            }
            catch ( Exception exception ) {
                exception.More();
            }
        }

        public IEnumerable<InstalledVoice> GetVoices() {
            return Synthesizer.Value.GetInstalledVoices();
        }

        public Boolean IsTalking() {
            return Synthesizer.Value.State == SynthesizerState.Speaking;
        }

        /// <summary>
        ///     Start speaking ASAP Start speaking (optionally interrupting anything already being said).
        /// </summary>
        /// <param name="interruptTalking"></param>
        /// <param name="message"></param>
        /// <param name="sayAs"></param>
        /// <param name="emphasis"></param>
        /// <param name="promptRate"></param>
        /// <param name="volume"></param>
        public void Speak( [CanBeNull] String message, Boolean interruptTalking = false, SayAs sayAs = SayAs.Text, PromptEmphasis emphasis = PromptEmphasis.None, PromptRate promptRate = PromptRate.Medium, PromptVolume volume = PromptVolume.NotSet ) {
            try {
                if ( String.IsNullOrEmpty( message ) ) {
                    return;
                }

                message = message.Trim();

                if ( String.IsNullOrEmpty( message ) ) {
                    return;
                }
                if ( message.StartsWith( "ECHO:" ) ) {
                    message = message.Substring( "ECHO:".Length );
                }
                if ( message.StartsWith( "INFO:" ) ) {
                    message = message.Substring( "INFO:".Length );
                }
                if ( message.Contains( "AIBrain" ) ) {
                    message = message.Replace( "AIBrain", "A-I-Brain" ); //HACK ugh.
                }

                message = message.Trim();

                if ( interruptTalking /*&& this.IsTalking()*/ ) {
                    StopTalking();
                }

                var prompt = new PromptBuilder(); //7.5

                var promptStyle = new PromptStyle { Volume = volume, Emphasis = emphasis, Rate = promptRate };

                if ( emphasis == PromptEmphasis.None ) {
                    if ( message.EndsWith( "!" ) ) {
                        promptStyle.Emphasis = PromptEmphasis.Strong;
                    }
                    if ( message.EndsWith( "!!" ) ) {
                        promptStyle.Volume = PromptVolume.Loud;
                    }
                    if ( message.EndsWith( "!!!" ) ) {
                        promptStyle.Volume = PromptVolume.ExtraLoud;
                    }
                }

                prompt.StartStyle( promptStyle );
                prompt.AppendTextWithHint( message, sayAs );
                prompt.EndStyle();

                Synthesizer.Value.SpeakAsync( prompt );
            }
            catch ( Exception exception ) {
                exception.More();
            }
        }

        public void StopTalking() {
            Synthesizer.Value.SpeakAsyncCancelAll();
        }

        /// <summary>Pumps message loop while Talking</summary>
        public void Wait() {
            while ( IsTalking() ) {
                Thread.Yield();
                Application.DoEvents();
            }
        }
    }
}