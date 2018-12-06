using System.Data;
using System.Data.Common;

namespace TestBase.AdoNet
{
    public class FakeDbTransaction : DbTransaction
    {
        readonly FakeDbConnection _dbConnection;

        public FakeDbTransaction(FakeDbConnection dbConnection) { _dbConnection = dbConnection; }

        protected override DbConnection DbConnection => _dbConnection;

        public override IsolationLevel IsolationLevel => IsolationLevel.Chaos;

        public override void Commit() { }

        public override void Rollback() { }
    }
}
