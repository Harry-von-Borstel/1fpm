using blueshell.rfc822;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Text;

namespace TestRfc822
{
	[TestClass]
	public class TestHeaderFieldBody
	{
        public TestHeaderFieldBody()
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        }

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
        public void TestHeaderFieldSpecial()
        // Tests RFC2047 encoding https://www.ietf.org/rfc/rfc2047.txt
        {
            var naturalText = "German: ÄÖÜäöüß French: ÁÀÉÈÔÊÂáàéèôêâÆæÔôÇç Spanish: Ññ¡¿";
            var encodedText = 
                    "=?iso-8859-15?Q?German:_=C4=D6=DC=E4=F6=FC=DF_French:_=C1=C0=C9=C8=D4=CA?=\r\n"
                    + " =?iso-8859-15?Q?=C2=E1=E0=E9=E8=F4=EA=E2=C6=E6=D4=F4=C7=E7_Spanish:_=D1?=\r\n"
                    + " =?iso-8859-15?Q?=F1=A1=BF?=\r\n";
            var hf = new HeaderFieldBody(naturalText);
            Assert.AreEqual(
                "Name:\r\n\t" + encodedText,
                hf.ToString("Name"));

            var hf2 = new HeaderFieldBody(encodedText);
            Assert.AreEqual(
                "Name2:\r\n\t" + encodedText,
                hf2.ToString("Name2"));
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

        [TestMethod]
        public void TestHeaderFieldEncodeFold()
        {
            var hf = new HeaderFieldBody(
            "ThisIsAReallyVeryLongLineThatNeedsToHaveSomeFoldingButThatHasNoWhitespaceToLetTheFoldHappenWithoutInterruptingTheWord");
            var act = hf.ToString("Field-name");
            Assert.AreEqual(
                "Field-name:\r\n" + "\t=?iso-8859-15?Q?ThisIsAReallyVeryLongLineThatNeedsToHaveSomeFoldingBut?=\r\n"
                    + " =?iso-8859-15?Q?ThatHasNoWhitespaceToLetTheFoldHappenWithoutInterrupti?=\r\n"
                    + " =?iso-8859-15?Q?ngTheWord?=\r\n",
                act);
        }

        [TestMethod]
        public void TestHeaderFieldDecode1()
        {
            var e1 = false;
            var act = HeaderFieldBody.DecodeWord("=?iso-8859-15?Q?De:_=C4=D6=DC=E4=F6=FC=DF_Fr:_=C1=C0=C9=C8=D4=CA?=", ref e1);

            Assert.AreEqual(
                "De: ÄÖÜäöüß Fr: ÁÀÉÈÔÊ",
                act
                );
        }

        [TestMethod]
        public void TestHeaderFieldDecode2()
        {
            var e1 = false;
            var act = HeaderFieldBody.DecodeWord("=?iso-8859-15?Q?Spec:_=A4=A6=A8=B4=B8=BC=BD=BE?=",
                ref e1);

            Assert.AreEqual(
                "Spec: €ŠšŽžŒœŸ",
                act
                );
        }
    }
}