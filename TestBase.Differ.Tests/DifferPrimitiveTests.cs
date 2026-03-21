using NUnit.Framework;

namespace TestBase.DifferTests;

[TestFixture]
public class DifferPrimitiveTests
{
    [Test] public void Equal_ints() => Assert.That(Differ.Diff(42, 42).AreEqual, Is.True);
    [Test] public void Different_ints() => Assert.That(Differ.Diff(42, 43).AreEqual, Is.False);
    [Test] public void Equal_bools() => Assert.That(Differ.Diff(true, true).AreEqual, Is.True);
    [Test] public void Different_bools() => Assert.That(Differ.Diff(true, false).AreEqual, Is.False);
    [Test] public void Equal_chars() => Assert.That(Differ.Diff('a', 'a').AreEqual, Is.True);
    [Test] public void Different_chars() => Assert.That(Differ.Diff('a', 'b').AreEqual, Is.False);
    [Test] public void Equal_longs() => Assert.That(Differ.Diff(42L, 42L).AreEqual, Is.True);
    [Test] public void Different_longs() => Assert.That(Differ.Diff(42L, 43L).AreEqual, Is.False);
    [Test] public void Equal_bytes() => Assert.That(Differ.Diff((byte)1, (byte)1).AreEqual, Is.True);
    [Test] public void Different_bytes() => Assert.That(Differ.Diff((byte)1, (byte)2).AreEqual, Is.False);
    [Test] public void Equal_guids()
    {
        var g = Guid.NewGuid();
        Assert.That(Differ.Diff(g, g).AreEqual, Is.True);
    }
    [Test] public void Different_guids() => Assert.That(Differ.Diff(Guid.NewGuid(), Guid.NewGuid()).AreEqual, Is.False);
    [Test] public void Equal_datetimes()
    {
        var d = DateTime.Now;
        Assert.That(Differ.Diff(d, d).AreEqual, Is.True);
    }
    [Test] public void Different_datetimes() => Assert.That(Differ.Diff(DateTime.MinValue, DateTime.MaxValue).AreEqual, Is.False);

    [Test]
    public void Equal_enums() => Assert.That(Differ.Diff(DayOfWeek.Monday, DayOfWeek.Monday).AreEqual, Is.True);

    [Test]
    public void Different_enums()
    {
        var result = Differ.Diff(DayOfWeek.Monday, DayOfWeek.Friday);
        Assert.That(result.AreEqual, Is.False);
        var text = result.ToString();
        Assert.That(text, Does.Contain("Monday"));
        Assert.That(text, Does.Contain("Friday"));
    }

    [Test]
    public void Different_ints_show_values()
    {
        var result = Differ.Diff(42, 99);
        var text = result.ToString();
        Assert.That(text, Does.Contain("42"));
        Assert.That(text, Does.Contain("99"));
    }
}
