using System.Text.RegularExpressions;
using blueshell.rfc822;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestRfc822
{
	[TestClass]
	public class TestRe
	{
		[TestMethod]
		public void TestRegularExpressions()
		{
			RegTester(Re.ATOM, "The");
			RegTester(Re.WORD, "The");
			RegTester(Re.PHRASE, "The quick brown fox");
			RegTester(Re.MAILBOX, "Alfred.E@Neumann.com");
			RegTester(Re.GROUP, "Nerds: Alfred.E@Neumann.com, Sheldon@Cooper.org");
		}

		static void RegTester(string regex, string argument)
		{
			var r = new Regex(regex);
			var m = r.Match(argument);
			Assert.AreEqual(argument, m.Value, string.Format("Regex: '{0}'", regex));
		}
	}
}