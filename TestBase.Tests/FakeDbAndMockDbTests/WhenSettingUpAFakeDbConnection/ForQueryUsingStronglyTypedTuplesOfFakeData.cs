using System;
using System.Linq;
using NUnit.Framework;
using TestBase.FakeDb;
using TestBase.Shoulds;

namespace TestBase.Tests.FakeDbAndMockDbTests.WhenSettingUpAFakeDbConnection
{
    class Source1 { public int Id { get; set; } public string Name { get; set; } }

    class Source2
    {
        public int Id { get; set; }
        public int Source1Id { get; set; }
        public Source1 HydratedSource1Parent { get; set; }
    }

    [TestFixture]
    public class ForQueryUsingStronglyTypedTuplesOfFakeData
    {

        [Test]
        public void Should_return_the_setup_data__Given_an_array_of_tuples_of_fakedata()
        {
            //A
            var dataToReturn = new[]
                {
                    new Tuple<Source1,Source2>(new Source1{ Id = 11, Name = "cell 1,1"},new Source2{Id=21, Source1Id = 11}), 
                    new Tuple<Source1,Source2>(new Source1{ Id = 21, Name = "cell 2,1"},new Source2{Id=22, Source1Id = 21}), 
                };

            //A
            var fakeConnection = new FakeDbConnection().SetUpForQuery(dataToReturn);

            //A 
            //Dapper -- the easy way to read a DbDataReader.
            fakeConnection.Query<Source1, Source2, Source2>(
                "",
                (s1, s2) => { s2.HydratedSource1Parent = s1; return s2; },
                splitOn:"Id"
                ).ShouldEqualByValue(
                    dataToReturn
                        .Select(
                            t => { t.Item2.HydratedSource1Parent = t.Item1; return t.Item2; }
                            )
                );
        }
    }
}
