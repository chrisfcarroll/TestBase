using System;
using System.Collections;
using System.Data;
using System.Data.Common;

namespace TestBase.AdoNet
{
    public class FakeMarsDataReader : DbDataReader
    {
        DbDataReader internalReader;

        public FakeMarsDataReader(DbDataReader dbDataReader){ internalReader = dbDataReader; }
        public FakeMarsDataReader(DbDataReader dbDataReader, FakeDbConnection connectionForNextResult):this(dbDataReader)
        {
            Connection = connectionForNextResult;
        }

        public override void Close() {}

        /// <returns><see cref="FakeSchemaTable"/> which defaults to an empty DataTable</returns>
        public override DataTable GetSchemaTable() => FakeSchemaTable;
        public DataTable FakeSchemaTable { get; set; } = new DataTable();

        public override bool NextResult()
        {
            if (IsPretendingToBePartOfMars)
            {
                internalReader=Connection.NextCommand().ExecuteDbDataReaderAsNextMarsResult();
                return internalReader !=null;
            }
            else { return internalReader.NextResult(); }
        }

        public bool IsPretendingToBePartOfMars => Connection != null;

        public FakeDbConnection Connection { get; set; }

        public override bool     GetBoolean(int ordinal)                                                             => internalReader.GetBoolean(ordinal);
        public override byte     GetByte(int ordinal)                                                                => internalReader.GetByte(ordinal);
        public override long     GetBytes(int ordinal, long dataOffset, byte[] buffer, int bufferOffset, int length) => internalReader.GetBytes(ordinal, dataOffset, buffer, bufferOffset, length);
        public override char     GetChar(int ordinal)                                                                => internalReader.GetChar(ordinal);
        public override long     GetChars(int ordinal, long dataOffset, char[] buffer, int bufferOffset, int length) => internalReader.GetChars(ordinal, dataOffset, buffer, bufferOffset, length);
        public override string   GetDataTypeName(int ordinal)                                                        => internalReader.GetDataTypeName(ordinal);
        public override DateTime GetDateTime(int ordinal)                                                            => internalReader.GetDateTime(ordinal);
        public override decimal  GetDecimal(int ordinal)                                                             => internalReader.GetDecimal(ordinal);
        public override double   GetDouble(int ordinal)                                                              => internalReader.GetDouble(ordinal);
        public override Type     GetFieldType(int ordinal)                                                           => internalReader.GetFieldType(ordinal);
        public override float    GetFloat(int ordinal)                                                               => internalReader.GetFloat(ordinal);
        public override Guid     GetGuid(int ordinal)                                                                => internalReader.GetGuid(ordinal);
        public override short    GetInt16(int ordinal)                                                               => internalReader.GetInt16(ordinal);
        public override int      GetInt32(int ordinal)                                                               => internalReader.GetInt32(ordinal);
        public override long     GetInt64(int ordinal)                                                               => internalReader.GetInt64(ordinal);
        public override string   GetName(int ordinal)                                                                => internalReader.GetName(ordinal);
        public override int      GetOrdinal(string name)                                                             => internalReader.GetOrdinal(name);
        public override string   GetString(int ordinal)                                                              => internalReader.GetString(ordinal);
        public override object   GetValue(int ordinal)                                                               => internalReader.GetValue(ordinal);
        public override int      GetValues(object[] values)                                                          => internalReader.GetValues(values);
        public override bool     IsDBNull(int ordinal)                                                               => internalReader.IsDBNull(ordinal);
        public override int      FieldCount                                                                          => internalReader.FieldCount;
        public override object this[int ordinal] => internalReader[ordinal];
        public override object this[string name] => internalReader[name];
        public override int         RecordsAffected => internalReader.RecordsAffected;
        public override bool        HasRows         => internalReader.HasRows;
        public override bool        IsClosed        => internalReader.IsClosed;
        public override bool        Read()          => internalReader.Read();
        public override int         Depth           => internalReader.Depth;
        public override IEnumerator GetEnumerator() => internalReader.GetEnumerator();
    }
}