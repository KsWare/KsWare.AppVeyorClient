using System;

namespace KsWare.AppVeyorClient.Api.Contracts {

	public class ProjectData {

		public int ProjectId { get; set; }

		public int AccountId { get; set; }

		public string AccountName { get; set; }

		public object[] Builds { get; set; } //TODO

		public string Name { get; set; }

		public string Slug { get; set; }

		public string RepositoryType { get; set; }

		public string RepositoryScm { get; set; }

		public string RepositoryName { get; set; }

		public bool IsPrivate { get; set; }

		public bool SkipBranchesWithoutAppveyorYml { get; set; }

		public bool EnableSecureVariablesInPullRequests { get; set; }

		public bool EnableSecureVariablesInPullRequestsFromSameRepo { get; set; }

		public bool EnableDeploymentInPullRequests { get; set; }

		public bool SaveBuildCacheInPullRequests { get; set; }

		public bool RollingBuilds { get; set; }

		public bool RollingBuildsDoNotCancelRunningBuilds { get; set; }

		public bool AlwaysBuildClosedPullRequests { get; set; }

		public string Tags { get; set; }

		public SecurityDescriptorData SecurityDescriptor { get; set; }

		public DateTime Created { get; set; }

		public DateTime Updated { get; set; }

	}

}
