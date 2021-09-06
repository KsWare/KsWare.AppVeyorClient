using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Threading;
using JetBrains.Annotations;
using KsWare.AppVeyor.Api;
using KsWare.AppVeyorClient.Helpers;
using KsWare.AppVeyorClient.Shared;
using KsWare.AppVeyorClient.Shared.AvalonEditExtension;
using KsWare.AppVeyorClient.UI.App;
using KsWare.AppVeyorClient.UI.Common;
using KsWare.AppVeyorClient.UI.PanelProjectSelector;
using KsWare.AppVeyorClient.UI.PanelSearch;
using KsWare.AppVeyorClient.UI.ViewModels;
using KsWare.Presentation;
using KsWare.Presentation.ViewModelFramework;
using BindingMode = System.Windows.Data.BindingMode;
using MessageBox = System.Windows.MessageBox;
using OpenFileDialog = Microsoft.Win32.OpenFileDialog;
using Path = System.IO.Path;
using SaveFileDialog = Microsoft.Win32.SaveFileDialog;

namespace KsWare.AppVeyorClient.UI.PanelConfiguration {

	public class ConfigurationPanelVM : ObjectVM,IHaveTitle {

		private static readonly Dictionary<string, HelpEntry> HelpDictionary = HelpEntry.LoadResource();

		private YamlBlock _selectedBlock;
		private readonly List<SectionTemplateData>_sectionTemplates = new List<SectionTemplateData>();
		private readonly MenuItemVM _editScriptBlockMenuItem;

		public ConfigurationPanelVM() {
			RegisterChildren(() => this);

			SearchPanel.Editor = YamlEditorController;

			FillSectionTemplates();
			FillNavigation();
			Fields[nameof(SelectedNavigationItem)].ValueChangedEvent.add = AtNavigationKeyChanged;
			Fields[nameof(IsNavigationDropDownOpen)].ValueChangedEvent.add = AtIsNavigationDropDownOpenChanged;
			CloseCodeEditor();

			Fields[nameof(IsModified)].SetBinding(new FieldBinding(YamlEditorController.Fields[nameof(YamlEditorController.IsModified)],BindingMode.OneWay));
//			YamlEditorController.Fields[nameof(YamlEditorController.IsModified)].ValueChangedEvent.add=
//				(s, e) => IsModified = YamlEditorController.IsModified;

			YamlEditorController.ContextMenu.Items.Add(_editScriptBlockMenuItem=new MenuItemVM {
				Caption = "Edit Code Block",
				CommandAction = {
					MːExecutedCallback = (s, e) => DoEditScriptBlock(),
				}
			});

			if (YamlEditorController.MːData != null) YamlEditorControllerOnViewConnected(null, null);
			else YamlEditorController.ViewConnected += YamlEditorControllerOnViewConnected;
		}

		private void YamlEditorControllerOnViewConnected(object sender, EventArgs e) {
			YamlEditorController.Data.ContextMenuOpening+=TextEditor_OnContextMenuOpening;
			YamlEditorController.Data.TextArea.Caret.PositionChanged+=CaretOnPositionChanged;
		}

		private void CaretOnPositionChanged(object sender, EventArgs e) {
			EditScriptBlockAction.SetCanExecute("IsCodeBlock", CanEditScriptBlock());
		}

		private void TextEditor_OnContextMenuOpening(object sender, ContextMenuEventArgs e) {
			_editScriptBlockMenuItem.CommandAction.SetCanExecute("IsCodeBlock", CanEditScriptBlock());
		}

		private bool CanEditScriptBlock() {
			var block = YamlEditorController.MeasureBlock();
			if (block.StartMatch != null && block.StartMatch.Success) {
				switch (block.StartMatch.Name) {
					case "": case "ps": case "cmd": case "sh": {
						return true;
					}
				}
			}

			return false;
		}

		public string Title => "Configuration";

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

		public ActionVM GetAction { get; [UsedImplicitly] private set; }
		public ActionVM LoadAction { get; [UsedImplicitly] private set; }
		public ActionVM SaveAction { get; [UsedImplicitly] private set; }
		public ActionVM PostAction { get; [UsedImplicitly] private set; }
		public ActionVM InsertTemplate { get; [UsedImplicitly] private set; }
		public ActionVM InsertVariableAction { get; [UsedImplicitly] private set; }
		public ActionVM ContextHelpAction { get; [UsedImplicitly] private set; }
		public ActionVM EscapeAction { get; [UsedImplicitly] private set; }

