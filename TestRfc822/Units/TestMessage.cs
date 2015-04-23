using Microsoft.VisualStudio.TestTools.UnitTesting;
using blueshell.rfc822;
using TestUtils;
using System.IO;

namespace TestRfc822.Units
{
	[TestClass]
	public class TestMessage
	{
		[TestMethod]
		public void TestMessageFromFilePlain()
		{
			var m = new Message();
			Assert.IsTrue(m.FromFile(@"etc\plain.eml"));
			AssertText.StringsAreEqual(
				@"Your message could not be delivered / please include <NOSPAM>

Sorry, but your mail could not be delivered, because it has been declined by spamdefence. Please send it again and include the text ""<NOSPAM>"" in the subject to bypass the spamdefence.

_____________________________________________________________

Es tut uns leid, aber Ihre Mail konnte nicht zugestellt werden, weil sie von Spamdefence abgelehnt wurde. Bitte senden Sie ihre Nachricht noch mal und fuegen Sie den Text ""<NOSPAM>"" in die Betreffzeile ein, um Spamdefence zu umgehen.



",
				m.Text);
			Assert.AreEqual("declined@blueshell.com", m.HeaderFields.From.ToString());
			Assert.AreEqual("test@blueshell.com", m.HeaderFields.To.ToString());
			Assert.AreEqual("0 (Authenticated)", m.HeaderFields["X-SmarterMail-TotalSpamWeight"].ToString());
			Assert.AreEqual(@"etc\plain.eml.parts\1.txt", m.Filename);
			Directory.SetCurrentDirectory("etc");
			m = new Message();
			Assert.IsTrue(m.FromFile(@"plain.eml"));
			Assert.AreEqual(@"plain.eml.parts\1.txt", m.Filename);
			m = new Message();
			var fp = Path.GetFullPath(@"plain.eml");
			Assert.IsTrue(m.FromFile(fp));
			Assert.AreEqual(fp + @".parts\1.txt", m.Filename);
			Directory.SetCurrentDirectory("..");
		}

