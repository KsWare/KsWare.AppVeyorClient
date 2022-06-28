using System;
using System.Diagnostics;
using System.Threading.Tasks;
using JetBrains.Annotations;
using KsWare.AppVeyor.Api;
using KsWare.AppVeyorClient.Shared;
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
			Client.Base.SendAsync(Client.Base.CreateRequest("GET", Url, null, ContentType))
				.ContinueWithUIDispatcher(delegate(Task<string> task) {
				
					if (task.Exception != null) {
						ResultText = task.Exception.ToString();
						return;
					}
					switch (ContentType) {
						case "application/json":
							try {
								var result = JsonConvert.SerializeObject(JsonConvert.DeserializeObject(task.Result), Formatting.Indented);
							}
							catch (Exception ex2) {}
							break;
					}

					Debug.WriteLine($"ResultText changing");
					ResultText = task.Result;
				});

		}
	}

}
