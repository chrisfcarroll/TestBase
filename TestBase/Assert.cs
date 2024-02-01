using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using FastExpressionCompiler;
#if NETSTANDARD || NET5_0_OR_GREATER
using FastExpressionCompiler;
#endif

namespace TestBase
{
    /// <summary>Static convenience methods for invoking <see cref="Assertion" />s.</summary>
    public static class Assert
    {
        /// <summary>
        /// If <paramref name="predicate" />(<paramref name="actual" />) evaluates to true,
        /// then <paramref name="actual" /> is returned.
        /// If not, an <see cref="Assertion{T}" /> is thrown, containing details of the assertion failure.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="actual"></param>
        /// <param name="predicate"></param>
        /// <param name="comments"></param>
        /// <param name="actualExpression">A compiler generated source snippet
        ///     for <paramref name="actual"/>, unless you override it.
        /// </param>
        /// <param name="assertionExpression"></param>
        /// <returns><paramref name="actual" />, if the precondition succeeds</returns>
        /// <exception cref="Assertion{T}">thrown if the precondition fails.</exception>
        public static T That<T>(T actual,
                                Expression<Func<T, bool>> predicate,
                                IEnumerable<(string, object)> comments,
                                [CallerArgumentExpression("actual")]string actualExpression=null,
                                [CallerArgumentExpression("predicate")]string assertionExpression=null)
        {
            var result = new Assertion<T>(actual, predicate, actualExpression, assertionExpression, comments);
            return result ? actual : throw result;
        }
        
        
        /// <summary>
        ///     If <paramref name="predicate" />(<paramref name="actual" />) evaluates to true, then <paramref name="actual" /> is
        ///     returned.
        ///     If not, an <see cref="Assertion{T}" /> is thrown, containing details of the assertion failure.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="actual"></param>
        /// <param name="predicate"></param>
        /// <param name="comment"></param>
        /// <param name="commentArgs"></param>
        /// <returns><paramref name="actual" />, if the precondition succeeds</returns>
        /// <exception cref="Assertion{T}">thrown if the precondition fails.</exception>
        public static T That<T>(
            T                                   actual,
            Expression<Func<T, BoolWithString>> predicate,
            string                              comment = null,
            params object[]                     commentArgs)
        {
            var result = new Assertion<T>(actual, predicate, comment, commentArgs);
            return result ? actual : throw result;
        }

        /// <summary>
        ///     If <paramref name="predicate" />(<paramref name="actual" />) evaluates to true, then <paramref name="actual" /> is
        ///     returned.
        ///     If not, an <see cref="Assertion{T}" /> is thrown, containing details of the assertion failure.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="actual"></param>
        /// <param name="predicate"></param>
        /// <param name="comment"></param>
        /// <param name="commentArgs"></param>
        /// <returns><paramref name="actual" />, if the precondition succeeds</returns>
        /// <exception cref="Assertion{T}">thrown if the precondition fails.</exception>
        public static T That<T>(
            T                         actual,
            Expression<Func<T, bool>> predicate,
            string                    comment = null,
            params object[]           commentArgs)
        {
            var result = new Assertion<T>(actual, predicate, comment, commentArgs);
            return result ? actual : throw result;
        }

        /// <summary>
        ///     If <paramref name="predicate" />(<paramref name="actual" />) evaluates to true, then <paramref name="actual" /> is
        ///     returned.
        ///     If not, an <see cref="Assertion{T}" /> is thrown, containing details of the assertion failure.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="actual"></param>
        /// <param name="comparedTo"></param>
        /// <param name="predicate"></param>
        /// <param name="comment"></param>
        /// <param name="commentArgs"></param>
        /// <returns><paramref name="actual" />, if the precondition succeeds</returns>
        /// <exception cref="Assertion{T}">thrown if the precondition fails.</exception>
        public static T That<T>(
            T                                      actual,
            T                                      comparedTo,
            Expression<Func<T, T, BoolWithString>> predicate,
            string                                 comment = null,
            params object[]                        commentArgs)
        {
            var result = new Assertion<T>(actual, comparedTo, predicate.Chain(x => x.AsBool), comment, commentArgs);
            return result ? actual : throw result;
        }

