using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JetBrains.Annotations;
using KsWare.Presentation;
using KsWare.Presentation.ViewModelFramework;

namespace KsWare.AppVeyorClient.UI.Common {
	
	public class SearchPanelVM : ObjectVM {

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

		public YamlTextBoxControllerVM Editor { get; set; }

		private List<int> SearchResults { get; set; } = new List<int>();

		private void OnSearchTextChanged(object sender, ValueChangedEventArgs valueChangedEventArgs) {
			SearchResults.Clear();
			var length = SearchText.Length;
			var textLength = Editor.Text.Length;
			if (length < 3) {

			}
			else {
				var i = 0;
				while (true) {
					i = Editor.Text.IndexOf(SearchText,i, StringComparison.InvariantCultureIgnoreCase);
					if(i<0) break;
					SearchResults.Add(i);
					i += SearchText.Length;
					if(i>= textLength) break;
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
		}

		/// <summary>
		/// Method for <see cref="PreviousResultAction"/>
		/// </summary>
		[UsedImplicitly]
		private void DoPreviousResult() {
			if (CurrentResultIndex > 1) {
				CurrentResultIndex--;
			}
			Editor.Data.Focus();
			Editor.Data.Select(SearchResults[CurrentResultIndex-1],SearchText.Length);
			
		}
	}
}
