namespace TestBase.AdoNet.Tests;

[TestFixture]
public class TestBaseDbNullPolyfill
{
    [Test] public void DbNullIsDbNull() { NUnit.Framework.Assert.AreSame(DBNull.Value, System.DBNull.Value); }
}