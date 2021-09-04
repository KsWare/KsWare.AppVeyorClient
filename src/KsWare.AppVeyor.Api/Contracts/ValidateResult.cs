namespace KsWare.AppVeyor.Api.Contracts {

	public class ValidateResult {
		// {isValid: true, line: 0, column: 0}
		// {isValid: false, line: 0, column: 0, errorMessage:"foo bar"}
		public bool IsValid { get; set; }
		public int Line { get; set; }
		public int Column { get; set; }
		public string ErrorMessage { get; set; }
	}

}
