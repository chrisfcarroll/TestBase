using System;
using NUnit.Framework;

namespace TestBase.Tests.AssertionFailureDisplay;

#if NET6_0_OR_GREATER
[TestFixture]
public class ShouldBeTrueFalse_ShouldOutputActualExpression
{
    [Test]
    public void ShouldBeTrue_OutputsActualExpression()
    {
        var someCondition = false;
        try
        {
            someCondition.ShouldBeTrue();
            NUnit.Framework.Assert.Fail("Should have thrown");
        }
        catch (Exception ex) when (ex is Assertion || ex.InnerException is Assertion)
        {
            var e = ex as Assertion ?? (Assertion)ex.InnerException;
            Console.WriteLine(e.Message);
            NUnit.Framework.Assert.That(e.Message, Does.Contain(nameof(someCondition)));
        }
    }

    [Test]
    public void ShouldBeFalse_OutputsActualExpression()
    {
        var someCondition = true;
        try
        {
            someCondition.ShouldBeFalse();
            NUnit.Framework.Assert.Fail("Should have thrown");
        }
        catch (Exception ex) when (ex is Assertion || ex.InnerException is Assertion)
        {
            var e = ex as Assertion ?? (Assertion)ex.InnerException;
            Console.WriteLine(e.Message);
            NUnit.Framework.Assert.That(e.Message, Does.Contain(nameof(someCondition)));
        }
    }
}
#endif
