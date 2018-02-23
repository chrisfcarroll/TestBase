using System;
using System.Linq.Expressions;
using System.Reflection;
using Newtonsoft.Json;

namespace TestBase
{
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
        public static T That<T>(T actual, Expression<Predicate<T>> predicate, string comment = null, params object[] commentArgs)
        {
            var result= new Assertion<T>(actual, predicate, comment, commentArgs);
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
        public static T Precondition<T>(T actual, Expression<Predicate<T>> predicate, string comment = null, params object[] commentArgs)
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

        public static T Throws<T,TE>(T actual, Expression<Predicate<T>> predicate, TE dummyForTypeInference=null, string comment = null, params object[] commentArgs) where TE : Exception
        {
            Assertion<T> a;
            try
            {
                a = new Assertion<T>(actual, predicate, comment, commentArgs);
            }
            catch (TE) { return actual;}
            catch (Exception ex)
            {
                throw That(ex, e => e is TE, $"Expected to throw a {typeof(TE)} but threw {ex}");
            }
            throw new ShouldHaveThrownException(a.Message);
        }
        public static T DoesNotThrow<T>(T actual, Expression<Predicate<T>> predicate, string comment = null, params object[] commentArgs)
        {
            try
            {
                predicate.Compile()(actual);
            }
            catch (Exception ex)
            {
                throw ShouldNotThrowException.For(actual,predicate, $"Threw {ex} but expected not to throw.", commentArgs);
            }
            return actual;
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

        public static ShouldHaveThrownException For<T>(T actual, Expression<Predicate<T>> predicate, string comment=null, params object[] args)
        {
            return new ShouldHaveThrownException( new Assertion<T>(actual, predicate, comment, args).Message );
        }
    }
    /// <summary>
    /// An Exception thrown when <see cref="Assert.DoesNotThrow{T}"/> finds that an Assertion <em>was</em> thrown.
    /// </summary>
    public class ShouldNotThrowException : Exception
    {
        public ShouldNotThrowException(string message) : base(message) { }
        public ShouldNotThrowException(Exception a) : base(a.Message){}
        public static ShouldNotThrowException For<T>(T actual, Expression<Predicate<T>> predicate, string comment = null, params object[] args)
        {
            return new ShouldNotThrowException(new Assertion<T>(actual, predicate, comment, args).Message);
        }
    }

    /// <summary>A Precondition is an Assertion. Calling it a precondition is presumed to indicate that it is to be understood as a precondition</summary>
    /// <typeparam name="T"></typeparam>
    public class Precondition<T> : Assertion<T>
    {
        public Precondition(T actual, Expression<Predicate<T>> predicate, string comment = null, params object[] commentArgs) : base(actual, predicate, comment, commentArgs){}
    }

    /// <summary>An Assertion is throwable (it inherits from Exception) but need not indicate an assertion failure.</summary>
    /// <typeparam name="T"></typeparam>
    public class Assertion<T> : Exception
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
        /// <param name="predicate">The predicate to apply</param>
        /// <param name="comment">Occurrences of "{{actual}}" in the comment string will be replace with <paramref name="actual"/>?.ToString()</param>
        /// <param name="commentArgs">will be inserted into <paramref name="comment"/> using string.Format()</param>
        public Assertion(T actual, Expression<Predicate<T>> predicate, string comment, object[] commentArgs)
        {
            try
            {
                Actual=ActualToString(actual);
                Comment=CommentFormatted(Actual, comment, commentArgs);
                var result = predicate.Compile()(actual);
                Result = result;
                Asserted = result.ToString();
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
                var result = predicate.Compile()(actual);
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
            string commentFormatted;
            commentFormatted = comment?.Replace("{{actual}}", actual);
            if (commentFormatted != null && commentArgs?.Length > 0) { commentFormatted = string.Format(commentFormatted, commentArgs); }
            return commentFormatted;
        }

        static string ActualToString(T actual)
        {
            string actualToString;
            if (actual == null) { actualToString = "<null>"; }
            else
                try
                {
                    actualToString =
                        (actual.GetType().GetMethod("ToString", BindingFlags.DeclaredOnly | BindingFlags.Public) != null)
                            ? actual.ToString()
                            : JsonConvert.SerializeObject(actual, Assert.BestEffortJsonSerializerSettings.Serializer);
                }
                catch
                {
                    try { actualToString = actual.ToString(); }
                    catch { actualToString = "actual"; }
                }
            return actualToString;
        }

        public static implicit operator bool(Assertion<T> assertion) { return assertion.Result.HasValue && assertion.Result.Value; }
        public static implicit operator BoolWithString(Assertion<T> assertion) { return new BoolWithString(assertion.DidPass, assertion.ToString()); }

        public override string ToString()
        {
            return DidPass ? "" : ToStringEvenIfPassed();
        }
        public string ToStringEvenIfPassed()
        {
            var resultHeader = DidPass ? "" : "Fail :";
            var actualHeader ="Actual:";
            return Result.HasValue
                ? string.Join(nl+nl, resultHeader, Comment, actualHeader,  Actual, Asserted)
                : string.Join(nl+nl, resultHeader, Comment, actualHeader,  Actual, Asserted, Exception.ToString());
        }
    }
}