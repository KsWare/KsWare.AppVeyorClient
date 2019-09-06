using System;
using System.Collections.Generic;
using System.Windows.Threading;
using JetBrains.Annotations;
using KsWare.AppVeyorClient.Shared.AvalonEditExtension;
using KsWare.Presentation;
using KsWare.Presentation.ViewModelFramework;

namespace KsWare.AppVeyorClient.UI.PanelSearch {
	
	public class SearchPanelVM : ObjectVM {

		private ColorizeSearchText _colorizer;

		public SearchPanelVM() {
			RegisterChildren(()=>this);
			Fields[nameof(SearchText)].ValueChangedEvent.add=OnSearchTextChanged;
		}

		public string SearchText { get => Fields.GetValue<string>(); set => Fields.SetValue(value); }

		public int CurrentResultIndex { get => Fields.GetValue<int>(); set => Fields.SetValue(value); }

		public int ResultCount { get => Fields.GetValue<int>(); set => Fields.SetValue(value); }

		/// <summary>
		/// Gets the <see cref="ActionVM"/> to NextResult
		/// </summary>
		/// <seealso cref="DoNextResult"/>
		public ActionVM NextResultAction { get; [UsedImplicitly] private set; }

		/// <summary>
		/// Gets the <see cref="ActionVM"/> to PreviousResult
		/// </summary>
		/// <seealso cref="DoPreviousResult"/>
		public ActionVM PreviousResultAction { get; [UsedImplicitly] private set; }

		public YamlEditorControllerVM Editor { get; set; }

		private List<int> SearchResults { get; set; } = new List<int>();

		private void OnSearchTextChanged(object sender, ValueChangedEventArgs valueChangedEventArgs) {
			if (_colorizer==null) {
				_colorizer = new ColorizeSearchText{CaseSensitive = false};
				Editor.Data.TextArea.TextView.LineTransformers.Add(_colorizer);
			}

			_colorizer.SearchText = SearchText;
			Editor.Data.TextArea.TextView.Redraw(DispatcherPriority.Background);
			
			SearchResults.Clear();
			var length = SearchText.Length;
			var textLength = Editor.Text.Length;
			if (length >= 1) {
				var i = 0;
				while (true) {
					i = Editor.Text.IndexOf(SearchText, i, StringComparison.OrdinalIgnoreCase);
					if (i < 0) break;
					SearchResults.Add(i);
					i += SearchText.Length;
					if (i >= textLength) break;
				}
			}
			CurrentResultIndex = 0;
			ResultCount = SearchResults.Count;
		}

		/// <summary>
		/// Method for <see cref="NextResultAction"/>
		/// </summary>
		[UsedImplicitly]
		private void DoNextResult() {
			if (CurrentResultIndex < ResultCount) {
				CurrentResultIndex++;
			}
			Editor.Data.Focus();
			Editor.Data.Select(SearchResults[CurrentResultIndex-1], SearchText.Length);
			Editor.Data.ScrollToLine(Editor.Data.TextArea.Selection.StartPosition.Line);
		}

		/// <summary>
		/// Method for <see cref="PreviousResultAction"/>
		/// </summary>
		[UsedImplicitly]
		private void DoPreviousResult() {
			if (CurrentResultIndex > 1) {
				CurrentResultIndex--;
			}
			else if(CurrentResultIndex==0)
			{
				if (SearchResults.Count > 0) CurrentResultIndex = 1;
				else return;
			}
			Editor.Data.Focus();
			Editor.Data.Select(SearchResults[CurrentResultIndex-1],SearchText.Length);
			Editor.Data.ScrollToLine(Editor.Data.TextArea.Selection.StartPosition.Line);
		}

//		public void Search(string pattern, SearchMode searchMode) {
//			_skipSearch = true;
//			SearchText = pattern;
//			SearchMode = searchMode;
//			_skipSearch = false;
//			DoNextResult();
//		}
	}
}
