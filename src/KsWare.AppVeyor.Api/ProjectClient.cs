using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using JetBrains.Annotations;
using KsWare.AppVeyor.Api.Contracts;
using KsWare.AppVeyor.Api.Shared;

namespace KsWare.AppVeyor.Api {

	public class ProjectClient {

		private readonly HttpClientEx _client;

		public ProjectClient([NotNull] HttpClientEx client) {
			_client = client ?? throw new ArgumentNullException(nameof(client), "Argument 'client' must not be null!");
			_client.TokenChanged += ClientOnTokenChanged;
			ClientOnTokenChanged(null, null);
		}

		private void ClientOnTokenChanged(object sender, EventArgs e) {
			ApiVersion = "v1";
			if ($"{_client.InsecureToken}".StartsWith("v2.")) ApiVersion = "v2";
		}

		public string ApiVersion { get; private set; }

		// public string ProjectName { get; set; }

		/// <summary>
		/// Add Project
		/// </summary>
		/// <param name="repositoryProvider">Repository provider
		/// <list type="bullet">
		/// <item><term>gitHub</term></item>
		/// <item><term>bitBucket</term></item>
		/// <item><term>vso (Visual Studio Online)</term></item>
		/// <item><term>gitLab</term></item>
		/// <item><term>kiln</term></item>
		/// <item><term>stash</term></item>
		/// <item><term>git</term></item>
		/// <item><term>mercurial</term></item>
		/// <item><term>subversion</term></item>
		/// </list>
		/// </param>
		/// <param name="repositoryName">Repository Name<example>FeodorFitsner/demo-app</example></param>
		/// <param name="accountName">Account name. Mandatory for API v2.</param>
		/// <returns></returns>
		public Task<AddProjectResponse> AddProject(string repositoryProvider, string repositoryName, string accountName = null) {
			// POST /api/projects
			// {"repositoryProvider":"gitHub","repositoryName":"FeodorFitsner/demo-app"}
			string api;
			switch (ApiVersion) {
				case "v2" :
					if (string.IsNullOrWhiteSpace(accountName)) throw new ArgumentNullException(nameof(accountName));
					api = $"/api/account/{accountName}/projects"; 
					break;
				default: api = "/api/projects"; break;
			}
			var s = $@"{{""repositoryProvider"":""{repositoryProvider}"",""repositoryName"":""{repositoryName}""}}";
			return _client.PostJsonAsync<AddProjectResponse>(api, s);
		}

		public Task DeleteProject(string accountName, string projectSlug) {
			// DELETE /api/projects/{accountName}/{projectSlug}
			// Response: 204
			var api = $"/api/projects/{accountName}/{projectSlug}";
			return _client.Delete(api, expectedStatusCode: (HttpStatusCode)204);
		}

		/// <summary>
		/// Get the projects.
		/// </summary>
		/// <param name="accountName">The account name. Mandatory for API v2</param>
		/// <returns></returns>
		public async Task<GetProjectsResponse> GetProjects(string accountName = null) {
			// GET /api/projects
			// GET /api/account/<account-name>/projects		// API v2

			string api;
			switch (ApiVersion) {
				// case "v2" : api = $"/api/account/{accountName}/projects"; break;
				default: api = "/api/projects"; break;
			}

			const string n = nameof(ProjectClient) + "." + nameof(GetProjects);
			var c = FileStore.Instance.GetEntry<GetProjectsResponse>(n);

			if (!c.IsUsable) {
				var response = await _client.GetJsonAsync<GetProjectsResponse>(api);
				c.Data = response;
				c.CacheTime = TimeSpan.FromMinutes(5);
			}

			return c.Data;
		}

		// private async Task<ProjectData> Project() {
		// 	const string n = nameof(ProjectClient) + "." + nameof(Project);
		// 	var entry = FileStore.Instance.GetEntry<ProjectData>(n);
		// 	if (!entry.HasData) {
		// 		var projects = await GetProjects();
		// 		entry.Data = string.IsNullOrEmpty(ProjectName)
		// 			? projects.First()
		// 			: projects.First(p => string.Compare(p.Name, ProjectName, StringComparison.OrdinalIgnoreCase) == 0);
		// 		ProjectName = entry.Data.Name;
		// 		entry.IsPersistent = true;
		// 		entry.CacheTime = TimeSpan.FromHours(24);
		// 		FileStore.Instance.Flush(n);
		// 	}
		// 	return entry.Data;
		// }

		// private async Task<ProjectData> Project(string projectName) {
		// 	string n = $"{nameof(ProjectClient)}.{nameof(Project)}.{projectName}";
		// 	var entry = FileStore.Instance.GetEntry<ProjectData>(n);
		// 	if (!entry.HasData) {
		// 		var projects = await GetProjects();
		// 		entry.Data = string.IsNullOrEmpty(projectName)
		// 			? projects.First()
		// 			: projects.First(p => string.Compare(p.Name, projectName, StringComparison.OrdinalIgnoreCase) == 0);
		// 		projectName = entry.Data.Name;
		// 		entry.IsPersistent = true;
		// 		entry.CacheTime = TimeSpan.FromHours(24);
		// 		FileStore.Instance.Flush(n);
		// 	}
		// 	return entry.Data;
		// }

