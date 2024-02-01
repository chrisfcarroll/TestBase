using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.RegularExpressions;
using ExpressionToCodeLib;
using Newtonsoft.Json;
using FastExpressionCompiler;
#if NET5_0_OR_GREATER
using TooString;
using System.Runtime.CompilerServices;
#endif

namespace TestBase
{

    /// <summary>
    ///     An Assertion about an instance.
    ///     An Assertion is throwable (it inherits from Exception) but need not indicate an assertion failure; it might hold an
    ///     assertion pass.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Assertion<T> : Assertion
    {
        #pragma warning disable 1591
        public enum StringifyMethod
        {
            DeclaredToStringElseThrow = 0,
            ExpressionToCodeStringify = 1,
            NewtonsoftJsonSerialize = 2,
            InheritedToString = 3,
            TooString = 4
        }
        #pragma warning restore 1591
        // ReSharper disable once StaticMemberInGenericType
        static readonly string nl = Environment.NewLine;

        /// <summary>
        ///     Called by <seealso cref="ActualToString" />. Modify this if you want to change the order of methods attempted
        ///     to Stringify actual.
        /// </summary>
        public static StringifyMethod[] PreferredToStringMethod =
        {
            StringifyMethod.TooString,
            StringifyMethod.DeclaredToStringElseThrow,
            StringifyMethod.ExpressionToCodeStringify,
            StringifyMethod.NewtonsoftJsonSerialize,
            StringifyMethod.InheritedToString
        };

        static readonly Dictionary<StringifyMethod, Func<object, string>>
            StringifyMethods =
                new Dictionary<StringifyMethod, Func<object, string>>
                {
                    {
                        StringifyMethod.DeclaredToStringElseThrow,
                        o => o == null ? "<null>" :
                            o.GetType().GetMethod("ToString",
                                BindingFlags.Instance | BindingFlags.DeclaredOnly |
                                BindingFlags.Public)
                            != null ? o.ToString() :
                            throw new ArgumentNullException("actual")
                    },
                    {
                        StringifyMethod.ExpressionToCodeStringify,
                        ObjectStringify.Default.PlainObjectToCode
                    },
                    {
                        StringifyMethod.NewtonsoftJsonSerialize,
                        o => JsonConvert.SerializeObject(o,
                            BestEffortJsonSerializerSettings.Serializer)
                    },
                    { StringifyMethod.InheritedToString, o => o.ToString() },
#if NET5_0_OR_GREATER                    
                    { StringifyMethod.TooString, o => o.ToDebugViewString() },
#else
                    { StringifyMethod.TooString, o => o.TooString() },
#endif
                };

        /// <summary>
        ///     Evaluates whether <paramref name="predicate" /> is true of <paramref name="actual" />, and stores the result of the
        ///     evaluation or, if
        ///     an exception is thrown during evaluation, catches and stores the exception instead.
        /// </summary>
        /// <param name="actual">The value under assertion</param>
        /// <param name="predicate">The predicate to apply</param>
        /// <param name="actualExpression">
        ///     Compiler-provided code string for <paramref name="actual"/>, unless you
        ///     override it.
        /// </param>
        /// <param name="asserted"></param>
        /// <param name="comments">
        ///     Occurrences of "{{actual}}" in any comment strings will be replace with <paramref name="actual" />
        ///     ?.ToString()
        /// </param>
        public Assertion(T actual,
                         Expression<Func<T, bool>> predicate,
                         string actualExpression,
                         string asserted, 
                         IEnumerable<(string, object)> comments = default)
        {
            try
            {
                Actual = ActualToString(actual);
                ActualExpression = actualExpression;
                Comment = string.Join(nl,
                    (comments ?? new List<(string, object)>())
                    .Select(c => $"{c.Item1} : {c.Item2.TooString()}"));
                Asserted = asserted;
                AssertedDetail = 
                    ExpressionToCodeConfiguration.DefaultAssertionConfiguration
                    .WithPrintedListLengthLimit(15)
                    .WithMaximumValueLength(80)
                    .WithOmitImplicitCasts(true)
                    .GetAnnotatedToCode()
                    .AnnotatedToCode(predicate);
                var result = predicate.CompileFast()(actual);
                Result = result;
            }
            catch (Exception e)
            {
                ExceptionThrownByEvaluation = e;
            }
        }

        /// <summary>Fills the Properties with the given values.
        /// This constructor is only useful if you have already run the
        /// assertion, and obtained expression values.
        /// </summary>
        /// <param name="actual"><see cref="Actual"/></param>
        /// <param name="actualExpression"><see cref="ActualExpression/></param>
        /// <param name="asserted"><see cref="Asserted"/></param>
        /// <param name="assertedDetail"><see cref="AssertedDetail"/></param>
        /// <param name="comment"><see cref="Comment"/></param>
        /// <param name="result"><see cref="Result"/></param>
        public Assertion(string actual,
                         string actualExpression,
                         string asserted,
                         string assertedDetail,
                         string comment,
                         bool? result)
        {
            Actual = actual;
            ActualExpression = actualExpression;
            Asserted = asserted;
            AssertedDetail = assertedDetail;
            Comment = comment;
            Result = result;
        }

        /// <summary>
        ///     Evaluates whether <paramref name="predicate" /> is true of <paramref name="actual" />, and stores the result of the
        ///     evaluation or, if
        ///     an exception is thrown during evaluation, catches and stores the exception instead.
        /// </summary>
        /// <param name="actual">The value under assertion</param>
        /// <param name="assertions">
        /// A delegate expected to run one or more assertions. If it doesn't throw,
        /// the assertion passes.
        /// </param>
        /// <param name="asserted">A description of what <paramref name="assertions"/> asserts.</param>
        /// <param name="argumentExpression"></param>
        /// <param name="assertionsExpression">
        /// Compiler-provided code string for <paramref name="assertions"/>, unless you
        /// override it.
        /// </param>
        /// <param name="comments">
        ///     Occurrences of "{{actual}}" in any comment strings will be replace with <paramref name="actual" />
        ///     ?.ToString()
        /// </param>
        public Assertion(T actual,
                         Action<T> assertions,
                         string asserted,
                         [CallerArgumentExpression("actual")] string argumentExpression = null,
                         [CallerArgumentExpression("assertions")] string assertionsExpression = null,
                         IEnumerable<(string, object)> comments = default)
        {
            try
            {
                Actual = ActualToString(actual);
                ActualExpression = argumentExpression;
                Asserted = asserted;
                AssertedDetail = assertionsExpression; 
                Comment = string.Join(nl,
                    (comments ?? new List<(string, object)>())
                    .Select(c => $"{c.Item1} : {c.Item2.TooString()}"));
                assertions(actual);
                Result = true;
            }
            catch (Exception e)
            {
                if (e is Assertion)
                {
                    Result = false;
                }
                else
                {
                    ExceptionThrownByEvaluation = e;
                }
            }
        }


        /// <summary>
        ///     Evaluates whether <paramref name="predicate" /> is true of <paramref name="actual" />, and stores the result of the
        ///     evaluation or, if
        ///     an exception is thrown during evaluation, catches and stores the exception instead.
        /// </summary>
        /// <param name="actual">The value under assertion</param>
        /// <param name="comparedTo">
        ///     The expected value, or the 'comparable' value to be used in <paramref name="predicate" />
        ///     (actual,expected).
        /// </param>
        /// <param name="predicate">The predicate to apply</param>
        /// <param name="comment">
        ///     A format string to provide information about the assertion.
        ///     <em>Occurrences of "{{actual}}" in the comment string will be replace with <paramref name="actual" />?.ToString()</em>
        /// </param>
        /// <param name="commentArgs">will be inserted into <paramref name="comment" /> using string.Format()</param>
        public Assertion(
            T                            actual,
            T                            comparedTo,
            Expression<Func<T, T, bool>> predicate,
            string                       comment,
            object[]                     commentArgs)
        {
            try
            {
                Actual  = ActualToString(actual);
                Comment = CommentFormattedV1(Actual, comment, commentArgs);
                var assertActual = Expression.Lambda(predicate.Body,
                                                     false,
                                                     Expression.Parameter(actual?.GetType() ?? typeof(T), "actual"));
                AssertedDetail = ExpressionToCodeConfiguration.DefaultAssertionConfiguration
                                                        .WithPrintedListLengthLimit(15)
                                                        .WithMaximumValueLength(80)
                                                        .WithOmitImplicitCasts(true)
                                                        .GetAnnotatedToCode()
                                                        .AnnotatedToCode(assertActual);
                var result = predicate.CompileFast()(actual, comparedTo);
                Result = result;
            } catch (Exception e) { ExceptionThrownByEvaluation = e; }
        }

        /// <summary>
        ///     Evaluates whether <paramref name="predicate" /> is true of <paramref name="actual" />, and stores the result of the
        ///     evaluation or, if
        ///     an exception is thrown during evaluation, catches and stores the exception instead.
        /// </summary>
        /// <param name="actual">The value under assertion</param>
        /// <param name="predicate">The predicate to apply</param>
        /// <param name="comment">
        ///     Occurrences of "{{actual}}" in the comment string will be replace with <paramref name="actual" />
        ///     ?.ToString()
        /// </param>
        /// <param name="commentArgs">will be inserted into <paramref name="comment" /> using string.Format()</param>
        public Assertion(T actual, Expression<Func<T, bool>> predicate, string comment, object[] commentArgs)
        {
            try
            {
                Actual  = ActualToString(actual);
                Comment = CommentFormattedV1(Actual, comment, commentArgs);
                var assertActual = Expression.Lambda(predicate.Body,
                                                     false,
                                                     Expression.Parameter(actual?.GetType() ?? typeof(T), "actual"));
                AssertedDetail = ExpressionToCodeConfiguration.DefaultAssertionConfiguration
                                                        .WithPrintedListLengthLimit(15)
                                                        .WithMaximumValueLength(80)
                                                        .WithOmitImplicitCasts(true)
                                                        .GetAnnotatedToCode()
                                                        .AnnotatedToCode(assertActual);
                var result = predicate.CompileFast()(actual);
                Result = result;
            } catch (Exception e) { ExceptionThrownByEvaluation = e; }
        }

        /// <summary>
        ///     Evaluates whether <paramref name="predicate" /> is true of <paramref name="actual" />, and stores the result of the
        ///     evaluation or, if
        ///     an exception is thrown during evaluation, catches and stores the exception instead.
        /// </summary>
        /// <param name="actual">The value under assertion</param>
        /// <param name="predicate">The predicate to apply</param>
        /// <param name="comment">
        ///     Occurrences of "{{actual}}" in the comment string will be replace with <paramref name="actual" />
        ///     ?.ToString()
        /// </param>
        /// <param name="commentArgs">will be inserted into <paramref name="comment" /> using string.Format()</param>
        public Assertion(
            T                                   actual,
            Expression<Func<T, BoolWithString>> predicate,
            string                              comment = null,
            params object[]                     commentArgs)
        {
            try
            {
                Actual  = ActualToString(actual);
                Comment = CommentFormattedV1(Actual, comment, commentArgs);
                var result = predicate.CompileFast()(actual);
                Result   = result;
                AssertedDetail =                 AssertedDetail = ExpressionToCodeConfiguration.DefaultAssertionConfiguration
                    .WithPrintedListLengthLimit(15)
                    .WithMaximumValueLength(80)
                    .WithOmitImplicitCasts(true)
                    .GetAnnotatedToCode()
                    .AnnotatedToCode(predicate);
            } catch (Exception e) { ExceptionThrownByEvaluation = e; }
        }

        /// <summary>The actual value about which something is to be asserted.</summary>
        public string Actual { get; }

        /// <summary>The expression which was evaluated to obtain <see cref="Actual"/>.</summary>
        public string ActualExpression { get; }
        
        /// <summary>
        ///     A description of what was asserted.
        /// </summary>
        public string Asserted { get; }
        
        /// <summary>
        ///     A C# code description of what was asserted. The value is generated using <see cref="ExpressionToCode" />
        /// </summary>
        public string AssertedDetail { get; }

        /// <summary>
        ///     <c>True</c> if the predicate asserted of <see cref="Actual" /> was true
        ///     <c>False</c> if the predicate asserted of <see cref="Actual" /> was false
        ///     <c>null</c> if attempting to evaluate the predicate of <see cref="Actual" /> failed. In this case,
        ///     <see cref="ExceptionThrownByEvaluation" /> will usually be populated
        /// </summary>
        /// <remarks>If an Exception is thrown during evaluation, then <see cref="ExceptionThrownByEvaluation" /> will contain the Exception thrown.</remarks>
        public bool? Result { get; }

        /// <summary>Synonym for <c>Result.HasValue &amp;&amp; Result.Value</c></summary>
        public bool DidPass => Result.HasValue && Result.Value;

        /// <summary>If an Exception is thrown when attempting to evaluate the assertion, it will be recorded and exposed here.</summary>
        public Exception ExceptionThrownByEvaluation { get; }

        /// <summary>An explanation of the assertion, if one was provided by the calling code.</summary>
        public string Comment { get; }

        /// <summary>
        ///     A full description of the Assertion including string representations of
        ///     <see cref="P:TestBase.Assertion`1.Actual" />,
        ///     <see cref="P:TestBase.Assertion`1.Asserted" /> and <see cref="P:TestBase.Assertion`1.Comment" />
        /// </summary>
        public override string Message => ToStringEvenIfPassed();

        static string CommentFormattedV1(string actual, string comment, object[] commentArgs)
        {
            var commentFormatted = comment?.Replace("{{actual}}", actual);
            if (commentFormatted != null && commentArgs?.Length > 0)
                commentFormatted = string.Format(commentFormatted, commentArgs);
            return commentFormatted;
        }

        static string ActualToString(T actual)
        {
            foreach (var toString in PreferredToStringMethod)
                try { return StringifyMethods[toString](actual); }
                // ReSharper disable once EmptyGeneralCatchClause
                catch { }

            try { return actual.ToString(); } catch { return "actual"; }
        }

        /// <summary>Treat an <see cref="Assertion" /> as a <c>Boolean</c></summary>
        /// <param name="assertion"></param>
        /// <returns>
        ///     <c>assertion.Result.HasValue &amp;&amp; assertion.Result.Value</c>
        /// </returns>
        public static implicit operator bool(Assertion<T> assertion)
        {
            return assertion.Result.HasValue && assertion.Result.Value;
        }

        /// <summary>Treat an <see cref="Assertion" /> as a <see cref="BoolWithString" /></summary>
        /// <param name="assertion"></param>
        /// <returns>
        ///     A <see cref="BoolWithString" /> which, if the assertion has failed, will contain the assertion failure
        ///     description.
        /// </returns>
        public static implicit operator BoolWithString(Assertion<T> assertion)
        {
            return new BoolWithString(assertion.DidPass, assertion.ToString());
        }

        /// <returns>A description of the failed assertion.</returns>
        public override string ToString() => ToStringEvenIfPassed();

        /// <returns>A description of the assertion.</returns>
        public string ToStringEvenIfPassed()
        {
            var          resultHeader   = DidPass ? "Passed : " : "Failed : ";
            const string actualHeader   = "Actual : ";
            const string assertedHeader = "Asserted : ";
            const string divider        = "----------------------------";
            string output = string.Join(nl,
                resultHeader,
                actualHeader,
                divider,
                Actual);
            if (!ActualExpression.HasTheSameWordsAs(Actual))
            {
                output += nl + ActualExpression;
            }
            output += nl + divider;
            output = string.Join(nl, output, assertedHeader + Asserted);
            if (!AssertedDetail.HasTheSameWordsAs(Asserted))
            {
                output += nl + AssertedDetail;
            }
            if (!string.IsNullOrEmpty(Comment))
            {
                output += nl + divider + Comment;
            }
            if (!Result.HasValue)
            {
                output += nl + divider + ExceptionThrownByEvaluation;
            }
            return output;
        }
        
        static readonly string RegexCSharpIdentifier =
            @"@?[_\p{L}\p{Nl}][\p{L}\p{Nl}\p{Mn}\p{Mc}\p{Nd}\p{Pc}\p{Cf}\.]*"; 
    }
    /// <inheritdoc />
    /// <summary>
    ///     An Assertion is throwable (it inherits from Exception) but need not indicate an assertion
    ///     <em>failure</em>; it might hold an assertion pass.
    /// </summary>
    /// <remarks>
    ///     The <c>Assertion</c> class and its subclasses does not conform to the convention that
    ///     <c>Exception</c> class names end in "Exception". Instead they end in "Assertion".
    /// </remarks>
    public class Assertion : Exception
    {
        /// <inheritdoc />
        protected Assertion() { }

