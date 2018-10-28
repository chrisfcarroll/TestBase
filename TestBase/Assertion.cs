using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using ExpressionToCodeLib;
using FastExpressionCompiler;
using Newtonsoft.Json;

namespace TestBase
{
    /// <inheritdoc />
    /// <summary>An Assertion is throwable (it inherits from Exception) but need not indicate an assertion
    /// <em>failure</em>; it might hold an assertion pass.</summary>
    /// <remarks>The <c>Assertion</c> class and its subclasses does not conform to the convention that
    /// <c>Exception</c> class names end in "Exception". Instead they end in "Assertion".</remarks>
    public class Assertion : Exception
    {
        /// <summary>Create an <c>Assertion</c> that <paramref name="predicate"/> holds.
        /// A convenience method to avoid having specify the TypeParameter.</summary>
        /// <param name="actual"></param>
        /// <param name="predicate"></param>
        /// <param name="comment"></param>
        /// <param name="commentArgs"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static Assertion<T> New<T>(T actual, Expression<Func<T, bool>> predicate, string comment,object[] commentArgs) 
                      => new Assertion<T>(actual, predicate, comment, commentArgs);

        /// <summary>Create an <c>Assertion</c> that <paramref name="predicate"/> holds.
        /// A convenience method to avoid having specify the TypeParameter <typeparamref name="T"/>.</summary>
        /// <param name="actual"></param>
        /// <param name="predicate"></param>
        /// <param name="comment"></param>
        /// <param name="commentArgs"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static Assertion<T> New<T>(T actual, Expression<Func<T, BoolWithString>> predicate, string comment,object[] commentArgs) 
                      => new Assertion<T>(actual, predicate, comment, commentArgs);

        /// <inheritdoc />
        protected Assertion(){ } 

        /// <inheritdoc />
        public Assertion(string message):base(message) { }
    }

    /// <inheritdoc />
    /// <summary>A Precondition is an Assertion. Calling it a precondition is presumed to indicate that it is to be understood as a precondition</summary>
    /// <typeparam name="T"></typeparam>
    public class Precondition<T> : Assertion<T>
    {
        /// <inheritdoc />
        /// <summary>Create a <c>Precondition</c> which asserts <paramref name="predicate" /></summary>
        /// <param name="actual"></param>
        /// <param name="predicate"></param>
        /// <param name="comment"></param>
        /// <param name="commentArgs"></param>
        public Precondition(T actual, Expression<Func<T,bool>> predicate, string comment = null, params object[] commentArgs) : base(actual, predicate, comment, commentArgs){}
    }

    /// <summary>An Assertion about an instance. 
    /// An Assertion is throwable (it inherits from Exception) but need not indicate an assertion failure; it might hold an assertion pass.</summary>
    /// <typeparam name="T"></typeparam>
    public class Assertion<T> : Assertion
    {
        static readonly string nl = Environment.NewLine;

        /// <summary>The actual value about which something is to be asserted.</summary>
        public string Actual { get; }

        /// <summary>A C# code description of what was asserted. The value is generated using <see cref="ExpressionToCode"/></summary>
        public string Asserted { get; }

        /// <summary>
        /// <c>True</c> if the predicate asserted of <see cref="Actual"/> was true
        /// <c>False</c> if the predicate asserted of <see cref="Actual"/> was false
        /// <c>null</c> if attempting to evaluate the predicate of <see cref="Actual"/> failed. In this case, <see cref="Exception"/> will usually be populated
        /// </summary>
        /// <remarks>If an Exception is thrown during evaluation, then <see cref="Exception"/> will contain the Exception thrown.</remarks>
        public bool? Result { get; }

        /// <summary>Synonym for <c>Result.HasValue && Result.Value</c></summary>
        public bool DidPass => Result.HasValue && Result.Value;

        /// <summary>If an Exception is thrown when attempting to evaluate the assertion, it will be recorded and exposed here.</summary>
        public Exception Exception { get; }

        /// <summary>An explanation of the assertion, if one was provided by the calling code.</summary>
        public string Comment { get; }

