using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace TestBase.Tests.EqualByValueTests;

[TestFixture]
public class WhenComparingRecordsByValue
{
    static readonly ARecord object1 = new ARecord
    {
        Id   = 1,
        Name = "1",
        More = new BRecord {More = 1, EvenMore = "Evenmore1"}
    };

    static readonly ARecord object1again = new ARecord
    {
        Id   = 1,
        Name = "1",
        More = new BRecord {More = 1, EvenMore = "Evenmore1"}
    };

    static readonly ARecord object2 = new ARecord
    {
        Id   = 1,
        Name = "1",
        More = new BRecord {EvenMore = "Evenmore2"}
    };

    static readonly ARecord object3 = new ARecord
    {
        Id   = 1,
        Name = "1",
        More = new BRecord {More = 2, EvenMore = "Evenmore1"}
    };

    [Test]
    public void Should_return_false_when_not_the_same()
    {
            object1.EqualsByValue(object2).ShouldBeFalse("Failed to distinguish object1 from object 2");
            object1.EqualsByValue(object3).ShouldBeFalse("Failed to distinguish object1 from object 3");
    }

    [Test] public void Should_return_true_when_the_same() { object1.EqualsByValue(object1again).ShouldBeTrue(); }

    [Test]
    public void Should_return_true_for_copies()
    {
        var left = ExampleRecords.Data[0];
        var c1 = left with { FirstName = "Boo" };
        var right = c1 with { FirstName = left.FirstName };
    }
    
    [Test]
    public void Should_return_false_when_left_is_null_and_right_is_not()
    {
            (null as ARecord).EqualsByValue(object1)
                .ShouldBeFalse("Failed to distinguish left is not null from right=null");
        }
        
    [Test]
    public void Should_return_false_when_left_is_not_null_and_right_is_null()
    {
        // ReSharper disable once RedundantCast
        object1.EqualsByValue(null as ARecord)
                .ShouldBeFalse("Failed to distinguish left=null right=not null");
        }
}

public record ARecord
{
    public int    Id   { get; set; }
    public string Name { get; set; }
    public BRecord More { get; set; }
}

public record BRecord
{
    public int    More     { get; set; }
    public string EvenMore { get; set; }
}

#if !NET6_0_OR_GREATER
    record struct DateOnly(int Year, int Month, int Day);
#endif


record ComplexRecord(
    int Id,
    DateOnly DateOfBirth,
    string Title,
    string FirstName,
    string LastName,
    string DisplayName)
{
    public bool HasARecord { get; set; }
    public bool HasBRecord { get; set; }
    public Description[] ADescriptions { get; set; }
    public FlaggedDescription[] EmailAddresses { get; set; }
    public List<ARecord> Type1s { get; set; }
    public List<BRecord> Type2s { get; set; }
}

record Description(string name,string value);
record FlaggedDescription(string name,string value, bool flagged);

static class ExampleRecords
{
    public static int RowCount => Data.Length;

    public static readonly ComplexRecord[] Data =
    [
        new(Id: 1,
            DateOfBirth: new DateOnly(2001,1,1),
            Title : null,
            FirstName: "Bob",
            LastName: "Smith",
            DisplayName: "Bob Smith"){
                HasARecord = true,
                HasBRecord = true,
                ADescriptions =[
                    new Description("03"," Data 3"),
                    new Description("04"," Data 4"),
                ],
                EmailAddresses = [
                    new("C","bsmith@test.test",false),
                ],
                Type1s = new(), Type2s = new()
            },
        new(Id: 2,
            DateOfBirth: new DateOnly(2001,1,1),
            Title : null,
            FirstName: "Fred",
            LastName: "Jones",
            DisplayName: "Fred Jones"){
                HasARecord = true,
                HasBRecord = false,
                ADescriptions =[
                    new Description("01"," Data 1"),
                    new Description("02"," Data 2"),
                    ],
                EmailAddresses = [
                    new("A","FredJones@test.test",true),
                    new("B","FredJones2@othertest.test",true),
                ]
            }
    ];
}