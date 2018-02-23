using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Globalization;

namespace TestBase.AdoNet
{
    public class FakeDbResultSet
    {
        public struct MetaData
        {
            public MetaData(string name, Type type) : this()
            {
                Name = name;
                Type = type;
                MaxSize = 0;
            }

            public string Name { get; private set; }
            public Type Type { get; private set; }
            public int MaxSize { get; private set; }

            public override bool Equals(Object obj)
            {
                return obj is MetaData && AreEqual(this, (MetaData) obj);
            }

            public override int GetHashCode()
            {
                unchecked // Overflow is fine, just wrap
                {
                    int hash = 17;
                    // Suitable nullity checks etc, of course :)
                    hash = hash * 23 + (Name ?? "").GetHashCode();
                    hash = hash * 23 + (Type ?? typeof(object)).GetHashCode();
                    hash = hash * 23 + MaxSize.GetHashCode();
                    return hash;
                }
            }

            public static bool operator ==(MetaData x, MetaData y)
            {
                return AreEqual(x, y);
            }

            private static bool AreEqual(MetaData x, MetaData y)
            {
                return x.Name == y.Name
                       && x.Type == y.Type
                       && x.MaxSize == y.MaxSize;
            }

            public static bool operator !=(MetaData x, MetaData y)
            {
                return !(x == y);
            }

            public override string ToString()
            {
                return string.Format("{{ Name:{0}, Type:{1}, MaxSize:{2} }}", Name, Type, MaxSize);
            }
        }

        public int recordsAffected;
        public MetaData[] metaData;
        public object[,] Data;
    }

    public class FakeDbResultSetReader : DbDataReader
    {
        // The DataReader should always be open when returned to the user.
        bool _fOpen = true;

        // Keep track of the results and position
        // within the resultset (starts prior to first record).
        public FakeDbResultSet Resultset;

        static int _STARTPOS = -1;
        int _nPos = _STARTPOS;

        public override int Depth => 0;

        public override bool HasRows => HasRowsSetter;

        public bool HasRowsSetter { get; set; }

        public override bool IsClosed => !_fOpen;

        public override int RecordsAffected => -1;

        public override void Close()
        {
            _fOpen = false;
        }

        public override bool NextResult() => false;

        public override bool Read()
        {
            // Return true if it is possible to advance and if you are still positioned
            // on a valid row. Because the data array in the resultset
            // is two-dimensional, you must divide by the number of columns.
            if (++_nPos >= Resultset.Data.Length / Resultset.metaData.Length)
                return false;
            else
                return true;
        }

        //public override DataTable GetSchemaTable() => base.GetSchemaTable();

        public override int FieldCount
        {
            get { return Resultset.metaData.Length; }
        }

        public override String GetName(int i)
        {
            return Resultset.metaData[i].Name;
        }

        public override String GetDataTypeName(int i)
        {
            /*
             * Usually this would return the name of the type
             * as used on the back end, for example 'smallint' or 'varchar'.
             * The sample returns the simple name of the .NET Framework type.
             */
            return Resultset.metaData[i].Type.Name;
        }

        public override IEnumerator GetEnumerator()
        {
            var rows = Resultset.Data.Length / Resultset.metaData.Length;
            var results = new List<object[]>();
            for (int i = 0; i < rows; i++)
            {
                var row = new object[Resultset.metaData.Length];
                results.Add(row);
            }
            return results.GetEnumerator();
        }

        public override Type GetFieldType(int i)
        {
            // Return the actual Type class for the data type.
            return Resultset.metaData[i].Type;
        }

        public override Object GetValue(int i)
        {
            return Resultset.Data[_nPos, i];
        }

        public override int GetValues(object[] values)
        {
            int i = 0, j = 0;
            for (; i < values.Length && j < Resultset.metaData.Length; i++, j++)
            {
                values[i] = Resultset.Data[_nPos, j];
            }

            return i;
        }

        public override int GetOrdinal(string name)
        {
            // Look for the ordinal of the column with the same name and return it.
            for (int i = 0; i < Resultset.metaData.Length; i++)
            {
                if (0 == _cultureAwareCompare(name, Resultset.metaData[i].Name))
                {
                    return i;
                }
            }

            // Throw an exception if the ordinal cannot be found.
            throw new IndexOutOfRangeException("Could not find specified column in results");
        }

        public override object this[int i]
        {
            get { return Resultset.Data[_nPos, i]; }
        }

        public override object this[String name]
        {
            // Look up the ordinal and return 
            // the value at that position.
            get { return this[GetOrdinal(name)]; }
        }

        public override bool GetBoolean(int i)
        {
            /*
             * Force the cast to return the type. InvalidCastException
             * should be thrown if the data is not already of the correct type.
             */
            return (bool) Resultset.Data[_nPos, i];
        }

        public override byte GetByte(int i)
        {
            /*
             * Force the cast to return the type. InvalidCastException
             * should be thrown if the data is not already of the correct type.
             */
            return (byte) Resultset.Data[_nPos, i];
        }

        public override long GetBytes(int i, long fieldOffset, byte[] buffer, int bufferoffset, int length)
        {
            // The sample does not support this method.
            throw new NotSupportedException("GetBytes not supported.");
        }

        public override char GetChar(int i)
        {
            /*
             * Force the cast to return the type. InvalidCastException
             * should be thrown if the data is not already of the correct type.
             */
            return (char) Resultset.Data[_nPos, i];
        }

        public override long GetChars(int i, long fieldoffset, char[] buffer, int bufferoffset, int length)
        {
            // The sample does not support this method.
            throw new NotSupportedException("GetChars not supported.");
        }

