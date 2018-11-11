using System;
using System.Collections.Generic;
using Xunit;
using TestBase;

namespace PredicateDictionary.Specs
{
    public class ReturnsTheRightStoredValue
    {
        [Fact]
        public void GivenAKeySatisfyingAPredicate()
        {
            var uut=new PredicateDictionary<string, int>();            
            uut.Add(new KeyValuePair<Func<string, bool>, int>(s => true, 9));
            
            uut[""].ShouldBe(9);
            uut["Anything"].ShouldBe(9);
        }
        
        [Fact]
        public void GivenMultipleEntries()
        {
            var uut=new PredicateDictionary<string, int>();            
            uut.Add(new KeyValuePair<Func<string, bool>, int>(s => s.StartsWith("N"), 9));
            uut.Add(new KeyValuePair<Func<string, bool>, int>(s => s.StartsWith("E"), 8));            
        }
        
        [Fact]
        public void GivenAKeySatisfyingSeveralPredicate()
        {
            var uut=new PredicateDictionary<string, int>();            
            uut.Add(new KeyValuePair<Func<string, bool>, int>(s => s.StartsWith("N"), 9));
            uut.Add(new KeyValuePair<Func<string, bool>, int>(s => s.EndsWith("N"),  99));
            uut.Add(new KeyValuePair<Func<string, bool>, int>(s => s.StartsWith("E"), 8));
            uut.Add(new KeyValuePair<Func<string, bool>, int>(s => s.EndsWith("E"), 88));
            
            uut["N"].ShouldBeOneOf( 9, 99);
            uut["E"].ShouldBeOneOf( 8, 88);
            uut["Nine"].ShouldBe(9);
            uut["Eight"].ShouldBe(8);
            uut["eN"].ShouldBe(99);
            uut["nE"].ShouldBe(88);
        }
    }
}
