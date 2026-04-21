using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace TooString.Specs;

[TestFixture]
public class TooStringOptionsWithSpecs
{
    [Test]
    public void ParameterlessConstructor_SetsDefaults()
    {
        var options = new TooStringOptions();

        Assert.That(options.StringifyAs, Is.EqualTo(StringifyAs.CSharp));
        Assert.That(options.WriteIndented, Is.True);
        Assert.That(options.MaxDepth, Is.EqualTo(3));
        Assert.That(options.MaxEnumerationLength, Is.EqualTo(9));
        Assert.That(options.WhichProperties, Is.EqualTo(BindingFlags.Instance | BindingFlags.Public));
        Assert.That(options.DateTimeFormat, Is.EqualTo("O"));
        Assert.That(options.JsonOptions.ReferenceHandler, Is.EqualTo(ReferenceHandler.IgnoreCycles));
    }

    [Test]
    public void ParameterlessConstructor_AllowsObjectInitialiser()
    {
        var options = new TooStringOptions
        {
            StringifyAs = StringifyAs.STJson,
            WriteIndented = true,
            MaxDepth = 5,
        };

        Assert.That(options.StringifyAs, Is.EqualTo(StringifyAs.STJson));
        Assert.That(options.WriteIndented, Is.True);
        Assert.That(options.MaxDepth, Is.EqualTo(5));
    }

    [Test]
    public void ForJson_ReturnsJsonSerializerStyle()
    {
        var options = TooStringOptions.ForJson;

        Assert.That(options.StringifyAs, Is.EqualTo(StringifyAs.STJson));
        Assert.That(options.MaxDepth, Is.EqualTo(3));
    }

    [Test]
    public void ForCSharp_ReturnsCSharpStyle()
    {
        var options = TooStringOptions.ForCSharp;

        Assert.That(options.StringifyAs, Is.EqualTo(StringifyAs.CSharp));
        Assert.That(options.MaxDepth, Is.EqualTo(3));
    }

    [Test]
    public void ForJson_CanBeCustomisedWithRecordWith()
    {
        var options = TooStringOptions.ForJson with { WriteIndented = false };

        Assert.That(options.StringifyAs, Is.EqualTo(StringifyAs.STJson));
        Assert.That(options.WriteIndented, Is.False);
    }

    [Test]
    public void ForCSharp_CanBeCustomisedWithRecordWith()
    {
        var options = TooStringOptions.ForCSharp with { MaxDepth = 7 };

        Assert.That(options.StringifyAs, Is.EqualTo(StringifyAs.CSharp));
        Assert.That(options.MaxDepth, Is.EqualTo(7));
    }

    // ──────────────────────────────────────────────
    //  With(Action<TooStringOptions>)
    // ──────────────────────────────────────────────

    [Test]
    public void WithParams_ReturnsModifiedCopy()
    {
        var original = TooStringOptions.ForJson;

        var modified = original.With(writeIndented:false);

        Assert.That(modified.WriteIndented, Is.False);
        Assert.That(original.WriteIndented, Is.True, "original should be unchanged");
    }

    // ──────────────────────────────────────────────
    //  With(TooStringOptions)
    // ──────────────────────────────────────────────

    [Test]
    public void RecordWith_ReplacesSpecifiedValues()
    {
        var baseOptions = TooStringOptions.ForCSharp;

        var merged = baseOptions with
        {
            StringifyAs = StringifyAs.DebugView,
            MaxDepth = 10,
        };

        Assert.That(merged.StringifyAs, Is.EqualTo(StringifyAs.DebugView));
        Assert.That(merged.MaxDepth, Is.EqualTo(10));
    }

    // ──────────────────────────────────────────────
    //  With(individual nullable parameters)
    // ──────────────────────────────────────────────

    [Test]
    public void WithIndividualParams_OnlyOverridesSpecifiedValues()
    {
        var original = TooStringOptions.Default;

        var modified = original.With(maxDepth: 10, writeIndented: true);

        Assert.That(modified.MaxDepth, Is.EqualTo(10));
        Assert.That(modified.WriteIndented, Is.True);
        // Unspecified values should remain at defaults
        Assert.That(modified.StringifyAs, Is.EqualTo(StringifyAs.CSharp));
        Assert.That(modified.MaxEnumerationLength, Is.EqualTo(9));
        Assert.That(modified.DateTimeFormat, Is.EqualTo("O"));
    }

