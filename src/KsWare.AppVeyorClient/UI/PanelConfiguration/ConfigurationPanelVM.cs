using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Threading;
using JetBrains.Annotations;
using KsWare.AppVeyorClient.Api;
using KsWare.AppVeyorClient.Helpers;
using KsWare.AppVeyorClient.Shared;
using KsWare.AppVeyorClient.Shared.AvalonEditExtension;
using KsWare.AppVeyorClient.UI.App;
using KsWare.AppVeyorClient.UI.PanelProjectSelector;
using KsWare.AppVeyorClient.UI.PanelSearch;
using KsWare.AppVeyorClient.UI.ViewModels;
using KsWare.Presentation;
using KsWare.Presentation.ViewModelFramework;
using Microsoft.Win32;

namespace KsWare.AppVeyorClient.UI.PanelConfiguration {

	public class ConfigurationPanelVM : ObjectVM {

		private YamlBlock _selectedBlock;

		public ConfigurationPanelVM() {
			RegisterChildren(()=>this);

			SearchPanel.Editor = YamlEditorController;

			FillNavigation();
			Fields[nameof(SelectedNavigationItem)].ValueChangedEvent.add = AtNavigationKeyChanged;
			CloseCodeEditor();
		}

		public ListVM<NavigationItemVM> NavigationItems { get; [UsedImplicitly] private set; }

		private void FillNavigation() {
			var lines = File.ReadAllLines("Data\\Navigation.txt");
			foreach (var line in lines) Add(line);

			void Add(string key) {
				key = key.Trim(); 
				var pattern = @"(?mnx-is)^" + Regex.Escape(key) + @"(\x20|\r\n|\n)";
				NavigationItems.Add(new NavigationItemVM {
					DisplayName = key.StartsWith("-- ") ? key.Substring(3) : "  "+key,
					RegexPattern = key.StartsWith("-- ") ? null : pattern,
					Regex = new Regex(pattern,RegexOptions.Compiled)
				});
			}
		}

		private Client Client => AppVM.Client;

		public  ActionVM GetAction { get; [UsedImplicitly] private set; }
		public  ActionVM LoadAction { get; [UsedImplicitly] private set; }
		public  ActionVM SaveAction { get; [UsedImplicitly] private set; }
		public  ActionVM PostAction { get; [UsedImplicitly] private set; }

		public AppVeyorYamlEditorControllerVM YamlEditorController { get; [UsedImplicitly] private set; }
		public TextEditorControllerVM CodeEditorController { get; [UsedImplicitly] private set; }

		/// <summary>
		/// Gets the <see cref="ActionVM"/> to EditCode
		/// </summary>
		/// <seealso cref="DoEditCode"/>
		public ActionVM EditCodeAction { get; [UsedImplicitly] private set; }

		/// <summary>
		/// Method for <see cref="EditCodeAction"/>
		/// </summary>
		[UsedImplicitly]
		private void DoEditCode() {
			YamlEditorController.ExpandCodeBlock();
			YamlEditorController.SetEnabled("YAML Editor is open", false);
			YamlEditorHeight=new GridLength(50);
			CodeEditorHeight=new GridLength(100,GridUnitType.Star);
			_selectedBlock = YamlHelper.ExtractBlock(YamlEditorController.SelectedText);
			CodeEditorController.Text = _selectedBlock.Content;
			CodeEditorController.SetEnabled("Code Editor is open", true);
			Dispatcher.BeginInvoke(DispatcherPriority.Background, () => YamlEditorController.Data.ScrollToLine(YamlEditorController.Data.TextArea.Caret.Line));
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
					s = YamlHelper.FormatBlock(CodeEditorController.Text, PanelConfiguration.BlockFormat.Literal, _selectedBlock.Indent,
						_selectedBlock.Suffix);
					break;
				case "Split":
					s = YamlHelper.FormatBlock(CodeEditorController.Text, PanelConfiguration.BlockFormat.None, _selectedBlock.Indent,
						_selectedBlock.Suffix);
					break;
				default:return;
			}

			YamlEditorController.SelectedText = s;
			CodeEditorController.Text = "";
			CloseCodeEditor();
		}

		private void CloseCodeEditor() {
			YamlEditorController.SetEnabled("YAML Editor is open", true);
			YamlEditorHeight = new GridLength(100,GridUnitType.Star);

			CodeEditorController.SetEnabled("Code Editor is open", false);
			CodeEditorHeight = new GridLength(0, GridUnitType.Star);
		}

		private void AtNavigationKeyChanged(object sender, ValueChangedEventArgs e) {
			if(SelectedNavigationItem==null) return;

			var match = SelectedNavigationItem.Regex.Match(YamlEditorController.Data.Text);
			SelectedNavigationItem.ExistsInDocument = match.Success;
			if (!match.Success) return;
			YamlEditorController.Data.Select(match.Index+match.Length,0);
			YamlEditorController.Data.ScrollTo(YamlEditorController.Data.TextArea.Caret.Line,0);
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

		public SearchPanelVM SearchPanel { get; private set; }

		[Hierarchy(HierarchyType.Reference)]
		public NavigationItemVM SelectedNavigationItem { get => Fields.GetValue<NavigationItemVM>(); set => Fields.SetValue(value); }

		public GridLength YamlEditorHeight { get => Fields.GetValue<GridLength>(); set => Fields.SetValue(value); }
		public GridLength CodeEditorHeight { get => Fields.GetValue<GridLength>(); set => Fields.SetValue(value); }

		/// <summary>
		/// Method for <see cref="CancelEditAction"/>
		/// </summary>
		[UsedImplicitly]
		private void DoCancelEdit() {
			CodeEditorController.Text = "";
			CloseCodeEditor();
		}

		/// <summary>
		/// Method for <see cref="GetAction"/>
		/// </summary>
		[UsedImplicitly]
		private void DoGet() {
			if (ProjectSelector.SelectedProject == null) {
				StatusBarText = "WARNING: No project selected!";
				return;
			}
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
					YamlEditorController.Text = task.Result;
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
				YamlEditorController.Text = reader.ReadToEnd();
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
				writer.Write(YamlEditorController.Text);
				writer.Flush();
			}
		}

		/// <summary>
		/// Method for <see cref="PostAction"/>
		/// </summary>
		[UsedImplicitly]
		private void DoPost() {
			if (ProjectSelector.SelectedProject == null) {
				StatusBarText = "WARNING: No project selected!";
				return;
			}
			StatusBarText = "Send project settings...";
			Client.Project.UpdateProjectSettingsYamlAsync(
				ProjectSelector.SelectedProject.Data.AccountName,
				ProjectSelector.SelectedProject.Data.Slug, 
				YamlEditorController.Text)
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
