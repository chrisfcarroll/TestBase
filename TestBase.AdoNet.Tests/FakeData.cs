namespace TestBase.AdoNet.Tests;

static class FakeData
{
    public static AClass[] GivenFakeDataInFakeDb()
    {
            return new[]
                   {
                   new AClass {Id = 1, Name = "Name"},
                   new AClass {Id = 2, Name = "Name2"}
                   };
        }
}