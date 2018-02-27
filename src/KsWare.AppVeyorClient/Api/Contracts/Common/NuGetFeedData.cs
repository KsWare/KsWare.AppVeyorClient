using System;

namespace KsWare.AppVeyorClient.Api.Contracts {

	public class NuGetFeedData {

		public string Id { get; set; }

		public string Name { get; set; }

		public int AccountId { get; set; }

		public int ProjectId { get; set; }

		public bool IsPrivateProject { get; set; }

		public string ApiKey { get; set; }

		public bool PublishingEnabled { get; set; }

		public string AccountTimeZoneId { get; set; }

		public DateTime Created { get; set; }

		public DateTime Updated { get; set; }

	}
}