        public override Guid GetGuid(int i)
        {
            /*
             * Force the cast to return the type. InvalidCastException
             * should be thrown if the data is not already of the correct type.
             */
            return (Guid) Resultset.Data[_nPos, i];
        }

        public override Int16 GetInt16(int i)
        {
            /*
             * Force the cast to return the type. InvalidCastException
             * should be thrown if the data is not already of the correct type.
             */
            return (Int16) Resultset.Data[_nPos, i];
        }

        public override Int32 GetInt32(int i)
        {
            /*
             * Force the cast to return the type. InvalidCastException
             * should be thrown if the data is not already of the correct type.
             */
            return (Int32) Resultset.Data[_nPos, i];
        }

        public override Int64 GetInt64(int i)
        {
            /*
             * Force the cast to return the type. InvalidCastException
             * should be thrown if the data is not already of the correct type.
             */
            return (Int64) Resultset.Data[_nPos, i];
        }

        public override float GetFloat(int i)
        {
            /*
             * Force the cast to return the type. InvalidCastException
             * should be thrown if the data is not already of the correct type.
             */
            return (float) Resultset.Data[_nPos, i];
        }

        public override double GetDouble(int i)
        {
            /*
             * Force the cast to return the type. InvalidCastException
             * should be thrown if the data is not already of the correct type.
             */
            return (double) Resultset.Data[_nPos, i];
        }

        public override String GetString(int i)
        {
            /*
             * Force the cast to return the type. InvalidCastException
             * should be thrown if the data is not already of the correct type.
             */
            return (String) Resultset.Data[_nPos, i];
        }

        public override Decimal GetDecimal(int i)
        {
            /*
             * Force the cast to return the type. InvalidCastException
             * should be thrown if the data is not already of the correct type.
             */
            return (Decimal) Resultset.Data[_nPos, i];
        }

        public override DateTime GetDateTime(int i)
        {
            /*
             * Force the cast to return the type. InvalidCastException
             * should be thrown if the data is not already of the correct type.
            */
            return (DateTime) Resultset.Data[_nPos, i];
        }

        public override bool IsDBNull(int i)
        {
            return Resultset.Data[_nPos, i] == DBNull.Value;
        }

        /*
         * Implementation specific methods.
         */
        private int _cultureAwareCompare(string strA, string strB)
        {
            return CultureInfo.CurrentCulture.CompareInfo.Compare(strA, strB, CompareOptions.IgnoreKanaType | CompareOptions.IgnoreWidth | CompareOptions.IgnoreCase);
        }
    }

    public class FakeMarsDataReader : DbDataReader
    {
        DbDataReader internalReader;

        public FakeMarsDataReader(DbDataReader dbDataReader){ internalReader = dbDataReader; }
        public FakeMarsDataReader(DbDataReader dbDataReader, FakeDbConnection connectionForNextResult):this(dbDataReader)
        {
            Connection = connectionForNextResult;
        }

        public override bool NextResult()
        {
            if (IsPretendingToBePartOfMars)
            {
                internalReader=Connection.NextCommand().ExecuteDbDataReaderAsNextMarsResult();
                return internalReader!=null;
            }
            else { return internalReader.NextResult(); }
        }

        public bool IsPretendingToBePartOfMars => Connection != null;

        public FakeDbConnection Connection { get; set; }

        public override bool GetBoolean(int ordinal) => internalReader.GetBoolean(ordinal);
        public override byte GetByte(int ordinal) => internalReader.GetByte(ordinal);
        public override long GetBytes(int ordinal, long dataOffset, byte[] buffer, int bufferOffset, int length) => internalReader.GetBytes(ordinal, dataOffset, buffer, bufferOffset, length);
        public override char GetChar(int ordinal) => internalReader.GetChar(ordinal);
        public override long GetChars(int ordinal, long dataOffset, char[] buffer, int bufferOffset, int length) => internalReader.GetChars(ordinal, dataOffset, buffer, bufferOffset, length);
        public override string GetDataTypeName(int ordinal) => internalReader.GetDataTypeName(ordinal);
        public override DateTime GetDateTime(int ordinal) => internalReader.GetDateTime(ordinal);
        public override decimal GetDecimal(int ordinal) => internalReader.GetDecimal(ordinal);
        public override double GetDouble(int ordinal) => internalReader.GetDouble(ordinal);
        public override Type GetFieldType(int ordinal) => internalReader.GetFieldType(ordinal);
        public override float GetFloat(int ordinal) => internalReader.GetFloat(ordinal);
        public override Guid GetGuid(int ordinal) => internalReader.GetGuid(ordinal);
        public override short GetInt16(int ordinal) => internalReader.GetInt16(ordinal);
        public override int GetInt32(int ordinal) => internalReader.GetInt32(ordinal);
        public override long GetInt64(int ordinal) => internalReader.GetInt64(ordinal);
        public override string GetName(int ordinal) => internalReader.GetName(ordinal);
        public override int GetOrdinal(string name) => internalReader.GetOrdinal(name);
        public override string GetString(int ordinal) => internalReader.GetString(ordinal);
        public override object GetValue(int ordinal) => internalReader.GetValue(ordinal);
        public override int GetValues(object[] values) => internalReader.GetValues(values);
        public override bool IsDBNull(int ordinal) => internalReader.IsDBNull(ordinal);
        public override int FieldCount => internalReader.FieldCount;
        public override object this[int ordinal] => internalReader[ordinal];
        public override object this[string name] => internalReader[name];
        public override int RecordsAffected => internalReader.RecordsAffected;
        public override bool HasRows => internalReader.HasRows;
        public override bool IsClosed => internalReader.IsClosed;
        public override bool Read() => internalReader.Read();
        public override int Depth => internalReader.Depth;
        public override IEnumerator GetEnumerator() => internalReader.GetEnumerator();
    }
}