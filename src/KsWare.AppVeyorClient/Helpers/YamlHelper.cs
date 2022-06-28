using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace KsWare.AppVeyorClient.Helpers {

	public static class YamlHelper {
		// ATTENTION: This is NOT a complete YAML parser/generator!
		// Only parts used in appveyor.yaml are recognized/generated.
		// - comments are not supported

		private static readonly Dictionary<int,Encoding> HexEncoding=new Dictionary<int, Encoding>() {
			{2, Encoding.GetEncoding("iso-8859-1")},
			{4, Encoding.Unicode},
			{8, Encoding.UTF32}
		};

		public static string GetSuffix(string s) => ExtractBlock(s).Suffix; 

		public static int GetIndent(string s) => ExtractBlock(s).Indent;

		public static string FormatBlock(string content, string suffix, int indent, ScalarType scalarType) {
			var lines = content.Split(new[] {"\r\n", "\n"}, StringSplitOptions.None);
			var sb = new StringBuilder();
			var sp = new string(' ', indent);
			var sp2 = new string(' ', indent + 4);
			var name = Regex.Match(suffix, @"^(?<name>-(\s+[a-z]+:)?\s)\s*", RegexOptions.Compiled | RegexOptions.IgnoreCase).Groups["name"].Value;

			if (scalarType == ScalarType.None) scalarType = DetectScalarType(suffix);
			//TODO implement chomping
			switch (scalarType) {
				case ScalarType.BlockLiteral:
					sb.AppendLine($"{sp}{name}|");
					foreach (var line in lines) sb.AppendLine($"{sp2}{line}");
					break;
				case ScalarType.BlockLiteralKeep:
					sb.AppendLine($"{sp}{name}|+");
					foreach (var line in lines) sb.AppendLine($"{sp2}{line}");
					break;
				case ScalarType.BlockLiteralStrip: 
					sb.AppendLine($"{sp}{name}|-");
					foreach (var line in lines) sb.AppendLine($"{sp2}{line}");
					break;

				case ScalarType.BlockFolded:
					sb.AppendLine($"{sp}{name}>");
					foreach (var line in lines)sb.AppendLine(string.IsNullOrWhiteSpace(line) ? "" : $"{sp2}{line}");
					break;
				case ScalarType.BlockFoldedKeep:
					sb.AppendLine($"{sp}{name}>+");
					foreach (var line in lines)sb.AppendLine(string.IsNullOrWhiteSpace(line) ? "" : $"{sp2}{line}");
					break;
				case ScalarType.BlockFoldedStrip:
					sb.AppendLine($"{sp}{name}>-");
					foreach (var line in lines)sb.AppendLine(string.IsNullOrWhiteSpace(line) ? "" : $"{sp2}{line}");
					break;

				case ScalarType.FlowDoubleQuoted:
					sb.AppendLine($"{sp}{name}\"" + EscapeDoubleQuotedString(content) + "\"");
					break;
				case ScalarType.FlowSingleQuoted:
					sb.AppendLine($"{sp}{name}'" + EscapeSingleQuotedString(content) + "'");
					break;
				case ScalarType.Plain:
					foreach (var line in lines) sb.AppendLine($"{sp}{name}{line}");
					break;
				default: 
					throw new NotImplementedException();
			}
			return sb.ToString().TrimEnd();
		}

		private static ScalarType DetectScalarType(YamlRegExMatch match) {
			if(!match.Success) return ScalarType.None;
			switch (match.Multiline + match.Chomping) {
				case "|" : return ScalarType.BlockLiteral/*Clip*/;
				case "|+": return ScalarType.BlockLiteralKeep;
				case "|-": return ScalarType.BlockLiteralStrip;
				case ">" : return ScalarType.BlockFolded/*Clip*/;
				case ">+": return ScalarType.BlockFoldedKeep;
				case ">-": return ScalarType.BlockFoldedStrip;
				default:
					switch (match.Quotes) {
						case "\"": return ScalarType.FlowDoubleQuoted;
						case "'": return ScalarType.FlowSingleQuoted;
						case "": return ScalarType.Plain;
					}
					return ScalarType.None;
			}
		}

		public static ScalarType DetectScalarType(string line) => DetectScalarType(YamlRegEx.Match(line));

		public static YamlBlock ExtractBlock(string s) {
			var match = YamlRegEx.MatchFull(s);
			var scalarType = DetectScalarType(match);
			switch (scalarType) {
				case ScalarType.FlowDoubleQuoted:
					return new YamlBlock(match.IndentLength, match.PreContent, UnescapeDoubleQuotedString(match.Content));
				case ScalarType.FlowSingleQuoted:
					return new YamlBlock(match.IndentLength, match.PreContent, UnescapeSingleQuotedString(match.Content));
				case ScalarType.BlockFolded:
				case ScalarType.BlockFoldedKeep:
				case ScalarType.BlockFoldedStrip:
				case ScalarType.BlockLiteral:
				case ScalarType.BlockLiteralKeep:
				case ScalarType.BlockLiteralStrip:
					return new YamlBlock(match.IndentLength, match.PreContent, UnescapeBlock(match.Content, scalarType));
				case ScalarType.Plain:
					return new YamlBlock(match.IndentLength, match.PreContent, match.Content);
				default:
					return null;
			}
		}

		public static string UnescapeBlock(string content, ScalarType scalarType) {
			if (content.StartsWith("\r\n")) content = content.Substring(2);
			else if (content.StartsWith("\n")) content = content.Substring(1);
			var indent = Regex.Match(content, @"^\s+").Value;
			var indentPattern = "^" + indent.Replace(" ", @"\x20").Replace("\t", @"\x09");
			switch (scalarType) {
				case ScalarType.BlockFolded:
				case ScalarType.BlockFoldedStrip:
				case ScalarType.BlockFoldedKeep:
				case ScalarType.BlockLiteral:
				case ScalarType.BlockLiteralStrip:
				case ScalarType.BlockLiteralKeep:
					var s = Regex.Replace(content, indentPattern, "", RegexOptions.Multiline | RegexOptions.Compiled); // remove indentation
					return Regex.Replace(s, @"\r?\n", "\r\n", RegexOptions.Multiline | RegexOptions.Compiled); //normalize line endings
				default: throw new NotSupportedException();
			}
		}

		public static string EscapeSingleQuotedString(string s) {
			var s1=s.Replace("'", "''");
			return s1;
		}

		public static string EscapeDoubleQuotedString(string s) {
			// https://yaml.org/spec/current.html#id2517668
			var s1 = s
				.Replace("\\", "\\\\")
				.Replace("\r\n", "\\n")
				.Replace("\n", "\\n")
				.Replace("\"", "\\\""
				);
			return s1;
		}

		public static string UnescapeSingleQuotedString(string s) {
			var s1 = s.Replace("''", "'");
			return s1;
		}

		public static string UnescapeDoubleQuotedString(string s) {
			/*  YAML’s double-quoted style uses familiar C-style escape sequences. This enables ASCII encoding of non-printable or 8-
				bit (ISO 8859-1) characters such as “\x3B”. Non-printable 16-bit Unicode and 32-bit (ISO/IEC 10646) characters are
				supported with escape sequences such as “\u003B” and “\U0000003B”. */
			var sb = new StringBuilder();
			bool esc = false;
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
}
