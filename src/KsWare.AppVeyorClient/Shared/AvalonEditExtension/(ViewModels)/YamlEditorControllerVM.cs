using System.Text.RegularExpressions;
using System.Xml;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Highlighting.Xshd;

namespace KsWare.AppVeyorClient.Shared.AvalonEditExtension {

	public class YamlEditorControllerVM:CodeEditorControllerVM {

		public YamlEditorControllerVM() {
			RegisterChildren(()=>this);
		}

		protected override void OnViewConnected() {
			base.OnViewConnected();
			var reader = XmlReader.Create("AppVeyor-yaml.xshd");
			Data.SyntaxHighlighting = HighlightingLoader.Load(reader, HighlightingManager.Instance);
		}

		public void ExpandCodeBlock() {
			
			var l0=Data.Document.GetLineByOffset(Data.SelectionStart);
			var t0 = "";
			var l1 = l0.Offset + l0.TotalLength >= Data.SelectionStart + Data.SelectionLength
				? Data.Document.GetLineByOffset(Data.SelectionStart + Data.SelectionLength)
				: l0;
			var multiLine = "";

			// find block-start "- ps: "
			while (true) {
				t0 = Data.GetLineText(l0);
				var m = Regex.Match(t0, @"^-\x20ps:\x20+(?<multiline>[>|][+-]?)?"); // TODO 
				if (m.Success) {
					multiLine = m.Groups["multiline"].Value;
					if (!string.IsNullOrWhiteSpace(multiLine)) l0 = l0.NextLine;
					break;
				}
				if(l0.LineNumber==1) break;
				l0 = l0.PreviousLine;
			}

			if (!string.IsNullOrWhiteSpace(multiLine)) {
				// find block-end
				while (true) {
					var t = Data.GetLineText(l1);
					if (!Regex.IsMatch(t, @"^\x20{4}|^$")) {
						l1 = l1.PreviousLine;
						break;
					}
					l1 = l1.NextLine;
				}
			}
			else {
				l1 = l0;
			}

			Data.Select(l0.Offset,l1.Offset+l1.Length - l0.Offset);
		}
	}
}
