using System.Data;
using Moq;
using NUnit.Framework;
using TestBase.AdoNet;

namespace TestBase.Tests.FakeDbAndMockDbTests
{
    [TestFixture]
    public class WhenMockingADbCommandExecuteReader
    {
        [Test]
        public void Should_return_all_valuetype_and_string_properties_of_the_given_fakedata()
        {
            //A
            var fakeData = new[]
                {
                        new AClass{Id = 1,Name = "Name"},
                        new AClass{Id = 2,Name = "Name2"},
                };
            var mockCommand = new Mock<IDbCommand>();
            //A
            mockCommand
                    .Setup(x => x.ExecuteReader())
                    .Returns(new DataTableReader(fakeData.ToDataTable(typeof(AClass))));

            var reader = mockCommand.Object.ExecuteReader();
            int i = 0;
            while (reader.Read())
            {
                EqualsByValueShoulds.ShouldEqualByValue(reader.GetInt32(0), fakeData[i].Id);
                EqualsByValueShoulds.ShouldEqualByValue(reader.GetString(1), fakeData[i].Name);
                i++;
            }
        }
    }
}
