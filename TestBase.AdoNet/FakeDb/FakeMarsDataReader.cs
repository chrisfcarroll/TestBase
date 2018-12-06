using System;
using System.Collections;
using System.Data;
using System.Data.Common;

namespace TestBase.AdoNet
{
    public class FakeMarsDataReader : DbDataReader
    {
        DbDataReader internalReader;

        public FakeMarsDataReader(DbDataReader dbDataReader) { internalReader = dbDataReader; }

        public FakeMarsDataReader(DbDataReader dbDataReader, FakeDbConnection connectionForNextResult) :
        this(dbDataReader)
        {
            Connection = connectionForNextResult;
        }

        public DataTable FakeSchemaTable { get; set; } = new DataTable();

        public bool IsPretendingToBePartOfMars => Connection != null;

        public          FakeDbConnection Connection { get; set; }
        public override int              FieldCount => internalReader.FieldCount;
        public override object this[int    ordinal] => internalReader[ordinal];
        public override object this[string name] => internalReader[name];
        public override int  RecordsAffected => internalReader.RecordsAffected;
        public override bool HasRows         => internalReader.HasRows;
        public override bool IsClosed        => internalReader.IsClosed;
        public override int  Depth           => internalReader.Depth;

        public override void Close() { }

        /// <returns><see cref="FakeSchemaTable" /> which defaults to an empty DataTable</returns>
        public override DataTable GetSchemaTable() { return FakeSchemaTable; }

        public override bool NextResult()
        {
            if (IsPretendingToBePartOfMars)
            {
                internalReader = Connection.NextCommand().ExecuteDbDataReaderAsNextMarsResult();
                return internalReader != null;
            }
            else { return internalReader.NextResult(); }
        }

        public override bool GetBoolean(int ordinal) { return internalReader.GetBoolean(ordinal); }

        public override byte GetByte(int ordinal) { return internalReader.GetByte(ordinal); }

        public override long GetBytes(int ordinal, long dataOffset, byte[] buffer, int bufferOffset, int length)
        {
            return internalReader.GetBytes(ordinal, dataOffset, buffer, bufferOffset, length);
        }

        public override char GetChar(int ordinal) { return internalReader.GetChar(ordinal); }

        public override long GetChars(int ordinal, long dataOffset, char[] buffer, int bufferOffset, int length)
        {
            return internalReader.GetChars(ordinal, dataOffset, buffer, bufferOffset, length);
        }

        public override string GetDataTypeName(int ordinal) { return internalReader.GetDataTypeName(ordinal); }

        public override DateTime GetDateTime(int ordinal) { return internalReader.GetDateTime(ordinal); }

        public override decimal GetDecimal(int ordinal) { return internalReader.GetDecimal(ordinal); }

        public override double GetDouble(int ordinal) { return internalReader.GetDouble(ordinal); }

        public override Type GetFieldType(int ordinal) { return internalReader.GetFieldType(ordinal); }

        public override float GetFloat(int ordinal) { return internalReader.GetFloat(ordinal); }

        public override Guid GetGuid(int ordinal) { return internalReader.GetGuid(ordinal); }

        public override short GetInt16(int ordinal) { return internalReader.GetInt16(ordinal); }

        public override int GetInt32(int ordinal) { return internalReader.GetInt32(ordinal); }

        public override long GetInt64(int ordinal) { return internalReader.GetInt64(ordinal); }

        public override string GetName(int ordinal) { return internalReader.GetName(ordinal); }

        public override int GetOrdinal(string name) { return internalReader.GetOrdinal(name); }

        public override string GetString(int ordinal) { return internalReader.GetString(ordinal); }

        public override object GetValue(int ordinal) { return internalReader.GetValue(ordinal); }

        public override int GetValues(object[] values) { return internalReader.GetValues(values); }

        public override bool IsDBNull(int ordinal) { return internalReader.IsDBNull(ordinal); }

        public override bool Read() { return internalReader.Read(); }

        public override IEnumerator GetEnumerator() { return internalReader.GetEnumerator(); }
    }
}
