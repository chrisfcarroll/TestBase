using System.Dynamic;
using System.Numerics;
using System.Reflection;
using System.Text.Json;

namespace TooString.Specs;

[TestFixture]
public class TooStringJsonReturnsJson
{
    internal static readonly HttpClient httpClient = new HttpClient() { BaseAddress = new Uri("http://127.0.0.1") };

    [TestCase(null, "null")]
    [TestCase("boo", "\"boo\"")]
    [TestCase(1, "1")]
    [TestCase(1.2f, "1.2")]
    [TestCase(1.2d, "1.2")]
    public void GivenAScalar(object value, string expected)
    {
        Assert.That(value.TooString(TooStringHow.Json ), Is.EqualTo(expected));
    }

    [Test]
    public void GivenADate()
    {
        var now = DateTime.Now;
        Assert.That(
            now.TooString(TooStringHow.Json),
            Is.EqualTo($"\"{now.ToString("O")}\""));
    }

    [Test]
    public void GivenACompositeObject()
    {
        var value = new CompositeA { A = "boo", B = new Complex(3,4) };
        var expected = 
            "{\"A\":\"boo\",\"B\":{\"Real\":3,\"Imaginary\":4,\"Magnitude\":5,\"Phase\":0.9272952180016122}}";
        
        TestContext.Progress.WriteLine(value.TooString(TooStringHow.Json));
        
        Assert.That(
            value.TooString(TooStringHow.Json), 
            Is.EqualTo(expected) 
        );
    }
    
   
    [Test]
    public void GivenNamedTupleReturnsEmptyObjectUnlessIncludeFieldsIsSpecified()
    {
        var value = (one:1, two:"boo", three:false, four:UriKind.Absolute, five: new CompositeA{A = "A", B= new Complex(3,4)});

        var defaultJsonned = JsonSerializer.Serialize(value);

        var options = TooStringOptions.Default with
        {
            JsonOptions = new JsonSerializerOptions { IncludeFields = true }
        };
        
        var jsonnedIncludeFields = value.TooString(options);
        
        TestContext.Progress.WriteLine("jsonnedIncludeFields  :" + jsonnedIncludeFields);
        TestContext.Progress.WriteLine("defaultJsonned:" + defaultJsonned);
        TestContext.Progress.WriteLine("ToString:" + value);
        
        Assert.That(
            value.TooString(TooStringHow.Json,ReflectionStyle.Json ), 
            Is.EqualTo("{}"));
        
        var valueAsAnonymousObject = new
        {
            Item1 = 1, Item2 = "boo", Item3 = false, Item4 = UriKind.Absolute,
            Item5 = new CompositeA { A = "A", B = new Complex(3, 4) }
        };
        Assert.That(
            jsonnedIncludeFields, 
            Is.EqualTo( valueAsAnonymousObject.TooString(TooStringHow.Json)));
    }

    
    [Test]
    public void GivenAnAnonymousObject()
    {
        var value = new { A = "boo", B = new Complex(3,4) };
        var expected = 
            "{\"A\":\"boo\",\"B\":{\"Real\":3,\"Imaginary\":4,\"Magnitude\":5,\"Phase\":0.9272952180016122}}";
        
        TestContext.Progress.WriteLine(value.TooString(TooStringHow.Json));
        
        Assert.That(
            value.TooString(TooStringHow.Json), 
            Is.EqualTo(expected) 
        );
    }
    
