using System;
using System.Diagnostics;
using JetBrains.Annotations;
using KsWare.AppVeyorClient.Api;
using KsWare.AppVeyorClient.UI.App;
using KsWare.AppVeyorClient.UI.ViewModels;
using KsWare.Presentation;
using KsWare.Presentation.ViewModelFramework;

namespace KsWare.AppVeyorClient.UI.PanelProjectSelector {

	public class ProjectSelectorVM : ObjectVM {
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
				var projects=await AppVM.Client.Project.GetProjects();
				Projects.MːData=projects;
			}
			catch (Exception ex) {
				Debug.WriteLine($"Initialization error on {GetType().FullName}.\n{ex}");
				if(Debugger.IsAttached) Debugger.Break();
			}
		}

		public ListVM<ProjectVM> Projects { get; [UsedImplicitly] private set; }

		[Hierarchy(HierarchyType.Reference)]
		public ProjectVM SelectedProject { get => Fields.GetValue<ProjectVM>(); set => Fields.SetValue(value); }
	}

}
