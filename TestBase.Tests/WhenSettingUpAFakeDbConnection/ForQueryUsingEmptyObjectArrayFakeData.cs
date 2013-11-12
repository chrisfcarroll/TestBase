using System.Collections.Generic;
using System.Linq;
using Dapper;
using NUnit.Framework;
using TestBase.FakeDb;
using TestBase.Shoulds;

namespace TestBase.Tests.WhenSettingUpAFakeDbConnection
{
    [TestFixture]
    public class ForQueryUsingEmptyObjectArrayFakeData
    {

        [Test]
        public void Should_return_the_setup_data__Given_empty_array_and_a_column_name()
        {
            //A
            var uut= new FakeDbConnection().SetUpForQuery(new object[][] {}, new[] {"PdmsId"});
            //A
            uut.Query<int>("").ShouldEqualByValue(new int[] {});

        }

        [Test]
        public void Should_return_the_setup_data__Given_empty_array_and_metadata()
        {
            //A
            var metaData = new FakeDbResultSet.MetaData[2]
                {
                    new FakeDbResultSet.MetaData("",typeof(int)),
                    new FakeDbResultSet.MetaData("",typeof(string))
                };
            var uut = new FakeDbConnection().SetUpForQuery(new object[][] { }, metaData);
            //A
            uut.Query<KeyValuePair<int, string>>("").ShouldEqualByValue(new KeyValuePair<int, string>[] { });

        }
    }
}
