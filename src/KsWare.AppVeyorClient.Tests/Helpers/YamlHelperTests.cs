using KsWare.AppVeyorClient.Helpers;
using Moq;
using NUnit.Framework;
using System;

namespace KsWare.AppVeyorClient.Tests.Helpers
{
	[TestFixture]
	public class YamlHelperTests
	{
		private MockRepository _mockRepository; 


		[SetUp]
		public void SetUp()
		{
			_mockRepository = new MockRepository(MockBehavior.Strict);


		}

		[TearDown]
		public void TearDown()
		{
			_mockRepository.VerifyAll();
		}

		[TestCase("    - ps: foo", ExpectedResult = "- ps: ")]
		[TestCase("    - ps: |+2\n", ExpectedResult = "- ps: |+2")]
		[TestCase("    - ps: >+\n", ExpectedResult = "- ps: >+")]
		[TestCase("    - ps: foo", ExpectedResult = "- ps: ")]
		[TestCase("    - ps: \"foo\"", ExpectedResult = "- ps: \"")]
		[TestCase("    - ps: 'foo'", ExpectedResult = "- ps: '")]
		[TestCase("    - foo", ExpectedResult = "- ")]
		[TestCase("    - |+2\n", ExpectedResult = "- |+2")]
		[TestCase("    - >+\n", ExpectedResult = "- >+")]
		[TestCase("    - foo", ExpectedResult = "- ")]
		[TestCase("    - \"foo\"", ExpectedResult = "- \"")]
		[TestCase("    - 'foo'", ExpectedResult = "- '")]
		[TestCase("- ps: |+2\n", ExpectedResult = "- ps: |+2")]
		[TestCase("- |+2\n", ExpectedResult = "- |+2")]
		public string GetSuffix_TestData_Success(string input)
			=> YamlHelper.GetSuffix(input);

		[TestCase("    - ps: |+2\r\n", ExpectedResult = 4)]
		[TestCase("- ps: |+2\r\n", ExpectedResult = 0)]
		public int GetIndent_TestData_Success(string input)
			=> YamlHelper.GetIndent(input);


		[TestCase("foo", "- ps: ", ScalarType.Plain, ExpectedResult = "  - ps: foo")]
		[TestCase("foo", "- ps: ", ScalarType.BlockFolded, ExpectedResult = "  - ps: >\r\n      foo")]
		[TestCase("foo", "- ps: ", ScalarType.BlockFoldedKeep, ExpectedResult = "  - ps: >+\r\n      foo")]
		[TestCase("foo", "- ps: ", ScalarType.BlockFoldedStrip, ExpectedResult = "  - ps: >-\r\n      foo")]
		[TestCase("foo", "- ps: ", ScalarType.BlockLiteral, ExpectedResult = "  - ps: |\r\n      foo")]
		[TestCase("foo", "- ps: ", ScalarType.BlockLiteralKeep, ExpectedResult = "  - ps: |+\r\n      foo")]
		[TestCase("foo", "- ps: ", ScalarType.BlockLiteralStrip, ExpectedResult = "  - ps: |-\r\n      foo")]
		[TestCase("foo", "- ps: ", ScalarType.FlowDoubleQuoted, ExpectedResult = "  - ps: \"foo\"")]
		[TestCase("foo", "- ps: ", ScalarType.FlowSingleQuoted, ExpectedResult = "  - ps: 'foo'")]
		public string FormatBlock_TestData_Success(string input, string suffix, ScalarType scalarType)
			=> YamlHelper.FormatBlock(input, suffix, 2, scalarType);

		[TestCase("foo", ExpectedResult = "foo")]
		[TestCase("foo' \n\tbar", ExpectedResult = "foo'' \n\tbar")]
		public string EscapeSingleQuotedString_TestData_Success(string input)
			=> YamlHelper.EscapeSingleQuotedString(input);

