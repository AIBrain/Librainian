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
// "Librainian/SpeechOutput.cs" was last cleaned by Rick on 2014/08/11 at 12:40 AM
#endregion

namespace Librainian.Speech {
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Speech.Synthesis;
    using System.Threading;
    using System.Windows.Forms;
    using Annotations;
    using Ninject;
    using Ninject.Modules;

    public interface ISpeechOutput {
        IEnumerable< InstalledVoice > GetVoices();

        /// <summary>
        ///     Pumps message loop while Talking
        /// </summary>
        void Wait();

        Boolean IsTalking();

        /// <summary>
        ///     Start speaking ASAP
        ///     Start speaking (optionally interrupting anything already being said).
        /// </summary>
        /// <param name="message"></param>
        /// <param name="interruptTalking"></param>
        void Speak( [CanBeNull] String message, Boolean interruptTalking = false );
    }

    /// <summary>
    ///     Use whatever TTS engine is available...
    /// </summary>
    public sealed class SpeechOutput : NinjectModule, IInitializable, IStartable, ISpeechOutput {
        [CanBeNull]
        private SpeechSynthesizer SpeechSynthesizer { get; set; }

        public void Initialize() {
            this.SpeechSynthesizer = new SpeechSynthesizer();
            this.SpeechSynthesizer.SelectVoiceByHints( VoiceGender.Female, VoiceAge.Teen );
        }

        public IEnumerable< InstalledVoice > GetVoices() {
            var speechSynthesizer = this.SpeechSynthesizer;
            return speechSynthesizer == null ? Enumerable.Empty< InstalledVoice >() : speechSynthesizer.GetInstalledVoices();
        }

        /// <summary>
        ///     Pumps message loop while Talking
        /// </summary>
        public void Wait() {
            while ( this.IsTalking() ) {
                Thread.Yield();
                Application.DoEvents();
            }
        }

        public Boolean IsTalking() {
            var speechSynthesizer = this.SpeechSynthesizer;
            return speechSynthesizer != null && speechSynthesizer.State == SynthesizerState.Speaking;
        }

        /// <summary>
        ///     Start speaking ASAP
        ///     Start speaking (optionally interrupting anything already being said).
        /// </summary>
        /// <param name="message"></param>
        /// <param name="interruptTalking"></param>
        public void Speak( [CanBeNull] String message, Boolean interruptTalking = false ) {
            try {
                if ( null == this.SpeechSynthesizer ) {
                    return;
                }
                if ( String.IsNullOrEmpty( message ) ) {
                    return;
                }
// ReSharper disable once PossibleNullReferenceException
                message = message.Trim();

                if ( String.IsNullOrEmpty( message ) ) {
                    return;
                }
                if ( message.StartsWith( "ECHO:" ) ) {
                    return;
                }
                if ( message.StartsWith( "INFO:" ) ) {
                    message = message.Replace( "INFO:", String.Empty );
                }

                if ( message.Contains( "AIBrain" ) ) {
                    message = message.Replace( "AIBrain", "A-I-Brain" ); //ugh..
                }

                message = message.Trim();

                if ( interruptTalking /*&& this.IsTalking()*/ ) {
                    this.SpeechSynthesizer.SpeakAsyncCancelAll();
                }
                this.SpeechSynthesizer.Speak( message );
            }
            catch ( Exception exception ) {
                exception.Error();
            }
        }

        public void Start() { }

        public void Stop() {
            var speechSynthesizer = this.SpeechSynthesizer;
            if ( speechSynthesizer != null ) {
                speechSynthesizer.SpeakAsyncCancelAll();
            }
        }

        public override void Load() { }

        public void AttachEvents( Action< EventArgs > speechFeedbackEvent = null ) {
            try {
                if ( null == speechFeedbackEvent ) {
                    return;
                }

                var speechSynthesizer = this.SpeechSynthesizer;
                if ( speechSynthesizer == null ) {
                    return;
                }

                speechSynthesizer.SpeakStarted += ( sender, e ) => speechFeedbackEvent( e );
                speechSynthesizer.SpeakStarted += ( sender, e ) => speechFeedbackEvent( e );
                speechSynthesizer.SpeakProgress += ( sender, e ) => speechFeedbackEvent( e );
                speechSynthesizer.PhonemeReached += ( sender, e ) => speechFeedbackEvent( e );
                speechSynthesizer.SpeakCompleted += ( sender, e ) => speechFeedbackEvent( e );
                speechSynthesizer.StateChanged += ( sender, e ) => speechFeedbackEvent( e );
            }
            catch ( Exception exception ) {
                exception.Error();
            }
        }
    }
}
