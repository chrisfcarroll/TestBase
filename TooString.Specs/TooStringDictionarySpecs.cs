using System.Collections;
using System.Collections.Specialized;

namespace TooString.Specs;

[TestFixture]
public class TooStringDictionarySpecs
{
    [Test]
    public void DictionaryToJson_OutputsObjectNotArray()
    {
        var dict = new Dictionary<string, int> { ["a"] = 1, ["b"] = 2 };
        var actual = dict.ToJson(writeIndented: false);
        Assert.That(actual, Is.EqualTo("""{"a":1,"b":2}"""));
    }

    [Test]
    public void DictionaryToCSharp_OutputsIndexerNotation()
    {
        var dict = new Dictionary<string, int> { ["a"] = 1, ["b"] = 2 };
        var actual = dict.ToCSharpString(writeIndented: false);
        Assert.That(actual, Is.EqualTo("""{ ["a"] = 1, ["b"] = 2 }"""));
    }

    [Test]
    public void DictionaryToDebugView_OutputsIndexerNotation()
    {
        var dict = new Dictionary<string, int> { ["a"] = 1, ["b"] = 2 };
        var actual = dict.TooString(TooStringOptions.Default with
            { WriteIndented = false, StringifyAs = StringifyAs.DebugView });
        Assert.That(actual, Is.EqualTo("{ [a] = 1, [b] = 2 }"));
    }

    [Test]
    public void DictionaryWithComplexValues_ToJson()
    {
        var dict = new Dictionary<string, object>
        {
            ["name"] = "Alice",
            ["age"] = 30
        };
        var actual = dict.ToJson(writeIndented: false);
        Assert.That(actual, Is.EqualTo("""{"name":"Alice","age":30}"""));
    }

    [Test]
    public void DictionaryWithIntKeys_ToJson()
    {
        var dict = new Dictionary<int, string> { [1] = "one", [2] = "two" };
        var actual = dict.ToJson(writeIndented: false);
        Assert.That(actual, Is.EqualTo("""{"1":"one","2":"two"}"""));
    }

    [Test]
    public void DictionaryWithIntKeys_ToCSharp()
    {
        var dict = new Dictionary<int, string> { [1] = "one", [2] = "two" };
        var actual = dict.ToCSharpString(writeIndented: false);
        Assert.That(actual, Is.EqualTo("""{ [1] = "one", [2] = "two" }"""));
    }

    [Test]
    public void EmptyDictionary_ToJson()
    {
        var dict = new Dictionary<string, int>();
        var actual = dict.ToJson(writeIndented: false);
        Assert.That(actual, Is.EqualTo("{}"));
    }

    [Test]
    public void EmptyDictionary_ToCSharp()
    {
        var dict = new Dictionary<string, int>();
        var actual = dict.ToCSharpString(writeIndented: false);
        Assert.That(actual, Is.EqualTo("{ }"));
    }

    [Test]
    public void SortedDictionary_ToJson()
    {
        var dict = new SortedDictionary<string, int> { ["b"] = 2, ["a"] = 1 };
        var actual = dict.ToJson(writeIndented: false);
        Assert.That(actual, Is.EqualTo("""{"a":1,"b":2}"""));
    }

    [Test]
    public void ReadOnlyDictionary_ToJson()
    {
        var inner = new Dictionary<string, int> { ["x"] = 10 };
        var dict = new System.Collections.ObjectModel.ReadOnlyDictionary<string, int>(inner);
        var actual = dict.ToJson(writeIndented: false);
        Assert.That(actual, Is.EqualTo("""{"x":10}"""));
    }

    [Test]
    public void DictionaryToJson_Indented()
    {
        var dict = new Dictionary<string, int> { ["a"] = 1, ["b"] = 2 };
        var actual = dict.ToJson(writeIndented: true);
        var expected = string.Join(Environment.NewLine,
            "{",
            "  \"a\": 1,",
            "  \"b\": 2",
            "}");
        Assert.That(actual, Is.EqualTo(expected));
    }

    [Test]
    public void DictionaryToCSharp_Indented()
    {
        var dict = new Dictionary<string, int> { ["a"] = 1, ["b"] = 2 };
        var actual = dict.ToCSharpString(writeIndented: true);
        var expected = string.Join(Environment.NewLine,
            "{",
            "  [\"a\"] = 1,",
            "  [\"b\"] = 2",
            "}");
        Assert.That(actual, Is.EqualTo(expected));
    }

    [Test]
    public void NestedDictionary_ToJson()
    {
        var dict = new Dictionary<string, Dictionary<string, int>>
        {
            ["inner"] = new() { ["x"] = 1 }
        };
        var actual = dict.ToJson(writeIndented: false);
        Assert.That(actual, Is.EqualTo("""{"inner":{"x":1}}"""));
    }

    [Test]
    public void NonGenericHashtable_ToJson()
    {
        var ht = new Hashtable { ["a"] = 1, ["b"] = 2 };
        var actual = ht.ToJson(writeIndented: false);
        // Hashtable order is not guaranteed, so check both entries are present
        Assert.That(actual, Does.StartWith("{"));
        Assert.That(actual, Does.EndWith("}"));
        Assert.That(actual, Does.Contain("\"a\":1"));
        Assert.That(actual, Does.Contain("\"b\":2"));
    }
}
