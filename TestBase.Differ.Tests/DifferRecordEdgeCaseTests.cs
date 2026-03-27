using NUnit.Framework;

namespace TestBase.DifferTests;

[TestFixture]
public class DifferRecordEdgeCaseTests
{
    record ARecord
    {
        public int    Id   { get; set; }
        public string Name { get; set; }
        public BRecord More { get; set; }
    }

    record BRecord
    {
        public int    More     { get; set; }
        public string EvenMore { get; set; }
    }

    record Description(string Name, string Value);
    record FlaggedDescription(string Name, string Value, bool Flagged);

    record ComplexRecord(int Id, DateOnly DateOfBirth, string FirstName, string LastName)
    {
        public bool HasARecord { get; set; }
        public Description[] Descriptions { get; set; }
        public FlaggedDescription[] EmailAddresses { get; set; }
        public List<ARecord> Records { get; set; }
    }

    static readonly ARecord object1 = new()
    {
        Id   = 1,
        Name = "1",
        More = new BRecord { More = 1, EvenMore = "Evenmore1" }
    };

    static readonly ARecord object1again = new()
    {
        Id   = 1,
        Name = "1",
        More = new BRecord { More = 1, EvenMore = "Evenmore1" }
    };

    static readonly ARecord object2 = new()
    {
        Id   = 1,
        Name = "1",
        More = new BRecord { EvenMore = "Evenmore2" }
    };

    [Test]
    public void Equal_records() =>
        Assert.That(Differ.Diff(object1, object1again).AreEqual, Is.True);

    [Test]
    public void Different_records_nested()
    {
        var result = Differ.Diff(object1, object2);
        //D
        TestContext.Progress.WriteLine(result.ToString());
        //A
        Assert.That(result.AreEqual, Is.False);
    }

    [Test]
    public void Record_copy_with_same_values_is_equal()
    {
        var left = object1;
        var modified = left with { Name = "Changed" };
        var right = modified with { Name = left.Name };
        Assert.That(Differ.Diff(left, right).AreEqual, Is.True);
    }

    [Test]
    public void Left_null_right_record()
    {
        var result = Differ.Diff(null, object1);
        //D
        TestContext.Progress.WriteLine(result.ToString());
        //A
        Assert.That(result.AreEqual, Is.False);
    }

    [Test]
    public void Record_right_null()
    {
        var result = Differ.Diff(object1, (ARecord)null);
        //D
        TestContext.Progress.WriteLine(result.ToString());
        //A
        Assert.That(result.AreEqual, Is.False);
    }

    [Test]
    public void Complex_records_equal()
    {
        var left = new ComplexRecord(1, new DateOnly(2001, 1, 1), "Bob", "Smith")
        {
            HasARecord = true,
            Descriptions = [new Description("A", "B")],
            EmailAddresses = [new FlaggedDescription("C", "d@test.test", false)],
            Records = new()
        };
        var right = new ComplexRecord(1, new DateOnly(2001, 1, 1), "Bob", "Smith")
        {
            HasARecord = true,
            Descriptions = [new Description("A", "B")],
            EmailAddresses = [new FlaggedDescription("C", "d@test.test", false)],
            Records = new()
        };
        Assert.That(Differ.Diff(left, right).AreEqual, Is.True);
    }

    [Test]
    public void Complex_records_differ_on_nested_collection()
    {
        var left = new ComplexRecord(1, new DateOnly(2001, 1, 1), "Bob", "Smith")
        {
            HasARecord = true,
            Descriptions = [new Description("A", "B")],
            EmailAddresses = [new FlaggedDescription("C", "d@test.test", false)],
            Records = new()
        };
        var right = new ComplexRecord(1, new DateOnly(2001, 1, 1), "Bob", "Smith")
        {
            HasARecord = true,
            Descriptions = [new Description("A", "DIFFERENT")],
            EmailAddresses = [new FlaggedDescription("C", "d@test.test", false)],
            Records = new()
        };
        var result = Differ.Diff(left, right);
        //D
        TestContext.Progress.WriteLine(result.ToString());
        //A
        Assert.That(result.AreEqual, Is.False);
    }
}
