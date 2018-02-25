namespace KsWare.AppVeyorClient.Api.Contracts.Common {

	public class BuildMessage {
		public BuildMessage() { }
		public BuildMessage(string message, string category, string details) {
			Message = message;
			Category = category;
			Details = details;
		}
		public string Message { get; set; }
		public string Category { get; set; } // {Information | Warning | Error}
		public string Details { get; set; }
	}
}
/*
{
    "message": "This is a test message",
    "category": "warning",
    "details": "Additional information for the message"
}
*/