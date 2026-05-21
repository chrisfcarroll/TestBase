using System;
using System.Linq.Expressions;

namespace TestBase;

/// <inheritdoc />
/// <summary>
///     A Precondition is an Assertion. Calling it a precondition is presumed to indicate that it is to be understood
///     as a precondition
/// </summary>
/// <typeparam name="T"></typeparam>
public class Precondition<T> : Assertion<T>
{
    /// <inheritdoc />
    /// <summary>Create a <c>Precondition</c> which asserts <paramref name="predicate" /></summary>
    /// <param name="actual"></param>
    /// <param name="predicate"></param>
    /// <param name="comment"></param>
    /// <param name="commentArgs"></param>
    public Precondition(
        T                         actual,
        Expression<Func<T, bool>> predicate,
        string                    comment = null,
        params object[]           commentArgs) : base(actual, predicate, comment, commentArgs) { }
}