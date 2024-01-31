using NUnit.Framework;

namespace TestBase.Tests.FakeDbAndMockDbTests;

[TestFixture]
public class TestBaseDbNullPolyfill
{
    [Test] public void DbNullIsDbNull() { NUnit.Framework.Assert.AreSame(DBNull.Value, System.DBNull.Value); }
}