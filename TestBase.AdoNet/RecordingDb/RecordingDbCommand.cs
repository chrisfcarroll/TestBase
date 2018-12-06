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

        public readonly DbCommandInvocationList Invocations = new DbCommandInvocationList();
        readonly RecordingDbConnection recordingDbConnection;

        public RecordingDbCommand(DbCommand innerCommand, RecordingDbConnection recordingDbConnection)
        {
            this.innerCommand          = innerCommand;
            this.recordingDbConnection = recordingDbConnection;
        }

        public override string CommandText
        {
            get { return RecordE(() => innerCommand.CommandText); }
            set { Record(() => innerCommand.CommandText = value); }
        }

        public override int CommandTimeout
        {
            get { return RecordE(() => innerCommand.CommandTimeout); }
            set { Record(() => innerCommand.CommandTimeout = value); }
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

        protected override DbParameterCollection DbParameterCollection => innerCommand.Parameters;

        protected override DbTransaction DbTransaction
        {
            get { return RecordE(() => innerCommand.Transaction); }
            set { Record(() => innerCommand.Transaction = value); }
        }

        public override bool DesignTimeVisible { get; set; }

        void Record(Action                  action) { action(); }
        T    Record<T>(Func<T>              funct)  { return funct(); }
        void RecordE(Expression<Action>     action) { action.Compile()(); }
        T    RecordE<T>(Expression<Func<T>> funct)  { return funct.Compile()(); }

        public override void Prepare() { RecordE(() => innerCommand.Prepare()); }

        public override void Cancel() { RecordE(() => innerCommand.Cancel()); }

        protected override DbParameter CreateDbParameter() { return innerCommand.CreateParameter(); }

        protected override DbDataReader ExecuteDbDataReader(CommandBehavior behavior)
        {
            RecordInvocation(behavior);
            return innerCommand.ExecuteReader(behavior);
        }

        public override int ExecuteNonQuery()
        {
            RecordInvocation();
            return RecordE(() => innerCommand.ExecuteNonQuery());
        }

        public override object ExecuteScalar()
        {
            RecordInvocation();
            return RecordE(() => innerCommand.ExecuteScalar());
        }

        void RecordInvocation(CommandBehavior behavior = default(CommandBehavior))
        {
            var copiedParameters =
            new FakeDbParameterCollection().WithAddRange(DbParameterCollection.Cast<DbParameter>());

            Invocations.Add(new FakeDbCommand
                            {
                            CommandText = innerCommand.CommandText,
                            CommandType = innerCommand.CommandType
                            },
                            copiedParameters,
                            behavior);

            if (recordingDbConnection != null)
                recordingDbConnection.Invocations.Add(
                                                      new FakeDbCommand
                                                      {
                                                      CommandText    = CommandText,
                                                      CommandTimeout = CommandTimeout,
                                                      CommandType    = CommandType,
                                                      Connection     = Connection
                                                      }.With(c => c.ParameterCollectionToReturn =
                                                                  copiedParameters)
                                                     );
        }
    }
}
