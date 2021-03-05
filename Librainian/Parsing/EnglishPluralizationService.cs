#nullable enable

namespace Librainian.Parsing {

	using System;
	using System.Collections.Generic;
	using System.Diagnostics.CodeAnalysis;
	using System.Globalization;
	using System.Linq;
	using System.Text;
	using System.Text.RegularExpressions;
	using Collections;

	/// <summary>
	///     Originally based from
	///     https://github.com/doctorcode-org/DoctorCode.Pluralization/blob/master/DoctorCode.Pluralization/EnglishPluralizationService.cs
	/// </summary>
	public class EnglishPluralizationService : PluralizationService {

		private readonly IDictionary<String, String> _assimilatedClassicalInflectionDictionary = new Dictionary<String, String> {
			{
				"alumna", "alumnae"
			}, {
				"alga", "algae"
			}, {
				"vertebra", "vertebrae"
			}, {
				"codex", "codices"
			}, {
				"murex", "murices"
			}, {
				"silex", "silices"
			}, {
				"aphelion", "aphelia"
			}, {
				"hyperbaton", "hyperbata"
			}, {
				"perihelion", "perihelia"
			}, {
				"asyndeton", "asyndeta"
			}, {
				"noumenon", "noumena"
			}, {
				"phenomenon", "phenomena"
			}, {
				"criterion", "criteria"
			}, {
				"organon", "organa"
			}, {
				"prolegomenon", "prolegomena"
			}, {
				"agendum", "agenda"
			}, {
				"datum", "data"
			}, {
				"extremum", "extrema"
			}, {
				"bacterium", "bacteria"
			}, {
				"desideratum", "desiderata"
			}, {
				"stratum", "strata"
			}, {
				"candelabrum", "candelabra"
			}, {
				"erratum", "errata"
			}, {
				"ovum", "ova"
			}, {
				"forum", "fora"
			}, {
				"addendum", "addenda"
			}, {
				"stadium", "stadia"
			}, {
				"automaton", "automata"
			}, {
				"polyhedron", "polyhedra"
			}
		};

		private readonly IDictionary<String, String> _classicalInflectionDictionary = new Dictionary<String, String> {
			{
				"stamen", "stamina"
			}, {
				"foramen", "foramina"
			}, {
				"lumen", "lumina"
			}, {
				"anathema", "anathemata"
			}, {
				"----", "----ta"
			}, {
				"oedema", "oedemata"
			}, {
				"bema", "bemata"
			}, {
				"enigma", "enigmata"
			}, {
				"sarcoma", "sarcomata"
			}, {
				"carcinoma", "carcinomata"
			}, {
				"gumma", "gummata"
			}, {
				"schema", "schemata"
			}, {
				"charisma", "charismata"
			}, {
				"lemma", "lemmata"
			}, {
				"soma", "somata"
			}, {
				"diploma", "diplomata"
			}, {
				"lymphoma", "lymphomata"
			}, {
				"stigma", "stigmata"
			}, {
				"dogma", "dogmata"
			}, {
				"magma", "magmata"
			}, {
				"stoma", "stomata"
			}, {
				"drama", "dramata"
			}, {
				"melisma", "melismata"
			}, {
				"trauma", "traumata"
			}, {
				"edema", "edemata"
			}, {
				"miasma", "miasmata"
			}, {
				"abscissa", "abscissae"
			}, {
				"formula", "formulae"
			}, {
				"medusa", "medusae"
			}, {
				"amoeba", "amoebae"
			}, {
				"hydra", "hydrae"
			}, {
				"nebula", "nebulae"
			}, {
				"antenna", "antennae"
			}, {
				"hyperbola", "hyperbolae"
			}, {
				"nova", "novae"
			}, {
				"aurora", "aurorae"
			}, {
				"lacuna", "lacunae"
			}, {
				"parabola", "parabolae"
			}, {
				"apex", "apices"
			}, {
				"latex", "latices"
			}, {
				"vertex", "vertices"
			}, {
				"cortex", "cortices"
			}, {
				"pontifex", "pontifices"
			}, {
				"vortex", "vortices"
			}, {
				"index", "indices"
			}, {
				"simplex", "simplices"
			}, {
				"iris", "irides"
			}, {
				"----oris", "----orides"
			}, {
				"alto", "alti"
			}, {
				"contralto", "contralti"
			}, {
				"soprano", "soprani"
			}, {
				"b----o", "b----i"
			}, {
				"crescendo", "crescendi"
			}, {
				"tempo", "tempi"
			}, {
				"canto", "canti"
			}, {
				"solo", "soli"
			}, {
				"aquarium", "aquaria"
			}, {
				"interregnum", "interregna"
			}, {
				"quantum", "quanta"
			}, {
				"compendium", "compendia"
			}, {
				"lustrum", "lustra"
			}, {
				"rostrum", "rostra"
			}, {
				"consortium", "consortia"
			}, {
				"maximum", "maxima"
			}, {
				"spectrum", "spectra"
			}, {
				"cranium", "crania"
			}, {
				"medium", "media"
			}, {
				"speculum", "specula"
			}, {
				"curriculum", "curricula"
			}, {
				"memorandum", "memoranda"
			}, {
				"stadium", "stadia"
			}, {
				"dictum", "dicta"
			}, {
				"millenium", "millenia"
			}, {
				"t----zium", "t----zia"
			}, {
				"emporium", "emporia"
			}, {
				"minimum", "minima"
			}, {
				"ultimatum", "ultimata"
			}, {
				"enconium", "enconia"
			}, {
				"momentum", "momenta"
			}, {
				"vacuum", "vacua"
			}, {
				"gymnasium", "gymnasia"
			}, {
				"optimum", "optima"
			}, {
				"velum", "vela"
			}, {
				"honorarium", "honoraria"
			}, {
				"phylum", "phyla"
			}, {
				"focus", "foci"
			}, {
				"nimbus", "nimbi"
			}, {
				"succubus", "succubi"
			}, {
				"fungus", "fungi"
			}, {
				"nucleolus", "nucleoli"
			}, {
				"torus", "tori"
			}, {
				"genius", "genii"
			}, {
				"radius", "radii"
			}, {
				"umbilicus", "umbilici"
			}, {
				"incubus", "incubi"
			}, {
				"stylus", "styli"
			}, {
				"uterus", "uteri"
			}, {
				"stimulus", "stimuli"
			}, {
				"apparatus", "apparatus"
			}, {
				"impetus", "impetus"
			}, {
				"prospectus", "prospectus"
			}, {
				"cantus", "cantus"
			}, {
				"nexus", "nexus"
			}, {
				"sinus", "sinus"
			}, {
				"coitus", "coitus"
			}, {
				"plexus", "plexus"
			}, {
				"status", "status"
			}, {
				"hiatus", "hiatus"
			}, {
				"afreet", "afreeti"
			}, {
				"afrit", "afriti"
			}, {
				"efreet", "efreeti"
			}, {
				"cherub", "cherubim"
			}, {
				"goy", "goyim"
			}, {
				"seraph", "seraphim"
			}, {
				"alumnus", "alumni"
			}
		};

