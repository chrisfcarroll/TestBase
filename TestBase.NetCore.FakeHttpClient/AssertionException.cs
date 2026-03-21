namespace TestBase;

/// <summary>Thrown when a FakeHttpClient assertion fails.</summary>
public class AssertionException : Exception
{
    public AssertionException(string message) : base(message) { }
}
