using System.Diagnostics;
using System.Windows;
using System.Windows.Documents;
using KsWare.Presentation.ViewModelFramework;

namespace KsWare.AppVeyorClient.UI {

	/// <summary>
	/// Interaction logic for AboutWindow.xaml
	/// </summary>
	public partial class AboutWindow : Window {

		public AboutWindow() {
			InitializeComponent();
		}

		private void Hyperlink_OnClick(object sender, RoutedEventArgs e) {
			e.Handled = true;
			var url = ((Hyperlink)sender).NavigateUri.ToString();
			Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
		}
	}

	public class AboutWindowVM : DialogWindowVM {

	}
}
