using System;
using System.Collections.Generic;
using TestBase;
using Xunit;
using Assert = TestBase.Assert;

namespace PredicateDictionary.Specs
{
    public class IndexerReturnsTheRightStoredValue
    {
        [Fact]
        public void GivenAKeySatisfyingAPredicate()
        {
            var uut = new PredicateDictionary<string, int>();
            uut.Add(new KeyValuePair<Func<string, bool>, int>(s => true, 9));

            uut[""].ShouldBe(9);
            uut["Anything"].ShouldBe(9);
        }

        [Fact]
        public void GivenAKeySatisfyingSeveralPredicate()
        {
            var uut = new PredicateDictionary<string, int>();
            uut.Add(new KeyValuePair<Func<string, bool>, int>(s => s.StartsWith("N"), 9));
            uut.Add(new KeyValuePair<Func<string, bool>, int>(s => s.EndsWith("N"),   99));
            uut.Add(new KeyValuePair<Func<string, bool>, int>(s => s.StartsWith("E"), 8));
            uut.Add(new KeyValuePair<Func<string, bool>, int>(s => s.EndsWith("E"),   88));

            uut["N"].ShouldBeOneOf(9, 99);
            uut["E"].ShouldBeOneOf(8, 88);
            uut["Nine"].ShouldBe(9);
            uut["Eight"].ShouldBe(8);
            uut["eN"].ShouldBe(99);
            uut["nE"].ShouldBe(88);
        }

        [Fact]
        public void GivenMultipleEntries()
        {
            var uut = new PredicateDictionary<string, int>();
            uut.Add(s => s.StartsWith("N"), 9);
            uut.Add(s => s.StartsWith("E"), 8);

            uut["N"].ShouldBe(9);
            uut["Nine"].ShouldBe(9);
            uut["E"].ShouldBe(8);
            uut["Eight"].ShouldBe(8);
        }

        [Fact]
        public void GivenMultipleEntriesExamplesB()
        {
            var pdict = new PredicateDictionary<int, string>();
            pdict.Add(i => i == 0,           "No points");
            pdict.Add(i => i > 0 && i < 100, "bingo");
            pdict.Add(i => i < 0,            "Don't go there");

            pdict[1].ShouldBe("bingo");
            pdict[42].ShouldBe("bingo");
            pdict[0].ShouldBe("No points");
            pdict[-1234].ShouldBe("Don't go there");
            Assert.Throws<KeyNotFoundException>(() => pdict[101] = pdict[101]);
        }
    }
}
