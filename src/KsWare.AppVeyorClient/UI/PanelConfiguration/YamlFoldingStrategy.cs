using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Folding;

namespace KsWare.AppVeyorClient.UI.PanelConfiguration {

	public class YamlFoldingStrategy {

		/// <summary>
		/// Create <see cref="T:ICSharpCode.AvalonEdit.Folding.NewFolding" />s for the specified document and updates the folding manager with them.
		/// </summary>
		public void UpdateFoldings(FoldingManager manager, TextDocument document) {
			var newFoldings = CreateNewFoldings(document, out var firstErrorOffset);
			manager.UpdateFoldings(newFoldings, firstErrorOffset);
		}

		/// <summary>
		/// Create <see cref="T:ICSharpCode.AvalonEdit.Folding.NewFolding" />s for the specified document.
		/// </summary>
		public IEnumerable<NewFolding> CreateNewFoldings(TextDocument document, out int firstErrorOffset) {
			try {
				return CreateNewFoldings(document, new YamlReader(document.CreateReader()), out firstErrorOffset);
			}
			catch (XmlException ex) {
				firstErrorOffset = 0;
				return Enumerable.Empty<NewFolding>();
			}
		}

		/// <summary>
		/// Create <see cref="T:ICSharpCode.AvalonEdit.Folding.NewFolding" />s for the specified document.
		/// </summary>
		public IEnumerable<NewFolding> CreateNewFoldings(TextDocument document, YamlReader reader, out int firstErrorOffset) {
			Stack<YamlFoldStart> yamlFoldStarts = new Stack<YamlFoldStart>();
			List<NewFolding>    foldMarkers       = new List<NewFolding>();
			try {
				while (reader.ReadLine()) {
					if (reader.Indent==0 && !reader.IsEmptyLine) {
						if (yamlFoldStarts.Count > 0) {
							var foldStart=yamlFoldStarts.Pop();
							if (foldStart.StartLine < reader.LineNumber-1) 
								CreateElementFold(document, foldMarkers, reader, foldStart);
						}
						var elementFoldStart = CreateElementFoldStart(document, reader);
						yamlFoldStarts.Push(elementFoldStart);
						continue;
					}
				}
				while (yamlFoldStarts.Count>0) {
					var foldStart = yamlFoldStarts.Pop();
					if (foldStart.StartLine < reader.LineNumber) CreateElementFold(document, foldMarkers, reader, foldStart);
				}
				firstErrorOffset = -1;
			}
			catch (XmlException ex) {
				firstErrorOffset = ex.LineNumber < 1 || ex.LineNumber > document.LineCount
					? 0
					: document.GetOffset(ex.LineNumber, ex.LinePosition);
			}
			foldMarkers.Sort((Comparison<NewFolding>) ((a, b) => a.StartOffset.CompareTo(b.StartOffset)));
			return (IEnumerable<NewFolding>) foldMarkers;
		}

		private YamlFoldStart CreateElementFoldStart(TextDocument document, YamlReader reader) {
			var yamlFoldStart = new YamlFoldStart {
				StartLine   = reader.LineNumber,
				StartOffset = document.GetOffset(reader.LineNumber, 1),
				Name        = reader.Key
			};
			return yamlFoldStart;
		}

		private static void CreateElementFold(TextDocument document, List<NewFolding> foldMarkers, YamlReader reader, YamlFoldStart foldStart) {
			if (reader.IsLastLine) {
				foldStart.EndOffset = reader.LineOffset+reader.LineLength;
			}
			else {
				foldStart.EndOffset = reader.LineOffset - 2;
			}
			
			foldMarkers.Add((NewFolding) foldStart);
		}

		internal sealed class YamlFoldStart : NewFolding {
			internal int StartLine;
		}
	}

	public class YamlReader {

		private readonly TextReader _textReader;
		private readonly StringBuilder _line=new StringBuilder();
		private int _lineNumber;
		private int _lineOffset;
		private int _offset;
		private int _lineTotalLength;

