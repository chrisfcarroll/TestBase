﻿using Dapper;

namespace TestBase.AdoNet.Tests.WhenSettingUpAFakeDbConnection;

public class IdAndName
{
    public int    Id   { get; set; }
    public string Name { get; set; }
}

class WithJoin
{
    public int       Id        { get; set; }
    public IdAndName IdAndName { get; set; }
}

[TestFixture]
public class ForQueryUsingStronglyTypedFakeData
{
    [Test]
    public void Should_return_the_setup_data__Given_an_array_of_fakedata()
    {
            //A
            var dataToReturn = new[]
                               {
                               new IdAndName {Id = 11, Name = "cell 1,2"},
                               new IdAndName {Id = 21, Name = "cell 2,2"}
                               };

            //A
            var fakeConnection = new FakeDbConnection().SetUpForQuery(dataToReturn, new[] {"Id", "Name"});

            //A      //Dapper -- the easy way to read a DbDataReader.
            fakeConnection.Query<IdAndName>("").ShouldEqualByValue(dataToReturn);
        }

    [Test]
    public void Should_return_the_setup_data__Given_an_array_of_fakedata_with_joins()
    {
            //A
            var dataToReturn = new[]
                               {
                               new WithJoin {Id = 100, IdAndName = new IdAndName {Id = 11, Name = "cell 1,2"}},
                               new WithJoin {Id = 200, IdAndName = new IdAndName {Id = 21, Name = "cell 2,2"}}
                               };

            //A
            var fakeConnection =
            new FakeDbConnection().SetUpForQuery(dataToReturn, new[] {"Id", "IdAndName.Id", "IdAndName.Name"});

            //A      //Dapper -- the easy way to read a DbDataReader.
            fakeConnection.Query<WithJoin, IdAndName, WithJoin>(
                                                                "",
                                                                (w, i) =>
                                                                {
                                                                    w.IdAndName = i;
                                                                    return w;
                                                                }
                                                               )
                          .ShouldEqualByValue(dataToReturn);
        }

    [Test]
    public void Should_return_the_setup_data__Given_some_nulls_but_non_nullable_id_field()
    {
            //A
            var dataToReturn = new[]
                               {
                               new WithJoin {Id = 100, IdAndName = new IdAndName {Id = 11, Name = null}},
                               new WithJoin {Id = 200, IdAndName = null}
                               };

            //A
            var fakeConnection = new FakeDbConnection().SetUpForQuery(dataToReturn,
                                                                      new[] {"Id", "IdAndName.Id", "IdAndName.Name"});

            //A      //Dapper -- the easy way to read a DbDataReader.
            var result = fakeConnection.Query<WithJoin, IdAndName, WithJoin>("",
                                                                             (w, i) =>
                                                                             {
                                                                                 w.IdAndName = i;
                                                                                 return w;
                                                                             }
                                                                            )
                                       .ToArray();
            result[0].Id.ShouldEqual(100);
            result[0].IdAndName.Id.ShouldEqual(11);
            result[0].IdAndName.Name.ShouldBeNull();
            result[1].Id.ShouldEqual(200);
            //
            // Not the same as the original fake data - simulated non-nullable Db columns means null related objects are populated with default values
            //
            result[1].IdAndName.Id.ShouldEqual(0);
            result[1].IdAndName.Name.ShouldBeNull();
        }
}