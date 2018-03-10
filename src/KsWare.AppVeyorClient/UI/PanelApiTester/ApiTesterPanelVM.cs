using System;
using System.Diagnostics;
using JetBrains.Annotations;
using KsWare.AppVeyorClient.Api;
using KsWare.AppVeyorClient.UI.App;
using KsWare.Presentation.ViewModelFramework;
using Newtonsoft.Json;

namespace KsWare.AppVeyorClient.UI.PanelApiTester {

	public class ApiTesterPanelVM : ObjectVM {

		public ApiTesterPanelVM() {
			RegisterChildren(() => this);
			Url         = "/api/users";
			ContentType = "application/json";
			Fields[nameof(ResultText)].ValueChangedEvent.add = (s, e) => Debug.WriteLine($"ResultText Changed");
			
		}

		private Client Client => AppVM.Client;

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

		public string ContentType { get => Fields.GetValue<string>(); set => Fields.SetValue(value); }

		/// <summary>
		/// Method for <see cref="TestAction"/>
		/// </summary>
		[UsedImplicitly]
		private async void DoTest() {

		}

		/// <summary>
		/// Method for <see cref="SendAction"/>
		/// </summary>
		[UsedImplicitly]
		private void DoSend() {
			var result = Client.Base.Send("GET", Url, null, ContentType, out var ex);
			if (ex != null) {
				result = ex?.ToString();
				return;
			}
			switch (ContentType) {
				case "application/json":
					try {
						result = JsonConvert.SerializeObject(JsonConvert.DeserializeObject(result), Formatting.Indented);
					}
					catch (Exception ex2) {}
					break;
			}

			Debug.WriteLine($"ResultText changing");
			ResultText=result;
		}
	}

}
