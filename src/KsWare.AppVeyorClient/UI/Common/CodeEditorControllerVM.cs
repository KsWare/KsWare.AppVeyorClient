using System.Text.RegularExpressions;
using System.Windows.Input;

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
			Data.Options.ConvertTabsToSpaces = true;
			Data.Options.IndentationSize = 4;
			Data.Options.CutCopyWholeLine = false;

			Data.PreviewKeyDown += DataOnPreviewKeyDown;
			Data.PreviewKeyUp   += DataOnPreviewKeyUp;
//			Data.SelectionChanged+=DataOnSelectionChanged;
		}

//		private void DataOnSelectionChanged(object s, RoutedEventArgs e) {
//			if (Data.SelectionLength == 0) {
//				if (ActivePoint.IsInIndentRegion && ActivePoint.CharPosition % 4 != 0) {
//					Data.Select(ActivePoint.LineStart + ActivePoint.IndentPosition*4,0);
//				}
//			}
//		}

		private void DataOnPreviewKeyUp(object sender, KeyEventArgs e) {
			var modKey = e.KeyboardDevice.Modifiers;
			var combinedKey = (Key) ((int) modKey << 16 | (int) e.Key);
			switch (combinedKey) {
//				case (Key) ((int) Key.Tab | (int) ModifierKeys.None << 16): e.Handled = true; break;
//				case (Key) ((int) Key.Tab | (int) ModifierKeys.Shift << 16): e.Handled = true; break;
				case (Key) ((int) Key.Back | (int) ModifierKeys.None << 16): e.Handled = true; break;
			}
		}
		private void DataOnPreviewKeyDown(object sender, KeyEventArgs e) {
			var modKey = e.KeyboardDevice.Modifiers;

			var combinedKey = (Key) ((int) modKey << 16 | (int) e.Key);
			switch (combinedKey) {
//				case (Key)((int)Key.Tab | (int)ModifierKeys.None<<16) : e.Handled = true; OnTabPressed(); break;
//				case (Key)((int)Key.Tab | (int)ModifierKeys.Shift<<16) : e.Handled = true; OnTabBackPressed(); break;
				case (Key)((int)Key.Back | (int)ModifierKeys.None<<16) : e.Handled = true; OnBackPressed(); break;
			}
		}

		private void OnBackPressed() {
			if (Data.SelectionLength > 0) {
				Data.SelectedText = "";
			}
			else {
				ActivePoint.Refresh();
				if (ActivePoint.IsInIndentRegion && ActivePoint.CharPosition>0) {
					if (ActivePoint.CharPosition % 4 == 0) {
						Data.Select(ActivePoint.LineStart + (ActivePoint.IndentPosition-1) * 4, 4);
					}
					else {
						Data.Select(ActivePoint.LineStart + (ActivePoint.IndentPosition) * 4, 4);
					}
					Data.SelectedText = "";
				}
				else {
					if(Data.SelectionStart==0) return;
					var m = 1;
					if (Data.Text[Data.SelectionStart - 1] == '\n') {
						if (Data.SelectionStart>1 && Data.Text[Data.SelectionStart - 2] == '\r') m = 2;
					}
					Data.Select(Data.SelectionStart-m,m);
					Data.SelectedText = "";					
				}
			}
		}

//		private void OnTabPressed() {
//			if (Data.SelectionLength > 0) {
//				var li0 = Data.GetLineIndexFromCharacterIndex(Data.SelectionStart);
//				var lp0 = Data.GetCharacterIndexFromLineIndex(li0);
//				var cp0 = Data.SelectionStart - lp0;
//				var li1 = Data.GetLineIndexFromCharacterIndex(Data.SelectionStart + Data.SelectionLength);
//				var lp1 = Data.GetCharacterIndexFromLineIndex(li1);
//				var cp1 = Data.SelectionStart + Data.SelectionLength - lp1;
//				for (int i = li0; i <= li1; i++) {
//					var ci = Data.GetCharacterIndexFromLineIndex(i);
//					Data.Select(ci,0);
//					Data.SelectedText = new string(' ', 4);
//				}
//				var p0 = lp0 + cp0 + 4;
//				var p1 = Data.GetCharacterIndexFromLineIndex(li1) + cp1 + 4;
//				Data.Select(p0,p1-p0);
//			}
//			else {
//				var li=Data.GetLineIndexFromCharacterIndex(Data.SelectionStart);
//				var lci=Data.GetCharacterIndexFromLineIndex(li);
//				var cp = Data.SelectionStart - lci;
//				var t=Data.GetLineText(li);
//				var sp= Regex.Match(t, @"^\x20*");
//				if (cp <= sp.Length) {
//					var ti = cp / 4;
//					var ind = sp.Length / 4;
//					var sp1 = (ind + 1) * 4;
//					Data.SelectedText=new string(' ',sp1-sp.Length);
//					Data.Select(lci + (ti+1) * 4, 0);
//				}
//				else {
////					int i = (cp / 4) * 4;
////					Data.SelectedText = new string(' ', sp1 - sp0);
//				}
//			}
//		}

//		private void OnTabBackPressed() {
//			if (Data.SelectionLength > 0) {
//				var li0 = Data.GetLineIndexFromCharacterIndex(Data.SelectionStart);
//				var lp0 = Data.GetCharacterIndexFromLineIndex(li0);
//				var cp0 = Data.SelectionStart - lp0;
//				var li1 = Data.GetLineIndexFromCharacterIndex(Data.SelectionStart + Data.SelectionLength);
//				var lp1 = Data.GetCharacterIndexFromLineIndex(li1);
//				var cp1 = Data.SelectionStart + Data.SelectionLength - lp1;
//				var x0 = false;
//				var x1 = false;
//				for (int i = li0; i <= li1; i++) {
//					if (Data.GetLineText(i).StartsWith("    ")) {
//						if (i == li0) x0 = true;
//						if (i == li1) x1 = true;
//						var ci = Data.GetCharacterIndexFromLineIndex(i);
//						Data.Select(ci, 4);
//						Data.SelectedText = "";
//					}
//				}
//				var p0 = lp0                                      + cp0 + (x0 ? 4 : 0);
//				var p1 = Data.GetCharacterIndexFromLineIndex(li1) + cp1 + (x1 ? 4 : 0);
//				Data.Select(p0, p1 - p0);
//			}
//		}
	}

}