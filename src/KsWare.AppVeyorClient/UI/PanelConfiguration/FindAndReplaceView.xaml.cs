using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Threading;
using JetBrains.Annotations;
using KsWare.AppVeyorClient.Shared.AvalonEditExtension;
using KsWare.Presentation;
using KsWare.Presentation.ViewModelFramework;

namespace KsWare.AppVeyorClient.UI.PanelConfiguration {
	/// <summary>
	/// Interaction logic for FindAndReplaceView.xaml
	/// </summary>
	public partial class FindAndReplaceView {
		public FindAndReplaceView() {
			InitializeComponent();
		}
	}

	public class FindAndReplaceVM : WindowVM {

		private static FindAndReplaceVM _instance;

		public static void ShowInstance(AppVeyorYamlEditorControllerVM editor) {
			if (_instance == null) {
				_instance = new FindAndReplaceVM();
			}

			_instance.Editor = editor;
			_instance.Show();
		}

		private readonly ColorizeSearchText _colorizer = new ColorizeSearchText();

		/// <inheritdoc />
		public FindAndReplaceVM() {
			RegisterChildren(() => this);
			Fields[nameof(Editor)].ValueChangedEvent.add = AtEditorChanged;

			Fields[nameof(SearchText)].ValueChangedEvent.add = (s, e) => _colorizer.SearchText = (string)e.NewValue; ;
			Fields[nameof(MatchCase)].ValueChangedEvent.add = (s, e) => _colorizer.CaseSensitive = (bool)e.NewValue;
			Fields[nameof(UseRegex)].ValueChangedEvent.add = (s, e) => _colorizer.UseRegex = (bool)e.NewValue;

			Fields[nameof(SearchText)].ValueChangedEvent.add = (s, e) => OnSearchParameterChanged();
			Fields[nameof(MatchCase)].ValueChangedEvent.add = (s, e) => OnSearchParameterChanged();
			Fields[nameof(UseRegex)].ValueChangedEvent.add = (s, e) => OnSearchParameterChanged();

			Fields[nameof(SelectedSearchResult)].ValueChangedEvent.add = (s, e) => OnSelectedSearchResultChanged();

			if (UIAccess.HasWindow) AtWindowChanged(null, new ValueChangedEventArgs<Window>(null, UIAccess.Window));
			else UIAccess.WindowChanged += AtWindowChanged;
		}

		private void OnSelectedSearchResultChanged() {
			if(SelectedSearchResult==null) return;
			Select(SelectedSearchResult.Offset, SelectedSearchResult.Length);
		}

		private void OnSearchParameterChanged() {
			Editor.Data.TextArea.TextView.Redraw(DispatcherPriority.Background);
		}

		private void AtWindowChanged(object sender, ValueChangedEventArgs<Window> e) {
			AssignColorizer(Editor, e.NewValue != null);
		}

		private void AssignColorizer(YamlEditorControllerVM editor, bool assign) {
			if (assign && editor!=null) {
				var idx = editor.Data.TextArea.TextView.LineTransformers.IndexOf(_colorizer);
				if(idx<0) editor.Data.TextArea.TextView.LineTransformers.Add(_colorizer);
			}
			else if (!assign && editor!=null) {
				Editor?.Data.TextArea.TextView.LineTransformers.Remove(_colorizer);
			}
		}

		private void AtEditorChanged(object sender, ValueChangedEventArgs e) {
			var oldEditor = (YamlEditorControllerVM)e.OldValue;
			var newEditor = (YamlEditorControllerVM)e.NewValue;

			AssignColorizer(oldEditor, false);
			AssignColorizer(newEditor, true);

			// Editor.Data.TextArea.DocumentChanged
			// Editor.Data.TextArea.Caret.PositionChanged
			// Editor.Data.TextChanged
		}

		[Hierarchy(HierarchyType.Reference)]
		public YamlEditorControllerVM Editor { get => Fields.GetValue<YamlEditorControllerVM>(); set => Fields.SetValue(value); }

		public string SearchText { get => Fields.GetValue<string>(); set => Fields.SetValue(value); }

		public IList<string> SearchTextMru { get; } = new ObservableCollection<string>();

		public string ReplaceText { get => Fields.GetValue<string>(); set => Fields.SetValue(value); }

		public IList<string> ReplaceTextMru { get; } = new ObservableCollection<string>();

		public bool MatchCase { get => Fields.GetValue<bool>(); set => Fields.SetValue(value); }

		public bool UseRegex { get => Fields.GetValue<bool>(); set => Fields.SetValue(value); }

		public IList<SearchResult> SearchResults { get; } = new ObservableCollection<SearchResult>();

		public SearchResult SelectedSearchResult { get => Fields.GetValue<SearchResult>(); set => Fields.SetValue(value); }

		/// <summary>
		/// Gets the <see cref="ActionVM"/> to FindNext
		/// </summary>
		/// <seealso cref="DoFindNext"/>
		public ActionVM FindNextAction { get; [UsedImplicitly] private set; }

		/// <summary>
		/// Method for <see cref="FindNextAction"/>
		/// </summary>
		[UsedImplicitly]
		private void DoFindNext() {
			var startIndex = Editor.Data.SelectionStart + 1;
			if (startIndex >= Editor.Data.Text.Length) return;

			int pos = -1;
			int length = 0;
			if (UseRegex) {
				var match = Regex.Match(Editor.Text.Substring(startIndex+1), SearchText, MatchCase ? RegexOptions.None : RegexOptions.IgnoreCase);
				if (match.Success) {
					pos = match.Index + startIndex + 1;
					length = match.Length;
				}
			}
			else {
				pos = Editor.Text.IndexOf(SearchText, startIndex, MatchCase ? StringComparison.CurrentCulture : StringComparison.CurrentCultureIgnoreCase);
				length = SearchText.Length;
			}

			if (pos >= 0) {
				Select(pos, length);
			}
		}

