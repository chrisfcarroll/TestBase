using NUnit.Framework;

namespace TestBase.Tests;

[TestFixture]
public class KnownTestRunnerAssertionsTests
{
    [Test]
    public void Create_ShouldReturnNUnitAssertionException()
    {
        var assertion = new Assertion("test message");

        var result = KnownTestRunnerAssertions.Create(assertion);

        result.ShouldBeOfType<NUnit.Framework.AssertionException>();
        result.Message.ShouldContain("test message");
        result.InnerException.ShouldBe(assertion);
    }

    [Test]
    public void ForActiveTestRunner_ShouldReturnNUnitAssertionException()
    {
        var assertion = new Assertion("fluent test");

        var result = assertion.ForActiveTestRunner();

        result.ShouldBeOfType<NUnit.Framework.AssertionException>();
        result.Message.ShouldContain("fluent test");
        result.InnerException.ShouldBe(assertion);
    }

    [Test]
    public void Throw_ShouldThrowNUnitAssertionException()
    {
        var assertion = new Assertion("thrown message");

        var ex = Assert.Throws<NUnit.Framework.AssertionException>(
            () => KnownTestRunnerAssertions.Throw(assertion));

        ex.Message.ShouldContain("thrown message");
        ex.InnerException.ShouldBe(assertion);
    }
}
