using System.Data;
using Moq;
using NUnit.Framework;
using TestBase.AdoNet;

namespace TestBase.TestsNet45.FakeDbAndMockDbTests
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
                           new AClass {Id = 1, Name = "Name"},
                           new AClass {Id = 2, Name = "Name2"}
                           };
            var mockCommand = new Mock<IDbCommand>();
            //A
            mockCommand
           .Setup(x => x.ExecuteReader())
           .Returns(new DataTableReader(fakeData.ToDataTable(typeof(AClass))));

            var reader = mockCommand.Object.ExecuteReader();
            var i      = 0;
            while (reader.Read())
            {
                reader.GetInt32(0).ShouldEqualByValue(fakeData[i].Id);
                reader.GetString(1).ShouldEqualByValue(fakeData[i].Name);
                i++;
            }
        }
    }
}
