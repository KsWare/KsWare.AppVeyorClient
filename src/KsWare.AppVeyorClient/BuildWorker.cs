using System.Threading.Tasks;
using KsWare.AppVeyorClient.Api.Contracts;
using KsWare.AppVeyorClient.Api.Contracts.Common;

namespace KsWare.AppVeyorClient {

	public class BuildWorker:BaseClient {

		public BuildWorker(BaseClient client):base(client) { }

		// POST api/build/messages
		public async Task AddMessage(string message, string category = "information", string details = null) {
			await PostJson($"/api/build/messages", new BuildMessage(message, category, details));
		}

		// POST api/build/compilationmessages
		public async Task AddCompilationMessage(CompilationMessage message) {
			await PostJson($"/api/build/compilationmessages", message);
		}

		// POST api/build/variables
		public async Task SetEnvironmentVariable(string name, string value) {
			await PostJson($"/api/build/compilationmessages", new Variable(name, value));
		}
	}

}