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

        var ex = NUnit.Framework.Assert.Throws<NUnit.Framework.AssertionException>(
            () => KnownTestRunnerAssertions.Throw(assertion));

        ex.Message.ShouldContain("thrown message");
        ex.InnerException.ShouldBe(assertion);
    }

    [Test]
    public void CreateSkip_ShouldReturnNUnitInconclusiveException()
    {
        var result = KnownTestRunnerAssertions.CreateSkip("skip reason");

        result.ShouldBeOfType<NUnit.Framework.InconclusiveException>();
        result.Message.ShouldContain("skip reason");
    }

    [Test]
    public void ThrowSkip_ShouldThrowNUnitInconclusiveException()
    {
        var ex = NUnit.Framework.Assert.Throws<NUnit.Framework.InconclusiveException>(
            () => KnownTestRunnerAssertions.ThrowSkip("skip this"));

        ex.Message.ShouldContain("skip this");
    }

    [Test]
    public void SkipBecause_ShouldThrowNUnitInconclusiveException()
    {
        var ex = NUnit.Framework.Assert.Throws<NUnit.Framework.InconclusiveException>(
            () => Skip.Because("not ready yet"));

        ex.Message.ShouldContain("not ready yet");
    }

    [Test]
    public void SkipBecause_ShouldFormatMessageArgs()
    {
        var ex = NUnit.Framework.Assert.Throws<NUnit.Framework.InconclusiveException>(
            () => Skip.Because("missing {0}", "dependency"));

        ex.Message.ShouldContain("missing dependency");
    }

    [Test]
    public void SkipIfPreconditionFails_ShouldReturnActual_WhenPreconditionHolds()
    {
        var result = Skip.IfPreconditionFails(42, x => x > 0);

        result.ShouldBe(42);
    }

    [Test]
    public void SkipIfPreconditionFails_ShouldThrowInconclusive_WhenPreconditionFails()
    {
        NUnit.Framework.Assert.Throws<NUnit.Framework.InconclusiveException>(
            () => Skip.IfPreconditionFails(0, x => x > 0));
    }

    [Test]
    public void SkipIf_ShouldReturnActual_WhenPreconditionHolds()
    {
        var result = Skip.If(42, x => x > 0);

        result.ShouldBe(42);
    }

    [Test]
    public void SkipIf_ShouldThrowInconclusive_WhenPreconditionFails()
    {
        NUnit.Framework.Assert.Throws<NUnit.Framework.InconclusiveException>(
            () => Skip.If(0, x => x > 0));
    }
}
