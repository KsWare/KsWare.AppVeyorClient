using System;
using System.Threading.Tasks;
using NUnit.Framework;

namespace KsWare.AppVeyor.Api.Tests {

	//[TestFixture]// one time manual test
	public class ProjectClientTests {

		// string token = GetLocalToken();
		private string token = ""; //<== input YOUR token";
		string accountName = "KsWare"; //<== input YOUR account name";

		//[Test]//manual test, 2021-08-27 tested with v1 and v2 tokens
		public async Task AddDeleteProject() {
			var httpClientEx=new HttpClientEx(token) {
				BaseUri = new Uri("https://ci.appveyor.com/")
			};
			var projectClient = new ProjectClient(httpClientEx);

			var projectInfo = await projectClient.AddProject("gitHub", "KsWare/Playground", accountName);
			await projectClient.DeleteProject(accountName, projectInfo.Slug);
		}

		//[Test]//manual test, 2021-09-01 tested with v1 and v2 tokens
		public async Task ValidateYaml() {
			var httpClientEx = new HttpClientEx(token) {
				BaseUri = new Uri("https://ci.appveyor.com/")
			};
			var projectClient = new ProjectClient(httpClientEx);

			var result = await projectClient.ValidateYaml("version: 1.0.{build}", accountName);
			Assert.That(result.IsValid, Is.True);
		}
	}
}
