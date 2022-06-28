using System;
using Newtonsoft.Json;

namespace KsWare.AppVeyor.Api.Contracts {

	public class NuGetFeedData {

		public string Id { get; set; }

		public string Name { get; set; }

		public int AccountId { get; set; }

		public int ProjectId { get; set; }

		public bool IsPrivateProject { get; set; }

		[JsonProperty(DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
		public string ApiKey { get; set; }

		public bool PublishingEnabled { get; set; }

		public string AccountTimeZoneId { get; set; }

		public DateTime Created { get; set; }

		[JsonProperty(DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
		public DateTime Updated { get; set; }

	}
}

