using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
using System.Xml;
using ICSharpCode.AvalonEdit.CodeCompletion;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Editing;
using ICSharpCode.AvalonEdit.Folding;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Highlighting.Xshd;
using KsWare.AppVeyorClient.Shared.AvalonEditExtension;
using KsWare.AppVeyorClient.Shared;
using KsWare.AppVeyorClient.Shared.PresentationFramework;
using KsWare.AppVeyorClient.UI.App;
using KsWare.AppVeyorClient.UI.ViewModels;
using KsWare.Presentation;
using BindingMode = System.Windows.Data.BindingMode;

namespace KsWare.AppVeyorClient.UI.PanelConfiguration {

	public class AppVeyorYamlEditorControllerVM : YamlEditorControllerVM {

		CompletionWindow completionWindow;
		private List<MyCompletionData> _appVeyorEnvironmentVariables;
		private List<MyCompletionData> _dynamicVariables;
		private FoldingManager _foldingManager;
		private YamlFoldingStrategy _foldingStrategy;

		public AppVeyorYamlEditorControllerVM() {
			RegisterChildren(() => this);
			_appVeyorEnvironmentVariables=PrepareAppVeyorEnvironmentVariables();
		}

		private SettingsVM Settings => AppVM.Current.Settings;

		private List<MyCompletionData> PrepareAppVeyorEnvironmentVariables() {
			var lines = File.ReadAllLines("Data\\AppVeyorEnvironmentVariables.txt");
			var list = new List<MyCompletionData>();
			foreach (var line in lines) {
				if(string.IsNullOrWhiteSpace(line) || line.Trim().StartsWith("#")) continue;
				var tokens=line.Split(new[] {": "}, StringSplitOptions.None);
				list.Add(new MyCompletionData(tokens[0], tokens[1]));
			}
			return list;
		}

		protected override void OnViewConnected() {
			Fields[nameof(DisableSyntaxHighlighting)].SetBinding(new FieldBinding(Settings.Fields[nameof(SettingsVM.EnableYamlSyntaxHighlighting)], BindingMode.TwoWay, InvertBoolConverter.Default));
//			Fields[nameof(DisableSearchResultHighlighting)].SetBinding(new FieldBinding(Settings.Fields[nameof(SettingsVM.EnableYamlSearchResultHighlighting)] , BindingMode.TwoWay, InvertBoolConverter.Default));
			Fields[nameof(DisableFolding)].SetBinding(new FieldBinding(Settings.Fields[nameof(SettingsVM.EnableYamlFolding)], BindingMode.TwoWay, InvertBoolConverter.Default));

			base.OnViewConnected();  

			Data.TextArea.TextEntering += textEditor_TextArea_TextEntering;
			Data.TextArea.TextEntered  += textEditor_TextArea_TextEntered;

			Data.Document.PropertyChanged+= (s, e) => {
				switch (e.PropertyName) {
					case N.Document.LineCount: UpdateFoldings("LineCount changed"); break;
				}
			};

//			Data.TextArea.TextEntered += (s, e) => {
//				if (e.Text == "\n") UpdateFoldings("NewLine inserted");
//			};

			_foldingManager  = FoldingManager.Install(Data.TextArea);
			_foldingStrategy = new YamlFoldingStrategy();
		}

		protected override void OnDisableFoldingChanged(bool value) {
			if (Data == null) return;
		}

		protected override void OnDisableSearchResultHighlightingChanged(bool value) {
			if (Data == null) return;
		}

		protected override void OnDisableSyntaxHighlightingChanged(bool value) {
			if(Data==null)return;
			if (false == value) {
				var reader = XmlReader.Create("Data\\AppVeyor-yaml.xshd");
				Data.SyntaxHighlighting = HighlightingLoader.Load(reader, HighlightingManager.Instance);
			}
			else {
				Data.SyntaxHighlighting = null;
			}
		}

		private void UpdateFoldings(string reason) {
			if(Settings.EnableYamlFolding==false) return;

			Debug.WriteLine($"UpdateFoldings: {reason}");
			_foldingStrategy.UpdateFoldings(_foldingManager, Data.Document);
		}

		protected override void OnTextChanged(object changedRegion) {
			base.OnTextChanged(changedRegion);
			if (changedRegion is IDocument) {
				var text = ((IDocument) changedRegion).Text;
				var variables=ParseVariables(text);
				_dynamicVariables = variables.Select(v => new MyCompletionData(v)).ToList();

				UpdateFoldings("Document changed");
			}
		}

		private List<string> ParseVariables(string text) {
			var matches=Regex.Matches(text, @"(?<=%)\w+(?=%)|(?<=\$)[\w:]+",RegexOptions.Compiled);
			var list = matches.Cast<Match>()
				.Select(m=>m.Value)
				.Distinct(StringComparer.OrdinalIgnoreCase)
				.Except(_appVeyorEnvironmentVariables.Select(v => v.Text), StringComparer.OrdinalIgnoreCase)
				.ToList();

			//TODO parse environment:
			return list;
		}

		void textEditor_TextArea_TextEntered(object sender, TextCompositionEventArgs e) {
			if (e.Text == "$") { // Open code completion after the user has pressed $:
				completionWindow = new CompletionWindow(Data.TextArea) {
					SizeToContent = SizeToContent.Width,
					WindowStyle = WindowStyle.None,
					AllowsTransparency = true
				};
				var data = completionWindow.CompletionList.CompletionData;
				data.AddRange(_dynamicVariables);
				data.AddRange(_appVeyorEnvironmentVariables);

				completionWindow.Show();
				completionWindow.Closed += delegate { completionWindow = null; };
			}
		}

		void textEditor_TextArea_TextEntering(object sender, TextCompositionEventArgs e) {
			if (e.Text.Length > 0 && completionWindow != null) {
				if (!char.IsLetterOrDigit(e.Text[0])) {
					// Whenever a non-letter is typed while the completion window is open,
					// insert the currently selected element.
					completionWindow.CompletionList.RequestInsertion(e);
				}
			}
			// Do not set e.Handled=true.
			// We still want to insert the character that was typed.
		}

		private static class N {
			public static class Document {
				public const string LineCount = nameof(TextDocument.LineCount);
			}
		}
	}

	/// Implements AvalonEdit ICompletionData interface to provide the entries in the
	/// completion drop down.
	public class MyCompletionData : ICompletionData {

		/// <summary>
		/// Initializes a new instance of the <see cref="MyCompletionData"/> class.
		/// </summary>
		/// <param name="text">The text.</param>
		public MyCompletionData(string text) {
			Text = text;
			Content = text;
		}

		public MyCompletionData(string text, string description) {
			Text = text;
			Description = description;
			Content = text;
		}

		/// <inheritdoc />
		public System.Windows.Media.ImageSource Image { get; private set; }

		/// <inheritdoc />
		public string Text { get; private set; }

		// Use this property if you want to show a fancy UIElement in the list.
		/// <inheritdoc />
		public object Content { get; private set; }

		/// <inheritdoc />
		public object Description { get; private set; }

		/// <inheritdoc />
		public double Priority { get; private set; } = 1;

		public void Complete(TextArea textArea, ISegment completionSegment, EventArgs insertionRequestEventArgs) {
			textArea.Document.Replace(completionSegment, this.Text);
		}
	}

}
