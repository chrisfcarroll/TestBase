using System.Dynamic;
using System.Numerics;
using System.Reflection;
using Newtonsoft.Json;

namespace TooString.Specs;

public class TooStringJsonReturnsJson
{
    static readonly HttpClient httpClient = new HttpClient() { BaseAddress = new Uri("http://127.0.0.1") };

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
    public void GivenAnUnJsonableReflectionType()
    {
        var value = typeof(Type)
            .GetMethods(BindingFlags.Instance|BindingFlags.Public)
            .First(m=>m.Name=="GetMethods");

        var expected = "{\"Type\":\"System.Reflection.MethodInfo\",\"Name\":\"GetMethods\",\"DeclaringType\":\"System.Type\",\"ReflectedType\":\"System.Type\",\"MemberType\":\"Method\",\"MetadataToken\":\"100665554\",\"Module\":\"System.Private.CoreLib.dll\",\"IsSecurityCritical\":\"True\",\"IsSecuritySafeCritical\":\"False\",\"IsSecurityTransparent\":\"False\",\"MethodHandle\":\"System.RuntimeMethodHandle\",\"Attributes\":\"PrivateScope, Public, HideBySig\",\"CallingConvention\":\"Standard, HasThis\",\"ReturnType\":\"System.Reflection.MethodInfo[]\",\"ReturnTypeCustomAttributes\":\"System.Reflection.MethodInfo[]\",\"ReturnParameter\":\"System.Reflection.MethodInfo[]\",\"IsCollectible\":\"False\",\"IsGenericMethod\":\"False\",\"IsGenericMethodDefinition\":\"False\",\"ContainsGenericParameters\":\"False\",\"MethodImplementationFlags\":\"Managed\",\"IsAbstract\":\"False\",\"IsConstructor\":\"False\",\"IsFinal\":\"False\",\"IsHideBySig\":\"True\",\"IsSpecialName\":\"False\",\"IsStatic\":\"False\",\"IsVirtual\":\"False\",\"IsAssembly\":\"False\",\"IsFamily\":\"False\",\"IsFamilyAndAssembly\":\"False\",\"IsFamilyOrAssembly\":\"False\",\"IsPrivate\":\"False\",\"IsPublic\":\"True\",\"IsConstructedGenericMethod\":\"False\",\"CustomAttributes\":\"System.Collections.ObjectModel.ReadOnlyCollection\\u00601[System.Reflection.CustomAttributeData]\"}";

        var actual = value.TooString(TooStringMethod.SystemTextJson);

        TestContext.Progress.WriteLine(actual);
        
        Assert.That(
            value.TooString(TooStringMethod.SystemTextJson), 
            Is.EqualTo(expected) 
        );

        Assert.That(
            System.Text.Json.JsonSerializer.Deserialize<ExpandoObject>(actual)
                .TooString(TooStringMethod.SystemTextJson)  ,
            
            Is.EqualTo(expected)
            );
        
        
    }
}

class CompositeA { public string A { get; set; } public Complex B { get; set; } }

class Circular { public string A { get; set; } public Circular B { get; set; } }