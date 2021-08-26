namespace KsWare.AppVeyorClient.Api.Contracts {

	public class AddProjectResponse {

		public string ProjectId { get; set; } // "projectId":43682,
		public string AccountId { get; set; } // "accountId":2,
		public string AccountName { get; set; } // "accountName":"appvyr",
		public object[] Builds { get; set; } // "builds":[],
		public string Name { get; set; } // "name":"demo-app",
		public string Slug { get; set; } // "slug":"demo-app-335",
		public string RepositoryType { get; set; } // "repositoryType":"gitHub",
		public string RepositoryScm { get; set; } // "repositoryScm":"git",
		public string RepositoryName { get; set; } // "repositoryName":"FeodorFitsner/demo-app",
		public string IsPrivate { get; set; } // "isPrivate":false,
		public string SkipBranchesWithoutAppveyorYml { get; set; } // "skipBranchesWithoutAppveyorYml":false,
		public string Created { get; set; } // "created":"2014-08-16T00:52:15.6604826+00:00"

	}

}