		public YamlReader(TextReader textReader) {
			_textReader = textReader;
		}

		public string Text => _line.ToString();

		/// <summary>Gets the current line number.</summary>
		/// <returns>The current line number or 0 if no line information is available (for example, <see cref="M:System.Xml.IXmlLineInfo.HasLineInfo" /> returns <see langword="false" />).</returns>
		public int LineNumber => _lineNumber;

		/// <summary>Gets the current line position.</summary>
		/// <returns>The current line position or 0 if no line information is available (for example, <see cref="M:System.Xml.IXmlLineInfo.HasLineInfo" /> returns <see langword="false" />).</returns>
		public int LinePosition => 1;

		public int LineOffset => _lineOffset;

		public int LineLength => _lineTotalLength;

		public int Indent { get; private set; }

		public int IndentChanged { get; set; }

		public bool IsSequence { get; private set; }

		public string Key { get; private set; }

		public string BlockFormat { get; private set; } // TODO Name

		public bool IsStartBlock { get; private set; }

		public bool IsEmptyLine { get; private set; }

		public bool IsLastLine { get; private set; }

		public int LineEndTerminatorLength { get; private set; }

		public bool ReadLine() {
			var v = _textReader.Read();
			if (v == -1) return false;
			var isFirstChar = true;
			
			_line.Clear();
			_lineOffset = _offset;
			LineEndTerminatorLength = 0;
			IsLastLine = false;
			IsEmptyLine = false;
			var lastIndent = Indent;
			Indent = 0;
			var isInIdent=true;
			var hasSequenceChar = false;
			IsSequence = false;

			while (true) {
				if (isFirstChar) isFirstChar = false;
				else v = _textReader.Read();
				
				if (v == -1) { IsLastLine = true; break;}
				_offset++;
				var c = (char) v;
				_line.Append(c);

				#region line end detection
				if (c == '\r') LineEndTerminatorLength++;
				else if (c == '\n') { LineEndTerminatorLength++; break; }
				else LineEndTerminatorLength = 0;
				#endregion

				#region indentation detection
				if (isInIdent) {
					if (hasSequenceChar) {
						if (c == ' ') {
							Indent += 2;
							isInIdent = false;
						}
					}
					else {
						if (c == ' ') Indent++;
						else if (c == '-') hasSequenceChar = true;
						else isInIdent                     = false;
					}
				}
				#endregion
			}
			IndentChanged = Indent - lastIndent;
			_lineNumber++;
			_lineTotalLength = _offset - _lineOffset;
			IsEmptyLine = _lineTotalLength - LineEndTerminatorLength == 0;
			ParseLine();
			return true;
		}

		//                                         "             -             keyx:                |+
		static readonly Regex BlockRegex=new Regex(@"^(?<sequence>-\x20)?(?<key>\w+):(\x20+(?<block>[|>][+-]?))?(?<ws>\s*)$",RegexOptions.Compiled);

		static readonly Regex TopLevelKey = new Regex(@"^(?<key>\w+):", RegexOptions.Compiled);
		
		private void ParseLine() {
			var keyMatch = TopLevelKey.Match(_line.ToString());
			Key = keyMatch.Success ? keyMatch.Value : null;

//			// only top-level blocks
//			var match = BlockRegex.Match(_line.ToString());
//			if (match.Success) {
//				Indent = match.Groups["indent"].Value.Length;
//				IsSequence = match.Groups["sequence"].Success;
//				Key= match.Groups["key"].Value;
//				BlockFormat = match.Groups["block"].Success ? match.Groups["block"].Value : "";
//				IsStartBlock = true;
//			}
//			else {
//				Indent = 0;
//				IsSequence = false;
//				Key = null;
//				BlockFormat = null;
//				IsStartBlock = false;
//			}

		}

		

	}

}