using System;
using System.Windows;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Document;
using KsWare.Presentation.Core.Providers;
using KsWare.Presentation.ViewModelFramework;

namespace KsWare.AppVeyorClient.Shared.AvalonEditExtension {

	public class TextEditorControllerVM:DataVM<TextEditor> {

		TextEditorData _data=new TextEditorData();

		public TextEditorControllerVM() {
			RegisterChildren(()=>this);

			IsEnabledChanged+=AtIsEnabledChanged;
		}

		private void AtIsEnabledChanged(object sender, RoutedPropertyChangedEventArgs<bool> e) { Data.IsEnabled = e.NewValue; }

		public string SelectedText {
			get {
				return Data?.SelectedText;
			}
			set {
				if(Data==null) throw new InvalidOperationException("View is not connected.");
				Data.SelectedText = value;
			}
		}

		public string Text {
			get { return Data == null ? _data.Text : Data.Text; }
			set {
				if (Data == null) _data.Text=value;
				else Data.Text = value;
				OnTextChanged(Data.Document);
			}
		}

		protected virtual void OnTextChanged(object changedRegion) {

		}

		protected sealed override void OnDataChanged(DataChangedEventArgs e) {
			base.OnDataChanged(e);
			if (e.NewData != null) {
				OnViewConnected();
			}
		}

		protected virtual void OnViewConnected() {
			Data.Text = _data.Text;
			Data.IsEnabled = _data.IsEnabled;
		}

		public DocumentPosition SelectionStartPosition => Data.GetSelectionStartPosition();

		public DocumentPosition SelectionEndPosition => Data.GetSelectionEndPosition();

		public DocumentPosition CarretPosition => Data.GetCaretPosition();

		private class TextEditorData {
			public bool IsEnabled { get; set; } = true;
			public string Text { get; set; }
		}
	}



}
