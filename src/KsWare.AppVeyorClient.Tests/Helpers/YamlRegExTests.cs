using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KsWare.AppVeyorClient.Helpers;
using NUnit.Framework;

namespace KsWare.AppVeyorClient.Tests.Helpers {

	[TestFixture]
	public class YamlRegExTests {

		[TestCase("my_name: value", "my_name:")]
		[TestCase("- Name: value", "- Name:")]
		[TestCase("name:", "name:")]
		[TestCase("  name:", "name:")]
		[TestCase("- name:", "- name:")]
		[TestCase("name: |", "name:")]
		[TestCase("  - name:", "- name:")]
		[TestCase("- git", "- ")]
		public void A(string s, string entry) {
			var match = YamlRegEx.Match(s);
			Assert.That(match.Success, Is.True);
			Assert.That(match.Entry, Is.EqualTo(entry));
		}
	}
}
