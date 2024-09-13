using System.Windows;

namespace KsWare.AppVeyorClient.UI {

	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window {

		public MainWindow() {
			InitializeComponent();


			Loaded += (sender, args) => {
				PresentationSource source = PresentationSource.FromVisual(this);

				double dpiX, dpiY;
				if (source != null) {
					dpiX = 96.0 * source.CompositionTarget.TransformToDevice.M11;
					dpiY = 96.0 * source.CompositionTarget.TransformToDevice.M22;
				}
			};
		}
	}

}

