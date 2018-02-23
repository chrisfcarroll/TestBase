using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Runtime.InteropServices;

namespace TestBase.AdoNet.FakeDb
{
    public class FakeDbConnection : DbConnection
    {
        public Queue<FakeDbCommand> DbCommandsQueued = new Queue<FakeDbCommand>();
        public List<FakeDbCommand> Invocations = new List<FakeDbCommand>();
        ConnectionState _state= ConnectionState.Closed;

        public FakeDbConnection QueueCommand(FakeDbCommand command)
        {
            command.Connection = this;
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

        public override void Close(){_state=ConnectionState.Open;}

        public override void ChangeDatabase(string databaseName){}

        public override void Open(){ _state=ConnectionState.Open;}

        public override string ConnectionString { get; set; }

        public override string Database { get { return "FakeDatabase"; } }

        public override ConnectionState State { get { return _state; } }

        public override string DataSource { get { return "FakeDatasource"; } }

        public override string ServerVersion { get { return "FakeServerVersion"; } }

        protected override DbCommand CreateDbCommand()
        {
            var result =  DbCommandsQueued.Any() ? DbCommandsQueued.Dequeue() : new FakeDbCommand{Connection = this};
            result.ParameterCollectionToReturn= new FakeDbParameterCollection();
            return result;
        }
    }
}