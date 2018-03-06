using System.Text.RegularExpressions;
using System.Windows.Controls;
using ICSharpCode.AvalonEdit;

namespace KsWare.AppVeyorClient.UI.Common {

	public class ActivePoint {

		private readonly TextEditor _editor;

		public ActivePoint(TextEditor editor) {
			_editor                  =  editor;
//TODO		_editor.SelectionChanged += (s, e) => Refresh();
		}

		public bool IsInIndentRegion { get; private set; }

		public int IndentPosition { get; private set; }
		public int CharPosition { get; private set; }

		public int LineStart { get; private set; }

		public int LineIndex { get; private set; }

		public void Refresh() {
			if (_editor.Text.Length == 0) {
				LineIndex        = 0;
				LineStart        = 0;
				CharPosition     = 0;
				IsInIndentRegion = true;
				IndentPosition   = 0;
				return;
			}
//			var li  = LineIndex=_editor.GetLineIndexFromCharacterIndex(_editor.SelectionStart);
//			var lci = LineStart = _editor.GetCharacterIndexFromLineIndex(li);
//			var cp  = CharPosition = _editor.SelectionStart - lci;
//			var t   = _editor.GetLineText(li);
//			var sp  = Regex.Match(t, @"^\x20*");

			var l = _editor.Document.GetLineByOffset(_editor.SelectionStart);
			var li = LineIndex = l.LineNumber;
			var lci = LineStart = l.Offset;
			var cp = CharPosition = _editor.SelectionStart - lci;
			var t = _editor.Text.Substring(LineStart, l.TotalLength);
			var sp = Regex.Match(t, @"^\x20*");

			IsInIndentRegion = cp <= sp.Length;
			IndentPosition   = IsInIndentRegion ? cp / 4 : -1;
		}
	}

}