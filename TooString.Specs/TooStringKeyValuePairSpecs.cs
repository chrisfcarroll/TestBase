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
    public void ScalarKey_CSharp_OutputsPropertyStyle()
    {
        var kvp = new KeyValuePair<string, int>("age", 30);
        var result = kvp.ToCSharpString(writeIndented: false);
        TestContext.Out.WriteLine(result);
        Assert.That(result, Is.EqualTo("""new /*KeyValuePair*/ { Key = "age", Value = 30 }"""));
    }

    [Test]
    public void ScalarKey_DebugView_OutputsIndexerNotation()
    {
        var kvp = new KeyValuePair<string, int>("age", 30);
        var result = kvp.TooString(TooStringOptions.Default with
            { WriteIndented = false, StringifyAs = StringifyAs.DebugView });
        TestContext.Out.WriteLine(result);
        Assert.That(result, Is.EqualTo("[age] = 30"));
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
    public void IntKey_CSharp_OutputsPropertyStyle()
    {
        var kvp = new KeyValuePair<int, string>(42, "answer");
        var result = kvp.ToCSharpString(writeIndented: false);
        TestContext.Out.WriteLine(result);
        Assert.That(result, Is.EqualTo("""new /*KeyValuePair*/ { Key = 42, Value = "answer" }"""));
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
        Assert.That(result, Is.EqualTo("""new /*KeyValuePair*/ { Key = "home", Value = new /*Address*/ { Street = "123 Main", City = "Springfield" } }"""));
    }

    [Test]
    public void ScalarKey_CSharp_StaysOneLineEvenWhenIndented()
    {
        var kvp = new KeyValuePair<string, Address>("home", new Address("123 Main", "Springfield"));
        var result = kvp.ToCSharpString(writeIndented: true);
        TestContext.Out.WriteLine(result);
        Assert.That(result, Does.Not.Contain(Environment.NewLine));
        Assert.That(result, Is.EqualTo("""new /*KeyValuePair*/ { Key = "home", Value = new /*Address*/ { Street = "123 Main", City = "Springfield" } }"""));
    }

    [Test]
    public void ComplexKey_FallsBackToPropertyStyleForCSharp()
    {
        var kvp = new KeyValuePair<Address, int>(new Address("123 Main", "Springfield"), 1);
        var result = kvp.ToCSharpString(writeIndented: false);
        TestContext.Out.WriteLine(result);
        Assert.That(result, Does.Contain("Key"));
        Assert.That(result, Does.Contain("Value"));
        Assert.That(result, Is.EqualTo("new /*KeyValuePair*/ { Key = new /*Address*/ { Street = \"123 Main\", City = \"Springfield\" }, Value = 1 }"));
    }
    [Test]
    public void ComplexKey_FallsBackToPropertyStyleForJson()
    {
        var kvp = new KeyValuePair<Address, int>(new Address("123 Main", "Springfield"), 1);
        var result = kvp.ToJson(writeIndented: false);
        TestContext.Out.WriteLine(result);
        Assert.That(result, Does.Contain("Key"));
        Assert.That(result, Does.Contain("Value"));
    }
}