		#region ProjectSettingsYaml

		public async Task<string> GetProjectSettingsYaml(string accountName, string projectSlug) {
			// Request  GET /api/projects/{accountName}/{projectSlug}/settings/yaml
			// Response (plain/text)
			var api = $"/api/projects/{accountName}/{projectSlug}/settings/yaml";
			var yaml = await _client.GetTextAsync(api);
			return yaml;
		}

		// public async Task<string> GetProjectSettingsYaml() {
		// 	var p    = await Project();
		// 	var yaml = await GetProjectSettingsYaml(p.AccountName, p.Slug);
		// 	return yaml;
		// }

		// public async Task UpdateProjectSettingsYaml(string yaml) {
		// 	var project = await Project();
		// 	await UpdateProjectSettingsYamlAsync(project.AccountName, project.Slug, yaml);
		// }

		public async Task UpdateProjectSettingsYamlAsync(string accountName, string projectSlug, string yaml) {
			// Request: PUT /api/projects/{accountName}/{projectSlug}/settings/yaml
			// Request body (plain/text):

			var api=$"/api/projects/{accountName}/{projectSlug}/settings/yaml";
			await _client.PutTextAsync(api, yaml);
		}

		#endregion


		// Request  GET /api/projects/{accountName}/{projectSlug}/settings
		// Response (plain/text)
		public async Task<GetProjectSettingsResponse> GetProjectSettings(string accountName, string projectSlug) {
			var projectSettings = await _client.GetJsonAsync<GetProjectSettingsResponse>($"/api/projects/{accountName}/{projectSlug}/settings");
			return projectSettings;
		}

		// public async Task<GetProjectSettingsResponse> GetProjectSettings() {
		// 	var p    = await Project();
		// 	var projectSettings = await GetProjectSettings(p.AccountName, p.Slug);
		// 	return projectSettings;
		// }

		public async Task UpdateProjectSettings(ProjectSettingsData projectSettings) {
			await _client.PutJsonAsync($"/api/projects", projectSettings);
		}

		// Request: PUT /api/projects
		private async Task UpdateProjectSettings(string accountName, string projectSlug, GetProjectSettingsResponse projectSettings) {
			await _client.PutJsonAsync($"/api/projects", projectSettings);
		}


		// GET /api/environments/{deploymentEnvironmentId}/settings
		public async Task<GetEnvironmentSettingsResponse> GetEnvironmentSettings(int deploymentEnvironmentId) {
			var result = await _client.GetJsonAsync<GetEnvironmentSettingsResponse>($"/api/environments/{deploymentEnvironmentId}/settings");
			return result;
		}

		// GET /api/environments
		public async Task<EnvironmentData[]> GetEnvironments() {
			var result = await _client.GetJsonAsync<EnvironmentData[]>("/api/environments");
			return result;
		}

		/*
Request:

GET /api/projects/{accountName}/{projectSlug}/settings/environment-variables
*/
		// GET /api/projects/{accountName}/{projectSlug}/settings/environment-variables
		public async Task<NameValueSecurePair[]> GetProjectEnvironmentVariables(string accountName, string projectSlug) {
			var result = await _client.GetJsonAsync<NameValueSecurePair[]>($"/api/projects/{accountName}/{projectSlug}/settings/environment-variables");
			return result;
		}

		// public async Task<NameValueSecurePair[]> GetProjectEnvironmentVariables() {
		// 	var project = await Project();
		// 	return await GetProjectEnvironmentVariables(project.AccountName, project.Slug);
		// }

		// public async void UpdateProjectEnvironmentVariables(IEnumerable<NameValueSecurePair> variables) {
		// 	var project = await Project();
		// 	await UpdateProjectEnvironmentVariables(project.AccountName, project.Slug, variables);
		// }

		// PUT /api/projects/{accountName}/{projectSlug}/settings/environment-variables
		public async Task UpdateProjectEnvironmentVariables(string accountName, string projectSlug, IEnumerable<NameValueSecurePair> variables) {
			await _client.PutJsonAsync($"/api/projects/{accountName}/{projectSlug}/settings/environment-variables", variables);
		}

		// PUT /api/projects/{accountName}/{projectSlug}/settings/build-number
		public async Task UpdateProjectBuildNumber(string accountName,string projectSlug,int nextBuildNumber) {
			var s = $@"{{""nextBuildNumber"": {nextBuildNumber}}}";
			await _client.PutJsonTextAsync($"/api/projects/{accountName}/{projectSlug}/settings/build-number", s);
		}

		public Task<ValidateResult> ValidateYaml(string yaml, string accountName) {
			// POST https://ci.appveyor.com/api/account/KsWare/projects/validate-yaml

			string api;
			switch (ApiVersion) {
				case "v2":
					if (string.IsNullOrWhiteSpace(accountName)) throw new ArgumentNullException(nameof(accountName), "Value must not null or empty.");
					api = $"api/account/{accountName}/projects/validate-yaml"; 
					break;
				default: 
					api = $"api/projects/validate-yaml";
					break;
			}

			return _client.PostJsonAsync<ValidateResult>(api, yaml);
		}
	}

}
