namespace TooString.Specs;

[TestFixture]
public class TooStringIndentedEnumerableSpecs
{
    record Person(int Id, string Name, bool Active);

    [Test]
    public void ScalarList_StaysOnOneLine()
    {
        var items = new List<int> { 1, 2, 3 };
        var result = items.TooString(StringifyAs.CSharp);
        TestContext.Out.WriteLine(result);

        Assert.That(result, Is.EqualTo("new []{ 1, 2, 3 }"));
    }

    [Test]
    public void ListOfComplexObjects_IsIndentedMultiLine_CSharp()
    {
        var people = new List<Person>
        {
            new(1, "Alice", true),
            new(2, "Bob", false)
        };
        var result = people.TooString(StringifyAs.CSharp);
        TestContext.Out.WriteLine(result);

        var expected = string.Join(Environment.NewLine,
            "new []{",
            "  new /*Person*/ {",
            "    Id = 1,",
            "    Name = \"Alice\",",
            "    Active = true",
            "  },",
            "  new /*Person*/ {",
            "    Id = 2,",
            "    Name = \"Bob\",",
            "    Active = false",
            "  }",
            "}");
        Assert.That(result, Is.EqualTo(expected));
    }

    [Test]
    public void ListOfComplexObjects_IsIndentedMultiLine_Json()
    {
        var people = new List<Person>
        {
            new(1, "Alice", true),
            new(2, "Bob", false)
        };
        var result = people.TooString(TooStringOptions.Default with
        {
            StringifyAs = StringifyAs.Json, WriteIndented = true
        });
        TestContext.Out.WriteLine(result);

        var expected = string.Join(Environment.NewLine,
            "[",
            "  {",
            "    \"Id\": 1,",
            "    \"Name\": \"Alice\",",
            "    \"Active\": true",
            "  },",
            "  {",
            "    \"Id\": 2,",
            "    \"Name\": \"Bob\",",
            "    \"Active\": false",
            "  }",
            "]");
        Assert.That(result, Is.EqualTo(expected));
    }
}
