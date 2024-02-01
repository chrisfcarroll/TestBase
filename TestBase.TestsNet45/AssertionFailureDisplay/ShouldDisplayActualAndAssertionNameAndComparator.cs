using NUnit.Framework;

namespace TestBase.TestsNet45.AssertionFailureDisplay;

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

        actual.RegexReplaceWhitespaceAndBlankOutGuids()
            .ShouldBe(
                """
                Failed : 
                Actual : 
                ----------------------------
                1
                ----------------------------
                Asserted : ShouldBe
                x => x != null && x.Equals(expected)
                expected   →   2
                """
                    .RegexReplaceWhitespaceAndBlankOutGuids());
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

        assActual.RegexReplaceWhitespaceAndBlankOutGuids()
            .ShouldBe(
                $"""
             Failed :
             Actual :
             ----------------------------
             {namedActual}
             namedActual
             ----------------------------
             Asserted : ShouldBe
             x => x != null && x.Equals(expected)
             expected   →   {namedExpected}
             """
                    .RegexReplaceWhitespaceAndBlankOutGuids());
    }
    [Test]
    public void GivenExpressions()
    {
        var ass= Assert.Throws<Assertion>(
            () => (1 + 1).ShouldBe(2+2)
        );

        TestContext.WriteLine(ass);

        var actual = ass.ToString();

        actual.RegexReplaceWhitespaceAndBlankOutGuids()
            .ShouldBe(
                """
                    Failed :
                    Actual :
                    ----------------------------
                    2
                    1 + 1
                    ----------------------------
                    Asserted : ShouldBe
                    x => x != null && x.Equals(expected)
                    expected   →   4
                    """
                    .RegexReplaceWhitespaceAndBlankOutGuids());
    }
    
 }