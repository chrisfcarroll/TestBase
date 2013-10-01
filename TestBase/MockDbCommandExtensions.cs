using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using Moq;
using TestBase.FakeDb;

namespace TestBase
{
    public static class MockDbCommandExtensions
    {
        public static Mock<DbCommand> SetupExecuteReaderToReturnDataSetWithFieldNamesForType(this Mock<DbCommand> mock, Type pocoTypeToReturn, IEnumerable<object> resultToReturn)
        {
            mock.Setup(x => x.ExecuteReader()).Returns(new DataTableReader(DbCommandExtensions.ToDataTable(resultToReturn, pocoTypeToReturn)));
            return mock;
        }
    }
}