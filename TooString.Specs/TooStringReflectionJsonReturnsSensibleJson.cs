using System.Numerics;
using System.Reflection;
using System.Text.Json;

namespace TooString.Specs;

[TestFixture]
public class TooStringReflectionJsonReturnsSensibleJson
{
    internal static readonly HttpClient httpClient = new HttpClient() { BaseAddress = new Uri("http://127.0.0.1") };

    [TestCase(null, "null")]
    [TestCase("boo", "\"boo\"")]
    [TestCase(1, "1")]
    [TestCase(1.2f, "1.2")]
    [TestCase(1.2d, "1.2")]
    [TestCase(UriKind.Absolute,"\"Absolute\"")]
    public void GivenAScalar(object value, string expected)
    {
        Assert.That(
            value.TooString(ReflectionStyle.Json ),
            Is.EqualTo(expected));
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
        var value = (one: new Complex(3,4), two:2);

        var actual = value.TooString(ReflectionStyle.Json );
        var expected = "[[3,4],2]";

        Assert.That(actual,Is.EqualTo(expected));
    }

    [Test]
    public void GivenNamedTuple2()
    {
        var value = (one:1, two:"boo", three:false, four:UriKind.Absolute, five: new CompositeA{A = "A", B= new Complex(3,4)});

        var defaultJsonned = JsonSerializer.Serialize(value);
        var jsonnedIncludeFields =
            value.TooString(new TooStringOptions(new ReflectionOptions(),
                                                 new JsonSerializerOptions()
                                                 {
                                                     IncludeFields = true
                                                 },
                                                 new[] { TooStringHow.Json }));
        
        TestContext.Progress.WriteLine("jsonnedIncludeFields  :" + jsonnedIncludeFields);
        TestContext.Progress.WriteLine("defaultJsonned:" + defaultJsonned);
        TestContext.Progress.WriteLine("ToString:" + value);

        var actual = value.TooString(ReflectionStyle.Json );
        var expected = """
                       [1,"boo",false,"Absolute",{"A":"A","B":[3,4]}]
                       """;
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
                       {"CodeBase":"file:///--filename--","FullName":"TooString.Specs, Version=X.X.X.X, Culture=neutral, PublicKeyToken=null","EntryPoint":{"Name":"Main","DeclaringType":"AutoGeneratedProgram","ReflectedType":"AutoGeneratedProgram","MemberType":"Method","MetadataToken":100000000,"Module":"TooString.Specs.dll","IsSecurityCritical":true,"IsSecuritySafeCritical":false,"IsSecurityTransparent":false,"MethodHandle":"System.RuntimeMethodHandle","Attributes":"PrivateScope, Private, Static, HideBySig","CallingConvention":"Standard","ReturnType":"System.Void","ReturnTypeCustomAttributes":"Void","ReturnParameter":"Void","IsCollectible":false,"IsGenericMethod":false,"IsGenericMethodDefinition":false,"ContainsGenericParameters":false,"MethodImplementationFlags":"Managed","IsAbstract":false,"IsConstructor":false,"IsFinal":false,"IsHideBySig":true,"IsSpecialName":false,"IsStatic":true,"IsVirtual":false,"IsAssembly":false,"IsFamily":false,"IsFamilyAndAssembly":false,"IsFamilyOrAssembly":false,"IsPrivate":true,"IsPublic":false,"IsConstructedGenericMethod":false,"CustomAttributes":[]},"DefinedTypes":["\u003C\u003Ef__AnonymousType0\u00602","\u003C\u003Ef__AnonymousType1\u00605","\u003C\u003Ef__AnonymousType2\u00605","Microsoft.CodeAnalysis.EmbeddedAttribute","System.Runtime.CompilerServices.NullableAttribute","System.Runtime.CompilerServices.NullableContextAttribute","System.Runtime.CompilerServices.RefSafetyRulesAttribute","AutoGeneratedProgram","TooString.Specs.TooStringBestEffortMakesGoodChoices"],"IsCollectible":false,"ManifestModule":{"MDStreamVersion":131072,"FullyQualifiedName":"--filename--","ModuleVersionId":"00000000-0000-0000-0000-000000000000","MetadataToken":100000000,"ScopeName":"TooString.Specs.dll","Name":"TooString.Specs.dll","Assembly":"TooString.Specs, Version=X.X.X.X, Culture=neutral, PublicKeyToken=null","ModuleHandle":"System.ModuleHandle","CustomAttributes":[]},"ReflectionOnly":false,"Location":"--filename--","ImageRuntimeVersion":"v4.0.30319","GlobalAssemblyCache":false,"HostContext":0,"IsDynamic":false,"ExportedTypes":["TooString.Specs.TooStringBestEffortMakesGoodChoices","TooString.Specs.TooStringCallerArgumentExpressionReturnsLiteralCode","TooString.Specs.TooStringDepthOptionSpecs","TooString.Specs.TooStringJsonReturnsJson","TooString.Specs.TooStringLengthAndDepthOptionSpecs","TooString.Specs.TooStringReadMeExamples","TooString.Specs.AStruct","TooString.Specs.TooStringReflectionDebugViewReturnsDebugView","TooString.Specs.TooStringReflectionJsonReturnsSensibleJson"],"IsFullyTrusted":true,"CustomAttributes":["[System.Runtime.CompilerServices.CompilationRelaxationsAttribute((Int32)8)]","[System.Runtime.CompilerServices.RuntimeCompatibilityAttribute(WrapNonExceptionThrows = True)]","[System.Diagnostics.DebuggableAttribute((System.Diagnostics.DebuggableAttribute\u002BDebuggingModes)263)]","[System.Runtime.Versioning.TargetFrameworkAttribute(\u0022.NETCoreApp,Version=v6.0\u0022, FrameworkDisplayName = \u0022.NET 6.0\u0022)]","[System.Reflection.AssemblyCompanyAttribute(\u0022TooString.Specs\u0022)]","[System.Reflection.AssemblyConfigurationAttribute(\u0022Debug\u0022)]","[System.Reflection.AssemblyFileVersionAttribute(\u00221.0.0.0\u0022)]","[System.Reflection.AssemblyInformationalVersionAttribute(\u00221.0.0\u00000000-0000-0000-0000-00000000000067caa437\u0022)]","[System.Reflection.AssemblyProductAttribute(\u0022TooString.Specs\u0022)]"],"EscapedCodeBase":"file:///--filename--","Modules":["TooString.Specs.dll"],"SecurityRuleSet":"None"}
                       """;
        var actual = value.TooString(TooStringHow.Reflection,TooStringOptions.ForJson() with {ReflectionOptions = TooStringOptions.ForJson().ReflectionOptions with {MaxDepth = 2}});

        TestContext.Progress.WriteLine(actual.RegexReplaceKnownRuntimeVariableValues());
        
        var comparableValue = actual.RegexReplaceKnownRuntimeVariableValues();

        #if NET6_0
        Assert.That(comparableValue,Is.EqualTo(expected.RegexReplaceKnownRuntimeVariableValues()));
        #else
        Assert.That(comparableValue.Substring(0,300),
                    Is.EqualTo(expected.Substring(0,300)
                                   .RegexReplaceKnownRuntimeVariableValues()));
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
                       {"MDStreamVersion":131072,"FullyQualifiedName":"--filename--","ModuleVersionId":{},"MetadataToken":100000000,"ScopeName":"System.Private.CoreLib.dll","Name":"System.Private.CoreLib.dll","Assembly":{"CodeBase":"file:///--filename--","FullName":"System.Private.CoreLib, Version=X.X.X.X, Culture=neutral, PublicKeyToken=7cec85d7bea7798e","EntryPoint":"null","DefinedTypes":[],"IsCollectible":false,"ManifestModule":"System.Private.CoreLib.dll","ReflectionOnly":false,"Location":"--filename--","ImageRuntimeVersion":"v4.0.30319","GlobalAssemblyCache":false,"HostContext":0,"IsDynamic":false,"ExportedTypes":[],"IsFullyTrusted":true,"CustomAttributes":[],"EscapedCodeBase":"file:///--filename--","Modules":[],"SecurityRuleSet":"None"},"ModuleHandle":{"MDStreamVersion":131072},"CustomAttributes":["[System.Runtime.CompilerServices.NullablePublicOnlyAttribute((Boolean)False)]","[System.Runtime.CompilerServices.SkipLocalsInitAttribute()]"]}
                       """;
        
        var actual = value.TooString(TooStringHow.Reflection,TooStringOptions.ForJson() with {ReflectionOptions = TooStringOptions.ForJson().ReflectionOptions with {MaxDepth = 2}});

        TestContext.Progress.WriteLine(actual.RegexReplaceKnownRuntimeVariableValues());

        var comparableValue = actual.RegexReplaceKnownRuntimeVariableValues();
        #if NET6_0
        Assert.That(comparableValue,Is.EqualTo(expected.RegexReplaceKnownRuntimeVariableValues()));
        #else
        Assert.That(comparableValue.Substring(0,300),
                    Is.EqualTo(expected.Substring(0,300)
                                   .RegexReplaceKnownRuntimeVariableValues()));
        Assert.That(actual.Length >= expected.Length);
        #endif
    }
    
}