		private readonly IDictionary<String, String> _irregularPluralsDictionary = new Dictionary<String, String> {
			{
				"brother", "brothers"
			}, {
				"child", "children"
			}, {
				"cow", "cows"
			}, {
				"ephemeris", "ephemerides"
			}, {
				"genie", "genies"
			}, {
				"money", "moneys"
			}, {
				"mongoose", "mongooses"
			}, {
				"mythos", "mythoi"
			}, {
				"octopus", "octopuses"
			}, {
				"ox", "oxen"
			}, {
				"soliloquy", "soliloquies"
			}, {
				"trilby", "trilbys"
			}, {
				"crisis", "crises"
			}, {
				"synopsis", "synopses"
			}, {
				"rose", "roses"
			}, {
				"gas", "gases"
			}, {
				"bus", "buses"
			}, {
				"axis", "axes"
			}, {
				"memo", "memos"
			}, {
				"casino", "casinos"
			}, {
				"silo", "silos"
			}, {
				"stereo", "stereos"
			}, {
				"studio", "studios"
			}, {
				"lens", "lenses"
			}, {
				"alias", "aliases"
			}, {
				"pie", "pies"
			}, {
				"corpus", "corpora"
			}, {
				"viscus", "viscera"
			}, {
				"hippopotamus", "hippopotami"
			}, {
				"trace", "traces"
			}, {
				"person", "people"
			}, {
				"chili", "chilies"
			}, {
				"analysis", "analyses"
			}, {
				"basis", "bases"
			}, {
				"neurosis", "neuroses"
			}, {
				"oasis", "oases"
			}, {
				"synthesis", "syntheses"
			}, {
				"thesis", "theses"
			}, {
				"change", "changes"
			}, {
				"lie", "lies"
			}, {
				"calorie", "calories"
			}, {
				"freebie", "freebies"
			}, {
				"case", "cases"
			}, {
				"house", "houses"
			}, {
				"valve", "valves"
			}, {
				"cloth", "clothes"
			}, {
				"tie", "ties"
			}, {
				"movie", "movies"
			}, {
				"bonus", "bonuses"
			}, {
				"specimen", "specimens"
			}
		};

		private readonly IDictionary<String, String> _irregularVerbList = new Dictionary<String, String> {
			{
				"am", "are"
			}, {
				"are", "are"
			}, {
				"is", "are"
			}, {
				"was", "were"
			}, {
				"were", "were"
			}, {
				"has", "have"
			}, {
				"have", "have"
			}
		};

		/// <summary>
		///     this list contains all the plural words that being treated as singluar form, for example, "they" -> "they"
		/// </summary>
		private readonly List<String> _knownConflictingPluralList = new() {
			"they",
			"them",
			"their",
			"have",
			"were",
			"yourself",
			"are"
		};

		private readonly IDictionary<String, String> _oSuffixDictionary = new Dictionary<String, String> {
			{
				"albino", "albinos"
			}, {
				"generalissimo", "generalissimos"
			}, {
				"manifesto", "manifestos"
			}, {
				"archipelago", "archipelagos"
			}, {
				"ghetto", "ghettos"
			}, {
				"medico", "medicos"
			}, {
				"armadillo", "armadillos"
			}, {
				"guano", "guanos"
			}, {
				"octavo", "octavos"
			}, {
				"commando", "commandos"
			}, {
				"inferno", "infernos"
			}, {
				"photo", "photos"
			}, {
				"ditto", "dittos"
			}, {
				"jumbo", "jumbos"
			}, {
				"pro", "pros"
			}, {
				"dynamo", "dynamos"
			}, {
				"lingo", "lingos"
			}, {
				"quarto", "quartos"
			}, {
				"embryo", "embryos"
			}, {
				"lumbago", "lumbagos"
			}, {
				"rhino", "rhinos"
			}, {
				"fiasco", "fiascos"
			}, {
				"magneto", "magnetos"
			}, {
				"stylo", "stylos"
			}
		};