		public PopupVM Popup { get; } = new PopupVM();

		private void DoEscape() {
			if (Popup.IsOpen) Popup.IsOpen = false;
		}

		public void DoContextHelp() {
			var path=YamlEditorController.GetPath();
			Debug.WriteLine(path);
			var pathSegments = path.Split('/');
			var helpEntries = new List<HelpEntry>();
			for (int i = pathSegments.Length - 1; i >= 0; i--) {
				var key = string.Join("/", pathSegments.Take(i+1));
				if(HelpDictionary.TryGetValue(key, out var help)) helpEntries.Add(help);
			}

			helpEntries.Reverse();

			Popup.View.PlacementTarget = YamlEditorController.Data;
			Popup.View.Placement=PlacementMode.Relative;
			Popup.Title = "AppVeyor Help";
			Popup.Document = HelpEntryFormatter.Instance.CreateFlowDocument(helpEntries);
			Popup.Show();

			var caretRect = YamlEditorController.Data.TextArea.Caret.CalculateCaretRectangle();
			Popup.View.HorizontalOffset = caretRect.X - YamlEditorController.Data.TextArea.TextView.HorizontalOffset;
			Popup.View.VerticalOffset = caretRect.Bottom - YamlEditorController.Data.TextArea.TextView.VerticalOffset;
		}

		public string EditorProjectName { get => Fields.GetValue<string>(); private set => Fields.SetValue(value); }

		private void DoInsertVariable() {
			// CMD:
			//	%MY_VARIABLE%
			//	set MY_VARIABLE
			// PS|PWSH:  (also SH?)
			//	$env:MY_VARIABLE
			// Property:
			//	$(MY_VARIABLE)

			if(SelectedEnvironmentVariable==null) return;

			var block = YamlEditorController.MeasureBlock();
			string s;
			switch (block.StartMatch.Name) {
				case "ps": case "pwsh":case "sh":
					s = "$env:" + SelectedEnvironmentVariable.Name;
					break;
				case "cmd":
				case "": // e.g. "- git commit"
					s = $"%{SelectedEnvironmentVariable.Name}%";
					break;
				default:
					s = $"$({SelectedEnvironmentVariable.Name})";
					break;
			}

			if (YamlEditorController.IsEnabled) {
				YamlEditorController.SelectedText = s;
			}
			else if (CodeEditorController.IsEnabled) {
				CodeEditorController.SelectedText = s;
			}
		}

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
				if (dlg.ShowDialog() != true) return;
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
		/// <seealso cref="DoEditScriptBlock"/>
		public ActionVM EditScriptBlockAction { get; [UsedImplicitly] private set; }
		public ActionVM OpenAppVeyorAction { get; [UsedImplicitly] private set; }
		public ActionVM OpenGitHubAction { get; [UsedImplicitly] private set; }

