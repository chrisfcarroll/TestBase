namespace Serilog.Assert.Tests;

[TestFixture]
public class StateLabelIfHelpfulTests
{
    [Test]
    public void ReturnsEmpty_WhenNull()
    {
        NUnit.Framework.Assert.That(Serilog.Assert.LogAssert.StateLabelIfHelpful("action", null), Is.EqualTo(""));
    }

    [Test]
    public void ReturnsEmpty_WhenEmpty()
    {
        NUnit.Framework.Assert.That(Serilog.Assert.LogAssert.StateLabelIfHelpful("action", ""), Is.EqualTo(""));
    }

    [Test]
    public void ReturnsEmpty_WhenSameAsAction()
    {
        NUnit.Framework.Assert.That(Serilog.Assert.LogAssert.StateLabelIfHelpful("DoStuff", "DoStuff"), Is.EqualTo(""));
    }

    [Test]
    public void ReturnsEmpty_WhenEndsWithToLoggableState()
    {
        NUnit.Framework.Assert.That(Serilog.Assert.LogAssert.StateLabelIfHelpful("action", "obj.ToLoggableState()"), Is.EqualTo(""));
    }

    [Test]
    public void ReturnsNameWithColon_WhenHelpful()
    {
        NUnit.Framework.Assert.That(Serilog.Assert.LogAssert.StateLabelIfHelpful("action", "userId"), Is.EqualTo("userId="));
    }

    [Test]
    public void ReturnsEmpty_WhenHelpfulInformationIsStringMatchingTrimmedLabel()
    {
        NUnit.Framework.Assert.That(Serilog.Assert.LogAssert.StateLabelIfHelpful("action", "\"Comment\"", "Comment"), Is.EqualTo(""));
    }

    [Test]
    public void ReturnsLabel_WhenHelpfulInformationIsStringNotMatchingLabel()
    {
        NUnit.Framework.Assert.That(Serilog.Assert.LogAssert.StateLabelIfHelpful("action", "\"myLabel\"", "different"), Is.EqualTo("myLabel="));
    }
}
