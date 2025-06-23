namespace TooString.Specs;

class CircularLong
{
    public int A { get; set; }
    public List<CircularLong> B { get; set; } = new();
    public static implicit operator CircularLong(int a) => new() {A = a};
}
class CircularLongS
{
    public int A { get; set; }
    public List<CircularLongS> B { get; set; } = new();
    public static implicit operator CircularLongS(int a) => new() {A = a};

    public override string ToString() => $"(A = {A}, B.Count = {B.Count})";
}

[TestFixture]
public class TooStringLengthAndDepthOptionSpecs
{
    static CircularLong deep;
    static CircularLong lengthy;
    static CircularLong lengthyAndDeep;

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
        lengthyAndDeep = new()
        {
            A = 1,
            B = [
                new(){A=1, B= [11, 12, 13]},
                new(){A=2, B= [21, 22, 23]},
                new(){A=3, B= [new(){A=31,B=[311,312]}, 32, 33] }
            ]
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
        var expected30 = """{ A = 0, B = { Type = List<CircularLong>, Count = 9 } }""";
        Assert.That(actual30, Is.EqualTo(expected30));
    }
    [Test]
    public void NegativeMaxLengthOptionIsRespected()
    {
        var actual21= lengthyAndDeep.TooString(maxDepth: 2, maxLength: -1, style:ReflectionStyle.DebugView);
        var expected21 = """{ A = 1, B = { Type = List<CircularLong>, Count = 3 } }""";
        Assert.That(actual21, Is.EqualTo(expected21));

        var actual22= lengthyAndDeep.TooString(maxDepth: 2, maxLength: -2, style:ReflectionStyle.DebugView);
        var expected22 = """{ A = 1, B = { Type = List<CircularLong>, Count = 3 } }""";
        Assert.That(actual22, Is.EqualTo(expected22));

        var actual32= lengthyAndDeep.TooString(maxDepth: 3, maxLength: -2, style:ReflectionStyle.DebugView);
        var expected32 = """{ A = 1, B = [ { A = 1, B = { Type = List<CircularLong>, Count = 3 } } ] }""";
        Assert.That(actual32, Is.EqualTo(expected32));

        var actual92= lengthyAndDeep.TooString(maxDepth: 9, maxLength: -2, style:ReflectionStyle.DebugView);
        var expected92 = "{ A = 1, B = [ " +
                                        "{ A = 1, B = [ { A = 11, B = { Type = List<CircularLong>, Count = 0 } } ] } " +
                                        "] }";
        Assert.That(actual92, Is.EqualTo(expected92));

        var actual94= lengthyAndDeep.TooString(maxDepth: 9, maxLength: -4, style:ReflectionStyle.DebugView);
        var expected94 =
            "{ A = 1, B = [ " +
                        "{ A = 1, B = [ { A = 11, B = { Type = List<CircularLong>, Count = 0 } } ] }, " +
                        "{ A = 2, B = [ { A = 21, B = { Type = List<CircularLong>, Count = 0 } } ] }, " +
                        "{ A = 3, B = [ { A = 31, B = [ { A = 311, B = { Type = List<CircularLong>, Count = 0 } } ] } ] } " +
                        "] }";
        Assert.That(actual94, Is.EqualTo(expected94));

        var actual95= lengthyAndDeep.TooString(maxDepth: 9, maxLength: -5, style:ReflectionStyle.DebugView);
        var expected95 =
            "{ A = 1, B = [ " +
            "{ A = 1, B = [ { A = 11, B = { Type = List<CircularLong>, Count = 0 } }, { A = 12, B = { Type = List<CircularLong>, Count = 0 } } ] }, " +
            "{ A = 2, B = [ { A = 21, B = { Type = List<CircularLong>, Count = 0 } }, { A = 22, B = { Type = List<CircularLong>, Count = 0 } } ] }, " +
            "{ A = 3, B = [ { A = 31, B = { Type = List<CircularLong>, Count = 2 } }, { A = 32, B = { Type = List<CircularLong>, Count = 0 } } ] } " +
            "] }";
        Assert.That(actual95, Is.EqualTo(expected95));

        var actual96= lengthyAndDeep.TooString(maxDepth: 9, maxLength: -6, style:ReflectionStyle.DebugView);
        var expected96 =
            "{ A = 1, B = [ " +
            "{ A = 1, B = [ { A = 11, B = { Type = List<CircularLong>, Count = 0 } }, { A = 12, B = { Type = List<CircularLong>, Count = 0 } }, { A = 13, B = { Type = List<CircularLong>, Count = 0 } } ] }, " +
            "{ A = 2, B = [ { A = 21, B = { Type = List<CircularLong>, Count = 0 } }, { A = 22, B = { Type = List<CircularLong>, Count = 0 } }, { A = 23, B = { Type = List<CircularLong>, Count = 0 } } ] }, " +
            "{ A = 3, B = [ { A = 31, B = [ { A = 311, B = { Type = List<CircularLong>, Count = 0 } } ] }, { A = 32, B = { Type = List<CircularLong>, Count = 0 } }, { A = 33, B = { Type = List<CircularLong>, Count = 0 } } ] } " +
            "] }";
        Assert.That(actual96, Is.EqualTo(expected96));
    }
}