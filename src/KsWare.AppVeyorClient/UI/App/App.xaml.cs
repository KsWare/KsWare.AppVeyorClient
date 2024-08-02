using System.Diagnostics;

namespace KsWare.AppVeyorClient.UI.App {
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App {

		public App() {
			CatchUnhandledExceptions = !Debugger.IsAttached;
		}
	}
}
