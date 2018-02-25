using System;

namespace KsWare.AppVeyorClient.Api.Contracts {

	public class Environment {
		/*
			"deploymentEnvironmentId": 14,
			"name": "azure-blob-1",
			"provider": "AzureBlob",
			"created": "2014-01-23T18:13:52.2268502+00:00",
			"updated": "2014-06-02T18:13:32.5106126+00:00"
		*/

		public int DeploymentEnvironmentId { get; set; }
		public string Name { get; set; }
		public string Provider { get; set; }
		public DateTime Created { get; set; }
		public DateTime Updated { get; set; }
	}

}
