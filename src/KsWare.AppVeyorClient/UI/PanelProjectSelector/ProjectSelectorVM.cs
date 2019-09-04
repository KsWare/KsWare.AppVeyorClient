using System;
using System.Diagnostics;
using JetBrains.Annotations;
using KsWare.AppVeyorClient.Api;
using KsWare.AppVeyorClient.UI.App;
using KsWare.AppVeyorClient.UI.ViewModels;
using KsWare.Presentation;
using KsWare.Presentation.ViewModelFramework;
using KsWare.Presentation.ViewModelFramework.Providers;

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
				((ErrorProvider)Metadata.ErrorProvider).ResetError();
			}
			catch (Exception ex) {
				((ErrorProvider) Metadata.ErrorProvider).SetError($"Could not receive projects.\n{ex.Message}");
				Debug.WriteLine($"Initialization error on {GetType().FullName}.\n{ex}");
				//Metadata.ErrorProvider
				if(Debugger.IsAttached) Debugger.Break();
			}
		}

		public ListVM<ProjectVM> Projects { get; [UsedImplicitly] private set; }

		[Hierarchy(HierarchyType.Reference)]
		public ProjectVM SelectedProject { get => Fields.GetValue<ProjectVM>(); set => Fields.SetValue(value); }
		public bool IsDropDownOpen { get => Fields.GetValue<bool>(); set => Fields.SetValue(value); }

		public ActionVM SelectProject { get; private set; }

		private void DoSelectProject()
		{
			IsDropDownOpen = true;
		}
	}

}
