using System;
using System.Linq;
using System.Reflection;
using NUnit.Framework;


namespace TestBase.Tests.AssertionFailureDisplay;

[TestFixture]
public class ShouldDisplayActualAndAssertionNameAndComparator
{
    [Test]
    public void GivenAValue()
    {
        var ass= Assert.Throws<Assertion>(
            () => 1.ShouldBe(2)
        );
        
        TestContext.WriteLine("—actual—");
        TestContext.WriteLine(ass);
        TestContext.WriteLine("^^^");
        
        ass.ToString()
            .ShouldContain("1")
            .ShouldContain("ShouldBe 2");
    }
    
    [Test, Ignore("TODO")]
    public void GivenLiteralValues()
    {
        var ass= Assert.Throws<Assertion>(
            () => 1.ShouldBe(2)
        );

        TestContext.WriteLine(ass);

        var actual = ass.ToString();

        actual.RegexReplaceWhitespaceAndBlankOutGuids()
            .ShouldBe(
                """
                Failed : 
                Actual : 
                ----------------------------
                1
                ----------------------------
                Asserted : ShouldBe
                x => x != null && x.Equals(expected)
                expected   →   2
                """
                    .RegexReplaceWhitespaceAndBlankOutGuids());
    }
    
    [Test, Ignore("TODO")]
    public void GivenVariableValues()
    {
        var namedActual=1;
        var namedExpected = 2;
        
        var ass= Assert.Throws<Assertion>(
            () => namedActual.ShouldBe(namedExpected)
        );

        TestContext.WriteLine(ass);

        var assActual = ass.ToString();

        assActual.RegexReplaceWhitespaceAndBlankOutGuids()
            .ShouldBe(
                $"""
             Failed :
             Actual :
             ----------------------------
             {namedActual}
             namedActual
             ----------------------------
             Asserted : ShouldBe
             x => x != null && x.Equals(expected)
             expected   →   {namedExpected}
             """
                    .RegexReplaceWhitespaceAndBlankOutGuids());
    }
    
    [Test, Ignore("TODO")]
    public void GivenExpressions()
    {
        var ass= Assert.Throws<Assertion>(
            () => (1 + 1).ShouldBe(2+2)
        );

        TestContext.WriteLine(ass);

        var actual = ass.ToString();

        actual.RegexReplaceWhitespaceAndBlankOutGuids()
            .ShouldBe(
                """
                    Failed :
                    Actual :
                    ----------------------------
                    2
                    1 + 1
                    ----------------------------
                    Asserted : ShouldBe
                    x => x != null && x.Equals(expected)
                    expected   →   4
                    """
                    .RegexReplaceWhitespaceAndBlankOutGuids());
    }
    
    [Test, Ignore("TODO")]
    public void GivenObjectExpression()
    {
        var ass= Assert.Throws<Assertion>(
            () => (new {A=1,B=2}).ShouldBe(new {A=2, B=3})
        );

        TestContext.WriteLine(ass);

        var actual = ass.ToString();

        actual.RegexReplaceWhitespaceAndBlankOutGuids()
            .ShouldBe(
                """
                Failed : 
                Actual : 
                ----------------------------
                { A = 1, B = 2 }
                new {A=1,B=2}
                ----------------------------
                Asserted : ShouldBe
                x => x != null && x.Equals(expected)
                expected   →   new {
                              A = 2,
                              B = 3,
                          }
                
                """
                .RegexReplaceWhitespaceAndBlankOutGuids());
    }
     
    [Test, Ignore("TODO")]
    public void GivenEnumerableExpression()
    {
        var ass= Assert.Throws<Assertion>(
            () => (new System.Collections.Generic.List<AClass>
                {
                    new AClass(){Name = "Name",Id=1},
                    new AClass(){Name = "Name2",Id=2},
                })
                .ShouldBe(
                    new System.Collections.Generic.List<AClass>
                    {
                        new AClass(){Name = "Name3",Id=3},
                        new AClass(){Name = "Name4",Id=4},
                    }
                    )
        );

        TestContext.WriteLine(ass);

        var actual = ass.ToString();

        actual.RegexReplaceWhitespaceAndBlankOutGuids()
            .ShouldBe(
                """
                Failed : 
                Actual : 
                ----------------------------
                { Capacity = 4, Count = 2, Item = AClass }
                new System.Collections.Generic.List<AClass>
                                {
                                    new AClass(){Name = "Name",Id=1},
                                    new AClass(){Name = "Name2",Id=2},
                                }
                ----------------------------
                Asserted : ShouldBe
                x => x != null && x.Equals(expected)
                expected   →   {TestBase.Tests.AClass, TestBase.Tests.AClass}
                """
                    .RegexReplaceWhitespaceAndBlankOutGuids());
    }
      
