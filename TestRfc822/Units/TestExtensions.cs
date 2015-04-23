using Microsoft.VisualStudio.TestTools.UnitTesting;

using blueshell.rfc822;
using System.IO;

namespace TestRfc822
{
	[TestClass]
	public class TestExtensions
	{
		[TestMethod]
		public void TestReadMessageLine()
		{
			var s = "1st Line\r\n2nd Line with\rembedded CR\r\n3rd Line with\nembedded LF";
			var sr = new StringReader(s);
			foreach (var pass in new string[] { "1st pass", "2nd pass" })
			{
				int lineCounter = 0;
				Assert.AreEqual("1st Line", sr.ReadMessageLine(ref lineCounter), pass);
				Assert.AreEqual(1, lineCounter);
				Assert.AreEqual("2nd Line with\rembedded CR", sr.ReadMessageLine(ref lineCounter), pass);
				Assert.AreEqual(2, lineCounter);
				Assert.AreEqual("3rd Line with\nembedded LF", sr.ReadMessageLine(ref lineCounter), pass);
				Assert.AreEqual(3, lineCounter);
				s = s + "\r\n";
				sr.Close();
				sr = new StringReader(s);
			}
			sr.Close();
		}

		[TestMethod]
		public void TestFullFieldLine()
		{
			var s = "1st Line\r\n2nd Line\r\n\tTAB-folded with\rembedded CR\r\n3rd Line\r\n SPACE-folded with\nembedded LF";
			var sr = new StringReader(s);
			foreach (var pass in new string[] { "1st pass", "2nd pass" })
			{
				int lineCounter = 0;
				Assert.AreEqual("1st Line", sr.ReadFullFieldLine(ref lineCounter), pass);
				Assert.AreEqual(1, lineCounter);
				Assert.AreEqual("2nd Line TAB-folded with\rembedded CR", sr.ReadFullFieldLine(ref lineCounter), pass);
				Assert.AreEqual(3, lineCounter);
				Assert.AreEqual("3rd Line SPACE-folded with\nembedded LF", sr.ReadFullFieldLine(ref lineCounter), pass);
				Assert.AreEqual(5, lineCounter);
				s = s + "\r\n";
				sr.Close();
				sr = new StringReader(s);
			}
			sr.Close();
		}

	}
}