		private readonly List<String> _pronounList = new() {
			"I",
			"we",
			"you",
			"he",
			"she",
			"they",
			"it",
			"me",
			"us",
			"him",
			"her",
			"them",
			"myself",
			"ourselves",
			"yourself",
			"himself",
			"herself",
			"itself",
			"oneself",
			"oneselves",
			"my",
			"our",
			"your",
			"his",
			"their",
			"its",
			"mine",
			"yours",
			"hers",
			"theirs",
			"this",
			"that",
			"these",
			"those",
			"all",
			"another",
			"any",
			"anybody",
			"anyone",
			"anything",
			"both",
			"each",
			"other",
			"either",
			"everyone",
			"everybody",
			"everything",
			"most",
			"much",
			"nothing",
			"nobody",
			"none",
			"one",
			"others",
			"some",
			"somebody",
			"someone",
			"something",
			"what",
			"whatever",
			"which",
			"whichever",
			"who",
			"whoever",
			"whom",
			"whomever",
			"whose"
		};

		private readonly String[] _uninflectiveSuffixList = {
			"fish", "ois", "sheep", "deer", "pos", "itis", "ism"
		};

		private readonly String[] _uninflectiveWordList = {
			"bison", "flounder", "pliers", "bream", "gallows", "proceedings", "breeches", "graffiti", "rabies", "britches",
			"headquarters", "salmon", "carp", "----", "scissors", "ch----is", "high-jinks", "sea-bass", "clippers", "homework",
			"series", "cod", "innings", "shears", "contretemps", "jackanapes", "species", "corps", "mackerel", "swine",
			"debris", "measles", "trout", "diabetes", "mews", "tuna", "djinn", "mumps", "whiting", "eland",
			"news", "wildebeest", "elk", "pincers", "police", "hair", "ice", "chaos", "milk", "cotton",
			"pneumonoultramicroscopicsilicovolcanoconiosis", "information", "aircraft", "scabies", "traffic", "corn", "millet", "rice", "hay", "----",
			"tobacco", "cabbage", "okra", "broccoli", "asparagus", "lettuce", "beef", "pork", "venison", "mutton",
			"cattle", "offspring", "molasses", "shambles", "shingles"
		};

		private readonly IDictionary<String, String> _wordsEndingWithInxAnxYnxDictionary = new Dictionary<String, String> {
			{
				"sphinx", "sphinxes"
			}, {
				"larynx", "larynges"
			}, {
				"lynx", "lynxes"
			}, {
				"pharynx", "pharynxes"
			}, {
				"phalanx", "phalanxes"
			}
		};

		/// <summary>
		///     this list contains the words ending with "se" and we special case these words since we need to add a rule for "ses"
		///     singularize to "s"
		/// </summary>
		private readonly IDictionary<String, String> _wordsEndingWithSeDictionary = new Dictionary<String, String> {
			{
				"house", "houses"
			}, {
				"case", "cases"
			}, {
				"enterprise", "enterprises"
			}, {
				"purchase", "purchases"
			}, {
				"surprise", "surprises"
			}, {
				"release", "releases"
			}, {
				"disease", "diseases"
			}, {
				"promise", "promises"
			}, {
				"refuse", "refuses"
			}, {
				"whose", "whoses"
			}, {
				"phase", "phases"
			}, {
				"noise", "noises"
			}, {
				"nurse", "nurses"
			}, {
				"rose", "roses"
			}, {
				"franchise", "franchises"
			}, {
				"supervise", "supervises"
			}, {
				"farmhouse", "farmhouses"
			}, {
				"suitcase", "suitcases"
			}, {
				"recourse", "recourses"
			}, {
				"impulse", "impulses"
			}, {
				"license", "licenses"
			}, {
				"diocese", "dioceses"
			}, {
				"excise", "excises"
			}, {
				"demise", "demises"
			}, {
				"blouse", "blouses"
			}, {
				"bruise", "bruises"
			}, {
				"misuse", "misuses"
			}, {
				"curse", "curses"
			}, {
				"prose", "proses"
			}, {
				"purse", "purses"
			}, {
				"goose", "gooses"
			}, {
				"tease", "teases"
			}, {
				"poise", "poises"
			}, {
				"vase", "vases"
			}, {
				"fuse", "fuses"
			}, {
				"muse", "muses"
			}, {
				"slaughterhouse", "slaughterhouses"
			}, {
				"clearinghouse", "clearinghouses"
			}, {
				"endonuclease", "endonucleases"
			}, {
				"steeplechase", "steeplechases"
			}, {
				"metamorphose", "metamorphoses"
			}, {
				"----", "----s"
			}, {
				"commonsense", "commonsenses"
			}, {
				"intersperse", "intersperses"
			}, {
				"merchandise", "merchandises"
			}, {
				"phosphatase", "phosphatases"
			}, {
				"summerhouse", "summerhouses"
			}, {
				"watercourse", "watercourses"
			}, {
				"catchphrase", "catchphrases"
			}, {
				"compromise", "compromises"
			}, {
				"greenhouse", "greenhouses"
			}, {
				"lighthouse", "lighthouses"
			}, {
				"paraphrase", "paraphrases"
			}, {
				"mayonnaise", "mayonnaises"
			}, {
				"----course", "----courses"
			}, {
				"apocalypse", "apocalypses"
			}, {
				"courthouse", "courthouses"
			}, {
				"powerhouse", "powerhouses"
			}, {
				"storehouse", "storehouses"
			}, {
				"glasshouse", "glasshouses"
			}, {
				"hypotenuse", "hypotenuses"
			}, {
				"peroxidase", "peroxidases"
			}, {
				"pillowcase", "pillowcases"
			}, {
				"roundhouse", "roundhouses"
			}, {
				"streetwise", "streetwises"
			}, {
				"expertise", "expertises"
			}, {
				"discourse", "discourses"
			}, {
				"warehouse", "warehouses"
			}, {
				"staircase", "staircases"
			}, {
				"workhouse", "workhouses"
			}, {
				"briefcase", "briefcases"
			}, {
				"clubhouse", "clubhouses"
			}, {
				"clockwise", "clockwises"
			}, {
				"concourse", "concourses"
			}, {
				"playhouse", "playhouses"
			}, {
				"turquoise", "turquoises"
			}, {
				"boathouse", "boathouses"
			}, {
				"cellulose", "celluloses"
			}, {
				"epitomise", "epitomises"
			}, {
				"gatehouse", "gatehouses"
			}, {
				"grandiose", "grandioses"
			}, {
				"menopause", "menopauses"
			}, {
				"penthouse", "penthouses"
			}, {
				"----horse", "----horses"
			}, {
				"transpose", "transposes"
			}, {
				"almshouse", "almshouses"
			}, {
				"customise", "customises"
			}, {
				"footloose", "footlooses"
			}, {
				"galvanise", "galvanises"
			}, {
				"princesse", "princesses"
			}, {
				"universe", "universes"
			}, {
				"workhorse", "workhorses"
			}
		};

