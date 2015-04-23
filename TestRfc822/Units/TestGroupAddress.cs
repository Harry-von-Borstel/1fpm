using System;
using blueshell.rfc822;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestRfc822
{
	[TestClass]
	public class TestGroupAddress
	{
		[TestMethod]
		[ExpectedException(typeof(FormatException))]
		public void TestGroupAddressNonsense()
		{
			var m = new GroupAddress("Nonsense");
		}

		[TestMethod]
		public void TestGroupAddressEmpty()
		{
			var m = new GroupAddress("MyGroup:");
			Assert.AreEqual("MyGroup:", m.ToString());
		}

		[TestMethod]
		public void TestGroupAddressNeumann()
		{
			var m = new GroupAddress("Nerds: Alfred.E@Neumann.com, Sheldon@Cooper.org");
			Assert.AreEqual("Nerds: Alfred.E@Neumann.com, Sheldon@Cooper.org", m.ToString());
		}

	}
}