// Copyright © Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible in any binaries, libraries, repositories, or source code (directly or derived) from our binaries, libraries, projects, solutions, or applications.
//
// All source code belongs to Protiguous@Protiguous.com unless otherwise specified or the original license has been overwritten by formatting. (We try to avoid it from happening, but it does accidentally happen.)
//
// Any unmodified portions of source code gleaned from other sources still retain their original license and our thanks goes to those Authors.
// If you find your code unattributed in this source code, please let us know so we can properly attribute you and include the proper license and/or copyright(s).
//
// If you want to use any of our code in a commercial project, you must contact Protiguous@Protiguous.com for permission, license, and a quote.
//
// Donations, payments, and royalties are accepted via bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2 and PayPal: Protiguous@Protiguous.com
//
// ====================================================================
// Disclaimer:  Usage of the source code or binaries is AS-IS.
//     No warranties are expressed, implied, or given.
//     We are NOT responsible for Anything You Do With Our Code.
//     We are NOT responsible for Anything You Do With Our Executables.
//     We are NOT responsible for Anything You Do With Your Computer.
// ====================================================================
//
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
// For business inquiries, please contact me at Protiguous@Protiguous.com.
//
// Our software can be found at "https://Protiguous.Software/"
// Our GitHub address is "https://github.com/Protiguous".

namespace Librainian.Graphics.Video {

	using System;
	using JetBrains.Annotations;

	public struct CarrierImage {

		//count of frames in the video stream, or 0
		public Int32 AviCountFrames;

		public Int64[] AviMessageBytesToHide;

		//width * height
		public Int64 CountPixels;

		//how many bytes will be hidden in this image - this field is set by CryptUtility.HideOrExtract()
		public Int64 MessageBytesToHide;

		//file name to save the new image
		public String ResultFileName;

		//file name of the clean image
		public String SourceFileName;

		//produce colorful (false) or grayscale noise (true) for this picture
		public Boolean UseGrayscale;

		public CarrierImage( [CanBeNull] String sourceFileName, [CanBeNull] String resultFileName, Int64 countPixels, Int32 aviCountFrames, Boolean useGrayscale ) {
			this.SourceFileName = sourceFileName;
			this.ResultFileName = resultFileName;
			this.CountPixels = countPixels;
			this.AviCountFrames = aviCountFrames;
			this.UseGrayscale = useGrayscale;
			this.MessageBytesToHide = 0;
			this.AviMessageBytesToHide = null;
		}

		public void SetCountBytesToHide( Int64 messageBytesToHide ) {
			this.MessageBytesToHide = messageBytesToHide;

			if ( this.SourceFileName.ToLower().EndsWith( ".avi" ) ) {
				this.AviMessageBytesToHide = new Int64[ this.AviCountFrames ];

				//calculate count of message-bytes to hide in (or extract from) each image
				Int64 sumBytes = 0;

				for ( var n = 0; n < this.AviCountFrames; n++ ) {
					this.AviMessageBytesToHide[ n ] = ( Int64 )Math.Ceiling( messageBytesToHide / ( Single )this.AviCountFrames );
					sumBytes += this.AviMessageBytesToHide[ n ];
				}

				if ( sumBytes > messageBytesToHide ) {

					//correct Math.Ceiling effects
					this.AviMessageBytesToHide[ this.AviCountFrames - 1 ] -= sumBytes - messageBytesToHide;
				}
			}
		}
	}
}