		private readonly IDictionary<String, String> _wordsEndingWithSisDictionary = new Dictionary<String, String> {
			{
				"analysis", "analyses"
			}, {
				"crisis", "crises"
			}, {
				"basis", "bases"
			}, {
				"atherosclerosis", "atheroscleroses"
			}, {
				"electrophoresis", "electrophoreses"
			}, {
				"psychoanalysis", "psychoanalyses"
			}, {
				"photosynthesis", "photosyntheses"
			}, {
				"amniocentesis", "amniocenteses"
			}, {
				"metamorphosis", "metamorphoses"
			}, {
				"toxoplasmosis", "toxoplasmoses"
			}, {
				"endometriosis", "endometrioses"
			}, {
				"tuberculosis", "tuberculoses"
			}, {
				"pathogenesis", "pathogeneses"
			}, {
				"osteoporosis", "osteoporoses"
			}, {
				"parenthesis", "parentheses"
			}, {
				"anastomosis", "anastomoses"
			}, {
				"peristalsis", "peristalses"
			}, {
				"hypothesis", "hypotheses"
			}, {
				"antithesis", "antitheses"
			}, {
				"apotheosis", "apotheoses"
			}, {
				"thrombosis", "thromboses"
			}, {
				"diagnosis", "diagnoses"
			}, {
				"synthesis", "syntheses"
			}, {
				"paralysis", "paralyses"
			}, {
				"prognosis", "prognoses"
			}, {
				"cirrhosis", "cirrhoses"
			}, {
				"sclerosis", "scleroses"
			}, {
				"psychosis", "psychoses"
			}, {
				"apoptosis", "apoptoses"
			}, {
				"symbiosis", "symbioses"
			}
		};

		private readonly IDictionary<String, String> _wordsEndingWithSusDictionary = new Dictionary<String, String> {
			{
				"consensus", "consensuses"
			}, {
				"census", "censuses"
			}
		};

		public EnglishPluralizationService( CultureInfo cultureInfo ) : base( cultureInfo ) {
			this.Culture = cultureInfo;

			this._irregularPluralsPluralizationService = new StringBidirectionalDictionary( this._irregularPluralsDictionary );
			this._assimilatedClassicalInflectionPluralizationService = new StringBidirectionalDictionary( this._assimilatedClassicalInflectionDictionary );
			this._oSuffixPluralizationService = new StringBidirectionalDictionary( this._oSuffixDictionary );
			this._classicalInflectionPluralizationService = new StringBidirectionalDictionary( this._classicalInflectionDictionary );
			this._wordsEndingWithSePluralizationService = new StringBidirectionalDictionary( this._wordsEndingWithSeDictionary );
			this._wordsEndingWithSisPluralizationService = new StringBidirectionalDictionary( this._wordsEndingWithSisDictionary );
			this._wordsEndingWithSusPluralizationService = new StringBidirectionalDictionary( this._wordsEndingWithSusDictionary );
			this._wordsEndingWithInxAnxYnxPluralizationService = new StringBidirectionalDictionary( this._wordsEndingWithInxAnxYnxDictionary );

			// verb
			this._irregularVerbPluralizationService = new StringBidirectionalDictionary( this._irregularVerbList );

			this._knownSingluarWords = new List<String>( this._irregularPluralsDictionary.Keys.Concat( this._assimilatedClassicalInflectionDictionary.Keys )
			                                                 .Concat( this._oSuffixDictionary.Keys ).Concat( this._classicalInflectionDictionary.Keys )
			                                                 .Concat( this._irregularVerbList.Keys ).Concat( this._irregularPluralsDictionary.Keys )
			                                                 .Concat( this._wordsEndingWithSeDictionary.Keys ).Concat( this._wordsEndingWithSisDictionary.Keys )
			                                                 .Concat( this._wordsEndingWithSusDictionary.Keys ).Concat( this._wordsEndingWithInxAnxYnxDictionary.Keys )
			                                                 .Concat( this._uninflectiveWordList )
			                                                 .Except( this._knownConflictingPluralList ) ); // see the _knowConflictingPluralList comment above

			this._knownPluralWords = new List<String>( this._irregularPluralsDictionary.Values.Concat( this._assimilatedClassicalInflectionDictionary.Values )
			                                               .Concat( this._oSuffixDictionary.Values ).Concat( this._classicalInflectionDictionary.Values )
			                                               .Concat( this._irregularVerbList.Values ).Concat( this._irregularPluralsDictionary.Values )
			                                               .Concat( this._wordsEndingWithSeDictionary.Values ).Concat( this._wordsEndingWithSisDictionary.Values )
			                                               .Concat( this._wordsEndingWithSusDictionary.Values ).Concat( this._wordsEndingWithInxAnxYnxDictionary.Values )
			                                               .Concat( this._uninflectiveWordList ) );
		}

