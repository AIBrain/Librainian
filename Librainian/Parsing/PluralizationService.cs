#nullable enable
namespace Librainian.Parsing {

	using System;
	using System.Globalization;
	using JetBrains.Annotations;

	public abstract class PluralizationService {

		protected PluralizationService( CultureInfo cultureInfo ) {
			if ( !cultureInfo.TwoLetterISOLanguageName.Like( "en" ) ) {
				throw new NotImplementedException( $"Unsupported {nameof( cultureInfo )} for {nameof(PluralizationService)}." );
			}

			this.Culture = cultureInfo;
		}

		[NotNull]
		public CultureInfo Culture { get; protected set; }

		public abstract Boolean IsPlural( String word );

		public abstract Boolean IsSingular( String word );

		public abstract String Pluralize( String word );

		public abstract String Singularize( String word );

		/// <summary>
		///     Factory method for PluralizationService. Only support english pluralization.
		///     Please set the PluralizationService on the System.Data.Entity.Design.EntityModelSchemaGenerator
		///     to extend the service to other locales.
		/// </summary>
		/// <param name="culture">CultureInfo</param>
		/// <returns>PluralizationService</returns>
		[NotNull]
		public static PluralizationService CreateService( CultureInfo culture ) => new EnglishPluralizationService( culture );

	}

}