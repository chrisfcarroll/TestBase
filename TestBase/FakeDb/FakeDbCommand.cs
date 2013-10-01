using System;
using System.Data;
using System.Data.Common;

namespace TestBase.FakeDb
{
    public class FakeDbCommand : DbCommand
    {
        public static FakeDbCommand ForExecuteQuery(DataTable executeDbDataReaderTabletoReturn)
        {
            return new FakeDbCommand { ExecuteQueryResultTable = executeDbDataReaderTabletoReturn };
        }

        public static FakeDbCommand ForExecuteQuery(FakeDbResultSet dbDataReaderResultSetToReturn)
        {
            return new FakeDbCommand { ExecuteQueryResultDbDataReader = dbDataReaderResultSetToReturn };
        }

        public static FakeDbCommand ForExecuteNonQuery(int rowsAffected)
        {
            return new FakeDbCommand { ExecuteNonQueryRowsAffected = rowsAffected };
        }

        public static FakeDbCommand forExecuteScalarResult(object executeScalarResult)
        {
            return new FakeDbCommand { ExecuteScalarResult = executeScalarResult };
        }

        public DbCommandInvocationList Invocations = new DbCommandInvocationList();
        public DataTable ExecuteQueryResultTable { get; set; }
        public int ExecuteNonQueryRowsAffected=0;
        public object ExecuteScalarResult = 0;
        public FakeDbParameterCollection ParameterCollectionToReturn;
        public FakeDbResultSet ExecuteQueryResultDbDataReader;

        public override void Prepare()
        {
            
        }

        public override string CommandText { get; set; }
        public override int CommandTimeout { get; set; }
        public override CommandType CommandType { get; set; }
        public override UpdateRowSource UpdatedRowSource { get; set; }
        protected override DbConnection DbConnection { get; set; }

        protected override DbParameterCollection DbParameterCollection
        {
            get { return ParameterCollectionToReturn; }
        }

        protected override DbTransaction DbTransaction { get; set; }
        public override bool DesignTimeVisible { get; set; }

        public override void Cancel()
        {
            throw new NotImplementedException();
        }

        protected override DbParameter CreateDbParameter()
        {
            return new FakeDbParameter();
        }

        protected override DbDataReader ExecuteDbDataReader(CommandBehavior behavior)
        {
            RecordInvocation();

            if (ExecuteQueryResultDbDataReader != null)
            {
                return new FakeDataReader {Resultset = ExecuteQueryResultDbDataReader};
            }
            else
            {
                return new DataTableReader(ExecuteQueryResultTable);
            }
        }

        public override int ExecuteNonQuery()
        {
            RecordInvocation();
            return ExecuteNonQueryRowsAffected;
        }

        public override object ExecuteScalar()
        {
            RecordInvocation();
            return ExecuteScalarResult;
        }

        private void RecordInvocation()
        {
            Invocations.Add(new FakeDbCommand{
                                CommandText = CommandText,
                                CommandType = CommandType,
                            },
                            ParameterCollectionToReturn);
        }
    }
}