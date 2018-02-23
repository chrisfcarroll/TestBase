using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Mono.Linq.Expressions;

namespace TestBase
{
    public static class IEnumerableShoulds
    {
        public static IEnumerable<T> ShouldContain<T>(this IEnumerable<T> actual, Expression<Func<T,bool>> predicate,string comment = null, params object[] args)
        {
            return Assert.That(actual, a => a.Any(predicate.Compile()), comment ?? $"Should contain {predicate.ToCSharpCode()}", args);
        }
        public static IEnumerable<T> ShouldContain<T>(this IEnumerable<T> actual, T expectedItem, string comment = null, params object[] args)
        {
            return Assert.That(actual, a => a.Any(i=>i.Equals(expectedItem) ), comment ?? $"Should contain {expectedItem}", args);
        }

        public static IEnumerable<T> ShouldNotBeEmpty<T>(this IEnumerable<T> actual, string comment = null, params object[] args)
        {
            return Assert.That(actual, a => a.Any(), comment??"Should not be empty but was.", args);
        }

        public static IEnumerable<T> ShouldBeEmpty<T>(this IEnumerable<T> actual, string comment = null, params object[] args)
        {
            return Assert.That(actual, a => !a.Any(), comment, args);
        }

        public static IEnumerable<T> ShouldBeOfLength<T>(this IEnumerable<T> actual, int expected, string comment=null, params object[] args)
        {
            return Assert.That(actual, a => a.Count() == expected, comment, args);
        }
        public static IEnumerable<T> ShouldHaveCount<T>(this IEnumerable<T> actual, int expected, string comment = null, params object[] args)
        {
            return Assert.That(actual, a => a.Count() == expected, comment, args);
        }

        public static T SingleOrAssertFail<T>(this IEnumerable<T> @this, string message = null, params object[] args)
        {
            try
            {
                return @this.Single();
            }
            catch
            {
                throw new Assertion<IEnumerable<T>>(@this, s=>s.Count()==1,message,args);
            }
        }
        public static T SingleOrAssertFail<T>(this IEnumerable<T> @this, Func<T, bool> predicate, string message = null, params object[] args)
        {
            try
            {
                return @this.Single(predicate);
            }
            catch
            {
                throw new Assertion<IEnumerable<T>>(@this, s => s.Count(predicate) == 1, message?? "SingleOrAssertFail expected exactly 1", args);
            }
        }
    }

    public static class IEnumerablePredicates
    {
        public static bool IsInList<T>(this T item, IEnumerable<T> list)
        {
            return list.Contains(item);
        }

        public static bool DoesNotContain<T>(this IEnumerable<T> list, T item)
        {
            return !list.Contains(item);
        }
    }
}