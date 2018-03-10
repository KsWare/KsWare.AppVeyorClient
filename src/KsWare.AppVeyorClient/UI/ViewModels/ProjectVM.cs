using KsWare.AppVeyorClient.Api.Contracts;
using KsWare.Presentation.Core.Providers;
using KsWare.Presentation.ViewModelFramework;

namespace KsWare.AppVeyorClient.UI.ViewModels {

	public class ProjectVM : DataVM<ProjectData> {

		public ProjectVM() {
			RegisterChildren(() => this);
		}

		protected override void OnDataChanged(DataChangedEventArgs e) {
			base.OnDataChanged(e);

			OnPropertyChanged(nameof(DisplayName));
		}

		public string DisplayName {
			get {
				if (Data == null) return null;
				var tags = !string.IsNullOrWhiteSpace(Data.Tags) ? $" ({Data.Tags})" : "";
				return Data.Name + tags ;
			}
		}
	}

}