        /// <summary>A full description of the Assertion including string representations of <see cref="P:TestBase.Assertion`1.Actual" />,
        /// <see cref="P:TestBase.Assertion`1.Asserted" /> and <see cref="P:TestBase.Assertion`1.Comment" /></summary>
        public override string Message => ToStringEvenIfPassed();

        /// <summary>
        /// Evaluates whether <paramref name="predicate"/> is true of <paramref name="actual"/>, and stores the result of the evaluation or, if 
        /// an exception is thrown during evaluation, catches and stores the exception instead.
        /// </summary>
        /// <param name="actual">The value under assertion</param>
        /// <param name="comparedTo">The expected value, or the 'comparable' value to be used in <paramref name="predicate"/>(actual,expected).</param>
        /// <param name="predicate">The predicate to apply</param>
        /// <param name="comment">A format string to provide information about the assertion.
        /// <em>Occurrences of "{{actual}}" in the comment string will be replace with <paramref name="actual"/>?.ToString()</em></param>
        /// <param name="commentArgs">will be inserted into <paramref name="comment"/> using string.Format()</param>
        public Assertion(T actual,  T comparedTo, Expression<Func<T,T,bool>> predicate, string comment, object[] commentArgs)
        {
            try
            {
                Actual=ActualToString(actual);
                Comment=CommentFormatted(Actual, comment, commentArgs);
                var assertActual = Expression.Lambda(predicate.Body, false, Expression.Parameter(actual?.GetType()??typeof(T), "actual"));
                Asserted= ExpressionToCodeConfiguration.DefaultAssertionConfiguration
                    .WithPrintedListLengthLimit(15)
                    .WithMaximumValueLength(80)
                    .WithOmitImplicitCasts(true)
                    .GetAnnotatedToCode().AnnotatedToCode(assertActual);
                var result = predicate.CompileFast()(actual,comparedTo);
                Result = result;
            }
            catch (Exception e)
            {
                Exception = e;
            }
        }

        /// <summary>
        /// Evaluates whether <paramref name="predicate"/> is true of <paramref name="actual"/>, and stores the result of the evaluation or, if 
        /// an exception is thrown during evaluation, catches and stores the exception instead.
        /// </summary>
        /// <param name="actual">The value under assertion</param>
        /// <param name="predicate">The predicate to apply</param>
        /// <param name="comment">Occurrences of "{{actual}}" in the comment string will be replace with <paramref name="actual"/>?.ToString()</param>
        /// <param name="commentArgs">will be inserted into <paramref name="comment"/> using string.Format()</param>
        public Assertion(T actual, Expression<Func<T,bool>> predicate, string comment, object[] commentArgs)
        {
            try
            {
                Actual=ActualToString(actual);
                Comment=CommentFormatted(Actual, comment, commentArgs);
                var assertActual = Expression.Lambda(predicate.Body, false, Expression.Parameter(actual?.GetType()??typeof(T), "actual"));
                Asserted= ExpressionToCodeConfiguration.DefaultAssertionConfiguration
                    .WithPrintedListLengthLimit(15)
                    .WithMaximumValueLength(80)
                    .WithOmitImplicitCasts(true)
                    .GetAnnotatedToCode().AnnotatedToCode(assertActual);
                var result = predicate.CompileFast()(actual);
                Result = result;
            }
            catch (Exception e)
            {
                Exception = e;
            }
        }
        /// <summary>
        /// Evaluates whether <paramref name="predicate"/> is true of <paramref name="actual"/>, and stores the result of the evaluation or, if 
        /// an exception is thrown during evaluation, catches and stores the exception instead.
        /// </summary>
        /// <param name="actual">The value under assertion</param>
        /// <param name="predicate">The predicate to apply</param>
        /// <param name="comment">Occurrences of "{{actual}}" in the comment string will be replace with <paramref name="actual"/>?.ToString()</param>
        /// <param name="commentArgs">will be inserted into <paramref name="comment"/> using string.Format()</param>
        public Assertion(T actual, Expression<Func<T, BoolWithString>> predicate, string comment = null, params object[] commentArgs)
        {
            try
            {
                Actual=ActualToString(actual);
                Comment=CommentFormatted(Actual, comment, commentArgs);
                var result = predicate.CompileFast()(actual);
                Result = result;
                Asserted = result.ToString();
            }
            catch (Exception e)
            {
                Exception = e;
            }
        }

