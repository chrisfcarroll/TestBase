namespace SerilogAssert.Tests;

[TestFixture]
public class StateLabelIfHelpfulTests
{
    [Test]
    public void ReturnsEmpty_WhenNull()
    {
        Assert.That(SerilogAssertions.StateLabelIfHelpful("action", null), Is.EqualTo(""));
    }

    [Test]
    public void ReturnsEmpty_WhenEmpty()
    {
        Assert.That(SerilogAssertions.StateLabelIfHelpful("action", ""), Is.EqualTo(""));
    }

    [Test]
    public void ReturnsEmpty_WhenSameAsAction()
    {
        Assert.That(SerilogAssertions.StateLabelIfHelpful("DoStuff", "DoStuff"), Is.EqualTo(""));
    }

    [Test]
    public void ReturnsEmpty_WhenEndsWithToLoggableState()
    {
        Assert.That(SerilogAssertions.StateLabelIfHelpful("action", "obj.ToLoggableState()"), Is.EqualTo(""));
    }

    [Test]
    public void ReturnsNameWithColon_WhenHelpful()
    {
        Assert.That(SerilogAssertions.StateLabelIfHelpful("action", "userId"), Is.EqualTo("userId:"));
    }
}
