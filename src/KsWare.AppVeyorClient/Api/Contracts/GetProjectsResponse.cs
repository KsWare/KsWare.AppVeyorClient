using System;
using System.Collections.Generic;

namespace KsWare.AppVeyorClient.Api.Contracts {

	public class GetProjectsResponse: List<GetProjectsResponse.Project> {

		public class Project {
			public int ProjectId { get; set; }
			public int AccountId { get; set; }
			public string AccountName { get; set; }
			public Build[] Builds { get; set; }
			public string Name { get; set; }
			public string Slug { get; set; }
			public string RepositoryType { get; set; }
			public string RepositoryScm { get; set; }
			public string RepositoryName { get; set; }
			public string RepositoryBranch { get; set; }
			public bool IsPrivate { get; set; }
			public bool SkipBranchesWithoutAppveyorYml { get; set; }

			public NuGetFeed NuGetFeed { get; set; }

			public DateTime Created { get; set; }
		}

		public class NuGetFeed {
			public string Id { get; set; }
			public string Name { get; set; }
			public bool PublishingEnabled { get; set; }
			public DateTime Created { get; set; }
		}

		public class Build {
			public int BuildId { get; set; }
			public object[] Jobs { get; set; } //TODO
			public int BuildNumber { get; set; }
			public string Version { get; set; }
			public string Message { get; set; }
			public string Branch { get; set; }
			public string CommitId { get; set; }
			public string AuthorName { get; set; }
			public string AuthorUsername { get; set; }
			public string CommitterName { get; set; }
			public string CommitterUsername { get; set; }
			public DateTime Committed { get; set; }
			public object[] Messages { get; set; } //TODO
			public string Status { get; set; }
			public DateTime Started { get; set; }
			public DateTime Finished { get; set; }
			public DateTime Created { get; set; }
			public DateTime Updated { get; set; }
		}
	}

	


}
/*
[
   {
      "projectId":19096,
      "accountId":2,
      "accountName":"FeodorFitsner",
      "builds":[
         {
            "buildId":23864,
            "jobs":[

            ],
            "buildNumber":3,
            "version":"1.0.3",
            "message":"replaced with command [skip ci]",
            "branch":"master",
            "commitId":"c2892a70d60c96c1b65a7c665ab806b7731fea8a",
            "authorName":"Feodor Fitsner",
            "authorUsername":"FeodorFitsner",
            "committerName":"Feodor Fitsner",
            "committerUsername":"FeodorFitsner",
            "committed":"2014-08-15T22:05:54+00:00",
            "messages":[

            ],
            "status":"success",
            "started":"2014-08-15T22:36:38.1757886+00:00",
            "finished":"2014-08-15T22:37:00.6171479+00:00",
            "created":"2014-08-15T22:33:15.9833328+00:00",
            "updated":"2014-08-15T22:37:00.6171479+00:00"
         }
      ],
      "name":"appveyor-artifact-test",
      "slug":"appveyor-artifact-test",
      "repositoryType":"gitHub",
      "repositoryScm":"git",
      "repositoryName":"FeodorFitsner/appveyor-artifact-test",
      "repositoryBranch":"master",
      "isPrivate":false,
      "skipBranchesWithoutAppveyorYml":false,
      "nuGetFeed":{
         "id":"appveyor-artifact-test-j8kk0o",
         "name":"Project appveyor-artifact-test",
         "publishingEnabled":false,
         "created":"2014-08-15T22:04:21.3111546+00:00"
      },
      "created":"2014-08-15T22:04:19.2868375+00:00"
   }
]

*/