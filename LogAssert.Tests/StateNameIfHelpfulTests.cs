namespace LogAssert.Tests;

[TestFixture]
public class StateNameIfHelpfulTests
{
    [Test]
    public void ReturnsEmpty_WhenNull()
    {
        Assert.That(LogAssertions.StateNameIfHelpful("action", null), Is.EqualTo(""));
    }

    [Test]
    public void ReturnsEmpty_WhenEmpty()
    {
        Assert.That(LogAssertions.StateNameIfHelpful("action", ""), Is.EqualTo(""));
    }

    [Test]
    public void ReturnsEmpty_WhenSameAsAction()
    {
        Assert.That(LogAssertions.StateNameIfHelpful("DoStuff", "DoStuff"), Is.EqualTo(""));
    }

    [Test]
    public void ReturnsEmpty_WhenEndsWithToLoggableState()
    {
        Assert.That(LogAssertions.StateNameIfHelpful("action", "obj.ToLoggableState()"), Is.EqualTo(""));
    }

    [Test]
    public void ReturnsNameWithColon_WhenHelpful()
    {
        Assert.That(LogAssertions.StateNameIfHelpful("action", "userId"), Is.EqualTo("userId:"));
    }
}
