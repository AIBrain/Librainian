#nullable enable

namespace Librainian.Parsing
{

	using System;
	using System.Collections.Generic;
	using System.Globalization;
	using System.Linq;

	internal static class PluralizationServiceUtil
	{
		internal static Boolean DoesWordContainSuffix(String word, IEnumerable<String> suffixes, CultureInfo culture)
		{
			return suffixes.Any(s => word.EndsWith(s, true, culture));
		}

		internal static Boolean TryInflectOnSuffixInWord(String word, IEnumerable<String> suffixes, Func<String, String> operationOnWord, CultureInfo culture, out String? newWord)
		{
			newWord = null;

			if ( DoesWordContainSuffix( word, suffixes, culture ) ) {
				newWord = operationOnWord( word );

				return true;
			}

			return false;

		}
	}
}