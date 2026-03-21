using NUnit.Framework;

namespace TestBase.DifferTests;

[TestFixture]
public class DifferFloatTests
{
    [Test]
    public void Equal_doubles_within_default_tolerance()
    {
        var result = Differ.Diff(1.0, 1.0 + 1e-15);
        Assert.That(result.AreEqual, Is.True);
    }

    [Test]
    public void Different_doubles_outside_default_tolerance()
    {
        var result = Differ.Diff(1.0, 1.1);
        Assert.That(result.AreEqual, Is.False);
    }

    [Test]
    public void Custom_tolerance_allows_larger_difference()
    {
        var result = Differ.Diff(1.0, 1.05, new DiffOptions { FloatTolerance = 0.1 });
        Assert.That(result.AreEqual, Is.True);
    }

    [Test]
    public void Custom_tolerance_rejects_too_large_difference()
    {
        var result = Differ.Diff(1.0, 1.2, new DiffOptions { FloatTolerance = 0.1 });
        Assert.That(result.AreEqual, Is.False);
        Assert.That(result.ToString(), Does.Contain("tolerance"));
    }

    [Test]
    public void Equal_floats()
    {
        var result = Differ.Diff(1.0f, 1.0f);
        Assert.That(result.AreEqual, Is.True);
    }

    [Test]
    public void Different_floats()
    {
        var result = Differ.Diff(1.0f, 2.0f);
        Assert.That(result.AreEqual, Is.False);
    }

    [Test]
    public void Equal_decimals()
    {
        var result = Differ.Diff(1.0m, 1.0m);
        Assert.That(result.AreEqual, Is.True);
    }

    [Test]
    public void Different_decimals()
    {
        var result = Differ.Diff(1.0m, 2.0m);
        Assert.That(result.AreEqual, Is.False);
    }
}
