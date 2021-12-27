#nullable enable

namespace Librainian.Parsing;

using System;
using System.Globalization;

public abstract class PluralizationService {

	public CultureInfo Culture { get; protected set; }

	protected PluralizationService( CultureInfo cultureInfo ) {
		if ( !cultureInfo.TwoLetterISOLanguageName.Like( "en" ) ) {
			throw new NotImplementedException( $"Unsupported {nameof( cultureInfo )} for {nameof( PluralizationService )}." );
		}

		this.Culture = cultureInfo;
	}

	/// <summary>
	///     Factory method for PluralizationService. Only support english pluralization.
	///     Please set the PluralizationService on the System.Data.Entity.Design.EntityModelSchemaGenerator
	///     to extend the service to other locales.
	/// </summary>
	/// <param name="culture">CultureInfo</param>
	/// <returns>PluralizationService</returns>
	public static PluralizationService CreateService( CultureInfo culture ) => new EnglishPluralizationService( culture );

	public abstract Boolean IsPlural( String word );

	public abstract Boolean IsSingular( String word );

	public abstract String Pluralize( String word );

	public abstract String Singularize( String word );
}