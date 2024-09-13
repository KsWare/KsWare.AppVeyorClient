using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Threading;
using KsWare.Presentation.Resources.Core;

namespace KsWare.AppVeyorClient.UI.App {

	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App {

		public App() {
			CatchUnhandledExceptions = !Debugger.IsAttached;
			ThemeManager.RegisterTheme("Aero2Dark","/KsWare.Presentation.Themes.Aero2Dark;component/Resources/Aero2Dark.NormalColor.xaml",true);
		}

		protected override void OnStartup(StartupEventArgs e) {
			base.OnStartup(e);

			// Hauptfenster referenzieren
			var mainWindow = Application.Current.MainWindow;

			// Verhindere, dass das Fenster sichtbar ist, bevor alle Ressourcen geladen sind
			mainWindow.Visibility = Visibility.Hidden;

			// Warte, bis die Anwendung bereit ist (inkl. Theme-Umschaltung)
			Application.Current.Dispatcher.InvokeAsync(() => {
				mainWindow.Visibility = Visibility.Visible;
			}, DispatcherPriority.ApplicationIdle); // Wartet, bis alle Hintergrundprozesse abgeschlossen sind
		}

	}
}
