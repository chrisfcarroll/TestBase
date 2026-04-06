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
    public void TooStringOptionsWriteIndentedMatchesJsonOptionsWriteIdented()
    {
        var options = TooStringOptions.Default;
        Assert.That(options.WriteIndented, Is.False);
        Assert.That(options.JsonOptions.WriteIndented, Is.False);

        options = options with { WriteIndented = true };
        Assert.That(options.WriteIndented, Is.True);
        Assert.That(options.JsonOptions.WriteIndented, Is.True);

        options = options with {WriteIndented = false};
        Assert.That(options.WriteIndented, Is.False);
        Assert.That(options.JsonOptions.WriteIndented, Is.False);
    }

    [Test]
    public void NoIndentOutputIsSingleLine()
    {
        Assert.That(depth4.ToJson(writeIndented: false).IndexOf('\n'), Is.EqualTo(-1));
        Assert.That(depth4.TooString().IndexOf('\n'), Is.EqualTo(-1));
        Assert.That(depth1.ToJson(writeIndented: false).IndexOf('\n'), Is.EqualTo(-1));
        Assert.That(depth1.TooString().IndexOf('\n'), Is.EqualTo(-1));
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
    public void IndentOutputIsMultiLineForReflectedJsonOutput()
    {
        var expected1 = System.Text.Json.JsonSerializer.Serialize(depth4,stjOptionsForIndentedNoCycles);

        var actual = depth4.TooString(options: TooStringOptions.ForJson with
        {
            JsonOptions = TooStringOptions.DefaultJsonSerializerOptions.With(js =>
            {
                js.MaxDepth = 99;
                js.WriteIndented = true;
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