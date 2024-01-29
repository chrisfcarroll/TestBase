using System.Dynamic;
using System.Numerics;
using System.Reflection;

namespace TooString.Specs;

[TestFixture]
public class TooStringBestEffortMakesGoodChoices
{
    [TestCase(null, "null")]
    [TestCase("boo", "\"boo\"")]
    [TestCase(1, "1")]
    [TestCase(1.2f, "1.2")]
    [TestCase(1.2d, "1.2")]
    public void GivenAScalar__ReturnsJson(object value, string expected)
    {
        Assert.That(value.TooString(), Is.EqualTo(expected));
        
        TestContext.Progress.WriteLine(value.TooString());
    }
    
    [Test]
    public void GivenACompositeObject__ReturnsJson()
    {
        var value = new CompositeA { A = "boo", B = new Complex(3,4) };
        var expected = 
            "{\"A\":\"boo\",\"B\":{\"Real\":3,\"Imaginary\":4,\"Magnitude\":5,\"Phase\":0.9272952180016122}}";
        
        Assert.That(
            value.TooString(), 
            Is.EqualTo(expected) 
        );
        TestContext.Progress.WriteLine(value.TooString());
    }
    
    [Test]
    public void GivenACompositeObjectWithCircularReferences__ReturnsReflectedJson()
    {
        var value = new Circular{ A = "boo"};
        value.B = value;
        var expected = 
            "{\"A\":\"boo\",\"B\":null}";
        
        Assert.That(
            value.TooString(), 
            Is.EqualTo(expected) 
        );
        TestContext.Progress.WriteLine(value.TooString());
    }
    
    [Test,Ignore("Don't create HttpClient on each run")]
    public void GivenDifficultObject__ReturnsReflectedJson()
    {
        var value = TooStringJsonReturnsJson.httpClient;
        var expected = 
            "{\"DefaultRequestHeaders\":[],\"DefaultRequestVersion\":\"1.1\"," +
            "\"DefaultVersionPolicy\":0,\"BaseAddress\":\"http://127.0.0.1\"," +
            "\"Timeout\":\"00:01:40\",\"MaxResponseContentBufferSize\":2147483647}";
        
        Assert.That(
            value.TooString(), 
            Is.EqualTo(expected) 
        );
        TestContext.Progress.WriteLine(value.TooString());
    }
    
