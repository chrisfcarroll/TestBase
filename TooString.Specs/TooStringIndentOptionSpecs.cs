using System.Text.Json;
using System.Text.Json.Serialization;
using NUnit.Framework.Interfaces;

namespace TooString.Specs;

[TestFixture]
public class TooStringIndentOptionSpecs
{
    static Circular depth1;
    static Circular depth4;
    static readonly JsonSerializerOptions stjOptionsForIndentedNoCycles = new JsonSerializerOptions(){WriteIndented = true, ReferenceHandler = ReferenceHandler.IgnoreCycles};

    static TooStringIndentOptionSpecs()
    {
        depth1 = new Circular() { A = "One" };

        depth4 = new ()
        {
            A = "1",
            B = new()
            {
                A = "2",
                B = new()
                {
                    A = "3",
                    C = "3"
                },
                C = "2"
            },
            C = "1"
        };
    }

    [Test]
    public void WriteIndented_IsIndependentOfJsonOptions()
    {
        var options = TooStringOptions.Default;
        Assert.That(options.WriteIndented, Is.True);

        options = options with { WriteIndented = false };
        Assert.That(options.WriteIndented, Is.False);

        options = options with { WriteIndented = true };
        Assert.That(options.WriteIndented, Is.True);
    }

    [Test]
    public void NoIndentOutputIsSingleLine()
    {
        Assert.That(depth4.ToJson(writeIndented: false).IndexOf('\n'), Is.EqualTo(-1));
        Assert.That(depth4.ToCSharpString(writeIndented: false).IndexOf('\n'), Is.EqualTo(-1));
        Assert.That(depth1.ToJson(writeIndented: false).IndexOf('\n'), Is.EqualTo(-1));
        Assert.That(depth1.ToCSharpString(writeIndented: false).IndexOf('\n'), Is.EqualTo(-1));
    }

    [Test]
    public void IndentOutputIsMultiLineForSTJOutput()
    {
        // ToSTJson uses System.Text.Json
        var actual1 = depth4.ToSTJson(stjOptionsForIndentedNoCycles);
        var expected1 = System.Text.Json.JsonSerializer.Serialize(depth4,stjOptionsForIndentedNoCycles);
        Assert.That(actual1, Is.EqualTo(expected1));

        // ToJson uses JsonStringifier, so its indented output differs from STJ
        var actual2 = depth4.ToJson(writeIndented: true);
        Assert.That(actual2, Does.Contain("\n"));
        Assert.That(actual2, Does.Contain("\"A\""));
    }

    [Test]
    public void IndentedCSharpString_HasCorrectIndentation()
    {
        var actual = depth4.ToCSharpString(writeIndented: true);
        TestContext.Out.WriteLine(actual);

        var expected = string.Join(Environment.NewLine,
            "/*Circular*/ new {",
            "  A = \"1\",",
            "  B = /*Circular*/ new {",
            "    A = \"2\",",
            "    B = /*Circular*/ new {",
            "      A = \"3\",",
            "      B = null,",
            "      C = \"3\"",
            "    },",
            "    C = \"2\"",
            "  },",
            "  C = \"1\"",
            "}");
        Assert.That(actual, Is.EqualTo(expected));
    }

    [Test]
    public void IndentedDebugView_HasCorrectIndentation()
    {
        var actual = depth4.TooString(TooStringOptions.Default with { StringifyAs = StringifyAs.DebugView });
        TestContext.Out.WriteLine(actual);

        var expected = string.Join(Environment.NewLine,
            "{",
            "  A = 1,",
            "  B = {",
            "    A = 2,",
            "    B = {",
            "      A = 3,",
            "      B = null,",
            "      C = 3",
            "    },",
            "    C = 2",
            "  },",
            "  C = 1",
            "}");
        Assert.That(actual, Is.EqualTo(expected));
    }

    [Test]
    public void IndentedJsonStringifier_HasCorrectIndentation()
    {
        var actual = depth4.ToJson(writeIndented: true);
        TestContext.Out.WriteLine(actual);

        var expected = string.Join(Environment.NewLine,
            "{",
            "  \"A\": \"1\",",
            "  \"B\": {",
            "    \"A\": \"2\",",
            "    \"B\": {",
            "      \"A\": \"3\",",
            "      \"B\": null,",
            "      \"C\": \"3\"",
            "    },",
            "    \"C\": \"2\"",
            "  },",
            "  \"C\": \"1\"",
            "}");
        Assert.That(actual, Is.EqualTo(expected));
    }

    [Test]
    public void IndentOutputIsMultiLineForReflectedJsonOutput()
    {
        var expected1 = System.Text.Json.JsonSerializer.Serialize(depth4,stjOptionsForIndentedNoCycles);

        var actual = depth4.TooString(options: TooStringOptions.ForJson with
        {
            WriteIndented = true,
            JsonOptions = TooStringOptions.DefaultJsonSerializerOptions.With(js =>
            {
                js.MaxDepth = 99;
            }),
        });

        //D
        TestContext.Out.WriteLine(expected1);
        TestContext.Out.WriteLine(actual);

        //A
        Assert.That(actual, Is.EqualTo(expected1));

        // var actual2 = circular.TooString(new AdvancedOptions()
        // {
        //     Style = TooStringStyle.Json
        // });
        // Assert.That(actual2, Is.EqualTo(expected1));
    }
}