using System.Diagnostics;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows.Data;
using System.Xml;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Highlighting.Xshd;
using KsWare.AppVeyorClient.Shared.PresentationFramework;
using KsWare.Presentation;
using KsWare.Presentation.ViewModelFramework;
using KsWare.Presentation.ViewModelFramework.Providers;
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

		public void ExpandCodeBlock() {
			
			var l0=Data.Document.GetLineByOffset(Data.SelectionStart);
			var t0 = "";
			var l1 = l0.Offset + l0.TotalLength >= Data.SelectionStart + Data.SelectionLength
				? Data.Document.GetLineByOffset(Data.SelectionStart + Data.SelectionLength)
				: l0;
			var multiLine = "";

			// find block-start "- ps: "
			while (true) {
				t0 = Data.GetLineText(l0);
				var m = Regex.Match(t0, @"^-\x20ps:\x20+(?<multiline>[>|][+-]?)?"); // TODO 
				if (m.Success) {
					multiLine = m.Groups["multiline"].Value;
					if (!string.IsNullOrWhiteSpace(multiLine)) l0 = l0.NextLine;
					break;
				}
				if(l0.LineNumber==1) break;
				l0 = l0.PreviousLine;
			}

			if (!string.IsNullOrWhiteSpace(multiLine)) {
				// find block-end
				while (true) {
					var t = Data.GetLineText(l1);
					if (!Regex.IsMatch(t, @"^\x20{4}|^$")) {
						l1 = l1.PreviousLine;
						break;
					}
					l1 = l1.NextLine;
				}
			}
			else {
				l1 = l0;
			}

			Data.Select(l0.Offset,l1.Offset+l1.Length - l0.Offset);
		}
	}

}
