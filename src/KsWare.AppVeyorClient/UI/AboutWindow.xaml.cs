using System.Diagnostics;
using System.Reflection;
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

		public AboutWindowVM() {
			var a = Assembly.GetExecutingAssembly();
			Version = "v" + a.GetName().Version.ToString(3);
			Copyright = a.GetCustomAttribute<AssemblyCopyrightAttribute>()?.Copyright;
		}
		public string Copyright { get; }
		public string Version { get; }
	}
}
