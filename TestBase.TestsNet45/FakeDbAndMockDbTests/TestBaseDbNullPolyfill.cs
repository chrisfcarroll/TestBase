using NUnit.Framework;

namespace TestBase.TestsNet45.FakeDbAndMockDbTests
{
    [TestFixture]
    public class TestBaseDbNullPolyfill
    {
        [Test] public void DbNullIsDbNull() { NUnit.Framework.Assert.AreSame(DBNull.Value, System.DBNull.Value); }
    }
}
