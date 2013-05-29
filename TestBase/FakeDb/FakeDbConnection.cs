using System.Data;
using System.Data.Common;
using System.Runtime.InteropServices;

namespace XSell.Tests
{
    public class FakeDbConnection : DbConnection
    {
        public DbCommand DbCommandToReturn;

        public FakeDbConnection([Optional] DbCommand dbCommandToReturn)
        {
            DbCommandToReturn = dbCommandToReturn;
        }

        protected override DbTransaction BeginDbTransaction(IsolationLevel isolationLevel)
        {
            return new FakeDbTransaction(this);
        }

        public override void Close()
        {
        }

        public override void ChangeDatabase(string databaseName)
        {
        }

        public override void Open()
        {
        }

        public override string ConnectionString { get; set; }

        public override string Database
        {
            get { return "FakeDatabase"; }
        }

        public override ConnectionState State
        {
            get { return ConnectionState.Open; }
        }

        public override string DataSource
        {
            get { return "FakeDatasource"; }
        }

        public override string ServerVersion
        {
            get { return "FakeServerVersion"; }
        }

        protected override DbCommand CreateDbCommand()
        {
            return DbCommandToReturn;
        }
    }

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