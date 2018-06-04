// Copyright © 1995-2018 to Rick@AIBrain.org and Protiguous. All Rights Reserved.
// 
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
// 
// This source code contained in "SpeechOutput.cs" belongs to Rick@AIBrain.org and
// Protiguous@Protiguous.com unless otherwise specified or the original license has
// been overwritten by automatic formatting.
// (We try to avoid it from happening, but it does accidentally happen.)
// 
// Any unmodified portions of source code gleaned from other projects still retain their original
// license and our thanks goes to those Authors. If you find your code in this source code, please
// let us know so we can properly attribute you and include the proper license and/or copyright.
// 
// Donations, royalties from any software that uses any of our code, or license fees can be paid
// to us via bitcoin at the address 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2.
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
// For business inquiries, please contact me at Protiguous@Protiguous.com .
// 
// Our software can be found at "https://Protiguous.Software/"
// Our GitHub address is "https://github.com/Protiguous".
// Feel free to browse any source code we might have available.
// 
// ***  Project "Librainian"  ***
// File "SpeechOutput.cs" was last formatted by Protiguous on 2018/06/04 at 4:25 PM.

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

		[NotNull]
		public Lazy<SpeechSynthesizer> Synthesizer { get; } = new Lazy<SpeechSynthesizer>( () => {
			var synthesizer = new SpeechSynthesizer();

			return synthesizer;
		}, isThreadSafe: true );

		public void AttachEvents( Action<EventArgs> speechFeedbackEvent = null ) {
			try {
				if ( null == speechFeedbackEvent ) { return; }

				if ( this.Synthesizer.Value is null ) { return; }

				this.Synthesizer.Value.SpeakStarted += ( sender, e ) => speechFeedbackEvent( e );
				this.Synthesizer.Value.SpeakStarted += ( sender, e ) => speechFeedbackEvent( e );
				this.Synthesizer.Value.SpeakProgress += ( sender, e ) => speechFeedbackEvent( e );
				this.Synthesizer.Value.PhonemeReached += ( sender, e ) => speechFeedbackEvent( e );
				this.Synthesizer.Value.SpeakCompleted += ( sender, e ) => speechFeedbackEvent( e );
				this.Synthesizer.Value.StateChanged += ( sender, e ) => speechFeedbackEvent( e );
			}
			catch ( Exception exception ) { exception.More(); }
		}

		public IEnumerable<InstalledVoice> GetVoices() => this.Synthesizer.Value.GetInstalledVoices();

		public Boolean IsTalking() => this.Synthesizer.Value.State == SynthesizerState.Speaking;

		/// <summary>
		///     Start speaking ASAP Start speaking (optionally interrupting anything already being said).
		/// </summary>
		/// <param name="interruptTalking"></param>
		/// <param name="message">         </param>
		/// <param name="sayAs">           </param>
		/// <param name="emphasis">        </param>
		/// <param name="promptRate">      </param>
		/// <param name="volume">          </param>
		public void Speak( [CanBeNull] String message, Boolean interruptTalking = false, SayAs sayAs = SayAs.Text, PromptEmphasis emphasis = PromptEmphasis.None, PromptRate promptRate = PromptRate.Medium,
			PromptVolume volume = PromptVolume.NotSet ) {
			try {
				if ( String.IsNullOrEmpty( message ) ) { return; }

				message = message.Trim();

				if ( String.IsNullOrEmpty( message ) ) { return; }

				if ( message.StartsWith( "ECHO:" ) ) { message = message.Substring( "ECHO:".Length ); }

				if ( message.StartsWith( "INFO:" ) ) { message = message.Substring( "INFO:".Length ); }

				if ( message.Contains( "AIBrain" ) ) {
					message = message.Replace( "AIBrain", "A-I-Brain" ); //HACK ugh.
				}

				message = message.Trim();

				if ( interruptTalking /*&& this.IsTalking()*/ ) { this.StopTalking(); }

				var prompt = new PromptBuilder(); //7.5

				var promptStyle = new PromptStyle { Volume = volume, Emphasis = emphasis, Rate = promptRate };

				if ( emphasis == PromptEmphasis.None ) {
					if ( message.EndsWith( "!" ) ) { promptStyle.Emphasis = PromptEmphasis.Strong; }

					if ( message.EndsWith( "!!" ) ) { promptStyle.Volume = PromptVolume.Loud; }

					if ( message.EndsWith( "!!!" ) ) { promptStyle.Volume = PromptVolume.ExtraLoud; }
				}

				prompt.StartStyle( promptStyle );
				prompt.AppendTextWithHint( message, sayAs );
				prompt.EndStyle();

				this.Synthesizer.Value.SpeakAsync( prompt );
			}
			catch ( Exception exception ) { exception.More(); }
		}

		public void StopTalking() => this.Synthesizer.Value.SpeakAsyncCancelAll();

		/// <summary>
		///     Pumps message loop while Talking
		/// </summary>
		public void Wait() {
			while ( this.IsTalking() ) {
				Thread.Yield();
				Application.DoEvents();
			}
		}

		public SpeechOutput( VoiceGender gender = VoiceGender.Female, VoiceAge age = VoiceAge.Teen ) => this.Synthesizer.Value.SelectVoiceByHints( gender, age );

	}

}