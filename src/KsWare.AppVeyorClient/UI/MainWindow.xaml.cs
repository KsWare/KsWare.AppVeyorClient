using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using KsWare.AppVeyorClient.UI.App;
using Microsoft.Win32;
using Newtonsoft.Json;

namespace KsWare.AppVeyorClient.UI {

	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window {


		public MainWindow() {
			InitializeComponent();
			AppVM.Client.Base.TokenChanged += (s, e) => TokenPasswordBox.Password = AppVM.Client.Base.UnsecureToken;
			TokenPasswordBox.Password = AppVM.Client.Base.UnsecureToken;
		}

		private void AtTokenTextBoxLostFocus(object sender, RoutedEventArgs e) {
			var pb = (PasswordBox) sender;
			if(pb.SecurePassword.Length==0) return; // do not store empty password
			AppVM.StoreToken(pb.SecurePassword);
		}
	}

}

