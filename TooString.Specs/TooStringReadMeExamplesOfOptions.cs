using System.Text.Json;

namespace TooString.Specs;

[TestFixture]
public class TooStringReadMeExamplesOfOptions
{
    Circular circular = new() { A = "1" ,B = new Circular() { A = "2" } };
    JsonSerializerOptions webIndented = new(JsonSerializerDefaults.Web) { WriteIndented = true };

    [Test]
    public void ToJsonExamples()
    {
        // ToJson() uses JsonStringifier (reflection-based), now indented by default
        var toJson = circular.ToJson();
        Assert.That(toJson, Does.Contain("\"A\": \"1\""));

        // ToSTJson() uses System.Text.Json
        var stJson1 = circular.ToSTJson(this.webIndented);
        var stJson2 = ObjectTooString.ToSTJson(circular,this.webIndented);
        Assert.That(stJson1, Is.EqualTo(stJson2));
    }

    [Test]
    public void ValueTupleToJsonExample()
    {
        var toJson = (one:1, two:"2").ToJson(writeIndented: false);
        var stj = System.Text.Json.JsonSerializer.Serialize((one: 1,two: "2"));
        var reflected = (one:1, two:"2").ToJson(writeIndented: false);

        Assert.That(toJson, Is.EqualTo("""[1,"2"]"""));
        Assert.That(stj, Is.EqualTo("{}"));
        Assert.That(reflected, Is.EqualTo( """[1,"2"]""" ));
    }

    [Test]
    public void ToCSharpStringExamples()
    {
        var value = circular;
        var d1 = value.ToCSharpString();
        var d2 = value.TooString(StringifyAs.CSharp);
        var d3 = value.ToCSharpString(maxDepth: 4, maxEnumerableLength: 9);
        var d4 = value.TooString(options: TooStringOptions.Default with
        {
            WriteIndented = true,
            DateTimeFormat = "yyyyMMdd HH:mm:ss",
            TimeSpanFormat = @"d\.hh\:mm\:ss",
        });
        Assert.That(d1, Is.EqualTo(d2));
        Assert.That(d1, Is.EqualTo(d3));
        Assert.That(d1, Is.EqualTo(d4));
    }
}