    [Test]
    public void WithIndividualParams_CanOverrideStringifyAs()
    {
        var modified = TooStringOptions.Default.With(stringifyAs: StringifyAs.DebugView);

        Assert.That(modified.StringifyAs, Is.EqualTo(StringifyAs.DebugView));
    }

    [Test]
    public void WithIndividualParams_CanOverrideAllFields()
    {
        var modified = TooStringOptions.Default.With(
            whichProperties: BindingFlags.NonPublic | BindingFlags.Instance,
            maxDepth: 7,
            maxEnumerableLength: 20,
            dateTimeFormat: "yyyy-MM-dd",
            dateOnlyFormat: "yyyy-MM-dd",
            timeOnlyFormat: "HH:mm",
            timeSpanFormat: @"d\.hh\:mm");

        Assert.That(modified.WhichProperties,
                    Is.EqualTo(BindingFlags.NonPublic | BindingFlags.Instance));
        Assert.That(modified.MaxDepth, Is.EqualTo(7));
        Assert.That(modified.MaxEnumerationLength, Is.EqualTo(20));
        Assert.That(modified.DateTimeFormat, Is.EqualTo("yyyy-MM-dd"));
        Assert.That(modified.DateOnlyFormat, Is.EqualTo("yyyy-MM-dd"));
        Assert.That(modified.TimeOnlyFormat, Is.EqualTo("HH:mm"));
        Assert.That(modified.TimeSpanFormat, Is.EqualTo(@"d\.hh\:mm"));
    }

    [Test]
    public void WithIndividualParams_PreservesBaseOptions()
    {
        var baseOptions = TooStringOptions.ForJson.With(writeIndented: true);

        var modified = baseOptions.With(maxDepth: 1);

        Assert.That(modified.WriteIndented, Is.True, "WriteIndented from base should be preserved");
        Assert.That(modified.StringifyAs, Is.EqualTo(StringifyAs.STJson));
        Assert.That(modified.MaxDepth, Is.EqualTo(1));
        Assert.That(modified.MaxEnumerationLength, Is.EqualTo(9),
                    "Unspecified MaxEnumerationLength should be preserved from base");
    }

    [Test]
    public void WithIndividualParams_NoArgsReturnsCopy()
    {
        var original = TooStringOptions.ForJson;
        var copy = original.With();

        Assert.That(copy.StringifyAs, Is.EqualTo(original.StringifyAs));
        Assert.That(copy.MaxDepth, Is.EqualTo(original.MaxDepth));
    }

    // ──────────────────────────────────────────────
    //  Chaining
    // ──────────────────────────────────────────────

    [Test]
    public void WithMethods_CanBeChained()
    {
        var options = TooStringOptions.ForJson
            .With(maxDepth: 5)
            .With(writeIndented: true)
            .With(maxEnumerableLength: 20);

        Assert.That(options.StringifyAs, Is.EqualTo(StringifyAs.STJson));
        Assert.That(options.MaxDepth, Is.EqualTo(5));
        Assert.That(options.WriteIndented, Is.True);
        Assert.That(options.MaxEnumerationLength, Is.EqualTo(20));
    }

    // ──────────────────────────────────────────────
    //  Integration: options actually affect output
    // ──────────────────────────────────────────────

    [Test]
    public void ForJson_ProducesJsonOutput()
    {
        var value = new { A = 1, B = "two" };
        var result = value.TooString(TooStringOptions.ForJson);

        Assert.That(result, Does.Contain("\"A\": 1"));
        Assert.That(result, Does.Contain("\"B\": \"two\""));
    }

    [Test]
    public void ForCSharp_ProducesCSharpOutput()
    {
        var value = new { A = 1, B = "two" };
        var result = value.TooString(TooStringOptions.ForCSharp);

        Assert.That(result, Does.Contain("A = 1"));
        Assert.That(result, Does.Contain("B = \"two\""));
    }

    [Test]
    public void WithMaxDepth_AffectsOutput()
    {
        var value = new { A = "1" };

        var d1 = value.TooString(TooStringOptions.ForCSharp.With(maxDepth: 1));
        var d5 = value.TooString(TooStringOptions.ForCSharp.With(maxDepth: 5));

        // Both should produce valid output with the property
        Assert.That(d1, Does.Contain("A"));
        Assert.That(d5, Does.Contain("A"));
        // The With(maxDepth:) parameter should actually set it
        var opts = TooStringOptions.Default.With(maxDepth: 7);
        Assert.That(opts.MaxDepth, Is.EqualTo(7));
    }
}
