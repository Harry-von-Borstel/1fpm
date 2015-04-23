using Microsoft.VisualStudio.TestTools.UnitTesting;
using blueshell.rfc822;
using System.Net.Mime;
using System.Text;
using TestUtils;

namespace TestRfc822.Units
{
    [TestClass]
    public class TestMessagePart
    {
        [TestMethod]
        public void TestMessagePartDecode()
        {
            bool doLineBreak;
            AssertText.BytesAreEqual(
                Encoding.ASCII.GetBytes( "The quick brown"), 
                MessagePart.Decode( "The quick brown= " , TransferEncoding.QuotedPrintable, Encoding.ASCII, out doLineBreak));
            Assert.IsFalse(doLineBreak);
            AssertText.BytesAreEqual(
                Encoding.ASCII.GetBytes("Hello!"),
				MessagePart.Decode("Hello=21", TransferEncoding.QuotedPrintable, Encoding.ASCII, out doLineBreak));
            Assert.IsTrue(doLineBreak);

			AssertText.BytesAreEqual(
				Encoding.GetEncoding("ISO-8859-15").GetBytes( "ÄÖÜäöüÁÀáà"),
				MessagePart.Decode("ÄÖÜäöüÁÀáà", TransferEncoding.EightBit, Encoding.GetEncoding("ISO-8859-15"), out doLineBreak));
			Assert.IsTrue(doLineBreak);
		}

        [TestMethod]
        public void TestMessagePartEncode()
        {
            var data = System.Text.Encoding.ASCII.GetBytes("1+2=3");
            Assert.AreEqual(
                "1+2=3D3",
                MessagePart.Encode(data, data.Length, TransferEncoding.QuotedPrintable, Encoding.ASCII)
                );

            data = System.Text.Encoding.ASCII.GetBytes(
                "This is a long line that should be folded by soft breaks in order to prevent too long lines. Thus, Quoted-printable encoding ensures, that each line is not longer than 76 characters. Even special characters like \"=\" should not violate this rule.");
            var expected1 = "This is a long line that should be folded by soft breaks in order to preven=";
            Assert.AreEqual(76, expected1.Length);
            var expected2 = "t too long lines. Thus, Quoted-printable encoding ensures, that each line i=";
            Assert.AreEqual(76, expected2.Length);
            var expected3 = "s not longer than 76 characters. Even special characters like \"=3D\" should =";
            Assert.AreEqual(76, expected3.Length);
            var expected4 = "not violate this rule.";
            Assert.AreEqual(
                expected1 + "\r\n" + expected2 + "\r\n" + expected3 + "\r\n" + expected4,
				MessagePart.Encode(data, data.Length, TransferEncoding.QuotedPrintable, Encoding.ASCII)
                );

			data = System.Text.Encoding.ASCII.GetBytes(
				"Sequences of CRLF should\r\nstart a new line in order to maintain readability.");
			Assert.AreEqual(
				"Sequences of CRLF should\r\nstart a new line in order to maintain readability.",
				 MessagePart.Encode(data, data.Length, TransferEncoding.QuotedPrintable, Encoding.ASCII)
				);


            data = System.Text.Encoding.ASCII.GetBytes(
                "Trailing linear whitespace must be coded as hex-octets.\t \t");
            Assert.AreEqual(
                "Trailing linear whitespace must be coded as hex-octets.\t =09",
				 MessagePart.Encode(data, data.Length, TransferEncoding.QuotedPrintable, Encoding.ASCII)
                );

            data = System.Text.Encoding.UTF8.GetBytes(
                "Text including controls like bell \a or international unicode characters like ÄÖÜßÁÀé et al. can be encoded using Base64.");
            AssertText.StringsAreEqual(
                @"VGV4dCBpbmNsdWRpbmcgY29udHJvbHMgbGlrZSBiZWxsIAcgb3IgaW50ZXJuYXRpb25hbCB1bmlj
b2RlIGNoYXJhY3RlcnMgbGlrZSDDhMOWw5zDn8OBw4DDqSBldCBhbC4gY2FuIGJlIGVuY29kZWQg
dXNpbmcgQmFzZTY0Lg==
",
				 MessagePart.Encode(data, data.Length, TransferEncoding.Base64, Encoding.ASCII)
                );
        }
    }
}
