using System.Collections.Generic;
using System.Collections.ObjectModel;
using KsWare.AppVeyorClient.Api.Contracts;

namespace KsWare.AppVeyorClient.UI.PanelConfiguration {

	public class EnvironmentVariableInfo {

		public string Name { get; }
		public string Description { get; }

		private EnvironmentVariableInfo(string name, string description) {
			Name = name;
			Description = description;
		}

		public static void Fill(IList<EnvironmentVariableInfo> col) {
			col.Add(new EnvironmentVariableInfo("APPVEYOR", "True (true on Ubuntu image) if build runs in AppVeyor environment"));
			col.Add(new EnvironmentVariableInfo("CI", "True (true on Ubuntu image) if build runs in AppVeyor environment"));
			col.Add(new EnvironmentVariableInfo("APPVEYOR_URL", "AppVeyor central server(s) URL, https://ci.appveyor.com for Hosted service, specific server URL for on-premise"));
			col.Add(new EnvironmentVariableInfo("APPVEYOR_API_URL", "AppVeyor Build Agent API URL"));
			col.Add(new EnvironmentVariableInfo("APPVEYOR_ACCOUNT_NAME", "account name"));
			col.Add(new EnvironmentVariableInfo("APPVEYOR_PROJECT_ID", "AppVeyor unique project ID"));
			col.Add(new EnvironmentVariableInfo("APPVEYOR_PROJECT_NAME", "project name"));
			col.Add(new EnvironmentVariableInfo("APPVEYOR_PROJECT_SLUG", "project slug (as seen in project details URL)"));
			col.Add(new EnvironmentVariableInfo("APPVEYOR_BUILD_FOLDER", "path to clone directory"));
			col.Add(new EnvironmentVariableInfo("APPVEYOR_BUILD_ID", "AppVeyor unique build ID"));
			col.Add(new EnvironmentVariableInfo("APPVEYOR_BUILD_NUMBER", "build number"));
			col.Add(new EnvironmentVariableInfo("APPVEYOR_BUILD_VERSION", "build version"));
			col.Add(new EnvironmentVariableInfo("APPVEYOR_BUILD_WORKER_IMAGE", "current build worker image the build is running on, e.g. Visual Studio 2015 (also can be used as “tweak” environment variable to set build worker image)"));
			col.Add(new EnvironmentVariableInfo("APPVEYOR_PULL_REQUEST_NUMBER", "Pull (Merge) Request number"));
			col.Add(new EnvironmentVariableInfo("APPVEYOR_PULL_REQUEST_TITLE", "Pull (Merge) Request title"));
			col.Add(new EnvironmentVariableInfo("APPVEYOR_PULL_REQUEST_HEAD_REPO_NAME", "Pull (Merge) Request source repo"));
			col.Add(new EnvironmentVariableInfo("APPVEYOR_PULL_REQUEST_HEAD_REPO_BRANCH", "Pull (Merge) Request source branch"));
			col.Add(new EnvironmentVariableInfo("APPVEYOR_PULL_REQUEST_HEAD_COMMIT", "Pull (Merge) Request source commit ID (SHA)"));
			col.Add(new EnvironmentVariableInfo("APPVEYOR_JOB_ID", "AppVeyor unique job ID"));
			col.Add(new EnvironmentVariableInfo("APPVEYOR_JOB_NAME", "job name"));
			col.Add(new EnvironmentVariableInfo("APPVEYOR_JOB_NUMBER", "job number, i.g. 1, 2, etc."));
			col.Add(new EnvironmentVariableInfo("APPVEYOR_REPO_PROVIDER", "gitHub, bitBucket, kiln, vso, gitLab, gitHubEnterprise, gitLabEnterprise, stash, gitea, git, mercurial or subversion"));
			col.Add(new EnvironmentVariableInfo("APPVEYOR_REPO_SCM", "git or mercurial"));
			col.Add(new EnvironmentVariableInfo("APPVEYOR_REPO_NAME", "repository name in format owner-name/repo-name"));
			col.Add(new EnvironmentVariableInfo("APPVEYOR_REPO_BRANCH", "build branch. For Pull Request commits it is base branch PR is merging into"));
			col.Add(new EnvironmentVariableInfo("APPVEYOR_REPO_TAG", "true if build has started by pushed tag; otherwise false"));
			col.Add(new EnvironmentVariableInfo("APPVEYOR_REPO_TAG_NAME", "contains tag name for builds started by tag; otherwise this variable is undefined"));
			col.Add(new EnvironmentVariableInfo("APPVEYOR_REPO_COMMIT", "commit ID (SHA)"));
			col.Add(new EnvironmentVariableInfo("APPVEYOR_REPO_COMMIT_AUTHOR", "commit author’s name"));
			col.Add(new EnvironmentVariableInfo("APPVEYOR_REPO_COMMIT_AUTHOR_EMAIL", "commit author’s email address"));
			col.Add(new EnvironmentVariableInfo("APPVEYOR_REPO_COMMIT_TIMESTAMP", "commit date/time in ISO 8601 format"));
			col.Add(new EnvironmentVariableInfo("APPVEYOR_REPO_COMMIT_MESSAGE", "commit message"));
			col.Add(new EnvironmentVariableInfo("APPVEYOR_REPO_COMMIT_MESSAGE_EXTENDED", "the rest of commit message after line break (if exists)"));
			col.Add(new EnvironmentVariableInfo("APPVEYOR_SCHEDULED_BUILD", "True if the build runs by scheduler"));
			col.Add(new EnvironmentVariableInfo("APPVEYOR_FORCED_BUILD (True or undefined)", "builds started by “New build” button or from the same API"));
			col.Add(new EnvironmentVariableInfo("APPVEYOR_RE_BUILD (True or undefined)", "build started by “Re-build commit/PR” button of from the same API"));
			col.Add(new EnvironmentVariableInfo("APPVEYOR_RE_RUN_INCOMPLETE (True or undefined)", "build job started by “Re-run incomplete” button of from the same API"));
			col.Add(new EnvironmentVariableInfo("PLATFORM", "platform name set on Build tab of project settings (or through platform parameter in appveyor.yml)"));
			col.Add(new EnvironmentVariableInfo("CONFIGURATION", "configuration name set on Build tab of project settings (or through configuration parameter in appveyor.yml)"));
			col.Add(new EnvironmentVariableInfo("{version}", "Shortcut"));
			col.Add(new EnvironmentVariableInfo("{build}", "Shortcut"));
			col.Add(new EnvironmentVariableInfo("{branch}", "Shortcut"));

		}

		public static T Create<T>() where T:IList<EnvironmentVariableInfo>,new() {
			var col = new T();
			Fill(col);
			return col;
		}
	}

}
