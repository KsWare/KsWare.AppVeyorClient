using System.Text.RegularExpressions;

namespace KsWare.AppVeyorClient.Helpers {

	public static class YamlRegEx {

		private static readonly string indent = /*         lang=regex*/ @"(?<indent>\s*)";
		private static readonly string name = /*           lang=regex*/ @"(?<name>[a-z_]+)";
		private static readonly string entry = /*          lang=regex*/ $@"(?<entry>(-\s+({name}\s*:)?|{name}\s*:))"; 
		private static readonly string multiline = /*      lang=regex*/ @"(?<multiline>[>|])";
		private static readonly string indentIndicator = /*lang=regex*/ @"(?<indentIndicator>[0-9]*)";
		private static readonly string chomping = /*       lang=regex*/ @"(?<chomping>[+-]?)";
		private static readonly string flow = /*           lang=regex*/ @"(?<flow>[\x22|']?)";
		private static readonly string pattern = /*        lang=regex*/ $@"^{indent}(?<preContent>{entry}\s*({multiline}{chomping}{indentIndicator}|{flow})?)";
		private static readonly string fullPattern = /*    lang=regex*/ $@"{pattern}(?<content>.*?)[\x22']?\s*$";
		private static readonly Regex regex = new Regex(pattern, RegexOptions.Compiled | RegexOptions.IgnoreCase);
		private static readonly Regex regexFull = new Regex(fullPattern, RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Singleline);

		public static YamlRegExMatch Match(string input) => new YamlRegExMatch(regex.Match(input));

		public static YamlRegExMatch MatchFull(string input) => new YamlRegExMatch(regexFull.Match(input));
	}

	//DRAFT
	public static class AppVeyorYamlRegEx {

		private static readonly string scriptSections =
			"init|clone_script|install"
			+ "|before_build|before_package|after_build|before_test|after_test|before_deploy|after_deploy"
			+ "|build_script|test_script|deploy_script"
			+ "|"
			+ "|on_success|on_failure|on_finish|on_image_bake";

		private static readonly Regex scriptRegEx = new Regex($@"^(?<entry>{scriptSections}):\s*$", RegexOptions.Compiled);

		public static AppVeyorYamlRegExMatch ScriptMatch(string input) => new AppVeyorYamlRegExMatch(scriptRegEx.Match(input));
	}
}