        static string CommentFormatted(string actual, string comment, object[] commentArgs)
        {
            var commentFormatted = comment?.Replace("{{actual}}", actual);
            if (commentFormatted != null && commentArgs?.Length > 0) { commentFormatted = string.Format(commentFormatted, commentArgs); }
            return commentFormatted;
        }
        
        static string ActualToString(T actual)
        {
            foreach (var toString in PreferredToStringMethod)
                try
                {
                    return StringifyMethods[toString](actual);
                }
                // ReSharper disable once EmptyGeneralCatchClause
                catch{}
            try { return actual.ToString(); }
            catch { return "actual"; }
        }

        /// <summary>Called by <seealso cref="ActualToString"/>. Modify this if you want to change the order of methods attempted to Stringify actual.</summary>
        public static StringifyMethod[] PreferredToStringMethod =
        {
            StringifyMethod.DeclaredToStringElseThrow,
            StringifyMethod.ExpressionToCodeStringify,
            StringifyMethod.NewtonsoftJsonSerialize,
            StringifyMethod.InheritedToString,
        };

        static Dictionary<StringifyMethod, Func<object, string>> StringifyMethods = new Dictionary<StringifyMethod, Func<object, string>>
        {
            {
                StringifyMethod.DeclaredToStringElseThrow,
                o => o==null ? "<null>" : o.GetType().GetMethod("ToString", BindingFlags.Instance |BindingFlags.DeclaredOnly | BindingFlags.Public) !=null ? o.ToString() : throw new ArgumentNullException("actual")
            },
            {StringifyMethod.ExpressionToCodeStringify, ObjectStringify.Default.PlainObjectToCode},
            {StringifyMethod.NewtonsoftJsonSerialize, o => JsonConvert.SerializeObject(o, BestEffortJsonSerializerSettings.Serializer)},
            {StringifyMethod.InheritedToString, o=>o.ToString()},
        };


        /// <summary>Treat an <see cref="Assertion"/> as a <c>Boolean</c></summary>
        /// <param name="assertion"></param>
        /// <returns><c>assertion.Result.HasValue && assertion.Result.Value</c></returns>
        public static implicit operator bool(Assertion<T> assertion) { return assertion.Result.HasValue && assertion.Result.Value; }

        /// <summary>Treat an <see cref="Assertion"/> as a <see cref="BoolWithString"/></summary>
        /// <param name="assertion"></param>
        /// <returns>A <see cref="BoolWithString"/> which, if the assertion has failed, will contain the assertion failure description.</returns>
        public static implicit operator BoolWithString(Assertion<T> assertion) { return new BoolWithString(assertion.DidPass, assertion.ToString()); }

        /// <returns>A description of the failed assertion.</returns>
        public override string ToString()
        {
            return ToStringEvenIfPassed();
        }

        /// <returns>A description of the assertion.</returns>
        public string ToStringEvenIfPassed()
        {
            var resultHeader = DidPass ? "Passed : " : "Failed : ";
            const string actualHeader = "Actual : ";
            const string assertedHeader = "Asserted : ";
            const string divider = "----------------------------";
            return Result.HasValue
                ? string.Join(nl, resultHeader , Comment, actualHeader, divider, Actual, divider, assertedHeader + Asserted)
                : string.Join(nl, resultHeader , Comment, actualHeader, divider, Actual, divider, assertedHeader + Asserted, Exception.ToString());
        }

#pragma warning disable 1591
        public enum StringifyMethod
        {
            DeclaredToStringElseThrow=0,
            ExpressionToCodeStringify=1,
            NewtonsoftJsonSerialize=2,
            InheritedToString=3
        }
#pragma warning restore 1591

    }

}