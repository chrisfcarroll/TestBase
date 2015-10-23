using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Reflection;

namespace TestBase.FakeDb
{
    public class FakeDbCommand : DbCommand
    {
        public static FakeDbCommand ForExecuteSingleColumnQuery<T>(IEnumerable<T> dataToReturn)
        {
            return ForExecuteQuery(dataToReturn.Select(x=>new object[]{x}), "col1");
        }

        public static FakeDbCommand ForExecuteQuery(IEnumerable<object[]> dataToReturn, params string[] columnNames)
        {
            var rowToDeduceMetaData = dataToReturn.FirstOrDefault();
            FakeDbResultSet.MetaData[] metaData;
            if (rowToDeduceMetaData != null)
            {
                metaData = new FakeDbResultSet.MetaData[rowToDeduceMetaData.Length];

                for (int j = 0; j < metaData.Length; j++)
                {
                    if (columnNames.Length > j)
                    {
                        var columnName = columnNames[j];
                        var itemToDeduceMetaData = rowToDeduceMetaData[j] ?? new object();
                        metaData[j] = new FakeDbResultSet.MetaData(columnName, itemToDeduceMetaData.GetType());
                    }
                }
            }
            else if(columnNames.Length>0)
            {
                
                metaData = Enumerable.Range(1, columnNames.Length)
                              .Select(x => new FakeDbResultSet.MetaData("", typeof (object)))
                              .ToArray();

            }
            else
            {
                throw new InvalidOperationException("To set up a Query response you must either pass MetaData for the result set " +
                                                    "or pass data from which the metadata can be deduced. Can't deduce metadata from" +
                                                    "an empty dataset and no column names.");
            }
            return ForExecuteQuery(dataToReturn, metaData);
        }

        public static FakeDbCommand ForExecuteQuery(IEnumerable<object[]> dataToReturn, FakeDbResultSet.MetaData[] columns)
        {
            var rows = dataToReturn.Count();
            var fakeDbCommand = new FakeDbCommand();
            var newCaseRefDbDataReader = new FakeDbResultSet {Data = new object[rows, columns.Length]};
            int i = 0;
            foreach (var row in dataToReturn)
            {
                for (int j = 0; j < row.Length; j++)
                {
                    newCaseRefDbDataReader.Data[i, j] = row[j];
                }
                i++;
            }

            newCaseRefDbDataReader.metaData = columns;

            fakeDbCommand.ExecuteQueryResultDbDataReader = newCaseRefDbDataReader;
            return fakeDbCommand;
        }

