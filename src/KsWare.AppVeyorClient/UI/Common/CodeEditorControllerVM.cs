using System;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Windows.Input;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Search;
using KsWare.AppVeyorClient.Helpers;

namespace KsWare.AppVeyorClient.UI.Common {

	public class CodeEditorControllerVM : TextEditorControllerVM {

		public CodeEditorControllerVM() {
			RegisterChildren(()=>this);
		}

		public void ExpandSelection() {
			int selStart;
			int selEnd;
			if (Data.SelectionLength == 0) {
				var li = Data.TextArea.Caret.Line;
				var l = Data.Document.GetLineByNumber(li);
				selStart = l.Offset;
				selEnd = l.Offset + l.TotalLength - l.DelimiterLength;
			}
			else {
				var li0 = Data.TextArea.Selection.StartPosition.Line;
				var li1 = Data.TextArea.Selection.EndPosition.Line;
				selStart = Data.Document.GetLineByNumber(li0).Offset;
				var l1 = Data.Document.GetLineByNumber(li1);
				selEnd = l1.Offset + l1.TotalLength - l1.DelimiterLength;				
			}
			
			Data.Focus();
			Data.Select(selStart, selEnd - selStart);
//			Data.ScrollToHorizontalOffset(0);
		}

		protected override void OnViewConnected() {
			base.OnViewConnected();
			Data.Options.ConvertTabsToSpaces = false;
			Data.Options.IndentationSize = 4;
			Data.Options.CutCopyWholeLine = false;

			Data.PreviewKeyDown += DataOnPreviewKeyDown;
			Data.PreviewKeyUp   += DataOnPreviewKeyUp;
//			Data.SelectionChanged+=DataOnSelectionChanged;

			// internal search panel
			// SearchPanel.Install(Data);
		}

//		private void DataOnSelectionChanged(object s, RoutedEventArgs e) {
//			if (Data.SelectionLength == 0) {
//				if (DocumentPosition.IsInIndentRegion && DocumentPosition.LineCharPosition % 4 != 0) {
//					Data.Select(DocumentPosition.LineStart + DocumentPosition.IndentPosition*4,0);
//				}
//			}
//		}

		private void DataOnPreviewKeyUp(object sender, KeyEventArgs e) {
			var modKey = e.KeyboardDevice.Modifiers;
			var combinedKey = (Key) ((int) modKey << 16 | (int) e.Key);
			switch (combinedKey) {
				case (Key) ((int) Key.Tab | (int) ModifierKeys.None << 16): OnTabPressed(e); break;
//				case (Key) ((int) Key.Tab | (int) ModifierKeys.Shift << 16): OnTabBackPressed(e); break;
				case (Key) ((int) Key.Back | (int) ModifierKeys.None << 16): e.Handled = true; break;
			}
		}
		private void DataOnPreviewKeyDown(object sender, KeyEventArgs e) {
			var modKey = e.KeyboardDevice.Modifiers;

			var combinedKey = (Key) ((int) modKey << 16 | (int) e.Key);
			switch (combinedKey) {
				case (Key)((int)Key.Tab | (int)ModifierKeys.None<<16) : OnTabPressed(e); break;
//				case (Key)((int)Key.Tab | (int)ModifierKeys.Shift<<16) : OnTabBackPressed(e); break;
				case (Key)((int)Key.Back | (int)ModifierKeys.None<<16) : e.Handled = true; OnBackPressed(); break;
			}
		}

		private void OnBackPressed() {
			if (Data.SelectionLength > 0) {
				Data.SelectedText = "";
			}
			else {
				var pos = Data.GetCaretPosition();
				if (pos.IsInIndentRegion && pos.LineCharPosition>1) {
					if (pos.LineCharIndex % 4 == 0) {
						Data.Select(pos.LineOffset + (pos.IndentIndex-1) * 4, 4);
					}
					else {
						var p0 = pos.LineOffset + (pos.IndentIndex) * 4;
						var le = pos.Offset - p0;
						Data.Select(p0, le);
					}
					Data.SelectedText = "";
				}
				else {
					if(pos.Offset==0) return;
					var m = 1;
					if (Data.Document.GetCharAt(pos.Offset - 1) == '\n') {
						if (Data.SelectionStart>1 && Data.Document.GetCharAt(pos.Offset - 2) == '\r') m = 2;
					}
					Data.Select(Data.SelectionStart-m,m);
					Data.SelectedText = "";					
				}
			}
		}

		private void OnTabPressed(KeyEventArgs e) {
//			Debug.WriteLine($"e.KeyStates {e.KeyStates}"); // Down Down ... None
			Debug.WriteLine($"{e.IsDown,-5} {e.IsUp,-5} {e.IsRepeat}");
			// D 
			// D IsRepeat
			// ..
			// U

			if (Data.Options.ConvertTabsToSpaces == false) e.Handled = true; // handled in OnTabPressed
			else return;
			if (e.IsDown && !e.IsRepeat) return;

			if (Data.SelectionLength > 0) {
				var li0 = Data.TextArea.Selection.StartPosition.Line;
				var cp0 = Data.TextArea.Selection.StartPosition.Column;
				var li1 = Data.TextArea.Selection.EndPosition.Line;
				var cp1 = Data.TextArea.Selection.EndPosition.Column;
				var l0 = Data.Document.GetLineByNumber(li0);

				for (int i = li0; i <= li1; i++) {
					var l = i == li0 ? l0 : Data.Document.GetLineByNumber(i);
					Data.Select(l.Offset,0);
					Data.SelectedText=new string(' ', 4);
				}

				var ss = l0.Offset + cp0 - 1 + 4;
				var sl = li0 == li1 ? l0.Offset + cp1 - 1 + 4 : (li1 - li0 - 1) * 4;
				Data.Select(ss,sl);
			}
			else {
				var l=Data.Document.GetLineByOffset(Data.SelectionStart);
				var lci=l.Offset;
				var cp = Data.SelectionStart - lci;
				var t = Data.Text.Substring(l.Offset, l.Length);
				var sp= Regex.Match(t, @"^\x20*");
				if (cp <= sp.Length) {
					var ti = cp / 4;
					var ind = sp.Length / 4;
					var sp1 = (ind + 1) * 4;
					Data.SelectedText=new string(' ',sp1-sp.Length);
					Data.Select(lci + (ti+1) * 4, 0);
				}
				else {
//					int i = (cp / 4) * 4;
//					Data.SelectedText = new string(' ', sp1 - sp0);
				}
			}
		}
	}

}