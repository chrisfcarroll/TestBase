using static System.Math;

namespace TooString.Specs;

public class TooStringArgumentExpressionReturnsLiteralCode
{
    [Test]
    public void GivenASimpleExpression()
    {
        Assert.That( 
            (1+1).TooString(TooStringMethod.ArgumentExpression), 
            Is.EqualTo( "1+1" ) );
        
        Assert.That( 
            ( 2 + 2 ).TooString(TooStringMethod.ArgumentExpression), 
            Is.EqualTo( "2 + 2" ) );
        
        Assert.That( 
            ( Sqrt(4 * PI / 3)  ).TooString(TooStringMethod.ArgumentExpression), 
            Is.EqualTo( "Sqrt(4 * PI / 3)" ) );

        Assert.That( 
            ( Math.Sqrt(4 * Math.PI / 3)  ).TooString(TooStringMethod.ArgumentExpression), 
            Is.EqualTo( "Math.Sqrt(4 * Math.PI / 3)" ) );
    }

    [Test]
    public void GivenACollectionExpression()
    {
        var expectedOneOf = new[] { 1, 2, 3 };
        var actual = 4;
        Assert.That( 
            ( expectedOneOf.Contains(actual)  ).TooString(TooStringMethod.ArgumentExpression), 
            Is.EqualTo( "expectedOneOf.Contains(actual)" ) );
        
        Assert.That( 
            ( expectedOneOf.Any(e=> e==actual)  ).TooString(TooStringMethod.ArgumentExpression), 
            Is.EqualTo( "expectedOneOf.Any(e=> e==actual)" ) );
    }
    
}