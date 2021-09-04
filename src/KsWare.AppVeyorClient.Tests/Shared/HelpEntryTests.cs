using System.Windows;
using KsWare.AppVeyorClient.Shared;
using NUnit.Framework;

namespace KsWare.AppVeyorClient.Tests.Shared {

	[TestFixture]
	public class HelpEntryTests {

		[OneTimeSetUp]
		public void OneTimeSetUp() {
			if (Application.Current == null) new Application(); // registers "pack:"
		}

		[Test]
		public void LoadResourceTest() {
			var help = HelpEntry.LoadResource();
		}

	}

}
