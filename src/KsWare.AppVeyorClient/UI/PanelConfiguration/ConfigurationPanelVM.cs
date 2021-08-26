using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Document;
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
using BindingMode = System.Windows.Data.BindingMode;

namespace KsWare.AppVeyorClient.UI.PanelConfiguration {

	public class ConfigurationPanelVM : ObjectVM {

		private YamlBlock _selectedBlock;
		private List<SectionTemplateData>_sectionTemplates = new List<SectionTemplateData>();
		private string _editorProjectName;

		public ConfigurationPanelVM() {
			RegisterChildren(()=>this);

			SearchPanel.Editor = YamlEditorController;

			FillSectionTemplates();
			FillNavigation();
			Fields[nameof(SelectedNavigationItem)].ValueChangedEvent.add = AtNavigationKeyChanged;
			Fields[nameof(IsNavigationDropDownOpen)].ValueChangedEvent.add = AtIsNavigationDropDownOpenChanged;
			CloseCodeEditor();

			Fields[nameof(IsModified)].SetBinding(new FieldBinding(YamlEditorController.Fields[nameof(YamlEditorController.IsModified)],BindingMode.OneWay));
//			YamlEditorController.Fields[nameof(YamlEditorController.IsModified)].ValueChangedEvent.add=
//				(s, e) => IsModified = YamlEditorController.IsModified;
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
					IsGroupTitle = key.StartsWith("-- "),
					RegexPattern = key.StartsWith("-- ") ? null : pattern,
					Regex = new Regex(pattern,RegexOptions.Compiled),
					HasTemplate = _sectionTemplates.Any(t=>t.Key==key)
				});
			}
		}

		private void FillSectionTemplates() {
			_sectionTemplates.Clear();

			var lines = File.ReadAllLines("Data\\Templates.txt");

			var templateString = new StringBuilder();
			var template = new SectionTemplateData();
			_sectionTemplates.Add(template);
			foreach (var line in lines) {
				if (string.IsNullOrWhiteSpace(line)) {
					template.Content = templateString.ToString().TrimEnd();

					templateString = new StringBuilder();
					template = new SectionTemplateData();
					_sectionTemplates.Add(template);
					continue;
				}

				templateString.AppendLine(line);
				if (template.Key == null) {
					var match = Regex.Match(line, @"^(?<key>[a-z_0-9]+:)", RegexOptions.Compiled | RegexOptions.IgnoreCase);
					if (match.Success) { template.Key = match.Groups["key"].Value; }
				}
			}

			template.Content = templateString.ToString().TrimEnd();

			// clear empty keys
			for (int i = 0; i < _sectionTemplates.Count; i++) {
				if(string.IsNullOrWhiteSpace(_sectionTemplates[i].Key))
					_sectionTemplates.RemoveAt(i);
			}
		}

		private Client Client => AppVM.Client;

		public  ActionVM GetAction { get; [UsedImplicitly] private set; }
		public  ActionVM LoadAction { get; [UsedImplicitly] private set; }
		public  ActionVM SaveAction { get; [UsedImplicitly] private set; }
		public  ActionVM PostAction { get; [UsedImplicitly] private set; }
		public  ActionVM InsertTemplate { get; [UsedImplicitly] private set; }

		public string EditorProjectName { get => Fields.GetValue<string>(); private set => Fields.SetValue(value); }

		private void DoInsertTemplate() {
			if(SelectedNavigationItem==null) return;
			var templates = _sectionTemplates.Where(t => t.Key == SelectedNavigationItem.DisplayName.Trim()).ToArray();

			SectionTemplateData selectedTemplate = null;
			if (Keyboard.Modifiers == ModifierKeys.Control && templates.Length > 0) {
				selectedTemplate = templates.First();
			}
			else {
				var dlg = new SelectSectionTemplateDialog {
					Templates = templates,
					SelectedTemplate = templates.FirstOrDefault()
				};
				if (dlg.ShowDialog()!=true) return;
				selectedTemplate = dlg.SelectedTemplate;
			}

			if(selectedTemplate==null) return;

			var navItemIndex = NavigationItems.IndexOf(SelectedNavigationItem);
			for (int i = navItemIndex; i < NavigationItems.Count; i++) {
				var match = NavigationItems[i].Regex.Match(YamlEditorController.Data.Text);
				SelectedNavigationItem.ExistsInDocument = match.Success;
				if (!match.Success) continue;

				YamlEditorController.Data.Select(match.Index, 0);
				YamlEditorController.Data.ScrollTo(YamlEditorController.Data.TextArea.Caret.Line, 0);
				YamlEditorController.SelectedText = selectedTemplate.Content + "\r\n";
				SelectedNavigationItem.ExistsInDocument = true;
				return;
			}
			YamlEditorController.Data.Select(YamlEditorController.Data.Document.TextLength, 0);
			var pos = new DocumentPosition(YamlEditorController.Data, YamlEditorController.Data.Document.TextLength);
			if(pos.LineCharIndex>0)
				YamlEditorController.SelectedText = "\r\n" + selectedTemplate.Content;
			else 
				YamlEditorController.SelectedText = selectedTemplate.Content + "\r\n";
			SelectedNavigationItem.ExistsInDocument = true;
			//TODO optimize line breaks
		}

		public AppVeyorYamlEditorControllerVM YamlEditorController { get; [UsedImplicitly] private set; }
		public TextEditorControllerVM CodeEditorController { get; [UsedImplicitly] private set; }

		/// <summary>
		/// Gets the <see cref="ActionVM"/> to EditCode
		/// </summary>
		/// <seealso cref="DoEditScript"/>
		public ActionVM EditScriptAction { get; [UsedImplicitly] private set; }
		public ActionVM OpenAppVeyorAction { get; [UsedImplicitly] private set; }
		public ActionVM OpenGitHubAction { get; [UsedImplicitly] private set; }

		private void DoOpenGitHub() {
			string url = null;
			var d = ProjectSelector.SelectedProject.Data;
			switch (d.RepositoryType) {
				case "gitHub": {
					var tree = d.Builds.Any() ? $"tree/{d.Builds.FirstOrDefault().Branch}" : "";
					url = $"https://github.com/{d.RepositoryName}/{tree}";
					break;
				}
					// gitHub
					// bitBucket
					// vso (Visual Studio Online)
					// gitLab
					// kiln
					// stash
					// git
					// mercurial
					// subversion
			}

			if (url != null) {
				var psi = new ProcessStartInfo(url) { UseShellExecute = true };
				Process.Start(psi);
			}
		}

		private void DoOpenAppVeyor() {
			var d = ProjectSelector.SelectedProject.Data;
			// Current build ""
			// history
			// deployments
			// events
			// settings
			var page = ""; //
			var url = $"https://ci.appveyor.com/project/{d.AccountName}/{d.Slug}/{page}";
			var psi = new ProcessStartInfo(url) { UseShellExecute = true };
			Process.Start(psi);
		}

		/// <summary>
		/// Method for <see cref="EditScriptAction"/>
		/// </summary>
		[UsedImplicitly]
		private void DoEditScript() {
			YamlEditorController.ExpandCodeBlock();
			_selectedBlock = YamlHelper.ExtractBlock(YamlEditorController.SelectedText);
			if(_selectedBlock==null) return;

			BlockFormat = "(unchanged)";
			YamlEditorController.SetEnabled("YAML Editor is open", false);
			YamlEditorHeight=new GridLength(50);
			CodeEditorHeight=new GridLength(100,GridUnitType.Star);
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
				case "(unchanged)":
					s = YamlHelper.FormatBlock(CodeEditorController.Text,
						_selectedBlock.Suffix, _selectedBlock.Indent, ScalarType.None);
					break;
				case "Block":
					s = YamlHelper.FormatBlock(CodeEditorController.Text,
						_selectedBlock.Suffix, _selectedBlock.Indent, ScalarType.BlockFoldedStrip);
					break;
				case "Split":
					s = YamlHelper.FormatBlock(CodeEditorController.Text,
						_selectedBlock.Suffix, _selectedBlock.Indent, ScalarType.Plain);
					break;
				case "Single":
					s = YamlHelper.FormatBlock(CodeEditorController.Text,
						_selectedBlock.Suffix, _selectedBlock.Indent, ScalarType.FlowDoubleQuoted);
					break;
				default:
					return;
			}

			YamlEditorController.SelectedText = s;
			CodeEditorController.Text = "";
			CloseCodeEditor();
			ApplicationDispatcher.Instance.BeginInvoke(DispatcherPriority.Render, YamlEditorController.BringSelectionIntoView);
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
			if (!match.Success) {
				if (Keyboard.Modifiers == ModifierKeys.Control) {
					DoInsertTemplate();
					return;
				}
				return;
			}
			YamlEditorController.Data.Select(match.Index+match.Length,0);
			YamlEditorController.Data.ScrollTo(YamlEditorController.Data.TextArea.Caret.Line,0);
		}

		private void AtIsNavigationDropDownOpenChanged(object sender, ValueChangedEventArgs e) {
			if ((bool) e.NewValue == true) Task.Run(RefreshNavigationItemsAsync);
		}

		private async Task RefreshNavigationItemsAsync() {
			string text = null;
			ApplicationDispatcher.Instance.Invoke(() => text = YamlEditorController.Data.Text);
			foreach (var navItem in NavigationItems) {
				await Task.Run(() => {
					var match = navItem.Regex.Match(text);
					ApplicationDispatcher.Instance.Invoke(() => navItem.ExistsInDocument = match.Success);
				}).ConfigureAwait(false);
			}
		}

		/// <summary>
		/// Gets the <see cref="ActionVM"/> to CancelEdit
		/// </summary>
		/// <seealso cref="DoCancelEdit"/>
		public ActionVM CancelEditAction { get; [UsedImplicitly] private set; }

		public string BlockFormat { get => Fields.GetValue<string>(); set => Fields.SetValue(value); }
		public bool IsModified { get => Fields.GetValue<bool>(); private set => Fields.SetValue(value); }

		[Hierarchy(HierarchyType.Reference)]
		public ProjectSelectorVM ProjectSelector { get => Fields.GetValue<ProjectSelectorVM>(); set => Fields.SetValue(value); }

		public string StatusBarText { get => Fields.GetValue<string>(); set => Fields.SetValue(value); }

		public SearchPanelVM SearchPanel { get; private set; }

		[Hierarchy(HierarchyType.Reference)]
		public NavigationItemVM SelectedNavigationItem { get => Fields.GetValue<NavigationItemVM>(); set => Fields.SetValue(value); }
		public bool IsNavigationDropDownOpen { get => Fields.GetValue<bool>(); set => Fields.SetValue(value); }

		public GridLength YamlEditorHeight { get => Fields.GetValue<GridLength>(); set => Fields.SetValue(value); }
		public GridLength CodeEditorHeight { get => Fields.GetValue<GridLength>(); set => Fields.SetValue(value); }

		/// <summary>
		/// Method for <see cref="CancelEditAction"/>
		/// </summary>
		[UsedImplicitly]
		private void DoCancelEdit() {
			CodeEditorController.Text = "";
			CloseCodeEditor();
			ApplicationDispatcher.Instance.BeginInvoke(DispatcherPriority.Render, YamlEditorController.BringSelectionIntoView);
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
						// ReSharper disable once AsyncConverter.AsyncWait // ContinueWithUIDispatcher
						YamlEditorController.Text = task.Result;
					YamlEditorController.ResetHasChanges();
					EditorProjectName = ProjectSelector.SelectedProject.Data.Name;
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
			EditorProjectName = Path.GetFileName(dlg.FileName);
		}

		/// <summary>
		/// Method for <see cref="SaveAction"/>
		/// </summary>
		[UsedImplicitly]
		private void DoSave() {
			var projectName = ProjectSelector.SelectedProject?.Data.Name ?? "NewProject";
			var dlg = new SaveFileDialog {
				Title = "Save configuration as...",
				Filter = "YAML-File|*.yml",
				FilterIndex = 1,
				FileName = $"{projectName}.{DateTime.Now:yyyyMMddHHmmss}.AutoSave.yml"
			};
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
						MessageBox.Show($"Update failed.\n\nDetails:\n{task.Exception.InnerException?.Message}",
							"Error", MessageBoxButton.OK, MessageBoxImage.Error);
					} else {
						StatusBarText = "Update done.";
						YamlEditorController.ResetHasChanges();
					}
				});
		}
	}

	public class SectionTemplateData {
		public string Key { get; set; }
		public string Content { get; set; }
	}

	// http://stackoverflow.com/questions/3790454/in-yaml-how-do-i-break-a-string-over-multiple-lines/21699210#21699210

	public enum BlockFormat {
		None,
		Folded,		// >	removes single newlines within the string (but adds one at the end, and converts double newlines to singles):
		Literal,	// |	turns every newline within the string into a literal newline, and adds one at the end
		Original
	}

	public enum ChompingIndicator {
		None,
		Clip,       //		"clip": keep the line feed, remove the trailing blank lines. (TrimEnd)
		Strip,      // -	"strip": remove the line feed, remove the trailing blank lines.	(Trim)
		Keep,       // +	"keep": keep the line feed, keep trailing blank lines.	
	}

}
