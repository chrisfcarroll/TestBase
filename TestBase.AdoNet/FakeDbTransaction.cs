using System.Data;
using System.Data.Common;

namespace TestBase.AdoNet
{
    public class FakeDbTransaction : DbTransaction
    {
        private readonly FakeDbConnection _dbConnection;

        public FakeDbTransaction(FakeDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public override void Commit()
        {
        }

        public override void Rollback()
        {
        }

        protected override DbConnection DbConnection
        {
            get { return _dbConnection; }
        }

        public override IsolationLevel IsolationLevel
        {
            get { return IsolationLevel.Chaos;}
        }
    }
}