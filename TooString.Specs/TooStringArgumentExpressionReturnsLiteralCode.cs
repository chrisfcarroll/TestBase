using System.Numerics;
using static System.Math;

namespace TooString.Specs;

[TestFixture]
public class TooStringArgumentExpressionReturnsLiteralCode
{
    [Test]
    public void GivenASimpleExpression()
    {
        Assert.That( 
            (1+1).TooString(TooStringMethod.CallerArgument), 
            Is.EqualTo( "1+1" ) );
        
        Assert.That( 
            ( 2 + 2 ).TooString(TooStringMethod.CallerArgument), 
            Is.EqualTo( "2 + 2" ) );
        
        Assert.That( 
            ( Sqrt(4 * PI / 3)  ).TooString(TooStringMethod.CallerArgument), 
            Is.EqualTo( "Sqrt(4 * PI / 3)" ) );

        Assert.That( 
            ( Math.Sqrt(4 * Math.PI / 3)  ).TooString(TooStringMethod.CallerArgument), 
            Is.EqualTo( "Math.Sqrt(4 * Math.PI / 3)" ) );
    }

    [Test]
    public void GivenACollectionExpression()
    {
        var expectedOneOf = new[] { 1, 2, 3 };
        var actual = 4;
        Assert.That( 
            ( expectedOneOf.Contains(actual)  ).TooString(TooStringMethod.CallerArgument), 
            Is.EqualTo( "expectedOneOf.Contains(actual)" ) );
        
        Assert.That( 
            ( expectedOneOf.Any(e=> e==actual)  ).TooString(TooStringMethod.CallerArgument), 
            Is.EqualTo( "expectedOneOf.Any(e=> e==actual)" ) );
    }
    
    [Test]
    public void GivenAnObjectInitializer()
    {
        Assert.That( 
            ( new CompositeA { A = "boo", B = new Complex(123,45) }  )
                .TooString(TooStringMethod.CallerArgument), 
            Is.EqualTo( "new CompositeA { A = \"boo\", B = new Complex(123,45) }" ) );
        
    }
}