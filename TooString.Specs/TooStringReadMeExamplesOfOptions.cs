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
        var toJson = circular.ToJson();
        var webIndented1 = circular.ToJson(this.webIndented);
        var webIndented2 = circular.TooString(this.webIndented);
        Assert.That(webIndented1, Is.EqualTo(webIndented2));
    }

    [Test]
    public void ValueTupleToJsonExample()
    {
        var toJson = (one:1, two:"2").TooString( TooStringHow.Json );
        var stj = System.Text.Json.JsonSerializer.Serialize((one: 1,two: "2"));
        var reflected = (one:1, two:"2").TooString( ReflectionOptions.ForJson);

        Assert.That(toJson, Is.EqualTo("""[1,"2"]"""));
        Assert.That(stj, Is.EqualTo("{}"));
        Assert.That(reflected, Is.EqualTo( """[1,"2"]""" ));
    }

    [Test]
    public void ToDebugViewStringExamples()
    {
        var value = circular;
        var d1= value.ToDebugViewString();
        var d2 = value.TooString(ReflectionOptions.ForDebugView);
        var d3 = value.TooString(maxDepth: 4,maxLength: 9,style: ReflectionStyle.DebugView);
        var d4= value.TooString(ReflectionOptions.ForDebugView with
                        {
                            DateTimeFormat = "yyyyMMdd HH:mm:ss",
                            TimeSpanFormat = @"d\.hh\:mm\:ss",
                        });
        var d5 = value.TooString(TooStringOptions.Default with
        {
            Fallbacks = [TooStringHow.Reflection]
        });
        Assert.That(d1, Is.EqualTo(d2));
        Assert.That(d1, Is.EqualTo(d3));
        Assert.That(d1, Is.EqualTo(d4));
        Assert.That(d1, Is.EqualTo(d5));
    }
}