﻿using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Windows.Input;

namespace KsWare.AppVeyorClient.Shared.AvalonEditExtension {

	public class CodeEditorControllerVM : TextEditorControllerVM {

		public CodeEditorControllerVM() {
			RegisterChildren(() => this);
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
			var combinedKey = (Key)(((int)modKey << 16) | (int)e.Key);
			switch (combinedKey) {
				case Key.Tab: OnTabPressed(e); break;
//				case (Key) ((int) Key.Tab | (int)ModifierKeys.Shift << 16): OnTabBackPressed(e); break;
				case Key.Back: e.Handled = true; break;
			}
		}

		private void DataOnPreviewKeyDown(object sender, KeyEventArgs e) {
			var modKey = e.KeyboardDevice.Modifiers;

			var combinedKey = (Key)(((int)modKey << 16) | (int)e.Key);
			switch (combinedKey) {
				case Key.Tab: OnTabPressed(e); break;
//				case (Key)((int)Key.Tab | (int)ModifierKeys.Shift<<16) : OnTabBackPressed(e); break;
				case Key.Back: e.Handled = true; OnBackPressed(); break;
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
				var li1 = Data.TextArea.Selection.EndPosition.Line;
				var l0 = Data.Document.GetLineByNumber(li0);
				var ss = Data.TextArea.Selection.StartPosition.Line;
				var sl = Data.TextArea.Selection.Length;

				for (int i = li0; i <= li1; i++) {
					var l = i == li0 ? l0 : Data.Document.GetLineByNumber(i);
					Data.Select(l.Offset, 0);
					Data.SelectedText = new string(' ', 4);
				}

				Data.Select(ss + 4, sl);
			}
			else {
				var selectedLine = Data.Document.GetLineByOffset(Data.SelectionStart);
				var cp = Data.SelectionStart - selectedLine.Offset;
				var lineText = Data.Text.Substring(selectedLine.Offset, selectedLine.Length);
				var indentation = Regex.Match(lineText, @"^\x20*");
				if (cp <= indentation.Length) {
					var ti = cp / 4;
					var ind = indentation.Length / 4;
					var sp1 = (ind + 1) * 4;
					Data.SelectedText = new string(' ', sp1 - indentation.Length);
					Data.Select(selectedLine.Offset + (ti + 1) * 4, 0);
				}
				else {
//					int i = (cp / 4) * 4;
//					Data.SelectedText = new string(' ', sp1 - sp0);
				}
			}
		}
	}

}
