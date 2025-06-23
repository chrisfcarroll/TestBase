using System.Dynamic;
using System.Numerics;
using System.Reflection;
using System.Text.Json;

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

    [Test]
    public void GivenAValueTuple__ReturnsReflectedJson()
    {
        var value = (1,"boo",new Complex(3,4));
        var expected = """[1,"boo",[3,4]]""";

        Assert.That(
            value.TooString(),
            Is.EqualTo(expected)
            );
        TestContext.Progress.WriteLine(value.TooString());
    }

    [Test/*,Ignore("Don't create HttpClient on each run")*/]
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
                       {"Name":"GetMethods","DeclaringType":"System.Type","ReflectedType":"System.Type","MemberType":"Method","MetadataToken":100000000,"Module":{"MDStreamVersion":131072,"FullyQualifiedName":"--filename--","ModuleVersionId":{},"MetadataToken":100000000,"ScopeName":"System.Private.CoreLib.dll","Name":"System.Private.CoreLib.dll","Assembly":{"CodeBase":"file:///--filename--","FullName":"System.Private.CoreLib, Version=X.X.X.X, Culture=neutral, PublicKeyToken=7cec85d7bea7798e","EntryPoint":"null","DefinedTypes":["Microsoft.CodeAnalysis.EmbeddedAttribute","System.Runtime.CompilerServices.IsUnmanagedAttribute","System.Runtime.CompilerServices.NullableAttribute"],"IsCollectible":false,"ManifestModule":{"MDStreamVersion":131072,"FullyQualifiedName":"--filename--","ModuleVersionId":"00000000-0000-0000-0000-000000000000","MetadataToken":100000000,"ScopeName":"System.Private.CoreLib.dll","Name":"System.Private.CoreLib.dll","Assembly":"System.Private.CoreLib, Version=X.X.X.X, Culture=neutral, PublicKeyToken=7cec85d7bea7798e","ModuleHandle":"System.ModuleHandle","CustomAttributes":[]},"ReflectionOnly":false,"Location":"--filename--","ImageRuntimeVersion":"v4.0.30319","GlobalAssemblyCache":false,"HostContext":0,"IsDynamic":false,"ExportedTypes":["Microsoft.Win32.SafeHandles.CriticalHandleMinusOneIsInvalid","Microsoft.Win32.SafeHandles.CriticalHandleZeroOrMinusOneIsInvalid","Microsoft.Win32.SafeHandles.SafeHandleMinusOneIsInvalid"],"IsFullyTrusted":true,"CustomAttributes":["[System.Runtime.CompilerServices.ExtensionAttribute()]","[System.Runtime.CompilerServices.CompilationRelaxationsAttribute((Int32)8)]","[System.Runtime.CompilerServices.RuntimeCompatibilityAttribute(WrapNonExceptionThrows = True)]"],"EscapedCodeBase":"file:///--filename--","Modules":["System.Private.CoreLib.dll"],"SecurityRuleSet":"None"},"ModuleHandle":{"MDStreamVersion":131072},"CustomAttributes":[{"Constructor":"Void .ctor(Boolean)","ConstructorArguments":[],"NamedArguments":[],"AttributeType":"System.Runtime.CompilerServices.NullablePublicOnlyAttribute"},{"Constructor":"Void .ctor()","ConstructorArguments":[],"NamedArguments":[],"AttributeType":"System.Runtime.CompilerServices.SkipLocalsInitAttribute"}]},"IsSecurityCritical":true,"IsSecuritySafeCritical":false,"IsSecurityTransparent":false,"MethodHandle":{"Value":9999999999},"Attributes":"PrivateScope, Public, HideBySig","CallingConvention":"Standard, HasThis","ReturnType":"System.Reflection.MethodInfo[]","ReturnTypeCustomAttributes":{"ParameterType":"System.Reflection.MethodInfo[]","Name":"null","HasDefaultValue":true,"DefaultValue":"null","RawDefaultValue":"null","MetadataToken":100000000,"Attributes":"None","Member":{"Name":"GetMethods","DeclaringType":"System.Type","ReflectedType":"System.Type","MemberType":"Method","MetadataToken":100000000,"Module":{"MDStreamVersion":131072,"FullyQualifiedName":"--filename--","ModuleVersionId":"00000000-0000-0000-0000-000000000000","MetadataToken":100000000,"ScopeName":"System.Private.CoreLib.dll","Name":"System.Private.CoreLib.dll","Assembly":"System.Private.CoreLib, Version=X.X.X.X, Culture=neutral, PublicKeyToken=7cec85d7bea7798e","ModuleHandle":"System.ModuleHandle","CustomAttributes":[]},"IsSecurityCritical":true,"IsSecuritySafeCritical":false,"IsSecurityTransparent":false,"MethodHandle":{"Value":9999999999},"Attributes":"PrivateScope, Public, HideBySig","CallingConvention":"Standard, HasThis","ReturnType":"System.Reflection.MethodInfo[]","ReturnTypeCustomAttributes":{"ParameterType":"System.Reflection.MethodInfo[]","Name":"null","HasDefaultValue":true,"DefaultValue":"null","RawDefaultValue":"null","MetadataToken":100000000,"Attributes":"None","Member":"System.Reflection.MethodInfo[] GetMethods()","Position":-1,"IsIn":false,"IsLcid":false,"IsOptional":false,"IsOut":false,"IsRetval":false,"CustomAttributes":[]},"ReturnParameter":{"ParameterType":"System.Reflection.MethodInfo[]","Name":"null","HasDefaultValue":true,"DefaultValue":"null","RawDefaultValue":"null","MetadataToken":100000000,"Attributes":"None","Member":"System.Reflection.MethodInfo[] GetMethods()","Position":-1,"IsIn":false,"IsLcid":false,"IsOptional":false,"IsOut":false,"IsRetval":false,"CustomAttributes":[]},"IsCollectible":false,"IsGenericMethod":false,"IsGenericMethodDefinition":false,"ContainsGenericParameters":false,"MethodImplementationFlags":"Managed","IsAbstract":false,"IsConstructor":false,"IsFinal":false,"IsHideBySig":true,"IsSpecialName":false,"IsStatic":false,"IsVirtual":false,"IsAssembly":false,"IsFamily":false,"IsFamilyAndAssembly":false,"IsFamilyOrAssembly":false,"IsPrivate":false,"IsPublic":true,"IsConstructedGenericMethod":false,"CustomAttributes":["[System.Diagnostics.CodeAnalysis.DynamicallyAccessedMembersAttribute((System.Diagnostics.CodeAnalysis.DynamicallyAccessedMemberTypes)8)]"]},"Position":-1,"IsIn":false,"IsLcid":false,"IsOptional":false,"IsOut":false,"IsRetval":false,"CustomAttributes":[]},"ReturnParameter":{"ParameterType":"System.Reflection.MethodInfo[]","Name":"null","HasDefaultValue":true,"DefaultValue":"null","RawDefaultValue":"null","MetadataToken":100000000,"Attributes":"None","Member":{"Name":"GetMethods","DeclaringType":"System.Type","ReflectedType":"System.Type","MemberType":"Method","MetadataToken":100000000,"Module":{"MDStreamVersion":131072,"FullyQualifiedName":"--filename--","ModuleVersionId":"00000000-0000-0000-0000-000000000000","MetadataToken":100000000,"ScopeName":"System.Private.CoreLib.dll","Name":"System.Private.CoreLib.dll","Assembly":"System.Private.CoreLib, Version=X.X.X.X, Culture=neutral, PublicKeyToken=7cec85d7bea7798e","ModuleHandle":"System.ModuleHandle","CustomAttributes":[]},"IsSecurityCritical":true,"IsSecuritySafeCritical":false,"IsSecurityTransparent":false,"MethodHandle":{"Value":9999999999},"Attributes":"PrivateScope, Public, HideBySig","CallingConvention":"Standard, HasThis","ReturnType":"System.Reflection.MethodInfo[]","ReturnTypeCustomAttributes":{"ParameterType":"System.Reflection.MethodInfo[]","Name":"null","HasDefaultValue":true,"DefaultValue":"null","RawDefaultValue":"null","MetadataToken":100000000,"Attributes":"None","Member":"System.Reflection.MethodInfo[] GetMethods()","Position":-1,"IsIn":false,"IsLcid":false,"IsOptional":false,"IsOut":false,"IsRetval":false,"CustomAttributes":[]},"ReturnParameter":{"ParameterType":"System.Reflection.MethodInfo[]","Name":"null","HasDefaultValue":true,"DefaultValue":"null","RawDefaultValue":"null","MetadataToken":100000000,"Attributes":"None","Member":"System.Reflection.MethodInfo[] GetMethods()","Position":-1,"IsIn":false,"IsLcid":false,"IsOptional":false,"IsOut":false,"IsRetval":false,"CustomAttributes":[]},"IsCollectible":false,"IsGenericMethod":false,"IsGenericMethodDefinition":false,"ContainsGenericParameters":false,"MethodImplementationFlags":"Managed","IsAbstract":false,"IsConstructor":false,"IsFinal":false,"IsHideBySig":true,"IsSpecialName":false,"IsStatic":false,"IsVirtual":false,"IsAssembly":false,"IsFamily":false,"IsFamilyAndAssembly":false,"IsFamilyOrAssembly":false,"IsPrivate":false,"IsPublic":true,"IsConstructedGenericMethod":false,"CustomAttributes":["[System.Diagnostics.CodeAnalysis.DynamicallyAccessedMembersAttribute((System.Diagnostics.CodeAnalysis.DynamicallyAccessedMemberTypes)8)]"]},"Position":-1,"IsIn":false,"IsLcid":false,"IsOptional":false,"IsOut":false,"IsRetval":false,"CustomAttributes":[]},"IsCollectible":false,"IsGenericMethod":false,"IsGenericMethodDefinition":false,"ContainsGenericParameters":false,"MethodImplementationFlags":"Managed","IsAbstract":false,"IsConstructor":false,"IsFinal":false,"IsHideBySig":true,"IsSpecialName":false,"IsStatic":false,"IsVirtual":false,"IsAssembly":false,"IsFamily":false,"IsFamilyAndAssembly":false,"IsFamilyOrAssembly":false,"IsPrivate":false,"IsPublic":true,"IsConstructedGenericMethod":false,"CustomAttributes":[{"Constructor":{"Name":".ctor","MemberType":"Constructor","DeclaringType":"System.Diagnostics.CodeAnalysis.DynamicallyAccessedMembersAttribute","ReflectedType":"System.Diagnostics.CodeAnalysis.DynamicallyAccessedMembersAttribute","MetadataToken":100000000,"Module":"System.Private.CoreLib.dll","MethodHandle":"System.RuntimeMethodHandle","Attributes":"PrivateScope, Public, HideBySig, SpecialName, RTSpecialName","CallingConvention":"Standard, HasThis","IsSecurityCritical":true,"IsSecuritySafeCritical":false,"IsSecurityTransparent":false,"ContainsGenericParameters":false,"MethodImplementationFlags":"Managed","IsAbstract":false,"IsConstructor":true,"IsFinal":false,"IsHideBySig":true,"IsSpecialName":true,"IsStatic":false,"IsVirtual":false,"IsAssembly":false,"IsFamily":false,"IsFamilyAndAssembly":false,"IsFamilyOrAssembly":false,"IsPrivate":false,"IsPublic":true,"IsConstructedGenericMethod":false,"IsGenericMethod":false,"IsGenericMethodDefinition":false,"CustomAttributes":[],"IsCollectible":true},"ConstructorArguments":["(System.Diagnostics.CodeAnalysis.DynamicallyAccessedMemberTypes)8"],"NamedArguments":[],"AttributeType":"System.Diagnostics.CodeAnalysis.DynamicallyAccessedMembersAttribute"}]}
                       """;

        var actual = value.TooString(TooStringHow.Json);

        TestContext.Progress.WriteLine(actual.RegexReplaceKnownRuntimeVariableValues());

        var comparableValue = actual.RegexReplaceKnownRuntimeVariableValues();

        #if NET6_0 // expected values are hard coded to a platform
        Assert.That(comparableValue,
                    Is.EqualTo(expected.RegexReplaceKnownRuntimeVariableValues()));

        var expandoObject = System.Text.Json.JsonSerializer.Deserialize<ExpandoObject>(actual);
        var expandoTooStringRx = expandoObject.TooString(TooStringHow.Json)
                                              .RegexReplaceKnownRuntimeVariableValues();
        Assert.That(comparableValue,Is.EqualTo(expandoTooStringRx));

        #else

        Assert.That(comparableValue.Substring(0,300),
                    Is.EqualTo(expected.Substring(0,300).RegexReplaceKnownRuntimeVariableValues()));
        Assert.That(actual.Length >= expected.Length);
        #endif

    }
}
