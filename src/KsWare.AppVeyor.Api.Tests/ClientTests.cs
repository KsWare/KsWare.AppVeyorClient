using System.Threading.Tasks;
using NUnit.Framework;

namespace KsWare.AppVeyor.Api.Tests {

	//[TestFixture]// one time manual test
	public class ClientTests {

		string token = ""; //<== input YOUR token";
		string accountName = "KsWare";//<== input YOUR account name";

		//[Test]//manual test, 2021-09-01 tested with v1 and v2 tokens
		public async Task EncryptTest() {
			// var token = GetLocalToken();
			var client = new Client(token);

			var encrypted = await client.Encrypt("xyz", accountName);
			Assert.That(encrypted, Is.EqualTo("YPkroYAmw40xB0P7WEeVyA=="));//<== input YOUR result";
		}
	}
}
