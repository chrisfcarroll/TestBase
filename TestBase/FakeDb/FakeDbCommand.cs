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
        public static FakeDbCommand ForExecuteQuery<T>(IEnumerable<T> dataToReturn)
        {
            var rows = dataToReturn.Count();
            var fakeDbCommand = new FakeDbCommand();
            var newCaseRefDbDataReader = new FakeDbResultSet();
            newCaseRefDbDataReader.Data = new object[rows, 1];
            int i = 0;
            foreach (var item in dataToReturn)
            {
                newCaseRefDbDataReader.Data[i, 0] = item;
            }
            newCaseRefDbDataReader.metaData = new[] { new FakeDbResultSet.MetaData("col1", typeof(T)) };

            fakeDbCommand.ExecuteQueryResultDbDataReader = newCaseRefDbDataReader;
            return fakeDbCommand;
        }

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
                    var propertyInfo = GetPropertyInfo(propertyName, typeof(T));
                    EnsurePropertyOrThrow<T>(propertyInfo, propertyName);
                    newCaseRefDbDataReader.Data[i, j] = GetPropertyValue(propertyInfo, row, propertyName);
                }
                i++;
            }

            newCaseRefDbDataReader.metaData = new FakeDbResultSet.MetaData[propertyNames.Length];

            for (int j = 0; j < propertyNames.Length; j++)
            {
                var propertyName = propertyNames[j];
                var propertyInfo = GetPropertyInfo(propertyName, typeof(T)); ;
                EnsurePropertyOrThrow<T>(propertyInfo, propertyNames[j]);
                newCaseRefDbDataReader.metaData[j] = new FakeDbResultSet.MetaData(propertyInfo.Name, propertyInfo.PropertyType);
            }

            fakeDbCommand.ExecuteQueryResultDbDataReader = newCaseRefDbDataReader;
            return fakeDbCommand;
        }

        private static object GetPropertyValue<T>(PropertyInfo propertyInfo, T obj, string propertyName)
        {
            var nestedPropertyInfo = propertyInfo;
            object nestedObject=obj;
            var nestedPropertyName = propertyName;
            object nullOrDefault = default(T);

            while (!Equals(nestedObject, nullOrDefault) && nestedPropertyName.Contains("."))
            {
                var nameBeforeDot = nestedPropertyName.Substring(0, propertyName.IndexOf('.'));
                var beforeDot = GetPropertyInfo(nameBeforeDot, nestedObject.GetType());
                var typeBeforeDot = beforeDot.PropertyType;
                var nameAfterDot = nestedPropertyName.Substring(1 + nestedPropertyName.IndexOf('.'));
                //var afterDot = GetPropertyInfo(nameAfterDot, typeBeforeDot);
                //nestedPropertyInfo=afterDot;
                nestedObject = beforeDot.GetValue(nestedObject, null);
                nestedPropertyName = nameAfterDot;
                nullOrDefault = typeBeforeDot.IsValueType ? Activator.CreateInstance(typeBeforeDot) : null;
            }

            return Equals(nestedObject, nullOrDefault)
                        ? nestedPropertyInfo.PropertyType.IsValueType ? Activator.CreateInstance(nestedPropertyInfo.PropertyType) : null
                        : nestedPropertyInfo.GetValue(nestedObject, null);
        }

        private static PropertyInfo GetPropertyInfo(string propertyName, Type objectType)
        {
            if (propertyName.Contains("."))
            {
                var nameBeforeDot = propertyName.Substring(0, propertyName.IndexOf('.'));
                var beforeDot = GetPropertyInfo(nameBeforeDot, objectType);
                var typeBeforeDot = beforeDot.PropertyType;
                var nameAfterDot = propertyName.Substring(1 + propertyName.IndexOf('.'));
                var afterDot = GetPropertyInfo(nameAfterDot, typeBeforeDot);
                return afterDot;
            }
            else
            {
                var propertyInfo = objectType.GetProperty(propertyName,
                                                          BindingFlags.IgnoreCase | BindingFlags.Public |
                                                          BindingFlags.Instance);
                return propertyInfo;
                
            }
        }

        private static void EnsurePropertyOrThrow<T>(PropertyInfo propertyInfo, string propertyName)
        {
            if (propertyInfo == null)
            {
                throw new ArgumentException(
                    string.Format("Didn't find a public property \"{1}\" of type {0} which has properties ({2}).", 
                                    typeof (T), propertyName, string.Join(", ", typeof(T).GetProperties().Cast<PropertyInfo>() )),
                    "propertyName");
            }
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