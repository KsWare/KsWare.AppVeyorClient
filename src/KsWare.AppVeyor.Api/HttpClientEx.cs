using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using Common.Logging;
using JetBrains.Annotations;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace KsWare.AppVeyor.Api {

	public sealed class HttpClientEx {

		// TODO move out of class
		private static readonly JsonSerializerSettings JsonSerializerSettings = new JsonSerializerSettings {
			ContractResolver = new CamelCasePropertyNamesContractResolver(),
		};

		private static readonly ILog Log = LogManager.GetLogger<HttpClientEx>();

		private SecureString _secureToken;
		private readonly HttpClient _httpClient;

		public HttpClientEx(SecureString token) {
			_secureToken = token;
			_httpClient = new HttpClient();
		}

		public HttpClientEx(string insecureToken) : this(CreateSecureToken(insecureToken)) {}

		public Uri BaseUri { get; set; }

		public bool HasToken => _secureToken != null && _secureToken.Length > 0;

		public event EventHandler TokenChanged;

		public static SecureString CreateSecureToken(string insecureToken) {
			var s = new SecureString();
			if (insecureToken == null) return s;
			foreach (var c in insecureToken) s.AppendChar(c);
			return s;
		}

		public void SetToken(SecureString secureToken) {
			_secureToken = secureToken;
			TokenChanged?.Invoke(this, EventArgs.Empty);
		}

		internal string InsecureToken =>
			System.Runtime.InteropServices.Marshal.PtrToStringAuto(
				System.Runtime.InteropServices.Marshal.SecureStringToBSTR(_secureToken));

//		private string GetUrl(string api) {
//			if(string.IsNullOrWhiteSpace(Protocol)) throw new InvalidOperationException("Protocol not specified.");
//			if(string.IsNullOrWhiteSpace(Server)) throw new InvalidOperationException("Server not specified.");
//			if (!api.StartsWith("/")) api = "/" + api;
//			return $"{Protocol}://{Server}{api}";
//			return new Uri(BaseUri, api);
//		}

		public async Task<T> GetJsonAsync<T>(string api) {
			var content = await SendAsync("GET", api, null, null);
			return FromJson<T>(content);
		}

		/// <summary>
		/// Gets or sets a value indicating whether returned json result shall validated.
		/// </summary>
		/// <value><c>true</c> if json result shall validated; otherwise, <c>false</c>.</value>
		[PublicAPI]
		public bool ValidateJsonResult { get; set; }

		// public string GetJsonText(string api) => TaskExtensions.RunSync(async () => await GetJsonTextAsync(api));

		// public string GetJsonText(string api, out Exception exception) => TaskExtensions.RunSync(async () => await GetJsonTextAsync(api),out exception);

		public Task<string> GetJsonTextAsync(string api) => SendAsync("GET", api);

		public Task<string> GetTextAsync(string api) => SendAsync("GET", api);

		public Task PutTextAsync(string api, string text) => SendAsync("PUT", api, text, "text/plain");

		[Obsolete("use PostJsonAsync")]
		public Task PutJsonTextAsync(string api, string jsonString) {
			return SendAsync("PUT", api, jsonString, "application/json");
		}

		public Task PutJsonAsync(string api, object json) {
			var jsonString = json is string s ? s : ToJsonString(json);
			return SendAsync("PUT", api, jsonString, "application/json");
		}

		public Task PostJsonAsync(string api, object json) {
			var jsonString = json is string s ? s : ToJsonString(json);
			return SendAsync("POST", api, jsonString, "application/json");
		}

		public async Task<T> PostJsonAsync<T>(string api, object json) {
			var jsonString = json is string s ? s : ToJsonString(json);
			var content = await SendAsync("POST", api, jsonString, "application/json");
			return typeof(T) == typeof(string) ? (T)(object)content : FromJson<T>(content);
		}

		public async Task<T> PostTextAsync<T>(string api, string content) {
			var responseText = await SendAsync("POST", api, content, "text/plain");
			return typeof(T) == typeof(string) ? (T)(object)content : FromJson<T>(responseText);
		}

		private T FromJson<T>(string content) {
			try {
				// return JsonConvert.DeserializeObject<T>(content);

				var o = JsonConvert.DeserializeObject<T>(content, JsonSerializerSettings);
				if (ValidateJsonResult) {
					var ot = JsonConvert.SerializeObject(o,Formatting.Indented, JsonSerializerSettings);
					var j = JsonConvert.DeserializeObject(content, JsonSerializerSettings);
					var jt = JsonConvert.SerializeObject(j, Formatting.Indented, JsonSerializerSettings);
					if (ot != jt) {
						Debug.WriteLine($"JSON: incomplete deserialized! {typeof(T).FullName}");
						var p1=Path.Combine(Path.GetTempPath(), "mine - {C29066B1-293D-4CAA-AE55-5EF68764F184}.json");
						var p2=Path.Combine(Path.GetTempPath(), "origin - {00E1C157-7B19-41C6-87AB-DAA57EFC3D9D}.json");
						File.WriteAllText(p1,ot);
						File.WriteAllText(p2,jt);
						var winMerge = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonProgramFilesX86), @"WinMerge\WinMergeU.exe");
						if(File.Exists(winMerge)) Process.Start(winMerge, $"\"{p1}\" \"{p2}\"");
					}
				}
				return o;
			}
			catch (Exception ex) {
				ex.Data.Add("ResponseText", content);
				throw;
			}
		}

		// public string Send(HttpRequestMessage message, out Exception exception) =>
		// 	TaskExtensions.RunSync(async () => await SendAsync(message), out exception);

		public async Task<string> SendAsync(HttpRequestMessage message) {
			Debug.WriteLine($"{message.Method} {message.RequestUri.PathAndQuery}");

			using (var response = await _httpClient.SendAsync(message)){
				Log.Trace(response.StatusCode);
				//TODO response.Content.Headers.ContentType
				var responseContent = await response.Content.ReadAsStringAsync();
				if (!response.IsSuccessStatusCode) {
					//  400 (Invalid input parameters. See response body for detailed error message.)
					// {"message":"The request is invalid.","modelState":{"variables.Headers":["An error has occurred."]}}
					try {
						response.EnsureSuccessStatusCode();
					}
					catch (Exception ex) {
						ex.Data.Add("Response.StatusCode", response.StatusCode);
						ex.Data.Add("Response",            response.ToString());
						ex.Data.Add("Response.Body",       responseContent);
						throw;
					}
				}
				return responseContent;
			}
		}

		private async Task<string> SendAsync(string method, string api, string content = null, string contentType = null) {
			var message = CreateRequest(method, api, content, contentType);
			return await SendAsync(message);
		}

		// public string Send(string method, string api, string content, string contentType, out Exception exception) {
		// 	var message = CreateRequest(method, api, content, contentType);
		// 	return Send(message, out exception);
		// }

		public HttpRequestMessage CreateRequest(string method, string api, string content, string contentType) {
			var m = new HttpMethod(method.ToUpperInvariant());
			var r = new HttpRequestMessage(m, new Uri(BaseUri,api));
			if (contentType != null) r.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(contentType));
			r.Headers.Authorization = new AuthenticationHeaderValue("Bearer", InsecureToken); // make optional/configurable
			if (content != null) {
				//TODO validate contentType
				r.Content = new StringContent(content, Encoding.UTF8, contentType);
			}
			return r;
		}

		private string ToJsonString(object json) {
			return JsonConvert.SerializeObject(json, settings: JsonSerializerSettings);
		}

		public async Task Delete(string api, HttpStatusCode expectedStatusCode = HttpStatusCode.OK) {
			var request = CreateRequest("DELETE", api, null, null);
			Debug.WriteLine($"{request.Method} {request.RequestUri?.PathAndQuery}");

			using (var response = await _httpClient.SendAsync(request)) {
				Log.Trace(response.StatusCode);
				//TODO response.Content.Headers.ContentType

				if (response.StatusCode == expectedStatusCode) return;

				var responseContent = await response.Content.ReadAsStringAsync();
				if (!response.IsSuccessStatusCode) {
					//  400 (Invalid input parameters. See response body for detailed error message.)
					// {"message":"The request is invalid.","modelState":{"variables.Headers":["An error has occurred."]}}
					try {
						response.EnsureSuccessStatusCode();
					}
					catch (Exception ex) {
						ex.Data.Add("Response.StatusCode", response.StatusCode);
						ex.Data.Add("Response", response.ToString());
						ex.Data.Add("Response.Body", responseContent);
						throw;
					}
				}
			}
		}
	}

	public static class HttpClientExLogExtension {
		public static void Trace(this ILog log, HttpStatusCode statusCode) {
			log.Trace($"done {(int)statusCode} {statusCode}");
		}
	}
}
