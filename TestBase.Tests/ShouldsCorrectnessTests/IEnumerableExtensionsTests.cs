using System.Collections.Generic;
using NUnit.Framework;

namespace TestBase.Tests.ShouldsCorrectnessTests;

[TestFixture]
public class IEnumerableExtensionsTests
{
    [TestCase(new[] {1})]
    public void SingleOrAssertFail_Should_Return_not_throw(IEnumerable<int> testCase)
    {
            testCase.SingleOrAssertFail();
        }

    [TestCase(new[] {1, 2, 3})]
    public void SingleOrAssertFail_Should_Assert(IEnumerable<int> testCase)
    {
            try { testCase.SingleOrAssertFail(); } catch (Assertion) { return; }

            throw new Assertion("Should have thrown");
        }
}