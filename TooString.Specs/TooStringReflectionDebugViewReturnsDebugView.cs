using System.Numerics;
using System.Reflection;
using System.Text.Json;

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
        TestContext.Progress.WriteLine(value.TooString(TooStringHow.Reflection, ReflectionStyle.DebugView));
        
        Assert.That(value.TooString(TooStringHow.Reflection ), Is.EqualTo(value.ToString()));
    }
    

    [Test]
    public void GivenNamedTupleIgnoresSerializationStyleJson()
    {
        var value = (one: new Complex(3,4), two:2);

        var actual = value.TooString(TooStringHow.Reflection,ReflectionStyle.Json );
        var expected = "([3, 4], 2)";

        Assert.That(actual,Is.EqualTo(expected));
    }

    [Test]
    public void GivenNamedTuple2IgnoresSerializationStyleJson()
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

        var actual = value.TooString(TooStringHow.Reflection,ReflectionStyle.Json );
        var expected = """
                       (1, "boo", false, "Absolute", {"A":"A","B":[3, 4]})
                       """;
        Assert.That(actual,Is.EqualTo(expected));
    }
    
    [Test]
    public void GivenACompositeObject()
    {
        var value = new CompositeA { A = "boo", B = new Complex(3,4) };
        var expected = $"{{ A = boo, B = {new Complex(3,4)} }}";
        
        Assert.That(
            value.TooString(TooStringHow.Reflection,ReflectionStyle.DebugView), 
            Is.EqualTo(expected) 
        );
        
        Assert.That(
            value.TooString(TooStringHow.Reflection,ReflectionStyle.DebugView),
            Is.EqualTo(
                value.TooString(TooStringHow.Reflection) 
                ));
    }
    
    [Test]
    public void WithCircularReferencesTruncatedToTypeAtMaxDepth__GivenCircularReferences()
    {
        var value = new Circular{ A = "boo"};
        value.B = value;
        var expected = "{ A = boo, B = { A = boo, B = { A = boo, B = { A = boo, B = TooString.Specs.Circular } } } }";
        
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
                       { CodeBase = file:///--filename--, FullName = TooString.Specs, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null, EntryPoint = { Name = Main, DeclaringType = AutoGeneratedProgram, ReflectedType = AutoGeneratedProgram, MemberType = Method, MetadataToken = 100000000, Module = { MDStreamVersion = 131072, FullyQualifiedName = --filename--, ModuleVersionId = {  }, MetadataToken = 100000000, ScopeName = TooString.Specs.dll, Name = TooString.Specs.dll, Assembly = { CodeBase = file:///--filename--, FullName = TooString.Specs, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null, EntryPoint = Void Main(System.String[]), DefinedTypes = [], IsCollectible = False, ManifestModule = TooString.Specs.dll, ReflectionOnly = False, Location = --filename--, ImageRuntimeVersion = v4.0.30319, GlobalAssemblyCache = False, HostContext = 0, IsDynamic = False, ExportedTypes = [], IsFullyTrusted = True, CustomAttributes = [], EscapedCodeBase = file:///--filename--, Modules = [], SecurityRuleSet = None }, ModuleHandle = { MDStreamVersion = 131072 }, CustomAttributes = { Count = 1, Item = CustomAttributeData } }, IsSecurityCritical = True, IsSecuritySafeCritical = False, IsSecurityTransparent = False, MethodHandle = { Value = 9999999999 }, Attributes = PrivateScope, Private, Static, HideBySig, CallingConvention = Standard, ReturnType = System.Void, ReturnTypeCustomAttributes = { ParameterType = System.Void, Name = null, HasDefaultValue = True, DefaultValue = null, RawDefaultValue = null, MetadataToken = 100000000, Attributes = None, Member = { Name = Main, DeclaringType = AutoGeneratedProgram, ReflectedType = AutoGeneratedProgram, MemberType = Method, MetadataToken = 100000000, Module = TooString.Specs.dll, IsSecurityCritical = True, IsSecuritySafeCritical = False, IsSecurityTransparent = False, MethodHandle = System.RuntimeMethodHandle, Attributes = PrivateScope, Private, Static, HideBySig, CallingConvention = Standard, ReturnType = System.Void, ReturnTypeCustomAttributes = Void, ReturnParameter = Void, IsCollectible = False, IsGenericMethod = False, IsGenericMethodDefinition = False, ContainsGenericParameters = False, MethodImplementationFlags = Managed, IsAbstract = False, IsConstructor = False, IsFinal = False, IsHideBySig = True, IsSpecialName = False, IsStatic = True, IsVirtual = False, IsAssembly = False, IsFamily = False, IsFamilyAndAssembly = False, IsFamilyOrAssembly = False, IsPrivate = True, IsPublic = False, IsConstructedGenericMethod = False, CustomAttributes = [] }, Position = -1, IsIn = False, IsLcid = False, IsOptional = False, IsOut = False, IsRetval = False, CustomAttributes = { Length = 9999999999, LongLength = 9999999999, Rank = 1, SyncRoot = [], IsReadOnly = False, IsFixedSize = True, IsSynchronized = False } }, ReturnParameter = { ParameterType = System.Void, Name = null, HasDefaultValue = True, DefaultValue = null, RawDefaultValue = null, MetadataToken = 100000000, Attributes = None, Member = { Name = Main, DeclaringType = AutoGeneratedProgram, ReflectedType = AutoGeneratedProgram, MemberType = Method, MetadataToken = 100000000, Module = TooString.Specs.dll, IsSecurityCritical = True, IsSecuritySafeCritical = False, IsSecurityTransparent = False, MethodHandle = System.RuntimeMethodHandle, Attributes = PrivateScope, Private, Static, HideBySig, CallingConvention = Standard, ReturnType = System.Void, ReturnTypeCustomAttributes = Void, ReturnParameter = Void, IsCollectible = False, IsGenericMethod = False, IsGenericMethodDefinition = False, ContainsGenericParameters = False, MethodImplementationFlags = Managed, IsAbstract = False, IsConstructor = False, IsFinal = False, IsHideBySig = True, IsSpecialName = False, IsStatic = True, IsVirtual = False, IsAssembly = False, IsFamily = False, IsFamilyAndAssembly = False, IsFamilyOrAssembly = False, IsPrivate = True, IsPublic = False, IsConstructedGenericMethod = False, CustomAttributes = [] }, Position = -1, IsIn = False, IsLcid = False, IsOptional = False, IsOut = False, IsRetval = False, CustomAttributes = { Length = 9999999999, LongLength = 9999999999, Rank = 1, SyncRoot = [], IsReadOnly = False, IsFixedSize = True, IsSynchronized = False } }, IsCollectible = False, IsGenericMethod = False, IsGenericMethodDefinition = False, ContainsGenericParameters = False, MethodImplementationFlags = Managed, IsAbstract = False, IsConstructor = False, IsFinal = False, IsHideBySig = True, IsSpecialName = False, IsStatic = True, IsVirtual = False, IsAssembly = False, IsFamily = False, IsFamilyAndAssembly = False, IsFamilyOrAssembly = False, IsPrivate = True, IsPublic = False, IsConstructedGenericMethod = False, CustomAttributes = { Length = 9999999999, LongLength = 9999999999, Rank = 1, SyncRoot = { Length = 9999999999, LongLength = 9999999999, Rank = 1, SyncRoot = [], IsReadOnly = False, IsFixedSize = True, IsSynchronized = False }, IsReadOnly = False, IsFixedSize = True, IsSynchronized = False } }, DefinedTypes = { Length = 9999999999, LongLength = 9999999999, Rank = 1, SyncRoot = { Length = 9999999999, LongLength = 9999999999, Rank = 1, SyncRoot = { Length = 9999999999, LongLength = 9999999999, Rank = 1, SyncRoot = [], IsReadOnly = False, IsFixedSize = True, IsSynchronized = False }, IsReadOnly = False, IsFixedSize = True, IsSynchronized = False }, IsReadOnly = False, IsFixedSize = True, IsSynchronized = False }, IsCollectible = False, ManifestModule = { MDStreamVersion = 131072, FullyQualifiedName = --filename--, ModuleVersionId = {  }, MetadataToken = 100000000, ScopeName = TooString.Specs.dll, Name = TooString.Specs.dll, Assembly = { CodeBase = file:///--filename--, FullName = TooString.Specs, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null, EntryPoint = { Name = Main, DeclaringType = AutoGeneratedProgram, ReflectedType = AutoGeneratedProgram, MemberType = Method, MetadataToken = 100000000, Module = TooString.Specs.dll, IsSecurityCritical = True, IsSecuritySafeCritical = False, IsSecurityTransparent = False, MethodHandle = System.RuntimeMethodHandle, Attributes = PrivateScope, Private, Static, HideBySig, CallingConvention = Standard, ReturnType = System.Void, ReturnTypeCustomAttributes = Void, ReturnParameter = Void, IsCollectible = False, IsGenericMethod = False, IsGenericMethodDefinition = False, ContainsGenericParameters = False, MethodImplementationFlags = Managed, IsAbstract = False, IsConstructor = False, IsFinal = False, IsHideBySig = True, IsSpecialName = False, IsStatic = True, IsVirtual = False, IsAssembly = False, IsFamily = False, IsFamilyAndAssembly = False, IsFamilyOrAssembly = False, IsPrivate = True, IsPublic = False, IsConstructedGenericMethod = False, CustomAttributes = [] }, DefinedTypes = { Length = 9999999999, LongLength = 9999999999, Rank = 1, SyncRoot = [], IsReadOnly = False, IsFixedSize = True, IsSynchronized = False }, IsCollectible = False, ManifestModule = { MDStreamVersion = 131072, FullyQualifiedName = --filename--, ModuleVersionId = 00000000-0000-0000-0000-000000000000, MetadataToken = 100000000, ScopeName = TooString.Specs.dll, Name = TooString.Specs.dll, Assembly = TooString.Specs, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null, ModuleHandle = System.ModuleHandle, CustomAttributes = [] }, ReflectionOnly = False, Location = --filename--, ImageRuntimeVersion = v4.0.30319, GlobalAssemblyCache = False, HostContext = 0, IsDynamic = False, ExportedTypes = { Length = 9999999999, LongLength = 9999999999, Rank = 1, SyncRoot = [], IsReadOnly = False, IsFixedSize = True, IsSynchronized = False }, IsFullyTrusted = True, CustomAttributes = { Count = 10, Item = CustomAttributeData }, EscapedCodeBase = file:///--filename--, Modules = { Length = 9999999999, LongLength = 9999999999, Rank = 1, SyncRoot = [], IsReadOnly = False, IsFixedSize = True, IsSynchronized = False }, SecurityRuleSet = None }, ModuleHandle = { MDStreamVersion = 131072 }, CustomAttributes = { Count = 1, Item = CustomAttributeData } }, ReflectionOnly = False, Location = --filename--, ImageRuntimeVersion = v4.0.30319, GlobalAssemblyCache = False, HostContext = 0, IsDynamic = False, ExportedTypes = { Length = 9999999999, LongLength = 9999999999, Rank = 1, SyncRoot = { Length = 9999999999, LongLength = 9999999999, Rank = 1, SyncRoot = { Length = 9999999999, LongLength = 9999999999, Rank = 1, SyncRoot = [], IsReadOnly = False, IsFixedSize = True, IsSynchronized = False }, IsReadOnly = False, IsFixedSize = True, IsSynchronized = False }, IsReadOnly = False, IsFixedSize = True, IsSynchronized = False }, IsFullyTrusted = True, CustomAttributes = { Count = 10, Item = CustomAttributeData }, EscapedCodeBase = file:///--filename--, Modules = { Length = 9999999999, LongLength = 9999999999, Rank = 1, SyncRoot = { Length = 9999999999, LongLength = 9999999999, Rank = 1, SyncRoot = { Length = 9999999999, LongLength = 9999999999, Rank = 1, SyncRoot = [], IsReadOnly = False, IsFixedSize = True, IsSynchronized = False }, IsReadOnly = False, IsFixedSize = True, IsSynchronized = False }, IsReadOnly = False, IsFixedSize = True, IsSynchronized = False }, SecurityRuleSet = None }
                       """;
        var actual = value.TooString(TooStringHow.Reflection);

        TestContext.Progress.WriteLine(actual.RegexReplaceKnownRuntimeVariableValues());
#if NET6_0
        Assert.That(
            value
                .TooString(TooStringHow.Reflection)
                .RegexReplaceKnownRuntimeVariableValues(),
            
            Is.EqualTo(expected.RegexReplaceKnownRuntimeVariableValues())
        );
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
                       { MDStreamVersion = 131072, FullyQualifiedName = --filename--, ModuleVersionId = {  }, MetadataToken = 100000000, ScopeName = System.Private.CoreLib.dll, Name = System.Private.CoreLib.dll, Assembly = { CodeBase = file:///--filename--, FullName = System.Private.CoreLib, Version=6.0.0.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e, EntryPoint = null, DefinedTypes = { Length = 9999999999, LongLength = 9999999999, Rank = 1, SyncRoot = { Length = 9999999999, LongLength = 9999999999, Rank = 1, SyncRoot = [], IsReadOnly = False, IsFixedSize = True, IsSynchronized = False }, IsReadOnly = False, IsFixedSize = True, IsSynchronized = False }, IsCollectible = False, ManifestModule = { MDStreamVersion = 131072, FullyQualifiedName = --filename--, ModuleVersionId = {  }, MetadataToken = 100000000, ScopeName = System.Private.CoreLib.dll, Name = System.Private.CoreLib.dll, Assembly = { CodeBase = file:///--filename--, FullName = System.Private.CoreLib, Version=6.0.0.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e, EntryPoint = null, DefinedTypes = [], IsCollectible = False, ManifestModule = System.Private.CoreLib.dll, ReflectionOnly = False, Location = --filename--, ImageRuntimeVersion = v4.0.30319, GlobalAssemblyCache = False, HostContext = 0, IsDynamic = False, ExportedTypes = [], IsFullyTrusted = True, CustomAttributes = [], EscapedCodeBase = file:///--filename--, Modules = [], SecurityRuleSet = None }, ModuleHandle = { MDStreamVersion = 131072 }, CustomAttributes = { Count = 2, Item = CustomAttributeData } }, ReflectionOnly = False, Location = --filename--, ImageRuntimeVersion = v4.0.30319, GlobalAssemblyCache = False, HostContext = 0, IsDynamic = False, ExportedTypes = { Length = 9999999999, LongLength = 9999999999, Rank = 1, SyncRoot = { Length = 9999999999, LongLength = 9999999999, Rank = 1, SyncRoot = [], IsReadOnly = False, IsFixedSize = True, IsSynchronized = False }, IsReadOnly = False, IsFixedSize = True, IsSynchronized = False }, IsFullyTrusted = True, CustomAttributes = { Count = 22, Item = CustomAttributeData }, EscapedCodeBase = file:///--filename--, Modules = { Length = 9999999999, LongLength = 9999999999, Rank = 1, SyncRoot = { Length = 9999999999, LongLength = 9999999999, Rank = 1, SyncRoot = [], IsReadOnly = False, IsFixedSize = True, IsSynchronized = False }, IsReadOnly = False, IsFixedSize = True, IsSynchronized = False }, SecurityRuleSet = None }, ModuleHandle = { MDStreamVersion = 131072 }, CustomAttributes = { Count = 2, Item = CustomAttributeData } }
                       """;
        
        var actual = value.TooString(TooStringHow.Reflection);

        TestContext.Progress.WriteLine(actual.RegexReplaceKnownRuntimeVariableValues());

        #if NET6_0
        Assert.That(
            value.TooString(TooStringHow.Reflection)
                .RegexReplaceKnownRuntimeVariableValues(),
            Is.EqualTo(
                expected.RegexReplaceKnownRuntimeVariableValues()));
        #endif
    }
    
}
