using NUnit.Framework;

namespace TestBase.Tests;

[TestFixture]
public class PreconditionTests
{
    [Test]
    public void Inconclusive_ShouldThrowInconclusiveException()
    {
        var ex = Assert.Throws<InconclusiveException>(
            () => Precondition.InconclusiveBecause("not ready"));

        ex.Message.ShouldContain("not ready");
    }

    [Test]
    public void Inconclusive_ShouldFormatMessageArgs()
    {
        var ex = Assert.Throws<InconclusiveException>(
            () => Precondition.InconclusiveBecause("missing {0}", "dependency"));

        ex.Message.ShouldContain("missing dependency");
    }

    [Test]
    public void InconclusiveIf_ShouldReturnActual_WhenPredicateHolds()
    {
        var result = Precondition.InconclusiveIf(42, x => x > 0);

        result.ShouldBe(42);
    }

    [Test]
    public void InconclusiveIf_ShouldThrowInconclusive_WhenPredicateFails()
    {
        Assert.Throws<InconclusiveException>(
            () => Precondition.InconclusiveIf(0, x => x > 0));
    }

    [Test]
    public void InconclusiveIf_BoolWithString_ShouldReturnResult_WhenTrue()
    {
        var result = Precondition.InconclusiveIf(new BoolWithString(true, "ok"));

        ((bool)result).ShouldBe(true);
    }

    [Test]
    public void InconclusiveIf_BoolWithString_ShouldThrowInconclusive_WhenFalse()
    {
        Assert.Throws<InconclusiveException>(
            () => Precondition.InconclusiveIf(new BoolWithString(false, "nope")));
    }

    [Test]
    public void Fail_ShouldThrowAssertionException()
    {
        var ex = Assert.Throws<AssertionException>(
            () => Precondition.Failed("bad state"));

        ex.Message.ShouldContain("bad state");
    }

    [Test]
    public void Fail_ShouldFormatMessageArgs()
    {
        var ex = Assert.Throws<AssertionException>(
            () => Precondition.Failed("expected {0} but got {1}", "a", "b"));

        ex.Message.ShouldContain("expected a but got b");
    }

    [Test]
    public void FailIf_ShouldReturnActual_WhenPredicateHolds()
    {
        var result = Precondition.FailIf(42, x => x > 0);

        result.ShouldBe(42);
    }

    [Test]
    public void FailIf_ShouldThrowAssertionFailure_WhenPredicateFails()
    {
        Assert.Throws<AssertionException>(
            () => Precondition.FailIf(0, x => x > 0));
    }

    [Test]
    public void FailIf_BoolWithString_ShouldReturnResult_WhenTrue()
    {
        var result = Precondition.FailIf(new BoolWithString(true, "ok"));

        ((bool)result).ShouldBe(true);
    }

    [Test]
    public void FailIf_BoolWithString_ShouldThrowAssertionFailure_WhenFalse()
    {
        Assert.Throws<AssertionException>(
            () => Precondition.FailIf(new BoolWithString(false, "nope")));
    }
}
