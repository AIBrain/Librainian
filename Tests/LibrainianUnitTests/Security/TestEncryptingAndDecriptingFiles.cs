// Copyright © Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible in any binaries, libraries, repositories,
// or source code (directly or derived) from our binaries, libraries, projects, solutions, or applications.
//
// All source code belongs to Protiguous@Protiguous.com unless otherwise specified or the original license has been overwritten
// by formatting. (We try to avoid it from happening, but it does accidentally happen.)
//
// Any unmodified portions of source code gleaned from other sources still retain their original license and our thanks goes to
// those Authors. If you find your code unattributed in this source code, please let us know so we can properly attribute you
// and include the proper license and/or copyright(s). If you want to use any of our code in a commercial project, you must
// contact Protiguous@Protiguous.com for permission, license, and a quote.
//
// Donations, payments, and royalties are accepted via bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2 and PayPal: Protiguous@Protiguous.com
//
// ====================================================================
// Disclaimer:  Usage of the source code or binaries is AS-IS. No warranties are expressed, implied, or given. We are NOT
// responsible for Anything You Do With Our Code. We are NOT responsible for Anything You Do With Our Executables. We are NOT
// responsible for Anything You Do With Your Computer. ====================================================================
//
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
// For business inquiries, please contact me at Protiguous@Protiguous.com. Our software can be found at
// "https://Protiguous.Software/" Our GitHub address is "https://github.com/Protiguous".
//
// File "TestEncryptingAndDecriptingFiles.cs" last touched on 2021-11-30 at 7:23 PM by Protiguous.

namespace LibrainianUnitTests.Security;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Librainian;
using Librainian.Collections.Extensions;
using Librainian.Exceptions;
using Librainian.FileSystem;
using Librainian.Maths;
using Librainian.Measurement.Time;
using Librainian.Security;
using NUnit.Framework;

//[TestFixture]
public class TestEncryptingAndDecriptingFiles {

	private IDocument? SourceFile { get; set; }

	private IReadOnlyList<String> SourceNumbers { get; } = 0.To( 1024 * 1024 ).Select( i => i.ToString() ).ToListTrimExcess();

	//[SetUp]
	public async Task<Status> CreateSourceFile() {
		this.SourceFile = Document.GetTempDocument( "$$$" );

		await using var sw = await this.SourceFile.StreamWriter( CancellationToken.None, Encoding.Unicode ).ConfigureAwait( false );
		if ( sw is null ) {
			throw new InvalidOperationException( $"Couldn't create the {nameof( this.SourceFile )} {nameof( Document )}.{nameof( StreamWriter )}." );
		}

		foreach ( var s in this.SourceNumbers ) {
			await sw.WriteLineAsync( s ).ConfigureAwait( false );
		}

		return Status.Done;
	}

	//[TearDown]
	public async Task Done() {
		var sourceFile = this.SourceFile;
		if ( sourceFile != null ) {
			await sourceFile.Delete( CancellationToken.None );
		}
	}

	//[Test]
	public async Task TestEncryptFile() {
		var cancellationToken = new CancellationTokenSource( Minutes.One ).Token;
		var sourceFile = this.SourceFile ?? throw new NullException( nameof( this.SourceFile ) );
		var encryptedFile = Document.GetTempDocument( "$$$" );
		var decryptedFile = Document.GetTempDocument( "$$$" );

		IProgress<Decimal> progress = new Progress<Decimal>( d => TestContext.WriteLine( $"{d:P2}" ) );

		var result = await sourceFile.TryEncryptFile( encryptedFile, nameof( this.TestEncryptFile ), nameof( this.TestEncryptFile ), progress, cancellationToken )
									 .ConfigureAwait( false );

		//result.keyBytes;
		//result.ivBytes;

		await encryptedFile.TryDecryptFile( decryptedFile, nameof( this.TestEncryptFile ), nameof( this.TestEncryptFile ), progress, cancellationToken )
						   .ConfigureAwait( false );
	}
}