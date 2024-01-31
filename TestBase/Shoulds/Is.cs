using System;
using System.Collections;
using System.Linq.Expressions;
using ExpressionToCodeLib;

namespace TestBase
{
    /// <summary>
    ///     A set of <see cref="Expression{TDelegate}" /> Predicates with a simple fluent syntax
    ///     Intended to provide self-explanatory code when expanded by <see cref="ExpressionToCode" />
    /// </summary>
    public static class Is
    {
        public static Expression<Func<object, bool>>      Null     { get; } = x => x == null;
        public static Expression<Func<object, bool>>      NotNull  { get; } = x => x != null;
        public static Expression<Func<IEnumerable, bool>> Empty    { get; } = x => !x.HasAnyElements();
        public static Expression<Func<IEnumerable, bool>> NotEmpty { get; } = x => x.HasAnyElements();
        
        public static Expression<Func<string, bool>>      NotNullOrEmpty  { get; } = x => string.IsNullOrEmpty(x);
        public static Expression<Func<object, bool>> NullOrEmptyOrWhitespace { get; } 
            = (object o) => o == null || (o is string) && string.IsNullOrWhiteSpace((string)o);
        
        public static Expression<Func<string, bool>> NotNullOrEmptyOrWhitespace { get; } 
            = s => !string.IsNullOrWhiteSpace(s);


        public static Expression<Func<object, bool>> EqualTo<TRight>(TRight expected)
        {
            if (expected == null)
                return x => x == null;
            else
                return x => x != null && x.Equals(expected);
        }

        public static Expression<Func<object, bool>> NotEqualTo<TRight>(TRight expected)
        {
            if (expected == null)
                return x => x != null;
            else
                return x => x == null || !x.Equals(expected);
        }

        public static Expression<Func<IComparable<T>, bool>> InRange<T>(T left, T right)
        {
            return x => x.CompareTo(left) >= 0 && x.CompareTo(right) <= 0;
        }

        public static Expression<Func<IComparable<T>, bool>> NotInRange<T>(T left, T right)
        {
            return x => x.CompareTo(left) < 0 || x.CompareTo(right) > 0;
        }
        public static Expression<Func<object, bool>> GreaterThan(object minimumExpected)
        {
            return x => new NUnitComparer().Compare(x, minimumExpected) > 0;
        }

        public static Expression<Func<object, bool>> GreaterThanOrEqualTo(object minimumExpected)
        {
            return x => new NUnitComparer().Compare(x, minimumExpected) >= 0;
        }

        public static Expression<Func<object, bool>> LessThan(object minimumExpected)
        {
            return x => new NUnitComparer().Compare(x, minimumExpected) < 0;
        }

        public static Expression<Func<object, bool>> LessThanOrEqualTo(object minimumExpected)
        {
            return x => new NUnitComparer().Compare(x, minimumExpected) <= 0;
        }
    }
}
