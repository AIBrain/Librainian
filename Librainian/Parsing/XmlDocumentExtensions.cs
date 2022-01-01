﻿// Copyright © Protiguous. All Rights Reserved.
// 
// This entire copyright notice and license must be retained and must be kept visible in any binaries, libraries, repositories, or source code (directly or derived) from our binaries, libraries, projects, solutions, or applications.
// 
// All source code belongs to Protiguous@Protiguous.com unless otherwise specified or the original license has been overwritten by formatting. (We try to avoid it from happening, but it does accidentally happen.)
// 
// Any unmodified portions of source code gleaned from other sources still retain their original license and our thanks goes to those Authors.
// If you find your code unattributed in this source code, please let us know so we can properly attribute you and include the proper license and/or copyright(s).
// If you want to use any of our code in a commercial project, you must contact Protiguous@Protiguous.com for permission, license, and a quote.
// 
// Donations, payments, and royalties are accepted via bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2 and PayPal: Protiguous@Protiguous.com
// 
// ====================================================================
// Disclaimer:  Usage of the source code or binaries is AS-IS.
// No warranties are expressed, implied, or given.
// We are NOT responsible for Anything You Do With Our Code.
// We are NOT responsible for Anything You Do With Our Executables.
// We are NOT responsible for Anything You Do With Your Computer.
// ====================================================================
// 
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
// For business inquiries, please contact me at Protiguous@Protiguous.com.
// Our software can be found at "https://Protiguous.Software/"
// Our GitHub address is "https://github.com/Protiguous".
// 
// File "XmlDocumentExtensions.cs" last touched on 2022-01-01 at 5:06 AM by Protiguous.

namespace Librainian.Parsing;

using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using Maths;
using Microsoft.IO;

public static class XmlDocumentExtensions {

	private static RecyclableMemoryStreamManager MemoryStreamManager { get; } = new(MathConstants.Sizes.OneMegaByte, MathConstants.Sizes.OneGigaByte);

	public static Stream ToMemoryStream( this XmlDocument doc ) {
		var xmlStream = MemoryStreamManager.GetStream();
		doc.Save( xmlStream );
		xmlStream.Flush();
		xmlStream.Position = 0;
		return xmlStream;
	}

	public static Stream ToMemoryStream( this XDocument doc ) {
		var xmlStream = MemoryStreamManager.GetStream();
		doc.Save( xmlStream );
		xmlStream.Flush();
		xmlStream.Position = 0;
		return xmlStream;
	}

	public static XDocument ToXDocument( this XmlDocument xmlDocument ) {
		using var nodeReader = new XmlNodeReader( xmlDocument );
		nodeReader.MoveToContent();
		return XDocument.Load( nodeReader );
	}

	public static XmlDocument ToXmlDocument( this XDocument xDocument ) {
		var xmlDocument = new XmlDocument();
		using var xmlReader = xDocument.CreateReader();
		xmlDocument.Load( xmlReader );
		return xmlDocument;
	}

	public static XmlDocument ToXmlDocument( this XElement xElement ) {
		var sb = new StringBuilder();
		var xws = new XmlWriterSettings {
			OmitXmlDeclaration = true,
			Indent = false
		};
		using var xw = XmlWriter.Create( sb, xws );
		xElement.WriteTo( xw );
		var doc = new XmlDocument();
		doc.LoadXml( sb.ToString() );
		return doc;
	}

}