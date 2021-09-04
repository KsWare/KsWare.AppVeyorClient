namespace KsWare.AppVeyor.Api {

	public class TeamClient {

		private readonly HttpClientEx _client;

		public TeamClient(HttpClientEx client) { _client = client; }

		// public async Task GetUsers() {
		// 	// GET /api/users
		// 	var api = "/api/users";
		// 	var response = await _client.GetJsonAsync<GetProjectsResponse>(api);
		// }

		// public async Task<IEnumerable<string>> GetUserNames() {
		// 	// GET /api/users
		// 	var api = "/api/users";
		// 	var response = await _client.GetJsonAsync<GetProjectsResponse>(api);
		// 	return new List<string>(); //TODO
		// }
	}

}
// Teams (Users, Roles, Collaborators)
// https://www.appveyor.com/docs/api/team/