		private void Select(int pos, int length) {
			Editor.Data.Focus();
			Editor.Data.Select(pos, length);
			Editor.Data.TextArea.Caret.Offset = pos;
			Editor.Data.TextArea.Caret.BringCaretToView();
			Editor.Data.TextArea.Caret.Offset = pos+length;
			Editor.Data.TextArea.Caret.BringCaretToView();
		}

		/// <summary>
		/// Gets the <see cref="ActionVM"/> to FindPrevious
		/// </summary>
		/// <seealso cref="DoFindPrevious"/>
		public ActionVM FindPreviousAction { get; [UsedImplicitly] private set; }

		/// <summary>
		/// Method for <see cref="FindPreviousAction"/>
		/// </summary>
		[UsedImplicitly]
		private void DoFindPrevious() {
			var startIndex = Editor.Data.SelectionStart - 1;
			if (startIndex < 0) return;

			int pos = -1;
			int length = 0;

			if (UseRegex) {
				var match = Regex.Match(Editor.Text.Substring(0, startIndex+1), SearchText,
					MatchCase ? RegexOptions.None|RegexOptions.RightToLeft : RegexOptions.IgnoreCase|RegexOptions.RightToLeft);
				if (match.Success) {
					pos = match.Index;
					length = match.Length;
				}
			}
			else {
				pos = Editor.Text.LastIndexOf(SearchText, startIndex, MatchCase ? StringComparison.CurrentCulture : StringComparison.CurrentCultureIgnoreCase);
				length = SearchText.Length;
			}

			if (pos >= 0) {
				Select(pos, length);
			}
		}

		public int ResultCount { get => Fields.GetValue<int>(); set => Fields.SetValue(value); }

		public int CurrentResultIndex { get; set; }

		/// <summary>
		/// Gets the <see cref="ActionVM"/> to FindAll
		/// </summary>
		/// <seealso cref="DoFindAll"/>
		public ActionVM FindAllAction { get; [UsedImplicitly] private set; }

		/// <summary>
		/// Method for <see cref="FindAllAction"/>
		/// </summary>
		[UsedImplicitly]
		private void DoFindAll() {
			SearchResults.Clear();

			if (UseRegex) {
				var matches = Regex.Matches(Editor.Text, SearchText, MatchCase ? RegexOptions.None : RegexOptions.IgnoreCase);
				foreach (Match match in matches) {
					SearchResults.Add(new SearchResult{Offset = match.Index, Length = match.Length, Text = match.Value});
				}
			}
			else {
				var length = SearchText.Length;
				var textLength = Editor.Text.Length;
				if (length >= 1) {
					var i = 0;
					while (true) {
						i = Editor.Text.IndexOf(SearchText, i, StringComparison.OrdinalIgnoreCase);
						if (i < 0) break;
						SearchResults.Add(new SearchResult{Offset = i, Length = length, Text = SearchText});
						i += SearchText.Length;
						if (i >= textLength) break;
					}
				}
			}
			CurrentResultIndex = 0;
			ResultCount = SearchResults.Count;
		}

		/// <summary>
		/// Gets the <see cref="ActionVM"/> to ReplaceNext
		/// </summary>
		/// <seealso cref="DoReplaceNext"/>
		public ActionVM ReplaceNextAction { get; [UsedImplicitly] private set; }

		/// <summary>
		/// Method for <see cref="ReplaceNextAction"/>
		/// </summary>
		[UsedImplicitly]
		private void DoReplaceNext() {
			var startIndex = Editor.Data.SelectionStart;
			int pos;
			int length;
			string replaceText;
			if (UseRegex) {
				var match = Regex.Match(Editor.Text.Substring(startIndex), SearchText, MatchCase ? RegexOptions.None : RegexOptions.IgnoreCase);
				if (!match.Success) return;
				pos = match.Index + startIndex;
				length = match.Length;
				replaceText = Regex.Replace(match.Value, SearchText, ReplaceText, MatchCase ? RegexOptions.None : RegexOptions.IgnoreCase);
			}
			else {
				pos = Editor.Text.IndexOf(SearchText, startIndex, MatchCase ? StringComparison.CurrentCulture : StringComparison.CurrentCultureIgnoreCase);
				length = SearchText.Length;
				if (pos < 0) return;
				replaceText = ReplaceText;
			}
			Editor.Data.Select(pos, length);
			Editor.Data.SelectedText = replaceText;
			DoFindNext();
		}

		/// <summary>
		/// Gets the <see cref="ActionVM"/> to ReplaceAll
		/// </summary>
		/// <seealso cref="DoReplaceAll"/>
		public ActionVM ReplaceAllAction { get; [UsedImplicitly] private set; }

		/// <summary>
		/// Method for <see cref="ReplaceAllAction"/>
		/// </summary>
		[UsedImplicitly]
		private void DoReplaceAll() {
			if (UseRegex) {
				Editor.Data.Text = Regex.Replace(Editor.Data.Text, SearchText, ReplaceText,
					MatchCase ? RegexOptions.None : RegexOptions.IgnoreCase);
			}
			else {
				Editor.Data.Text = Editor.Data.Text.Replace(SearchText, ReplaceText,
					MatchCase ? StringComparison.CurrentCulture : StringComparison.CurrentCultureIgnoreCase);
			}
		}
	}

	public class SearchResult {
		public int Offset { get; set; }
		public int Length { get; set; }
		public int Line { get; set; }
		public int CharOffset { get; set; }
		public string Text { get; set; }
	}

}
