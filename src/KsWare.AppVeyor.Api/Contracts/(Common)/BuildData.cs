using System;

namespace KsWare.AppVeyor.Api.Contracts {

	public class BuildData {
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
