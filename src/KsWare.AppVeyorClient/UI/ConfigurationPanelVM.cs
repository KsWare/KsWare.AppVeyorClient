using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using JetBrains.Annotations;
using KsWare.AppVeyorClient.Api;
using KsWare.AppVeyorClient.Api.Contracts;
using KsWare.AppVeyorClient.Helpers;
using KsWare.AppVeyorClient.Shared;
using KsWare.AppVeyorClient.UI.Common;
using KsWare.Presentation;
using KsWare.Presentation.ViewFramework;
using KsWare.Presentation.ViewModelFramework;
using Microsoft.Win32;

namespace KsWare.AppVeyorClient.UI {

	public class ConfigurationPanelVM : ObjectVM {
		private YamlBlock _selectedBlock;

		public ConfigurationPanelVM() {
			RegisterChildren(()=>this);
		}

		private Client Client => AppVM.Client;

		public  ActionVM GetAction { get; [UsedImplicitly] private set; }
		public  ActionVM LoadAction { get; [UsedImplicitly] private set; }
		public  ActionVM SaveAction { get; [UsedImplicitly] private set; }
		public  ActionVM PostAction { get; [UsedImplicitly] private set; }

		public YamlTextBoxControllerVM YamlTextBoxController { get; [UsedImplicitly] private set; }
		public TextBoxControllerVM CodeTextBoxController { get; [UsedImplicitly] private set; }

		/// <summary>
		/// Gets the <see cref="ActionVM"/> to EditPs
		/// </summary>
		/// <seealso cref="DoEditPs"/>
		public ActionVM EditPsAction { get; [UsedImplicitly] private set; }

		/// <summary>
		/// Method for <see cref="EditPsAction"/>
		/// </summary>
		[UsedImplicitly]
		private void DoEditPs() {
			YamlTextBoxController.ExpandSelection();
			YamlTextBoxController.SetEnabled("Editor is open", false);
			_selectedBlock = YamlHelper.ExtractBlock(YamlTextBoxController.SelectedText);
			CodeTextBoxController.Text = _selectedBlock.Content;
		}

		/// <summary>
		/// Gets the <see cref="ActionVM"/> to ApplyEdit
		/// </summary>
		/// <seealso cref="DoApplyEdit"/>
		public ActionVM ApplyEditAction { get; [UsedImplicitly] private set; }

		/// <summary>
		/// Method for <see cref="ApplyEditAction"/>
		/// </summary>
		[UsedImplicitly]
		private void DoApplyEdit() {
			string s;
			switch (BlockFormat) {
				case "Block":
					s = YamlHelper.FormatBlock(CodeTextBoxController.Text, AppVeyorClient.UI.BlockFormat.Literal, _selectedBlock.Indent,
						_selectedBlock.Suffix);
					break;
				case "Split":
					s = YamlHelper.FormatBlock(CodeTextBoxController.Text, AppVeyorClient.UI.BlockFormat.None, _selectedBlock.Indent,
						_selectedBlock.Suffix);
					break;
				default:return;
			}

			YamlTextBoxController.SelectedText = s;
			YamlTextBoxController.SetEnabled("Editor is open", true);
		}

		/// <summary>
		/// Gets the <see cref="ActionVM"/> to CancelEdit
		/// </summary>
		/// <seealso cref="DoCancelEdit"/>
		public ActionVM CancelEditAction { get; [UsedImplicitly] private set; }

		public string BlockFormat { get => Fields.GetValue<string>(); set => Fields.SetValue(value); }

		[Hierarchy(HierarchyType.Reference)]
		public ProjectSelectorVM ProjectSelector { get => Fields.GetValue<ProjectSelectorVM>(); set => Fields.SetValue(value); }

		public string StatusBarText { get => Fields.GetValue<string>(); set => Fields.SetValue(value); }

		/// <summary>
		/// Method for <see cref="CancelEditAction"/>
		/// </summary>
		[UsedImplicitly]
		private void DoCancelEdit() {
			YamlTextBoxController.SetEnabled("Editor is open", true);
		}

		/// <summary>
		/// Method for <see cref="GetAction"/>
		/// </summary>
		[UsedImplicitly]
		private void DoGet() {
			StatusBarText = "Get project settings.";
			Client.Project.GetProjectSettingsYaml(ProjectSelector.SelectedProject.Data.AccountName, ProjectSelector.SelectedProject.Data.Slug)
				.ContinueWithUIDispatcher(task => {
				if (task.Exception != null) {
					StatusBarText = $"Get failed. {task.Exception.Message}";
					MessageBox.Show($"Get failed.\n\nDetails:\n{task.Exception.Message}", "Error", MessageBoxButton.OK,
						MessageBoxImage.Error);
				}
				else {
					StatusBarText = "Get done.";
					YamlTextBoxController.Text = task.Result;
				}
			});
		}

		/// <summary>
		/// Method for <see cref="LoadAction"/>
		/// </summary>
		[UsedImplicitly]
		private void DoLoad() {
			var dlg = new OpenFileDialog {Title = "Load configuration...", Filter = "YAML-File|*.yml", FilterIndex = 1};
			if (dlg.ShowDialog() != true) return;
			using (var reader = File.OpenText(dlg.FileName)) {
				YamlTextBoxController.Text = reader.ReadToEnd();
			}
		}

		/// <summary>
		/// Method for <see cref="SaveAction"/>
		/// </summary>
		[UsedImplicitly]
		private void DoSave() {
			var dlg = new SaveFileDialog {Title = "Save configuration as...", Filter = "YAML-File|*.yml", FilterIndex = 1};
			if (dlg.ShowDialog() != true) return;
			using (var writer = File.CreateText(dlg.FileName)) {
				writer.Write(YamlTextBoxController.Text);
				writer.Flush();
			}
		}

		/// <summary>
		/// Method for <see cref="PostAction"/>
		/// </summary>
		[UsedImplicitly]
		private void DoPost() {
			Client.Project.UpdateProjectSettingsYamlAsync(
				ProjectSelector.SelectedProject.Data.AccountName,
				ProjectSelector.SelectedProject.Data.Slug, 
				YamlTextBoxController.Text)
				.ContinueWithUIDispatcher(task => {
				if (task.Exception != null) {
					StatusBarText = $"Update failed. {task.Exception.InnerException?.Message}";
					MessageBox.Show($"Update failed.\n\nDetails:\n{task.Exception.InnerException?.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
				}
				else {
					StatusBarText="Update done.";
				}
			});
		}
	}

	// http://stackoverflow.com/questions/3790454/in-yaml-how-do-i-break-a-string-over-multiple-lines/21699210#21699210

	public enum BlockFormat {
		None,
		Folded,		// >	removes single newlines within the string (but adds one at the end, and converts double newlines to singles):
		Literal,	// |	turns every newline within the string into a literal newline, and adds one at the end
	}

	public enum ChompingIndicator {
		None,
		Clip,       //		"clip": keep the line feed, remove the trailing blank lines. (TrimEnd)
		Strip,      // -	"strip": remove the line feed, remove the trailing blank lines.	(Trim)
		Keep,       // +	"keep": keep the line feed, keep trailing blank lines.	
	}

}
