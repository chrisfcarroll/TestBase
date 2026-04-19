using System.Numerics;
using NUnit.Framework.Internal;
using static System.Math;

namespace TooString.Specs;

[TestFixture]
public class TooStringCallerArgumentExpressionReturnsLiteralCode
{
    [Test]
    public void GivenASimpleExpression()
    {
        Assert.That( 
            (1+1).ToCallerArgumentString(),
            Is.EqualTo( "1+1" ) );
        
        Assert.That( 
            ( 2 + 2 ).ToCallerArgumentString(),
            Is.EqualTo( "2 + 2" ) );
        
        TestContext.Progress.WriteLine( ( Sqrt(4 * PI / 3)  ).ToCallerArgumentString());
        
        Assert.That( 
            ( Sqrt(4 * PI / 3)  ).ToCallerArgumentString(),
            Is.EqualTo( "Sqrt(4 * PI / 3)" ) );

        Assert.That( 
            ( Math.Sqrt(4 * Math.PI / 3)  ).ToCallerArgumentString(),
            Is.EqualTo( "Math.Sqrt(4 * Math.PI / 3)" ) );
    }

    [Test]
    public void GivenACollectionExpression()
    {
        var expectedOneOf = new[] { 1, 2, 3 };
        var actual = 4;

        var expectedContainsActual =
            (expectedOneOf.Contains(actual)).ToCallerArgumentString();
        
        TestContext.Progress.WriteLine(expectedContainsActual);
        Assert.That( 
            expectedContainsActual, 
            Is.EqualTo( "expectedOneOf.Contains(actual)" ) );

        var expectedOneOfAny = ( expectedOneOf.Any(e=> e==actual)  ).ToCallerArgumentString();
        TestContext.Progress.WriteLine(expectedOneOfAny);
        Assert.That( 
            expectedOneOfAny, 
            Is.EqualTo( "expectedOneOf.Any(e=> e==actual)" ) );
    }
    
    [Test]
    public void GivenAnObjectInitializer()
    {
        var newCompositeA = ( new CompositeA { A = "boo", B = new Complex(123,45) }  )
            .ToCallerArgumentString();
        TestContext.Progress.WriteLine(newCompositeA);
        Assert.That( 
            newCompositeA, 
            Is.EqualTo( "new CompositeA { A = \"boo\", B = new Complex(123,45) }" ) );
    }
    
    [Test]
    public void GivenAnAnonymousObjectInitializer()
    {
        var newCompositeA = 
            new { A = "boo", B = new Complex(123,45) }.ToCallerArgumentString();
        
        TestContext.Progress.WriteLine(newCompositeA);
        Assert.That( 
            newCompositeA, 
            Is.EqualTo( "new { A = \"boo\", B = new Complex(123,45) }" ) );
    }
}