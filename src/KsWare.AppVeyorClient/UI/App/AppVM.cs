using System;
using System.Diagnostics;
using System.IO;
using System.Security;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Threading;
using JetBrains.Annotations;
using KsWare.AppVeyor.Api;
using KsWare.AppVeyor.Api.Shared;
using KsWare.AppVeyorClient.UI.ViewModels;
using KsWare.Presentation.ViewModelFramework;

namespace KsWare.AppVeyorClient.UI.App {

	public class AppVM : ApplicationVM {

		public new static AppVM Current => (AppVM)ApplicationVM.Current;
		private static bool _firstLoad = true;

		public AppVM() {
			RegisterChildren(() => this);
			StartupUri = typeof(MainWindowVM);
		}

		protected override void OnStartup(StartupEventArgs e) {
			base.OnStartup(e);
			Application.Current.Dispatcher.Invoke(DispatcherPriority.ApplicationIdle, new Action(OnStartIdle));

			var args = Environment.GetCommandLineArgs();
			if (args.Length > 1) {
				if (Regex.IsMatch(args[1], @"^([/-][h?]|--help)$")) 
					Application.Current.Dispatcher.Invoke(DispatcherPriority.ApplicationIdle,new Action(() => new CommandLineHelpWindow().ShowDialog()));
			}
		}

		private void OnStartIdle() {
			Settings.Load();
		}

		internal static Client Client { get; } = new Client("");

		internal static FileStore FileStore { get; private set; }

		public SettingsVM Settings { get; [UsedImplicitly] private set; }

		internal static void StoreToken([NotNull]SecureString secureToken) {
			if(Current.Settings.SaveToken==false) return;
			if (secureToken == null) throw new ArgumentNullException(nameof(secureToken));

			var path = Path.Combine(Path.GetDirectoryName(SettingsVM.FilePath), "{97E1F04E-A097-477B-A29A-92889BE79C39}");
			Directory.CreateDirectory(Path.GetDirectoryName(SettingsVM.FilePath));

			byte[] plaintext = Encoding.UTF8.GetBytes(
				System.Runtime.InteropServices.Marshal.PtrToStringAuto(
					System.Runtime.InteropServices.Marshal.SecureStringToBSTR(secureToken)));

//			byte[] plaintext = Encoding.UTF8.GetBytes(unsecureToken);

			// Generate additional entropy (will be used as the Initialization vector)
			byte[] entropy = new byte[20];
			using (RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider()) 
				rng.GetBytes(entropy);

			byte[] ciphertext = ProtectedData.Protect(plaintext, entropy, DataProtectionScope.CurrentUser);

			int chk = 0;
			foreach (var b in ciphertext) chk += b;
			Debug.WriteLine(chk);


			//			IsolatedStorageFile isoStore =
			//				IsolatedStorageFile.GetStore(IsolatedStorageScope.User | IsolatedStorageScope.Machine, null, null);
			//			using (var isoStream = new IsolatedStorageFileStream("Token", FileMode.Create, isoStore)) {
			using (var isoStream = File.Create(path)) {

				using (var writer = new BinaryWriter(isoStream)) {
					writer.Write(entropy);
					writer.Write(ciphertext.Length);
					writer.Write(ciphertext);
					writer.Flush();
				}
			}
			Client.SetToken(secureToken);
		}

		internal static void LoadToken() {
			var path = Path.Combine(Path.GetDirectoryName(SettingsVM.FilePath), "{97E1F04E-A097-477B-A29A-92889BE79C39}");
			Directory.CreateDirectory(Path.GetDirectoryName(SettingsVM.FilePath));

//			var isoStore = IsolatedStorageFile.GetStore(IsolatedStorageScope.User|IsolatedStorageScope.Machine, null, null);
//			if (!isoStore.FileExists("Token")) return;
			if (!File.Exists(path)) return;

			byte[] entropy;
			byte[] ciphertext;
			//			using (var isoStream = new IsolatedStorageFileStream("Token", FileMode.Open, isoStore)
			using (var isoStream = File.OpenRead(path)
			) {

				using (var reader = new BinaryReader(isoStream)) {
					entropy = reader.ReadBytes(20);
					ciphertext = reader.ReadBytes(reader.ReadInt32());
				}
			}

			byte[] plaintext;
			try {
				plaintext = ProtectedData.Unprotect(ciphertext, entropy, DataProtectionScope.CurrentUser);
			}
			catch (Exception ex) {
				MessageBox.Show("Error", "Can not restore token. \n\n" + ex.Message);
				return;
			}
			var ss = new SecureString();
			foreach (var c in Encoding.UTF8.GetString(plaintext)) ss.AppendChar(c);
			Client.SetToken(ss);
		}

		internal static void InitFileStore() {
			var path = Path.Combine(Path.GetDirectoryName(SettingsVM.FilePath), "Cache");
			Directory.CreateDirectory(path);
			FileStore=FileStore.Instance=new FileStore(path);
		}

		public static void OnMainWindowLoading() {
			if (_firstLoad == false) return;
			_firstLoad = false;
			AppVM.LoadToken(); // TODO load after UI loaded
			AppVM.InitFileStore(); // TODO load after UI loaded
		}
	}
}