        /// <summary>
        ///     If <paramref name="predicate" />(<paramref name="actual" />) evaluates to true, then <paramref name="actual" /> is
        ///     returned.
        ///     If not, an <see cref="Precondition{T}" /> is thrown, containing details of the assertion failure.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="actual"></param>
        /// <param name="predicate"></param>
        /// <param name="comment"></param>
        /// <param name="commentArgs"></param>
        /// <returns><paramref name="actual" />, if the assertion succeeds</returns>
        /// <exception cref="Precondition{T}">thrown if the assertion fails.</exception>
        public static T Precondition<T>(
            T                         actual,
            Expression<Func<T, bool>> predicate,
            string                    comment = null,
            params object[]           commentArgs)
        {
            var result = new Precondition<T>(actual, predicate, comment, commentArgs);
            return result ? actual : throw result;
        }

        /// <summary>
        ///     Creates an <see cref="Assertion{T}" /> for the assertion <paramref name="actual" />.
        ///     If <paramref name="actual" /> evaluates to true, then the <see cref="Assertion{T}" /> is returned.
        ///     Otherwise, it is thrown.
        ///     If not, an <see cref="Assertion{T}" /> is thrown, containing details of the assertion failure.
        /// </summary>
        /// <param name="actual"></param>
        /// <param name="comment"></param>
        /// <param name="commentArgs"></param>
        /// <returns>An <see cref="Assertion{T}" /> for <paramref name="actual" /></returns>
        /// <exception cref="Assertion{T}">thrown if the precondition fails.</exception>
        public static Assertion<BoolWithString> That(
            BoolWithString  actual,
            string          comment = null,
            params object[] commentArgs)
        {
            var result = new Assertion<BoolWithString>(actual, a => a, comment, commentArgs);
            return result ? result : throw result;
        }

        /// <summary>
        ///     Creates an <see cref="Precondition{T}" /> for the assertion <paramref name="actual" />.
        ///     If <paramref name="actual" /> evaluates to true, then the <see cref="Precondition{T}" /> is returned.
        ///     Otherwise, it is thrown.
        ///     If not, an <see cref="Precondition{T}" /> is thrown, containing details of the assertion failure.
        /// </summary>
        /// <param name="actual"></param>
        /// <param name="comment"></param>
        /// <param name="commentArgs"></param>
        /// <returns>An <see cref="Precondition{T}" /> for <paramref name="actual" /></returns>
        /// <exception cref="Precondition{T}">thrown if the precondition fails.</exception>
        public static Precondition<BoolWithString> Precondition(
            BoolWithString  actual,
            string          comment = null,
            params object[] commentArgs)
        {
            var result = new Precondition<BoolWithString>(actual, a => a, comment, commentArgs);
            return result ? result : throw result;
        }

        /// <summary>
        ///     Assert that <code><paramref name="action" />.Compile()()</code> throws, catching the exception and returning it.
        /// </summary>
        /// <returns>The caught exception.</returns>
        /// <exception cref="ShouldHaveThrownException">is thrown if <paramref name="action" /> does not throw.</exception>
        public static Exception Throws(Expression<Action> action, string comment = null, params object[] commentArgs)
        {
            return Throws<Exception>(action.CompileFast(), comment, commentArgs);
        }

