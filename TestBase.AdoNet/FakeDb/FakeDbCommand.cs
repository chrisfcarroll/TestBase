using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace TestBase.AdoNet.FakeDb
{
    public class FakeDbCommand : DbCommand
    {
        public FakeDbCommand(){}

        public FakeDbCommand(DbConnection connection)
        {
            Debug.Assert(connection!=null);
            Connection = connection;
        }

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
                    newCaseRefDbDataReader.Data[i, j] = propertyInfo.GetPropertyValue(row, propertyName);
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
            var itemTypes = new[]
            {
                new Tuple<Type,PropertyInfo>(typeof (T1), typeof(Tuple<T1, T2>).GetProperty("Item1")),
                new Tuple<Type,PropertyInfo>(typeof (T2), typeof(Tuple<T1, T2>).GetProperty("Item2")),
            };
            return ForExecuteQueryReturningDataFromTuples(dataToReturn,itemTypes);
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
        public static FakeDbCommand ForExecuteQuery<T1, T2,T3>(IEnumerable<Tuple<T1, T2, T3>> dataToReturn)
        {
            var itemTypes = new[]
            {
                new Tuple<Type,PropertyInfo>(typeof (T1), typeof(Tuple<T1, T2>).GetProperty("Item1")),
                new Tuple<Type,PropertyInfo>(typeof (T2), typeof(Tuple<T1, T2>).GetProperty("Item2")),
                new Tuple<Type,PropertyInfo>(typeof (T3), typeof(Tuple<T1, T2, T3>).GetProperty("Item3")),
            };
            return ForExecuteQueryReturningDataFromTuples(dataToReturn, itemTypes);
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
        public static FakeDbCommand ForExecuteQuery<T1, T2, T3, T4>(IEnumerable<Tuple<T1, T2, T3, T4>> dataToReturn)
        {
            var itemTypes = new[]
            {
                new Tuple<Type,PropertyInfo>(typeof (T1), typeof(Tuple<T1, T2>).GetProperty("Item1")),
                new Tuple<Type,PropertyInfo>(typeof (T2), typeof(Tuple<T1, T2>).GetProperty("Item2")),
                new Tuple<Type,PropertyInfo>(typeof (T3), typeof(Tuple<T1, T2, T3>).GetProperty("Item3")),
                new Tuple<Type,PropertyInfo>(typeof (T3), typeof(Tuple<T1, T2, T3>).GetProperty("Item4")),
            };
            return ForExecuteQueryReturningDataFromTuples(dataToReturn, itemTypes);
        }

        public static FakeDbCommand ForExecuteQueryReturningDataFromTuples<TTupleN>(IEnumerable<TTupleN> dataToReturn, IEnumerable<Tuple<Type, PropertyInfo>> itemTypes)
        {
            var propertyNames = itemTypes.Select(t => t.Item1.GetDbRehydratablePropertyNames());
            var ColumnCount = propertyNames.Sum(pn => pn.Count());
            var rowCount = dataToReturn.Count();
            var newDbDataReader = new FakeDbResultSet
            {
                Data = new object[rowCount, ColumnCount],
                metaData = new FakeDbResultSet.MetaData[ColumnCount]
            };
            int i = 0, j = 0;
            foreach (var row in dataToReturn)
            {
                j = 0;
                foreach (var typeAndProp in itemTypes)
                {
                    var t = typeAndProp.Item1;
                    var rowItem = typeAndProp.Item2.GetValue(row,null);
                    foreach (var propertyName in t.GetDbRehydratablePropertyNames())
                    {
                        var propertyInfo = TypeAndReflectionExtensions.GetPropertyInfo(t, propertyName);
                        TypeAndReflectionExtensions.EnsurePropertyOrThrow(t, propertyInfo, propertyName);
                        newDbDataReader.Data[i, j] = TypeAndReflectionExtensions.GetPropertyValue(propertyInfo, rowItem, propertyName);
                        newDbDataReader.metaData[j] = new FakeDbResultSet.MetaData(propertyInfo.Name, propertyInfo.PropertyType);
                        ++j;
                    }
                }
                i++;
            }

            return new FakeDbCommand
            {
                ExecuteQueryResultDbDataReader = newDbDataReader
            };
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
            var copiedParameters = new FakeDbParameterCollection().WithAddRange(ParameterCollectionToReturn.Cast<FakeDbParameter>());

            Invocations.Add(new FakeDbCommand{
                                CommandText = CommandText,
                                CommandType = CommandType,
                            },
                            copiedParameters);

            if (Connection is FakeDbConnection)
            {
                (Connection as FakeDbConnection).Invocations.Add(new FakeDbCommand
                {
                    CommandText = CommandText,
                    CommandTimeout = CommandTimeout,
                    CommandType = CommandType,
                    DbConnection = DbConnection,

                }.With(c=> c.ParameterCollectionToReturn=copiedParameters));
            }
        }
    }
}