using System.Dynamic;
using System.Numerics;
using System.Reflection;

namespace TooString.Specs;

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
        Assert.That(value.TooString(TooStringMethod.SystemTextJson ), Is.EqualTo(expected));
    }
    
    [Test]
    public void GivenACompositeObject()
    {
        var value = new CompositeA { A = "boo", B = new Complex(3,4) };
        var expected = 
            "{\"A\":\"boo\",\"B\":{\"Real\":3,\"Imaginary\":4,\"Magnitude\":5,\"Phase\":0.9272952180016122}}";
        
        Assert.That(
            value.TooString(TooStringMethod.SystemTextJson), 
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
            value.TooString(TooStringMethod.SystemTextJson), 
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
            value.TooString(TooStringMethod.SystemTextJson), 
            Is.EqualTo(expected) 
        );
    }
    
    [Test]
    public void GivenAnUnJsonableReflectionType_Assembly()
    {
        var value = Assembly.GetExecutingAssembly();

        var expected = """
                       {"CodeBase":"file:///C:/Users/itsccarr/repos/3rd/TestBase/TooString.Specs/bin/Debug/net6.0/TooString.Specs.dll","FullName":"TooString.Specs, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null","EntryPoint":{"Name":"Main","DeclaringType":"AutoGeneratedProgram","ReflectedType":"AutoGeneratedProgram","MemberType":{},"MetadataToken":"100663302","Module":{"MDStreamVersion":"131072","FullyQualifiedName":"C:\\Users\\itsccarr\\repos\\3rd\\TestBase\\TooString.Specs\\bin\\Debug\\net6.0\\TooString.Specs.dll","ModuleVersionId":"935778f0-60b3-430e-888a-34a7d5e0c3ca","MetadataToken":"1","ScopeName":"TooString.Specs.dll","Name":"TooString.Specs.dll","Assembly":"TooString.Specs, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null","ModuleHandle":"System.ModuleHandle","CustomAttributes":"System.Collections.ObjectModel.ReadOnlyCollection\\u00601[System.Reflection.CustomAttributeData]"},"IsSecurityCritical":"True","IsSecuritySafeCritical":"False","IsSecurityTransparent":"False","MethodHandle":{"Value":"140716304582424"},"Attributes":{},"CallingConvention":{},"ReturnType":"System.Void","ReturnTypeCustomAttributes":{"ParameterType":"System.Void","Name":"null","HasDefaultValue":"True","DefaultValue":"null","RawDefaultValue":"null","MetadataToken":"134217728","Attributes":"None","Member":"Void Main(System.String[])","Position":"-1","IsIn":"False","IsLcid":"False","IsOptional":"False","IsOut":"False","IsRetval":"False","CustomAttributes":[]},"ReturnParameter":{"ParameterType":"System.Void","Name":"null","HasDefaultValue":"True","DefaultValue":"null","RawDefaultValue":"null","MetadataToken":"134217728","Attributes":"None","Member":"Void Main(System.String[])","Position":"-1","IsIn":"False","IsLcid":"False","IsOptional":"False","IsOut":"False","IsRetval":"False","CustomAttributes":[]},"IsCollectible":"False","IsGenericMethod":"False","IsGenericMethodDefinition":"False","ContainsGenericParameters":"False","MethodImplementationFlags":{},"IsAbstract":"False","IsConstructor":"False","IsFinal":"False","IsHideBySig":"True","IsSpecialName":"False","IsStatic":"True","IsVirtual":"False","IsAssembly":"False","IsFamily":"False","IsFamilyAndAssembly":"False","IsFamilyOrAssembly":"False","IsPrivate":"True","IsPublic":"False","IsConstructedGenericMethod":"False","CustomAttributes":{"Length":"0","LongLength":"0","Rank":"1","SyncRoot":[],"IsReadOnly":"False","IsFixedSize":"True","IsSynchronized":"False"}},"DefinedTypes":{"Length":"15","LongLength":"15","Rank":"1","SyncRoot":{"Length":"15","LongLength":"15","Rank":"1","SyncRoot":[],"IsReadOnly":"False","IsFixedSize":"True","IsSynchronized":"False"},"IsReadOnly":"False","IsFixedSize":"True","IsSynchronized":"False"},"IsCollectible":"False","ManifestModule":{"MDStreamVersion":"131072","FullyQualifiedName":"C:\\Users\\itsccarr\\repos\\3rd\\TestBase\\TooString.Specs\\bin\\Debug\\net6.0\\TooString.Specs.dll","ModuleVersionId":{},"MetadataToken":"1","ScopeName":"TooString.Specs.dll","Name":"TooString.Specs.dll","Assembly":{"CodeBase":"file:///C:/Users/itsccarr/repos/3rd/TestBase/TooString.Specs/bin/Debug/net6.0/TooString.Specs.dll","FullName":"TooString.Specs, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null","EntryPoint":"Void Main(System.String[])","DefinedTypes":[],"IsCollectible":"False","ManifestModule":"TooString.Specs.dll","ReflectionOnly":"False","Location":"C:\\Users\\itsccarr\\repos\\3rd\\TestBase\\TooString.Specs\\bin\\Debug\\net6.0\\TooString.Specs.dll","ImageRuntimeVersion":"v4.0.30319","GlobalAssemblyCache":"False","HostContext":"0","IsDynamic":"False","ExportedTypes":[],"IsFullyTrusted":"True","CustomAttributes":"System.Collections.ObjectModel.ReadOnlyCollection\\u00601[System.Reflection.CustomAttributeData]","EscapedCodeBase":"file:///C:/Users/itsccarr/repos/3rd/TestBase/TooString.Specs/bin/Debug/net6.0/TooString.Specs.dll","Modules":[],"SecurityRuleSet":"None"},"ModuleHandle":{"MDStreamVersion":"131072"},"CustomAttributes":{"Count":"1","Item":"cantretrievevalue"}},"ReflectionOnly":"False","Location":"C:\\Users\\itsccarr\\repos\\3rd\\TestBase\\TooString.Specs\\bin\\Debug\\net6.0\\TooString.Specs.dll","ImageRuntimeVersion":"v4.0.30319","GlobalAssemblyCache":"False","HostContext":"0","IsDynamic":"False","ExportedTypes":{"Length":"3","LongLength":"3","Rank":"1","SyncRoot":{"Length":"3","LongLength":"3","Rank":"1","SyncRoot":[],"IsReadOnly":"False","IsFixedSize":"True","IsSynchronized":"False"},"IsReadOnly":"False","IsFixedSize":"True","IsSynchronized":"False"},"IsFullyTrusted":"True","CustomAttributes":{"Count":"10","Item":"cantretrievevalue"},"EscapedCodeBase":"file:///C:/Users/itsccarr/repos/3rd/TestBase/TooString.Specs/bin/Debug/net6.0/TooString.Specs.dll","Modules":{"Length":"1","LongLength":"1","Rank":"1","SyncRoot":{"Length":"1","LongLength":"1","Rank":"1","SyncRoot":[],"IsReadOnly":"False","IsFixedSize":"True","IsSynchronized":"False"},"IsReadOnly":"False","IsFixedSize":"True","IsSynchronized":"False"},"SecurityRuleSet":{}}
                       """;
        var actual = value.TooString(TooStringMethod.SystemTextJson);

        TestContext.Progress.WriteLine(actual);
        
        Assert.That(
            value
                .TooString(TooStringMethod.SystemTextJson)
                .RegexReplaceKnownRuntimeVariableValues(),
            
            Is.EqualTo(expected.RegexReplaceKnownRuntimeVariableValues())
        );

        Assert.That(
            System.Text.Json.JsonSerializer.Deserialize<ExpandoObject>(actual)
                .TooString(TooStringMethod.SystemTextJson)  
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
                       {"MDStreamVersion":"131072","FullyQualifiedName":"C:\\Program Files\\dotnet\\shared\\Microsoft.NETCore.App\\6.0.26\\System.Private.CoreLib.dll","ModuleVersionId":{},"MetadataToken":"1","ScopeName":"System.Private.CoreLib.dll","Name":"System.Private.CoreLib.dll","Assembly":{"CodeBase":"file:///C:/Program Files/dotnet/shared/Microsoft.NETCore.App/6.0.26/System.Private.CoreLib.dll","FullName":"System.Private.CoreLib, Version=6.0.0.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e","EntryPoint":"null","DefinedTypes":{"Length":"2341","LongLength":"2341","Rank":"1","SyncRoot":[],"IsReadOnly":"False","IsFixedSize":"True","IsSynchronized":"False"},"IsCollectible":"False","ManifestModule":{"MDStreamVersion":"131072","FullyQualifiedName":"C:\\Program Files\\dotnet\\shared\\Microsoft.NETCore.App\\6.0.26\\System.Private.CoreLib.dll","ModuleVersionId":"b756d57b-94fb-4ab9-aa95-a9b2a97fce1f","MetadataToken":"1","ScopeName":"System.Private.CoreLib.dll","Name":"System.Private.CoreLib.dll","Assembly":"System.Private.CoreLib, Version=6.0.0.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e","ModuleHandle":"System.ModuleHandle","CustomAttributes":"System.Collections.ObjectModel.ReadOnlyCollection\\u00601[System.Reflection.CustomAttributeData]"},"ReflectionOnly":"False","Location":"C:\\Program Files\\dotnet\\shared\\Microsoft.NETCore.App\\6.0.26\\System.Private.CoreLib.dll","ImageRuntimeVersion":"v4.0.30319","GlobalAssemblyCache":"False","HostContext":"0","IsDynamic":"False","ExportedTypes":{"Length":"1186","LongLength":"1186","Rank":"1","SyncRoot":[],"IsReadOnly":"False","IsFixedSize":"True","IsSynchronized":"False"},"IsFullyTrusted":"True","CustomAttributes":{"Count":"22","Item":"cantretrievevalue"},"EscapedCodeBase":"file:///C:/Program%20Files/dotnet/shared/Microsoft.NETCore.App/6.0.26/System.Private.CoreLib.dll","Modules":{"Length":"1","LongLength":"1","Rank":"1","SyncRoot":[],"IsReadOnly":"False","IsFixedSize":"True","IsSynchronized":"False"},"SecurityRuleSet":{}},"ModuleHandle":{"MDStreamVersion":"131072"},"CustomAttributes":{"Count":"2","Item":"cantretrievevalue"}}
                       """;
        
        var actual = value.TooString(TooStringMethod.SystemTextJson);

        TestContext.Progress.WriteLine(actual);

        Assert.That(
            value.TooString(TooStringMethod.SystemTextJson)
                .RegexReplaceKnownRuntimeVariableValues(),
            Is.EqualTo(
                expected.RegexReplaceKnownRuntimeVariableValues()));

        Assert.That(
            System.Text.Json.JsonSerializer.Deserialize<ExpandoObject>(actual)
                .TooString(TooStringMethod.SystemTextJson)
                .RegexReplaceKnownRuntimeVariableValues(),
            Is.EqualTo(
                expected.RegexReplaceKnownRuntimeVariableValues()));
    }
    
}

class CompositeA { public string A { get; set; } public Complex B { get; set; } }

class Circular { public string A { get; set; } public Circular B { get; set; } }