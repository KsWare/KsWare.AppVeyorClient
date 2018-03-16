﻿using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Document;
using JetBrains.Annotations;
using KsWare.AppVeyorClient.Shared.PresentationFramework;
using KsWare.Presentation.Core.Providers;
using KsWare.Presentation.ViewModelFramework;

namespace KsWare.AppVeyorClient.Shared.AvalonEditExtension {

	public class TextEditorControllerVM:DataVM<TextEditor> {

		TextEditorData _data=new TextEditorData();

		public TextEditorControllerVM() {
			RegisterChildren(() => this);

			IsEnabledChanged += AtIsEnabledChanged;

			ContextMenu.Items.Add(new MenuItemVM {Caption = "Cut", CommandAction   = {MːDoAction = () => Data.Cut()}});
			ContextMenu.Items.Add(new MenuItemVM {Caption = "Copy", CommandAction  = {MːDoAction = () => Data.Copy()}});
			ContextMenu.Items.Add(new MenuItemVM {Caption = "Paste", CommandAction = {MːDoAction = () => Data.Paste()}});
//			ContextMenu.Items.Add(new MenuItemVM {Caption = "Delete", CommandAction = {MːDoAction = () => Data.Delete()}});
//			ContextMenu.Items.Add(new MenuItemVM {Caption = "Undo", CommandAction = {MːDoAction = () => Data.Undo()}});
//			ContextMenu.Items.Add(new MenuItemVM {Caption = "Redo", CommandAction = {MːDoAction = () => Data.Redo()}});
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

			if (Data.ContextMenu != null) {
				Data.ContextMenu.DataContext = ContextMenu;
			}

//			Data.ContextMenu = new ContextMenu {
//				DataContext = ContextMenu, 
//				ItemsSource = ContextMenu.Items,
//				ItemTemplate = 
//			};
		}

		public ContextMenuVM ContextMenu { get; [UsedImplicitly] private set; }

		public DocumentPosition SelectionStartPosition => Data.GetSelectionStartPosition();

		public DocumentPosition SelectionEndPosition => Data.GetSelectionEndPosition();

		public DocumentPosition CarretPosition => Data.GetCaretPosition();

		private class TextEditorData {
			public bool IsEnabled { get; set; } = true;
			public string Text { get; set; }
		}
	}



}
