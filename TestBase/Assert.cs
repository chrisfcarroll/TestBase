using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using FastExpressionCompiler;
using Newtonsoft.Json;
using ExpressionToCodeLib;

namespace TestBase
{
    /// <summary>Static convenience methods for invoking <see cref="Assertion"/>s.</summary>
    public static class Assert
    {
        /// <summary>
        /// If <paramref name="predicate"/>(<paramref name="actual"/>) evaluates to true, then <paramref name="actual"/> is returned.
        /// If not, an <see cref="Assertion{T}"/> is thrown, containing details of the assertion failure.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="actual"></param>
        /// <param name="predicate"></param>
        /// <param name="comment"></param>
        /// <param name="commentArgs"></param>
        /// <returns><paramref name="actual"/>, if the precondition succeeds</returns>
        /// <exception cref="Assertion{T}">thrown if the precondition fails.</exception>
        public static T That<T>(T actual, Expression<Func<T,BoolWithString>> predicate, string comment = null, params object[] commentArgs)
        {
            var result = new Assertion<T>(actual, predicate, comment, commentArgs);
            return result ? actual : throw result;
        }
        /// <summary>
        /// If <paramref name="predicate"/>(<paramref name="actual"/>) evaluates to true, then <paramref name="actual"/> is returned.
        /// If not, an <see cref="Assertion{T}"/> is thrown, containing details of the assertion failure.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="actual"></param>
        /// <param name="predicate"></param>
        /// <param name="comment"></param>
        /// <param name="commentArgs"></param>
        /// <returns><paramref name="actual"/>, if the precondition succeeds</returns>
        /// <exception cref="Assertion{T}">thrown if the precondition fails.</exception>
        public static T That<T>(T actual, Expression<Func<T,bool>> predicate, string comment = null, params object[] commentArgs)
        {
            var result= new Assertion<T>(actual, predicate, comment, commentArgs);
            return result ? actual : throw result;
        }

        /// <summary>
        /// If <paramref name="predicate"/>(<paramref name="actual"/>) evaluates to true, then <paramref name="actual"/> is returned.
        /// If not, an <see cref="Assertion{T}"/> is thrown, containing details of the assertion failure.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="actual"></param>
        /// <param name="comparedTo"></param>
        /// <param name="predicate"></param>
        /// <param name="comment"></param>
        /// <param name="commentArgs"></param>
        /// <returns><paramref name="actual"/>, if the precondition succeeds</returns>
        /// <exception cref="Assertion{T}">thrown if the precondition fails.</exception>
        public static T That<T>(T actual, T comparedTo, Expression<Func<T,T,BoolWithString>> predicate, string comment = null, params object[] commentArgs)
        {
            var result = new Assertion<T>(actual, comparedTo, predicate.Chain( x=>x.AsBool)  , comment, commentArgs);
            return result ? actual : throw result;
        }

        /// <summary>
        /// If <paramref name="predicate"/>(<paramref name="actual"/>) evaluates to true, then <paramref name="actual"/> is returned.
        /// If not, an <see cref="Precondition{T}"/> is thrown, containing details of the assertion failure.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="actual"></param>
        /// <param name="predicate"></param>
        /// <param name="comment"></param>
        /// <param name="commentArgs"></param>
        /// <returns><paramref name="actual"/>, if the assertion succeeds</returns>
        /// <exception cref="Precondition{T}">thrown if the assertion fails.</exception>
        public static T Precondition<T>(T actual, Expression<Func<T,bool>> predicate, string comment = null, params object[] commentArgs)
        {
            var result = new Precondition<T>(actual, predicate, comment, commentArgs);
            return result ? actual : throw result;
        }

        /// <summary>
        /// Creates an <see cref="Assertion{T}"/> for the assertion <paramref name="actual"/>.
        /// If <paramref name="actual"/> evaluates to true, then the <see cref="Assertion{T}"/> is returned.
        /// Otherwise, it is thrown.
        /// If not, an <see cref="Assertion{T}"/> is thrown, containing details of the assertion failure.
        /// </summary>
        /// <param name="actual"></param>
        /// <param name="comment"></param>
        /// <param name="commentArgs"></param>
        /// <returns>An <see cref="Assertion{T}"/> for <paramref name="actual"/></returns>
        /// <exception cref="Assertion{T}">thrown if the precondition fails.</exception>
        public static Assertion<BoolWithString> That(BoolWithString actual, string comment = null, params object[] commentArgs)
        {
            var result = new Assertion<BoolWithString>(actual, a => a, comment, commentArgs);
            return result ? result : throw result;
        }

