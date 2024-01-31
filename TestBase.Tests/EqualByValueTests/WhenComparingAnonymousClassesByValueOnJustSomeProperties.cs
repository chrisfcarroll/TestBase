using System.Collections.Generic;
using NUnit.Framework;

namespace TestBase.Tests.EqualByValueTests;

[TestFixture]
public class WhenComparingAnonymousClassesByValueOnJustSomeMembers
{
    [Test]
    public void Should_not_be_fooled_by_members_with_overlapping_names()
    {
            //A
            var objectL        = new {Id = 1, Name = "1", NameOhDear = "Did we compare on field NameOhDear?"};
            var objectR        = new {Id = 1, Name = "1", NameOhDear = "Oh Dear!"};
            var matchedMembers = new List<string> {"Name"};

            //A & A
            objectL.EqualsByValuesJustOnMembersNamed(objectR, matchedMembers).ShouldBeTrue();
            objectL.ShouldEqualByValueOnMembers(objectR, matchedMembers);
        }

    [Test]
    public void Should_return_false_when_not_the_same()
    {
            var objectL           = new {Id = 1, Name = "1", Nested = new {NestedName = "N1", NestedMore = "M1"}};
            var objectR           = new {Id = 1, Name = "1", Nested = new {NestedName = "N2", NestedMore = "M2"}};
            var mismatchedMembers = new List<string> {"Nested", "Nested.NestedName"};

            //A&A
            objectL.EqualsByValuesJustOnMembersNamed(objectR, mismatchedMembers).ShouldBeFalse();
            Assert.Throws<Assertion>(
                                     () => objectL.ShouldEqualByValueOnMembers(objectR, mismatchedMembers)
                                    );
        }

    [Test]
    public void Should_return_true_when_the_same()
    {
            //A
            var objectL = new
 {
                                     Id     = 1, Name = "1",
                                     Nested = new {NestedName = "N1", NestedMember2 = "NestedMember"}
                                     };
            var objectR = new
 {
                                     Id     = 1, Name = "1",
                                     Nested = new {NestedName = "N2", NestedMember2 = "NestedMember"}
                                     };
            var matchedMembers = new List<string> {"IrrelevantMemberName", "Nested.NestedMember"};

            //A & A
            objectL.EqualsByValuesJustOnMembersNamed(objectR, matchedMembers).ShouldBeTrue();
            objectL.ShouldEqualByValueOnMembers(objectR, matchedMembers);
        }
}