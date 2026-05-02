namespace SerilogAssert.Tests;

[TestFixture]
public class StateNameIfHelpfulTests
{
    [Test]
    public void ReturnsEmpty_WhenNull()
    {
        Assert.That(SerilogAssertions.StateNameIfHelpful("action", null), Is.EqualTo(""));
    }

    [Test]
    public void ReturnsEmpty_WhenEmpty()
    {
        Assert.That(SerilogAssertions.StateNameIfHelpful("action", ""), Is.EqualTo(""));
    }

    [Test]
    public void ReturnsEmpty_WhenSameAsAction()
    {
        Assert.That(SerilogAssertions.StateNameIfHelpful("DoStuff", "DoStuff"), Is.EqualTo(""));
    }

    [Test]
    public void ReturnsEmpty_WhenEndsWithToLoggableState()
    {
        Assert.That(SerilogAssertions.StateNameIfHelpful("action", "obj.ToLoggableState()"), Is.EqualTo(""));
    }

    [Test]
    public void ReturnsNameWithColon_WhenHelpful()
    {
        Assert.That(SerilogAssertions.StateNameIfHelpful("action", "userId"), Is.EqualTo("userId:"));
    }
}
