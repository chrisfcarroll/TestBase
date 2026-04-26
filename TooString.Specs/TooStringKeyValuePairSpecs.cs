namespace TooString.Specs;

[TestFixture]
public class TooStringKeyValuePairSpecs
{
    record Address(string Street, string City);

    [Test]
    public void ScalarKey_Json_OutputsAsJsonProperty()
    {
        var kvp = new KeyValuePair<string, int>("age", 30);
        var result = kvp.ToJson(writeIndented: false);
        TestContext.Out.WriteLine(result);
        Assert.That(result, Is.EqualTo("""{"age":30}"""));
    }

    [Test]
    public void ScalarKey_CSharp_OutputsIndexerNotation()
    {
        var kvp = new KeyValuePair<string, int>("age", 30);
        var result = kvp.ToCSharpString(writeIndented: false);
        TestContext.Out.WriteLine(result);
        Assert.That(result, Is.EqualTo("""{ ["age"] = 30 }"""));
    }

    [Test]
    public void ScalarKey_DebugView_OutputsIndexerNotation()
    {
        var kvp = new KeyValuePair<string, int>("age", 30);
        var result = kvp.TooString(TooStringOptions.Default with
            { WriteIndented = false, StringifyAs = StringifyAs.DebugView });
        TestContext.Out.WriteLine(result);
        Assert.That(result, Is.EqualTo("{ [age] = 30 }"));
    }

    [Test]
    public void IntKey_Json_OutputsAsJsonProperty()
    {
        var kvp = new KeyValuePair<int, string>(42, "answer");
        var result = kvp.ToJson(writeIndented: false);
        TestContext.Out.WriteLine(result);
        Assert.That(result, Is.EqualTo("""{"42":"answer"}"""));
    }

    [Test]
    public void IntKey_CSharp_OutputsIndexerNotation()
    {
        var kvp = new KeyValuePair<int, string>(42, "answer");
        var result = kvp.ToCSharpString(writeIndented: false);
        TestContext.Out.WriteLine(result);
        Assert.That(result, Is.EqualTo("""{ [42] = "answer" }"""));
    }

    [Test]
    public void ScalarKey_ComplexValue_Json()
    {
        var kvp = new KeyValuePair<string, Address>("home", new Address("123 Main", "Springfield"));
        var result = kvp.ToJson(writeIndented: false);
        TestContext.Out.WriteLine(result);
        Assert.That(result, Is.EqualTo("""{"home":{"Street":"123 Main","City":"Springfield"}}"""));
    }

    [Test]
    public void ScalarKey_ComplexValue_CSharp()
    {
        var kvp = new KeyValuePair<string, Address>("home", new Address("123 Main", "Springfield"));
        var result = kvp.ToCSharpString(writeIndented: false);
        TestContext.Out.WriteLine(result);
        Assert.That(result, Is.EqualTo("""{ ["home"] = new /*Address*/ { Street = "123 Main", City = "Springfield" } }"""));
    }

    [Test]
    public void ComplexKey_FallsBackToPropertyStyle()
    {
        var kvp = new KeyValuePair<Address, int>(new Address("123 Main", "Springfield"), 1);
        var result = kvp.ToCSharpString(writeIndented: false);
        TestContext.Out.WriteLine(result);
        Assert.That(result, Does.Contain("Key"));
        Assert.That(result, Does.Contain("Value"));
    }
}
