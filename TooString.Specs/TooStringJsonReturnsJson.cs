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
        //account for Json divergence from ToString("O") format if fractional seconds has trailing 0
        const long ticksPerTenthOfMicroSecond= TimeSpan.TicksPerSecond / 10_000_000;
        if (now.TimeOfDay.Ticks / ticksPerTenthOfMicroSecond % 10 == 0)
        {
            now = now.AddTicks(ticksPerTenthOfMicroSecond * 111);
        }

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


#if NET6_0 // Expected values are hard-coded to a platform

    [Test]
    public void GivenNamedTupleReturnsJson()
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
            value.TooString(TooStringHow.Json),
            Is.EqualTo("""[1,"boo",false,"Absolute",{"A":"A","B":[3,4]}]"""));

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
    public void GivenAnUnJsonableReflectionType_Assembly()
    {
        //A
        var value = Assembly.GetExecutingAssembly();

        var expected = """
                       {"CodeBase":"file:///--filename--","FullName":"TooString.Specs, Version=X.X.X.X, Culture=neutral, PublicKeyToken=null","EntryPoint":{"Name":"Main","DeclaringType":"AutoGeneratedProgram","ReflectedType":"AutoGeneratedProgram","MemberType":"Method","MetadataToken":100000000,"Module":{"MDStreamVersion":131072,"FullyQualifiedName":"--filename--","ModuleVersionId":"00000000-0000-0000-0000-000000000000","MetadataToken":100000000,"ScopeName":"TooString.Specs.dll","Name":"TooString.Specs.dll","Assembly":"TooString.Specs, Version=X.X.X.X, Culture=neutral, PublicKeyToken=null","ModuleHandle":"System.ModuleHandle","CustomAttributes":[]},"IsSecurityCritical":true,"IsSecuritySafeCritical":false,"IsSecurityTransparent":false,"MethodHandle":{"Value":9999999999},"Attributes":"PrivateScope, Private, Static, HideBySig","CallingConvention":"Standard","ReturnType":"System.Void","ReturnTypeCustomAttributes":{"ParameterType":"System.Void","Name":"null","HasDefaultValue":true,"DefaultValue":"null","RawDefaultValue":"null","MetadataToken":100000000,"Attributes":"None","Member":"Void Main(System.String[])","Position":-1,"IsIn":false,"IsLcid":false,"IsOptional":false,"IsOut":false,"IsRetval":false,"CustomAttributes":[]},"ReturnParameter":{"ParameterType":"System.Void","Name":"null","HasDefaultValue":true,"DefaultValue":"null","RawDefaultValue":"null","MetadataToken":100000000,"Attributes":"None","Member":"Void Main(System.String[])","Position":-1,"IsIn":false,"IsLcid":false,"IsOptional":false,"IsOut":false,"IsRetval":false,"CustomAttributes":[]},"IsCollectible":false,"IsGenericMethod":false,"IsGenericMethodDefinition":false,"ContainsGenericParameters":false,"MethodImplementationFlags":"Managed","IsAbstract":false,"IsConstructor":false,"IsFinal":false,"IsHideBySig":true,"IsSpecialName":false,"IsStatic":true,"IsVirtual":false,"IsAssembly":false,"IsFamily":false,"IsFamilyAndAssembly":false,"IsFamilyOrAssembly":false,"IsPrivate":true,"IsPublic":false,"IsConstructedGenericMethod":false,"CustomAttributes":[]},"DefinedTypes":[{"IsCollectible":false,"DeclaringMethod":"MethodBase","FullName":"\u003C\u003Ef__AnonymousType0\u00602","AssemblyQualifiedName":"\u003C\u003Ef__AnonymousType0\u00602, TooString.Specs, Version=X.X.X.X, Culture=neutral, PublicKeyToken=null","Namespace":"null","GUID":"00000000-0000-0000-0000-000000000000","GenericParameterAttributes":"GenericParameterAttributes","IsSZArray":false,"GenericParameterPosition":"Int32","ContainsGenericParameters":true,"StructLayoutAttribute":"System.Runtime.InteropServices.StructLayoutAttribute","Name":"\u003C\u003Ef__AnonymousType0\u00602","DeclaringType":"null","Assembly":"TooString.Specs, Version=X.X.X.X, Culture=neutral, PublicKeyToken=null","BaseType":"System.Object","IsByRefLike":false,"IsConstructedGenericType":false,"IsGenericType":true,"IsGenericTypeDefinition":true,"IsGenericParameter":false,"IsTypeDefinition":true,"IsSecurityCritical":true,"IsSecuritySafeCritical":false,"IsSecurityTransparent":false,"MemberType":"TypeInfo","MetadataToken":100000000,"Module":"TooString.Specs.dll","ReflectedType":"null","TypeHandle":"System.RuntimeTypeHandle","UnderlyingSystemType":"\u003C\u003Ef__AnonymousType0\u00602","GenericTypeParameters":[],"DeclaredConstructors":[],"DeclaredEvents":[],"DeclaredFields":[],"DeclaredMembers":[],"DeclaredMethods":[],"DeclaredNestedTypes":[],"DeclaredProperties":[],"ImplementedInterfaces":[],"IsInterface":false,"IsNested":false,"IsArray":false,"IsByRef":false,"IsPointer":false,"IsGenericTypeParameter":false,"IsGenericMethodParameter":false,"IsVariableBoundArray":false,"HasElementType":false,"GenericTypeArguments":[],"Attributes":"AutoLayout, AnsiClass, Class, Sealed, BeforeFieldInit","IsAbstract":false,"IsImport":false,"IsSealed":true,"IsSpecialName":false,"IsClass":true,"IsNestedAssembly":false,"IsNestedFamANDAssem":false,"IsNestedFamily":false,"IsNestedFamORAssem":false,"IsNestedPrivate":false,"IsNestedPublic":false,"IsNotPublic":true,"IsPublic":false,"IsAutoLayout":true,"IsExplicitLayout":false,"IsLayoutSequential":false,"IsAnsiClass":true,"IsAutoClass":false,"IsUnicodeClass":false,"IsCOMObject":false,"IsContextful":false,"IsEnum":false,"IsMarshalByRef":false,"IsPrimitive":false,"IsValueType":false,"IsSignatureType":false,"TypeInitializer":"null","IsSerializable":false,"IsVisible":false,"CustomAttributes":[]},{"IsCollectible":false,"DeclaringMethod":"MethodBase","FullName":"\u003C\u003Ef__AnonymousType1\u00605","AssemblyQualifiedName":"\u003C\u003Ef__AnonymousType1\u00605, TooString.Specs, Version=X.X.X.X, Culture=neutral, PublicKeyToken=null","Namespace":"null","GUID":"00000000-0000-0000-0000-000000000000","GenericParameterAttributes":"GenericParameterAttributes","IsSZArray":false,"GenericParameterPosition":"Int32","ContainsGenericParameters":true,"StructLayoutAttribute":"System.Runtime.InteropServices.StructLayoutAttribute","Name":"\u003C\u003Ef__AnonymousType1\u00605","DeclaringType":"null","Assembly":"TooString.Specs, Version=X.X.X.X, Culture=neutral, PublicKeyToken=null","BaseType":"System.Object","IsByRefLike":false,"IsConstructedGenericType":false,"IsGenericType":true,"IsGenericTypeDefinition":true,"IsGenericParameter":false,"IsTypeDefinition":true,"IsSecurityCritical":true,"IsSecuritySafeCritical":false,"IsSecurityTransparent":false,"MemberType":"TypeInfo","MetadataToken":100000000,"Module":"TooString.Specs.dll","ReflectedType":"null","TypeHandle":"System.RuntimeTypeHandle","UnderlyingSystemType":"\u003C\u003Ef__AnonymousType1\u00605","GenericTypeParameters":[],"DeclaredConstructors":[],"DeclaredEvents":[],"DeclaredFields":[],"DeclaredMembers":[],"DeclaredMethods":[],"DeclaredNestedTypes":[],"DeclaredProperties":[],"ImplementedInterfaces":[],"IsInterface":false,"IsNested":false,"IsArray":false,"IsByRef":false,"IsPointer":false,"IsGenericTypeParameter":false,"IsGenericMethodParameter":false,"IsVariableBoundArray":false,"HasElementType":false,"GenericTypeArguments":[],"Attributes":"AutoLayout, AnsiClass, Class, Sealed, BeforeFieldInit","IsAbstract":false,"IsImport":false,"IsSealed":true,"IsSpecialName":false,"IsClass":true,"IsNestedAssembly":false,"IsNestedFamANDAssem":false,"IsNestedFamily":false,"IsNestedFamORAssem":false,"IsNestedPrivate":false,"IsNestedPublic":false,"IsNotPublic":true,"IsPublic":false,"IsAutoLayout":true,"IsExplicitLayout":false,"IsLayoutSequential":false,"IsAnsiClass":true,"IsAutoClass":false,"IsUnicodeClass":false,"IsCOMObject":false,"IsContextful":false,"IsEnum":false,"IsMarshalByRef":false,"IsPrimitive":false,"IsValueType":false,"IsSignatureType":false,"TypeInitializer":"null","IsSerializable":false,"IsVisible":false,"CustomAttributes":[]},{"IsCollectible":false,"DeclaringMethod":"MethodBase","FullName":"\u003C\u003Ef__AnonymousType2\u00605","AssemblyQualifiedName":"\u003C\u003Ef__AnonymousType2\u00605, TooString.Specs, Version=X.X.X.X, Culture=neutral, PublicKeyToken=null","Namespace":"null","GUID":"00000000-0000-0000-0000-000000000000","GenericParameterAttributes":"GenericParameterAttributes","IsSZArray":false,"GenericParameterPosition":"Int32","ContainsGenericParameters":true,"StructLayoutAttribute":"System.Runtime.InteropServices.StructLayoutAttribute","Name":"\u003C\u003Ef__AnonymousType2\u00605","DeclaringType":"null","Assembly":"TooString.Specs, Version=X.X.X.X, Culture=neutral, PublicKeyToken=null","BaseType":"System.Object","IsByRefLike":false,"IsConstructedGenericType":false,"IsGenericType":true,"IsGenericTypeDefinition":true,"IsGenericParameter":false,"IsTypeDefinition":true,"IsSecurityCritical":true,"IsSecuritySafeCritical":false,"IsSecurityTransparent":false,"MemberType":"TypeInfo","MetadataToken":100000000,"Module":"TooString.Specs.dll","ReflectedType":"null","TypeHandle":"System.RuntimeTypeHandle","UnderlyingSystemType":"\u003C\u003Ef__AnonymousType2\u00605","GenericTypeParameters":[],"DeclaredConstructors":[],"DeclaredEvents":[],"DeclaredFields":[],"DeclaredMembers":[],"DeclaredMethods":[],"DeclaredNestedTypes":[],"DeclaredProperties":[],"ImplementedInterfaces":[],"IsInterface":false,"IsNested":false,"IsArray":false,"IsByRef":false,"IsPointer":false,"IsGenericTypeParameter":false,"IsGenericMethodParameter":false,"IsVariableBoundArray":false,"HasElementType":false,"GenericTypeArguments":[],"Attributes":"AutoLayout, AnsiClass, Class, Sealed, BeforeFieldInit","IsAbstract":false,"IsImport":false,"IsSealed":true,"IsSpecialName":false,"IsClass":true,"IsNestedAssembly":false,"IsNestedFamANDAssem":false,"IsNestedFamily":false,"IsNestedFamORAssem":false,"IsNestedPrivate":false,"IsNestedPublic":false,"IsNotPublic":true,"IsPublic":false,"IsAutoLayout":true,"IsExplicitLayout":false,"IsLayoutSequential":false,"IsAnsiClass":true,"IsAutoClass":false,"IsUnicodeClass":false,"IsCOMObject":false,"IsContextful":false,"IsEnum":false,"IsMarshalByRef":false,"IsPrimitive":false,"IsValueType":false,"IsSignatureType":false,"TypeInitializer":"null","IsSerializable":false,"IsVisible":false,"CustomAttributes":[]}],"IsCollectible":false,"ManifestModule":{"MDStreamVersion":131072,"FullyQualifiedName":"--filename--","ModuleVersionId":{},"MetadataToken":100000000,"ScopeName":"TooString.Specs.dll","Name":"TooString.Specs.dll","Assembly":{"CodeBase":"file:///--filename--","FullName":"TooString.Specs, Version=X.X.X.X, Culture=neutral, PublicKeyToken=null","EntryPoint":"Void Main(System.String[])","DefinedTypes":[],"IsCollectible":false,"ManifestModule":"TooString.Specs.dll","ReflectionOnly":false,"Location":"--filename--","ImageRuntimeVersion":"v4.0.30319","GlobalAssemblyCache":false,"HostContext":0,"IsDynamic":false,"ExportedTypes":[],"IsFullyTrusted":true,"CustomAttributes":[],"EscapedCodeBase":"file:///--filename--","Modules":[],"SecurityRuleSet":"None"},"ModuleHandle":{"MDStreamVersion":131072},"CustomAttributes":["[System.Runtime.CompilerServices.RefSafetyRulesAttribute((Int32)11)]"]},"ReflectionOnly":false,"Location":"--filename--","ImageRuntimeVersion":"v4.0.30319","GlobalAssemblyCache":false,"HostContext":0,"IsDynamic":false,"ExportedTypes":[{"IsCollectible":false,"DeclaringMethod":"MethodBase","FullName":"TooString.Specs.TooStringBestEffortMakesGoodChoices","AssemblyQualifiedName":"TooString.Specs.TooStringBestEffortMakesGoodChoices, TooString.Specs, Version=X.X.X.X, Culture=neutral, PublicKeyToken=null","Namespace":"TooString.Specs","GUID":"00000000-0000-0000-0000-000000000000","GenericParameterAttributes":"GenericParameterAttributes","IsSZArray":false,"GenericParameterPosition":"Int32","ContainsGenericParameters":false,"StructLayoutAttribute":"System.Runtime.InteropServices.StructLayoutAttribute","Name":"TooStringBestEffortMakesGoodChoices","DeclaringType":"null","Assembly":"TooString.Specs, Version=X.X.X.X, Culture=neutral, PublicKeyToken=null","BaseType":"System.Object","IsByRefLike":false,"IsConstructedGenericType":false,"IsGenericType":false,"IsGenericTypeDefinition":false,"IsGenericParameter":false,"IsTypeDefinition":true,"IsSecurityCritical":true,"IsSecuritySafeCritical":false,"IsSecurityTransparent":false,"MemberType":"TypeInfo","MetadataToken":100000000,"Module":"TooString.Specs.dll","ReflectedType":"null","TypeHandle":"System.RuntimeTypeHandle","UnderlyingSystemType":"TooString.Specs.TooStringBestEffortMakesGoodChoices","GenericTypeParameters":[],"DeclaredConstructors":[],"DeclaredEvents":[],"DeclaredFields":[],"DeclaredMembers":[],"DeclaredMethods":[],"DeclaredNestedTypes":[],"DeclaredProperties":[],"ImplementedInterfaces":[],"IsInterface":false,"IsNested":false,"IsArray":false,"IsByRef":false,"IsPointer":false,"IsGenericTypeParameter":false,"IsGenericMethodParameter":false,"IsVariableBoundArray":false,"HasElementType":false,"GenericTypeArguments":[],"Attributes":"AutoLayout, AnsiClass, Class, Public, BeforeFieldInit","IsAbstract":false,"IsImport":false,"IsSealed":false,"IsSpecialName":false,"IsClass":true,"IsNestedAssembly":false,"IsNestedFamANDAssem":false,"IsNestedFamily":false,"IsNestedFamORAssem":false,"IsNestedPrivate":false,"IsNestedPublic":false,"IsNotPublic":false,"IsPublic":true,"IsAutoLayout":true,"IsExplicitLayout":false,"IsLayoutSequential":false,"IsAnsiClass":true,"IsAutoClass":false,"IsUnicodeClass":false,"IsCOMObject":false,"IsContextful":false,"IsEnum":false,"IsMarshalByRef":false,"IsPrimitive":false,"IsValueType":false,"IsSignatureType":false,"TypeInitializer":"null","IsSerializable":false,"IsVisible":true,"CustomAttributes":[]},{"IsCollectible":false,"DeclaringMethod":"MethodBase","FullName":"TooString.Specs.TooStringCallerArgumentExpressionReturnsLiteralCode","AssemblyQualifiedName":"TooString.Specs.TooStringCallerArgumentExpressionReturnsLiteralCode, TooString.Specs, Version=X.X.X.X, Culture=neutral, PublicKeyToken=null","Namespace":"TooString.Specs","GUID":"00000000-0000-0000-0000-000000000000","GenericParameterAttributes":"GenericParameterAttributes","IsSZArray":false,"GenericParameterPosition":"Int32","ContainsGenericParameters":false,"StructLayoutAttribute":"System.Runtime.InteropServices.StructLayoutAttribute","Name":"TooStringCallerArgumentExpressionReturnsLiteralCode","DeclaringType":"null","Assembly":"TooString.Specs, Version=X.X.X.X, Culture=neutral, PublicKeyToken=null","BaseType":"System.Object","IsByRefLike":false,"IsConstructedGenericType":false,"IsGenericType":false,"IsGenericTypeDefinition":false,"IsGenericParameter":false,"IsTypeDefinition":true,"IsSecurityCritical":true,"IsSecuritySafeCritical":false,"IsSecurityTransparent":false,"MemberType":"TypeInfo","MetadataToken":100000000,"Module":"TooString.Specs.dll","ReflectedType":"null","TypeHandle":"System.RuntimeTypeHandle","UnderlyingSystemType":"TooString.Specs.TooStringCallerArgumentExpressionReturnsLiteralCode","GenericTypeParameters":[],"DeclaredConstructors":[],"DeclaredEvents":[],"DeclaredFields":[],"DeclaredMembers":[],"DeclaredMethods":[],"DeclaredNestedTypes":[],"DeclaredProperties":[],"ImplementedInterfaces":[],"IsInterface":false,"IsNested":false,"IsArray":false,"IsByRef":false,"IsPointer":false,"IsGenericTypeParameter":false,"IsGenericMethodParameter":false,"IsVariableBoundArray":false,"HasElementType":false,"GenericTypeArguments":[],"Attributes":"AutoLayout, AnsiClass, Class, Public, BeforeFieldInit","IsAbstract":false,"IsImport":false,"IsSealed":false,"IsSpecialName":false,"IsClass":true,"IsNestedAssembly":false,"IsNestedFamANDAssem":false,"IsNestedFamily":false,"IsNestedFamORAssem":false,"IsNestedPrivate":false,"IsNestedPublic":false,"IsNotPublic":false,"IsPublic":true,"IsAutoLayout":true,"IsExplicitLayout":false,"IsLayoutSequential":false,"IsAnsiClass":true,"IsAutoClass":false,"IsUnicodeClass":false,"IsCOMObject":false,"IsContextful":false,"IsEnum":false,"IsMarshalByRef":false,"IsPrimitive":false,"IsValueType":false,"IsSignatureType":false,"TypeInitializer":"null","IsSerializable":false,"IsVisible":true,"CustomAttributes":[]},{"IsCollectible":false,"DeclaringMethod":"MethodBase","FullName":"TooString.Specs.TooStringDepthOptionSpecs","AssemblyQualifiedName":"TooString.Specs.TooStringDepthOptionSpecs, TooString.Specs, Version=X.X.X.X, Culture=neutral, PublicKeyToken=null","Namespace":"TooString.Specs","GUID":"00000000-0000-0000-0000-000000000000","GenericParameterAttributes":"GenericParameterAttributes","IsSZArray":false,"GenericParameterPosition":"Int32","ContainsGenericParameters":false,"StructLayoutAttribute":"System.Runtime.InteropServices.StructLayoutAttribute","Name":"TooStringDepthOptionSpecs","DeclaringType":"null","Assembly":"TooString.Specs, Version=X.X.X.X, Culture=neutral, PublicKeyToken=null","BaseType":"System.Object","IsByRefLike":false,"IsConstructedGenericType":false,"IsGenericType":false,"IsGenericTypeDefinition":false,"IsGenericParameter":false,"IsTypeDefinition":true,"IsSecurityCritical":true,"IsSecuritySafeCritical":false,"IsSecurityTransparent":false,"MemberType":"TypeInfo","MetadataToken":100000000,"Module":"TooString.Specs.dll","ReflectedType":"null","TypeHandle":"System.RuntimeTypeHandle","UnderlyingSystemType":"TooString.Specs.TooStringDepthOptionSpecs","GenericTypeParameters":[],"DeclaredConstructors":[],"DeclaredEvents":[],"DeclaredFields":[],"DeclaredMembers":[],"DeclaredMethods":[],"DeclaredNestedTypes":[],"DeclaredProperties":[],"ImplementedInterfaces":[],"IsInterface":false,"IsNested":false,"IsArray":false,"IsByRef":false,"IsPointer":false,"IsGenericTypeParameter":false,"IsGenericMethodParameter":false,"IsVariableBoundArray":false,"HasElementType":false,"GenericTypeArguments":[],"Attributes":"Public","IsAbstract":false,"IsImport":false,"IsSealed":false,"IsSpecialName":false,"IsClass":true,"IsNestedAssembly":false,"IsNestedFamANDAssem":false,"IsNestedFamily":false,"IsNestedFamORAssem":false,"IsNestedPrivate":false,"IsNestedPublic":false,"IsNotPublic":false,"IsPublic":true,"IsAutoLayout":true,"IsExplicitLayout":false,"IsLayoutSequential":false,"IsAnsiClass":true,"IsAutoClass":false,"IsUnicodeClass":false,"IsCOMObject":false,"IsContextful":false,"IsEnum":false,"IsMarshalByRef":false,"IsPrimitive":false,"IsValueType":false,"IsSignatureType":false,"TypeInitializer":"Void .cctor()","IsSerializable":false,"IsVisible":true,"CustomAttributes":[]}],"IsFullyTrusted":true,"CustomAttributes":[{"Constructor":"Void .ctor(Int32)","ConstructorArguments":[],"NamedArguments":[],"AttributeType":"System.Runtime.CompilerServices.CompilationRelaxationsAttribute"},{"Constructor":"Void .ctor()","ConstructorArguments":[],"NamedArguments":[],"AttributeType":"System.Runtime.CompilerServices.RuntimeCompatibilityAttribute"},{"Constructor":"Void .ctor(DebuggingModes)","ConstructorArguments":[],"NamedArguments":[],"AttributeType":"System.Diagnostics.DebuggableAttribute"}],"EscapedCodeBase":"file:///--filename--","Modules":[{"MDStreamVersion":131072,"FullyQualifiedName":"--filename--","ModuleVersionId":"00000000-0000-0000-0000-000000000000","MetadataToken":100000000,"ScopeName":"TooString.Specs.dll","Name":"TooString.Specs.dll","Assembly":"TooString.Specs, Version=X.X.X.X, Culture=neutral, PublicKeyToken=null","ModuleHandle":"System.ModuleHandle","CustomAttributes":[]}],"SecurityRuleSet":"None"}
                       """;
        //A
        var optionsD3L3 = TooStringOptions.ForJson(js => js.MaxDepth = 3) with {ReflectionOptions = ReflectionOptions.Default with{MaxLength = 3}};
        var actual = value.TooString(TooStringHow.Json, optionsD3L3);

        //A
        var comparableValue = actual.RegexReplaceKnownRuntimeVariableValues();
        var expandoString = JsonSerializer.Deserialize<ExpandoObject>(actual).TooString(TooStringHow.Json, optionsD3L3);
        var comparableExpandoString = expandoString.RegexReplaceKnownRuntimeVariableValues();
        TestContext.Progress.WriteLine(comparableExpandoString);

        Assert.That(comparableValue,Is.EqualTo(comparableExpandoString));

        Assert.That(comparableValue,Is.EqualTo(expected.RegexReplaceKnownRuntimeVariableValues()));
    }


    [Test]
    public void GivenAnUnJsonableReflectionType_Module()
    {
        var value = typeof(Type)
            .GetMethods(BindingFlags.Instance|BindingFlags.Public)
            .First(m=>m.Name=="GetMethods")
            .Module;

        var expected = """
                       {"MDStreamVersion":131072,"FullyQualifiedName":"--filename--","ModuleVersionId":{},"MetadataToken":100000000,"ScopeName":"System.Private.CoreLib.dll","Name":"System.Private.CoreLib.dll","Assembly":{"CodeBase":"file:///--filename--","FullName":"System.Private.CoreLib, Version=X.X.X.X, Culture=neutral, PublicKeyToken=7cec85d7bea7798e","EntryPoint":"null","DefinedTypes":["Microsoft.CodeAnalysis.EmbeddedAttribute","System.Runtime.CompilerServices.IsUnmanagedAttribute","System.Runtime.CompilerServices.NullableAttribute"],"IsCollectible":false,"ManifestModule":{"MDStreamVersion":131072,"FullyQualifiedName":"--filename--","ModuleVersionId":"00000000-0000-0000-0000-000000000000","MetadataToken":100000000,"ScopeName":"System.Private.CoreLib.dll","Name":"System.Private.CoreLib.dll","Assembly":"System.Private.CoreLib, Version=X.X.X.X, Culture=neutral, PublicKeyToken=7cec85d7bea7798e","ModuleHandle":"System.ModuleHandle","CustomAttributes":[]},"ReflectionOnly":false,"Location":"--filename--","ImageRuntimeVersion":"v4.0.30319","GlobalAssemblyCache":false,"HostContext":0,"IsDynamic":false,"ExportedTypes":["Microsoft.Win32.SafeHandles.CriticalHandleMinusOneIsInvalid","Microsoft.Win32.SafeHandles.CriticalHandleZeroOrMinusOneIsInvalid","Microsoft.Win32.SafeHandles.SafeHandleMinusOneIsInvalid"],"IsFullyTrusted":true,"CustomAttributes":["[System.Runtime.CompilerServices.ExtensionAttribute()]","[System.Runtime.CompilerServices.CompilationRelaxationsAttribute((Int32)8)]","[System.Runtime.CompilerServices.RuntimeCompatibilityAttribute(WrapNonExceptionThrows = True)]"],"EscapedCodeBase":"file:///--filename--","Modules":["System.Private.CoreLib.dll"],"SecurityRuleSet":"None"},"ModuleHandle":{"MDStreamVersion":131072},"CustomAttributes":[{"Constructor":"Void .ctor(Boolean)","ConstructorArguments":[],"NamedArguments":[],"AttributeType":"System.Runtime.CompilerServices.NullablePublicOnlyAttribute"},{"Constructor":"Void .ctor()","ConstructorArguments":[],"NamedArguments":[],"AttributeType":"System.Runtime.CompilerServices.SkipLocalsInitAttribute"}]}
                       """;
        
        //A
        var optionsD3L3 = TooStringOptions.ForJson(js => js.MaxDepth = 3) with {ReflectionOptions = ReflectionOptions.Default with{MaxLength = 3}};
        var actual = value.TooString(TooStringHow.Json, optionsD3L3);

        //A
        var comparableValue = actual.RegexReplaceKnownRuntimeVariableValues();
        var expandoString = JsonSerializer.Deserialize<ExpandoObject>(actual).TooString(TooStringHow.Json, optionsD3L3);
        var comparableExpandoString = expandoString.RegexReplaceKnownRuntimeVariableValues();
        TestContext.Progress.WriteLine(comparableExpandoString);

        Assert.That(comparableValue,Is.EqualTo(comparableExpandoString));

        Assert.That(comparableValue,Is.EqualTo(expected.RegexReplaceKnownRuntimeVariableValues()));
    }
#endif

}

class CompositeA
{
    public string? A { get; set; } public Complex? B { get; set; }

    public override string ToString() => new { A, B }.ToString()!;
}

class Circular { public string? A { get; set; } public Circular? B { get; set; } }
