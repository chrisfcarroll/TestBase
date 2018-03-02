using System;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Linq.Expressions;

namespace TestBase.AdoNet.RecordingDb
{
    public class RecordingDbCommand : DbCommand
    {
        readonly DbCommand innerCommand;
        readonly RecordingDbConnection recordingDbConnection;

        void Record(Action action) { action(); }
        T Record<T>(Func<T> funct) { return funct(); }
        void RecordE(Expression<Action> action) { action.Compile()(); }
        T RecordE<T>(Expression<Func<T>> funct) { return funct.Compile()(); }

        public RecordingDbCommand(DbCommand innerCommand, RecordingDbConnection recordingDbConnection)
        {
            this.innerCommand = innerCommand;
            this.recordingDbConnection = recordingDbConnection;
        }

        public override void Prepare()
        {
            RecordE(()=>innerCommand.Prepare());
        }

        public override string CommandText
        {
            get { return RecordE(()=>innerCommand.CommandText); } 
            set { Record(()=>innerCommand.CommandText = value); }
        }

        public override int CommandTimeout 
        { 
            get { return RecordE(()=>innerCommand.CommandTimeout); }
            set { Record(()=>innerCommand.CommandTimeout = value); }
        }

        public override CommandType CommandType 
        {
            get { return RecordE(() => innerCommand.CommandType); }
            set { Record(() => innerCommand.CommandType = value); } 
        }

        public override UpdateRowSource UpdatedRowSource 
        {
            get { return RecordE(() => innerCommand.UpdatedRowSource); }
            set { Record(() => innerCommand.UpdatedRowSource = value); } 
        }

        protected override DbConnection DbConnection
        {
            get { return RecordE(() => innerCommand.Connection); }
            set { Record(() => innerCommand.Connection = value); }
        }

        protected override DbParameterCollection DbParameterCollection
        {
            get { return innerCommand.Parameters; }
        }

        protected override DbTransaction DbTransaction
        {
            get { return RecordE(() => innerCommand.Transaction); }
            set { Record(() => innerCommand.Transaction = value); }
        }

        public override bool DesignTimeVisible { get; set; }

        public override void Cancel() { RecordE(() => innerCommand.Cancel()); }

        protected override DbParameter CreateDbParameter() { return innerCommand.CreateParameter(); }

        protected override DbDataReader ExecuteDbDataReader(CommandBehavior behavior)
        {
            RecordInvocation();
            return innerCommand.ExecuteReader(behavior);
        }

        public override int ExecuteNonQuery()
        {
            RecordInvocation();
            return RecordE(()=>innerCommand.ExecuteNonQuery());
        }

        public override object ExecuteScalar()
        {
            RecordInvocation();
            return RecordE(() => innerCommand.ExecuteScalar());
        }

        private void RecordInvocation()
        {
            var copiedParameters = new FakeDb.FakeDbParameterCollection().WithAddRange(DbParameterCollection.Cast<DbParameter>());

            Invocations.Add(new FakeDb.FakeDbCommand{
                                CommandText = innerCommand.CommandText,
                                CommandType = innerCommand.CommandType,
                            },
                            copiedParameters);

            if (recordingDbConnection!=null)
            {
                recordingDbConnection.Invocations.Add(
                    new FakeDb.FakeDbCommand{
                        CommandText = CommandText,
                        CommandTimeout = CommandTimeout,
                        CommandType = CommandType,
                        Connection = Connection,
                    }.With(c=> c.ParameterCollectionToReturn=copiedParameters)
                );
            }
        }

        public readonly FakeDb.DbCommandInvocationList Invocations = new FakeDb.DbCommandInvocationList();
    }
}