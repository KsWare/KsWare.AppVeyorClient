using Newtonsoft.Json;

namespace KsWare.AppVeyor.Api.Contracts {

	public class AccessRightData {

		public string Name { get; set; }

		[JsonProperty(DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
		public bool Allowed { get; set; }
	}
}
