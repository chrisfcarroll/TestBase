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
            ObjectTooString.ToCallerArgumentExpression((1+1)),
            Is.EqualTo( "1+1" ) );
        
        Assert.That( 
            ObjectTooString.ToCallerArgumentExpression(( 2 + 2 )),
            Is.EqualTo( "2 + 2" ) );
        
        TestContext.Progress.WriteLine( ObjectTooString.ToCallerArgumentExpression(( Sqrt(4 * PI / 3)  )));
        
        Assert.That( 
            ObjectTooString.ToCallerArgumentExpression(( Sqrt(4 * PI / 3)  )),
            Is.EqualTo( "Sqrt(4 * PI / 3)" ) );

        Assert.That( 
            ObjectTooString.ToCallerArgumentExpression(( Math.Sqrt(4 * Math.PI / 3)  )),
            Is.EqualTo( "Math.Sqrt(4 * Math.PI / 3)" ) );
    }

    [Test]
    public void GivenACollectionExpression()
    {
        var expectedOneOf = new[] { 1, 2, 3 };
        var actual = 4;

        var expectedContainsActual =
            ObjectTooString.ToCallerArgumentExpression((expectedOneOf.Contains(actual)));
        
        TestContext.Progress.WriteLine(expectedContainsActual);
        Assert.That( 
            expectedContainsActual, 
            Is.EqualTo( "expectedOneOf.Contains(actual)" ) );

        var expectedOneOfAny = ObjectTooString.ToCallerArgumentExpression(( expectedOneOf.Any(e=> e==actual)  ));
        TestContext.Progress.WriteLine(expectedOneOfAny);
        Assert.That( 
            expectedOneOfAny, 
            Is.EqualTo( "expectedOneOf.Any(e=> e==actual)" ) );
    }
    
    [Test]
    public void GivenAnObjectInitializer()
    {
        var newCompositeA = ObjectTooString.ToCallerArgumentExpression(( new CompositeA { A = "boo", B = new Complex(123,45) }  ));
        TestContext.Progress.WriteLine(newCompositeA);
        Assert.That( 
            newCompositeA, 
            Is.EqualTo( "new CompositeA { A = \"boo\", B = new Complex(123,45) }" ) );
    }
    
    [Test]
    public void GivenAnAnonymousObjectInitializer()
    {
        var newCompositeA = 
            ObjectTooString.ToCallerArgumentExpression(new { A = "boo", B = new Complex(123,45) });
        
        TestContext.Progress.WriteLine(newCompositeA);
        Assert.That( 
            newCompositeA, 
            Is.EqualTo( "new { A = \"boo\", B = new Complex(123,45) }" ) );
    }
}