using KsWare.AppVeyorClient.UI.PanelConfiguration;
using KsWare.Presentation.ViewModelFramework;

namespace KsWare.AppVeyorClient.UI.PanelYamlCodeEditor {

	public class SelectSectionTemplateDialogVM : DialogWindowVM {

		public SelectSectionTemplateDialogVM() {
			CancelAction = new ActionVM { MːDoAction = DoCancel };
		}

		public SectionTemplateData[] Templates {
			get => Fields.GetValue<SectionTemplateData[]>();
			set => Fields.SetValue(value);
		}

		public SectionTemplateData SelectedTemplate {
			get => Fields.GetValue<SectionTemplateData>();
			set => Fields.SetValue(value);
		}

		public ActionVM CancelAction { get; private set; }

		protected override void DoClose() {
			DialogResult = true;
			base.DoClose();
		}

		private void DoCancel() {
			DialogResult = false;
			Close();
		}

	}

}
