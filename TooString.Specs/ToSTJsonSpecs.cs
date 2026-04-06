using System.Numerics;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace TooString.Specs;

[TestFixture]
public class ToSTJsonSpecs
{
    [Test]
    public void WithJsonSerializerOptions_UsesProvidedOptions()
    {
        var value = new { A = 1, B = "two" };
        var options = new JsonSerializerOptions { WriteIndented = false };

        var result = value.ToSTJson(options);

        Assert.That(result, Is.EqualTo("{\"A\":1,\"B\":\"two\"}"));
    }

    [Test]
    public void WithJsonSerializerOptions_WriteIndented()
    {
        var value = new { A = 1 };
        var options = new JsonSerializerOptions { WriteIndented = true };

        var result = value.ToSTJson(options);

        Assert.That(result, Does.Contain("\n"));
        Assert.That(result, Does.Contain("\"A\": 1"));
    }

    [Test]
    public void WithJsonSerializerOptions_CamelCase()
    {
        var value = new { MyProp = 1 };
        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        var result = value.ToSTJson(options);

        Assert.That(result, Does.Contain("\"myProp\""));
        Assert.That(result, Does.Not.Contain("\"MyProp\""));
    }

    [Test]
    public void WithIndividualParams_DefaultsProduceCompactJson()
    {
        var value = new { A = 1, B = "two" };

        var result = value.ToSTJson();

        Assert.That(result, Is.EqualTo("{\"A\":1,\"B\":\"two\"}"));
    }

    [Test]
    public void WithIndividualParams_WriteIndented()
    {
        var value = new { A = 1 };

        var result = value.ToSTJson(writeIndented: true);

        Assert.That(result, Does.Contain("\n"));
        Assert.That(result, Does.Contain("\"A\": 1"));
    }

    [Test]
    public void WithIndividualParams_CamelCaseNaming()
    {
        var value = new { MyProp = 1 };

        var result = value.ToSTJson(propertyNamingPolicy: JsonNamingPolicy.CamelCase);

        Assert.That(result, Does.Contain("\"myProp\""));
    }

    [Test]
    public void WithIndividualParams_IgnoreNulls()
    {
        var value = new CompositeA { A = "hello", B = null };

        var result = value.ToSTJson(
            defaultIgnoreCondition: JsonIgnoreCondition.WhenWritingNull);

        Assert.That(result, Does.Not.Contain("\"B\""));
        Assert.That(result, Does.Contain("\"A\":\"hello\""));
    }

    [Test]
    public void WithIndividualParams_IncludeFields()
    {
        var value = (one: 1, two: "2");

        var resultWithFields = value.ToSTJson(includeFields: true);
        var resultWithoutFields = value.ToSTJson(includeFields: false);

        Assert.That(resultWithFields, Does.Contain("1"));
        Assert.That(resultWithoutFields, Is.EqualTo("{}"));
    }

    [Test]
    public void WithIndividualParams_ReferenceHandlerIgnoreCycles()
    {
        var a = new SelfRef { Name = "root" };
        a.Self = a;

        var result = a.ToSTJson(referenceHandler: ReferenceHandler.IgnoreCycles);

        Assert.That(result, Does.Contain("\"Name\":\"root\""));
        Assert.That(result, Does.Contain("null")); // cycle becomes null
    }

    [Test]
    public void OverloadWithOptions_ReturnsSameAsJsonSerializer()
    {
        var value = new { X = 42, Y = "hello" };
        var options = new JsonSerializerOptions
        {
            WriteIndented = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        };

        var toSTJsonResult = value.ToSTJson(options);
        var directResult = JsonSerializer.Serialize(value, options);

        Assert.That(toSTJsonResult, Is.EqualTo(directResult));
    }

    [Test]
    public void IndividualParams_MatchesEquivalentOptions()
    {
        var value = new { X = 42, Y = "hello" };

        var fromParams = value.ToSTJson(
            writeIndented: true,
            propertyNamingPolicy: JsonNamingPolicy.CamelCase);

        var fromOptions = value.ToSTJson(new JsonSerializerOptions
        {
            WriteIndented = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        });

        Assert.That(fromParams, Is.EqualTo(fromOptions));
    }

    class SelfRef
    {
        public string? Name { get; set; }
        public SelfRef? Self { get; set; }
    }
}
