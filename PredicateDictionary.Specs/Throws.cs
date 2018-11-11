using System;
using System.Collections.Generic;
using Xunit;
using TestBase;
using Assert = Xunit.Assert;

namespace PredicateDictionary.Specs
{
    public class Throws
    {
        [Fact]
        public void GivenAKeyNotSatisfyingAnyPredicates()
        {
            var uut=new Dictionary<string, int>();
            Assert.Throws<KeyNotFoundException>(() => uut[""]);
        }
        
    }
}
