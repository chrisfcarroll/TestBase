using System;
using System.Collections.Generic;
using TestBase;
using Xunit;

namespace PredicateDictionary.Specs.PredicateToFuncDictionarySpecs
{
    public class TryGetValueReturnsTheRightStoredValue
    {
        [Fact]
        public void GivenAKeySatisfyingAPredicate()
        {
            var uut=new PredicateToFuncDictionary<string, int>();            
            uut.Add(new KeyValuePair<Func<string, bool>, Func<string, int>>(s => true, s=>9));
            
            uut.TryGetValue("", out int value).ShouldBeTrue();
            value.ShouldBe(9);
        }
        
        [Fact]
        public void GivenNoPredicates()
        {
            var uut=new PredicateToFuncDictionary<string, int>();            
            
            uut.TryGetValue("", out int value).ShouldBeFalse();
            value.ShouldBe(0);
            
        }
        [Fact]
        public void GivenAKeySatisfyingNoPredicates()
        {
            var uut=new PredicateToFuncDictionary<string, int>();
            uut.Add(s => s.StartsWith("1"), s=>1);
            
            uut.TryGetValue("", out int value).ShouldBeFalse();
            value.ShouldBe(0);
            
        }
        
        [Fact]
        public void GivenMultipleEntries()
        {
            var uut=new PredicateToFuncDictionary<string, int>();            
            uut.Add(new KeyValuePair<Func<string, bool>, Func<string, int>>(s => s.StartsWith("N"), s=>9));
            uut.Add(new KeyValuePair<Func<string, bool>, Func<string, int>>(s => s.StartsWith("E"), s=>8));

            uut.TryGetValue("N", out int value1).ShouldBeTrue();
            value1.ShouldBe(9);
            
            uut.TryGetValue(  "Nine" , out int value2 ).ShouldBeTrue();
            value2.ShouldBe(9);
            
            uut.TryGetValue(  "E" , out int value3 ).ShouldBeTrue();
            value3.ShouldBe(8);
            
            uut.TryGetValue(  "Eight" , out int value4 ).ShouldBeTrue();
            value4.ShouldBe(8);
            
        }
        
        [Fact]
        public void GivenAKeySatisfyingSeveralPredicate()
        {
            var uut=new PredicateToFuncDictionary<string, int>();            
            uut.Add(new KeyValuePair<Func<string, bool>, Func<string, int>>(s => s.StartsWith("N"), s=>9));
            uut.Add(new KeyValuePair<Func<string, bool>, Func<string, int>>(s => s.EndsWith("N"),  s=>99));
            uut.Add(new KeyValuePair<Func<string, bool>, Func<string, int>>(s => s.StartsWith("E"), s=>8));
            uut.Add(new KeyValuePair<Func<string, bool>, Func<string, int>>(s => s.EndsWith("E"), s=>88));
            
            uut["N"].ShouldBeOneOf( 9, 99);
            uut["E"].ShouldBeOneOf( 8, 88);
            uut["Nine"].ShouldBe(9);
            uut["Eight"].ShouldBe(8);
            uut["eN"].ShouldBe(99);
            uut["nE"].ShouldBe(88);
        }
    }
}
