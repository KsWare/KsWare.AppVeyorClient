using System.Text.RegularExpressions;

namespace KsWare.AppVeyorClient.Helpers {

	public class YamlRegExMatch {

		private readonly Match _match;

		public YamlRegExMatch(Match match) {
			_match = match;
		}

		public bool Success => _match.Success;

		public string Indent => _match.Groups["indent"].Value;
		public string Entry => _match.Groups["entry"].Value;
		public string Name => _match.Groups["name"].Value;
		public string Multiline => _match.Groups["multiline"].Value;
		public string IndentIndicator => _match.Groups["indentIndicator"].Value;
		public string Chomping => _match.Groups["chomping"].Value;
		public string Quotes => _match.Groups["flow"].Value;
		public int IndentLength => Indent?.Length ?? 0;
		public string PreContent => _match.Groups["preContent"].Value;
		public string Content => _match.Groups["content"].Value;
	}

	//DRAFT
	public class AppVeyorYamlRegExMatch {

		private readonly Match _match;

		public AppVeyorYamlRegExMatch(Match match) {
			_match = match;
		}

		public bool Success => _match.Success;

		public string Indent => _match.Groups["indent"].Value;
		public string Entry => _match.Groups["entry"].Value;
		public string Multiline => _match.Groups["multiline"].Value;
		public string IndentIndicator => _match.Groups["indentIndicator"].Value;
		public string Chomping => _match.Groups["chomping"].Value;
		public string Quotes => _match.Groups["flow"].Value;
		public int IndentLength => Indent?.Length ?? 0;
		public string Suffix => $"{Entry}{Multiline}{Chomping}{IndentIndicator}{Quotes}";
		public string Content => _match.Groups["content"].Value;
	}
}
