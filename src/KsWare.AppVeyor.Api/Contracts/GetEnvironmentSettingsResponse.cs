﻿using System;

namespace KsWare.AppVeyor.Api.Contracts {

	public class GetEnvironmentSettingsResponse {

		public Environment2 Environment { get; set; }

		public class Environment2 {
			public int DeploymentEnvironmentId { get; set; }
			public int AccountId { get; set; }
			public string Name { get; set; }
			public string Provider { get; set; }
			public string EnvironmentAccessKey { get; set; }
			public Settings3 Settings { get; set; }
			public int ProjectsMode { get; set; }
			public object[] SelectedProjects { get; set; } // TODO
			public Project[] Projects { get; set; }
			public SecurityDescriptorData SecurityDescriptorData { get; set; }
			public string Tags { get; set; }

			public DateTime Created { get; set; }
			//		public DateTime Updated { get; set; }
		}

		public class Project {
			public int ProjectId { get; set; }
			public string Name { get; set; }
			public bool IsSelected { get; set; }
		}

		public class Settings3 {
			public NameValueSecurePair[] ProviderSettings { get; set; }
			public object[] EnvironmentVariables { get; set; } //TODO
			public object[] Notifications { get; set; }        // TODO

		}
	}




}
