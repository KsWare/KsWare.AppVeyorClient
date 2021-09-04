using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using JetBrains.Annotations;
using KsWare.AppVeyor.Api;
using KsWare.AppVeyor.Api.Contracts;
using KsWare.AppVeyorClient.UI.App;
using KsWare.AppVeyorClient.UI.ViewModels;
using KsWare.Presentation;
using KsWare.Presentation.ViewModelFramework;
using KsWare.Presentation.ViewModelFramework.Providers;
using Microsoft.Win32;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

namespace KsWare.AppVeyorClient.UI.PanelProjectSelector {

	public class ProjectSelectorVM : ObjectVM {

		private static readonly JsonSerializerSettings JsonSerializerSettings = new JsonSerializerSettings {
			ContractResolver = new CamelCasePropertyNamesContractResolver(),
		};

		private bool _watchTokenChanged;

		public ProjectSelectorVM() {
			RegisterChildren(() => this);
			Dispatcher.BeginInvoke(Initialize);
		}

		private Client Client => AppVM.Client;

		private async void Initialize() {
			try {
				if (!_watchTokenChanged) {
					_watchTokenChanged = true;
					Client.Base.TokenChanged += (s, e) => Initialize();
				}

				if (!Client.Base.HasToken) return;
				var projects = await AppVM.Client.Project.GetProjects();
				// projects.Sort((a, b) => -a.Updated.CompareTo(b.Updated));
				projects.Sort((a, b) =>
					-(a.Builds.FirstOrDefault()?.Updated ?? a.Updated).CompareTo(b.Builds.FirstOrDefault()?.Updated ??
						b.Updated));

				Projects.Clear();
				Projects.MːData = projects;
				Debug.WriteLine($"Number of projects: {Projects.Count}");
				((ErrorProvider) Metadata.ErrorProvider).ResetError();
			}
			catch (Exception ex) {
				((ErrorProvider) Metadata.ErrorProvider).SetError($"Could not receive projects.\n{ex.Message}");
				Debug.WriteLine($"Initialization error on {GetType().FullName}.\n{ex}");
				//Metadata.ErrorProvider
				if (Debugger.IsAttached) Debugger.Break();
			}
		}

		public ListVM<ProjectVM> Projects { get; [UsedImplicitly] private set; }

		[Hierarchy(HierarchyType.Reference)]
		public ProjectVM SelectedProject { get => Fields.GetValue<ProjectVM>(); set => Fields.SetValue(value); }

		public bool IsDropDownOpen { get => Fields.GetValue<bool>(); set => Fields.SetValue(value); }

		public ActionVM SelectProject { get; private set; }
		public ActionVM RefreshAction { get; private set; }

		public ActionVM ExportAction { get; private set; }

		public ActionVM ImportAction { get; private set; }

		private void DoSelectProject() {
			IsDropDownOpen = true;
		}

		private void DoRefresh() => Dispatcher.BeginInvoke(Initialize);

		private void DoExport() {
			var s=JsonConvert.SerializeObject(Projects.MːData, settings: JsonSerializerSettings);
			var dlg = new SaveFileDialog {
				Filter = "JSON-Files|*.json",
				FilterIndex = 1,
				Title = "Export projects..."
			};
			if (dlg.ShowDialog() != true) return;
			using (var w = new StreamWriter(new FileStream(dlg.FileName, FileMode.Create, FileAccess.Write),
				Encoding.UTF8))
				w.Write(s);
		}


		private void DoImport() {
			var dlg = new OpenFileDialog {
				Filter = "JSON-Files|*.json",
				FilterIndex = 1,
				Title = "Import projects..."
			};
			if (dlg.ShowDialog() != true) return;
			string s;
			using (var r = new StreamReader(new FileStream(dlg.FileName, FileMode.Open, FileAccess.Read),
				Encoding.UTF8))
				s = r.ReadToEnd();
			Projects.MːData = JsonConvert.DeserializeObject<GetProjectsResponse>(s);
		}

	}

}
