using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using KsWare.Presentation.Core.Providers;
using KsWare.Presentation.ViewModelFramework;

namespace KsWare.AppVeyorClient.Shared.PresentationFramework {

	public class TextBoxControllerVM : DataVM<TextBox> {

		TextBoxData _data = new TextBoxData();

		public TextBoxControllerVM() {
			RegisterChildren(() => this);

			IsEnabledChanged += AtIsEnabledChanged;
		}

		private void AtIsEnabledChanged(object sender, RoutedPropertyChangedEventArgs<bool> e) {
			Data.IsEnabled = e.NewValue;
		}

		public string SelectedText {
			get { return Data?.SelectedText; }
			set {
				if (Data == null) throw new InvalidOperationException("View is not connected.");
				Data.SelectedText = value;
			}
		}

		public string Text {
			get { return Data == null ? _data.Text : Data.Text; }
			set {
				if (Data == null) _data.Text = value;
				else Data.Text               = value;
			}
		}

		protected sealed override void OnDataChanged(DataChangedEventArgs e) {
			base.OnDataChanged(e);
			if (e.NewData != null) {
				OnViewConnected();
			}
		}

		protected virtual void OnViewConnected() {
			ActivePoint    = new TextPoint(Data);
			Data.Text      = _data.Text;
			Data.IsEnabled = _data.IsEnabled;
		}

		public TextPoint ActivePoint { get; private set; }

		public void ExpandSelection() {
			Data.Focus();
			var l0       = Data.GetLineIndexFromCharacterIndex(Data.SelectionStart);
			var l1       = Data.GetLineIndexFromCharacterIndex(Data.SelectionStart + Data.SelectionLength);
			var selStart = Data.GetCharacterIndexFromLineIndex(l0);
			var selEnd = Data.GetCharacterIndexFromLineIndex(l1) +
			             (Data.LineCount - 1 == l1 ? Data.GetLineLength(l1) : Data.GetLineLength(l1) - 2);
			Data.Select(selStart, selEnd - selStart);
			Data.ScrollToHorizontalOffset(0);
		}

		public class TextBoxData {
			public bool IsEnabled { get; set; } = true;
			public string Text { get; set; }
		}

		public class TextPoint {
			private TextBox _textBox;

			public TextPoint(TextBox textBox) { _textBox = textBox; }

			public bool IsInIndentRegion { get; private set; }

			public int IndentPosition { get; private set; }
			public int CharPosition { get; private set; }

			public int LineStart { get; private set; }

			public int LineIndex { get; private set; }

			public void Refresh() {
				if (_textBox.Text.Length == 0) {
					LineIndex        = 0;
					LineStart        = 0;
					CharPosition     = 0;
					IsInIndentRegion = true;
					IndentPosition   = 0;
					return;
				}
				var li  = LineIndex = _textBox.GetLineIndexFromCharacterIndex(_textBox.SelectionStart);
				var lci = LineStart = _textBox.GetCharacterIndexFromLineIndex(li);
				var cp  = CharPosition = _textBox.SelectionStart - lci;
				var t   = _textBox.GetLineText(li);
				var sp  = Regex.Match(t, @"^\x20*");

				IsInIndentRegion = cp <= sp.Length;
				IndentPosition   = IsInIndentRegion ? cp / 4 : -1;
			}

		}
	}
}
