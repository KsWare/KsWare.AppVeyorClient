using System.Text.RegularExpressions;
using System.Windows.Controls;

namespace KsWare.AppVeyorClient.UI.Common {

	public class ActivePoint {
		private readonly TextBox _textBox;

		public ActivePoint(TextBox textBox) {
			_textBox                  =  textBox;
			_textBox.SelectionChanged += (s, e) => Refresh();
		}

		public bool IsInIndentRegion { get; private set; }

		public int IndentPosition { get; private set; }
		public int CharPosition { get; private set; }

		public int LineStart { get; private set; }

		public int LineIndex { get; private set; }

		public void Refresh() {
			if (_textBox.Text.Length == 0) {
				LineIndex        = 0;
				LineStart        = 0;
				CharPosition     = 0;
				IsInIndentRegion = true;
				IndentPosition   = 0;
				return;
			}
			var li  = LineIndex=_textBox.GetLineIndexFromCharacterIndex(_textBox.SelectionStart);
			var lci = LineStart = _textBox.GetCharacterIndexFromLineIndex(li);
			var cp  = CharPosition = _textBox.SelectionStart - lci;
			var t   = _textBox.GetLineText(li);
			var sp  = Regex.Match(t, @"^\x20*");

			IsInIndentRegion = cp <= sp.Length;
			IndentPosition   = IsInIndentRegion ? cp / 4 : -1;
		}
	}

}