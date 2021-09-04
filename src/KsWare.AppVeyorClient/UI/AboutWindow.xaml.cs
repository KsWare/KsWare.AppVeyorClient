using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace KsWare.AppVeyorClient.UI {
	/// <summary>
	/// Interaction logic for AboutWindow.xaml
	/// </summary>
	public partial class AboutWindow : Window {
		public AboutWindow() {
			InitializeComponent();
		}

		private void Button_Click(object sender, RoutedEventArgs e) {
			Close();
		}

		private void Hyperlink_OnClick(object sender, RoutedEventArgs e) {
			e.Handled = true;
			var url = ((Hyperlink)sender).NavigateUri.ToString();
			Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
		}
	}
}
