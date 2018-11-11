using System;
using System.Collections.Generic;
using Xunit;
using TestBase;
using Assert = Xunit.Assert;

namespace PredicateDictionary.Specs
{
    public class AddMethodsAdd
    {
        [Fact]
        public void AKeyValuePair()
        {
            var uut=new PredicateDictionary<string, int>();
            var keyValuePair = new KeyValuePair<Func<string, bool>, int>(s => true, 9);
            
            uut.Add(keyValuePair);

            uut.Contains(keyValuePair).ShouldBeTrue();

        }
        
        [Fact]
        public void APredicateValue()
        {
            var uut=new PredicateDictionary<string, int>();

            Func<string, bool> predicate = s => s.StartsWith("1");
            uut.Add(predicate, 1);
            uut.Contains(new KeyValuePair<Func<string, bool>, int>(predicate, 1)).ShouldBeTrue();
        }
        
        [Fact]
        public void ARangeOfKeyValuePair()
        {
            var uut=new PredicateDictionary<string, int>();
            var kv1 = new KeyValuePair<Func<string, bool>, int>(s => s=="1", 1);
            var kv2 = new KeyValuePair<Func<string, bool>, int>(s => s=="2", 2);
            
            uut.AddRange( new []{kv1, kv2});

            uut.Contains(kv1).ShouldBeTrue();
            uut.Contains(kv2).ShouldBeTrue();

        }
    }
}
