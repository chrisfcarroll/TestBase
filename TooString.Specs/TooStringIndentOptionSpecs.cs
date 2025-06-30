using System.Text.Json;
using System.Text.Json.Serialization;

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
                }
            },
        };
    }

    [Test]
    public void NoIndentOutputIsSingleLine()
    {
        Assert.That(depth4.ToJson(writeIndented: false).IndexOf('\n'), Is.EqualTo(-1));
        Assert.That(depth4.ToDebugViewString().IndexOf('\n'), Is.EqualTo(-1));
        Assert.That(depth1.ToJson(writeIndented: false).IndexOf('\n'), Is.EqualTo(-1));
        Assert.That(depth1.ToDebugViewString().IndexOf('\n'), Is.EqualTo(-1));
    }

    [Test]
    public void IndentOutputIsMultiLineForSTJOutput()
    {
        var actual1 = depth4.ToJson(writeIndented: true);
        var expected1 = System.Text.Json.JsonSerializer.Serialize(depth4,stjOptionsForIndentedNoCycles);
        Assert.That(actual1, Is.EqualTo(expected1));

        var actual2 = depth4.TooString(stjOptionsForIndentedNoCycles);
        Assert.That(actual2, Is.EqualTo(expected1));
    }

    [Test]
    public void IndentOutputIsMultiLineForReflectedJsonOutput()
    {
        var expected1 = System.Text.Json.JsonSerializer.Serialize(depth4,stjOptionsForIndentedNoCycles);

        var actual = depth4.TooString(
            TooStringHow.Reflection,
            TooStringOptions.ForJson(o=>
            {
                o.WriteIndented = true;
                o.MaxDepth = 99;
            }));

        Assert.That(actual, Is.EqualTo(expected1));

        // var actual2 = circular.TooString(new ReflectionOptions()
        // {
        //     Style = ReflectionStyle.Json
        // });
        // Assert.That(actual2, Is.EqualTo(expected1));
    }
}