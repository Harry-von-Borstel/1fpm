using System;
using blueshell.rfc822;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestRfc822
{
	[TestClass]
	public class TestHeaderFields
	{
		[TestMethod]
		public void TestMinimalHeaderFields()
		{
			var headerFields = new HeaderFields();

			headerFields.Date = new DateTime(1976, 8, 26, 14, 29, 7, DateTimeKind.Utc);
			headerFields.From = new Mailbox("Jones@Registry.Org");

			Assert.AreEqual("Date:\tThu, 26 Aug 1976 14:29:07 GMT\r\nFrom:\tJones@Registry.Org\r\n", headerFields.ToString());
		}

		[TestMethod]
		public void TestSpecialHeaderFields()
		{
			var headerFields = new HeaderFields();

			headerFields.ContentDispositionFileName = "MyFile.txt";

			Assert.AreEqual("ContentDisposition:	attachment; filename=MyFile.txt\r\n", headerFields.ToString());
		}


        [TestMethod]
        public void TestSubjectHeaderFieldPlain()
        {
            var headerFields = new HeaderFields();

            headerFields.Subject = "This is my subject";

            Assert.AreEqual("Subject:	This is my subject\r\n", headerFields.ToString());
        }

		[TestMethod]
		[DataRow("Content-ID:\t<xy.z>\r\n","Content-ID","<xy.z>")]
		public void TestCustomHeaderField(string expectedResult, string field,params string[] contentItems)
		{
			var headerFields = new HeaderFields();

			headerFields[field] = new HeaderFieldBody(contentItems);

			headerFields.ToString(field).Should().Be(expectedResult);
		}
	}
}