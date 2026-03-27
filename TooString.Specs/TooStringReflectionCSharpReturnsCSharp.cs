namespace TooString.Specs;

[TestFixture]
public class TooStringReflectionCSharpReturnsCSharp
{
    record Person(int Id, string Name, bool Active);
    record Address(string Street, string City);
    record PersonWithAddress(int Id, string Name, Address Home);
    enum Status { Active, Inactive, Pending }
    record PersonWithStatus(int Id, string Name, Status Status);

    [Test]
    public void CSharp_simple_object_has_valid_syntax()
    {
        var person = new Person(1, "John", true);
        var result = person.TooString(TooStringStyle.CSharp);
        TestContext.Out.WriteLine(result);

        Assert.That(result, Does.StartWith("/*Person*/ new {"));
        Assert.That(result, Does.Contain("Id = 1"));
        Assert.That(result, Does.Contain("Name = \"John\""));
        Assert.That(result, Does.Contain("Active = true"));
        Assert.That(result, Does.EndWith("}"));
    }

    [Test]
    public void CSharp_nested_object_has_valid_syntax()
    {
        var person = new PersonWithAddress(1, "John", new Address("123 Main St", "Springfield"));
        var result = person.TooString(TooStringStyle.CSharp);
        TestContext.Out.WriteLine(result);

        Assert.That(result, Does.StartWith("/*PersonWithAddress*/ new {"));
        Assert.That(result, Does.Contain("/*Address*/ new {"));
        Assert.That(result, Does.Contain("Street = \"123 Main St\""));
    }

    [Test]
    public void CSharp_array_uses_new_array_syntax()
    {
        var items = new[] { 1, 2, 3 };
        var result = items.TooString(TooStringStyle.CSharp);
        TestContext.Out.WriteLine(result);

        Assert.That(result, Does.StartWith("new[] {"));
        Assert.That(result, Does.Contain("1, 2, 3"));
        Assert.That(result, Does.EndWith("}"));
    }

    [Test]
    public void CSharp_array_of_objects()
    {
        var people = new[]
        {
            new Person(1, "John", true),
            new Person(2, "Jane", false)
        };
        var result = people.TooString(TooStringStyle.CSharp);
        TestContext.Out.WriteLine(result);

        Assert.That(result, Does.StartWith("new[] {"));
        Assert.That(result, Does.Contain("/*Person*/ new {"));
    }

    [Test]
    public void CSharp_enum_uses_type_prefix()
    {
        var person = new PersonWithStatus(1, "John", Status.Active);
        var result = person.TooString(TooStringStyle.CSharp);
        TestContext.Out.WriteLine(result);

        Assert.That(result, Does.Contain("Status.Active"));
    }

    [Test]
    public void CSharp_string_is_quoted()
    {
        var text = "Hello World";
        var result = text.TooString(TooStringStyle.CSharp);
        TestContext.Out.WriteLine(result);

        Assert.That(result, Is.EqualTo("\"Hello World\""));
    }

    [Test]
    public void CSharp_boolean_is_lowercase()
    {
        var t = true;
        var f = false;

        Assert.That(t.TooString(TooStringStyle.CSharp), Is.EqualTo("true"));
        Assert.That(f.TooString(TooStringStyle.CSharp), Is.EqualTo("false"));
    }

    [Test]
    public void CSharp_tuple_uses_parentheses()
    {
        var tuple = (Id: 1, Name: "John");
        var result = tuple.TooString(TooStringStyle.CSharp);
        TestContext.Out.WriteLine(result);

        Assert.That(result, Does.StartWith("("));
        Assert.That(result, Does.EndWith(")"));
        Assert.That(result, Does.Contain("1"));
        Assert.That(result, Does.Contain("\"John\""));
    }

    [Test]
    public void CSharp_null_is_lowercase()
    {
        string? value = null;
        var result = value.TooString(TooStringStyle.CSharp);

        Assert.That(result, Is.EqualTo("null"));
    }

    [Test]
    public void CSharp_int_is_unchanged()
    {
        var n = 42;
        var result = n.TooString(TooStringStyle.CSharp);

        Assert.That(result, Is.EqualTo("42"));
    }

    [Test]
    public void CSharp_output_is_valid_for_copy_paste()
    {
        var person = new Person(1, "John", true);
        var result = person.TooString(TooStringStyle.CSharp);
        TestContext.Out.WriteLine("// Can be pasted into C# code:");
        TestContext.Out.WriteLine($"var obj = {result};");

        // The output should be parseable as an anonymous object literal
        // (after removing the type comment)
        Assert.That(result, Does.Match(@"/\*\w+\*/ new \{.*\}"));
    }

    [Test]
    public void CSharp_list_uses_new_array_syntax()
    {
        var items = new List<int> { 1, 2, 3 };
        var result = items.TooString(TooStringStyle.CSharp);
        TestContext.Out.WriteLine(result);

        Assert.That(result, Does.StartWith("new[] {"));
    }

    [Test]
    public void CSharp_datetime_is_quoted_string()
    {
        var dt = new DateTime(2024, 1, 15, 10, 30, 0);
        var result = dt.TooString(TooStringStyle.CSharp);
        TestContext.Out.WriteLine(result);

        // DateTime becomes a quoted string in CSharp style since there's no literal
        Assert.That(result, Does.StartWith("\""));
        Assert.That(result, Does.EndWith("\""));
    }

    [Test]
    public void CSharp_string_with_quotes_is_escaped()
    {
        var text = "Say \"Hello\"";
        var result = text.TooString(TooStringStyle.CSharp);
        TestContext.Out.WriteLine(result);

        Assert.That(result, Does.Contain("\\"));
    }
}
