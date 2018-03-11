using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using JetBrains.Annotations;
using KsWare.AppVeyorClient.Api;
using KsWare.AppVeyorClient.Api.Contracts;
using KsWare.AppVeyorClient.UI.App;
using KsWare.AppVeyorClient.UI.PanelApiTester;
using KsWare.AppVeyorClient.UI.PanelCommon;
using KsWare.AppVeyorClient.UI.PanelConfiguration;
using KsWare.AppVeyorClient.UI.PanelProjectEnvironmentVariables;
using KsWare.AppVeyorClient.UI.PanelProjectSelector;
using KsWare.Presentation.ViewModelFramework;
using Newtonsoft.Json;

namespace KsWare.AppVeyorClient.UI {

	public class MainWindowVM : WindowVM {

		public MainWindowVM() {
			RegisterChildren(()=>this);
			AppVM.LoadToken(); // TODO load after UI loaded
			AppVM.InitFileStore(); // TODO load after UI loaded
			
			ConfigurationPanel.ProjectSelector = ProjectSelector;
			ProjectEnvironmentVariablesPanel.ProjectSelector = ProjectSelector;
		}

		private Client Client => AppVM.Client;

		public ProjectSelectorVM ProjectSelector { get; [UsedImplicitly] private set; }

		public CommonPanelVM CommonPanel { get; [UsedImplicitly] private set; }

		public ProjectEnvironmentVariablesVM ProjectEnvironmentVariablesPanel { get; [UsedImplicitly] private set; }

		public ConfigurationPanelVM ConfigurationPanel { get; [UsedImplicitly] private set; }

		public ApiTesterPanelVM ApiTesterPanel { get; [UsedImplicitly] private set; }


		/// <summary>
		/// Gets the <see cref="ActionVM"/> to NewWindow
		/// </summary>
		/// <seealso cref="DoNewWindow"/>
		public ActionVM NewWindowAction { get; [UsedImplicitly] private set; }

		

		/// <summary>
		/// Method for <see cref="NewWindowAction"/>
		/// </summary>
		[UsedImplicitly]
		private void DoNewWindow() {
			new MainWindowVM().Show();
		}
	}
}
