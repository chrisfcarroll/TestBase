using System;
using System.IO;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace TestBase.Tests.ShouldsCorrectnessTests;

[TestFixture]
public class StreamShouldsShould
{
    [Test]
    public void ShouldHaveSameStreamContentAs_Should_fail__Given_different_content()
    {
            using (var left = new MemoryStream(Encoding.UTF8.GetBytes("Hello there")))
            using (var right = new MemoryStream(Encoding.UTF8.GetBytes("Hello and Goodbye")))
            {
                Assert.Throws<Assertion>(() => left.ShouldEqualByStreamContent(right));
                Assert.Throws<Assertion>(() => left.ShouldHaveSameStreamContentAs(right));
            }
        }

    [Test]
    public void ShouldHaveSameStreamContentAs_Should_fail_WithUseableErrorMessage__Given_different_content()
    {
            using (var left = new MemoryStream(Encoding.UTF8.GetBytes("Hello there")))
            using (var right = new MemoryStream(Encoding.UTF8.GetBytes("Hello and Goodbye")))
            {
                const int expectedMismatchPosition = 6;

                var e = Assert.Throws<Assertion>(() => left.ShouldEqualByStreamContent(right));
                e.Message
                 .ShouldMatchIgnoringCase("(mismatch|differ)")
                 .ShouldMatchIgnoringCase($@"\b{expectedMismatchPosition}\b")
                 .ShouldMatchIgnoringCase("stream");

                e = Assert.Throws<Assertion>(() => left.ShouldHaveSameStreamContentAs(right));
                e.Message
                 .ShouldMatchIgnoringCase("mismatch|differ")
                 .ShouldMatchIgnoringCase($@"\b{expectedMismatchPosition}\b")
                 .ShouldMatchIgnoringCase("stream");
            }
        }

    [Test]
    public void ShouldHaveSameStreamContentAs_Should_fail_WithUseableErrorMessage__Given_long_different_content()
    {
            var rnd = new Random();
            var longtext = Enumerable.Range(0, 2000)
                                     .Aggregate("This is long ", (s, i) => s + (char) (32 + rnd.Next(60)));
            using (var left = new MemoryStream(Encoding.UTF8.GetBytes(longtext + "Hello there")))
            using (var right = new MemoryStream(Encoding.UTF8.GetBytes(longtext + "Hello and Goodbye")))
            {
                var e = Assert.Throws<Assertion>(() => left.ShouldEqualByStreamContent(right));
                e.Message
                 .ShouldMatchIgnoringCase("(mismatch|differ)")
                 .ShouldMatchIgnoringCase(@"\b2\d\d\d\b")
                 .ShouldMatchIgnoringCase("stream");

                e = Assert.Throws<Assertion>(() => left.ShouldHaveSameStreamContentAs(right));
                e.Message
                 .ShouldMatchIgnoringCase("mismatch|differ")
                 .ShouldMatchIgnoringCase(@"\b2\d\d\d\b")
                 .ShouldMatchIgnoringCase("stream");
            }
        }

    [Test]
    public void ShouldHaveSameStreamContentAs_Should_Pass__Given_the_same_content()
    {
            using (var left = new MemoryStream(Encoding.UTF8.GetBytes("Hello there")))
            using (var right = new MemoryStream(Encoding.UTF8.GetBytes("Hello there")))
            {
                left.ShouldEqualByStreamContent(right);
                left.ShouldHaveSameStreamContentAs(right);
            }
        }
}