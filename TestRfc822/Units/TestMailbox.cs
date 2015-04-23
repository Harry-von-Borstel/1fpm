using System;
using blueshell.rfc822;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestRfc822
{
	[TestClass]
	public class TestMailbox
	{
		[TestMethod]
		[ExpectedException(typeof(FormatException))]
		public void TestMailboxNonsense()
		{
			var m = new Mailbox("Nonsense");
		}

		[TestMethod]
		[ExpectedException(typeof(FormatException))]
		public void TestMailboxSemicolon()
		{
			var m = new Mailbox(";Alfred.E@Neumann.com");
		}

		[TestMethod]
		public void TestMailboxNeumann()
		{
			var m = new Mailbox("Alfred.E@Neumann.com");
			Assert.AreEqual("Alfred.E@Neumann.com", m.ToString());
		}

		[TestMethod]
		public void TestMailboxPlusAddress()
		{
			var m = new Mailbox("Alfred.E+private@Neumann.com");
			Assert.AreEqual("Alfred.E+private@Neumann.com", m.ToString());
		}
	}
}