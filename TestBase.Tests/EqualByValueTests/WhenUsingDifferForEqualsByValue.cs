using NUnit.Framework;

namespace TestBase.Tests.EqualByValueTests;

[TestFixture]
public class WhenUsingDifferForEqualsByValue
{
    [Test]
    public void ShouldEqualByValue_passes_when_equal()
    {
        var left = new { Id = 1, Name = "Alice" };
        var right = new { Id = 1, Name = "Alice" };
        left.ShouldEqualByValue(right);
    }

    [Test]
    public void ShouldEqualByValue_passes_when_different_types_structurally_equal_treating_null_equals_missing_property()
    {
        var left = new AClass() { Id = 1, Name = "Alice" };
        var right = new { Id = 1, Name = "Alice" };

        //D
        TestContext.WriteLine(left.ToJSon());
        TestContext.WriteLine(right.ToJSon());
        //A
#if NET6_0_OR_GREATER
        left.ShouldEqualByValue(right);
#else
        right.ShouldEqualByValue(left);
#endif
    }

    [Test]
    public void ShouldEqualByValue_passes_when_different_types_structurally()
    {
        var left = new AClass() { Id = 1, Name = "Alice" };
        var right = new { Id = 1, Name = "Alice", More =null as object };

        left.ShouldEqualByValue(right);
    }

    [Test]
    public void ShouldEqualByValue_FAILS_when_different_types_not_structurally_equal()
    {
        var left = new AClass() { Id = 1, Name = "Alice" };
        var right = new { Id = 1, Name = "Alice", SomethingElse = "unexpected" };

        Assert.Throws<Assertion>(() => left.ShouldEqualByValue(right));
    }

#if NET6_0_OR_GREATER

    [Test]
    public void ShouldEqualByValue_fails_with_clean_diff_for_objects()
    {
        var left = new { Id = 1, Name = "Alice" };
        var right = new { Id = 1, Name = "Bob" };
        var ex = Assert.Throws<Assertion>(() => left.ShouldEqualByValue(right));

        // D
        TestContext.WriteLine(ex.Message);

        // A
        var msg = ex.Message;
        msg.ShouldContain("Name");
        msg.ShouldContain("Alice");
        msg.ShouldContain("Bob");
        msg.ShouldContain("ShouldEqualByValue");
    }

    [Test]
    public void ShouldEqualByValue_fails_with_clean_diff_for_strings()
    {
        var ex = Assert.Throws<Assertion>(() => "hello world".ShouldEqualByValue("hello World"));

        TestContext.WriteLine(ex.Message);

        var msg = ex.Message;
        msg.ShouldContain("hello");
        msg.ShouldContain("ShouldEqualByValue");
    }

    [Test]
    public void ShouldEqualByValue_fails_with_clean_diff_for_collections()
    {
        var ex = Assert.Throws<Assertion>(() =>
            new[] { 1, 2, 3 }.ShouldEqualByValue(new[] { 1, 2, 4 }));

        TestContext.WriteLine(ex.Message);

        var msg = ex.Message;
        msg.ShouldContain("[2]");
        msg.ShouldContain("ShouldEqualByValue");
    }
#endif

    [Test]
    public void ShouldEqualByValueExceptFor_passes_when_excluded_member_differs()
    {
        var left = new { Id = 1, Name = "Alice" };
        var right = new { Id = 2, Name = "Alice" };
        left.ShouldEqualByValueExceptFor(right, "Id");
    }

    [Test]
    public void ShouldEqualByValueExceptFor_fails_when_non_excluded_member_differs()
    {
        var left = new { Id = 1, Name = "Alice" };
        var right = new { Id = 2, Name = "Bob" };
        var ex = Assert.Throws<Assertion>(() =>
            left.ShouldEqualByValueExceptFor(right, "Id"));

        TestContext.WriteLine(ex.Message);

        var msg = ex.Message;
        msg.ShouldContain("Name");
    }

    [Test]
    public void ShouldEqualByValueOnMembers_passes_when_specified_members_match()
    {
        var left = new { Id = 1, Name = "Alice", Age = 30 };
        var right = new { Id = 1, Name = "Bob", Age = 31 };
        left.ShouldEqualByValueOnProperties(right, "Id");
    }

    [Test]
    public void ShouldEqualByValueOnMembers_fails_when_specified_member_differs()
    {
        var left = new { Id = 1, Name = "Alice" };
        var right = new { Id = 1, Name = "Bob" };
        var ex = Assert.Throws<Assertion>(() =>
            left.ShouldEqualByValueOnMembers(right, new[] { "Name" }));

        TestContext.WriteLine(ex.Message);

        var msg = ex.Message;
        msg.ShouldContain("Name");
    }

    [Test]
    public void ShouldEqualByValue_fails_with_custom_message()
    {
        var ex = Assert.Throws<Assertion>(() =>
            1.ShouldEqualByValue(2, "Expected {0} to equal {1}", 1, 2));

        TestContext.WriteLine(ex.Message);

        var msg = ex.Message;
        msg.ShouldContain("Expected 1 to equal 2");
    }

#if NET6_0_OR_GREATER
    [Test]
    public void Diff_output_does_not_contain_expression_tree_noise()
    {
        var ex = Assert.Throws<Assertion>(() =>
            new { A = 1, B = "hello" }.ShouldEqualByValue(new { A = 1, B = "world" }));

        TestContext.WriteLine("-----exception message-----");
        TestContext.WriteLine(ex.Message);
        TestContext.WriteLine("---------------------------");

        var msg = ex.Message;
        // Should NOT contain lambda/expression noise
        msg.ShouldNotContain("x =>");
        msg.ShouldNotContain("EqualsByValue");
        msg.ShouldNotContain("CompileFast");
        // Should contain clean diff output
        msg.ShouldContain("Expected");
        msg.ShouldContain("Actual");
    }
#endif

}
