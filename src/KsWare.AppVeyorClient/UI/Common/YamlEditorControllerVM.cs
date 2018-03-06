using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using KsWare.Presentation.Core.Providers;

namespace KsWare.AppVeyorClient.UI.Common {

	public class YamlEditorControllerVM:CodeEditorControllerVM {

		public YamlEditorControllerVM() {
			RegisterChildren(()=>this);
		}
	}
}
