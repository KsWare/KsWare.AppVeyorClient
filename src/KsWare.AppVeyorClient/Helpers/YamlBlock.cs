namespace KsWare.AppVeyorClient.Helpers
{
	public class YamlBlock {
		public YamlBlock(int indentLength, string suffix, string content)
		{
			Indent=indentLength;
			Suffix = suffix;
			Content = content;
		}

		public int Indent { get; set; }
		public string Suffix { get; set; }
		public string Content { get; set; }
	}
}