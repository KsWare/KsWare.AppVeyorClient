using Newtonsoft.Json;

namespace KsWare.AppVeyor.Api.Contracts {

	public class ScriptData {

		public ScriptData() { }

		public ScriptData(string language, string script) {
			Language = language;
			Script = script;
		}

		[JsonProperty(DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
		public string Language { get; set; }

		public string Script { get; set; }

	}

}