		[TestMethod]
		public void TestMessageFromFileSimpleMultipart()
		{
			var m = new Message();
			Assert.IsTrue(m.FromFile(@"etc\simpleMultipart.eml"));
			Assert.IsTrue(m.IsMultiPart);
			var expPreamble = @"This is the preamble.  It is to be ignored, though it
is a handy place for composition agents to include an
explanatory note to non-MIME conformant readers.
";
			var expEpilogue = @"
This is the epilogue.  It is also to be ignored.


";
			AssertText.StringsAreEqual(expPreamble, m.Preamble);
			Assert.AreEqual(2, m.InnerParts.Count);
			AssertText.StringsAreEqual(expEpilogue, m.Epilogue);
			Assert.AreEqual("Nathaniel Borenstein <nsb@bellcore.com>", m.HeaderFields.From.ToString());
			Assert.AreEqual("Ned Freed <ned@innosoft.com>", m.HeaderFields.To.ToString());
			Assert.AreEqual("1.0", m.HeaderFields["MIME-Version"].ToString());
			AssertText.StringsAreEqual(
				@"This is implicitly typed plain US-ASCII text.
It does NOT end with a linebreak.",
			m.InnerParts[0].Text);
			AssertText.StringsAreEqual(@"This is explicitly typed plain US-ASCII text.
It DOES end with a linebreak.
", m.InnerParts[1].Text);
			Assert.AreEqual(@"etc\simpleMultipart.eml.parts\1", m.Filename);
			Assert.AreEqual(@"etc\simpleMultipart.eml.parts\1\1.txt", m.InnerParts[0].Filename);
			Assert.AreEqual(@"etc\simpleMultipart.eml.parts\1\2.txt", m.InnerParts[1].Filename);
		}

		[TestMethod]
		public void TestMessageFromFileMultipartHtml()
		{
			var m = new Message();
			Assert.IsTrue(m.FromFile(@"etc\multipartHtml.eml"));
			Assert.AreEqual("\"testa93@blueshell.com\" <TestA93@blueshell.com>", m.HeaderFields.From.ToString());
			Assert.AreEqual("<test94@blueshell.com>", m.HeaderFields.To.ToString());
			Assert.IsTrue(m.IsMultiPart);
			Assert.AreEqual(2, m.InnerParts.Count);
			AssertText.StringsAreEqual("\r\nTest\r\n\r\n", m.InnerParts[0].Text);
			AssertText.StringsAreEqual(
				@"<span style=""font-family: Arial, Helvetica, sans-serif; font-size: 10pt""><br />
T<strong>est</strong><br />
<div></div></span>
",
			m.InnerParts[1].Text);
			AssertText.StringsAreEqual(null, m.Text);
			AssertText.StringsAreEqual("This is a multipart message in MIME format.\r\n", m.Preamble);
			AssertText.StringsAreEqual("\r\n", m.Epilogue);
		}

		[TestMethod]
		public void TestMessageFromFileMultipartAndroid()
		{
			var m = new Message();
			Assert.IsTrue(m.FromFile(@"etc\multipartAndroid.eml"));
			Assert.IsTrue(m.IsMultiPart);
			Assert.AreEqual(2, m.InnerParts.Count);
			Assert.AreEqual(
				null,
				m.Text);
			Assert.AreEqual("Harry von Borstel <test94@blueshell.com>", m.HeaderFields.From.ToString());
			Assert.AreEqual("\"'Test93@Blueshell. Com'\" <test93@blueshell.com>", m.HeaderFields.To.ToString());
			Assert.AreEqual("1.0", m.HeaderFields["MIME-Version"].ToString());
			Assert.IsTrue(m.IsMultiPart);
			Assert.AreEqual(2, m.InnerParts.Count);
			AssertText.StringsAreEqual("Test....\n\n\n\nVon Samsung Mobile gesendet", m.InnerParts[0].Text);
			AssertText.StringsAreEqual(
				@"<html><head><meta http-equiv=""Content-Type"" content=""text/html; charset=UTF-8""></head><body ><div>Test....</div><div><br></div><div><br></div><div><br></div><div><div style=""font-size:75%;color:#575757"">Von Samsung Mobile gesendet</div></div></body></html>",
			m.InnerParts[1].Text);
		}

		[TestMethod]
		public void TestMessageFromFileMultipartHtmlImage()
		{
			var m = new Message();
			Assert.IsTrue(m.FromFile(@"etc\multipartHtmlImage.eml"));
			Assert.AreEqual("Maifestkomitee <maiko@blueshell.com>", m.HeaderFields.From.ToString());
			Assert.AreEqual("Mai Friends <maiko@blueshell.com>", m.HeaderFields.To.ToString());
			Assert.IsTrue(m.IsMultiPart);
			Assert.AreEqual(2, m.InnerParts.Count);
			AssertText.StringsAreEqual(null, m.Text);
			AssertText.StringsAreEqual("This is a multi-part message in MIME format.", m.Preamble);
			AssertText.StringsAreEqual("\r\n", m.Epilogue);
			var innerMulti = m.InnerParts[1];
			Assert.IsTrue(innerMulti.IsMultiPart);
			Assert.AreEqual(6, innerMulti.InnerParts.Count);
			AssertText.FoldersAreEqual(@"etc\expected\multipartHtmlImage.eml.parts", @"etc\multipartHtmlImage.eml.parts");
		}

		[TestMethod]
		public void TestMessageToFilePlain()
		{
			// Read message
			var m = new Message();
			Assert.IsTrue(m.FromFile(@"etc\plain.eml"));
			AssertText.FilesAreEqual(@"etc\expected\plain.eml.parts\1.txt", @"etc\plain.eml.parts\1.txt");

			// Render message
			m.ToFile(@"out\plain.eml");
			AssertText.FilesAreEqual(@"etc\expected\plain.eml", @"out\plain.eml");

			// Read rendered message
			var m2 = new Message();
			Assert.IsTrue(m2.FromFile(@"out\plain.eml"));
			AssertText.FilesAreEqual(@"etc\expected\plain.eml.parts\1.txt", @"out\plain.eml.parts\1.txt");

			// Re-Render message
			m2.ToFile(@"out\plain2.eml");
			AssertText.FilesAreEqual(@"etc\expected\plain.eml", @"out\plain2.eml");

		}

		[TestMethod]
		public void TestMessageToFilePlainWithAttachment()
		{
			TestMessageToFile("plainWithAttachment");
		}

		[TestMethod]
		public void TestMessageToFileSimpleMultipart()
		{
			TestMessageToFile("simpleMultipart");
		}

		[TestMethod]
		public void TestMessageToFileMultipartHtml()
		{
			TestMessageToFile("multipartHtml");
		}

		[TestMethod]
		public void TestMessageToFileMultipartAndroid()
		{
			TestMessageToFile("multipartAndroid");
		}

		[TestMethod]
		public void TestMessageToFileMultipartHtmlImage()
		{
			TestMessageToFile("multipartHtmlImage");
		}

		private static void TestMessageToFile(string bareName)
		{
			// Read Message
			var m = new Message();
			Assert.IsTrue(m.FromFile(string.Format(@"etc\{0}.eml", bareName)));

			// Render message
			m.ToFile(string.Format(@"out\{0}.eml", bareName));
			AssertText.FilesAreEqual(string.Format(@"etc\expected\{0}.eml", bareName), string.Format(@"out\{0}.eml", bareName));

			// Read rendered message
			var m2 = new Message();
			Assert.IsTrue(m2.FromFile(string.Format(@"out\{0}.eml", bareName)));

			// Re-Render message
			m2.ToFile(string.Format(@"out\{0}2.eml", bareName));
			AssertText.FilesAreEqual(string.Format(@"etc\expected\{0}.eml", bareName), string.Format(@"out\{0}2.eml", bareName));
		}

		/// <summary>
		/// Test adding a file to a message
		/// </summary>
		[TestMethod]
		public void TestMessageAddFile()
		{
			var bareName = "multipartHtmlImage";

			// Read Message
			var m = new Message();
			Assert.IsTrue(m.FromFile(string.Format(@"etc\{0}.eml", bareName)));

			var file1 = string.Format(@"etc\{0}.eml.parts\1\2\test.txt", bareName);
			File.Copy(@"etc\test.txt", file1, true);
			m.AddFile(file1);
			m.ToFile(@"out\multipartHtmlImageA.eml");
			AssertText.FilesAreEqual(@"etc\expected\multipartHtmlImageA.eml", @"out\multipartHtmlImageA.eml");
		}

	}
}