        /// <summary>
        /// Creates an <see cref="Precondition{T}"/> for the assertion <paramref name="actual"/>.
        /// If <paramref name="actual"/> evaluates to true, then the <see cref="Precondition{T}"/> is returned.
        /// Otherwise, it is thrown.
        /// If not, an <see cref="Precondition{T}"/> is thrown, containing details of the assertion failure.
        /// </summary>
        /// <param name="actual"></param>
        /// <param name="comment"></param>
        /// <param name="commentArgs"></param>
        /// <returns>An <see cref="Precondition{T}"/> for <paramref name="actual"/></returns>
        /// <exception cref="Precondition{T}">thrown if the precondition fails.</exception>
        public static Precondition<BoolWithString> Precondition(BoolWithString actual, string comment = null, params object[] commentArgs)
        {
            var result = new Precondition<BoolWithString>(actual, a => a, comment, commentArgs);
            return result ? result : throw result;
        }

        /// <summary>
        /// Assert that <code><paramref name="action"/>.Compile()()</code> throws, catching the exception and returning it.
        /// </summary>
        /// <returns>The caught exception.</returns>
        /// <exception cref="ShouldHaveThrownException">is thrown if <paramref name="action"/> does not throw.</exception>
        public static Exception Throws(Expression<Action> action, string comment = null, params object[] commentArgs) =>Throws<Exception>(action.CompileFast(), comment, commentArgs);

        /// <summary>
        /// Asserts that <code><paramref name="action"/>()</code> throws a <typeparamref name="TE"/>, catching the exception and returning it.
        /// For an overload that accepts an <see cref="Expression{Action}"/> argument, see <see cref="Throws"/>.
        /// </summary>
        /// <typeparam name="TE"></typeparam>
        /// <returns>The caught exception</returns>
        /// <exception cref="ShouldHaveThrownException">is thrown if <paramref name="action"/> does not throw.</exception>
        public static TE Throws<TE>(Action action, string comment = null,params object[] commentArgs) where TE : Exception
        {
            try
            {
                action();
            }
            catch (TE ex) { return ex;}
            catch (Exception ex)
            {
                throw That(ex, e => e is TE, $"Expected to throw a {typeof(TE)} but threw {ex}");
            }
            throw new ShouldHaveThrownException( action.ToString());
        }

        /// <summary>
        /// Asserts that <code><paramref name="predicate"/>( <paramref name="actual"/> )</code> throws a <typeparamref name="TE"/>, catching the exception and returning it.
        /// </summary>
        /// <typeparam name="TE"></typeparam>
        /// <returns>The caught exception</returns>
        /// <exception cref="ShouldHaveThrownException">is thrown if <paramref name="predicate"/> does not throw.</exception>
        public static T Throws<T,TE>(T actual, Expression<Func<T,bool>> predicate, TE dummyForTypeInference=null, string comment = null, params object[] commentArgs) where TE : Exception
        {
            return Throws<T, TE>(actual, predicate, comment, commentArgs);
        }

        static T Throws<T, TE>(T actual, Expression<Func<T,bool>> predicate, string comment, object[] commentArgs) where TE : Exception
        {
            Assertion<T> a;
            try
            {
                a = new Assertion<T>(actual, predicate, comment, commentArgs);
            }
            catch (TE)
            {
                return actual;
            }
            catch (Exception ex)
            {
                throw That(ex, e => e is TE, $"Expected to throw a {typeof(TE)} but threw {ex}");
            }

            throw new ShouldHaveThrownException(a.Message);
        }

        /// <summary>
        /// Executes <code><paramref name="action"/>()</code>. If the execution throws, the thrown exception is wrapped in an <see cref="ShouldNotThrowException"/> and thrown.
        /// </summary>
        /// <returns><paramref name="action"/></returns>
        /// <exception cref="ShouldNotThrowException">is thrown if <paramref name="action"/> throws.</exception>
        public static Expression<Action> DoesNotThrow(Expression<Action> action, string comment = null, params object[] commentArgs)
        {
            try
            {
                action.Compile();
            }
            catch (Exception ex)
            {
                throw ShouldNotThrowException.For(action, $"Threw {ex} but expected not to throw.", commentArgs);
            }
            return action;
        }

        internal static class BestEffortJsonSerializerSettings
        {
            public static readonly JsonSerializerSettings Serializer =
                new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore, MissingMemberHandling = MissingMemberHandling.Ignore, ReferenceLoopHandling = ReferenceLoopHandling.Ignore };

            static BestEffortJsonSerializerSettings()
            {
                Serializer.Converters.Add(new DBNullConverter());
            }

            /// <summary>Converts <see cref="DBNull" /> to and from its name string value.</summary>
            public class DBNullConverter : JsonConverter
            {
                /// <summary>Writes the JSON representation of the object.</summary>
                /// <param name="writer">The <see cref="T:Newtonsoft.Json.JsonWriter" /> to write to.</param>
                /// <param name="value">The value.</param>
                /// <param name="serializer">The calling serializer.</param>
                public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
                {
                    writer.WriteNull();
                }

                /// <summary>Reads the JSON representation of the object.</summary>
                /// <param name="reader">The <see cref="T:Newtonsoft.Json.JsonReader" /> to read from.</param>
                /// <param name="objectType">Type of the object.</param>
                /// <param name="existingValue">The existing value of object being read.</param>
                /// <param name="serializer">The calling serializer.</param>
                /// <returns>The object value.</returns>
                public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
                {
                    return DBNull.Value;
                }

