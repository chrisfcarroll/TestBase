

namespace Log.Assert.Tests;

[TestFixture]
public class StateNameIfHelpfulTests
{
    [Test]
    public void ReturnsEmpty_WhenNull()
    {
        NUnit.Framework.Assert.That(LogAssert.StateLabelIfHelpful("action", null), Is.EqualTo(""));
    }

    [Test]
    public void ReturnsEmpty_WhenEmpty()
    {
        NUnit.Framework.Assert.That(LogAssert.StateLabelIfHelpful("action", ""), Is.EqualTo(""));
    }

    [Test]
    public void ReturnsEmpty_WhenSameAsAction()
    {
        NUnit.Framework.Assert.That(LogAssert.StateLabelIfHelpful("DoStuff", "DoStuff"), Is.EqualTo(""));
    }

    [Test]
    public void ReturnsEmpty_WhenEndsWithToLoggableState()
    {
        NUnit.Framework.Assert.That(LogAssert.StateLabelIfHelpful("action", "obj.ToLoggableState()"), Is.EqualTo(""));
    }

    [Test]
    public void ReturnsNameWithColon_WhenHelpful()
    {
        NUnit.Framework.Assert.That(LogAssert.StateLabelIfHelpful("action", "userId"), Is.EqualTo("userId="));
    }

    [Test]
    public void ReturnsEmpty_WhenHelpfulInformationIsStringMatchingTrimmedLabel()
    {
        NUnit.Framework.Assert.That(LogAssert.StateLabelIfHelpful("action", "\"Comment\"", "Comment"), Is.EqualTo(""));
    }

    [Test]
    public void ReturnsLabel_WhenHelpfulInformationIsStringNotMatchingLabel()
    {
        NUnit.Framework.Assert.That(LogAssert.StateLabelIfHelpful("action", "\"myLabel\"", "different"), Is.EqualTo("myLabel="));
    }
}
