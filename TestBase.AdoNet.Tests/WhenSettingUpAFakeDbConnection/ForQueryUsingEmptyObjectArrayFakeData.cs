﻿using Dapper;

namespace TestBase.AdoNet.Tests.WhenSettingUpAFakeDbConnection;

[TestFixture]
public class ForQueryUsingEmptyObjectArrayFakeData
{
    [Test]
    public void Should_return_the_setup_data__Given_empty_array_and_a_column_name()
    {
            //A
            var uut = new FakeDbConnection().SetUpForQuery(new object[][] { }, "PdmsId");
            //A
            uut.Query<int>("").ShouldEqualByValue(new int[] { });
        }

    [Test]
    public async Task Should_return_the_setup_data__Given_empty_array_and_a_column_name__GivenQueryAsync()
    {
            //A
            var uut = new FakeDbConnection().SetUpForQuery(new object[][] { }, "PdmsId");
            //A
            (await uut.QueryAsync<int>("")).ShouldEqualByValue(new int[] { });
        }

    [Test]
    public void Should_return_the_setup_data__Given_empty_array_and_metadata()
    {
            //A
            var metaData = new FakeDbResultSet.MetaData[2]
                           {
                           new FakeDbResultSet.MetaData("", typeof(int)),
                           new FakeDbResultSet.MetaData("", typeof(string))
                           };
            var uut = new FakeDbConnection().SetUpForQuery(new object[][] { }, metaData);
            //A
            uut.Query<KeyValuePair<int, string>>("").ShouldEqualByValue(new KeyValuePair<int, string>[] { });
        }

    [Test]
    public async Task Should_return_the_setup_data__Given_empty_array_and_metadata__GivenQueryAsync()
    {
            //A
            var metaData = new FakeDbResultSet.MetaData[2]
                           {
                           new FakeDbResultSet.MetaData("", typeof(int)),
                           new FakeDbResultSet.MetaData("", typeof(string))
                           };
            var uut = new FakeDbConnection().SetUpForQuery(new object[][] { }, metaData);
            //A

            (await uut.QueryAsync<KeyValuePair<int, string>>(""))
           .ShouldEqualByValue(new KeyValuePair<int, string>[] { });
        }

    [Test]
    public void Should_throw_helpfully__Given__not_enough_metadata()
    {
            //A
            Assert.Throws<InvalidOperationException>(
                                                     () => new FakeDbConnection().SetUpForQuery(new object[][] { })
                                                    )
                  .Message.ShouldMatch("[Cc]an't .* metadata");
        }
}