using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using KsWare.Presentation.Core.Providers;

namespace KsWare.AppVeyorClient.UI.Common {

	public class YamlTextBoxControllerVM:CodeTextBoxControllerVM {

		public YamlTextBoxControllerVM() {
			RegisterChildren(()=>this);
		}

		public void ExpandSelection() {
			Data.Focus();
			var l0 = Data.GetLineIndexFromCharacterIndex(Data.SelectionStart);
			var l1 = Data.GetLineIndexFromCharacterIndex(Data.SelectionStart+Data.SelectionLength);
			var selStart=Data.GetCharacterIndexFromLineIndex(l0);
			var selEnd = Data.GetCharacterIndexFromLineIndex(l1) + (Data.LineCount-1 == l1 ? Data.GetLineLength(l1) : Data.GetLineLength(l1)-2);
			Data.Select(selStart, selEnd - selStart);
			Data.ScrollToHorizontalOffset(0);
		}
	}
}