		[JetBrains.Annotations.NotNull]
		public static PluralizationService Default { get; } = new EnglishPluralizationService( new CultureInfo( "en" ) );

		[JetBrains.Annotations.NotNull]
		private BidirectionalDictionary<String, String> _userDictionary { get; } = new();

		[JetBrains.Annotations.NotNull]
		private StringBidirectionalDictionary _irregularPluralsPluralizationService { get; }

		[JetBrains.Annotations.NotNull]
		private StringBidirectionalDictionary _assimilatedClassicalInflectionPluralizationService { get; }

		[JetBrains.Annotations.NotNull]
		private StringBidirectionalDictionary _oSuffixPluralizationService { get; }

		[JetBrains.Annotations.NotNull]
		private StringBidirectionalDictionary _classicalInflectionPluralizationService { get; }

		[JetBrains.Annotations.NotNull]
		private StringBidirectionalDictionary _irregularVerbPluralizationService { get; }

		[JetBrains.Annotations.NotNull]
		private StringBidirectionalDictionary _wordsEndingWithSePluralizationService { get; }

		[JetBrains.Annotations.NotNull]
		private StringBidirectionalDictionary _wordsEndingWithSisPluralizationService { get; }

		[JetBrains.Annotations.NotNull]
		private StringBidirectionalDictionary _wordsEndingWithSusPluralizationService { get; }

		[JetBrains.Annotations.NotNull]
		private StringBidirectionalDictionary _wordsEndingWithInxAnxYnxPluralizationService { get; }

		[JetBrains.Annotations.NotNull]
		private IList<String> _knownSingluarWords { get; }

		[JetBrains.Annotations.NotNull]
		private IList<String> _knownPluralWords { get; }

		[JetBrains.Annotations.NotNull]
		private Lazy<Regex> alpaLazy { get; } = new( () => new Regex( "[^a-zA-Z\\s]", RegexOptions.Compiled ) );

		public override Boolean IsPlural( String word ) {

			if ( this._userDictionary.ExistsInPlural( word ) ) {
				return true;
			}

			if ( this._userDictionary.ExistsInSingle( word ) ) {
				return false;
			}

			if ( this.IsUninflective( word ) || this._knownPluralWords.Contains( word.ToLower( this.Culture ) ) ) {
				return true;
			}

			return !this.Singularize(word).Equals(word, StringComparison.Ordinal);

		}

		public override Boolean IsSingular( String word ) {

			if ( this._userDictionary.ExistsInSingle( word ) ) {
				return true;
			}

			if ( this._userDictionary.ExistsInPlural( word ) ) {
				return false;
			}

			if ( this.IsUninflective( word ) || this._knownSingluarWords.Contains( word.ToLower( this.Culture ) ) ) {
				return true;
			}

			return !this.IsNoOpWord( word ) && this.Singularize(word).Equals(word, StringComparison.Ordinal);
		}

		public override String Pluralize( String word ) => Capitalize( word, this.InternalPluralize );

