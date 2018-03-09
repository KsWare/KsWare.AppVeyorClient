using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using JetBrains.Annotations;
using KsWare.AppVeyorClient.Api;
using KsWare.AppVeyorClient.Api.Contracts;
using KsWare.AppVeyorClient.Shared;
using KsWare.Presentation;
using KsWare.Presentation.ViewModelFramework;

namespace KsWare.AppVeyorClient.UI {

	public class ProjectEnvironmentVariablesVM: ObjectVM {

		public ProjectEnvironmentVariablesVM() {
			RegisterChildren(()=>this);
		}

		private Client Client => AppVM.Client;

		public  ActionVM GetAction { get; [UsedImplicitly] private set; }
		public  ActionVM LoadAction { get; [UsedImplicitly] private set; }
		public  ActionVM SaveAction { get; [UsedImplicitly] private set; }
		public  ActionVM PostAction { get; [UsedImplicitly] private set; }

		public string PlainText { get => Fields.GetValue<string>(); set => Fields.SetValue(value); }

		[Hierarchy(HierarchyType.Reference)]
		public ProjectSelectorVM ProjectSelector { get => Fields.GetValue<ProjectSelectorVM>(); set => Fields.SetValue(value); }

		private void DoGet() {
			if(ProjectSelector.SelectedProject==null) return;
//			StatusBarText = "Get project environment variables.";
			Client.Project
				.GetProjectEnvironmentVariables(ProjectSelector.SelectedProject.Data.AccountName, ProjectSelector.SelectedProject.Data.Slug)
				.ContinueWithUIDispatcher(task => {
					if (task.Exception != null) {
//						StatusBarText = $"Get failed. {task.Exception.Message}";
						MessageBox.Show($"Get failed.\n\nDetails:\n{task.Exception.Message}", "Error", MessageBoxButton.OK,
							MessageBoxImage.Error);
					}
					else {
//						StatusBarText              = "Get done.";
						var sb = new StringBuilder();
						var envvars = task.Result;
						foreach (var var in envvars) {
							sb.AppendLine($"{var}");
						}
						PlainText = sb.ToString();
					}
				});
		}

		private void DoLoad() { }

		private void DoSave() { }

		private void DoPost() {
			if (ProjectSelector.SelectedProject == null) return;

			if (string.IsNullOrWhiteSpace(PlainText)) return;
			var lines     = PlainText.Split(new[] {"\r\n", "\n", "\r"}, StringSplitOptions.None);
			var variables = new List<NameValueSecurePair>();
			foreach (var line in lines) {
				if (string.IsNullOrWhiteSpace(line)) continue;
				var tokens      = line.Split(new[] {":"}, 2, StringSplitOptions.None);
				var name        = tokens[0].Trim();
				var value       = tokens[1].Trim();
				var isEncrypted = value.StartsWith("secure!");
				value = isEncrypted ? value.Substring(7).Trim() : value;
				variables.Add(new NameValueSecurePair(name, value, isEncrypted));
			}
			Client.Project
				.UpdateProjectEnvironmentVariables(ProjectSelector.SelectedProject.Data.AccountName,
					ProjectSelector.SelectedProject.Data.Slug, variables).ContinueWithUIDispatcher(task => {
					if (task.Exception != null) {
//						StatusBarText = $"Update failed. {task.Exception.Message}";
						MessageBox.Show($"Update failed.\n\nDetails:\n{task.Exception.Message}", "Error", MessageBoxButton.OK,
							MessageBoxImage.Error);
					}
					else {
//						StatusBarText = "Update done.";
					}
				});
		}

	
	}
}