    [Test, Ignore("TODO")]
    public void GivenUnserializableExpression()
    {
        var ass= Assert.Throws<Assertion>(
            () => (Assembly.GetExecutingAssembly().Modules.FirstOrDefault())
                .ShouldBe(
                    Assembly.GetExecutingAssembly().Modules.Skip(1).FirstOrDefault()
                )
        );

        TestContext.WriteLine(ass.ToString().RegexReplaceKnownRuntimeVariableValues());

        var actual = ass.ToString();

        actual
            .RegexReplaceWhitespaceAndBlankOutGuids()
            .RegexReplaceKnownRuntimeVariableValues()
            .ShouldBe(
                """
                Failed : 
                Actual : 
                ----------------------------
                { MDStreamVersion = 131072, FullyQualifiedName = --filename--, ModuleVersionId = {  }, MetadataToken = 100000000, ScopeName = TestBase.Tests.dll, Name = TestBase.Tests.dll, Assembly = { CodeBase = file:///--filename--, FullName = TestBase.Tests, Version=4.1.3.1, Culture=neutral, PublicKeyToken=null, EntryPoint = { Name = Main, DeclaringType = AutoGeneratedProgram, ReflectedType = AutoGeneratedProgram, MemberType = Method, MetadataToken = 100000000, Module = { MDStreamVersion = 131072, FullyQualifiedName = --filename--, ModuleVersionId = 00000000-0000-0000-0000-000000000000, MetadataToken = 100000000, ScopeName = TestBase.Tests.dll, Name = TestBase.Tests.dll, Assembly = TestBase.Tests, Version=4.1.3.1, Culture=neutral, PublicKeyToken=null, ModuleHandle = System.ModuleHandle, CustomAttributes = System.Collections.ObjectModel.ReadOnlyCollection`1[System.Reflection.CustomAttributeData] }, IsSecurityCritical = True, IsSecuritySafeCritical = False, IsSecurityTransparent = False, MethodHandle = { Value = 9999999999 }, Attributes = PrivateScope, Private, Static, HideBySig, CallingConvention = Standard, ReturnType = System.Void, ReturnTypeCustomAttributes = { ParameterType = System.Void, Name = null, HasDefaultValue = True, DefaultValue = null, RawDefaultValue = null, MetadataToken = 100000000, Attributes = None, Member = Void Main(System.String[]), Position = -1, IsIn = False, IsLcid = False, IsOptional = False, IsOut = False, IsRetval = False, CustomAttributes = [] }, ReturnParameter = { ParameterType = System.Void, Name = null, HasDefaultValue = True, DefaultValue = null, RawDefaultValue = null, MetadataToken = 100000000, Attributes = None, Member = Void Main(System.String[]), Position = -1, IsIn = False, IsLcid = False, IsOptional = False, IsOut = False, IsRetval = False, CustomAttributes = [] }, IsCollectible = False, IsGenericMethod = False, IsGenericMethodDefinition = False, ContainsGenericParameters = False, MethodImplementationFlags = Managed, IsAbstract = False, IsConstructor = False, IsFinal = False, IsHideBySig = True, IsSpecialName = False, IsStatic = True, IsVirtual = False, IsAssembly = False, IsFamily = False, IsFamilyAndAssembly = False, IsFamilyOrAssembly = False, IsPrivate = True, IsPublic = False, IsConstructedGenericMethod = False, CustomAttributes = { Length = 0, LongLength = 0, Rank = 1, SyncRoot = [], IsReadOnly = False, IsFixedSize = True, IsSynchronized = False } }, DefinedTypes = { Length = 162, LongLength = 162, Rank = 1, SyncRoot = { Length = 162, LongLength = 162, Rank = 1, SyncRoot = [], IsReadOnly = False, IsFixedSize = True, IsSynchronized = False }, IsReadOnly = False, IsFixedSize = True, IsSynchronized = False }, IsCollectible = False, ManifestModule = { MDStreamVersion = 131072, FullyQualifiedName = --filename--, ModuleVersionId = {  }, MetadataToken = 100000000, ScopeName = TestBase.Tests.dll, Name = TestBase.Tests.dll, Assembly = { CodeBase = file:///--filename--, FullName = TestBase.Tests, Version=4.1.3.1, Culture=neutral, PublicKeyToken=null, EntryPoint = Void Main(System.String[]), DefinedTypes = [], IsCollectible = False, ManifestModule = TestBase.Tests.dll, ReflectionOnly = False, Location = --filename--, ImageRuntimeVersion = v4.0.30319, GlobalAssemblyCache = False, HostContext = 0, IsDynamic = False, ExportedTypes = [], IsFullyTrusted = True, CustomAttributes = System.Collections.ObjectModel.ReadOnlyCollection`1[System.Reflection.CustomAttributeData], EscapedCodeBase = file:///--filename--, Modules = [], SecurityRuleSet = None }, ModuleHandle = { MDStreamVersion = 131072 }, CustomAttributes = { Count = 1, Item = CustomAttributeData } }, ReflectionOnly = False, Location = --filename--, ImageRuntimeVersion = v4.0.30319, GlobalAssemblyCache = False, HostContext = 0, IsDynamic = False, ExportedTypes = { Length = 57, LongLength = 57, Rank = 1, SyncRoot = { Length = 57, LongLength = 57, Rank = 1, SyncRoot = [], IsReadOnly = False, IsFixedSize = True, IsSynchronized = False }, IsReadOnly = False, IsFixedSize = True, IsSynchronized = False }, IsFullyTrusted = True, CustomAttributes = { Count = 11, Item = CustomAttributeData }, EscapedCodeBase = file:///--filename--, Modules = { Length = 1, LongLength = 1, Rank = 1, SyncRoot = { Length = 1, LongLength = 1, Rank = 1, SyncRoot = [], IsReadOnly = False, IsFixedSize = True, IsSynchronized = False }, IsReadOnly = False, IsFixedSize = True, IsSynchronized = False }, SecurityRuleSet = None }, ModuleHandle = { MDStreamVersion = 131072 }, CustomAttributes = { Count = 1, Item = CustomAttributeData } }
                Assembly.GetExecutingAssembly().Modules.FirstOrDefault()
                ----------------------------
                Asserted : ShouldBe
                x => x == null
                """
                .RegexReplaceWhitespaceAndBlankOutGuids()
                .RegexReplaceKnownRuntimeVariableValues());
    }
 }

