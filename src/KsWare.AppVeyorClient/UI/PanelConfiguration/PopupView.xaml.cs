using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using KsWare.Presentation.ViewFramework.Behaviors;

namespace KsWare.AppVeyorClient.UI.PanelConfiguration {

	/// <summary>
	/// Interaction logic for PopupView.xaml
	/// </summary>
	public partial class PopupView : Popup {

		public PopupView() {
			InitializeComponent();

			DragMoveThumb.DragDelta += (sender, e) => {
				HorizontalOffset += e.HorizontalChange;
				VerticalOffset += e.VerticalChange;
			};
			DataContextChanged += (s, e) => {
				if (DataContext is PopupVM vm) vm.View = this;
			};
		}
	}
}