                /// <summary>
                /// Determines whether this instance can convert the specified object type.
                /// </summary>
                /// <param name="objectType">Type of the object.</param>
                /// <returns>
                /// <c>true</c> if this instance can convert the specified object type; otherwise, <c>false</c>.
                /// </returns>
                public override bool CanConvert(Type objectType)
                {
                    return objectType == typeof(DBNull);
                }
            }
        }

        public static void Fail(string format, params object[] args)
        {
            throw new Assertion(string.Format(format, args));
        }
    }

    /// <summary>
    /// An Exception indicating that an Exception was expected but was not thrown.
    /// </summary>
    public class ShouldHaveThrownException : Exception
    {
        public ShouldHaveThrownException(string message) : base(message){}

        public static ShouldHaveThrownException For<T>(Assertion<T> assertion)
        {
            return new ShouldHaveThrownException(assertion.Message);
        }

        public static ShouldHaveThrownException For<T>(T actual, Expression<Func<T,bool>> predicate, string comment=null, params object[] args)
        {
            return new ShouldHaveThrownException( new Assertion<T>(actual, predicate, comment, args).Message );
        }
    }
    /// <summary>
    /// An Exception thrown when <see cref="Assert.DoesNotThrow"/> finds that an Assertion <em>was</em> thrown.
    /// </summary>
    public class ShouldNotThrowException : Exception
    {
        public ShouldNotThrowException(string message) : base(message) { }
        public ShouldNotThrowException(Exception a) : base(a.Message){}
        public static ShouldNotThrowException For<T>(T actual, Expression<Func<T,bool>> predicate, string comment = null, params object[] args)
        {
            return new ShouldNotThrowException(new Assertion<T>(actual, predicate, comment, args).Message);
        }

        public static Exception For(Expression<Action> action, string comment, object[] commentArgs)
        {
            return new ShouldNotThrowException(Assertion.New(action, a=> BoolWithString.False( "Threw:" +ExpressionToCodeLib.ExpressionToCode.ToCode(action)), comment, commentArgs));
        }
    }

    /// <summary>A Precondition is an Assertion. Calling it a precondition is presumed to indicate that it is to be understood as a precondition</summary>
    /// <typeparam name="T"></typeparam>
    public class Precondition<T> : Assertion<T>
    {
        public Precondition(T actual, Expression<Func<T,bool>> predicate, string comment = null, params object[] commentArgs) : base(actual, predicate, comment, commentArgs){}
    }
    
    /// <summary>An Assertion is throwable (it inherits from Exception) but need not indicate an assertion <em>failure</em>; it might hold an assertion pass.</summary>
    public class Assertion : Exception
    {
        public static Assertion<T> New<T>(T actual, Expression<Func<T, bool>> predicate, string comment,object[] commentArgs)
        {
            return new Assertion<T>(actual, predicate, comment, commentArgs);
        }

        public static Assertion<T> New<T>(T actual, Expression<Func<T, BoolWithString>> predicate, string comment,object[] commentArgs)
        {
            return new Assertion<T>(actual, predicate, comment, commentArgs);
        }

        protected Assertion(){ } 
        public Assertion(string message):base(message) { }
    }

    /// <summary>An Assertion about an instance. 
    /// An Assertion is throwable (it inherits from Exception) but need not indicate an assertion failure; it might hold an assertion pass.</summary>
    /// <typeparam name="T"></typeparam>
    public class Assertion<T> : Assertion
    {
        static readonly string nl = Environment.NewLine;
        public string Actual { get; }
        public string Asserted { get; }
        public bool? Result { get; }
        public bool DidPass => Result.HasValue && Result.Value;
        public Exception Exception { get; }
        public string Comment { get; }
        public override string Message => ToStringEvenIfPassed();

        /// <summary>
        /// Evaluates whether <paramref name="predicate"/> is true of <paramref name="actual"/>, and stores the result of the evaluation or, if 
        /// an exception is thrown during evaluation, catches and stores the exception instead.
        /// </summary>
        /// <param name="actual">The value under assertion</param>
        /// <param name="comparedTo">The expected value, or the 'comparable' value to be used in <paramref name="predicate"/>(actual,expected).</param>
        /// <param name="predicate">The predicate to apply</param>
        /// <param name="comment">Occurrences of "{{actual}}" in the comment string will be replace with <paramref name="actual"/>?.ToString()</param>
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

        string CommentFormatted(string actual, string comment, object[] commentArgs)
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
                catch{}
            try { return actual.ToString(); }
            catch { return "actual"; }
        }

        /// <summary>
        /// Called by <seealso cref="ActualToString"/>. Modify this if you want to change the method used to Stringify actual.
        /// </summary>
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
            {StringifyMethod.NewtonsoftJsonSerialize, o => JsonConvert.SerializeObject(o, Assert.BestEffortJsonSerializerSettings.Serializer)},
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