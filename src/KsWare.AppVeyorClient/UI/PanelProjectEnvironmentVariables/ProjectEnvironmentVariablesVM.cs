using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using JetBrains.Annotations;
using KsWare.AppVeyor.Api;
using KsWare.AppVeyor.Api.Contracts;
using KsWare.AppVeyorClient.Shared;
using KsWare.AppVeyorClient.UI.App;
using KsWare.AppVeyorClient.UI.Common;
using KsWare.AppVeyorClient.UI.PanelProjectSelector;
using KsWare.Presentation;
using KsWare.Presentation.ViewModelFramework;

namespace KsWare.AppVeyorClient.UI.PanelProjectEnvironmentVariables {

	public class ProjectEnvironmentVariablesVM: ObjectVM,IHaveTitle {

		private static readonly Dictionary<string, string> _encryptedCache = new Dictionary<string, string>();

		public ProjectEnvironmentVariablesVM() {
			RegisterChildren(() => this);
		}

		public string Title => "Project Environment Variables";

		private Client Client => AppVM.Client;

		public  ActionVM GetAction { get; [UsedImplicitly] private set; }
		public  ActionVM LoadAction { get; [UsedImplicitly] private set; }
		public  ActionVM SaveAction { get; [UsedImplicitly] private set; }
		public  ActionVM PostAction { get; [UsedImplicitly] private set; }

		/// <summary>
		/// Gets the <see cref="ActionVM"/> to Add
		/// </summary>
		/// <seealso cref="DoAdd"/>
		public ActionVM AddAction { get; [UsedImplicitly] private set; }

		/// <summary>
		/// Method for <see cref="AddAction"/>
		/// </summary>
		[UsedImplicitly]
		private void DoAdd() {
			Variables.Add(new EnvVariableVM{Data = new NameValueSecurePair()});
		}

		public string PlainText { get => Fields.GetValue<string>(); set => Fields.SetValue(value); }

		[Hierarchy(HierarchyType.Reference)]
		public ProjectSelectorVM ProjectSelector { get => Fields.GetValue<ProjectSelectorVM>(); set => Fields.SetValue(value); }

		public ListVM<EnvVariableVM> Variables { get; [UsedImplicitly]private set; }
		
		private void DoGet() {
			if (ProjectSelector.SelectedProject == null) return;
//			StatusBarText = "Get project environment variables.";
			var d = ProjectSelector.SelectedProject.Data;
			Client.Project
				.GetProjectEnvironmentVariables(d.AccountName, d.Slug)
				.ContinueWithUIDispatcher(task => {
					if (task.Exception != null) {
//						StatusBarText = $"Get failed. {task.Exception.Message}";
						MessageBox.Show($"Get failed.\n\nDetails:\n{task.Exception.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
						return;
					}

//					StatusBarText              = "Get done.";
					var sb = new StringBuilder();
					var envvars = task.Result;
					// "value":"very-secret-key-in-clear-text"
					foreach (var var in envvars) {
						sb.AppendLine($"{var}"); 
					}
					PlainText = sb.ToString();
					Variables.MːData = envvars.ToList();
				});
		}

		private void DoLoad() { }

		private void DoSave() { }

		private void DoPost() {
			if (ProjectSelector.SelectedProject == null) return;
			if (Variables.Count==0) return;
			PlainText = string.Join("\r\n", Variables.Where(v=>!string.IsNullOrWhiteSpace(v.Name)).Select(v => v.Data.ToString())); // TODO remove use of plaintext
			// if (string.IsNullOrWhiteSpace(PlainText)) return;

			var configurationPanel = ((MainWindowVM)Parent).ConfigurationPanel;
			if (configurationPanel.YamlEditorController.Data.Text.Length > 0 && configurationPanel.YamlEditorController.Data.IsModified) {
				if (MessageBox.Show("Discard changes in project configuration?", "Send Environment Variables", MessageBoxButton.OKCancel, MessageBoxImage.Warning, MessageBoxResult.Cancel) != MessageBoxResult.OK) return;
				//TODO now Configuration has inconsistent data! clear/lock the data
			}

			Task.Run(PostAsync).ContinueWithUIDispatcher(task => {
				if (task.Exception != null) {
					// StatusBarText = $"Update failed. {task.Exception.Message}";
					MessageBox.Show($"Update failed.\n\nDetails:\n{task.Exception.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
				}
				else {
					if (configurationPanel.YamlEditorController.Data.Text.Length > 0) {
						configurationPanel.DoGet();
					}
					// StatusBarText = "Update done.";
				}
			});
		}

		private async Task PostAsync() {
			var d = ProjectSelector.SelectedProject.Data;
			var lines     = PlainText.Split(new[] {"\r\n", "\n", "\r"}, StringSplitOptions.None);
			var variables = new List<NameValueSecurePair>();
			foreach (var line in lines) {
				if (string.IsNullOrWhiteSpace(line)) continue;
				var tokens      = line.Split(new[] {":"}, 2, StringSplitOptions.None);
				var name        = tokens[0].Trim();
				var value       = tokens[1].Trim();
				var isEncrypted = value.StartsWith("secure!");
				//encrypt the value! => "value":"very-secret-key-encrypted"
				if (isEncrypted) {
					value = value.Substring(7).Trim();
					if (!_encryptedCache.TryGetValue(d.AccountName+value, out var encrypted)) {
						encrypted = await Client.Encrypt(value, d.AccountName);
						_encryptedCache.Add(d.AccountName+value, encrypted);
					}
					value = encrypted;
				}
				variables.Add(new NameValueSecurePair(name, value, isEncrypted));
			}

			await Client.Project.UpdateProjectEnvironmentVariables(d.AccountName, d.Slug, variables).ConfigureAwait(true);
		}
	}

}
