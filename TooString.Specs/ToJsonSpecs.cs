using System.Numerics;
using System.Reflection;

namespace TooString.Specs;

[TestFixture]
public class ToJsonSpecs
{
    [Test]
    public void ToJson_UsesJsonStringifier_NotSTJ()
    {
        // Complex via STJ would give {"Real":3,...}, JsonStringifier gives [3,4]
        var value = new CompositeA { A = "boo", B = new Complex(3, 4) };

        var result = value.ToJson(writeIndented: false);

        Assert.That(result, Is.EqualTo("""{"A":"boo","B":[3,4]}"""));
    }

    [Test]
    public void ToJson_DefaultIsIndented()
    {
        var value = new { A = 1, B = "two" };

        var result = value.ToJson();

        Assert.That(result, Does.Contain("\n"));
        Assert.That(result, Does.Contain("\"A\": 1"));
    }

    [Test]
    public void ToJson_WriteIndented()
    {
        var value = new { A = 1 };

        var result = value.ToJson(writeIndented: true);

        Assert.That(result, Does.Contain("\n"));
        Assert.That(result, Does.Contain("\"A\""));
    }

    [Test]
    public void ToJson_MaxDepth_LimitsNesting()
    {
        var value = new Circular
        {
            A = "1",
            B = new Circular
            {
                A = "2",
                B = new Circular { A = "3" }
            }
        };

        var shallow = value.ToJson(maxDepth: 1);
        var deep = value.ToJson(maxDepth: 5);

        // Shallow should show less nesting detail
        Assert.That(shallow.Length, Is.LessThan(deep.Length));
    }

    [Test]
    public void ToJson_MaxEnumerationLength_LimitsArrayElements()
    {
        var value = new { Items = Enumerable.Range(1, 100).ToArray() };

        var limited = value.ToJson(maxEnumerationLength: 3);
        var full = value.ToJson(maxEnumerationLength: 100);

        Assert.That(limited.Length, Is.LessThan(full.Length));
    }

    [Test]
    public void ToJson_WithMaxDepth_UsesJsonStringifier()
    {
        var value = new CompositeA { A = "test", B = new Complex(1, 2) };

        var result = value.ToJson(writeIndented: false, maxDepth: 5);

        // Should be JSON, not CSharp style
        Assert.That(result, Does.Contain("\"A\":\"test\""));
        Assert.That(result, Does.Not.Contain("/*"));
    }

    [Test]
    public void ToJson_DateTimeFormat_IsRespected()
    {
        var dt = new DateTime(2025, 6, 15, 10, 30, 0, DateTimeKind.Utc);
        var value = new { When = dt };

        var iso = value.ToJson(dateTimeFormat: "O");
        var custom = value.ToJson(dateTimeFormat: "yyyy-MM-dd");

        Assert.That(iso, Does.Contain("2025-06-15T10:30:00"));
        Assert.That(custom, Does.Contain("\"2025-06-15\""));
        Assert.That(custom, Does.Not.Contain("T10:30"));
    }

    [Test]
    public void ToJson_WhichProperties_NonPublic()
    {
        var value = new HasPrivate();

        var publicOnly = value.ToJson(
            whichProperties: BindingFlags.Instance | BindingFlags.Public);
        var withNonPublic = value.ToJson(
            whichProperties: BindingFlags.Instance | BindingFlags.NonPublic);

        Assert.That(publicOnly, Does.Contain("\"Pub\""));
        Assert.That(publicOnly, Does.Not.Contain("\"Priv\""));
        Assert.That(withNonPublic, Does.Contain("\"Priv\""));
    }

    class HasPrivate
    {
        public string Pub { get; set; } = "pub";
        private string Priv { get; set; } = "priv";
    }
}
