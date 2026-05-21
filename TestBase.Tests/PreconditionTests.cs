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
    public void InconclusiveIf_ShouldReturnActual_WhenPredicateFails()
    {
        var result = Precondition.InconclusiveIf(42, x => x < 0);

        result.ShouldBe(42);
    }

    [Test]
    public void InconclusiveIf_ShouldThrowInconclusive_WhenPredicateHolds()
    {
        Assert.Throws<InconclusiveException>(
            () => Precondition.InconclusiveIf(0, x => x == 0));
    }

    [Test]
    public void InconclusiveIf_True_ShouldThrowInconclusive()
    {
        Assert.Throws<InconclusiveException>(
            ()=> Precondition.InconclusiveIf(true));

    }

    [Test]
    public void InconclusiveIf_False_Should_Return_False()
    {
        Precondition.InconclusiveIf(false).ShouldBeFalse();
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
    public void FailIf_ShouldReturnActual_WhenPredicateFails()
    {
        Precondition.FailIf(42, x => x < 0).ShouldBe(42);
    }

    [Test]
    public void FailIf_ShouldThrowAssertionFailure_WhenPredicateHolds()
    {
        Assert.Throws<AssertionException>(
            () => Precondition.FailIf(0, x => x == 0));
    }

    [Test]
    public void FailIf_Bool_ShouldReturnResult_WhenFalse()
    {
        Precondition.FailIf(false).ShouldBeFalse();
    }

    [Test]
    public void FailIf_Bool_ShouldThrowAssertionFailure_WhenTrue()
    {
        Assert.Throws<AssertionException>(() => Precondition.FailIf(true));
    }
}
