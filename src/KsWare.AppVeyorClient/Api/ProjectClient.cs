using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KsWare.AppVeyorClient.Api.Contracts;
using KsWare.AppVeyorClient.Shared;

namespace KsWare.AppVeyorClient.Api {

	public class ProjectClient {

		private readonly HttpClientEx _client;

		public ProjectClient(HttpClientEx client) { _client = client; }

		public string ProjectName { get; set; } = "Playground";

		// GET /api/projects
		public async Task<GetProjectsResponse> GetProjects() {
			const string n = nameof(ProjectClient) + "." + nameof(GetProjects);
			var c = FileStore.Instance.GetEntry<GetProjectsResponse>(n);

			if (!c.IsUsable) {
				var response = await _client.GetJsonAsync<GetProjectsResponse>("/api/projects");
				c.Data = response;
				c.CacheTime = TimeSpan.FromMinutes(5);
			}
			
			return c.Data;
		}

		private async Task<ProjectData> Project() {
			const string n = nameof(ProjectClient) + "." + nameof(Project);
			var entry = FileStore.Instance.GetEntry<ProjectData>(n);
			if (!entry.HasData) { 
				var projects = await GetProjects();
				if(string.IsNullOrEmpty(ProjectName)) entry.Data = projects.First();
				else  entry.Data                                 = projects.First(p=>string.Compare(p.Name, ProjectName, StringComparison.OrdinalIgnoreCase) ==0);
				ProjectName = entry.Data.Name;
				entry.IsPersistent = true;
				entry.CacheTime=TimeSpan.FromHours(24);
				FileStore.Instance.Flush(n);
			}
			return entry.Data;
		}

		#region ProjectSettingsYaml

		// Request  GET /api/projects/{accountName}/{projectSlug}/settings/yaml
		// Response (plain/text)
		public async Task<string> GetProjectSettingsYaml(string accountName,string projectSlug) {
			var yaml = await _client.GetTextAsync($"/api/projects/{accountName}/{projectSlug}/settings/yaml");
			return yaml;
		}

		public async Task<string> GetProjectSettingsYaml() {
			var p    = await Project();
			var yaml = await GetProjectSettingsYaml(p.AccountName, p.Slug);
			return yaml;
		}

		public async Task UpdateProjectSettingsYaml(string yaml) {
			var project = await Project();
			await UpdateProjectSettingsYaml(project.AccountName, project.Slug, yaml);
		}

		// Request: PUT /api/projects/{accountName}/{projectSlug}/settings/yaml
		// Request body (plain/text):
		private async Task UpdateProjectSettingsYaml(string accountName, string projectSlug, string yaml) {
			await _client.PutTextAsync($"/api/projects/{accountName}/{projectSlug}/settings/yaml", yaml);
		}

		#endregion


		// Request  GET /api/projects/{accountName}/{projectSlug}/settings
		// Response (plain/text)
		public async Task<GetProjectSettingsResponse> GetProjectSettings(string accountName, string projectSlug) {
			var projectSettings = await _client.GetJsonAsync<GetProjectSettingsResponse>($"/api/projects/{accountName}/{projectSlug}/settings");
			return projectSettings;
		}

		public async Task<GetProjectSettingsResponse> GetProjectSettings() {
			var p    = await Project();
			var projectSettings = await GetProjectSettings(p.AccountName, p.Slug);
			return projectSettings;
		}

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
		public async Task<Api.Contracts.EnvironmentData[]> GetEnvironments() {
			var result = await _client.GetJsonAsync<Api.Contracts.EnvironmentData[]>("/api/environments");
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

		public async Task<NameValueSecurePair[]> GetProjectEnvironmentVariables() {
			var project = await Project();
			return await GetProjectEnvironmentVariables(project.AccountName, project.Slug);
		}

		public async void UpdateProjectEnvironmentVariables(IEnumerable<NameValueSecurePair> variables) {
			var project = await Project();
			await UpdateProjectEnvironmentVariables(project.AccountName, project.Slug, variables);
		}

		// PUT /api/projects/{accountName}/{projectSlug}/settings/environment-variables
		private async Task UpdateProjectEnvironmentVariables(string accountName, string projectSlug, IEnumerable<NameValueSecurePair> variables) {
			await _client.PutJsonAsync($"/api/projects/{accountName}/{projectSlug}/settings/environment-variables", variables);
		}

		// PUT /api/projects/{accountName}/{projectSlug}/settings/build-number
		private async Task UpdateProjectBuildNumber(string accountName,string projectSlug,int nextBuildNumber) {
			var s = $@"{{""nextBuildNumber"": {nextBuildNumber}}}";
			await _client.PutJsonTextAsync($"/api/projects/{accountName}/{projectSlug}/settings/build-number", s);
		}
	}

}