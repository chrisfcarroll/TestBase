using System.Numerics;
using System.Reflection;
using NUnit.Framework.Constraints;

namespace TooString.Specs;

public struct AStruct { public string A {get; init; } public Complex B { get; init; } }

[TestFixture]
public class TooStringReflectionDebugViewReturnsDebugView
{
    internal static readonly HttpClient httpClient = new HttpClient() { BaseAddress = new Uri("http://127.0.0.1") };

    [TestCase(null, "null")]
    [TestCase("boo", "boo")]
    [TestCase(1, "1")]
    [TestCase(1.2f, "1.2")]
    [TestCase(1.2d, "1.2")]
    [TestCase(UriKind.Absolute,"Absolute")]
    public void GivenAScalar(object value, string expected)
    {
        Assert.That(value.TooString(TooStringHow.Reflection ), Is.EqualTo(expected));
    }

    [TestCase(null, "null")]
    [TestCase("boo", "boo")]
    [TestCase(1, "1")]
    [TestCase(1.2f, "1.2")]
    [TestCase(1.2d, "1.2")]
    [TestCase(UriKind.Absolute,"Absolute")]
    public void GivenAnEnumerable(object value, string expectedPart)
    {
        var subject= new object[] { value, 1};
        var expected= "[ " + expectedPart + ", 1 ]";
        var actual = subject.TooString(TooStringHow.Reflection );
        TestContext.Progress.WriteLine(actual);
        Assert.That(actual, Is.EqualTo(expected));

        var subject2= new object[] { 1, value, 3};
        var expected2= "[ 1, " + expectedPart + ", 3 ]";
        var actual2 = subject2.TooString(TooStringHow.Reflection );
        TestContext.Progress.WriteLine(actual2);
        Assert.That(actual2, Is.EqualTo(expected2));
    }

    [Test]
    public void GivenANumeric()
    {
        var examples = new object[]
        {
            new Complex(3,4),
            new BigInteger(123),
            new Quaternion(1,2,3,4),
        };
        foreach (var example in examples)
        {
            var expected = example.ToString();
            TestContext.Progress.WriteLine(expected);

            var actual = example.TooString(TooStringHow.Reflection);
            Assert.That(actual, Is.EqualTo(expected));
        }
    }


    [Test]
    public void GivenADateTimeOrDateOrTimeOrTimeSpan()
    {
        var now = DateTime.Now;
        var nowActual = now.TooString(TooStringHow.Reflection);
        Assert.That(nowActual,Is.EqualTo(now.ToString("O")));
        TestContext.Out.WriteLine("DateTime: " + nowActual);

        var dateOnly = DateOnly.FromDateTime(now);
        var dateOnlyActual = dateOnly.TooString(TooStringHow.Reflection);
        Assert.That(dateOnlyActual,Is.EqualTo(dateOnly.ToString("O")));
        TestContext.Out.WriteLine("DateOnly: " + dateOnlyActual);

        var timeOnly = TimeOnly.FromDateTime(now);
        var timeOnlyActual = timeOnly.TooString(TooStringHow.Reflection);
        Assert.That(timeOnlyActual,Is.EqualTo(timeOnly.ToString("HH:mm:ss")));
        TestContext.Out.WriteLine("TimeOnly: " + timeOnlyActual);

        var timeSpan = timeOnly.ToTimeSpan();
        var timeSpanActual = timeSpan.TooString(TooStringHow.Reflection);
        Assert.That(timeSpanActual,Is.EqualTo(timeSpan.ToString("c")));
        TestContext.Out.WriteLine("TimeSpan: " + timeSpanActual);
    }

    
    [Test]
    public void GivenStruct()
    {
        //var value = new KeyValuePair<int, string>(1, "boo");
        var value = new AStruct { A = "boo", B = new Complex(3, 4) };

        TestContext.Progress.WriteLine(value.ToDebugViewString());
        
        Assert.That(
            value.TooString(TooStringHow.Reflection ), 
            Is.EqualTo($"{{ A = boo, B = {new Complex(3,4)} }}")
            );
    }
    
    [Test]
    public void GivenAnonymousObject()
    {
        var value = new {
                one=1, 
                two="boo", 
                three=false, 
                four=UriKind.Absolute, 
                five= new CompositeA{A = "A", B= new Complex(3,4)}
            };
        
        TestContext.Progress.WriteLine(value.ToDebugViewString());
        
        Assert.That(
            value.TooString(TooStringHow.Reflection ), 
            Is.EqualTo($"{{ one = 1, two = boo, three = False, four = Absolute, five = {{ A = A, B = {new Complex(3,4)} }} }}"));
    }
    
