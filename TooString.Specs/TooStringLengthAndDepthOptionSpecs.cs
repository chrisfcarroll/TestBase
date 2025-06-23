namespace TooString.Specs;

class CircularLong
{
    public int A { get; set; }
    public List<CircularLong> B { get; set; } = new();

    public static implicit operator CircularLong(int a) => new() {A = a};
}

[TestFixture]
public class TooStringLengthAndDepthOptionSpecs
{
    static CircularLong deep;
    static CircularLong lengthy;

    static TooStringLengthAndDepthOptionSpecs()
    {
        deep = new ()
        {
            A = 1,
            B = new()
            {
                new(){
                    A = 3,
                    B = new()
                    {
                        new()
                        {
                            A = 5,
                            B = new ()
                            {
                                new(){
                                    A = 7,
                                }
                            }
                        }
                    }
                }
            }
        };
        deep.B[0].B[0].B[0].B.Add(deep);
        lengthy = new()
        {
            A = 0,
            B = [1,2,3,4,5,6,7,8,9]
        };
    }

    [Test]
    public void MaxDepthOptionIsRespected()
    {
        var actual1= deep.TooString(maxDepth: 1,style:ReflectionStyle.DebugView);
        var expected1 = """{ A = 1, B = { Type = List<CircularLong>, Count = 1 } }""";
        Assert.That(actual1, Is.EqualTo(expected1));

        var actual2= deep.TooString(maxDepth: 2,style:ReflectionStyle.DebugView);
        var expected2 = """{ A = 1, B = { Type = List<CircularLong>, Count = 1 } }""";
        Assert.That(actual2, Is.EqualTo(expected2));

        var actual3= deep.TooString(maxDepth: 3,style:ReflectionStyle.DebugView);
        var expected3 = """{ A = 1, B = [ { A = 3, B = { Type = List<CircularLong>, Count = 1 } } ] }""";
        Assert.That(actual3, Is.EqualTo(expected3));

        var actual4= deep.TooString(maxDepth: 4,style:ReflectionStyle.DebugView);
        var expected4 = """{ A = 1, B = [ { A = 3, B = { Type = List<CircularLong>, Count = 1 } } ] }""";
         Assert.That(actual4, Is.EqualTo(expected4));

        var actual5= deep.TooString(maxDepth: 5,style:ReflectionStyle.DebugView);
        var expected5 = """{ A = 1, B = [ { A = 3, B = [ { A = 5, B = { Type = List<CircularLong>, Count = 1 } } ] } ] }""";
        Assert.That(actual5, Is.EqualTo(expected5));
    }

    [Test]
    public void MaxLengthOptionIsRespected()
    {
        var actual1= lengthy.TooString(maxDepth: 2, maxLength:1, style:ReflectionStyle.DebugView);
        var expected1 = """{ A = 0, B = { Type = List<CircularLong>, Count = 9 } }""";
        Assert.That(actual1, Is.EqualTo(expected1));

        var actual2= lengthy.TooString(maxDepth: 2, maxLength:2, style:ReflectionStyle.DebugView);
        var expected2 = """{ A = 0, B = { Type = List<CircularLong>, Count = 9 } }""";
        Assert.That(actual2, Is.EqualTo(expected2));

        var actual3= lengthy.TooString(maxDepth: 3, maxLength:2, style:ReflectionStyle.DebugView);
        var expected3 = """{ A = 0, B = [ { A = 1, B = { Type = List<CircularLong>, Count = 0 } }, { A = 2, B = { Type = List<CircularLong>, Count = 0 } } ] }""";
        Assert.That(actual3, Is.EqualTo(expected3));

        var actual4= lengthy.TooString(maxDepth: 3, maxLength:4, style:ReflectionStyle.DebugView);
        var expected4 = "{ A = 0, B = [ " +
                                        "{ A = 1, B = { Type = List<CircularLong>, Count = 0 } }, " +
                                        "{ A = 2, B = { Type = List<CircularLong>, Count = 0 } }, " +
                                        "{ A = 3, B = { Type = List<CircularLong>, Count = 0 } }, " +
                                        "{ A = 4, B = { Type = List<CircularLong>, Count = 0 } } " +
                                        "] }";
        Assert.That(actual4, Is.EqualTo(expected4));

        var actual5= lengthy.TooString(maxDepth: 3, maxLength:99, style:ReflectionStyle.DebugView);
        var expected5 = "{ A = 0, B = [ " +
                        "{ A = 1, B = { Type = List<CircularLong>, Count = 0 } }, " +
                        "{ A = 2, B = { Type = List<CircularLong>, Count = 0 } }, " +
                        "{ A = 3, B = { Type = List<CircularLong>, Count = 0 } }, " +
                        "{ A = 4, B = { Type = List<CircularLong>, Count = 0 } }, " +
                        "{ A = 5, B = { Type = List<CircularLong>, Count = 0 } }, " +
                        "{ A = 6, B = { Type = List<CircularLong>, Count = 0 } }, " +
                        "{ A = 7, B = { Type = List<CircularLong>, Count = 0 } }, " +
                        "{ A = 8, B = { Type = List<CircularLong>, Count = 0 } }, " +
                        "{ A = 9, B = { Type = List<CircularLong>, Count = 0 } } " +
                        "] }";
        Assert.That(actual5, Is.EqualTo(expected5));
    }

    [Test]
    public void MaxLength0IsRespected()
    {
        var actual20= lengthy.TooString(maxDepth: 2, maxLength:0, style:ReflectionStyle.DebugView);
        var expected20 = """{ A = 0, B = { Type = List<CircularLong>, Count = 9 } }""";
        Assert.That(actual20, Is.EqualTo(expected20));

        var actual30= lengthy.TooString(maxDepth: 3, maxLength:0, style:ReflectionStyle.DebugView);
        var expected30 = """{ A = 0, B = [ { A = 1, B = { Type = List<CircularLong>, Count = 0 } } ] }""";
        Assert.That(actual30, Is.EqualTo(expected30));
    }
}