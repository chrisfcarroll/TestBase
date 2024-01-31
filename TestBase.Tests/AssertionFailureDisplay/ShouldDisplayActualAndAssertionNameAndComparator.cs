using NUnit.Framework;

namespace TestBase.Tests.AssertionFailureDisplay;

[TestFixture]
public class ShouldDisplayActualAndAssertionNameAndComparator
{
    public void GivenAValue()
    {
        var ass= Assert.Throws<Assertion>(
            () => 1.ShouldBe(2)
        );

        ass.ToString().ShouldStartWith("1 ShouldBe 2");
    }
}