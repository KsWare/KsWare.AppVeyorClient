using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using JetBrains.Annotations;
using KsWare.AppVeyorClient.Api;
using KsWare.AppVeyorClient.Api.Contracts;
using KsWare.Presentation.ViewModelFramework;
using Newtonsoft.Json;

namespace KsWare.AppVeyorClient.UI {

	public class MainWindowVM : WindowVM {

		public MainWindowVM() {
			RegisterChildren(()=>this);
			AppVM.LoadToken(); // TODO load after UI loaded
			AppVM.InitFileStore(); // TODO load after UI loaded
			Url = "/api/users";
		}

		private Client Client => AppVM.Client;

		public ProjectEnvironmentVariablesVM ProjectEnvironmentVariablesPanel { get; [UsedImplicitly] private set; }
		public ConfigurationPanelVM ConfigurationPanel { get; [UsedImplicitly] private set; }

		public string Url { get => Fields.GetValue<string>(); set => Fields.SetValue(value); }
		public string ResultText { get => Fields.GetValue<string>(); set => Fields.SetValue(value); }

		/// <summary>
		/// Gets the <see cref="ActionVM"/> to Send
		/// </summary>
		/// <seealso cref="DoSend"/>
		public ActionVM SendAction { get; [UsedImplicitly] private set; }

		/// <summary>
		/// Gets the <see cref="ActionVM"/> to Test
		/// </summary>
		/// <seealso cref="DoTest"/>
		public ActionVM TestAction { get; [UsedImplicitly] private set; }

		/// <summary>
		/// Method for <see cref="TestAction"/>
		/// </summary>
		[UsedImplicitly]
		private async void DoTest() {
			var ps = await Client.Project.GetProjectSettings();
			var c = ps.Settings;
			c.NextBuildNumber = 26;
			await Client.Project.UpdateProjectSettings(c);
		}

		/// <summary>
		/// Method for <see cref="SendAction"/>
		/// </summary>
		[UsedImplicitly]
		private void DoSend() {
			var result = Client.Base.GetJsonText(Url, out var ex);
			ResultText = ex?.ToString() ?? JsonConvert.SerializeObject(JsonConvert.DeserializeObject(result), Formatting.Indented);
		}

	}
}
