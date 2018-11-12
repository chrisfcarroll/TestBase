using System;
using System.Collections.Generic;
using TestBase;
using Xunit;

namespace PredicateDictionary.Specs.PredicateToFuncDictionarySpecs
{
    public class AddMethodsAdd
    {
        [Fact]
        public void AKeyValuePair()
        {
            var uut=new PredicateToFuncDictionary<string, int>();
            var keyValuePair = new KeyValuePair<Func<string, bool>, Func<string,int>>(s => true, s=>9);
            
            uut.Add(keyValuePair);

            uut.Contains(keyValuePair).ShouldBeTrue();

        }
        
        [Fact]
        public void APredicateValue()
        {
            var uut=new PredicateToFuncDictionary<string, int>();

            Func<string, bool> predicate = s => s.StartsWith("1");
            Func<string, int> func = s => 1;
            uut.Add(predicate, s=>1);
            uut.Contains(new KeyValuePair<Func<string, bool>, Func<string,int>>(predicate, func)).ShouldBeTrue();
        }
        
        [Fact]
        public void ARangeOfKeyValuePair()
        {
            var uut=new PredicateToFuncDictionary<string, int>();
            var kv1 = new KeyValuePair<Func<string, bool>, Func<string,int>>(s => s=="1", int.Parse);
            var kv2 = new KeyValuePair<Func<string, bool>, Func<string,int>>(s => s=="2", int.Parse);
            
            uut.AddRange( new []{kv1, kv2});

            uut.Contains(kv1).ShouldBeTrue();
            uut.Contains(kv2).ShouldBeTrue();

        }
    }
}
