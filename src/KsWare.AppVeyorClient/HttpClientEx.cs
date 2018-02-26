using System;
using System.Diagnostics;
using System.Globalization;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Serialization;
using JetBrains.Annotations;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

namespace KsWare.AppVeyorClient {

	public sealed class HttpClientEx {

		// TODO remove out of class
		private static readonly JsonSerializerSettings JsonSerializerSettings = new JsonSerializerSettings {
			ContractResolver = new CamelCasePropertyNamesContractResolver()
		};

		private SecureString _secureToken;
		private readonly HttpClient _httpClient;

		public HttpClientEx(SecureString token) {
			_secureToken = token;
			_httpClient = new HttpClient();
			_httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
			_httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("plain/text"));
			_httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", UnsecureToken);
		}

		public HttpClientEx(string unsecureToken) : this(CreateSecureToken(unsecureToken)) {}

		public string Protocoll { get; set; } = "https";

		public string Server { get; set; }

		public bool HasToken => _secureToken != null && _secureToken.Length > 0;

		public static SecureString CreateSecureToken(string unsecureToken) {
			var s = new SecureString();
			if (unsecureToken != null) foreach (var c in unsecureToken) s.AppendChar(c);
			return s;
		}

		public void SetToken(SecureString secureToken) {
			_secureToken = secureToken;
			_httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", UnsecureToken); // TODO unsecure
//			TokenChanged?.BeginInvoke(this, EventArgs.Empty, null, null);
		}

		private string UnsecureToken =>
			System.Runtime.InteropServices.Marshal.PtrToStringAuto(
				System.Runtime.InteropServices.Marshal.SecureStringToBSTR(_secureToken));


		string GetUrl(string api) {
			if(string.IsNullOrWhiteSpace(Protocoll)) throw new InvalidOperationException("Protocol not specified.");
			if(string.IsNullOrWhiteSpace(Server)) throw new InvalidOperationException("Server not specified.");
			if (!api.StartsWith("/")) api = "/" + api;
			return $"{Protocoll}://{Server}{api}";
		}

		public async Task<JToken[]> GetJTokensAsync(string api) {
			Debug.WriteLine($"GET {api}");

			using (var response = await _httpClient.GetAsync(GetUrl(api))) {
				Log(response.StatusCode);
				response.EnsureSuccessStatusCode();

				var content = await response.Content.ReadAsAsync<JToken[]>();
				return content;
			}
		}
	
		public async Task<T> GetJsonAsync<T>(string api) {
			Debug.WriteLine($"GET {api}");

			using (var response = await _httpClient.GetAsync(GetUrl(api))) {
				Log(response.StatusCode);
				response.EnsureSuccessStatusCode();

				var text = await response.Content.ReadAsStringAsync();
				try {
					return JsonConvert.DeserializeObject<T>(text);
				}
				catch (Exception ex) {
					ex.Data.Add("ResponseText",text);
					throw;
				}
			}
		}

		public string GetJsonText(string api) {
			return RunSync(async () => {
				Debug.WriteLine($"GET {api}");

				using (var response = await _httpClient.GetAsync(GetUrl(api))) {
					Log(response.StatusCode);
					response.EnsureSuccessStatusCode();

					var content = await response.Content.ReadAsStringAsync();
					return content;
				}
			});
		}

		public string GetJsonText(string api, out Exception exception) {
			return RunSync(async () => {
				Debug.WriteLine($"GET {api}");
				// get the list of roles
				using (var response = await _httpClient.GetAsync(GetUrl(api))) {
					Log(response.StatusCode);
					response.EnsureSuccessStatusCode();
					var content = await response.Content.ReadAsStringAsync();
					return content;
				}
			},out exception);
		}

		public async Task<string> GetJsonTextAsync(string api) {
			Debug.WriteLine($"GET {api}");
			using (var client = new HttpClient()) {
				client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
				client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", UnsecureToken);

				// get the list of roles
				using (var response = await client.GetAsync(GetUrl(api))) {
					Log(response.StatusCode);
					response.EnsureSuccessStatusCode();

					var content = await response.Content.ReadAsStringAsync();
					return content;
				}
			}
		}

		public async Task<string> GetTextAsync(string api) {
			Debug.WriteLine($"GET {api}");

			using (var response = await _httpClient.GetAsync(GetUrl(api))) {
				Log(response.StatusCode);
				response.EnsureSuccessStatusCode();

				var content = await response.Content.ReadAsStringAsync();
				return content;
			}
		}

		public async Task PutTextAsync(string api, string text) {
			Debug.WriteLine($"PUT {api} => text: ...");
			await SendAsync("PUT", api, text, "plain/text");
		}

		public async Task PutJsonAsync(string api, object json) {
			Debug.WriteLine($"PUT {api} => json: ...");
			var jsonString = ToJsonString(json);
			await SendAsync("PUT", api, jsonString, "application/json");
		}

		public async Task PutJsonTextAsync(string api, string jsonString) {
			Debug.WriteLine($"PUT {api} => json: ...");
			await SendAsync("PUT", api, jsonString, "application/json");
		}

		public async Task PostJsonAsync(string api, object json) {
			Debug.WriteLine($"POST {api} => json: ...");
			var jsonString = JsonConvert.SerializeObject(json);
			await SendAsync("POST", api, jsonString, "application/json");
		}

//		private async Task<T> SendAsync<T>(string method, string api, string content, string contentType) {
//
//		}

