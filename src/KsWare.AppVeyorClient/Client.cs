using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using KsWare.AppVeyorClient.Api.Contracts;
using KsWare.AppVeyorClient.Api.Contracts.Common;
using KsWare.AppVeyorClient.Shared;
using Environment = System.Environment;

namespace KsWare.AppVeyorClient {

	public class Client:BaseClient {

		public Client(SecureString token) : base(token) {
			BuildWorker=new BuildWorker(this);
			Project=new ProjectClient(this);
		}

		public Client(string token) : this(CreateSecureToken(token)) { }

		public ProjectClient Project { get; }

		public BuildWorker BuildWorker  { get;} 
	}

	public class ProjectClient:BaseClient {

		CacheEntry<GetProjectsResponse> _cacheGetProjects=new CacheEntry<GetProjectsResponse>();
		private GetProjectsResponse.Project _project;


		public ProjectClient(BaseClient client) : base(client) {}

		public string ProjectName { get; set; } = "Playground";

		// GET /api/projects
		public async Task<GetProjectsResponse> GetProjects() {
			if (!_cacheGetProjects.IsUsable) {
				var response = await GetJson<GetProjectsResponse>("/api/projects");
				_cacheGetProjects.Data = response;
			}
			
			return _cacheGetProjects.Data;
		}

		private async Task<GetProjectsResponse.Project> Project() {
			if (_project == null) {
				var projects = await GetProjects();
				if(string.IsNullOrEmpty(ProjectName)) _project = projects.First();
				else  _project = projects.First(p=>string.Compare(p.Name,ProjectName,StringComparison.OrdinalIgnoreCase)==0);
				ProjectName = _project.Name;
			}
			return _project;
		}

		// Request  GET /api/projects/{accountName}/{projectSlug}/settings/yaml
		// Response (plain/text)
		public async Task<string> GetProjectSettingsYaml(string accountName,string projectSlug) {
			var yaml = await GetText($"/api/projects/{accountName}/{projectSlug}/settings/yaml");
			return yaml;
		}

		public async Task<string> GetProjectSettingsYaml() {
			var p = await Project();
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
			await PutText($"/api/projects/{accountName}/{projectSlug}/settings/yaml", yaml);
		}
		

		// GET /api/environments/{deploymentEnvironmentId}/settings
		public async Task<EnvironmentSettings> GetEnvironmentSettings(int deploymentEnvironmentId) {
			var result = await GetJson<EnvironmentSettings>($"/api/environments/{deploymentEnvironmentId}/settings");
			return result;
		}

		// GET /api/environments
		public async Task<Api.Contracts.Environment[]> GetEnvironments() {
			var result = await GetJson<Api.Contracts.Environment[]>("/api/environments");
			return result;
		}

		/*
Request:

GET /api/projects/{accountName}/{projectSlug}/settings/environment-variables
*/
		// GET /api/projects/{accountName}/{projectSlug}/settings/environment-variables
		public async Task<NameValueSecurePair[]> GetProjectEnvironmentVariables(string accountName, string projectSlug) {
			var result = await GetJson<NameValueSecurePair[]>($"/api/projects/{accountName}/{projectSlug}/settings/environment-variables");
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
			await PutJson($"/api/projects/{accountName}/{projectSlug}/settings/environment-variables", variables);
		}

		// PUT /api/projects/{accountName}/{projectSlug}/settings/build-number
		private async Task UpdateProjectBuildNumber(string accountName,string projectSlug,int nextBuildNumber) {
			var s = $@"{{""nextBuildNumber"": {nextBuildNumber}}}";
			await PutJsonText($"/api/projects/{accountName}/{projectSlug}/settings/build-number", s);
		}
	}

}
