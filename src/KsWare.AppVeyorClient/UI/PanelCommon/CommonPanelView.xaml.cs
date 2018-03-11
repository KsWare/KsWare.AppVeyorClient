using System;
using System.Collections.Generic;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
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
	}
}
