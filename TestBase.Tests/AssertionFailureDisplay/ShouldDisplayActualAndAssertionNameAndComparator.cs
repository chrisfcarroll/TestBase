using NUnit.Framework;

namespace TestBase.Tests.AssertionFailureDisplay;

[TestFixture]
public class ShouldDisplayActualAndAssertionNameAndComparator
{
    [Test]
    public void GivenLiteralValues()
    {
        var ass= Assert.Throws<Assertion>(
            () => 1.ShouldBe(2)
        );

        TestContext.WriteLine(ass);

        var actual = ass.ToString();

        actual.RegexReplaceWhitespaceEtc()
            .ShouldBe(
                """
                Failed :
                ShouldBe 2
                Actual :
                ----------------------------
                1
                ----------------------------
                Asserted : actual => x.Equals(expected)
                expected   →   2
                """
                    .RegexReplaceWhitespaceEtc());
    }
    
    [Test]
    public void GivenVariableValues()
    {
        var namedActual=1;
        var namedExpected = 2;
        
        var ass= Assert.Throws<Assertion>(
            () => namedActual.ShouldBe(namedExpected)
        );

        TestContext.WriteLine(ass);

        var assActual = ass.ToString();

        assActual.RegexReplaceWhitespaceEtc()
            .ShouldBe(
                $"""
                    Failed :
                    ShouldBe {namedExpected}
                    Actual :
                    ----------------------------
                    {namedActual}
                    ----------------------------
                    Asserted : parameter => x.Equals(parameterExpected)
                    expected   →   {namedExpected}
                    """
                    .RegexReplaceWhitespaceEtc());
    }    
 }