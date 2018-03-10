using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using KsWare.AppVeyorClient.UI;
using KsWare.AppVeyorClient.UI.PanelConfiguration;

namespace KsWare.AppVeyorClient.Helpers {

	public static class YamlHelper {

		static readonly Dictionary<int,Encoding> HexEncoding=new Dictionary<int, Encoding>() {
			{2, Encoding.GetEncoding("iso-8859-1")},
			{4, Encoding.Unicode},
			{8, Encoding.UTF32}
		};

		public static string GetSuffix(string s) => ExtractBlock(s).Suffix; 

		public static int GetIndent(string s) => ExtractBlock(s).Indent;

		public static string FormatBlock(string s, BlockFormat format,  int indent, string suffix) {
			var lines = s.Split(new[] {"\r\n", "\n"}, StringSplitOptions.None);
			var sb=new StringBuilder();
			var sp = new string(' ', indent);
			switch (format) {
				case BlockFormat.Literal: {
					sb.AppendLine($"{sp}{suffix} |");
					foreach (var line in lines) {
						sb.AppendLine($"{sp}    {line}");
					}
					break;
				}
				case BlockFormat.Folded: {
					sb.AppendLine($"{sp}{suffix} >-");
					foreach (var line in lines) {
						sb.AppendLine($"{sp}    {line}\r\n");
					}
					break;
				}
				default: {
					foreach (var line in lines) {
						sb.AppendLine($"{sp}{suffix}{line}");
					}
					break;
				}
			}
			return sb.ToString().TrimEnd();
		}

		public static YamlBlock ExtractBlock(string s) {
			// - ps: "...\"..."
			var m = Regex.Match(s, @"^(?<indent>\s*)(?<suffix>-\s*(ps|cmd|pwsh)\s*:\s)\s*""(?<content>.*?)""\s*$",
				RegexOptions.Compiled | RegexOptions.IgnoreCase);
			var b=new YamlBlock() {
				Indent = m.Groups["indent"].Value.Length,
				Suffix = m.Groups["suffix"].Value,
				Content = UnescapeDoubleQuotedString(m.Groups["content"].Value),
			};
			return b;
		}

		public static string UnescapeDoubleQuotedString(string s) {
			/*  YAML’s double-quoted style uses familiar C-style escape sequences. This enables ASCII encoding of non-printable or 8-
				bit (ISO 8859-1) characters such as “\x3B”. Non-printable 16-bit Unicode and 32-bit (ISO/IEC 10646) characters are
				supported with escape sequences such as “\u003B” and “\U0000003B”. */
			var sb=new StringBuilder();
			bool esc=false;
			for (int i = 0; i < s.Length; i++) {
				if (!esc && s[i] == '\\') {esc = true;continue;}
				if (!esc) {sb.Append(s[i]); continue;}
				esc = false;
				switch (s[i]) {
					case 'x': sb.Append(Hex(i+1,2)); i+=2; continue;	// \x00
					case 'u': sb.Append(Hex(i+1,4)); i+=2; continue;	// \u0000
					case 'U': sb.Append(Hex(i+1,8)); i+=2; continue;	// \U00000000
					case 'n': sb.Append('\n'); continue;
					case 'r': sb.Append('\r'); continue;
					case 'a': sb.Append('\a'); continue;
					case 'b': sb.Append('\b'); continue;
					case 'f': sb.Append('\f'); continue;
					case 't': sb.Append('\t'); continue;
					case 'v': sb.Append('\v'); continue;
					default : sb.Append(s[i]); continue;
				}
			}
			return sb.ToString();

			string Hex(int index,int numberChars) {
				byte[] bytes = new byte[numberChars / 2];
				for (int i = 0; i < numberChars; i += 2) bytes[i / 2] = Convert.ToByte(s.Substring(index+i, 2), 16);
				Array.Reverse(bytes);
				return HexEncoding[numberChars].GetString(bytes);
			}
			
		}
	}

	public class YamlBlock {
		public int Indent { get; set; }
		public string Suffix { get; set; }
		public string Content { get; set; }
	}

}
