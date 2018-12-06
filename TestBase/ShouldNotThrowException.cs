using System;
using System.Linq.Expressions;
using ExpressionToCodeLib;

namespace TestBase
{
    /// <summary>
    ///     A type of Exception thrown when <see cref="Assert.DoesNotThrow" /> finds that an Assertion <em>was</em> thrown.
    /// </summary>
    public class ShouldNotThrowException : Exception
    {
        /// <summary>Creates a new <see cref="ShouldNotThrowException" /> with message <paramref name="message" /></summary>
        /// <param name="message"></param>
        public ShouldNotThrowException(string message) : base(message) { }


        /// <summary>
        ///     Creates a new <see cref="ShouldNotThrowException" /> with message taken from <paramref name="exception" />
        /// </summary>
        /// <param name="exception"></param>
        public ShouldNotThrowException(Exception exception) : base(exception.Message) { }


        /// <summary>
        ///     Creates a new <see cref="ShouldHaveThrownException" /> with message constructed
        ///     by Asserting <paramref name="predicate" /> with comment,args=<paramref name="comment" />,<paramref name="args" />
        /// </summary>
        /// <param name="actual"></param>
        /// <param name="predicate"></param>
        /// <param name="comment"></param>
        /// <param name="args"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static ShouldNotThrowException For<T>(
            T                         actual,
            Expression<Func<T, bool>> predicate,
            string                    comment = null,
            params object[]           args)
        {
            return new ShouldNotThrowException(new Assertion<T>(actual, predicate, comment, args).Message);
        }


        /// <summary>
        ///     Creates a new <see cref="ShouldNotThrowException" /> with message constructed
        ///     by Asserting that <paramref name="action" /> doesn't throw. with comment,args=<paramref name="comment" />,
        ///     <paramref name="commentArgs" />
        /// </summary>
        /// <param name="action"></param>
        /// <param name="comment"></param>
        /// <param name="commentArgs"></param>
        /// <returns>a <c>new ShouldNotThrowException</c> instance.</returns>
        public static ShouldNotThrowException For(Expression<Action> action, string comment, object[] commentArgs)
        {
            return new ShouldNotThrowException(Assertion.New(action,
                                                             a => BoolWithString.False("Threw:"
                                                                                     + ExpressionToCode.ToCode(action)),
                                                             comment,
                                                             commentArgs));
        }
    }
}
