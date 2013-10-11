using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Runtime.InteropServices;

namespace TestBase.FakeDb
{
    public class FakeDbConnection : DbConnection
    {
        public Queue<FakeDbCommand> DbCommandsQueued = new Queue<FakeDbCommand>();
        public List<FakeDbCommand> Invocations = new List<FakeDbCommand>();

        [Obsolete("Used QueueCommand instead")]
        public FakeDbCommand DbCommandToReturn
        {
            get { return DbCommandsQueued.Dequeue(); }
            set { DbCommandsQueued.Enqueue(value); }
        }

        public FakeDbConnection QueueCommand(FakeDbCommand command)
        {
            DbCommandsQueued.Enqueue(command);
            return this;
        }

        public FakeDbConnection([Optional] FakeDbCommand dbCommandToReturn)
        {
            if(dbCommandToReturn!=null){QueueCommand(dbCommandToReturn);}
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
            var result = DbCommandsQueued.Dequeue();
            result.ParameterCollectionToReturn= new FakeDbParameterCollection();
            Invocations.Add(result);
            return result;
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