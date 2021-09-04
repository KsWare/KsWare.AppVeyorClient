using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using JetBrains.Annotations;
using KsWare.Presentation.ViewModelFramework;

namespace KsWare.AppVeyorClient.UI.PanelConfiguration {

	public class PopupVM : ObjectVM {

		/// <inheritdoc />
		public PopupVM() {
			if (IsInDesignMode) {
				Title = "Lorem ipsum dolor sit amet";
				Document = new FlowDocument(new Paragraph(new Run("Lorem ipsum dolor sit amet, consetetur sadipscing elitr, sed diam nonumy eirmod tempor invidunt ut labore et dolore magna aliquyam erat, sed diam voluptua. At vero eos et accusam et justo duo dolores et ea rebum. Stet clita kasd gubergren, no sea takimata sanctus est Lorem ipsum dolor sit amet. Lorem ipsum dolor sit amet, consetetur sadipscing elitr, sed diam nonumy eirmod tempor invidunt ut labore et dolore magna aliquyam erat, sed diam voluptua. At vero eos et accusam et justo duo dolores et ea rebum. Stet clita kasd gubergren, no sea takimata sanctus est Lorem ipsum dolor sit amet.")));
				return;
			}
			RegisterChildren(() => this);
		}

		public PopupView View { get; set; }

		public bool IsOpen { get => Fields.GetValue<bool>(); set => Fields.SetValue(value); }

		public string Title { get => Fields.GetValue<string>(); set => Fields.SetValue(value); }

		public ActionVM CloseAction { get; [UsedImplicitly] private set; }

		public object Document { get => Fields.GetValue<object>(); set => Fields.SetValue(value); }

		private void DoClose() {
			IsOpen = false;
		}

		public void Show() {
			IsOpen = true;
		}
	}

}