		private async Task<string> SendAsync(string method, string api, string content, string contentType) {
			var httpContent = new StringContent(content, Encoding.UTF8, contentType);
			using (var response = await _httpClient.SendAsync(method,GetUrl(api), httpContent)) {
				Log(response.StatusCode);
				//TODO response.Content.Headers.ContentType
				var responseContent = await response.Content.ReadAsStringAsync(); 
				if (!response.IsSuccessStatusCode) {
					//  400 (Invalid input parameters. See response body for detailed error message.)
					// {"message":"The request is invalid.","modelState":{"variables.Headers":["An error has occurred."]}}
					try {response.EnsureSuccessStatusCode();}
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

		

		private string ToJsonString(object json) {
			return JsonConvert.SerializeObject(json,settings: JsonSerializerSettings);
		}

		

		public async Task PutXml<T>(string api, T o) {
			var content = ToXmlContent(o);
			using (var client = new HttpClient()) {
				client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", UnsecureToken);
				using (var response = await client.PutAsXmlAsync(GetUrl(api), content)) {
					Log(response.StatusCode);
					response.EnsureSuccessStatusCode();
				}
			}
		}

		public async Task PostXml<T>(string api, T o) {
			var content = ToXmlContent(o);
			using (var client = new HttpClient()) {
				client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", UnsecureToken);
				using (var response = await client.PostAsXmlAsync(GetUrl(api), content)) {
					Log(response.StatusCode);
					response.EnsureSuccessStatusCode();
				}
			}
		}

		private static void Log(HttpStatusCode statusCode) {
			Debug.WriteLine($"done {(int)statusCode} {statusCode}");
		}

		public static StringContent ToXmlContent<T>(T o) {
			var stringwriter = new System.IO.StringWriter();
			var serializer   = new XmlSerializer(typeof(T));
			serializer.Serialize(stringwriter, o);
			return new StringContent(stringwriter.ToString(),Encoding.UTF8, "application/xml");
		}

		public static T FromXmlString<T>(string xmlText) {
			var stringReader = new System.IO.StringReader(xmlText);
			var serializer   = new XmlSerializer(typeof(T));
			return (T)serializer.Deserialize(stringReader);
		}

		private static readonly TaskFactory _myTaskFactory = new TaskFactory(CancellationToken.None, TaskCreationOptions.None,
			TaskContinuationOptions.None, TaskScheduler.Default);

		public static T RunSync<T>(Func<Task<T>> func) {
			var cultureUi = CultureInfo.CurrentUICulture;
			var culture   = CultureInfo.CurrentCulture;
			return _myTaskFactory.StartNew<Task<T>>(delegate {
				Thread.CurrentThread.CurrentCulture   = culture;
				Thread.CurrentThread.CurrentUICulture = cultureUi;
				return func();
			}).Unwrap<T>().GetAwaiter().GetResult();
		}
		// Helper.RunSync(new Func<Task<ReturnTypeGoesHere>>(async () => await AsyncCallGoesHere(myparameter)));

		public static RunSyncResult<T> RunSync2<T>(Func<Task<T>> func) {
			try {
				var cultureUi = CultureInfo.CurrentUICulture;
				var culture   = CultureInfo.CurrentCulture;
				var r = _myTaskFactory.StartNew<Task<T>>(delegate {
					Thread.CurrentThread.CurrentCulture   = culture;
					Thread.CurrentThread.CurrentUICulture = cultureUi;
					return func();
				}).Unwrap<T>().GetAwaiter().GetResult();
				return new RunSyncResult<T>(r);
			}
			catch (Exception ex) {
				return new RunSyncResult<T>(ex);
			}
		}

		public static T RunSync<T>(Func<Task<T>> func, out Exception exception) {
			exception = null;
			try {
				var cultureUi = CultureInfo.CurrentUICulture;
				var culture   = CultureInfo.CurrentCulture;
				return _myTaskFactory.StartNew<Task<T>>(delegate {
					Thread.CurrentThread.CurrentCulture   = culture;
					Thread.CurrentThread.CurrentUICulture = cultureUi;
					return func();
				}).Unwrap<T>().GetAwaiter().GetResult();
			}
			catch (Exception ex) {
				exception = ex;
				return default(T);
			}
		}

		public class RunSyncResult<T> {

			public RunSyncResult(T result) { Result = result; }

			public RunSyncResult([NotNull] Exception exception) {
				Exception = exception ?? throw new ArgumentNullException(nameof(exception));
			}

			public T Result { get; }

			public Exception Exception { get; }
		}
	}

	public static class HttpClientExtention {
		public static async Task<HttpResponseMessage> SendAsync(this HttpClient client,
			string method,
			string api,
			HttpContent httpContent) {
			switch (method?.ToUpperInvariant()) {
				case "GET":    return await client.PutAsync(api, httpContent);
				case "PUT":    return await client.PutAsync(api, httpContent);
				case "POST":   return await client.PostAsync(api, httpContent);
				case "DELETE": return await client.DeleteAsync(api);
				default:       throw new ArgumentOutOfRangeException(nameof(method), method??"{NULL}", @"Unsupported send method.");
			}
		}
	}
}
