using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KsWare.AppVeyorClient.Api;
using KsWare.AppVeyorClient.Shared;
using NUnit.Framework;

namespace KsWare.AppVeyorClient.Tests.Api {

	[TestFixture]
	public class ProjectClientTests {

		//[Test]//manual test, 2021-08-27 tested with v1 and v2 tokens
		public async Task AddDeleteProject() {
			// var token = GetLocalToken();
			var token = "input your token"; 
			var accountName = "KsWare";
			var httpClientEx=new HttpClientEx(token) {
				BaseUri = new Uri("https://ci.appveyor.com/")
			};

			var projectClient = new ProjectClient(httpClientEx);

			var projectInfo = await projectClient.AddProject("gitHub", "KsWare/Playground", accountName);
			await projectClient.DeleteProject(accountName, projectInfo.Slug);
		}
	}
}
