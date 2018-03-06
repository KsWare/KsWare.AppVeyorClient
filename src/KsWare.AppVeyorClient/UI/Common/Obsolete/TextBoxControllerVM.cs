using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using KsWare.Presentation.Core.Providers;
using KsWare.Presentation.ViewModelFramework;

namespace KsWare.AppVeyorClient.UI.Common {

	public class TextBoxControllerVM:DataVM<TextBox> {

		TextBoxData _data=new TextBoxData();

		public TextBoxControllerVM() {
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
			}
		}

		protected sealed override void OnDataChanged(DataChangedEventArgs e) {
			base.OnDataChanged(e);
			if (e.NewData != null) {
				OnViewConnected();
			}
		}

		protected virtual void OnViewConnected() {
			ActivePoint = new ActivePoint(Data);
			Data.Text = _data.Text;
			Data.IsEnabled = _data.IsEnabled;
		}

		public ActivePoint ActivePoint { get; private set; }
	}

	public class TextBoxData {
		public bool IsEnabled { get; set; } = true;
		public string Text { get; set; }
	}

}