		private String InternalPluralize( String word ) {

			var plural = this._userDictionary.GetPlural( word );
			if (plural != null) {
				return plural;
			}

			if ( this.IsNoOpWord( word ) ) {
				return word;
			}

			var suffixWord = GetSuffixWord( word, out var prefixWord );

			// by me -> by me
			if ( this.IsNoOpWord( suffixWord ) ) {
				return prefixWord + suffixWord;
			}

			// handle the word that do not inflect in the plural form
			if ( this.IsUninflective( suffixWord ) ) {
				return prefixWord + suffixWord;
			}

			// if word is one of the known plural forms, then just return
			if ( this._knownPluralWords.Contains( suffixWord.ToLowerInvariant() ) || this.IsPlural( suffixWord ) ) {
				return prefixWord + suffixWord;
			}

			// handle irregular plurals, e.g. "ox" -> "oxen"
			if ( this._irregularPluralsPluralizationService.ExistsInSingle( suffixWord ) ) {
				return prefixWord + this._irregularPluralsPluralizationService.GetPlural( suffixWord );
			}

			if ( PluralizationServiceUtil.TryInflectOnSuffixInWord( suffixWord, new List<String> {
				"man"
			}, s => s.Remove( s.Length - 2, 2 ) + "en", this.Culture, out var newSuffixWord ) ) {
				return prefixWord + newSuffixWord;
			}

			// handle irregular inflections for common suffixes, e.g. "mouse" -> "mice"
			if ( PluralizationServiceUtil.TryInflectOnSuffixInWord( suffixWord, new List<String> {
				"louse", "mouse"
			}, s => s.Remove( s.Length - 4, 4 ) + "ice", this.Culture, out newSuffixWord ) ) {
				return prefixWord + newSuffixWord;
			}

			if ( PluralizationServiceUtil.TryInflectOnSuffixInWord( suffixWord, new List<String> {
				"tooth"
			}, s => s.Remove( s.Length - 4, 4 ) + "eeth", this.Culture, out newSuffixWord ) ) {
				return prefixWord + newSuffixWord;
			}

			if ( PluralizationServiceUtil.TryInflectOnSuffixInWord( suffixWord, new List<String> {
				"goose"
			}, s => s.Remove( s.Length - 4, 4 ) + "eese", this.Culture, out newSuffixWord ) ) {
				return prefixWord + newSuffixWord;
			}

			if ( PluralizationServiceUtil.TryInflectOnSuffixInWord( suffixWord, new List<String> {
				"foot"
			}, s => s.Remove( s.Length - 3, 3 ) + "eet", this.Culture, out newSuffixWord ) ) {
				return prefixWord + newSuffixWord;
			}

			if ( PluralizationServiceUtil.TryInflectOnSuffixInWord( suffixWord, new List<String> {
				"zoon"
			}, s => s.Remove( s.Length - 3, 3 ) + "oa", this.Culture, out newSuffixWord ) ) {
				return prefixWord + newSuffixWord;
			}

			if ( PluralizationServiceUtil.TryInflectOnSuffixInWord( suffixWord, new List<String> {
				"cis", "sis", "xis"
			}, s => s.Remove( s.Length - 2, 2 ) + "es", this.Culture, out newSuffixWord ) ) {
				return prefixWord + newSuffixWord;
			}

			// handle assimilated classical inflections, e.g. vertebra -> vertebrae
			if ( this._assimilatedClassicalInflectionPluralizationService.ExistsInSingle( suffixWord ) ) {
				return prefixWord + this._assimilatedClassicalInflectionPluralizationService.GetPlural( suffixWord );
			}

			// Handle the classical variants of modern inflections
			// 
			if ( this._classicalInflectionPluralizationService.ExistsInSingle( suffixWord ) ) {
				return prefixWord + this._classicalInflectionPluralizationService.GetPlural( suffixWord );
			}

			if ( PluralizationServiceUtil.TryInflectOnSuffixInWord( suffixWord, new List<String> {
				"trix"
			}, s => s.Remove( s.Length - 1, 1 ) + "ces", this.Culture, out newSuffixWord ) ) {
				return prefixWord + newSuffixWord;
			}

			if ( PluralizationServiceUtil.TryInflectOnSuffixInWord( suffixWord, new List<String> {
				"eau", "ieu"
			}, s => s + "x", this.Culture, out newSuffixWord ) ) {
				return prefixWord + newSuffixWord;
			}

			if ( this._wordsEndingWithInxAnxYnxPluralizationService.ExistsInSingle( suffixWord ) ) {
				return prefixWord + this._wordsEndingWithInxAnxYnxPluralizationService.GetPlural( suffixWord );
			}

			// [cs]h and ss that take es as plural form
			if ( PluralizationServiceUtil.TryInflectOnSuffixInWord( suffixWord, new List<String> {
				"ch", "sh", "ss"
			}, s => s + "es", this.Culture, out newSuffixWord ) ) {
				return prefixWord + newSuffixWord;
			}

			// f, fe that take ves as plural form
			if ( PluralizationServiceUtil.TryInflectOnSuffixInWord( suffixWord, new List<String> {
				"alf",
				"elf",
				"olf",
				"eaf",
				"arf"
			}, s => s.EndsWith( "deaf", true, this.Culture ) ? s : s.Remove( s.Length - 1, 1 ) + "ves", this.Culture, out newSuffixWord ) ) {
				return prefixWord + newSuffixWord;
			}

			if ( PluralizationServiceUtil.TryInflectOnSuffixInWord( suffixWord, new List<String> {
				"nife", "life", "wife"
			}, s => s.Remove( s.Length - 2, 2 ) + "ves", this.Culture, out newSuffixWord ) ) {
				return prefixWord + newSuffixWord;
			}

			// y takes ys as plural form if preceded by a vowel, but ies if preceded by a consonant, e.g. stays, skies
			if ( PluralizationServiceUtil.TryInflectOnSuffixInWord( suffixWord, new List<String> {
				"ay",
				"ey",
				"iy",
				"oy",
				"uy"
			}, s => s + "s", this.Culture, out newSuffixWord ) ) {
				return prefixWord + newSuffixWord;
			}

			if ( suffixWord.EndsWith( "y", true, this.Culture ) ) {
				return prefixWord + suffixWord.Remove( suffixWord.Length - 1, 1 ) + "ies";
			}

			// handle some of the words o -> os, and [vowel]o -> os, and the rest are o->oes
			if ( this._oSuffixPluralizationService.ExistsInSingle( suffixWord ) ) {
				return prefixWord + this._oSuffixPluralizationService.GetPlural( suffixWord );
			}

			if ( PluralizationServiceUtil.TryInflectOnSuffixInWord( suffixWord, new List<String> {
				"ao",
				"eo",
				"io",
				"oo",
				"uo"
			}, s => s + "s", this.Culture, out newSuffixWord ) ) {
				return prefixWord + newSuffixWord;
			}

			if ( suffixWord.EndsWith( "o", true, this.Culture ) || suffixWord.EndsWith( "s", true, this.Culture ) ) {
				return prefixWord + suffixWord + "es";
			}

			if ( suffixWord.EndsWith( "x", true, this.Culture ) ) {
				return prefixWord + suffixWord + "es";
			}

			// cats, bags, hats, speakers
			return prefixWord + suffixWord + "s";
		}

		public override String Singularize( String word ) =>

			//EDesignUtil.CheckArgumentNull<string>(word, "word");
			Capitalize( word, this.InternalSingularize );