    [Test]
    public void WithCircularReferencesNulledOut__GivenCircularReferences()
    {
        var value = new Circular{ A = "boo"};
        value.B = value;
        var expected = 
            "{\"A\":\"boo\",\"B\":null}";
        
        Assert.That(
            value.TooString(TooStringHow.Json), 
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
            value.TooString(TooStringHow.Json), 
            Is.EqualTo(expected) 
        );
    }
    
    
    [Test]
    public void AndTestMethodsToRedactFilePathsDontBreakTheseTests()
    {
        var anAssembly = Assembly.GetExecutingAssembly();
        var actual = anAssembly.ManifestModule.FullyQualifiedName.TooString(TooStringHow.Json);
        var expected = "\"" +
                       /* expect Json to escape backslashes with backslashes */
                       Path.Combine(Directory.GetCurrentDirectory(), "TooString.Specs.dll").Replace("\\","\\\\") +
                       "\"";
        
        TestContext.Progress.WriteLine("""A single backslash looks like \""");
        TestContext.Progress.WriteLine(actual);
        
        Assert.That(
            actual.RegexReplaceKnownRuntimeVariableValues(),
            Is.EqualTo(expected.RegexReplaceKnownRuntimeVariableValues())
        );
        
        Assert.That(actual, Is.EqualTo(expected));
    }
    
    [Test]
    public void GivenAnUnJsonableReflectionType_Assembly()
    {
        var value = Assembly.GetExecutingAssembly();

        var expected = """
                       {"CodeBase":"file:///--filename--","FullName":"TooString.Specs, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null","EntryPoint":{"Name":"Main","DeclaringType":"AutoGeneratedProgram","ReflectedType":"AutoGeneratedProgram","MemberType":"Method","MetadataToken":100000000,"Module":{"MDStreamVersion":131072,"FullyQualifiedName":"--filename--","ModuleVersionId":{},"MetadataToken":100000000,"ScopeName":"TooString.Specs.dll","Name":"TooString.Specs.dll","Assembly":{"CodeBase":"file:///--filename--","FullName":"TooString.Specs, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null","EntryPoint":"Void Main(System.String[])","DefinedTypes":[],"IsCollectible":false,"ManifestModule":"TooString.Specs.dll","ReflectionOnly":false,"Location":"--filename--","ImageRuntimeVersion":"v4.0.30319","GlobalAssemblyCache":false,"HostContext":0,"IsDynamic":false,"ExportedTypes":[],"IsFullyTrusted":true,"CustomAttributes":"System.Collections.ObjectModel.ReadOnlyCollection\u00601[System.Reflection.CustomAttributeData]","EscapedCodeBase":"file:///--filename--","Modules":[],"SecurityRuleSet":"None"},"ModuleHandle":{"MDStreamVersion":131072},"CustomAttributes":{"Count":1,"Item":"CustomAttributeData"}},"IsSecurityCritical":true,"IsSecuritySafeCritical":false,"IsSecurityTransparent":false,"MethodHandle":{"Value":9999999999},"Attributes":"PrivateScope, Private, Static, HideBySig","CallingConvention":"Standard","ReturnType":"System.Void","ReturnTypeCustomAttributes":{"ParameterType":"System.Void","Name":"null","HasDefaultValue":true,"DefaultValue":"null","RawDefaultValue":"null","MetadataToken":100000000,"Attributes":"None","Member":{"Name":"Main","DeclaringType":"AutoGeneratedProgram","ReflectedType":"AutoGeneratedProgram","MemberType":"Method","MetadataToken":100000000,"Module":"TooString.Specs.dll","IsSecurityCritical":true,"IsSecuritySafeCritical":false,"IsSecurityTransparent":false,"MethodHandle":"System.RuntimeMethodHandle","Attributes":"PrivateScope, Private, Static, HideBySig","CallingConvention":"Standard","ReturnType":"System.Void","ReturnTypeCustomAttributes":"Void","ReturnParameter":"Void","IsCollectible":false,"IsGenericMethod":false,"IsGenericMethodDefinition":false,"ContainsGenericParameters":false,"MethodImplementationFlags":"Managed","IsAbstract":false,"IsConstructor":false,"IsFinal":false,"IsHideBySig":true,"IsSpecialName":false,"IsStatic":true,"IsVirtual":false,"IsAssembly":false,"IsFamily":false,"IsFamilyAndAssembly":false,"IsFamilyOrAssembly":false,"IsPrivate":true,"IsPublic":false,"IsConstructedGenericMethod":false,"CustomAttributes":[]},"Position":-1,"IsIn":false,"IsLcid":false,"IsOptional":false,"IsOut":false,"IsRetval":false,"CustomAttributes":{"Length":0,"LongLength":0,"Rank":1,"SyncRoot":[],"IsReadOnly":false,"IsFixedSize":true,"IsSynchronized":false}},"ReturnParameter":{"ParameterType":"System.Void","Name":"null","HasDefaultValue":true,"DefaultValue":"null","RawDefaultValue":"null","MetadataToken":100000000,"Attributes":"None","Member":{"Name":"Main","DeclaringType":"AutoGeneratedProgram","ReflectedType":"AutoGeneratedProgram","MemberType":"Method","MetadataToken":100000000,"Module":"TooString.Specs.dll","IsSecurityCritical":true,"IsSecuritySafeCritical":false,"IsSecurityTransparent":false,"MethodHandle":"System.RuntimeMethodHandle","Attributes":"PrivateScope, Private, Static, HideBySig","CallingConvention":"Standard","ReturnType":"System.Void","ReturnTypeCustomAttributes":"Void","ReturnParameter":"Void","IsCollectible":false,"IsGenericMethod":false,"IsGenericMethodDefinition":false,"ContainsGenericParameters":false,"MethodImplementationFlags":"Managed","IsAbstract":false,"IsConstructor":false,"IsFinal":false,"IsHideBySig":true,"IsSpecialName":false,"IsStatic":true,"IsVirtual":false,"IsAssembly":false,"IsFamily":false,"IsFamilyAndAssembly":false,"IsFamilyOrAssembly":false,"IsPrivate":true,"IsPublic":false,"IsConstructedGenericMethod":false,"CustomAttributes":[]},"Position":-1,"IsIn":false,"IsLcid":false,"IsOptional":false,"IsOut":false,"IsRetval":false,"CustomAttributes":{"Length":0,"LongLength":0,"Rank":1,"SyncRoot":[],"IsReadOnly":false,"IsFixedSize":true,"IsSynchronized":false}},"IsCollectible":false,"IsGenericMethod":false,"IsGenericMethodDefinition":false,"ContainsGenericParameters":false,"MethodImplementationFlags":"Managed","IsAbstract":false,"IsConstructor":false,"IsFinal":false,"IsHideBySig":true,"IsSpecialName":false,"IsStatic":true,"IsVirtual":false,"IsAssembly":false,"IsFamily":false,"IsFamilyAndAssembly":false,"IsFamilyOrAssembly":false,"IsPrivate":true,"IsPublic":false,"IsConstructedGenericMethod":false,"CustomAttributes":{"Length":0,"LongLength":0,"Rank":1,"SyncRoot":{"Length":0,"LongLength":0,"Rank":1,"SyncRoot":[],"IsReadOnly":false,"IsFixedSize":true,"IsSynchronized":false},"IsReadOnly":false,"IsFixedSize":true,"IsSynchronized":false}},"DefinedTypes":{"Length":21,"LongLength":21,"Rank":1,"SyncRoot":{"Length":21,"LongLength":21,"Rank":1,"SyncRoot":{"Length":21,"LongLength":21,"Rank":1,"SyncRoot":[],"IsReadOnly":false,"IsFixedSize":true,"IsSynchronized":false},"IsReadOnly":false,"IsFixedSize":true,"IsSynchronized":false},"IsReadOnly":false,"IsFixedSize":true,"IsSynchronized":false},"IsCollectible":false,"ManifestModule":{"MDStreamVersion":131072,"FullyQualifiedName":"--filename--","ModuleVersionId":{},"MetadataToken":100000000,"ScopeName":"TooString.Specs.dll","Name":"TooString.Specs.dll","Assembly":{"CodeBase":"file:///--filename--","FullName":"TooString.Specs, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null","EntryPoint":{"Name":"Main","DeclaringType":"AutoGeneratedProgram","ReflectedType":"AutoGeneratedProgram","MemberType":"Method","MetadataToken":100000000,"Module":"TooString.Specs.dll","IsSecurityCritical":true,"IsSecuritySafeCritical":false,"IsSecurityTransparent":false,"MethodHandle":"System.RuntimeMethodHandle","Attributes":"PrivateScope, Private, Static, HideBySig","CallingConvention":"Standard","ReturnType":"System.Void","ReturnTypeCustomAttributes":"Void","ReturnParameter":"Void","IsCollectible":false,"IsGenericMethod":false,"IsGenericMethodDefinition":false,"ContainsGenericParameters":false,"MethodImplementationFlags":"Managed","IsAbstract":false,"IsConstructor":false,"IsFinal":false,"IsHideBySig":true,"IsSpecialName":false,"IsStatic":true,"IsVirtual":false,"IsAssembly":false,"IsFamily":false,"IsFamilyAndAssembly":false,"IsFamilyOrAssembly":false,"IsPrivate":true,"IsPublic":false,"IsConstructedGenericMethod":false,"CustomAttributes":[]},"DefinedTypes":{"Length":21,"LongLength":21,"Rank":1,"SyncRoot":[],"IsReadOnly":false,"IsFixedSize":true,"IsSynchronized":false},"IsCollectible":false,"ManifestModule":{"MDStreamVersion":131072,"FullyQualifiedName":"--filename--","ModuleVersionId":"00000000-0000-0000-0000-000000000000","MetadataToken":100000000,"ScopeName":"TooString.Specs.dll","Name":"TooString.Specs.dll","Assembly":"TooString.Specs, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null","ModuleHandle":"System.ModuleHandle","CustomAttributes":"System.Collections.ObjectModel.ReadOnlyCollection\u00601[System.Reflection.CustomAttributeData]"},"ReflectionOnly":false,"Location":"--filename--","ImageRuntimeVersion":"v4.0.30319","GlobalAssemblyCache":false,"HostContext":0,"IsDynamic":false,"ExportedTypes":{"Length":5,"LongLength":5,"Rank":1,"SyncRoot":[],"IsReadOnly":false,"IsFixedSize":true,"IsSynchronized":false},"IsFullyTrusted":true,"CustomAttributes":{"Count":10,"Item":"CustomAttributeData"},"EscapedCodeBase":"file:///--filename--","Modules":{"Length":1,"LongLength":1,"Rank":1,"SyncRoot":[],"IsReadOnly":false,"IsFixedSize":true,"IsSynchronized":false},"SecurityRuleSet":"None"},"ModuleHandle":{"MDStreamVersion":131072},"CustomAttributes":{"Count":1,"Item":"CustomAttributeData"}},"ReflectionOnly":false,"Location":"--filename--","ImageRuntimeVersion":"v4.0.30319","GlobalAssemblyCache":false,"HostContext":0,"IsDynamic":false,"ExportedTypes":{"Length":5,"LongLength":5,"Rank":1,"SyncRoot":{"Length":5,"LongLength":5,"Rank":1,"SyncRoot":{"Length":5,"LongLength":5,"Rank":1,"SyncRoot":[],"IsReadOnly":false,"IsFixedSize":true,"IsSynchronized":false},"IsReadOnly":false,"IsFixedSize":true,"IsSynchronized":false},"IsReadOnly":false,"IsFixedSize":true,"IsSynchronized":false},"IsFullyTrusted":true,"CustomAttributes":{"Count":10,"Item":"CustomAttributeData"},"EscapedCodeBase":"file:///--filename--","Modules":{"Length":1,"LongLength":1,"Rank":1,"SyncRoot":{"Length":1,"LongLength":1,"Rank":1,"SyncRoot":{"Length":1,"LongLength":1,"Rank":1,"SyncRoot":[],"IsReadOnly":false,"IsFixedSize":true,"IsSynchronized":false},"IsReadOnly":false,"IsFixedSize":true,"IsSynchronized":false},"IsReadOnly":false,"IsFixedSize":true,"IsSynchronized":false},"SecurityRuleSet":"None"}
                       """;
        var actual = value.TooString(TooStringHow.Json);

        TestContext.Progress.WriteLine(actual);
        TestContext.Progress.WriteLine(actual.RegexReplaceKnownRuntimeVariableValues());
        
        Assert.That(
            value
                .TooString(TooStringHow.Json)
                .RegexReplaceKnownRuntimeVariableValues(),
            
            Is.EqualTo(expected.RegexReplaceKnownRuntimeVariableValues())
        );

        Assert.That(
            System.Text.Json.JsonSerializer.Deserialize<ExpandoObject>(actual)
                .TooString(TooStringHow.Json)  
                .RegexReplaceKnownRuntimeVariableValues()
            ,
            Is.EqualTo(expected.RegexReplaceKnownRuntimeVariableValues())
            );
    }


    [Test]
    public void GivenAnUnJsonableReflectionType_Module()
    {
        var value = typeof(Type)
            .GetMethods(BindingFlags.Instance|BindingFlags.Public)
            .First(m=>m.Name=="GetMethods")
            .Module;

        var expected = """
                       {"MDStreamVersion":131072,"FullyQualifiedName":"--filename--","ModuleVersionId":{},"MetadataToken":100000000,"ScopeName":"System.Private.CoreLib.dll","Name":"System.Private.CoreLib.dll","Assembly":{"CodeBase":"file:///--filename--","FullName":"System.Private.CoreLib, Version=6.0.0.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e","EntryPoint":"null","DefinedTypes":{"Length":2240,"LongLength":2240,"Rank":1,"SyncRoot":{"Length":2240,"LongLength":2240,"Rank":1,"SyncRoot":[],"IsReadOnly":false,"IsFixedSize":true,"IsSynchronized":false},"IsReadOnly":false,"IsFixedSize":true,"IsSynchronized":false},"IsCollectible":false,"ManifestModule":{"MDStreamVersion":131072,"FullyQualifiedName":"--filename--","ModuleVersionId":{},"MetadataToken":100000000,"ScopeName":"System.Private.CoreLib.dll","Name":"System.Private.CoreLib.dll","Assembly":{"CodeBase":"file:///--filename--","FullName":"System.Private.CoreLib, Version=6.0.0.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e","EntryPoint":"null","DefinedTypes":[],"IsCollectible":false,"ManifestModule":"System.Private.CoreLib.dll","ReflectionOnly":false,"Location":"--filename--","ImageRuntimeVersion":"v4.0.30319","GlobalAssemblyCache":false,"HostContext":0,"IsDynamic":false,"ExportedTypes":[],"IsFullyTrusted":true,"CustomAttributes":"System.Collections.ObjectModel.ReadOnlyCollection\u00601[System.Reflection.CustomAttributeData]","EscapedCodeBase":"file:///--filename--","Modules":[],"SecurityRuleSet":"None"},"ModuleHandle":{"MDStreamVersion":131072},"CustomAttributes":{"Count":2,"Item":"CustomAttributeData"}},"ReflectionOnly":false,"Location":"--filename--","ImageRuntimeVersion":"v4.0.30319","GlobalAssemblyCache":false,"HostContext":0,"IsDynamic":false,"ExportedTypes":{"Length":1187,"LongLength":1187,"Rank":1,"SyncRoot":{"Length":1187,"LongLength":1187,"Rank":1,"SyncRoot":[],"IsReadOnly":false,"IsFixedSize":true,"IsSynchronized":false},"IsReadOnly":false,"IsFixedSize":true,"IsSynchronized":false},"IsFullyTrusted":true,"CustomAttributes":{"Count":22,"Item":"CustomAttributeData"},"EscapedCodeBase":"file:///--filename--","Modules":{"Length":1,"LongLength":1,"Rank":1,"SyncRoot":{"Length":1,"LongLength":1,"Rank":1,"SyncRoot":[],"IsReadOnly":false,"IsFixedSize":true,"IsSynchronized":false},"IsReadOnly":false,"IsFixedSize":true,"IsSynchronized":false},"SecurityRuleSet":"None"},"ModuleHandle":{"MDStreamVersion":131072},"CustomAttributes":{"Count":2,"Item":"CustomAttributeData"}}
                       """;
        
        var actual = value.TooString(TooStringHow.Json);

        TestContext.Progress.WriteLine(actual);
        TestContext.Progress.WriteLine(actual.RegexReplaceKnownRuntimeVariableValues());

        Assert.That(
            value.TooString(TooStringHow.Json)
                .RegexReplaceKnownRuntimeVariableValues(),
            Is.EqualTo(
                expected.RegexReplaceKnownRuntimeVariableValues()));

        Assert.That(
            System.Text.Json.JsonSerializer.Deserialize<ExpandoObject>(actual)
                .TooString(TooStringHow.Json)
                .RegexReplaceKnownRuntimeVariableValues(),
            Is.EqualTo(
                expected.RegexReplaceKnownRuntimeVariableValues()));
    }
    
}

class CompositeA
{
    public string A { get; set; } public Complex B { get; set; }

    public override string ToString() => new { A, B }.ToString();
}

class Circular { public string A { get; set; } public Circular B { get; set; } }