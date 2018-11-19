namespace TestBase.Tests
{
    public class AClass
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public BClass More { get; set; }
    }

    public class BClass
    {
        public int More { get; set; }
        public string EvenMore { get; set; }
    }
}