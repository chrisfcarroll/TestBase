using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Runtime.InteropServices;

namespace TestBase.AdoNet
{
    public class FakeDbConnection : DbConnection
    {
        public Queue<FakeDbCommand> DbCommandsQueued = new Queue<FakeDbCommand>();
        public List<FakeDbCommand> Invocations = new List<FakeDbCommand>();
        ConnectionState _state= ConnectionState.Closed;

        public FakeDbConnection QueueCommand(FakeDbCommand command)
        {
            command.Connection = this;
            command.IsPretendingToBePartOfMars = IsQueueingCommandsWithPretendingToBePartOfAsMars;
            DbCommandsQueued.Enqueue(command);
            return this;
        }

        public bool IsQueueingCommandsWithPretendingToBePartOfAsMars { get; set; }

        public FakeDbConnection([Optional] FakeDbCommand dbCommandToReturn)
        {
            if(dbCommandToReturn!=null){QueueCommand(dbCommandToReturn);}
        }

        protected override DbTransaction BeginDbTransaction(IsolationLevel isolationLevel)
        {
            return new FakeDbTransaction(this);
        }

        public override void Close(){_state=ConnectionState.Open;}

        public override void ChangeDatabase(string databaseName){}

        public override void Open(){ _state=ConnectionState.Open;}

        public override string ConnectionString { get; set; }

        public override string Database => "FakeDatabase";

        public override ConnectionState State => _state;

        public override string DataSource => "FakeDatasource";

        public override string ServerVersion => "FakeServerVersion";

        protected override DbCommand CreateDbCommand()
        {
            return NextCommand();
        }

        public FakeDbCommand NextCommand()
        {
            FakeDbCommand result;
            if (!DbCommandsQueued.TryPeek(out result))
            {
                QueueCommand(FakeDbCommand.ForExecuteQuery(new string[0]));
            }
            result = DbCommandsQueued.Dequeue();
            result.ParameterCollectionToReturn = new FakeDbParameterCollection();
            return result;
        }
    }
}