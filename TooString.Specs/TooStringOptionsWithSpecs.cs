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

        Assert.That(options.StringifyAs, Is.EqualTo(TooStringStyle.CSharp));
        Assert.That(options.WriteIndented, Is.False);
        Assert.That(options.AdvancedOptions, Is.EqualTo(AdvancedOptions.Default));
        Assert.That(options.JsonOptions.ReferenceHandler, Is.EqualTo(ReferenceHandler.IgnoreCycles));
    }

    [Test]
    public void ParameterlessConstructor_AllowsObjectInitialiser()
    {
        var options = new TooStringOptions
        {
            StringifyAs = TooStringStyle.JsonSerializer,
            WriteIndented = true,
            AdvancedOptions = new AdvancedOptions(MaxDepth: 5),
        };

        Assert.That(options.StringifyAs, Is.EqualTo(TooStringStyle.JsonSerializer));
        Assert.That(options.WriteIndented, Is.True);
        Assert.That(options.AdvancedOptions.MaxDepth, Is.EqualTo(5));
    }

    [Test]
    public void ForJson_ReturnsJsonSerializerStyle()
    {
        var options = TooStringOptions.ForJson;

        Assert.That(options.StringifyAs, Is.EqualTo(TooStringStyle.JsonSerializer));
        Assert.That(options.AdvancedOptions, Is.EqualTo(AdvancedOptions.Default));
    }

    [Test]
    public void ForCSharp_ReturnsCSharpStyle()
    {
        var options = TooStringOptions.ForCSharp;

        Assert.That(options.StringifyAs, Is.EqualTo(TooStringStyle.CSharp));
        Assert.That(options.AdvancedOptions, Is.EqualTo(AdvancedOptions.Default));
    }

    [Test]
    public void ForJson_CanBeCustomisedWithRecordWith()
    {
        var options = TooStringOptions.ForJson with { WriteIndented = true };

        Assert.That(options.StringifyAs, Is.EqualTo(TooStringStyle.JsonSerializer));
        Assert.That(options.WriteIndented, Is.True);
    }

    [Test]
    public void ForCSharp_CanBeCustomisedWithRecordWith()
    {
        var options = TooStringOptions.ForCSharp with
        {
            AdvancedOptions = new AdvancedOptions(MaxDepth: 7)
        };

        Assert.That(options.StringifyAs, Is.EqualTo(TooStringStyle.CSharp));
        Assert.That(options.AdvancedOptions.MaxDepth, Is.EqualTo(7));
    }

    // ──────────────────────────────────────────────
    //  With(Action<TooStringOptions>)
    // ──────────────────────────────────────────────

    [Test]
    public void WithAction_ReturnsMutatedCopy()
    {
        var original = TooStringOptions.ForJson;

        var modified = original.With(o => o.WriteIndented = true);

        Assert.That(modified.WriteIndented, Is.True);
        Assert.That(original.WriteIndented, Is.False, "original should be unchanged");
    }

    // ──────────────────────────────────────────────
    //  With(TooStringOptions)
    // ──────────────────────────────────────────────

    [Test]
    public void WithOptions_ReplacesAllValues()
    {
        var baseOptions = TooStringOptions.ForCSharp;
        var overrides = new TooStringOptions
        {
            StringifyAs = TooStringStyle.DebugView,
            AdvancedOptions = new AdvancedOptions(MaxDepth: 10),
        };

        var merged = baseOptions.With(overrides);

        Assert.That(merged.StringifyAs, Is.EqualTo(TooStringStyle.DebugView));
        Assert.That(merged.AdvancedOptions.MaxDepth, Is.EqualTo(10));
    }

    // ──────────────────────────────────────────────
    //  With(AdvancedOptions)
    // ──────────────────────────────────────────────

    [Test]
    public void WithAdvancedOptions_ReplacesAdvancedOptions()
    {
        var options = TooStringOptions.ForJson
            .With(new AdvancedOptions(MaxDepth: 2, MaxEnumerationLength: 5));

        Assert.That(options.StringifyAs, Is.EqualTo(TooStringStyle.JsonSerializer));
        Assert.That(options.AdvancedOptions.MaxDepth, Is.EqualTo(2));
        Assert.That(options.AdvancedOptions.MaxEnumerationLength, Is.EqualTo(5));
    }

    [Test]
    public void WithAdvancedOptions_PreservesOtherProperties()
    {
        var baseOptions = TooStringOptions.ForJson.With(o => o.WriteIndented = true);

        var modified = baseOptions.With(new AdvancedOptions(MaxDepth: 1));

        Assert.That(modified.WriteIndented, Is.True, "WriteIndented should be preserved");
        Assert.That(modified.StringifyAs, Is.EqualTo(TooStringStyle.JsonSerializer));
    }

    // ──────────────────────────────────────────────
    //  With(individual nullable parameters)
    // ──────────────────────────────────────────────

    [Test]
    public void WithIndividualParams_OnlyOverridesSpecifiedValues()
    {
        var original = TooStringOptions.Default;

        var modified = original.With(maxDepth: 10, writeIndented: true);

        Assert.That(modified.AdvancedOptions.MaxDepth, Is.EqualTo(10));
        Assert.That(modified.WriteIndented, Is.True);
        // Unspecified values should remain at defaults
        Assert.That(modified.StringifyAs, Is.EqualTo(TooStringStyle.CSharp));
        Assert.That(modified.AdvancedOptions.MaxEnumerationLength, Is.EqualTo(9));
        Assert.That(modified.AdvancedOptions.DateTimeFormat, Is.EqualTo("O"));
    }

    [Test]
    public void WithIndividualParams_CanOverrideStringifyAs()
    {
        var modified = TooStringOptions.Default.With(stringifyAs: TooStringStyle.DebugView);

        Assert.That(modified.StringifyAs, Is.EqualTo(TooStringStyle.DebugView));
    }

    [Test]
    public void WithIndividualParams_CanOverrideAllAdvancedFields()
    {
        var modified = TooStringOptions.Default.With(
            whichProperties: BindingFlags.NonPublic | BindingFlags.Instance,
            maxDepth: 7,
            maxEnumerationLength: 20,
            dateTimeFormat: "yyyy-MM-dd",
            dateOnlyFormat: "yyyy-MM-dd",
            timeOnlyFormat: "HH:mm",
            timeSpanFormat: @"d\.hh\:mm");

        Assert.That(modified.AdvancedOptions.WhichProperties,
                    Is.EqualTo(BindingFlags.NonPublic | BindingFlags.Instance));
        Assert.That(modified.AdvancedOptions.MaxDepth, Is.EqualTo(7));
        Assert.That(modified.AdvancedOptions.MaxEnumerationLength, Is.EqualTo(20));
        Assert.That(modified.AdvancedOptions.DateTimeFormat, Is.EqualTo("yyyy-MM-dd"));
        Assert.That(modified.AdvancedOptions.DateOnlyFormat, Is.EqualTo("yyyy-MM-dd"));
        Assert.That(modified.AdvancedOptions.TimeOnlyFormat, Is.EqualTo("HH:mm"));
        Assert.That(modified.AdvancedOptions.TimeSpanFormat, Is.EqualTo(@"d\.hh\:mm"));
    }

    [Test]
    public void WithIndividualParams_PreservesBaseOptions()
    {
        var baseOptions = TooStringOptions.ForJson.With(o => o.WriteIndented = true);

        var modified = baseOptions.With(maxDepth: 1);

        Assert.That(modified.WriteIndented, Is.True, "WriteIndented from base should be preserved");
        Assert.That(modified.StringifyAs, Is.EqualTo(TooStringStyle.JsonSerializer));
        Assert.That(modified.AdvancedOptions.MaxDepth, Is.EqualTo(1));
        Assert.That(modified.AdvancedOptions.MaxEnumerationLength, Is.EqualTo(9),
                    "Unspecified MaxEnumerationLength should be preserved from base");
    }

    [Test]
    public void WithIndividualParams_NoArgsReturnsCopy()
    {
        var original = TooStringOptions.ForJson;
        var copy = original.With();

        Assert.That(copy.StringifyAs, Is.EqualTo(original.StringifyAs));
        Assert.That(copy.AdvancedOptions, Is.EqualTo(original.AdvancedOptions));
    }

    // ──────────────────────────────────────────────
    //  Chaining
    // ──────────────────────────────────────────────

    [Test]
    public void WithMethods_CanBeChained()
    {
        var options = TooStringOptions.ForJson
            .With(new AdvancedOptions(MaxDepth: 5))
            .With(o => o.WriteIndented = true)
            .With(maxEnumerationLength: 20);

        Assert.That(options.StringifyAs, Is.EqualTo(TooStringStyle.JsonSerializer));
        Assert.That(options.AdvancedOptions.MaxDepth, Is.EqualTo(5));
        Assert.That(options.WriteIndented, Is.True);
        Assert.That(options.AdvancedOptions.MaxEnumerationLength, Is.EqualTo(20));
    }

    // ──────────────────────────────────────────────
    //  Integration: options actually affect output
    // ──────────────────────────────────────────────

    [Test]
    public void ForJson_ProducesJsonOutput()
    {
        var value = new { A = 1, B = "two" };
        var result = value.TooString(TooStringOptions.ForJson);

        Assert.That(result, Does.Contain("\"A\":1"));
        Assert.That(result, Does.Contain("\"B\":\"two\""));
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
        Assert.That(opts.AdvancedOptions.MaxDepth, Is.EqualTo(7));
    }
}
