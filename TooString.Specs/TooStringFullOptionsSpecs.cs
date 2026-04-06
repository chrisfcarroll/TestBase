using System.Reflection;

namespace TooString.Specs;

[TestFixture]
public class TooStringFullOptionsSpecs
{
    [Test]
    public void MaxDepth_IsRespected()
    {
        var value = new Circular
        {
            A = "1",
            B = new Circular { A = "2", B = new Circular { A = "3" } }
        };

        var shallow = value.TooString(maxDepth: 1, style: StringifyAs.CSharp);
        var deep = value.TooString(maxDepth: 5, style: StringifyAs.CSharp);

        Assert.That(shallow.Length, Is.LessThan(deep.Length));
    }

    [Test]
    public void MaxLength_IsRespected()
    {
        var value = Enumerable.Range(1, 20).ToArray();

        var limited = value.TooString(maxDepth: 2, maxLength: 3, style: StringifyAs.CSharp);
        var full = value.TooString(maxDepth: 2, maxLength: 20, style: StringifyAs.CSharp);

        Assert.That(limited.Length, Is.LessThan(full.Length));
    }

    [Test]
    public void WriteIndented_IsRespected()
    {
        var value = new { A = 1, B = "two" };

        var compact = value.TooString(maxDepth: 3, writeIndented: false);
        var indented = value.TooString(maxDepth: 3, writeIndented: true);

        Assert.That(compact.IndexOf('\n'), Is.EqualTo(-1));
        Assert.That(indented, Does.Contain("\n"));
    }

    [Test]
    public void Style_IsRespected()
    {
        var value = new { A = 1 };

        var csharp = value.TooString(maxDepth: 3, style: StringifyAs.CSharp);
        var json = value.TooString(maxDepth: 3, style: StringifyAs.JsonStringifier);
        var debug = value.TooString(maxDepth: 3, style: StringifyAs.DebugView);

        Assert.That(csharp, Does.Contain("A = 1"));
        Assert.That(json, Does.Contain("\"A\":1"));
        Assert.That(debug, Does.Contain("A = 1"));
    }

    [Test]
    public void WhichProperties_NonPublic()
    {
        var value = new WithPrivate();

        var publicOnly = value.TooString(maxDepth: 3,
            whichProperties: BindingFlags.Instance | BindingFlags.Public);
        var withNonPublic = value.TooString(maxDepth: 3,
            whichProperties: BindingFlags.Instance | BindingFlags.NonPublic);

        Assert.That(publicOnly, Does.Contain("Pub = "));
        Assert.That(publicOnly, Does.Not.Contain("Priv = "));
        Assert.That(withNonPublic, Does.Not.Contain("Pub = "));
        Assert.That(withNonPublic, Does.Contain("Priv = "));
    }

    [Test]
    public void DateTimeFormat_IsRespected()
    {
        var dt = new DateTime(2025, 6, 15, 10, 30, 0, DateTimeKind.Utc);
        var value = new { When = dt };

        var iso = value.TooString(maxDepth: 3, dateTimeFormat: "O");
        var custom = value.TooString(maxDepth: 3, dateTimeFormat: "yyyy-MM-dd");

        Assert.That(iso, Does.Contain("2025-06-15T10:30:00"));
        Assert.That(custom, Does.Contain("\"2025-06-15\""));
    }

    [Test]
    public void TimeSpanFormat_IsRespected()
    {
        var value = new { Duration = TimeSpan.FromHours(2.5) };

        var c = value.TooString(maxDepth: 3, timeSpanFormat: "c");
        var custom = value.TooString(maxDepth: 3, timeSpanFormat: @"h\:mm");

        Assert.That(c, Does.Contain("02:30:00"));
        Assert.That(custom, Does.Contain("\"2:30\""));
    }

    [Test]
    public void OptionsBuiltCorrectly_CombinedTest()
    {
        var value = new { A = 1 };

        var result = value.TooString(
            maxDepth: 5,
            maxLength: 20,
            style: StringifyAs.JsonStringifier,
            writeIndented: true,
            dateTimeFormat: "yyyy-MM-dd");

        Assert.That(result, Does.Contain("\"A\""));
        Assert.That(result, Does.Contain("\n"));
    }

    class WithPrivate
    {
        public string Pub { get; set; } = "pub";
        private string Priv { get; set; } = "priv";
    }
}
