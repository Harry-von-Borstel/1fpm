using blueshell.rfc822;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestRfc822
{
	[TestClass]
	public class TestHeaderFieldBody
	{
		[TestMethod]
		public void TestHeaderFieldEmpty()
		{
			var hf = new HeaderFieldBody();
			Assert.IsNull(hf.ToString("Name"));
		}

		[TestMethod]
		public void TestHeaderFieldNull()
		{
			var hf = new HeaderFieldBody(null);
			Assert.IsNull(hf.ToString("Name"));
		}

		[TestMethod]
		public void TestHeaderFieldSimple()
		{
			var hf = new HeaderFieldBody("This is a simple body");
			Assert.AreEqual("Name:\tThis is a simple body\r\n" ,hf.ToString("Name"));
		}

		[TestMethod]
		public void TestHeaderFieldPreserveFolding()
		{
			var hf = new HeaderFieldBody("This is a\r\n\tfolded body");
			Assert.AreEqual("Name:\tThis is a\r\n\tfolded body\r\n", hf.ToString("Name"));
		}

		[TestMethod]
		public void TestHeaderFieldFold()
		{
			var hf = new HeaderFieldBody("This is a long body that needs some folding in order to prevent lines longer than 78+2 characters.");
			var act = hf.ToString("This-really-long-field-name-counts-for-the-total-length-too");
			Assert.AreEqual("This-really-long-field-name-counts-for-the-total-length-too:\tThis is a long\r\n body that needs some folding in order to prevent lines longer than 78+2\r\n characters.\r\n", act);
		}
	}
}