    [Test]
    public void GivenTuple()
    {
        var value = (1,"boo",false,UriKind.Absolute);
        Assert.That(value.TooString(TooStringHow.Reflection ), Is.EqualTo(value.ToString()));
    }
    
    [Test]
    public void GivenNamedTuple()
    {
        var value = (one:1, two:"boo", three:false, four:UriKind.Absolute, five: new CompositeA{A = "A", B= new (3,4)});

        TestContext.Progress.WriteLine(value.TooString(TooStringHow.Reflection));
        TestContext.Progress.WriteLine(value.TooString(ReflectionStyle.DebugView));
        
        Assert.That(value.TooString(TooStringHow.Reflection ), Is.EqualTo(value.ToString()));
    }
    

    [Test]
    public void GivenNamedTupleWithNumerics()
    {
        var value = (one: new Complex(3,4), two:2);

        var actual = value.TooString(ReflectionStyle.DebugView );
        var expected = $"({new Complex(3,4)}, 2)";

        Assert.That(actual,Is.EqualTo(expected));
    }

    [Test]
    public void GivenNamedTupleWithNumerics2()
    {
        var value = (one:1, two:"boo", three:false, four:UriKind.Absolute, five: new CompositeA{A = "A", B= new Complex(3,4)});

        TestContext.Progress.WriteLine("ToString:" + value);

        var actual = value.TooString(ReflectionStyle.DebugView );
        var expected = $"(1, boo, False, Absolute, {{ A = A, B = {new Complex(3,4)} }})";
        Assert.That(actual,Is.EqualTo(expected));
    }
    
    [Test]
    public void GivenACompositeObject()
    {
        var value = new CompositeA { A = "boo", B = new Complex(3,4) };
        var expected = $"{{ A = boo, B = {new Complex(3,4)} }}";
        
        Assert.That(
            value.TooString(ReflectionStyle.DebugView), 
            Is.EqualTo(expected) 
        );
        
        Assert.That(
            value.TooString(ReflectionStyle.DebugView),
            Is.EqualTo(
                value.TooString(TooStringHow.Reflection) 
                ));
    }
    
    [Test]
    public void WithCircularReferencesTruncatedToTypeAtMaxDepth__GivenCircularReferences()
    {
        var value = new Circular{ A = "boo"};
        value.B = value;
        var expected = "{ A = boo, B = { A = boo, B = { A = boo, B = TooString.Specs.Circular } } }";
        
        Assert.That(
            value.TooString(TooStringHow.Reflection), 
            Is.EqualTo(expected) 
        );
    }
    
    [Test,Ignore("Don't create an HttpClient on every test run")]
    public void GivenDifficultObject()
    {
        var value = httpClient;
        var expected = 
            "{\"DefaultRequestHeaders\":[],\"DefaultRequestVersion\":\"1.1\"," +
            "\"DefaultVersionPolicy\":0,\"BaseAddress\":\"http://127.0.0.1\"," +
            "\"Timeout\":\"00:01:40\",\"MaxResponseContentBufferSize\":2147483647}";
        
        Assert.That(
            value.TooString(TooStringHow.Reflection), 
            Is.EqualTo(expected) 
        );
    }
    
