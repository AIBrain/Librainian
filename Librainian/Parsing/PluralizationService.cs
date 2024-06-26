﻿// Copyright © Protiguous. All Rights Reserved.
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
// "https://Protiguous.com/Software/" Our GitHub address is "https://github.com/Protiguous".
//
// File "PluralizationService.cs" last formatted on 2021-11-30 at 7:22 PM by Protiguous.

#nullable enable

namespace Librainian.Parsing;

using System;
using System.Globalization;
using System.Threading;

public interface IPluralizationService {

	Boolean IsPlural( String word );

	Boolean IsSingular( String word );

	String Pluralize( String word );

	String Singularize( String word );
}

public abstract class PluralizationService : IPluralizationService {

	protected PluralizationService( CultureInfo? cultureInfo = null ) {
		cultureInfo ??= Thread.CurrentThread.CurrentCulture;

		if ( !cultureInfo.TwoLetterISOLanguageName.Like( "en" ) ) {
			throw new NotImplementedException( $"Unsupported {nameof( cultureInfo )} for {nameof( PluralizationService )}." );
		}

		this.Culture = cultureInfo;
	}

	protected CultureInfo Culture { get; }

	/// <summary>
	/// Factory method for PluralizationService. Only support english pluralization. Please set the PluralizationService on the
	/// System.Data.Entity.Design.EntityModelSchemaGenerator to extend the service to other locales.
	/// </summary>
	/// <param name="culture">CultureInfo</param>
	/// <returns>PluralizationService</returns>
	public static PluralizationService CreateService( CultureInfo culture ) => new EnglishPluralizationService( culture );

	public abstract Boolean IsPlural( String word );

	public abstract Boolean IsSingular( String word );

	public abstract String Pluralize( String word );

	public abstract String Singularize( String word );
}