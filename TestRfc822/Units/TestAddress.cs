using System;
using blueshell.rfc822;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestRfc822
{
	[TestClass]
	public class TestAddress
	{
		[TestMethod]
		[ExpectedException(typeof(FormatException))]
		public void TestAddressNonsense()
		{
			var m = new Address("Nonsense");
		}

		[TestMethod]
		public void TestAddressNeumann()
		{
			var m = new Address("Alfred.E@Neumann.com");
			Assert.AreEqual("Alfred.E@Neumann.com", m.ToString());
		}

	}
}