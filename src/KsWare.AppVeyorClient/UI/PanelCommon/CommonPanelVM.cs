using System.Diagnostics;
using JetBrains.Annotations;
using KsWare.AppVeyorClient.UI.App;
using KsWare.AppVeyorClient.UI.Common;
using KsWare.AppVeyorClient.UI.ViewModels;
using KsWare.Presentation.ViewModelFramework;

namespace KsWare.AppVeyorClient.UI.PanelCommon {

	public class CommonPanelVM : ObjectVM,IHaveTitle {

		public CommonPanelVM() {
			RegisterChildren(() => this);
		}

		public SettingsVM Settings => AppVM.Current.Settings;

		/// <summary>
		/// Gets the <see cref="ActionVM"/> to HelpUrl
		/// </summary>
		/// <seealso cref="DoHelpUrl"/>
		public ActionVM HelpUrlAction { get; [UsedImplicitly] private set; }

		/// <summary>
		/// Method for <see cref="HelpUrlAction"/>
		/// </summary>
		[UsedImplicitly]
		private void DoHelpUrl(object parameter) {
			string url = (string) parameter;
			Process.Start(url);
		}

		/// <inheritdoc />
		public string Title => "Settings";
	}

}
