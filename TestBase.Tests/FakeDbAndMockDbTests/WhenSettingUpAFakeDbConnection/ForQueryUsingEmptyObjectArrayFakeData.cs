using System;
using System.Collections.Generic;
using Dapper;
using NUnit.Framework;
using TestBase.AdoNet.FakeDb;

namespace TestBase.Tests.FakeDbAndMockDbTests.WhenSettingUpAFakeDbConnection
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

        [Test]
        public void Should_throw_helpfully__Given__not_enough_metadata()
        {
            //A
            Assert.Throws<InvalidOperationException>(
                () => new FakeDbConnection().SetUpForQuery(new object[][] {})
                ).Message.ShouldMatch("[Cc]an't .* metadata");

        }
    }
}