		private void DoOpenGitHub() {
			if (ProjectSelector.SelectedProject == null) {
				Process.Start(new ProcessStartInfo("https://github.com/") { UseShellExecute = true });
				return;
			}
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

		public ObservableCollection<EnvironmentVariableInfo> EnvironmentVariables { get; } = EnvironmentVariableInfo.Create<ObservableCollection<EnvironmentVariableInfo>>();
		
		public EnvironmentVariableInfo SelectedEnvironmentVariable { get => Fields.GetValue<EnvironmentVariableInfo>(); set => Fields.SetValue(value); }

		private void DoOpenAppVeyor() {
			if (ProjectSelector.SelectedProject == null) {
				Process.Start(new ProcessStartInfo("https://ci.appveyor.com/") { UseShellExecute = true });
				return;
			}

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
		/// Method for <see cref="EditScriptBlockAction"/>
		/// </summary>
		[UsedImplicitly]
		private void DoEditScriptBlock() {
			YamlEditorController.ExpandCodeBlock();
			_selectedBlock = YamlHelper.ExtractBlock(YamlEditorController.SelectedText);
			if (_selectedBlock == null) return;

			BlockFormat = "(unchanged)";
			YamlEditorController.SetEnabled("YAML Editor is open", false);
			YamlEditorHeight = new GridLength(50);
			CodeEditorHeight = new GridLength(100, GridUnitType.Star);
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
						_selectedBlock.Suffix, _selectedBlock.Indent, ScalarType.BlockLiteralStrip);
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
			YamlEditorHeight = new GridLength(100, GridUnitType.Star);

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
		public void DoGet() {
			if (ProjectSelector.SelectedProject == null) {
				StatusBarText = "WARNING: No project selected!";
				return;
			}
			if (YamlEditorController.Data.IsModified) {
				if (MessageBox.Show("Discard current changes?", "Get Configuration", MessageBoxButton.OKCancel) !=
				    MessageBoxResult.OK) return;
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
		public void DoLoad() {
			if (YamlEditorController.Data.IsModified) {
				if (MessageBox.Show("Discard current changes?", "Open File", MessageBoxButton.OKCancel) != MessageBoxResult.OK) return;
			}
			var dlg = new OpenFileDialog {Title = "Load configuration...", Filter = "YAML-File|*.y*ml", FilterIndex = 1};
			if (dlg.ShowDialog() != true) return;
			LoadFile(dlg.FileName);
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

		public void DoNewFile() {
			if (YamlEditorController.Data.IsModified) {
				if (MessageBox.Show("Discard current changes?", "New File", MessageBoxButton.OKCancel) !=
				    MessageBoxResult.OK) return;
			}

			YamlEditorController.Text = "";
			EditorProjectName = "NewFile.yml";
		}

		public void LoadFile(string path) {
			using (var reader = File.OpenText(path)) {
				YamlEditorController.Text = reader.ReadToEnd();
			}
			EditorProjectName = Path.GetFileName(path);
			StatusBarText = "File loaded " + Path.GetFileName(path);
		}

		/// <summary>
		/// Gets the <see cref="ActionVM"/> to ValidateYaml
		/// </summary>
		/// <seealso cref="DoValidateYaml"/>
		public ActionVM ValidateYamlAction { get; [UsedImplicitly] private set; }

		/// <summary>
		/// Method for <see cref="ValidateYamlAction"/>
		/// </summary>
		[UsedImplicitly]
		private void DoValidateYaml() {
			StatusBarText = "Validating project settings...";
			Client.Project.ValidateYaml(YamlEditorController.Text, ProjectSelector.SelectedProject.Data.AccountName)
				.ContinueWithUIDispatcher(task => {
					if (task.Exception != null) {
						StatusBarText = $"Validation failed. {task.Exception.InnerException?.Message}";
						MessageBox.Show($"Validation failed.\n\nDetails:\n{task.Exception.InnerException?.Message}",
							"Error", MessageBoxButton.OK, MessageBoxImage.Error);
						return;
					}

					var result = task.Result;
					if (result.IsValid) {
						StatusBarText = $"Validation successful.";
					}
					else {
						StatusBarText = $"Validation failed. In {result.Line},{result.Column}. {result.ErrorMessage}";
						var line = YamlEditorController.Data.Document.GetLineByNumber(result.Line);
						YamlEditorController.Data.Select(line.Offset+result.Column-1, line.Length-result.Column+1);
					}
				});
		}

		/// <summary>
		/// Gets the <see cref="ActionVM"/> to Encrypt
		/// </summary>
		/// <seealso cref="DoEncrypt"/>
		public ActionVM EncryptAction { get; [UsedImplicitly] private set; }

		/// <summary>
		/// Method for <see cref="EncryptAction"/>
		/// </summary>
		[UsedImplicitly]
		private void DoEncrypt() {
			// TODO IMPROVEMENT check for section "environment" consider property "secure"
			var t=YamlEditorController.Data.SelectedText;
			StatusBarText = "Encrypting value...";
			Task.Run(()=>Client.Encrypt(t))
				.ContinueWithUIDispatcher(task => {
					if (task.Exception!=null) throw task.Exception;
					StatusBarText = "Value encrypted";
					YamlEditorController.Data.SelectedText = task.Result;
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
