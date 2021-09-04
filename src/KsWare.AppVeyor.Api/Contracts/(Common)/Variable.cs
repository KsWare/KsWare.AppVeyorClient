namespace KsWare.AppVeyor.Api.Contracts {

	public class Variable {

		public Variable() { }
		public Variable(string name, string value) {
			Name = name;
			Value = value;
		}
		public string Name { get; set; }
		public string Value { get; set; }
	}
}
/*
{
    "name": "variable_name",
    "value": "hello, world!"
}
*/