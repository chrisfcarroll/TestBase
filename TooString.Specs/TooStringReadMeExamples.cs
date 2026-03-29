using System.Numerics;
using System.Text.Json;

namespace TooString.Specs;

[TestFixture]
public class TooStringReadMeExamples
{
    [Test]
    public void ExampleIsCorrectGivenCallerArgument()
    {

        var actual = (Math.Sqrt(4 * Math.PI / 3)).ToArgumentExpression();
        // Output is the literal code: "Math.Sqrt(4 * Math.PI / 3)"

        Assert.That(actual, Is.EqualTo("Math.Sqrt(4 * Math.PI / 3)"));
    }

    [Test]
    public void ExampleIsCorrectGivenAnonObjectJson()
    {
        // var anonObject = new { A = "boo", B = new Complex(3,4) };
        // anonObject.ToJson();
        // anonObject.TooString(TooStringStyle.Json);
        // Output is the System.Text.Json output:
        // {"A":"boo","B":{"Real":3,"Imaginary":4,"Magnitude":5,"Phase":0.9272952180016122}}

        var anonObject = new { A = "boo", B = new Complex(3,4) };
        var actualJson1 = anonObject.ToJson();
        var actualJson2 = anonObject.TooString(TooStringStyle.JsonSerializer);
        Assert.That(actualJson1, Is.EqualTo(
                """
                {"A":"boo","B":{"Real":3,"Imaginary":4,"Magnitude":5,"Phase":0.9272952180016122}}
                """));
        Assert.That(actualJson2, Is.EqualTo(actualJson1));
    }
    [Test]
    public void ExampleIsCorrectGivenAnonObjectReflection()
    {
        var anonObject = new { A = "boo", B = new Complex(3,4) };
        // Output is "{ A = boo, B = [3,4] }" or "{ A = boo, B = <3;4> }"
        var actual = anonObject.TooString(TooStringStyle.CSharp);
        var expected = $"{{ A = \"boo\", B = {new Complex(3,4)} }}";
        Assert.That(actual, Is.EqualTo(expected));
    }

    [Test]
    public void ExampleIsCorrectGivenValueTupleJson()
    {
        var valueTuple = (one: 1, two: "2", three: new Complex(3,4));
        var actual = valueTuple.TooString(TooStringStyle.JsonStringifier);
        Assert.That(actual, Is.EqualTo("""[1,"2",[3,4]]"""));;

        var stjOutput = System.Text.Json.JsonSerializer.Serialize(valueTuple);
        // Output is "{}" because there are no public fields
        Assert.That(stjOutput, Is.EqualTo("{}"));
    }

    [Test]
    public void ExampleIsCorrectGivenValueTupleReflection()
    {
        var valueTuple = (one: 1, two: "2", three: new Complex(3,4));

        var actual3 = valueTuple.TooString(TooStringStyle.CSharp);
        #if NET8_0_OR_GREATER
        Assert.That(actual3, Is.EqualTo("(1, \"2\", <3; 4>)"));
        #else
        Assert.That(actual3, Is.EqualTo("(1, \"2\", (3, 4))"));
        #endif

        var options = TooStringOptions.Default with
        {
            JsonOptions = new JsonSerializerOptions { IncludeFields = true }
        };
        var actualJsonWithFields = valueTuple .TooString(options);
        Assert.That(actualJsonWithFields,Is.EqualTo(
        """
                {"Item1":1,"Item2":"2","Item3":{"Real":3,"Imaginary":4,"Magnitude":5,"Phase":0.9272952180016122}}
                """));
    }
}