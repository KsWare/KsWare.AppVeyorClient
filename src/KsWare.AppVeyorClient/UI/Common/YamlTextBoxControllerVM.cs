using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KsWare.Presentation.Core.Providers;

namespace KsWare.AppVeyorClient.UI.Common {

	public class YamlTextBoxControllerVM:TextBoxControllerVM {

		public YamlTextBoxControllerVM() {
			RegisterChildren(()=>this);
		}

		public void ExpandSelection() {
			Data.Focus();
			var l0 = Data.GetLineIndexFromCharacterIndex(Data.SelectionStart);
			var l1 = Data.GetLineIndexFromCharacterIndex(Data.SelectionStart+Data.SelectionLength);
			var selStart=Data.GetCharacterIndexFromLineIndex(l0);
			var selEnd = Data.GetCharacterIndexFromLineIndex(l1) + Data.GetLineLength(l1);
			Data.Select(selStart, selEnd - selStart);
			Data.ScrollToHorizontalOffset(0);
		}
	}
}
