using System.Threading.Tasks;
using JetBrains.Annotations;
using KsWare.AppVeyor.Api.Contracts;

namespace KsWare.AppVeyor.Api {

	public class CommonClient {

		private readonly HttpClientEx _client;

		public CommonClient(HttpClientEx client) { _client = client; }

		public async Task<string> GetGlobalYamlAsync([CanBeNull] string accountName) {
			// GET /api/account/<Account>/global-yaml
			var api = $"/api/account/{accountName}/global-yaml";
			var response = await _client.GetTextAsync(api).ConfigureAwait(false);
			return response;
		}

		public Task<ValidateResult> PutGlobalYamlAsync(string yaml, [CanBeNull] string accountName) {
			// PUT /api/account/<Account>/global-yaml
			var api = $"/api/account/{accountName}/global-yaml";
			return _client.PutTextAsync<ValidateResult>(api, yaml);
		}
	}
}

/*

   PUT ci.appveyor.com/api/account/KsWare/global-yaml
   accept: application/json, text/plain, * /*
   response
   {"isValid":true,"line":0,"column":0}
   {
       "isValid": false,
       "errorMessage": "Invalid setting or section: -ps (Line: 2, Column: 1)",
       "line": 2,
       "column": 1
   }
   
   GET https://ci.appveyor.com/api/account/KsWare/global-yaml
   Content-Encoding: gzip
   Content-Type: text/plain
   
   
 */
