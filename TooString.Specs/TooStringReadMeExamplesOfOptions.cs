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
        var webIndented2 = ObjectTooString.ToJson(circular,this.webIndented);
        Assert.That(webIndented1, Is.EqualTo(webIndented2));
    }

    [Test]
    public void ValueTupleToJsonExample()
    {
        var toJson = (one:1, two:"2").TooString( TooStringStyle.JsonSerializer );
        var stj = System.Text.Json.JsonSerializer.Serialize((one: 1,two: "2"));
        var reflected = (one:1, two:"2").TooString( AdvancedOptions.ForJson);

        Assert.That(toJson, Is.EqualTo("""[1,"2"]"""));
        Assert.That(stj, Is.EqualTo("{}"));
        Assert.That(reflected, Is.EqualTo( """[1,"2"]""" ));
    }

    [Test]
    public void ToCSharpStringExamples()
    {
        var value = circular;
        var d1= value.ToStringified();
        var d2 = value.TooString(AdvancedOptions.ForCSharp);
        var d3 = value.TooString(maxDepth: 4,maxLength: 9,style: TooStringStyle.CSharp);
        var d4= value.TooString(AdvancedOptions.ForCSharp with
                        {
                            DateTimeFormat = "yyyyMMdd HH:mm:ss",
                            TimeSpanFormat = @"d\.hh\:mm\:ss",
                        });
        var d5 = value.TooString(TooStringOptions.WithAdvancedOptions(new(Style:TooStringStyle.CSharp)));
        Assert.That(d1, Is.EqualTo(d2));
        Assert.That(d1, Is.EqualTo(d3));
        Assert.That(d1, Is.EqualTo(d4));
        Assert.That(d1, Is.EqualTo(d5));
    }
}