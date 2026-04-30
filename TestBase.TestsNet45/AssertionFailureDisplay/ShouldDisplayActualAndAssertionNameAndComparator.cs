using NUnit.Framework;

namespace TestBase.TestsNet45.AssertionFailureDisplay;

[TestFixture]
public class ShouldDisplayActualAndAssertionNameAndComparator
{
    [Test]
    public void GivenLiteralValues()
    {
        var ass= Should.Throw<Assertion>(
            () => 1.ShouldBe(2)
        );

        TestContext.WriteLine(ass);

        ass.ToString()
            .ShouldContain("1")
            .ShouldContain("ShouldBe 2");
    }

    [Test]
    public void GivenVariableValues()
    {
        var namedActual=1;
        var namedExpected = 2;

        var ass= Should.Throw<Assertion>(
            () => namedActual.ShouldBe(namedExpected)
        );

        TestContext.WriteLine(ass);

        ass.ToString()
            .ShouldContain("1")
            .ShouldContain("ShouldBe 2");
    }

    [Test]
    public void GivenExpressions()
    {
        var ass= Should.Throw<Assertion>(
            () => (1 + 1).ShouldBe(2+2)
        );

        TestContext.WriteLine(ass);

        ass.ToString()
            .ShouldContain("2")
            .ShouldContain("ShouldBe 4");
    }
 }