		[SuppressMessage( "Microsoft.Globalization", "CA1308:NormalizeStringsToUppercase" )]
		private String InternalSingularize( String word ) {
			// words that we know of

			var single = this._userDictionary.GetSingle( word );

			if ( single != null ) {
				return single;
			}

			if ( this.IsNoOpWord( word ) ) {
				return word;
			}

			var suffixWord = GetSuffixWord( word, out var prefixWord );

			if ( this.IsNoOpWord( suffixWord ) ) {
				return prefixWord + suffixWord;
			}

			// handle the word that is the same as the plural form
			if ( this.IsUninflective( suffixWord ) ) {
				return prefixWord + suffixWord;
			}

			// if word is one of the known singular words, then just return

			if ( this._knownSingluarWords.Contains( suffixWord.ToLowerInvariant() ) ) {
				return prefixWord + suffixWord;
			}

			// handle simple irregular verbs, e.g. was -> were
			if ( this._irregularVerbPluralizationService.ExistsInPlural( suffixWord ) ) {
				return prefixWord + this._irregularVerbPluralizationService.GetSingle( suffixWord );
			}

			// handle irregular plurals, e.g. "ox" -> "oxen"
			if ( this._irregularPluralsPluralizationService.ExistsInPlural( suffixWord ) ) {
				return prefixWord + this._irregularPluralsPluralizationService.GetSingle( suffixWord );
			}

			// handle singluarization for words ending with sis and pluralized to ses, 
			// e.g. "ses" -> "sis"
			if ( this._wordsEndingWithSisPluralizationService.ExistsInPlural( suffixWord ) ) {
				return prefixWord + this._wordsEndingWithSisPluralizationService.GetSingle( suffixWord );
			}

			// handle words ending with se, e.g. "ses" -> "se"
			if ( this._wordsEndingWithSePluralizationService.ExistsInPlural( suffixWord ) ) {
				return prefixWord + this._wordsEndingWithSePluralizationService.GetSingle( suffixWord );
			}

			// handle words ending with sus, e.g. "suses" -> "sus"
			if ( this._wordsEndingWithSusPluralizationService.ExistsInPlural( suffixWord ) ) {
				return prefixWord + this._wordsEndingWithSusPluralizationService.GetSingle( suffixWord );
			}

			if ( PluralizationServiceUtil.TryInflectOnSuffixInWord( suffixWord, new List<String> {
				"men"
			}, s => s.Remove( s.Length - 2, 2 ) + "an", this.Culture, out var newSuffixWord ) ) {
				return prefixWord + newSuffixWord;
			}

			// handle irregular inflections for common suffixes, e.g. "mouse" -> "mice"
			if ( PluralizationServiceUtil.TryInflectOnSuffixInWord( suffixWord, new List<String> {
				"lice", "mice"
			}, s => s.Remove( s.Length - 3, 3 ) + "ouse", this.Culture, out newSuffixWord ) ) {
				return prefixWord + newSuffixWord;
			}

			if ( PluralizationServiceUtil.TryInflectOnSuffixInWord( suffixWord, new List<String> {
				"teeth"
			}, s => s.Remove( s.Length - 4, 4 ) + "ooth", this.Culture, out newSuffixWord ) ) {
				return prefixWord + newSuffixWord;
			}

			if ( PluralizationServiceUtil.TryInflectOnSuffixInWord( suffixWord, new List<String> {
				"geese"
			}, s => s.Remove( s.Length - 4, 4 ) + "oose", this.Culture, out newSuffixWord ) ) {
				return prefixWord + newSuffixWord;
			}

			if ( PluralizationServiceUtil.TryInflectOnSuffixInWord( suffixWord, new List<String> {
				"feet"
			}, s => s.Remove( s.Length - 3, 3 ) + "oot", this.Culture, out newSuffixWord ) ) {
				return prefixWord + newSuffixWord;
			}

			if ( PluralizationServiceUtil.TryInflectOnSuffixInWord( suffixWord, new List<String> {
				"zoa"
			}, s => s.Remove( s.Length - 2, 2 ) + "oon", this.Culture, out newSuffixWord ) ) {
				return prefixWord + newSuffixWord;
			}

			// [cs]h and ss that take es as plural form, this is being moved up since the sses will be override by the ses
			if ( PluralizationServiceUtil.TryInflectOnSuffixInWord( suffixWord, new List<String> {
				"ches", "shes", "sses"
			}, s => s.Remove( s.Length - 2, 2 ), this.Culture, out newSuffixWord ) ) {
				return prefixWord + newSuffixWord;
			}

			// handle assimilated classical inflections, e.g. vertebra -> vertebrae
			if ( this._assimilatedClassicalInflectionPluralizationService.ExistsInPlural( suffixWord ) ) {
				return prefixWord + this._assimilatedClassicalInflectionPluralizationService.GetSingle( suffixWord );
			}

			// Handle the classical variants of modern inflections
			// 
			if ( this._classicalInflectionPluralizationService.ExistsInPlural( suffixWord ) ) {
				return prefixWord + this._classicalInflectionPluralizationService.GetSingle( suffixWord );
			}

			if ( PluralizationServiceUtil.TryInflectOnSuffixInWord( suffixWord, new List<String> {
				"trices"
			}, s => s.Remove( s.Length - 3, 3 ) + "x", this.Culture, out newSuffixWord ) ) {
				return prefixWord + newSuffixWord;
			}

			if ( PluralizationServiceUtil.TryInflectOnSuffixInWord( suffixWord, new List<String> {
				"eaux", "ieux"
			}, s => s.Remove( s.Length - 1, 1 ), this.Culture, out newSuffixWord ) ) {
				return prefixWord + newSuffixWord;
			}

			if ( this._wordsEndingWithInxAnxYnxPluralizationService.ExistsInPlural( suffixWord ) ) {
				return prefixWord + this._wordsEndingWithInxAnxYnxPluralizationService.GetSingle( suffixWord );
			}

			// f, fe that take ves as plural form
			if ( PluralizationServiceUtil.TryInflectOnSuffixInWord( suffixWord, new List<String> {
				"alves",
				"elves",
				"olves",
				"eaves",
				"arves"
			}, s => s.Remove( s.Length - 3, 3 ) + "f", this.Culture, out newSuffixWord ) ) {
				return prefixWord + newSuffixWord;
			}

			if ( PluralizationServiceUtil.TryInflectOnSuffixInWord( suffixWord, new List<String> {
				"nives", "lives", "wives"
			}, s => s.Remove( s.Length - 3, 3 ) + "fe", this.Culture, out newSuffixWord ) ) {
				return prefixWord + newSuffixWord;
			}

			// y takes ys as plural form if preceded by a vowel, but ies if preceded by a consonant, e.g. stays, skies
			if ( PluralizationServiceUtil.TryInflectOnSuffixInWord( suffixWord, new List<String> {
				"ays",
				"eys",
				"iys",
				"oys",
				"uys"
			}, s => s.Remove( s.Length - 1, 1 ), this.Culture, out newSuffixWord ) ) {
				return prefixWord + newSuffixWord;
			}

			// 

			if ( suffixWord.EndsWith( "ies", true, this.Culture ) ) {
				return prefixWord + suffixWord.Remove( suffixWord.Length - 3, 3 ) + "y";
			}

			// handle some of the words o -> os, and [vowel]o -> os, and the rest are o->oes
			if ( this._oSuffixPluralizationService.ExistsInPlural( suffixWord ) ) {
				return prefixWord + this._oSuffixPluralizationService.GetSingle( suffixWord );
			}

			if ( PluralizationServiceUtil.TryInflectOnSuffixInWord( suffixWord, new List<String> {
				"aos",
				"eos",
				"ios",
				"oos",
				"uos"
			}, s => suffixWord.Remove( suffixWord.Length - 1, 1 ), this.Culture, out newSuffixWord ) ) {
				return prefixWord + newSuffixWord;
			}

			// 

			if ( PluralizationServiceUtil.TryInflectOnSuffixInWord( suffixWord, new List<String> {
				"ces"
			}, s => s.Remove( s.Length - 1, 1 ), this.Culture, out newSuffixWord ) ) {
				return prefixWord + newSuffixWord;
			}

			if ( PluralizationServiceUtil.TryInflectOnSuffixInWord( suffixWord, new List<String> {
				"ces", "ses", "xes"
			}, s => s.Remove( s.Length - 2, 2 ), this.Culture, out newSuffixWord ) ) {
				return prefixWord + newSuffixWord;
			}

			if ( suffixWord.EndsWith( "oes", true, this.Culture ) ) {
				return prefixWord + suffixWord.Remove( suffixWord.Length - 2, 2 );
			}

			if ( suffixWord.EndsWith( "ss", true, this.Culture ) ) {
				return prefixWord + suffixWord;
			}

			if ( suffixWord.EndsWith( "s", true, this.Culture ) ) {
				return prefixWord + suffixWord.Remove( suffixWord.Length - 1, 1 );
			}

			// word is a singlar
			return prefixWord + suffixWord;
		}

