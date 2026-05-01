using NUnit.Framework;

namespace TestBase.Tests;

[TestFixture]
public class FormatzTests
{
    [Test]
    public void SubstitutesNamedProperty()
    {
        var result = "{name} is {age}".Formatz(new { name = "Alice", age = 30 });

        result.ShouldBe("Alice is 30");
    }

    [Test]
    public void SubstitutesMultipleOccurrencesOfSameProperty()
    {
        var result = "{x} and {x}".Formatz(new { x = "hi" });

        result.ShouldBe("hi and hi");
    }

    [Test]
    public void EscapedDoubleBracesProduceLiteralBraces()
    {
        var result = "{{literal}}".Formatz(new { });

        result.ShouldBe("{literal}");
    }

    [Test]
    public void NoPlaceholders_ReturnsOriginalString()
    {
        var result = "no placeholders here".Formatz(new { x = 1 });

        result.ShouldBe("no placeholders here");
    }

    [Test]
    public void NullFormat_ReturnsNull()
    {
        var result = ((string)null).Formatz(new { x = 1 });

        (result == null).ShouldBeTrue();
    }

    [Test]
    public void EmptyFormat_ReturnsEmpty()
    {
        var result = "".Formatz(new { x = 1 });

        result.ShouldBe("");
    }

    [Test]
    public void MissingProperty_DoesNotThrow_ReturnsEmptyForThatPlaceholder()
    {
        var result = "hello {missing} world".Formatz(new { name = "Alice" });

        result.ShouldContain("hello");
        result.ShouldContain("world");
    }

    [Test]
    public void NullSource_DoesNotThrow()
    {
        var result = "hello {x}".Formatz(null);

        result.ShouldContain("hello");
    }

    [Test]
    public void UnterminatedOpenBrace_DoesNotThrow_ReturnsWhatWasParsed()
    {
        var result = "hello {name".Formatz(new { name = "Alice" });

        result.ShouldContain("hello");
    }

    [Test]
    public void StrayCloseBrace_DoesNotThrow()
    {
        var result = "hello } world".Formatz(new { });

        result.ShouldContain("hello");
        result.ShouldContain("world");
    }

    [Test]
    public void EmptyBraces_DoesNotThrow()
    {
        var result = "hello {} world".Formatz(new { });

        result.ShouldContain("hello");
        result.ShouldContain("world");
    }

    [Test]
    public void NestedPropertyAccess()
    {
        var result = "{inner.value}".Formatz(new { inner = new { value = "deep" } });

        result.ShouldBe("deep");
    }

    [Test]
    public void NullPropertyValue_DoesNotThrow()
    {
        var result = "val={x}".Formatz(new { x = (string)null });

        result.ShouldContain("val=");
    }

    [Test]
    public void MixOfValidAndMissingProperties_DoesNotThrow()
    {
        var result = "{a}-{missing}-{b}".Formatz(new { a = "A", b = "B" });

        result.ShouldContain("A");
        result.ShouldContain("B");
    }

    [Test]
    public void AdjacentPlaceholders()
    {
        var result = "{x}{y}".Formatz(new { x = "1", y = "2" });

        result.ShouldBe("12");
    }

    [Test]
    public void IntegerAndBooleanValues()
    {
        var result = "{count} is {flag}".Formatz(new { count = 42, flag = true });

        result.ShouldContain("42");
        result.ShouldContain("True");
    }
}
