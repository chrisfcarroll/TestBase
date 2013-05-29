using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using Moq;

namespace XSell.Tests
{
    public class FakeDbCommand : DbCommand
    {
        public List<FakeDbCommand> Invocations = new List<FakeDbCommand>();
        public DataTable ExecuteDbDataReaderResultTable { get; set; }
        public int ExecuteNonQueryResult=0;
        public object ExecuteScalarResult = 0;
        public Mock<DbParameterCollection> MockDbParameterCollection= new Mock<DbParameterCollection>();

        public FakeDbCommand(DataTable dbReaderTabletoReturn)
        {
            ExecuteDbDataReaderResultTable = dbReaderTabletoReturn;
        }

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
            get { return MockDbParameterCollection.Object; }
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
            Invocations.Add(new FakeDbCommand(ExecuteDbDataReaderResultTable)
                {
                    CommandText = CommandText, CommandType = CommandType
                });
            return new DataTableReader(ExecuteDbDataReaderResultTable);
        }

        public override int ExecuteNonQuery()
        {
            throw new NotImplementedException();
        }

        public override object ExecuteScalar()
        {
            throw new NotImplementedException();
        }
    }

    public class FakeDbParameter : DbParameter
    {
        public override void ResetDbType()
        {
        }

        public override DbType DbType { get; set; }
        public override ParameterDirection Direction { get; set; }
        public override bool IsNullable { get; set; }
        public override string ParameterName { get; set; }
        public override string SourceColumn { get; set; }
        public override DataRowVersion SourceVersion { get; set; }
        public override object Value { get; set; }
        public override bool SourceColumnNullMapping { get; set; }
        public override int Size { get; set; }
    }
}