		[TestCase("foo", ExpectedResult = "foo")]
		[TestCase("\n", ExpectedResult = "\\n")]
		[TestCase("\r\n", ExpectedResult = "\\r\\n")]
		[TestCase("foo\" \nbar\\glue\\n", ExpectedResult = "foo\\\" \\nbar\\\\glue\\\\n")]
		public string EscapeDoubleQuotedString_TestData_Success(string input)
			=> YamlHelper.EscapeDoubleQuotedString(input);

		[TestCase("- ps: ", ExpectedResult = ScalarType.Plain)]
		[TestCase("- ps: >", ExpectedResult = ScalarType.BlockFolded)]
		[TestCase("- ps: >+", ExpectedResult = ScalarType.BlockFoldedKeep)]
		[TestCase("- ps: >-", ExpectedResult = ScalarType.BlockFoldedStrip)]
		[TestCase("- ps: |", ExpectedResult = ScalarType.BlockLiteral)]
		[TestCase("- ps: |-", ExpectedResult = ScalarType.BlockLiteralStrip)]
		[TestCase("- ps: |+", ExpectedResult = ScalarType.BlockLiteralKeep)]
		[TestCase("- ps: \"", ExpectedResult = ScalarType.FlowDoubleQuoted)]
		[TestCase("- ps: '", ExpectedResult = ScalarType.FlowSingleQuoted)]
		[TestCase("- ", ExpectedResult = ScalarType.Plain)]
		[TestCase("- >", ExpectedResult = ScalarType.BlockFolded)]
		[TestCase("- >+", ExpectedResult = ScalarType.BlockFoldedKeep)]
		[TestCase("- >-", ExpectedResult = ScalarType.BlockFoldedStrip)]
		[TestCase("- |", ExpectedResult = ScalarType.BlockLiteral)]
		[TestCase("- |-", ExpectedResult = ScalarType.BlockLiteralStrip)]
		[TestCase("- |+", ExpectedResult = ScalarType.BlockLiteralKeep)]
		[TestCase("- \"", ExpectedResult = ScalarType.FlowDoubleQuoted)]
		[TestCase("- '", ExpectedResult = ScalarType.FlowSingleQuoted)]
		public ScalarType DetectScalarType_TestData_Success(string suffix)
			=> YamlHelper.DetectScalarType(suffix);

		[TestCase("- foo", ExpectedResult = "foo")]
		[TestCase("- ps: foo", ExpectedResult = "foo")]
		[TestCase("- ps: \"foo\"", ExpectedResult = "foo")]
		[TestCase("- ps: 'foo'", ExpectedResult = "foo")]
		[TestCase("- ps: |\nfoo", ExpectedResult = "foo")]
		[TestCase("- ps: >\nfoo", ExpectedResult = "foo")]
		[TestCase("- foo\nbar", ExpectedResult = "foo\nbar")]
		public string ExtractBlock_TestData_Content_Success(string input) 
			=> YamlHelper.ExtractBlock(input).Content;

		[TestCase("foo''\nbar\\n", ExpectedResult = "foo'\nbar\\n")]
		public string UnescapeSingleQuotedString_TestData_Success(string input)
			=> YamlHelper.UnescapeSingleQuotedString(input);

		[TestCase("\n  foo\n\n  bar", ScalarType.BlockFolded, ExpectedResult = "foo\n\nbar")]
		[TestCase("\n    foo\n\n    bar", ScalarType.BlockFolded, ExpectedResult = "foo\n\nbar")]
		public string UnescapeBlock_TestData_Success(string input, ScalarType scalarType)
			=> YamlHelper.UnescapeBlock(input, scalarType);

		[TestCase("\\\"", ExpectedResult = "\"")]
		[TestCase("\\n", ExpectedResult = "\n")]
		[TestCase("\\r\\n", ExpectedResult = "\r\n")]
		[TestCase("\\\\", ExpectedResult = "\\")]
		[TestCase("foo\\\"bar", ExpectedResult = "foo\"bar")]
		public string UnescapeDoubleQuotedString_TestData_Success(string input)
			=> YamlHelper.UnescapeDoubleQuotedString(input);
	}
}
