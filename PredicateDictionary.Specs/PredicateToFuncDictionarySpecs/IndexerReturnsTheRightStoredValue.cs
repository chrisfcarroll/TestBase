using System;
using System.Collections.Generic;
using TestBase;
using Xunit;

namespace PredicateDictionary.Specs.PredicateToFuncDictionarySpecs
{
    public class IndexerReturnsTheRightStoredValue
    {
        [Fact]
        public void GivenAKeySatisfyingAPredicate()
        {
            var uut=new PredicateToFuncDictionary<string, int>();            
            uut.Add( new KeyValuePair<Func<string, bool>, Func<string,int>>(s => true, s=>9));
            
            uut[""].ShouldBe(9);
            uut["Anything"].ShouldBe(9);
        }
        
        [Fact]
        public void GivenMultipleEntries()
        {
            var uut=new PredicateToFuncDictionary<string, int>();            
            uut.Add(new KeyValuePair<Func<string, bool>, Func<string,int>>(s => s.StartsWith("N"), s=>9));
            uut.Add(new KeyValuePair<Func<string, bool>, Func<string,int>>(s => s.StartsWith("E"), s=>8));
            
            uut["N"].ShouldBe(9);
            uut["Nine"].ShouldBe(9);
            uut["E"].ShouldBe(8);
            uut["Eight"].ShouldBe(8);
        }
        
        [Fact]
        public void GivenAKeySatisfyingSeveralPredicate()
        {
            var uut=new PredicateToFuncDictionary<string, int>();            
            uut.Add(new KeyValuePair<Func<string, bool>, Func<string,int>>(s => s.StartsWith("N"), s=>9));
            uut.Add(new KeyValuePair<Func<string, bool>, Func<string,int>>(s => s.EndsWith("N"),  s=>99));
            uut.Add(new KeyValuePair<Func<string, bool>, Func<string,int>>(s => s.StartsWith("E"), s=>8));
            uut.Add(new KeyValuePair<Func<string, bool>, Func<string,int>>(s => s.EndsWith("E"), s=>88));
            
            uut["N"].ShouldBeOneOf( 9, 99);
            uut["E"].ShouldBeOneOf( 8, 88);
            uut["Nine"].ShouldBe(9);
            uut["Eight"].ShouldBe(8);
            uut["eN"].ShouldBe(99);
            uut["nE"].ShouldBe(88);
        }
    }
}
