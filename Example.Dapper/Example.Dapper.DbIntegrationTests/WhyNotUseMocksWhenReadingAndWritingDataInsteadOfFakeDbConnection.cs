using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using TestBase.FakeDb;
using TestBase.Shoulds;

namespace Example.Dapper.Tests
{
    [TestClass]
    public class WhyNotUseMocksWhenReadingAndWritingDataInsteadOfFakeDbConnection : TestBase.TestBase<Repository>
    {
        private Mock<IDbConnection> dbConnectionMock;
        private Mock<IDbCommand> dbCommandMock;
        private List<Product> fakeProducts;
        private Mock<DbDataReader> dbReaderMock;
        private FakeDbParameterCollection fakeDbParameterCollection;

        [TestMethod,Ignore]
        public void This_is_why()
        {
            //To mock data access:

            // First wire up connection to command to cataReader
            dbConnectionMock = new Mock<IDbConnection>();
            dbCommandMock = new Mock<IDbCommand>();
            dbReaderMock = new Mock<DbDataReader>();
            UnitUnderTest = new Repository(dbConnectionMock.Object);
            fakeDbParameterCollection = new FakeDbParameterCollection();
            fakeProducts = new List<Product> { new Product { Description = "Product 1", Id = 1 } };
            dbConnectionMock.Setup(x => x.CreateCommand()).Returns(dbCommandMock.Object);

            //Then mock parametercollection and command properties
            dbCommandMock.SetupProperty(x => x.CommandText);
            dbCommandMock.SetupProperty(x => x.CommandType, CommandType.Text);
            dbCommandMock.SetupProperty(x => x.CommandTimeout, 1);
            dbCommandMock.SetupProperty(x => x.Connection, dbConnectionMock.Object);
            dbCommandMock.Setup(x => x.CreateParameter()).Returns(new FakeDbParameter());
            dbCommandMock.Setup(x => x.ExecuteReader()).Returns(dbReaderMock.Object);
            dbCommandMock.Setup(x => x.ExecuteReader(It.IsAny<CommandBehavior>())).Returns(dbReaderMock.Object);
            dbCommandMock.SetupGet(x => x.Parameters).Returns(fakeDbParameterCollection);

            //And then mock dbReader to return the correct metadata
            dbReaderMock.SetupGet(x=>x.FieldCount).Returns(2 /*insert column width of your fake data here*/);
            dbReaderMock.Setup(x => x.GetFieldType(0)).Returns(typeof (int));
            dbReaderMock.Setup(x => x.GetFieldType(1)).Returns(typeof(string));
            dbReaderMock.Setup(x => x.GetName(0)).Returns("Id");
            dbReaderMock.Setup(x => x.GetName(1)).Returns("Description");

            //And then setup every row and column of fake data
            //Row 1 ... the rest not done
            dbReaderMock.Setup(x => x.GetInt32(0)).Returns(1 /*but wire in the rest of the rows of fake data here*/);
            dbReaderMock.Setup(x => x.GetString(1)).Returns("Product 1" /*but wire in the rest of the rows of fake data here*/);
            dbReaderMock.Setup(x => x.GetValue(0)).Returns(1 /*but wire in the rest of the rows of fake data here*/);
            dbReaderMock.Setup(x => x.GetValue(1)).Returns("Product 1" /*but wire in the rest of the rows of fake data here*/);

            //And possibly finally setup the Read method for how many rows of data you have
            int count = 0;
            int rows = 1;
            dbReaderMock.Setup(x => x.Read()).Returns(()=>{
                                                            count += 1;
                                                            return count <= rows;
                                                        });

            //A&A

            //But sadly still doesn't quite work...
            UnitUnderTest.GetSomeData().ShouldEqualByValue(fakeProducts);
        }
    }
}