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
using System.Windows.Shapes;

namespace KsWare.AppVeyorClient.UI {

	/// <summary>
	/// Interaction logic for CommandLineHelpWindow.xaml
	/// </summary>
	public partial class CommandLineHelpWindow : Window {

		public CommandLineHelpWindow() {
			InitializeComponent();
		}

		private void CloseButton_OnClick(object sender, RoutedEventArgs e) {
			this.Close();
		}
	}
}
