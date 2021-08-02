using System.Windows;
using System.Windows.Controls;
using KsWare.AppVeyorClient.UI.App;

namespace KsWare.AppVeyorClient.UI.PanelCommon {

	/// <summary>
	/// Interaction logic for CommonPanelView.xaml
	/// </summary>
	public partial class CommonPanelView : UserControl {

		public CommonPanelView() {
			InitializeComponent();
			AppVM.Client.Base.TokenChanged += (s, e) => TokenPasswordBox.Password = AppVM.Client.Base.UnsecureToken;
			TokenPasswordBox.Password      =  AppVM.Client.Base.UnsecureToken;
		}

		private void AtTokenTextBoxLostFocus(object sender, RoutedEventArgs e) {
			var pb = (PasswordBox) sender;
			if (pb.SecurePassword.Length == 0) return; // do not store empty password
			AppVM.StoreToken(pb.SecurePassword);
		}

		private void AtAccountTextBoxBoxLostFocus(object sender, RoutedEventArgs e) {

		}
	}
}
