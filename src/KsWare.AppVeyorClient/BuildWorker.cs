using System.Threading.Tasks;
using KsWare.AppVeyorClient.Api.Contracts;
using KsWare.AppVeyorClient.Api.Contracts.Common;

namespace KsWare.AppVeyorClient {

	public class BuildWorker {
		private readonly HttpClientEx _client;

		public BuildWorker(HttpClientEx client) { _client = client; }

		// POST api/build/messages
		public async Task AddMessage(string message, string category = "information", string details = null) {
			await _client.PostJsonAsync($"/api/build/messages", new BuildMessage(message, category, details));
		}

		// POST api/build/compilationmessages
		public async Task AddCompilationMessage(CompilationMessage message) {
			await _client.PostJsonAsync($"/api/build/compilationmessages", message);
		}

		// POST api/build/variables
		public async Task SetEnvironmentVariable(string name, string value) {
			await _client.PostJsonAsync($"/api/build/compilationmessages", new Variable(name, value));
		}
	}

}