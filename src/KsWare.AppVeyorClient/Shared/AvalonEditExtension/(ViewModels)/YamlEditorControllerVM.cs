using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Xml;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Highlighting.Xshd;
using KsWare.AppVeyorClient.Helpers;
using KsWare.Presentation;
using KsWare.Presentation.ViewModelFramework;
using BindingMode = System.Windows.Data.BindingMode;

namespace KsWare.AppVeyorClient.Shared.AvalonEditExtension {

	public class YamlEditorControllerVM:CodeEditorControllerVM {

		public YamlEditorControllerVM() {
			RegisterChildren(()=>this);

			MenuItemVM shi,foi,srhi;
			ContextMenu.Items.Add(new MenuItemVM {Caption = "-"});
			ContextMenu.Items.Add(new MenuItemVM {
				Caption = "Options",
				Items = {
					(shi  = new MenuItemVM {Caption = "Syntax Highlighting", IsCheckable        = true}),
					(foi  = new MenuItemVM {Caption = "Folding", IsCheckable                    = true}),
					(srhi = new MenuItemVM {Caption = "Search Result Highlighting", IsCheckable = true}),
				}
			});

			shi.Fields[nameof(MenuItemVM.IsChecked)].SetBinding(new FieldBinding(Fields[nameof(DisableSyntaxHighlighting)],BindingMode.TwoWay,InvertBoolConverter.Default));
			foi.Fields[nameof(MenuItemVM.IsChecked)].SetBinding(new FieldBinding(Fields[nameof(DisableFolding)],BindingMode.TwoWay,InvertBoolConverter.Default));
			srhi.Fields[nameof(MenuItemVM.IsChecked)].SetBinding(new FieldBinding(Fields[nameof(DisableSearchResultHighlighting)],BindingMode.TwoWay,InvertBoolConverter.Default));

			Fields[nameof(DisableSyntaxHighlighting)].ValueChangedEvent.add = (s, e) => {OnDisableSyntaxHighlightingChanged((bool) e.NewValue);};
			Fields[nameof(DisableFolding)].ValueChangedEvent.add = (s, e) => {OnDisableFoldingChanged((bool) e.NewValue);};
			Fields[nameof(DisableSearchResultHighlighting)].ValueChangedEvent.add = (s, e) => {OnDisableSearchResultHighlightingChanged((bool) e.NewValue);};

			//DEBUG
			Fields[nameof(DisableSyntaxHighlighting)].ValueChangedEvent.add = (s, e) => Debug.WriteLine($"DisableSyntaxHighlighting: {e.NewValue}");
			shi.Fields[nameof(MenuItemVM.IsChecked)].ValueChangedEvent.add = (s, e) => Debug.WriteLine($"MenuItemVM.IsChecked: {e.NewValue}");
		}

		protected virtual void OnDisableSearchResultHighlightingChanged(bool value) {

		}

		protected virtual void OnDisableFoldingChanged(bool value) {

		}

		protected virtual void OnDisableSyntaxHighlightingChanged(bool value) {
			
		}

		#region options

		public bool DisableSyntaxHighlighting { get => Fields.GetValue<bool>(); set => Fields.SetValue(value); }

		public bool DisableFolding { get => Fields.GetValue<bool>(); set => Fields.SetValue(value); }

		public bool DisableSearchResultHighlighting { get => Fields.GetValue<bool>(); set => Fields.SetValue(value); }

		#endregion

		protected override void OnViewConnected() {
			base.OnViewConnected();

			if (DisableSyntaxHighlighting == false) {
				var reader = XmlReader.Create("Data\\AppVeyor-yaml.xshd");
				Data.SyntaxHighlighting = HighlightingLoader.Load(reader, HighlightingManager.Instance);
			}
		}

		public YamlTextRange MeasureBlock() {
			var tr = new YamlTextRange();

			tr.StartLine = Data.Document.GetLineByOffset(Data.SelectionStart);
			tr.EndLine = Data.SelectionStart + Data.SelectionLength > tr.StartLine.Offset + tr.StartLine.TotalLength
				? Data.Document.GetLineByOffset(Data.SelectionStart + Data.SelectionLength)
				: tr.StartLine;
			var multiLine = "";

			// find block-start "- ps: "
			while (true) {
				var s = Data.GetLineText(tr.StartLine);
				var match = YamlRegEx.Match(s);
				if (match.Success) {
					multiLine = match.Multiline;
					tr.StartMatch = match;
					break;
				}

				if (tr.StartLine.LineNumber == 1) break;
				tr.StartLine = tr.StartLine.PreviousLine;
			}

			if (!string.IsNullOrWhiteSpace(multiLine)) {
				if (tr.StartLine == tr.EndLine && tr.EndLine.NextLine == null) {
					// incomplete code block
				}
				else {
					// find block-end
					while (true) {
						tr.EndLine = tr.EndLine.NextLine;
						var s = Data.GetLineText(tr.EndLine);
						if (!Regex.IsMatch(s, @"^\x20{4}|^\s*$")) { // code has always 4 spaces because each command start without indentation
							tr.EndLine = tr.EndLine.PreviousLine;
							break;
						}
						if (tr.EndLine.NextLine == null) break;
					}
				}
			}
			else {
				tr.EndLine = tr.StartLine;
			}

			return tr;
		}

		public void ExpandCodeBlock() {
			var tr = MeasureBlock();
			Data.Select(tr.StartLine.Offset, tr.EndLine.Offset + tr.EndLine.Length - tr.StartLine.Offset);
		}
	}

	public class YamlTextRange {
		public DocumentLine StartLine { get; set; }
		public DocumentLine EndLine { get; set; }
		public YamlRegExMatch StartMatch { get; set; }
	}

}
