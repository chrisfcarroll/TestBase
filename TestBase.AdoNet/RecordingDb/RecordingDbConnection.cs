using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq.Expressions;

namespace TestBase.AdoNet.RecordingDb
{
    public class RecordingDbConnection : DbConnection
    {
        public readonly List<FakeDbCommand> Invocations = new List<FakeDbCommand>();
        public readonly DbConnection innerConnection;

        public RecordingDbConnection(DbConnection innerConnection) 
        {
            this.innerConnection = innerConnection;
        }

        void Record(Action action) { action(); }
        T Record<T>(Func<T> funct) { return funct(); }
        void RecordE(Expression<Action> action) { action.Compile()(); }
        T RecordE<T>(Expression<Func<T>> funct) { return funct.Compile()(); }

        protected override DbTransaction BeginDbTransaction(IsolationLevel isolationLevel)
        {
            return RecordE(() => innerConnection.BeginTransaction(isolationLevel));
        }

        public override void Close() { RecordE(() => innerConnection.Close()); }

        public override void ChangeDatabase(string databaseName) { RecordE(() => innerConnection.ChangeDatabase(databaseName)); }

        public override void Open() { RecordE(() => innerConnection.Open()); ;}

        public override string ConnectionString 
        { 
            get { return RecordE(() => innerConnection.ConnectionString); }
            set { Record(() => innerConnection.ConnectionString = value); } }

        public override string Database { get { return RecordE(()=>innerConnection.Database); } }

        public override ConnectionState State { get { return RecordE(() => innerConnection.State); } }

        public override string DataSource { get { return RecordE(() => innerConnection.DataSource); } }

        public override string ServerVersion { get { return RecordE(() => innerConnection.ServerVersion); } }

        protected override DbCommand CreateDbCommand()
        {
            var innerCommand = RecordE(() => innerConnection.CreateCommand());
            return new RecordingDbCommand(innerCommand,this);
        }
    }
}