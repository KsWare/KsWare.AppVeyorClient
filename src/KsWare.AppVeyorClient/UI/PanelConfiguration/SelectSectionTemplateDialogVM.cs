using KsWare.Presentation.ViewModelFramework;

namespace KsWare.AppVeyorClient.UI.PanelConfiguration {

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
			Data.DialogResult = true;
			base.DoClose();
		}

		private void DoCancel() {
			Data.DialogResult = false;
			Data.Close();
		}

	}

}
