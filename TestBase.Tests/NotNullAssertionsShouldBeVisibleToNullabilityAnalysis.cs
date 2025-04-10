using NUnit.Framework;

namespace TestBase.Tests;

//Not a test fixture. Code is for manual inspection unless you want to write
// a test to test the static analyzer.
public class NotNullAssertionsShouldBeVisibleToNullabilityAnalysis
{
    public void ForShouldNotBeNull()
    {
        var x = (null as string).ShouldNotBeNull();
        var _ = x;
    }

    public void ForShouldNotBeNullOrEmptyXXX()
    {
        var x = (null as string).ShouldNotBeNullOrEmpty();
        var y = (null as string).ShouldNotBeNullOrEmptyOrWhiteSpace();
        var _ = x;
        var __ = y;
    }
}