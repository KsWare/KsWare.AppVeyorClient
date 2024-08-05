using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using JetBrains.Annotations;
using KsWare.AppVeyor.Api;
using KsWare.AppVeyor.Api.Contracts;
using KsWare.AppVeyorClient.Shared;
using KsWare.AppVeyorClient.Shared.AvalonEditExtension;
using KsWare.AppVeyorClient.UI.App;
using KsWare.AppVeyorClient.UI.Common;
using KsWare.AppVeyorClient.UI.PanelConfiguration;
using KsWare.AppVeyorClient.UI.PanelProjectEnvironmentVariables;
using KsWare.AppVeyorClient.UI.PanelProjectSelector;
using KsWare.AppVeyorClient.UI.PanelYamlCodeEditor;
using KsWare.Presentation;
using KsWare.Presentation.ViewModelFramework;
using Microsoft.Win32;

namespace KsWare.AppVeyorClient.UI.PanelGlobalYaml {

	public class GlobalYamlVM: YamlCodeEditorVM,IHaveTitle {
		
		public GlobalYamlVM() {
			RegisterChildren(() => this);
		}

		public override string Title => "Global.Yaml";
		protected override string NavigationTemplate => @"Data\Navigation-Global.txt";

		private Client Client => AppVM.Client;
		
		public override void DoGet() {
//			if (AccountSelector.SelectedAccount == null) return;
			StatusBarText = "Get global.yaml";
//			var d = AccountSelector.SelectedAccount.Data;
//			var accountName = d.AccountName;
			var accountName = "KsWare"; //TODO
			Client.Common.GetGlobalYamlAsync(accountName).ContinueWithUIDispatcher<string>(task => {
				if (task.Exception != null) {
					StatusBarText = $"Get failed. {task.Exception.Message}";
					MessageBox.Show($"Get failed.\n\nDetails:\n{task.Exception.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
					return;
				}
				StatusBarText = "Get done.";
				YamlEditorController.Text = task.Result;
				YamlEditorController.ResetHasChanges();
				EditorProjectName = $"global.yaml ({accountName})";
			});
		}

		public override void DoLoad() {
			if (YamlEditorController.Data.IsModified) {
				if (MessageBox.Show("Discard current changes?", "Open File", MessageBoxButton.OKCancel) != MessageBoxResult.OK) return;
			}
			var dlg = new OpenFileDialog {Title = "Load global.yaml...", Filter = "Global.yaml|global*.y*ml|YAML-File|*.y*ml", FilterIndex = 1};
			if (dlg.ShowDialog() != true) return;
			LoadFile(dlg.FileName);
		}

		protected override void DoSave() {
			var projectName = "Global";
			var dlg = new SaveFileDialog {
				Title = "Save global.yaml as...",
				Filter = "Global.yaml|global*.y*ml|YAML-File|*.yml",
				FilterIndex = 1,
				FileName = $"{projectName}.{DateTime.Now:yyyyMMddHHmmss}.AutoSave.yml"
			};
			if (dlg.ShowDialog() != true) return;
			using var writer = File.CreateText(dlg.FileName);
			writer.Write(YamlEditorController.Text);
			writer.Flush();
		}

		protected override void DoPost() {
			Client.Common.PutGlobalYamlAsync(YamlEditorController.Text, null).ContinueWithUIDispatcher(task => {
				if (task.Exception != null) {
					StatusBarText = $"Update failed. {task.Exception.Message}";
					MessageBox.Show($"Update failed.\n\nDetails:\n{task.Exception.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
				}
				else {
					if(task.Result.IsValid)
						StatusBarText = "Update done.";
					else {
						StatusBarText = $"Validation failed. {task.Result.ErrorMessage}";
					}
				}
			});
		}

		protected override void DoValidateYaml() {
			StatusBarText = "Validating global.yaml...";
			var accountName = "KsWare"; //TODO
			Client.Project.ValidateYaml(YamlEditorController.Text, accountName)
				.ContinueWithUIDispatcher(OnFinishValidateYaml);
		}

		protected override void DoOpenAppVeyor() {
//			if (AccountSelector.SelectedAccount == null) {
				Process.Start(new ProcessStartInfo("https://ci.appveyor.com/tools/global-yaml") { UseShellExecute = true });
//				return;
//			}

//			var d = ProjectSelector.SelectedProject.Data;
//			var page = ""; //
//			var url = $"https://ci.appveyor.com/project/{d.AccountName}/{d.Slug}/{page}";
//			var psi = new ProcessStartInfo(url) { UseShellExecute = true };
//			Process.Start(psi);
		}

		protected override void DoOpenGitHub() { /* not supported */ }
	}

}