		/// <summary>
		///     captalize the return word if the parameter is capitalized
		///     if word is "Table", then return "Tables"
		/// </summary>
		/// <param name="word"></param>
		/// <param name="action"></param>
		/// <returns></returns>
		public static String Capitalize( String word, Func<String, String> action ) {
			var result = action( word );

			if ( IsCapitalized( word ) ) {
				if ( !result.Any() ) {
					return result;
				}

				var sb = new StringBuilder( result.Length );

				sb.Append( Char.ToUpperInvariant( result[0] ) );
				sb.Append( result[ 1.. ] );

				return sb.ToString();
			}

			return result;
		}

		/// <summary>
		///     separate one combine word in to two parts, prefix word and the last word(suffix word)
		/// </summary>
		/// <param name="word"></param>
		/// <param name="prefixWord"></param>
		/// <returns></returns>
		private static String GetSuffixWord( String word, out String prefixWord ) {
			// use the last space to separate the words
			var lastSpaceIndex = word.LastIndexOf( ' ' );
			prefixWord = word.Substring( 0, lastSpaceIndex + 1 );

			return word[ ( lastSpaceIndex + 1 ).. ];

			// 
		}

		private static Boolean IsCapitalized( String word ) => !String.IsNullOrEmpty( word ) && Char.IsUpper( word, 0 );

		private Boolean IsAlphabets( String word ) {
			// return false when the word is "[\s]*" or leading or tailing with spaces
			// or contains non alphabetical characters
			if ( String.IsNullOrEmpty( word.Trim() ) || !word.Equals(word.Trim(), StringComparison.Ordinal) || this.alpaLazy.Value.IsMatch( word ) ) {
				return false;
			}

			return true;
		}

		private Boolean IsUninflective( String word ) =>
			PluralizationServiceUtil.DoesWordContainSuffix( word, this._uninflectiveSuffixList, this.Culture ) ||
			!word.ToLower(this.Culture).Equals(word, StringComparison.Ordinal) && word.EndsWith( "ese", false, this.Culture ) || this._uninflectiveWordList.Contains( word.ToLowerInvariant() );

		/// <summary>
		///     return true when the word is "[\s]*" or leading or tailing with spaces
		///     or contains non alphabetical characters
		/// </summary>
		/// <param name="word"></param>
		/// <returns></returns>
		private Boolean IsNoOpWord( String word ) => !this.IsAlphabets( word ) || word.Length <= 1 || this._pronounList.Contains( word.ToLowerInvariant() );

		/// <summary>
		///     This method allow you to add word to internal PluralizationService of English.
		///     If the singluar or the plural value was already added by this method, then an ArgumentException will be thrown.
		/// </summary>
		/// <param name="singular"></param>
		/// <param name="plural"></param>
		public void AddWord( String singular, String plural ) {

			if ( this._userDictionary.ExistsInPlural( plural ) ) {
				throw new ArgumentException( $"Duplicate entry in user dictionary {plural}" );
			}

			if ( this._userDictionary.ExistsInSingle( singular ) ) {
				throw new ArgumentException( $"Duplicate entry in user dictionary {singular}" );
			}

			this._userDictionary.AddValue( singular, plural );
		}

	}

}