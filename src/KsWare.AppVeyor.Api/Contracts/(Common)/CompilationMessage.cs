namespace KsWare.AppVeyor.Api.Contracts {

	public class CompilationMessage {
		public string Message { get; set; }
		public string Category { get; set; }
		public string Details { get; set; }
		public string FileName { get; set; }
		public int Line { get; set; }
		public int Column { get; set; }
		public string ProjectName { get; set; }
		public string ProjectFileName { get; set; }
	}
}
/*
{
    "message": "This is a test message",
    "category": "warning",
    "details": "Additional information for the message",
    "fileName": "program.cs",
    "line": "1",
    "column": "10",
    "projectName": "MyProject",
    "projectFileName": "MyProject.csproj"
}
*/