        /// <summary>
        /// Creates a FakeDbCommand which, when executeQuery is called on it, will return a data reader 
        /// having rows of data populated from <paramref name="dataToReturn"/>
        /// and MetaData (i.e. column properties) taken from <paramref name="propertyNames"/>,
        /// </summary>
        /// <typeparam name="T">The properties of T named by <paramref name="propertyNames"/> should be compatible with your database, e.g. int, string, DateTime ... </typeparam>
        /// <param name="dataToReturn"></param>
        /// <param name="propertyNames">Will be used as the column names for the returned DataReader</param>
        /// <returns>A <see cref="FakeDbCommand"/> which will yield a DataReader containing the given <paramref name="dataToReturn"/></returns>
        public static FakeDbCommand ForExecuteQuery<T>(IEnumerable<T> dataToReturn, string[] propertyNames)
        {
            var rows = dataToReturn.Count();
            var fakeDbCommand = new FakeDbCommand();
            var newCaseRefDbDataReader = new FakeDbResultSet();
            newCaseRefDbDataReader.Data = new object[rows, propertyNames.Length];
            int i = 0;
            foreach (var row in dataToReturn)
            {
                for (int j = 0; j < propertyNames.Length; j++)
                {
                    var propertyName = propertyNames[j];
                    var propertyInfo = TypeAndReflectionExtensions.GetPropertyInfo(typeof(T), propertyName);
                    TypeAndReflectionExtensions.EnsurePropertyOrThrow<T>(propertyInfo, propertyName);
                    newCaseRefDbDataReader.Data[i, j] = TypeAndReflectionExtensions.GetPropertyValue(propertyInfo, row, propertyName);
                }
                i++;
            }

            newCaseRefDbDataReader.metaData = new FakeDbResultSet.MetaData[propertyNames.Length];

            for (int j = 0; j < propertyNames.Length; j++)
            {
                var propertyName = propertyNames[j];
                var propertyInfo = TypeAndReflectionExtensions.GetPropertyInfo(typeof(T), propertyName); ;
                TypeAndReflectionExtensions.EnsurePropertyOrThrow<T>(propertyInfo, propertyNames[j]);
                newCaseRefDbDataReader.metaData[j] = new FakeDbResultSet.MetaData(propertyInfo.Name, propertyInfo.PropertyType);
            }

            fakeDbCommand.ExecuteQueryResultDbDataReader = newCaseRefDbDataReader;
            return fakeDbCommand;
        }
        /// <summary>
        /// Creates a FakeDbCommand which, when executeQuery is called on it, will return a DataReader 
        /// having rows of data populated from <paramref name="dataToReturn"/>.
        /// Each <see cref="Tuple"/> <paramref name="dataToReturn"/> will be "flattened" in the resulting DataReader so that 
        /// the columns of the DataReader contain first the values of the properties of <paramref name="dataToReturn"/>.Item1, followed 
        /// by <paramref name="dataToReturn"/>.Item2, and MetaData (i.e. column properties) from first the reflected Properties 
        /// of <paramref name="dataToReturn"/>.Item1, followed by <paramref name="dataToReturn"/>.Item2
        /// 
        /// Only writeable ValueTypes and strings will be used: See <see cref="FakeDbRehydrationExtensions.GetDbRehydratableProperties"/> and https://github.com/chrisfcarroll/TestBase/blob/master/TestBase/FakeDb/FakeDbRehydrationExtensions.cs
        /// 
        /// </summary>
        /// <typeparam name="T">The properties of T named by <paramref name="propertyNames"/> should be compatible with your database, e.g. int, string, DateTime ... </typeparam>
        /// <param name="dataToReturn"></param>
        /// <param name="propertyNames">Will be used as the column names for the returned DataReader</param>
        /// <returns>A <see cref="FakeDbCommand"/> which will yield a DataReader containing the given <paramref name="dataToReturn"/></returns>
        public static FakeDbCommand ForExecuteQuery<T1, T2>(IEnumerable<Tuple<T1, T2>> dataToReturn)
        {
            var propertyNames1 = typeof(T1).GetDbRehydratablePropertyNames().ToArray();
            var propertyNames2 = typeof(T2).GetDbRehydratablePropertyNames().ToArray();
            var rowCount = dataToReturn.Count();
            var ColumnCount = propertyNames1.Length + propertyNames2.Length;
            var fakeDbCommand = new FakeDbCommand();
            var newDbDataReader = new FakeDbResultSet
            {
                Data = new object[rowCount, ColumnCount]
            };
            int i = 0;
            foreach (var row in dataToReturn)
            {
                int j = 0;
                for (j = 0; j < propertyNames1.Length; j++)
                {
                    var propertyName = propertyNames1[j];
                    var propertyInfo = TypeAndReflectionExtensions.GetPropertyInfo(typeof(T1), propertyName);
                    TypeAndReflectionExtensions.EnsurePropertyOrThrow<T1>(propertyInfo, propertyName);
                    newDbDataReader.Data[i, j] = TypeAndReflectionExtensions.GetPropertyValue(propertyInfo, row.Item1, propertyName);
                }
                for (int k = 0; k < propertyNames2.Length; k++)
                {
                    var propertyName = propertyNames2[k];
                    var propertyInfo = TypeAndReflectionExtensions.GetPropertyInfo(typeof(T2), propertyName);
                    TypeAndReflectionExtensions.EnsurePropertyOrThrow<T2>(propertyInfo, propertyName);
                    newDbDataReader.Data[i, j + k] = TypeAndReflectionExtensions.GetPropertyValue(propertyInfo, row.Item2, propertyName);
                }
                i++;
            }

            newDbDataReader.metaData = new FakeDbResultSet.MetaData[ColumnCount];

            for (int j = 0; j < propertyNames1.Length; j++)
            {
                var propertyName = propertyNames1[j];
                var propertyInfo = TypeAndReflectionExtensions.GetPropertyInfo(typeof(T1), propertyName); ;
                TypeAndReflectionExtensions.EnsurePropertyOrThrow<T1>(propertyInfo, propertyNames1[j]);
                newDbDataReader.metaData[j] = new FakeDbResultSet.MetaData(propertyInfo.Name, propertyInfo.PropertyType);
            }
            for (int k = 0; k < propertyNames2.Length; k++)
            {
                var propertyName = propertyNames2[k];
                var propertyInfo = TypeAndReflectionExtensions.GetPropertyInfo(typeof(T2), propertyName); ;
                TypeAndReflectionExtensions.EnsurePropertyOrThrow<T2>(propertyInfo, propertyNames2[k]);
                newDbDataReader.metaData[propertyNames1.Length + k] = new FakeDbResultSet.MetaData(propertyInfo.Name, propertyInfo.PropertyType);
            }


            fakeDbCommand.ExecuteQueryResultDbDataReader = newDbDataReader;
            return fakeDbCommand;
        }
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
            var toCancel=Invocations.LastOrDefault(ci => ci.Command.EqualsByValue(this));
            if (toCancel != null)
            {
                toCancel.CancelledAtTime = DateTime.Now;
            }
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