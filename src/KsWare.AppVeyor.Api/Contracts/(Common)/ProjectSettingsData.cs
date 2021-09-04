using System;
using Newtonsoft.Json;

namespace KsWare.AppVeyor.Api.Contracts {

	public class ProjectSettingsData {

		public int ProjectId { get; set; }

		public int AccountId { get; set; }

		public string AccountName { get; set; }

		public object[] Builds { get; set; } //TODO

		public string Name { get; set; }

		public string Slug { get; set; }

		public string VersionFormat { get; set; }

		public int NextBuildNumber { get; set; }

		public string RepositoryType { get; set; }

		public string RepositoryScm { get; set; }

		public string RepositoryName { get; set; }

		public string RepositoryBranch { get; set; }

		public string WebhookId { get; set; }

		public string WebhookUrl { get; set; }

		public string StatusBadgeId { get; set; }

		public bool IsPrivate { get; set; }

		[JsonProperty(DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
		public int BuildPriority { get; set; }

		public bool IgnoreAppveyorYml { get; set; }

		[JsonProperty(DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
		public string CommitStatusContextName { get; set; }

		public bool SkipBranchesWithoutAppveyorYml { get; set; }

		public bool EnableSecureVariablesInPullRequests { get; set; }

		public bool EnableSecureVariablesInPullRequestsFromSameRepo { get; set; }

		public bool EnableDeploymentInPullRequests { get; set; }

		public bool SaveBuildCacheInPullRequests { get; set; }

		public bool RollingBuilds { get; set; }

		public bool RollingBuildsDoNotCancelRunningBuilds { get; set; }

		public bool AlwaysBuildClosedPullRequests { get; set; }

		public GetProjectSettingsResponse.Configuration1 Configuration { get; set; }

		public string Tags { get; set; }

		public NuGetFeedData NuGetFeed { get; set; }

		public SecurityDescriptorData SecurityDescriptor { get; set; }

		public DateTime Created { get; set; }

		public DateTime Updated { get; set; }
	}

}