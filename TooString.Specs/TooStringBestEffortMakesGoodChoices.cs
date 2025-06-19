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
    public void GivenACompositeObjectAndDefaultOptions__ReturnsJson()
    {
        var value = new CompositeA { A = "boo", B = new Complex(3,4) };
        var expected = 
            "{\"A\":\"boo\",\"B\":{\"Real\":3,\"Imaginary\":4,\"Magnitude\":5,\"Phase\":0.9272952180016122}}";
        
        Assert.That(
            value.TooString(TooStringOptions.Default), 
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
                       {"Name":"GetMethods","DeclaringType":"System.Type","ReflectedType":"System.Type","MemberType":"Method","MetadataToken":100000000,"Module":{"MDStreamVersion":131072,"FullyQualifiedName":"--filename--","ModuleVersionId":{},"MetadataToken":100000000,"ScopeName":"System.Private.CoreLib.dll","Name":"System.Private.CoreLib.dll","Assembly":{"CodeBase":"file:///--filename--","FullName":"System.Private.CoreLib, Version=6.0.0.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e","EntryPoint":"null","DefinedTypes":{"Length":2240,"LongLength":2240,"Rank":1,"SyncRoot":[],"IsReadOnly":false,"IsFixedSize":true,"IsSynchronized":false},"IsCollectible":false,"ManifestModule":{"MDStreamVersion":131072,"FullyQualifiedName":"--filename--","ModuleVersionId":"00000000-0000-0000-0000-000000000000","MetadataToken":100000000,"ScopeName":"System.Private.CoreLib.dll","Name":"System.Private.CoreLib.dll","Assembly":"System.Private.CoreLib, Version=6.0.0.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e","ModuleHandle":"System.ModuleHandle","CustomAttributes":"System.Collections.ObjectModel.ReadOnlyCollection\u00601[System.Reflection.CustomAttributeData]"},"ReflectionOnly":false,"Location":"--filename--","ImageRuntimeVersion":"v4.0.30319","GlobalAssemblyCache":false,"HostContext":0,"IsDynamic":false,"ExportedTypes":{"Length":1187,"LongLength":1187,"Rank":1,"SyncRoot":[],"IsReadOnly":false,"IsFixedSize":true,"IsSynchronized":false},"IsFullyTrusted":true,"CustomAttributes":{"Count":22,"Item":"CustomAttributeData"},"EscapedCodeBase":"file:///--filename--","Modules":{"Length":1,"LongLength":1,"Rank":1,"SyncRoot":[],"IsReadOnly":false,"IsFixedSize":true,"IsSynchronized":false},"SecurityRuleSet":"None"},"ModuleHandle":{"MDStreamVersion":131072},"CustomAttributes":{"Count":2,"Item":"CustomAttributeData"}},"IsSecurityCritical":true,"IsSecuritySafeCritical":false,"IsSecurityTransparent":false,"MethodHandle":{"Value":9999999999},"Attributes":"PrivateScope, Public, HideBySig","CallingConvention":"Standard, HasThis","ReturnType":[],"ReturnTypeCustomAttributes":{"ParameterType":[],"Name":"null","HasDefaultValue":true,"DefaultValue":"null","RawDefaultValue":"null","MetadataToken":100000000,"Attributes":"None","Member":{"Name":"GetMethods","DeclaringType":"System.Type","ReflectedType":"System.Type","MemberType":"Method","MetadataToken":100000000,"Module":{"MDStreamVersion":131072,"FullyQualifiedName":"--filename--","ModuleVersionId":"00000000-0000-0000-0000-000000000000","MetadataToken":100000000,"ScopeName":"System.Private.CoreLib.dll","Name":"System.Private.CoreLib.dll","Assembly":"System.Private.CoreLib, Version=6.0.0.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e","ModuleHandle":"System.ModuleHandle","CustomAttributes":"System.Collections.ObjectModel.ReadOnlyCollection\u00601[System.Reflection.CustomAttributeData]"},"IsSecurityCritical":true,"IsSecuritySafeCritical":false,"IsSecurityTransparent":false,"MethodHandle":{"Value":9999999999},"Attributes":"PrivateScope, Public, HideBySig","CallingConvention":"Standard, HasThis","ReturnType":[],"ReturnTypeCustomAttributes":{"ParameterType":[],"Name":"null","HasDefaultValue":true,"DefaultValue":"null","RawDefaultValue":"null","MetadataToken":100000000,"Attributes":"None","Member":"System.Reflection.MethodInfo[] GetMethods()","Position":-1,"IsIn":false,"IsLcid":false,"IsOptional":false,"IsOut":false,"IsRetval":false,"CustomAttributes":[]},"ReturnParameter":{"ParameterType":[],"Name":"null","HasDefaultValue":true,"DefaultValue":"null","RawDefaultValue":"null","MetadataToken":100000000,"Attributes":"None","Member":"System.Reflection.MethodInfo[] GetMethods()","Position":-1,"IsIn":false,"IsLcid":false,"IsOptional":false,"IsOut":false,"IsRetval":false,"CustomAttributes":[]},"IsCollectible":false,"IsGenericMethod":false,"IsGenericMethodDefinition":false,"ContainsGenericParameters":false,"MethodImplementationFlags":"Managed","IsAbstract":false,"IsConstructor":false,"IsFinal":false,"IsHideBySig":true,"IsSpecialName":false,"IsStatic":false,"IsVirtual":false,"IsAssembly":false,"IsFamily":false,"IsFamilyAndAssembly":false,"IsFamilyOrAssembly":false,"IsPrivate":false,"IsPublic":true,"IsConstructedGenericMethod":false,"CustomAttributes":{"Count":1,"Item":"CustomAttributeData"}},"Position":-1,"IsIn":false,"IsLcid":false,"IsOptional":false,"IsOut":false,"IsRetval":false,"CustomAttributes":{"Length":0,"LongLength":0,"Rank":1,"SyncRoot":{"Length":0,"LongLength":0,"Rank":1,"SyncRoot":[],"IsReadOnly":false,"IsFixedSize":true,"IsSynchronized":false},"IsReadOnly":false,"IsFixedSize":true,"IsSynchronized":false}},"ReturnParameter":{"ParameterType":[],"Name":"null","HasDefaultValue":true,"DefaultValue":"null","RawDefaultValue":"null","MetadataToken":100000000,"Attributes":"None","Member":{"Name":"GetMethods","DeclaringType":"System.Type","ReflectedType":"System.Type","MemberType":"Method","MetadataToken":100000000,"Module":{"MDStreamVersion":131072,"FullyQualifiedName":"--filename--","ModuleVersionId":"00000000-0000-0000-0000-000000000000","MetadataToken":100000000,"ScopeName":"System.Private.CoreLib.dll","Name":"System.Private.CoreLib.dll","Assembly":"System.Private.CoreLib, Version=6.0.0.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e","ModuleHandle":"System.ModuleHandle","CustomAttributes":"System.Collections.ObjectModel.ReadOnlyCollection\u00601[System.Reflection.CustomAttributeData]"},"IsSecurityCritical":true,"IsSecuritySafeCritical":false,"IsSecurityTransparent":false,"MethodHandle":{"Value":9999999999},"Attributes":"PrivateScope, Public, HideBySig","CallingConvention":"Standard, HasThis","ReturnType":[],"ReturnTypeCustomAttributes":{"ParameterType":[],"Name":"null","HasDefaultValue":true,"DefaultValue":"null","RawDefaultValue":"null","MetadataToken":100000000,"Attributes":"None","Member":"System.Reflection.MethodInfo[] GetMethods()","Position":-1,"IsIn":false,"IsLcid":false,"IsOptional":false,"IsOut":false,"IsRetval":false,"CustomAttributes":[]},"ReturnParameter":{"ParameterType":[],"Name":"null","HasDefaultValue":true,"DefaultValue":"null","RawDefaultValue":"null","MetadataToken":100000000,"Attributes":"None","Member":"System.Reflection.MethodInfo[] GetMethods()","Position":-1,"IsIn":false,"IsLcid":false,"IsOptional":false,"IsOut":false,"IsRetval":false,"CustomAttributes":[]},"IsCollectible":false,"IsGenericMethod":false,"IsGenericMethodDefinition":false,"ContainsGenericParameters":false,"MethodImplementationFlags":"Managed","IsAbstract":false,"IsConstructor":false,"IsFinal":false,"IsHideBySig":true,"IsSpecialName":false,"IsStatic":false,"IsVirtual":false,"IsAssembly":false,"IsFamily":false,"IsFamilyAndAssembly":false,"IsFamilyOrAssembly":false,"IsPrivate":false,"IsPublic":true,"IsConstructedGenericMethod":false,"CustomAttributes":{"Count":1,"Item":"CustomAttributeData"}},"Position":-1,"IsIn":false,"IsLcid":false,"IsOptional":false,"IsOut":false,"IsRetval":false,"CustomAttributes":{"Length":0,"LongLength":0,"Rank":1,"SyncRoot":{"Length":0,"LongLength":0,"Rank":1,"SyncRoot":[],"IsReadOnly":false,"IsFixedSize":true,"IsSynchronized":false},"IsReadOnly":false,"IsFixedSize":true,"IsSynchronized":false}},"IsCollectible":false,"IsGenericMethod":false,"IsGenericMethodDefinition":false,"ContainsGenericParameters":false,"MethodImplementationFlags":"Managed","IsAbstract":false,"IsConstructor":false,"IsFinal":false,"IsHideBySig":true,"IsSpecialName":false,"IsStatic":false,"IsVirtual":false,"IsAssembly":false,"IsFamily":false,"IsFamilyAndAssembly":false,"IsFamilyOrAssembly":false,"IsPrivate":false,"IsPublic":true,"IsConstructedGenericMethod":false,"CustomAttributes":{"Count":1,"Item":"CustomAttributeData"}}
                       """;

        var actual = value.TooString(TooStringHow.Json);

        TestContext.Progress.WriteLine(actual.RegexReplaceKnownRuntimeVariableValues());
        
        Assert.That(
            value
                .TooString(TooStringHow.Json)
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
