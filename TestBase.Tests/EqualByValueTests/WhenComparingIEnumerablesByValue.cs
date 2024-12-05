using System;
using System.Linq;
using NUnit.Framework;

namespace TestBase.Tests.EqualByValueTests;

[TestFixture]
public class WhenComparingIEnumerablesByValue
{
    [Test]
    public void Should_return_false_when_not_the_same()
    {
        var objectL = new {Id = 1, Name = "1"};
        var objectR = new {Id = 1, Name = "2"};
        
        new[]{objectL}.EqualsByValue(new[]{objectR}).ShouldBeFalse();
        new[]{objectL}.ToList().EqualsByValue(new[]{objectR}).ShouldBeFalse();
        new[]{objectL}.EqualsByValue(new[]{objectR}.ToList()).ShouldBeFalse();
    }

    [Test]
    public void Should_return_true_when_the_elements_match_ignoring_enumerable_type()
    {
        var objectL = new {Id = 1, Name = "1"};
        var objectR = new {Id = 1, Name = "1"};
        
        new[]{objectL}.EqualsByValue(new[]{objectR}).ShouldBeTrue();
        new[]{objectL}.ToList().EqualsByValue(new[]{objectR}).ShouldBeTrue();
        new[]{objectL}.EqualsByValue(new[]{objectR}.ToList()).ShouldBeTrue();
    }
    
    [TestCase(1,2,3)]
    public void Should_return_false_when_not_the_same(params int[] values)
    {
        var left = values;
        var right = values.Reverse().ToArray();
        
        left.EqualsByValue(right).ShouldBeFalse();
        
        var leftStr = values.Select(x => x.ToString()).ToArray();
        var rightStr = values.Reverse().Select(x => x.ToString()).ToArray();
        
        leftStr.EqualsByValue(rightStr).ShouldBeFalse();
    }

    [TestCase(1,2,3)]
    public void Should_return_true_when_the_elements_match_ignoring_enumerable_type(params int[] values)
    {
        var left = values.ToArray();
        var right = values.ToList();
        
        left.EqualsByValue(right).ShouldBeTrue();
        
        var leftStr = values.Select(x => x.ToString()).ToArray();
        var rightStr = values.Select(x => x.ToString()).ToArray();
        
        leftStr.EqualsByValue(rightStr).ShouldBeTrue();
    }
    
    
    public void Should_return_false_when_left_is_shorter()
    {
        var left = Array.Empty<int>();
        var right = new[] { 1 };
        
        left.EqualsByValue(right).ShouldBeFalse();
    }
    
    [TestCase(1,2,3)]
    public void Should_return_false_when_left_is_shorter(params int[] values)
    {
        var left = values;
        var right = values.Append(1 ).ToArray();
        
        left.EqualsByValue(right).ShouldBeFalse();
        
        var leftStr = values.Select(x => x.ToString()).ToArray();
        var rightStr = values.Reverse().Select(x => x.ToString()).ToArray();
        
        leftStr.EqualsByValue(rightStr).ShouldBeFalse();
    }
    
    
}