static class RegexTestExtension
{
    internal static string? RegexReplaceKnownRuntimeVariableValues(this string? value) 
        => value?
            .ReplaceRegex("\"file:///[^\"]+\"","\"file:///--filename--\"")
            .ReplaceRegex("\"[BCD]:\\[^\"]+\"","\"--filename--\"")
            .ReplaceRegex("\"[BCD]:\\\\[^\"]+\"","\"--filename--\"")
            .ReplaceRegex("\"(/[^/\"]+)+\"","\"--filename--\"")
            .ReplaceRegex("\"Value\":\\d+","\"Value\":9999999999")
            .ReplaceRegex("\"MetadataToken\":\\d+","\"MetadataToken\":100000000")
        
            .ReplaceRegex(" (/[^\" ,]+)+,"," --filename--,")
            .ReplaceRegex(" file:///[^\" ,]+,"," file:///--filename--,")
            .ReplaceRegex(" [BCD]:\\[^,\"]+,"," --filename--,")
            .ReplaceRegex(" [BCD]:\\\\[^,\"]+,"," --filename--,")
            .ReplaceRegex(" Value = \\d+"," Value = 9999999999")
            .ReplaceRegex(" MetadataToken = \\d+"," MetadataToken = 100000000")
        
            .ReplaceRegex("[a-f0-9A-F\\-]{36}",System.Guid.Empty.ToString());
}