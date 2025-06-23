namespace TooString.Specs;

[TestFixture]
public class TooStringDepthOptionSpecs
{
    static Circular circular;

    static TooStringDepthOptionSpecs()
    {
        circular = new ()
        {
            A = "1",
            B = new()
            {
                A = "2",
                B = new()
                {
                    A = "3",
                    B = new ()
                    {
                        A = "4",
                    }
                }
            }
        };
        circular.B.B.B.B = circular;
    }


    [Test]
    public void DepthOptionIsRespected()
    {
        var actual1= circular.TooString(maxDepth: 1,style:ReflectionStyle.DebugView);
        var expected1 = """{ A = 1, B = TooString.Specs.Circular }""";
        Assert.That(actual1, Is.EqualTo(expected1));

        var actual5= circular.TooString(maxDepth: 5,style:ReflectionStyle.DebugView);
        var expected5 = """{ A = 1, B = { A = 2, B = { A = 3, B = { A = 4, B = { A = 1, B = TooString.Specs.Circular } } } } }""";
        Assert.That(actual5, Is.EqualTo(expected5));

        var actual4= circular.TooString(maxDepth: 4,style:ReflectionStyle.DebugView);
        var expected4 = """{ A = 1, B = { A = 2, B = { A = 3, B = { A = 4, B = TooString.Specs.Circular } } } }""";
        Assert.That(actual4, Is.EqualTo(expected4));

        var actual3= circular.TooString(maxDepth: 3,style:ReflectionStyle.DebugView);
        var expected3 = """{ A = 1, B = { A = 2, B = { A = 3, B = TooString.Specs.Circular } } }""";
        Assert.That(actual3, Is.EqualTo(expected3));

        var actual2= circular.TooString(maxDepth: 2,style:ReflectionStyle.DebugView);
        var expected2 = """{ A = 1, B = { A = 2, B = TooString.Specs.Circular } }""";
        Assert.That(actual2, Is.EqualTo(expected2));
    }
}