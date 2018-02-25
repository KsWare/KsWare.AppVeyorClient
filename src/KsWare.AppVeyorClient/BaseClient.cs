using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using KsWare.AppVeyorClient.Helpers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

namespace KsWare.AppVeyorClient {

	public class BaseClient {

		private SecureString secureToken;

//		string UnsecureToken = "2kluce34huli5y6dan3k";
		private string server = "ci.appveyor.com";
		private BaseClient _client;

		private static readonly JsonSerializerSettings JsonSerializerSettings = new JsonSerializerSettings {
			ContractResolver = new CamelCasePropertyNamesContractResolver()
		};

		public BaseClient(SecureString token) {
			secureToken = token;
			_client = this;
		}

		public BaseClient(string unsecureToken) : this(CreateSecureToken(unsecureToken)) {}

		public bool HasToken => secureToken != null && secureToken.Length > 0;

		public event EventHandler TokenChanged;

		protected static SecureString CreateSecureToken(string unsecureToken) {
			var s = new SecureString();
			if (unsecureToken != null) foreach (var c in unsecureToken) s.AppendChar(c);
			return s;
		}

		protected BaseClient(BaseClient client) {
			_client = client;
		}

		public void SetToken(SecureString secureToken) {
			_client.secureToken = secureToken;
			TokenChanged?.BeginInvoke(this, EventArgs.Empty, null, null);
		}

		private string UnsecureToken =>
			System.Runtime.InteropServices.Marshal.PtrToStringAuto(
				System.Runtime.InteropServices.Marshal.SecureStringToBSTR(_client.secureToken));


		string GetUrl(string api) {
			if (!api.StartsWith("/")) api = "/" + api;
			return $"https://{_client.server}{api}";
		}

		public async Task<JToken[]> GetJTokens(string api) {
			Debug.WriteLine($"GET {api}");
			using (var client = new HttpClient()) {
				client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
				client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", UnsecureToken);

				// get the list of roles
				using (var response = await client.GetAsync(GetUrl(api))) {
					Log(response.StatusCode);
					response.EnsureSuccessStatusCode();

					var content = await response.Content.ReadAsAsync<JToken[]>();
					return content;
				}
			}
		}

		public async Task<T> GetJson<T>(string api) {
			Debug.WriteLine($"GET {api}");
			using (var client = new HttpClient()) {
				client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
				client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", UnsecureToken);

				using (var response = await client.GetAsync(GetUrl(api))) {
					Log(response.StatusCode);
					response.EnsureSuccessStatusCode();

//					var content = await response.Content.ReadAsAsync<T>();
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
		}

		public string GetJsonText(string api) {
			Debug.WriteLine($"GET {api}");
			using (var client = new HttpClient()) {
				client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
				client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", UnsecureToken);

				// get the list of roles
				using (var response = client.GetAsync(GetUrl(api)).WaitForResult()) {
					Log(response.StatusCode);
					response.EnsureSuccessStatusCode();

					var content = response.Content.ReadAsStringAsync().WaitForResult();
					return content;
				}
			}
		}

		public string GetJsonText(string api, out Exception ex) {
			Debug.WriteLine($"GET {api}");
			using (var client = new HttpClient()) {
				client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
				client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", UnsecureToken);

				// get the list of roles
				using (var response = client.GetAsync(GetUrl(api)).WaitForResult(out ex)) {
					if (ex != null) return default(string);
					Log(response.StatusCode);
					try {response.EnsureSuccessStatusCode();}
					catch (Exception ex1) {ex=ex1; return default(string);}

					var content = response.Content.ReadAsStringAsync().WaitForResult(out ex);
					if (ex != null) return default(string);
					return content;
				}
			}
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

		public async Task<string> GetText(string api) {
			Debug.WriteLine($"GET {api}");
			using (var client = new HttpClient()) {
				client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("plain/text"));
				client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", UnsecureToken);

				using (var response = await client.GetAsync(GetUrl(api))) {
					Log(response.StatusCode);
					response.EnsureSuccessStatusCode();

					var content = await response.Content.ReadAsStringAsync();
					return content;
				}
			}
		}

		public async Task PutText(string api, string text) {
			Debug.WriteLine($"PUT {api} => text: ...");
			using (var client = new HttpClient()) {
				client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("plain/text"));
				client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", UnsecureToken);
				HttpContent content=new StringContent(text,Encoding.UTF8);

				using (var response = await client.PutAsync(GetUrl(api), content)) {
					Log(response.StatusCode);
					response.EnsureSuccessStatusCode();
					return;
				}
			}
		}

		public async Task PutJson(string api, object json) {
			Debug.WriteLine($"PUT {api} => json: ...");
			var jsonString = ToJsonString(json);
			await Send("PUT", api, jsonString, "application/json");
		}

		public async Task PutJsonText(string api, string jsonString) {
			Debug.WriteLine($"PUT {api} => json: ...");
			await Send("PUT", api, jsonString, "application/json");
		}

		public async Task PostJson(string api, object json) {
			Debug.WriteLine($"POST {api} => json: ...");
			var jsonString = JsonConvert.SerializeObject(json);
			await Send("POST", api, jsonString, "application/json");
		}

		private async Task Send(string method, string api, string content, string contentType) {
			var httpContent = new StringContent(content, Encoding.UTF8, contentType);
			using (var client = new HttpClient()) {
				client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(contentType));
				client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", UnsecureToken);
				using (var response = await SendAsync(client,method,api, httpContent)) {
					Log(response.StatusCode);
					if (!response.IsSuccessStatusCode) {
						//  400 (Invalid input parameters. See response body for detailed error message.)
						// {"message":"The request is invalid.","modelState":{"variables.Headers":["An error has occurred."]}}
						var responseContent = await response.Content.ReadAsStringAsync();
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
				}
			}
		}

		private async Task<HttpResponseMessage> SendAsync(HttpClient client,string method,string api,HttpContent httpContent) {
			switch (method) {
				case "GET": return await client.PutAsync(GetUrl(api), httpContent);
				case "PUT": return await client.PutAsync(GetUrl(api), httpContent);
				case "POST": return await client.PostAsync(GetUrl(api), httpContent);
				case "DELETE": return await client.DeleteAsync(GetUrl(api));
				default: throw new ArgumentOutOfRangeException(nameof(method),method, @"Unsupported send method.");
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
	}
}
