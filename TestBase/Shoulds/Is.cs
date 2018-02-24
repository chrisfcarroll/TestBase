using System;
using System.Collections;
using System.Linq.Expressions;

namespace TestBase
{
    public static class Is
    {
        public static Expression<Func<object,bool>> Null { get; } = x => x == null;
        public static Expression<Func<object,bool>> NotNull { get; } = x => x != null;
        public static Expression<Func<IEnumerable,bool>> Empty { get; } = x => !x.HasAnyElements();
        public static Expression<Func<IEnumerable,bool>> NotEmpty { get; } = x => x.HasAnyElements();

        public static Expression<Func<object,bool>> EqualTo<TRight>(TRight expected)
        {
            return x => (x ==null && expected ==null) || x.Equals(expected);
        }

        public static Expression<Func<object,bool>> NotEqualTo<TRight>(TRight expected)
        {
            return x => (x ==null && expected !=null) || !x.Equals(expected);
        }

        public static Expression<Func<IComparable<T>,bool>> InRange<T>(T left, T right)
        {
            return x => x.CompareTo(left) >=0 && x.CompareTo(right) <= 0;
        }

        public static Expression<Func<IComparable<T>,bool>> NotInRange<T>(T left, T right)
        {
            return x => x.CompareTo(left) <0 || x.CompareTo(right) > 0;
        }

        public static Expression<Func<object,bool>> GreaterThan(object minimumExpected)
        {
            return x => new NUnitComparer().Compare(x, minimumExpected) > 0;
        }

        public static Expression<Func<object,bool>> GreaterThanOrEqualTo(object minimumExpected)
        {
            return x => new NUnitComparer().Compare(x, minimumExpected) >= 0;
        }
        
        public static Expression<Func<object,bool>> LessThan(object minimumExpected)
        {
            return x => new NUnitComparer().Compare(x, minimumExpected) < 0;
        }

        public static Expression<Func<object,bool>> LessThanOrEqualTo(object minimumExpected)
        {
            return x => new NUnitComparer().Compare(x, minimumExpected) <= 0;
        }
    }
}