    [Test]
    public void GivenAnUnJsonableReflectionType__ReturnsReflectedJson()
    {
        var value = typeof(Type)
            .GetMethods(BindingFlags.Instance|BindingFlags.Public)
            .First(m=>m.Name=="GetMethods");

        var expected = """
                       {"Name":"GetMethods","DeclaringType":"System.Type","ReflectedType":"System.Type","MemberType":{},"MetadataToken":"100665554","Module":{"MDStreamVersion":"131072","FullyQualifiedName":"C:\\Program Files\\dotnet\\shared\\Microsoft.NETCore.App\\6.0.26\\System.Private.CoreLib.dll","ModuleVersionId":{},"MetadataToken":"1","ScopeName":"System.Private.CoreLib.dll","Name":"System.Private.CoreLib.dll","Assembly":{"CodeBase":"file:///C:/Program Files/dotnet/shared/Microsoft.NETCore.App/6.0.26/System.Private.CoreLib.dll","FullName":"System.Private.CoreLib, Version=6.0.0.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e","EntryPoint":"null","DefinedTypes":[],"IsCollectible":"False","ManifestModule":"System.Private.CoreLib.dll","ReflectionOnly":"False","Location":"C:\\Program Files\\dotnet\\shared\\Microsoft.NETCore.App\\6.0.26\\System.Private.CoreLib.dll","ImageRuntimeVersion":"v4.0.30319","GlobalAssemblyCache":"False","HostContext":"0","IsDynamic":"False","ExportedTypes":[],"IsFullyTrusted":"True","CustomAttributes":"System.Collections.ObjectModel.ReadOnlyCollection\u00601[System.Reflection.CustomAttributeData]","EscapedCodeBase":"file:///C:/Program%20Files/dotnet/shared/Microsoft.NETCore.App/6.0.26/System.Private.CoreLib.dll","Modules":[],"SecurityRuleSet":"None"},"ModuleHandle":{"MDStreamVersion":"131072"},"CustomAttributes":{"Count":"2","Item":"cantretrievevalue"}},"IsSecurityCritical":"True","IsSecuritySafeCritical":"False","IsSecurityTransparent":"False","MethodHandle":{"Value":"140716294633360"},"Attributes":{},"CallingConvention":{},"ReturnType":"System.Reflection.MethodInfo[]","ReturnTypeCustomAttributes":{"ParameterType":"System.Reflection.MethodInfo[]","Name":"null","HasDefaultValue":"True","DefaultValue":"null","RawDefaultValue":"null","MetadataToken":"134217728","Attributes":{},"Member":{"Name":"GetMethods","DeclaringType":"System.Type","ReflectedType":"System.Type","MemberType":"Method","MetadataToken":"100665554","Module":"System.Private.CoreLib.dll","IsSecurityCritical":"True","IsSecuritySafeCritical":"False","IsSecurityTransparent":"False","MethodHandle":"System.RuntimeMethodHandle","Attributes":"PrivateScope, Public, HideBySig","CallingConvention":"Standard, HasThis","ReturnType":"System.Reflection.MethodInfo[]","ReturnTypeCustomAttributes":"System.Reflection.MethodInfo[]","ReturnParameter":"System.Reflection.MethodInfo[]","IsCollectible":"False","IsGenericMethod":"False","IsGenericMethodDefinition":"False","ContainsGenericParameters":"False","MethodImplementationFlags":"Managed","IsAbstract":"False","IsConstructor":"False","IsFinal":"False","IsHideBySig":"True","IsSpecialName":"False","IsStatic":"False","IsVirtual":"False","IsAssembly":"False","IsFamily":"False","IsFamilyAndAssembly":"False","IsFamilyOrAssembly":"False","IsPrivate":"False","IsPublic":"True","IsConstructedGenericMethod":"False","CustomAttributes":"System.Collections.ObjectModel.ReadOnlyCollection\u00601[System.Reflection.CustomAttributeData]"},"Position":"-1","IsIn":"False","IsLcid":"False","IsOptional":"False","IsOut":"False","IsRetval":"False","CustomAttributes":{"Length":"0","LongLength":"0","Rank":"1","SyncRoot":[],"IsReadOnly":"False","IsFixedSize":"True","IsSynchronized":"False"}},"ReturnParameter":{"ParameterType":"System.Reflection.MethodInfo[]","Name":"null","HasDefaultValue":"True","DefaultValue":"null","RawDefaultValue":"null","MetadataToken":"134217728","Attributes":{},"Member":{"Name":"GetMethods","DeclaringType":"System.Type","ReflectedType":"System.Type","MemberType":"Method","MetadataToken":"100665554","Module":"System.Private.CoreLib.dll","IsSecurityCritical":"True","IsSecuritySafeCritical":"False","IsSecurityTransparent":"False","MethodHandle":"System.RuntimeMethodHandle","Attributes":"PrivateScope, Public, HideBySig","CallingConvention":"Standard, HasThis","ReturnType":"System.Reflection.MethodInfo[]","ReturnTypeCustomAttributes":"System.Reflection.MethodInfo[]","ReturnParameter":"System.Reflection.MethodInfo[]","IsCollectible":"False","IsGenericMethod":"False","IsGenericMethodDefinition":"False","ContainsGenericParameters":"False","MethodImplementationFlags":"Managed","IsAbstract":"False","IsConstructor":"False","IsFinal":"False","IsHideBySig":"True","IsSpecialName":"False","IsStatic":"False","IsVirtual":"False","IsAssembly":"False","IsFamily":"False","IsFamilyAndAssembly":"False","IsFamilyOrAssembly":"False","IsPrivate":"False","IsPublic":"True","IsConstructedGenericMethod":"False","CustomAttributes":"System.Collections.ObjectModel.ReadOnlyCollection\u00601[System.Reflection.CustomAttributeData]"},"Position":"-1","IsIn":"False","IsLcid":"False","IsOptional":"False","IsOut":"False","IsRetval":"False","CustomAttributes":{"Length":"0","LongLength":"0","Rank":"1","SyncRoot":[],"IsReadOnly":"False","IsFixedSize":"True","IsSynchronized":"False"}},"IsCollectible":"False","IsGenericMethod":"False","IsGenericMethodDefinition":"False","ContainsGenericParameters":"False","MethodImplementationFlags":{},"IsAbstract":"False","IsConstructor":"False","IsFinal":"False","IsHideBySig":"True","IsSpecialName":"False","IsStatic":"False","IsVirtual":"False","IsAssembly":"False","IsFamily":"False","IsFamilyAndAssembly":"False","IsFamilyOrAssembly":"False","IsPrivate":"False","IsPublic":"True","IsConstructedGenericMethod":"False","CustomAttributes":{"Count":"1","Item":"cantretrievevalue"}}
                       """;

        var actual = value.TooString();

        TestContext.Progress.WriteLine(actual);
        
        Assert.That(
            value
                .TooString(TooStringMethod.SystemTextJson)
                .RegexReplaceKnownRuntimeVariableValues(), 
            Is.EqualTo(
                expected.RegexReplaceKnownRuntimeVariableValues()));
        
        TestContext.Progress.WriteLine(value.TooString());

        Assert.That(
            System.Text.Json.JsonSerializer.Deserialize<ExpandoObject>(actual)
                .TooString(TooStringMethod.SystemTextJson)
                .RegexReplaceKnownRuntimeVariableValues(),
            
            Is.EqualTo(
                expected.RegexReplaceKnownRuntimeVariableValues()));
    }
}
