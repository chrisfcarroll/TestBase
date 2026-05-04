namespace LogAssert.Tests;

[TestFixture]
public class StateNameIfHelpfulTests
{
    [Test]
    public void ReturnsEmpty_WhenNull()
    {
        Assert.That(LogAssertions.StateLabelIfHelpful("action", null), Is.EqualTo(""));
    }

    [Test]
    public void ReturnsEmpty_WhenEmpty()
    {
        Assert.That(LogAssertions.StateLabelIfHelpful("action", ""), Is.EqualTo(""));
    }

    [Test]
    public void ReturnsEmpty_WhenSameAsAction()
    {
        Assert.That(LogAssertions.StateLabelIfHelpful("DoStuff", "DoStuff"), Is.EqualTo(""));
    }

    [Test]
    public void ReturnsEmpty_WhenEndsWithToLoggableState()
    {
        Assert.That(LogAssertions.StateLabelIfHelpful("action", "obj.ToLoggableState()"), Is.EqualTo(""));
    }

    [Test]
    public void ReturnsNameWithColon_WhenHelpful()
    {
        Assert.That(LogAssertions.StateLabelIfHelpful("action", "userId"), Is.EqualTo("userId:"));
    }
}
