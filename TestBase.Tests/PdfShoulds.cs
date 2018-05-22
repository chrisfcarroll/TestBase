using System.Text;
using NUnit.Framework;

namespace TestBase.Tests
{
    [TestFixture]
    public class PdfShoulds
    {
        [Test]
        public void PdfShouldInsertGivenLine()
        {
            Pdf.DocumentWithLineOfText("my line of text")
                .ShouldStartWith("%PDF-")
                .ShouldContain("my line of text");
        }

        [Test]
        public void PdfBytesShouldBeParseableAsAscii()
        {
            var bytes= Pdf.AsciiBytes("my bite of text");

            Encoding.ASCII.GetString(bytes)
                .ShouldStartWith("%PDF-")
                .ShouldContain("my bite of text");
        }

    }
}
