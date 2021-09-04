using System;
using System.Security;
using System.Threading.Tasks;

namespace KsWare.AppVeyor.Api {

	public class Client {

		private HttpClientEx _httpClientEx;

		public Client(SecureString token) {
			_httpClientEx=new HttpClientEx( token) {
				BaseUri = new Uri("https://ci.appveyor.com/")
			};
			BuildWorker =new BuildWorker(_httpClientEx);
			Project = new ProjectClient(_httpClientEx);
			Team = new TeamClient(_httpClientEx);
			ClientOnTokenChanged(null, null);
		}

		private void ClientOnTokenChanged(object sender, EventArgs e) {
			ApiVersion = "v1";
			if ($"{_httpClientEx.InsecureToken}".StartsWith("v2.")) ApiVersion = "v2";
		}

		public string ApiVersion { get; private set; }

		public Client(string token) : this(HttpClientEx.CreateSecureToken(token)) { }

		public ProjectClient Project { get; }

		public TeamClient Team { get; }

		public BuildWorker BuildWorker  { get;}

		public HttpClientEx Base => _httpClientEx;

		public void SetToken(SecureString secureToken) {
			_httpClientEx.SetToken(secureToken);
		}

		/// <summary>
		/// Encrypts the specified value.
		/// </summary>
		/// <param name="plainValue">The value to encrypt.</param>
		/// <param name="accountName">Account name. Mandatory vor v2 API.</param>
		/// <returns></returns>
		public Task<string> Encrypt(string plainValue, string accountName = null) {
			// POST https://ci.appveyor.com/api/account/encrypt
			// { "plainValue": "value_to_encrypt" }
			// var 
			string api;
			switch (ApiVersion) {
				case "v2":
					if (string.IsNullOrWhiteSpace(accountName)) throw new ArgumentNullException(nameof(accountName), "Value must not null or empty.");
					api = $"api/account/{accountName}/encrypt"; //v1+v2
					break;
				default: 
					api = $"api/account/encrypt";  //v1
					break;
			}
			var content = $@"{{""plainValue"":""{plainValue}""}}";
			return _httpClientEx.PostJsonAsync<string>(api, content);
		}

		// /// <summary>
		// /// Decrypts the specified value.
		// /// </summary>
		// /// <param name="encryptedValue">The value to decrypt.</param>
		// /// <param name="accountName">Account name. Mandatory vor v2 API.</param>
		// /// <returns></returns>
		// public Task<string> Decrypt(string encryptedValue, string accountName = null) {
		// 	// POST https://ci.appveyor.com/api/account/encrypt
		// 	// { "plainValue": "value_to_encrypt" }
		// 	// var 
		// 	string api;
		// 	switch (ApiVersion) {
		// 		case "v2":
		// 			if (string.IsNullOrWhiteSpace(accountName)) throw new ArgumentNullException(nameof(accountName), "Value must not null or empty.");
		// 			api = $"api/account/{accountName}/encrypt"; //v1+v2
		// 			break;
		// 		default: 
		// 			api = $"api/account/encrypt";  //v1
		// 			break;
		// 	}
		// 	var content = $@"{{""plainValue"":""{encryptedValue}""}}";
		// 	return _httpClientEx.PostJsonAsync<string>(api, content);
		// }
	}

}
