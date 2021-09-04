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
using System.Windows.Threading;
using JetBrains.Annotations;
using KsWare.AppVeyor.Api;
using KsWare.AppVeyorClient.UI.App;
using KsWare.AppVeyorClient.UI.Common;
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

			Pages = new IHaveTitle[] {
				ConfigurationPanel,
				ProjectEnvironmentVariablesPanel,
				CommonPanel
			};
			CurrentPageIndex = 0;

			ConfigurationPanel.ProjectSelector = ProjectSelector;
			ProjectEnvironmentVariablesPanel.ProjectSelector = ProjectSelector;

			var args = Environment.GetCommandLineArgs();
			if (args.Length > 1 && File.Exists(args.Last()))
				Application.Current.Dispatcher.Invoke(DispatcherPriority.ApplicationIdle, new Action(() => {
					Select(ConfigurationPanel);
					ConfigurationPanel.LoadFile(args.Last());
				}));
		}

		public int CurrentPageIndex { get => Fields.GetValue<int>(); set => Fields.SetValue(value); }

		public IHaveTitle[] Pages { get; }

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

		/// <summary>
		/// Gets the <see cref="ActionVM"/> to NewProject
		/// </summary>
		/// <seealso cref="DoNewProject"/>
		public ActionVM NewProjectAction { get; [UsedImplicitly] private set; }

		/// <summary>
		/// Method for <see cref="NewProjectAction"/>
		/// </summary>
		[UsedImplicitly]
		private void DoNewProject() {
			Process.Start(new ProcessStartInfo {
				FileName = "https://ci.appveyor.com/projects/new",
				UseShellExecute = true
			});
		}

		/// <summary>
		/// Gets the <see cref="ActionVM"/> to AppExit
		/// </summary>
		/// <seealso cref="DoAppExit"/>
		public ActionVM AppExitAction { get; [UsedImplicitly] private set; }

		/// <summary>
		/// Method for <see cref="AppExitAction"/>
		/// </summary>
		[UsedImplicitly]
		private void DoAppExit() {
			Application.Current.Shutdown();
		}

		/// <summary>
		/// Gets the <see cref="ActionVM"/> to About
		/// </summary>
		/// <seealso cref="DoAbout"/>
		public ActionVM AboutAction { get; [UsedImplicitly] private set; }

		/// <summary>
		/// Method for <see cref="AboutAction"/>
		/// </summary>
		[UsedImplicitly]
		private void DoAbout() {
			new AboutWindow() { Owner = Application.Current.MainWindow }.ShowDialog();
		}

		/// <summary>
		/// Gets the <see cref="ActionVM"/> to NewFile
		/// </summary>
		/// <seealso cref="DoNewFile"/>
		public ActionVM NewFileAction { get; [UsedImplicitly] private set; }

		/// <summary>
		/// Method for <see cref="NewFileAction"/>
		/// </summary>
		[UsedImplicitly]
		private void DoNewFile() {
			Select(ConfigurationPanel);
			ConfigurationPanel.DoNewFile();
		}

		private void Select(IObjectVM panelViewModel) {
			// TODO workaround as long tabs are not loaded in MVVM manner
			switch (panelViewModel) {
				case ConfigurationPanelVM: CurrentPageIndex = 0; break;
				case ProjectEnvironmentVariablesVM: CurrentPageIndex = 1; break;
				case CommonPanelVM: CurrentPageIndex = 4; break;
			}
		}

		/// <summary>
		/// Gets the <see cref="ActionVM"/> to OpenFile
		/// </summary>
		/// <seealso cref="DoOpenFile"/>
		public ActionVM OpenFileAction { get; [UsedImplicitly] private set; }

		/// <summary>
		/// Method for <see cref="OpenFileAction"/>
		/// </summary>
		[UsedImplicitly]
		private void DoOpenFile() {
			ConfigurationPanel.DoLoad();
		}

		/// <summary>
		/// Gets the <see cref="ActionVM"/> to CommandLineHelp
		/// </summary>
		/// <seealso cref="DoCommandLineHelp"/>
		public ActionVM CommandLineHelpAction { get; [UsedImplicitly] private set; } 

		/// <summary>
		/// Method for <see cref="CommandLineHelpAction"/>
		/// </summary>
		[UsedImplicitly]
		private void DoCommandLineHelp() {
			new CommandLineHelpWindow().ShowDialog();
		}
	}
}
