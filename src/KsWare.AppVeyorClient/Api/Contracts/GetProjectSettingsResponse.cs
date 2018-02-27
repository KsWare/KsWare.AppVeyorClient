using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace KsWare.AppVeyorClient.Api.Contracts {

	public partial class GetProjectSettingsResponse {

		public ProjectData Project { get; set; }

		public ProjectSettingsData Settings { get; set; }

		public Image1[] Images { get; set; }

		public object[] BuildClouds { get; set; } //TODO

		public string DefaultImageName { get; set; }

		public class Configuration1 {

			[JsonProperty(DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
			public string VersionFormat { get; set; }
			public bool DoNotIncrementBuildNumberOnPullRequests { get; set; }
			public ScriptData[] HotFixScripts { get; set; }
			public ScriptData[] InitScripts { get; set; }
			public string BranchesMode { get; set; }

			public Value1[] IncludeBranches { get; set; }
			public Value1[] ExcludeBranches { get; set; }
			public bool SkipTags { get; set; }
			public bool SkipNonTags { get; set; }

			public bool SkipBranchWithPullRequests { get; set; }

			public object[] SkipCommitsFiles { get; set; }
			public object[] OnlyCommitsFiles { get; set; }
			public ScriptData[] CloneScripts { get; set; }

			

			public ScriptData[] OnBuildSuccessScripts { get; set; }
			public ScriptData[] OnBuildErrorScripts { get; set; }
			public ScriptData[] OnBuildFinishScripts { get; set; }
			public bool PatchAssemblyInfo { get; set; }

			public string AssemblyInfoFile { get; set; }

			public string AssemblyVersionFormat { get; set; }

			public string AssemblyFileVersionFormat { get; set; }

			public string AssemblyInformationalVersionFormat { get; set; }

			public bool PatchDotnetCsproj { get; set; }
			public string DotnetCsprojFile { get; set; }

			public string DotnetCsprojVersionFormat { get; set; }

			public string DotnetCsprojPackageVersionFormat { get; set; }

			public string DotnetCsprojAssemblyVersionFormat { get; set; }

			public string DotnetCsprojFileVersionFormat { get; set; }

			public string DotnetCsprojInformationalVersionFormat { get; set; }

			[JsonProperty(DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
			public int BaxJobs { get; set; }

			
			public object[] BuildCloud { get; set; }

			public Value1[] OperatingSystem { get; set; }

			public Value1[] Services { get; set; }

			[JsonProperty(DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
			public string CloneFolder { get; set; }

			
			public bool ShallowClone { get; set; }
			public bool ForceHttpsClone { get; set; }

			[JsonProperty(DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
			public int CloneDepth { get; set; }

			
			public NameValueSecurePair[] EnvironmentVariables { get; set; }
			public VariablesMatrix1[] EnvironmentVariablesMatrix { get; set; }
			public ScriptData[] InstallScripts { get; set; }
			public HostEntry[] HostsEntries { get; set; }
			public Value1[] CacheEntries { get; set; }
			public bool ConfigureNuGetProjectSource { get; set; }
			public bool ConfigureNuGetAccountSource { get; set; }
			public bool DisableNuGetPublishOnPullRequests { get; set; }
			public string BuildMode { get; set; }
			public Value1[] Platform { get; set; }
			public Value1[] Configuration { get; set; }

			[JsonProperty(DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
			public string MsBuildProjectFileName { get; set; }

			
			public bool PackageWebApplicationProjects { get; set; }

			public bool PackageWebApplicationProjectsXCopy { get; set; }

			public bool PackageAzureCloudServiceProjects { get; set; }

			public bool PackageNuGetProjects { get; set; }

			public bool PackageNuGetSymbols { get; set; }

			public bool IncludeNuGetReferences { get; set; }

			public bool MsBuildInParallel { get; set; }

			[JsonProperty(DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
			public string MsBuildVerbosity { get; set; }

			public ScriptData[] BuildScripts { get; set; }
			public ScriptData[] BeforeBuildScripts { get; set; }
			public ScriptData[] BeforePackageScripts { get; set; }
			public ScriptData[] AfterBuildScripts { get; set; }
			public string TestMode { get; set; }

			public object[] TestAssemblies { get; set; }
			public object[] TestCategories { get; set; }
			public TestCategoriesMatrix1[] TestCategoriesMatrix { get; set; }
			public ScriptData[] TestScripts { get; set; }
			public ScriptData[] BeforeTestScripts { get; set; }
			public ScriptData[] AfterTestScripts { get; set; }
			public string DeployMode { get; set; }

			public Deployment1[] Deployments { get; set; }
			public ScriptData[] DeployScripts { get; set; }
			public ScriptData[] BeforeDeployScripts { get; set; }
			public ScriptData[] AfterDeployScripts { get; set; }
			public bool XamarinRegisterAndroidProduct { get; set; }

			public bool XamarinRegisterIosProduct { get; set; }

			
			public bool MatrixFastFinish { get; set; }
			public object[] MatrixAllowFailures { get; set; }
			public object[] MatrixExclude { get; set; }
			public Artifact1[] Artifacts { get; set; }
			public Notification1[] Notifications { get; set; }
		}

		public class Notification1 {
			public string Provider { get; set; }

			public object Settings { get; set; }
		}

		public class Artifact1 {
			public string Path { get; set; }
			public string Name { get; set; }
			public string Type { get; set; }

			
			
			
		}

		public class Deployment1 {
			public string Provider { get; set; }

			public NameValueSecurePair[] ProviderSettings { get; set; }

			public Value1[] OnBranch { get; set; }

			public NameValueSecurePair[] OnEnvironmentVariables { get; set; }

			
		}

		public class TestCategoriesMatrix1 {
			public Value1[] Categories { get; set; }
		}

		public class Image1 {
			public int BuildWorkerImageId { get; set; }

			
			public string Name { get; set; }

			[JsonProperty(DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
			public string BuildCloudName { get; set; }

			public string OsType { get; set; }

			
		}



		public class Value1 {
			public string Value { get; set; }
		}

		public class VariablesMatrix1 {

			public NameValueSecurePair[] Variables { get; set; }

			
		}

	}

	public class HostEntry {
		public string Host { get; set; }
		public string Ip { get; set; }

		
		
	}

}