        /// <inheritdoc />
        public Assertion(string message) : base(message) { }

        /// <summary>
        ///     Create an <c>Assertion</c> that <paramref name="predicate" /> holds.
        ///     A convenience method to avoid having specify the TypeParameter.
        /// </summary>
        /// <param name="actual"></param>
        /// <param name="predicate"></param>
        /// <param name="comment"></param>
        /// <param name="commentArgs"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static Assertion<T> New<T>(
            T                         actual,
            Expression<Func<T, bool>> predicate,
            string                    comment,
            object[]                  commentArgs)
        {
            return new Assertion<T>(actual, predicate, comment, commentArgs);
        }

        /// <summary>
        ///     Create an <c>Assertion</c> that <paramref name="predicate" /> holds.
        ///     A convenience method to avoid having specify the TypeParameter <typeparamref name="T" />.
        /// </summary>
        /// <param name="actual"></param>
        /// <param name="predicate"></param>
        /// <param name="comment"></param>
        /// <param name="commentArgs"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static Assertion<T> New<T>(
            T                                   actual,
            Expression<Func<T, BoolWithString>> predicate,
            string                              comment,
            object[]                            commentArgs)
        {
            return new Assertion<T>(actual, predicate, comment, commentArgs);
        }
    }

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
            params object[]           commentArgs) : base(actual, predicate, comment, commentArgs)
        {
        }
    }
}
