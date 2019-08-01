using System.Text.RegularExpressions;

namespace KsWare.AppVeyorClient.Helpers
{
	public static class YamlRegEx
	{
		static readonly string indent = /*         lang=regex*/ @"(?<indent>\s*)";
		static readonly string entry = /*          lang=regex*/ @"(?<entry>-(\s*([a-z]+)\s*:)?\x20+)";
		static readonly string multiline = /*      lang=regex*/ @"(?<multiline>[>|])";
		static readonly string indentIndicator = /*lang=regex*/ @"(?<indentIndicator>[0-9]*)";
		static readonly string chomping = /*       lang=regex*/ @"(?<chomping>[+-]?)";
		static readonly string flow = /*           lang=regex*/ @"(?<flow>[\x22|']?)";
		static readonly string pattern = /*        lang=regex*/ $@"^{indent}(?<suffix>{entry}({multiline}{chomping}{indentIndicator}|{flow}))";
		static readonly string fullPattern = /*    lang=regex*/ $@"{pattern}(?<content>.*?)[\x22']?\s*$";
		static readonly Regex regex = new Regex(pattern, RegexOptions.Compiled | RegexOptions.IgnoreCase);
		static readonly Regex regexFull = new Regex(fullPattern, RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Singleline);

		public static YamlRegExMatch Match(string input)
			=> new YamlRegExMatch(regex.Match(input));

		public static YamlRegExMatch MatchFull(string input)
			=> new YamlRegExMatch(regexFull.Match(input));
		
	}
}