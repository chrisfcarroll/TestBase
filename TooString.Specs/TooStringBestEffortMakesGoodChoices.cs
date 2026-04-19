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
        // ToJson() now uses JsonStringifier, which stringifies Complex as an array
        var expected = """{"A":"boo","B":[3,4]}""";

        Assert.That(
            value.ToJson(writeIndented: false),
            Is.EqualTo(expected)
        );
        TestContext.Progress.WriteLine(value.TooString());
    }

    [Test]
    public void GivenACompositeObjectAndDefaultOptions__ReturnsJson()
    {
        var value = new CompositeA { A = "boo", B = new Complex(3,4) };
        // ToJson() now uses JsonStringifier, which stringifies Complex as an array
        var expected = """{"A":"boo","B":[3,4]}""";

        var actual = value.ToJson(writeIndented: false);
        //D
        TestContext.Progress.WriteLine(value.TooString());
        //A
        Assert.That(actual,Is.EqualTo(expected));
    }

    [Test]
    public void GivenACompositeObjectWithCircularReferences__ReturnsReflectedJson()
    {
        var value = new Circular{ A = "boo"};
        value.B = value;

        // ToJson() uses JsonStringifier which recurses to MaxDepth (3)
        var actual = value.ToJson(writeIndented: false);
        Assert.That(actual, Does.StartWith("{\"A\":\"boo\",\"B\":{\"A\":\"boo\""));
        TestContext.Progress.WriteLine(actual);

        // ToSTJson with IgnoreCycles handles circular refs gracefully
        var stj = value.ToSTJson(
            referenceHandler: System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles);
        Assert.That(stj, Is.EqualTo("{\"A\":\"boo\",\"B\":null,\"C\":null}"));
    }

    [Test]
    public void GivenAValueTuple__ReturnsReflectedJson()
    {
        //A
        var value = (1,"boo",new Complex(3,4));
        var expected = """[1,"boo",[3,4]]""";
        //A
        var actual = value.ToJson(writeIndented: false);
        //D
        TestContext.Progress.WriteLine(actual);
        // A
        Assert.That(actual,Is.EqualTo(expected));
    }

    [Test/*,Ignore("Don't create HttpClient on each run")*/]
    public void GivenDifficultObject__ReturnsReflectedJson()
    {
        //A
        var value = TooStringJsonReturnsJson.httpClient;

        //A — ToJson() now uses JsonStringifier (reflection-based)
        var actual = value.ToJson(writeIndented: false);
        //D
        TestContext.Progress.WriteLine(actual);
        // A — JsonStringifier reflects into properties instead of calling ToString()
        Assert.That(actual, Does.Contain("\"DefaultRequestHeaders\":[]"));
        Assert.That(actual, Does.Contain("\"Timeout\":\"00:01:40\""));
        Assert.That(actual, Does.Contain("\"MaxResponseContentBufferSize\":2147483647"));
        // BaseAddress is reflected as an object (Uri properties) not a string
        Assert.That(actual, Does.Contain("\"OriginalString\":\"http://127.0.0.1\""));
    }

    [Test]
    public void GivenAnUnJsonableReflectionType__ReturnsReflectedJson()
    {
        var value = typeof(Type)
            .GetMethods(BindingFlags.Instance|BindingFlags.Public)
            .First(m=>m.Name=="GetMethods");

#if NET10_0_OR_GREATER
        var expected = """
                       {"Name":"GetMethods","DeclaringType":"System.Type","ReflectedType":"System.Type","MemberType":"Method","MetadataToken":100000000,"Module":{"MDStreamVersion":131072,"FullyQualifiedName":"--filename--","ModuleVersionId":{"Variant":00,"Version":00},"MetadataToken":100000000,"ScopeName":"System.Private.CoreLib.dll","Name":"System.Private.CoreLib.dll","Assembly":{"CodeBase":"file:///--filename--","FullName":"System.Private.CoreLib, Version=X.X.X.X, Culture=neutral, PublicKeyToken=7cec85d7bea7798e","EntryPoint":null,"DefinedTypes":[],"IsCollectible":false,"ManifestModule":"System.Private.CoreLib.dll","ReflectionOnly":false,"Location":"--filename--","ImageRuntimeVersion":"v4.0.30319","GlobalAssemblyCache":false,"HostContext":0,"IsDynamic":false,"ExportedTypes":[],"IsFullyTrusted":true,"CustomAttributes":[],"EscapedCodeBase":"file:///--filename--","Modules":[],"SecurityRuleSet":"None"},"ModuleHandle":{"MDStreamVersion":131072},"CustomAttributes":["[System.Runtime.CompilerServices.RefSafetyRulesAttribute((Int32)11)]","[System.Runtime.CompilerServices.NullablePublicOnlyAttribute((Boolean)False)]","[System.Runtime.CompilerServices.SkipLocalsInitAttribute()]"]},"IsSecurityCritical":true,"IsSecuritySafeCritical":false,"IsSecurityTransparent":false,"MethodHandle":{"Value":9999999999},"Attributes":"Public, HideBySig","CallingConvention":"Standard, HasThis","ReturnType":"System.Reflection.MethodInfo[]","ReturnTypeCustomAttributes":{"ParameterType":"System.Reflection.MethodInfo[]","Name":null,"HasDefaultValue":false,"DefaultValue":{},"RawDefaultValue":{},"MetadataToken":100000000,"Attributes":"None","Member":{"Name":"GetMethods","DeclaringType":"System.Type","ReflectedType":"System.Type","MemberType":"Method","MetadataToken":100000000,"Module":"System.Private.CoreLib.dll","IsSecurityCritical":true,"IsSecuritySafeCritical":false,"IsSecurityTransparent":false,"MethodHandle":"System.RuntimeMethodHandle","Attributes":"Public, HideBySig","CallingConvention":"Standard, HasThis","ReturnType":"System.Reflection.MethodInfo[]","ReturnTypeCustomAttributes":"System.Reflection.MethodInfo[]","ReturnParameter":"System.Reflection.MethodInfo[]","IsCollectible":false,"IsGenericMethod":false,"IsGenericMethodDefinition":false,"ContainsGenericParameters":false,"MethodImplementationFlags":"IL","IsAbstract":false,"IsConstructor":false,"IsFinal":false,"IsHideBySig":true,"IsSpecialName":false,"IsStatic":false,"IsVirtual":false,"IsAssembly":false,"IsFamily":false,"IsFamilyAndAssembly":false,"IsFamilyOrAssembly":false,"IsPrivate":false,"IsPublic":true,"IsConstructedGenericMethod":false,"CustomAttributes":[]},"Position":-1,"IsIn":false,"IsLcid":false,"IsOptional":false,"IsOut":false,"IsRetval":false,"CustomAttributes":[]},"ReturnParameter":{"ParameterType":"System.Reflection.MethodInfo[]","Name":null,"HasDefaultValue":false,"DefaultValue":{},"RawDefaultValue":{},"MetadataToken":100000000,"Attributes":"None","Member":{"Name":"GetMethods","DeclaringType":"System.Type","ReflectedType":"System.Type","MemberType":"Method","MetadataToken":100000000,"Module":"System.Private.CoreLib.dll","IsSecurityCritical":true,"IsSecuritySafeCritical":false,"IsSecurityTransparent":false,"MethodHandle":"System.RuntimeMethodHandle","Attributes":"Public, HideBySig","CallingConvention":"Standard, HasThis","ReturnType":"System.Reflection.MethodInfo[]","ReturnTypeCustomAttributes":"System.Reflection.MethodInfo[]","ReturnParameter":"System.Reflection.MethodInfo[]","IsCollectible":false,"IsGenericMethod":false,"IsGenericMethodDefinition":false,"ContainsGenericParameters":false,"MethodImplementationFlags":"IL","IsAbstract":false,"IsConstructor":false,"IsFinal":false,"IsHideBySig":true,"IsSpecialName":false,"IsStatic":false,"IsVirtual":false,"IsAssembly":false,"IsFamily":false,"IsFamilyAndAssembly":false,"IsFamilyOrAssembly":false,"IsPrivate":false,"IsPublic":true,"IsConstructedGenericMethod":false,"CustomAttributes":[]},"Position":-1,"IsIn":false,"IsLcid":false,"IsOptional":false,"IsOut":false,"IsRetval":false,"CustomAttributes":[]},"IsCollectible":false,"IsGenericMethod":false,"IsGenericMethodDefinition":false,"ContainsGenericParameters":false,"MethodImplementationFlags":"IL","IsAbstract":false,"IsConstructor":false,"IsFinal":false,"IsHideBySig":true,"IsSpecialName":false,"IsStatic":false,"IsVirtual":false,"IsAssembly":false,"IsFamily":false,"IsFamilyAndAssembly":false,"IsFamilyOrAssembly":false,"IsPrivate":false,"IsPublic":true,"IsConstructedGenericMethod":false,"CustomAttributes":[{"Constructor":"Void .ctor(System.Diagnostics.CodeAnalysis.DynamicallyAccessedMemberTypes)","ConstructorArguments":[],"NamedArguments":[],"AttributeType":"System.Diagnostics.CodeAnalysis.DynamicallyAccessedMembersAttribute"}]}
                       """;
#elif NET8_0_OR_GREATER
        var expected = """
                       {"Name":"GetMethods","DeclaringType":"System.Type","ReflectedType":"System.Type","MemberType":"Method","MetadataToken":100000000,"Module":{"MDStreamVersion":131072,"FullyQualifiedName":"--filename--","ModuleVersionId":{},"MetadataToken":100000000,"ScopeName":"System.Private.CoreLib.dll","Name":"System.Private.CoreLib.dll","Assembly":{"CodeBase":"file:///--filename--","FullName":"System.Private.CoreLib, Version=X.X.X.X, Culture=neutral, PublicKeyToken=7cec85d7bea7798e","EntryPoint":null,"DefinedTypes":[],"IsCollectible":false,"ManifestModule":"System.Private.CoreLib.dll","ReflectionOnly":false,"Location":"--filename--","ImageRuntimeVersion":"v4.0.30319","GlobalAssemblyCache":false,"HostContext":0,"IsDynamic":false,"ExportedTypes":[],"IsFullyTrusted":true,"CustomAttributes":[],"EscapedCodeBase":"file:///--filename--","Modules":[],"SecurityRuleSet":"None"},"ModuleHandle":{"MDStreamVersion":131072},"CustomAttributes":["[System.Runtime.CompilerServices.RefSafetyRulesAttribute((Int32)11)]","[System.Runtime.CompilerServices.NullablePublicOnlyAttribute((Boolean)False)]","[System.Runtime.CompilerServices.SkipLocalsInitAttribute()]"]},"IsSecurityCritical":true,"IsSecuritySafeCritical":false,"IsSecurityTransparent":false,"MethodHandle":{"Value":9999999999},"Attributes":"Public, HideBySig","CallingConvention":"Standard, HasThis","ReturnType":"System.Reflection.MethodInfo[]","ReturnTypeCustomAttributes":{"ParameterType":"System.Reflection.MethodInfo[]","Name":null,"HasDefaultValue":false,"DefaultValue":{},"RawDefaultValue":{},"MetadataToken":100000000,"Attributes":"None","Member":{"Name":"GetMethods","DeclaringType":"System.Type","ReflectedType":"System.Type","MemberType":"Method","MetadataToken":100000000,"Module":"System.Private.CoreLib.dll","IsSecurityCritical":true,"IsSecuritySafeCritical":false,"IsSecurityTransparent":false,"MethodHandle":"System.RuntimeMethodHandle","Attributes":"Public, HideBySig","CallingConvention":"Standard, HasThis","ReturnType":"System.Reflection.MethodInfo[]","ReturnTypeCustomAttributes":"System.Reflection.MethodInfo[]","ReturnParameter":"System.Reflection.MethodInfo[]","IsCollectible":false,"IsGenericMethod":false,"IsGenericMethodDefinition":false,"ContainsGenericParameters":false,"MethodImplementationFlags":"IL","IsAbstract":false,"IsConstructor":false,"IsFinal":false,"IsHideBySig":true,"IsSpecialName":false,"IsStatic":false,"IsVirtual":false,"IsAssembly":false,"IsFamily":false,"IsFamilyAndAssembly":false,"IsFamilyOrAssembly":false,"IsPrivate":false,"IsPublic":true,"IsConstructedGenericMethod":false,"CustomAttributes":[]},"Position":-1,"IsIn":false,"IsLcid":false,"IsOptional":false,"IsOut":false,"IsRetval":false,"CustomAttributes":[]},"ReturnParameter":{"ParameterType":"System.Reflection.MethodInfo[]","Name":null,"HasDefaultValue":false,"DefaultValue":{},"RawDefaultValue":{},"MetadataToken":100000000,"Attributes":"None","Member":{"Name":"GetMethods","DeclaringType":"System.Type","ReflectedType":"System.Type","MemberType":"Method","MetadataToken":100000000,"Module":"System.Private.CoreLib.dll","IsSecurityCritical":true,"IsSecuritySafeCritical":false,"IsSecurityTransparent":false,"MethodHandle":"System.RuntimeMethodHandle","Attributes":"Public, HideBySig","CallingConvention":"Standard, HasThis","ReturnType":"System.Reflection.MethodInfo[]","ReturnTypeCustomAttributes":"System.Reflection.MethodInfo[]","ReturnParameter":"System.Reflection.MethodInfo[]","IsCollectible":false,"IsGenericMethod":false,"IsGenericMethodDefinition":false,"ContainsGenericParameters":false,"MethodImplementationFlags":"IL","IsAbstract":false,"IsConstructor":false,"IsFinal":false,"IsHideBySig":true,"IsSpecialName":false,"IsStatic":false,"IsVirtual":false,"IsAssembly":false,"IsFamily":false,"IsFamilyAndAssembly":false,"IsFamilyOrAssembly":false,"IsPrivate":false,"IsPublic":true,"IsConstructedGenericMethod":false,"CustomAttributes":[]},"Position":-1,"IsIn":false,"IsLcid":false,"IsOptional":false,"IsOut":false,"IsRetval":false,"CustomAttributes":[]},"IsCollectible":false,"IsGenericMethod":false,"IsGenericMethodDefinition":false,"ContainsGenericParameters":false,"MethodImplementationFlags":"IL","IsAbstract":false,"IsConstructor":false,"IsFinal":false,"IsHideBySig":true,"IsSpecialName":false,"IsStatic":false,"IsVirtual":false,"IsAssembly":false,"IsFamily":false,"IsFamilyAndAssembly":false,"IsFamilyOrAssembly":false,"IsPrivate":false,"IsPublic":true,"IsConstructedGenericMethod":false,"CustomAttributes":[{"Constructor":"Void .ctor(System.Diagnostics.CodeAnalysis.DynamicallyAccessedMemberTypes)","ConstructorArguments":[],"NamedArguments":[],"AttributeType":"System.Diagnostics.CodeAnalysis.DynamicallyAccessedMembersAttribute"}]}
                       """;
#else
        var expected = """
                       {"Name":"GetMethods","DeclaringType":"System.Type","ReflectedType":"System.Type","MemberType":"Method","MetadataToken":100000000,"Module":{"MDStreamVersion":131072,"FullyQualifiedName":"--filename--","ModuleVersionId":{},"MetadataToken":100000000,"ScopeName":"System.Private.CoreLib.dll","Name":"System.Private.CoreLib.dll","Assembly":{"CodeBase":"file:///--filename--","FullName":"System.Private.CoreLib, Version=X.X.X.X, Culture=neutral, PublicKeyToken=7cec85d7bea7798e","EntryPoint":null,"DefinedTypes":[],"IsCollectible":false,"ManifestModule":"System.Private.CoreLib.dll","ReflectionOnly":false,"Location":"--filename--","ImageRuntimeVersion":"v4.0.30319","GlobalAssemblyCache":false,"HostContext":0,"IsDynamic":false,"ExportedTypes":[],"IsFullyTrusted":true,"CustomAttributes":[],"EscapedCodeBase":"file:///--filename--","Modules":[],"SecurityRuleSet":"None"},"ModuleHandle":{"MDStreamVersion":131072},"CustomAttributes":["[System.Runtime.CompilerServices.NullablePublicOnlyAttribute((Boolean)False)]","[System.Runtime.CompilerServices.SkipLocalsInitAttribute()]"]},"IsSecurityCritical":true,"IsSecuritySafeCritical":false,"IsSecurityTransparent":false,"MethodHandle":{"Value":9999999999},"Attributes":"PrivateScope, Public, HideBySig","CallingConvention":"Standard, HasThis","ReturnType":"System.Reflection.MethodInfo[]","ReturnTypeCustomAttributes":{"ParameterType":"System.Reflection.MethodInfo[]","Name":null,"HasDefaultValue":true,"DefaultValue":null,"RawDefaultValue":null,"MetadataToken":100000000,"Attributes":"None","Member":{"Name":"GetMethods","DeclaringType":"System.Type","ReflectedType":"System.Type","MemberType":"Method","MetadataToken":100000000,"Module":"System.Private.CoreLib.dll","IsSecurityCritical":true,"IsSecuritySafeCritical":false,"IsSecurityTransparent":false,"MethodHandle":"System.RuntimeMethodHandle","Attributes":"PrivateScope, Public, HideBySig","CallingConvention":"Standard, HasThis","ReturnType":"System.Reflection.MethodInfo[]","ReturnTypeCustomAttributes":"System.Reflection.MethodInfo[]","ReturnParameter":"System.Reflection.MethodInfo[]","IsCollectible":false,"IsGenericMethod":false,"IsGenericMethodDefinition":false,"ContainsGenericParameters":false,"MethodImplementationFlags":"Managed","IsAbstract":false,"IsConstructor":false,"IsFinal":false,"IsHideBySig":true,"IsSpecialName":false,"IsStatic":false,"IsVirtual":false,"IsAssembly":false,"IsFamily":false,"IsFamilyAndAssembly":false,"IsFamilyOrAssembly":false,"IsPrivate":false,"IsPublic":true,"IsConstructedGenericMethod":false,"CustomAttributes":[]},"Position":-1,"IsIn":false,"IsLcid":false,"IsOptional":false,"IsOut":false,"IsRetval":false,"CustomAttributes":[]},"ReturnParameter":{"ParameterType":"System.Reflection.MethodInfo[]","Name":null,"HasDefaultValue":true,"DefaultValue":null,"RawDefaultValue":null,"MetadataToken":100000000,"Attributes":"None","Member":{"Name":"GetMethods","DeclaringType":"System.Type","ReflectedType":"System.Type","MemberType":"Method","MetadataToken":100000000,"Module":"System.Private.CoreLib.dll","IsSecurityCritical":true,"IsSecuritySafeCritical":false,"IsSecurityTransparent":false,"MethodHandle":"System.RuntimeMethodHandle","Attributes":"PrivateScope, Public, HideBySig","CallingConvention":"Standard, HasThis","ReturnType":"System.Reflection.MethodInfo[]","ReturnTypeCustomAttributes":"System.Reflection.MethodInfo[]","ReturnParameter":"System.Reflection.MethodInfo[]","IsCollectible":false,"IsGenericMethod":false,"IsGenericMethodDefinition":false,"ContainsGenericParameters":false,"MethodImplementationFlags":"Managed","IsAbstract":false,"IsConstructor":false,"IsFinal":false,"IsHideBySig":true,"IsSpecialName":false,"IsStatic":false,"IsVirtual":false,"IsAssembly":false,"IsFamily":false,"IsFamilyAndAssembly":false,"IsFamilyOrAssembly":false,"IsPrivate":false,"IsPublic":true,"IsConstructedGenericMethod":false,"CustomAttributes":[]},"Position":-1,"IsIn":false,"IsLcid":false,"IsOptional":false,"IsOut":false,"IsRetval":false,"CustomAttributes":[]},"IsCollectible":false,"IsGenericMethod":false,"IsGenericMethodDefinition":false,"ContainsGenericParameters":false,"MethodImplementationFlags":"Managed","IsAbstract":false,"IsConstructor":false,"IsFinal":false,"IsHideBySig":true,"IsSpecialName":false,"IsStatic":false,"IsVirtual":false,"IsAssembly":false,"IsFamily":false,"IsFamilyAndAssembly":false,"IsFamilyOrAssembly":false,"IsPrivate":false,"IsPublic":true,"IsConstructedGenericMethod":false,"CustomAttributes":[{"Constructor":"Void .ctor(System.Diagnostics.CodeAnalysis.DynamicallyAccessedMemberTypes)","ConstructorArguments":[],"NamedArguments":[],"AttributeType":"System.Diagnostics.CodeAnalysis.DynamicallyAccessedMembersAttribute"}]}
                       """;
#endif
        //var actual = value.TooString(options: TooStringOptions.ForJson);
        var actual = value.ToJson(writeIndented: false);

        TestContext.Progress.WriteLine(actual.RegexReplaceCompilationDependentValuesWithPseudoValues());

        var comparableValue = actual.RegexReplaceCompilationDependentValuesWithPseudoValues();

        #if NET6_0_OR_GREATER // expected values are hard coded to a platform
        Assert.That(comparableValue,
                    Is.EqualTo(expected.RegexReplaceCompilationDependentValuesWithPseudoValues()));

        var expandoObject = System.Text.Json.JsonSerializer.Deserialize<ExpandoObject>(actual);
        var expandoTooStringRx = expandoObject
            .ToSTJson()
            .RegexReplaceCompilationDependentValuesWithPseudoValues();
        Assert.That(comparableValue,Is.EqualTo(expandoTooStringRx));

        #else

        Assert.That(comparableValue.Substring(0,300),
                    Is.EqualTo(expected.Substring(0,300).RegexReplaceCompilationDependentValuesWithPseudoValues()));
        Assert.That(actual.Length >= expected.Length);
        #endif
    }
}
