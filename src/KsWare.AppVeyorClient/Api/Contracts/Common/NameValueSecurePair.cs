namespace KsWare.AppVeyorClient.Api.Contracts {

	public class NameValueSecurePair {
		/*
			"name": "storage_account_name",
			"value": {
				"isEncrypted": false,
				"value": "myaccount"
			}
		 */

		public NameValueSecurePair() { }

		public NameValueSecurePair(string name, string value, bool isEncrypted = false) {
			Name = name;
			Value=new Value2(value, isEncrypted);
		}

		public string Name { get; set; }

		public Value2 Value { get; set; }

		public override string ToString() => $"{Name}: {Value}";
	}

	public class Value2 {

		public Value2() { }

		public Value2(string value, bool isEncrypted=false) {
			Value       = value;
			IsEncrypted = isEncrypted;
		}

		public bool IsEncrypted { get; set; }
		public string Value { get; set; }

		public override string ToString() {
			var secure = IsEncrypted ? "secure! " : "";
			return $"{secure}{Value}";
		}
	}
}
