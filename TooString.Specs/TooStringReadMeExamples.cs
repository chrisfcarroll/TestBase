using System.Numerics;
using System.Text.Json;

namespace TooString.Specs;

[TestFixture]
public class TooStringReadMeExamples
{
    [Test]
    public void ExampleIsCorrectGivenCallerArgument()
    {

        var actual = (Math.Sqrt(4 * Math.PI / 3)).TooString(TooStringHow.CallerArgument);
        // Output is the literal code: "Math.Sqrt(4 * Math.PI / 3)"

        Assert.That(actual, Is.EqualTo("Math.Sqrt(4 * Math.PI / 3)"));
    }

    [Test]
    public void ExampleIsCorrectGivenJson()
    {

        var actual = new { A = "boo", B = new Complex(3,4) }.TooString(TooStringHow.Json);
        // Output is the System.Text.Json output:
        // {"A":"boo","B":{"Real":3,"Imaginary":4,"Magnitude":5,"Phase":0.9272952180016122}}
        Assert.That(actual, Is.EqualTo(
                """
                {"A":"boo","B":{"Real":3,"Imaginary":4,"Magnitude":5,"Phase":0.9272952180016122}}
                """));;
    }
    [Test]
    public void ExampleIsCorrectGivenReflection()
    {

        var actual =
            new { A = "boo", B = new Complex(3,4) }.TooString(TooStringHow.Reflection);
        // Output is "{ A = boo, B = (3, 4) }"
        var expected = $"{{ A = boo, B = {new Complex(3,4)} }}";
        Assert.That(actual, Is.EqualTo(expected));
    }

    [Test]
    public void ValueTupleExampleIsCorrect()
    {
        var valueTuple = (one: 1,two: "2");

        var actual = valueTuple.TooString(TooStringHow.Json);
        var actual2= System.Text.Json.JsonSerializer.Serialize(valueTuple);
        var actual3 = valueTuple.ToDebugViewString();
        // Output is "{}" because there are no public fields
        Assert.That(actual, Is.EqualTo("{}"));
        Assert.That(actual2, Is.EqualTo("{}"));
        Assert.That(actual3, Is.EqualTo("(1, 2)"));

        // do this instead:
        var options = TooStringOptions.Default with
        {
            JsonOptions = new JsonSerializerOptions { IncludeFields = true }
        };
        var actualJsonWithFields = valueTuple .TooString(options);
        // Output is {"Item1":1,""Item2":"2"}"
        Assert.That(actualJsonWithFields, Is.EqualTo("{\"Item1\":1,\"Item2\":\"2\"}"));

    }
}