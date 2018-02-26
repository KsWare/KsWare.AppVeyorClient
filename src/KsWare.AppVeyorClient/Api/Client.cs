using System.Security;

namespace KsWare.AppVeyorClient.Api {

	public class Client {

		private HttpClientEx _httpClientEx;

		public Client(SecureString token) {
			_httpClientEx=new HttpClientEx(token) {
				Server = "ci.appveyor.com"
			};
			BuildWorker =new BuildWorker(_httpClientEx);
			Project=new ProjectClient(_httpClientEx);
		}

		public Client(string token) : this(HttpClientEx.CreateSecureToken(token)) { }

		public ProjectClient Project { get; }

		public BuildWorker BuildWorker  { get;}

		public HttpClientEx Base => _httpClientEx;

		public void SetToken(SecureString secureToken) {
			_httpClientEx.SetToken(secureToken);
		}
	}

}