    [Test]
    public void GivenAnUnJsonableReflectionType_Assembly()
    {
        var value = Assembly.GetExecutingAssembly();

        var expected = """
                       { CodeBase = file:///--filename--, FullName = TooString.Specs, Version=X.X.X.X, Culture=neutral, PublicKeyToken=null, EntryPoint = { Name = Main, DeclaringType = AutoGeneratedProgram, ReflectedType = AutoGeneratedProgram, MemberType = Method, MetadataToken = 100000000, Module = TooString.Specs.dll, IsSecurityCritical = True, IsSecuritySafeCritical = False, IsSecurityTransparent = False, MethodHandle = System.RuntimeMethodHandle, Attributes = PrivateScope, Private, Static, HideBySig, CallingConvention = Standard, ReturnType = System.Void, ReturnTypeCustomAttributes = Void, ReturnParameter = Void, IsCollectible = False, IsGenericMethod = False, IsGenericMethodDefinition = False, ContainsGenericParameters = False, MethodImplementationFlags = Managed, IsAbstract = False, IsConstructor = False, IsFinal = False, IsHideBySig = True, IsSpecialName = False, IsStatic = True, IsVirtual = False, IsAssembly = False, IsFamily = False, IsFamilyAndAssembly = False, IsFamilyOrAssembly = False, IsPrivate = True, IsPublic = False, IsConstructedGenericMethod = False, CustomAttributes = System.Reflection.CustomAttributeData[0] }, DefinedTypes = System.RuntimeType[31], IsCollectible = False, ManifestModule = { MDStreamVersion = 131072, FullyQualifiedName = --filename--, ModuleVersionId = 00000000-0000-0000-0000-000000000000, MetadataToken = 100000000, ScopeName = TooString.Specs.dll, Name = TooString.Specs.dll, Assembly = TooString.Specs, Version=X.X.X.X, Culture=neutral, PublicKeyToken=null, ModuleHandle = System.ModuleHandle, CustomAttributes = { Type = ReadOnlyCollection<CustomAttributeData>, Count = 1 } }, ReflectionOnly = False, Location = --filename--, ImageRuntimeVersion = v4.0.30319, GlobalAssemblyCache = False, HostContext = 0, IsDynamic = False, ExportedTypes = System.Type[10], IsFullyTrusted = True, CustomAttributes = { Type = ReadOnlyCollection<CustomAttributeData>, Count = 10 }, EscapedCodeBase = file:///--filename--, Modules = System.Reflection.RuntimeModule[1], SecurityRuleSet = None }
                       """;
        var actual = value.TooString(TooStringHow.Reflection,TooStringOptions.ForReflection(new(MaxDepth: 2)));

        TestContext.Progress.WriteLine(actual.RegexReplaceCompilationDependentValuesWithPseudoValues());

        var comparableValue = actual.RegexReplaceCompilationDependentValuesWithPseudoValues();
        #if NET6_0
        Assert.That(comparableValue,Is.EqualTo(expected.RegexReplaceCompilationDependentValuesWithPseudoValues()));
        #else
        Assert.That(comparableValue.Substring(0,300),
                    Is.EqualTo(expected.Substring(0,300)
                                                 .RegexReplaceCompilationDependentValuesWithPseudoValues()));
        Assert.That(actual.Length >= expected.Length);
        #endif
    }


    [Test]
    public void GivenAnUnJsonableReflectionType_Module()
    {
        var value = typeof(Type)
            .GetMethods(BindingFlags.Instance|BindingFlags.Public)
            .First(m=>m.Name=="GetMethods")
            .Module;

        var expected = """
                       { MDStreamVersion = 131072, FullyQualifiedName = --filename--, ModuleVersionId = {  }, MetadataToken = 100000000, ScopeName = System.Private.CoreLib.dll, Name = System.Private.CoreLib.dll, Assembly = { CodeBase = file:///--filename--, FullName = System.Private.CoreLib, Version=X.X.X.X, Culture=neutral, PublicKeyToken=7cec85d7bea7798e, EntryPoint = null, DefinedTypes = System.RuntimeType[2341], IsCollectible = False, ManifestModule = System.Private.CoreLib.dll, ReflectionOnly = False, Location = --filename--, ImageRuntimeVersion = v4.0.30319, GlobalAssemblyCache = False, HostContext = 0, IsDynamic = False, ExportedTypes = System.Type[1186], IsFullyTrusted = True, CustomAttributes = { Type = ReadOnlyCollection<CustomAttributeData>, Count = 22 }, EscapedCodeBase = file:///--filename--, Modules = System.Reflection.RuntimeModule[1], SecurityRuleSet = None }, ModuleHandle = { MDStreamVersion = 131072 }, CustomAttributes = { Type = ReadOnlyCollection<CustomAttributeData>, Count = 2 } }
                       """;
        
        var actual = value.TooString(TooStringHow.Reflection, TooStringOptions.ForReflection(new(MaxDepth: 2)));

        TestContext.Progress.WriteLine(actual.RegexReplaceCompilationDependentValuesWithPseudoValues());

        var comparableValue = actual.RegexReplaceCompilationDependentValuesWithPseudoValues();
        #if NET6_0
        Assert.That(comparableValue,Is.EqualTo(expected.RegexReplaceCompilationDependentValuesWithPseudoValues())
        );
        #else
        Assert.That(comparableValue.Substring(0,300),
                    Is.EqualTo(expected.Substring(0,300)
                                                 .RegexReplaceCompilationDependentValuesWithPseudoValues()));
        Assert.That(actual.Length >= expected.Length);
        #endif
    }
    
}
