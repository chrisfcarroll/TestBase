using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using NUnit.Framework;

namespace TestBase.Shoulds
{
    public static class IEnumerableShoulds
    {
        public static IEnumerable<T> ShouldContain<T>(this IEnumerable<T> @this, T item, [Optional] string message, params object[] args)
        {
            Assert.That(@this, Contains.Item(item), message, args);
            return @this;
        }

        public static IEnumerable<T> ShouldContainAsSubset<T>(this IEnumerable<T> @this, IEnumerable<T> subset, [Optional] string message, params object[] args)
        {
            foreach (var item in subset)
            {
                Assert.That(@this, Contains.Item(item), message, args);
            }
            return @this;
        }

        public static IEnumerable<T> ShouldBeEmpty<T>(this IEnumerable<T> @this, [Optional] string message, params object[] args)
        {
            Assert.That(@this, Is.Empty, message, args);
            return @this;
        }

        public static IEnumerable<T> ShouldNotBeEmpty<T>(this IEnumerable<T> @this, [Optional] string message, params object[] args)
        {
            Assert.That(@this, Is.Not.Empty, message, args);
            return @this;
        }

        public static List<T> ShouldContainInOrder<T>(this List<T> @this, T expectedFirst, T expectedAfter, [Optional] string message, params object[] args)
        {
            @this
                .TakeWhile(t => !t.Equals(expectedAfter))
                .ShouldContain(expectedFirst, message, args);

            @this.ShouldContain(expectedAfter, message, args);
            return @this;
        }

        public static T[] ShouldContainInOrder<T>(this T[] @this, T expectedFirst, T expectedAfter, [Optional] string message, params object[] args)
        {
            @this
                .TakeWhile(t => !t.Equals(expectedAfter))
                .ShouldContain(expectedFirst, message, args);

            @this.ShouldContain(expectedAfter, message, args);
            return @this;
        }
    }
}