        /// <summary>
        ///     Asserts that <code><paramref name="action" />()</code> throws a <typeparamref name="TE" />, catching the exception
        ///     and returning it.
        ///     For an overload that accepts an <see cref="Expression{Action}" /> argument, see <see cref="Throws" />.
        /// </summary>
        /// <typeparam name="TE"></typeparam>
        /// <returns>The caught exception</returns>
        /// <exception cref="ShouldHaveThrownException">is thrown if <paramref name="action" /> does not throw.</exception>
        public static TE Throws<TE>(Action action, string comment = null, params object[] commentArgs)
        where TE : Exception
        {
            try { action(); } catch (TE ex) { return ex; } catch (Exception ex)
            {
                throw That(ex, e => e is TE, $"Expected to throw a {typeof(TE)} but threw {ex}");
            }

            throw new ShouldHaveThrownException(action.ToString());
        }

        /// <summary>
        ///     Asserts that <code><paramref name="predicate" />( <paramref name="actual" /> )</code> throws a
        ///     <typeparamref name="TE" />, catching the exception and returning it.
        /// </summary>
        /// <typeparam name="TE"></typeparam>
        /// <typeparam name="T"></typeparam>
        /// <returns>The caught exception</returns>
        /// <exception cref="ShouldHaveThrownException">is thrown if <paramref name="predicate" /> does not throw.</exception>
        public static T Throws<T, TE>(
            T                         actual,
            Expression<Func<T, bool>> predicate,
            TE                        dummyForTypeInference = null,
            string                    comment               = null,
            params object[]           commentArgs) where TE : Exception
        {
            return Throws<T, TE>(actual, predicate, comment, commentArgs);
        }

        static T Throws<T, TE>(T actual, Expression<Func<T, bool>> predicate, string comment, object[] commentArgs)
        where TE : Exception
        {
            Assertion<T> a;
            try { a = new Assertion<T>(actual, predicate, comment, commentArgs); } catch (TE) { return actual; } catch (
                Exception ex) { throw That(ex, e => e is TE, $"Expected to throw a {typeof(TE)} but threw {ex}"); }

            throw new ShouldHaveThrownException(a.Message);
        }

        /// <summary>
        ///     Executes <code><paramref name="action" />()</code>. If the execution throws, the thrown exception is wrapped in an
        ///     <see cref="ShouldNotThrowException" /> and thrown.
        /// </summary>
        /// <returns>
        ///     <paramref name="action" />
        /// </returns>
        /// <exception cref="ShouldNotThrowException">is thrown if <paramref name="action" /> throws.</exception>
        public static Expression<Action> DoesNotThrow(
            Expression<Action> action,
            string             comment = null,
            params object[]    commentArgs)
        {
            try { action.Compile(); } catch (Exception ex)
            {
                throw ShouldNotThrowException.For(action, $"Threw {ex} but expected not to throw.", commentArgs);
            }

            return action;
        }

        /// <summary>
        ///     Immediately throws a new <see cref="Assertion" /> with message formatted from <paramref name="format" /> and
        ///     <paramref name="args" />
        /// </summary>
        /// <param name="format"></param>
        /// <param name="args"></param>
        /// <exception cref="Assertion"></exception>
        public static void Fail(string format, params object[] args)
        {
            throw new Assertion(string.Format(format, args));
        }
    }


    /// <summary>
    ///     An Exception indicating that an Exception was expected but was not thrown.
    /// </summary>
    public class ShouldHaveThrownException : Exception
    {
        /// <summary>Creates a new <see cref="ShouldHaveThrownException" /> with message <paramref name="message" /></summary>
        /// <param name="message"></param>
        public ShouldHaveThrownException(string message) : base(message) { }


        /// <summary>
        ///     Creates a new <see cref="ShouldHaveThrownException" /> with message taken from <paramref name="assertion" />
        /// </summary>
        /// <param name="assertion"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static ShouldHaveThrownException For<T>(Assertion<T> assertion)
        {
            return new ShouldHaveThrownException(assertion.Message);
        }

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
        public static ShouldHaveThrownException For<T>(
            T                         actual,
            Expression<Func<T, bool>> predicate,
            string                    comment = null,
            params object[]           args)
        {
            return new ShouldHaveThrownException(new Assertion<T>(actual, predicate, comment, args).Message);
        }
    }
}
