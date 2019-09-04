using System.Windows;
using KsWare.Presentation.ViewModelFramework;

namespace KsWare.AppVeyorClient.UI.PanelConfiguration
{
	/// <summary>
	/// Interaction logic for SelectSectionTemplateDialog.xaml
	/// </summary>
	public partial class SelectSectionTemplateDialog : Window
	{
		public SelectSectionTemplateDialog()
		{
			InitializeComponent();
			DataContext = new SelectSectionTemplateDialogVM{Data = this};
		}


		public SectionTemplateData[] Templates
		{
			get => ((SelectSectionTemplateDialogVM) DataContext).Templates;
			set => ((SelectSectionTemplateDialogVM) DataContext).Templates=value;
	}

		public SectionTemplateData SelectedTemplate
		{
			get => ((SelectSectionTemplateDialogVM)DataContext).SelectedTemplate;
			set => ((SelectSectionTemplateDialogVM)DataContext).SelectedTemplate = value;
		}
	
	}

	public class SelectSectionTemplateDialogVM : WindowVM
	{
		public SelectSectionTemplateDialogVM()
		{
			CancelAction=new ActionVM{MːDoAction = DoCancel};
		}

		public SectionTemplateData[] Templates { get => Fields.GetValue<SectionTemplateData[]>(); set => Fields.SetValue(value); }
		public SectionTemplateData SelectedTemplate { get => Fields.GetValue<SectionTemplateData>(); set => Fields.SetValue(value); }

		public ActionVM CancelAction { get; private set; }

		protected override void DoClose()
		{
			Data.DialogResult = true;
			base.DoClose();
		}

		private void DoCancel()
		{
			Data.DialogResult = false;
			Data.Close();
		}
	}
}
