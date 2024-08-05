using System.Windows.Controls.Primitives;

namespace KsWare.AppVeyorClient.UI.PanelYamlCodeEditor {

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
