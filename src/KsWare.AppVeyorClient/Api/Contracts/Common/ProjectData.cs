using System;
using Newtonsoft.Json;

namespace KsWare.AppVeyorClient.Api.Contracts {

	public class ProjectData {

		public int ProjectId { get; set; }

		public int AccountId { get; set; }

		public string AccountName { get; set; }

		public Build[] Builds { get; set; }

		public string Name { get; set; }

		public string Slug { get; set; }

		public string RepositoryType { get; set; } // gitHub, bitBucket, vso (Visual Studio Online), gitLab, kiln, stash, git, mercurial, subversion

		public string RepositoryScm { get; set; }

		public string RepositoryName { get; set; }

		public string RepositoryBranch { get; set; }

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
		public NuGetFeedData NuGetFeed { get; set; }

		public SecurityDescriptorData SecurityDescriptor { get; set; }

		public DateTime Created { get; set; }

		public DateTime Updated { get; set; }

	}

	public class Build {
		public int BuildId { get; set; } // "buildId": 26957076,
		public int ProjectId { get; set; } // "projectId": 605066,
		public object[] Jobs { get; set; } // "jobs": [],
		public int BuildNumber  { get; set; } // "buildNumber": 1,
		public string Version { get; set; } // "version": "0.1.1",
		public string Message { get; set; } // "message": "deploy provider NuGet",
		public string Branch { get; set; } // "branch": "PublishOnNuGet",
		public bool IsTag { get; set; } // "isTag": false,
		public string CommitId { get; set; } // "commitId": "daf6ebe9ca0d7ef3b9d0d15d7a0d3e33428e10fa",
		public string AuthorName { get; set; } // "authorName": "Kay-Uwe Schreiner",
		public string AuthorUsername { get; set; } // "authorUsername": "SchreinerK",
		public string CommitterName { get; set; } // "committerName": "Kay-Uwe Schreiner",
		public string CommitterUsername { get; set; } // "committerUsername": "SchreinerK",
		public DateTime Committed { get; set; } // "committed": "2019-08-26T10:17:43+00:00",
		public object[] Messages { get; set; } // "messages": [],
		public string Status { get; set; } // "status": "success",
		public DateTime Started { get; set; } // "started": "2019-08-26T10:20:15.1459989+00:00",
		public DateTime Finished { get; set; } // "finished": "2019-08-26T10:20:39.6179467+00:00",
		public DateTime Created { get; set; } // "created": "2019-08-26T10:17:56.1739485+00:00",
		public DateTime Updated { get; set; } // "updated": "2019-08-26T10:20:39.6179467+00:00"
	}

}
