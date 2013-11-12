using System.Linq;
using Dapper;
using NUnit.Framework;
using TestBase.FakeDb;
using TestBase.Shoulds;

namespace TestBase.Tests.WhenSettingUpAFakeDbConnection
{
    [TestFixture]
    public class ForQueryUsingObjectArrayFakeData
    {

        [Test]
        public void Should_return_the_setup_data__Given_an_array_of_fakedata()
        {
            //A
            var dataToReturn = new[]
                {
                    new object[] {11, "cell 1,2"}, 
                    new object[] {21, "cell 2,2"}
                };

            //A
            var fakeConnection = new FakeDbConnection().SetUpForQuery(dataToReturn,new[] {"Id", "Name"});

            //A 
            //Dapper -- the easy way to read a DbDataReader.
            fakeConnection
                .Query<IdAndName>("")
                .ShouldHaveCount(dataToReturn.Length)
                .ShouldEqualByValue(
                    new[]
                        {
                            new IdAndName
                                {
                                    Id=(int)dataToReturn[0][0],Name=(string)dataToReturn[0][1]
                                },
                            new IdAndName
                                {
                                    Id=(int)dataToReturn[1][0],Name=(string)dataToReturn[1][1]
                                },
                        }
                );
        }

        [Test] 
        public void Should_return_the_setup_data__Given_a_join___so_long_as__non_nullable_columns_have_no_nulls()
        {
            //A
            var dataToReturn = new[]
                {
                    new object[] {100, 11, null as string},
                    new object[] {200, 1,  null as string}
                };

            //A
            var fakeConnection = new FakeDbConnection()
                .SetUpForQuery(dataToReturn, 
                               new[]
                                   {
                                       new FakeDbResultSet.MetaData("Id", typeof(int)),
                                       new FakeDbResultSet.MetaData("Id", typeof(int)),
                                       new FakeDbResultSet.MetaData("Id", typeof(string)),
                                   });

            //A 
            var result = fakeConnection.Query<WithJoin, IdAndName, WithJoin>(
                            "",
                            (w, i) =>{
                                w.IdAndName = i;
                                return w;
                             },
                             splitOn:"Id"
                ).ToArray();
            result[0].Id.ShouldEqual(100);
            result[0].IdAndName.Id.ShouldEqual(11);
            result[0].IdAndName.Name.ShouldBeNull();
            result[1].Id.ShouldEqual(200);
            //
            // Not the same as the original fake data - simulated non-nullable Db columns means null related objects are populated with default values
            //
            result[1].IdAndName.Id.ShouldEqual(1);
            result[1].IdAndName.Name.ShouldBeNull();
        }

    }
}
