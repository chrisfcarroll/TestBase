using System.Collections.Generic;
using System.Linq;

namespace TestBase
{
    /// <summary>Shoulds to assert than an item is or is not in a given collection</summary>
    public static class ItemOfEnumerableShoulds
    {
        /// <summary>Asserts that <c>list.ShouldContain(item)</c></summary>
        /// Synonym for <see cref="ShouldBeOneOf{T}(T,IEnumerable{T},string,objects[])"/>
        /// <returns><<paramref name="item"/></returns>
        public static T ShouldBeOneOf<T>(this T item, params T[] expected)
        {
            expected.ShouldContain(item, "Expected actual {0} to be one of expected {1} but wasn't.", item, expected);
            return item;
        }
        /// <summary>Synonym for <see cref="ShouldBeOneOf{T}"/>
        /// Asserts that <c>list.ShouldContain(item)</c></summary>
        /// <returns><<paramref name="item"/></returns>
        public static T ShouldBeOneOf<T>(this T item, IEnumerable<T> expected, string comment = null, params object[] args)
        {
            expected.ShouldContain(item, comment, args); return item;
        }
        /// <summary>Asserts that <c>list.ShouldContain(item)</c></summary>
        /// Synonym for <see cref="ShouldBeOneOf{T}(T,IEnumerable{T},string,objects[])"/>
        /// and <see cref="ShouldBeIn{T}(T,IEnumerable{T},string,objects[])"/>
        /// <returns><<paramref name="item"/></returns>
        public static T ShouldBeIn<T>(this T item, params T[] expected)
        {
            expected.ShouldContain(item, "Expected actual {0} to be one of expected {1} but wasn't.", item, expected);
            return item;
        }
        /// <summary>Asserts that <c>list.ShouldContain(item)</c></summary>
        /// Synonym for <see cref="ShouldBeOneOf{T}(T,IEnumerable{T},string,objects[])"/>
        /// <returns><<paramref name="item"/></returns>
        public static T ShouldBeIn<T>(this T item, IEnumerable<T> expected, string comment = null, params object[] args)
        {
            expected.ShouldContain(item, comment, args); return item;
        }

        /// <summary>Asserts that <c>list.ShouldNotContain(item)</c>.
        ///Synonym for <see cref="ShouldNotBeOneOf{T}(T,IEnumerable{T},string,objects[])"/>
        /// </summary>
        /// <returns><<paramref name="item"/></returns>
        public static T ShouldNotBeOneOf<T>(this T item, params T[] notExpected)
        {
            notExpected.ShouldNotContain(item, "Expected actual {0} to not be any of notexpected {0} it is.", item, notExpected); return item;
        }
        
        /// <summary>Asserts that <c>list.ShouldContain(item)</c></summary>
        /// <returns><<paramref name="item"/></returns>
        public static T ShouldNotBeOneOf<T>(this T item, IEnumerable<T> notExpected, string comment = null, params object[] args)
        {
            notExpected.ShouldNotContain(item, comment, args); return item;
        }
        
        /// <summary>Asserts that <c>list.ShouldNotContain(item)</c></summary>
        ///Synonym for <see cref="ShouldNotBeOneOf{T}(T,IEnumerable{T},string,objects[])"/>
        ///and for <see cref="ShouldNotBeInList{T}(T,IEnumerable{T},string,object[])"/>
        /// <returns><<paramref name="item"/></returns>
        public static T ShouldNotBeInList<T>(this T item, params T[] notExpected)
        {
            notExpected.ShouldNotContain(item, "Expected actual {0} to not be any of notexpected {0} it is.", item, notExpected); return item;
        }
        
        /// <summary>Asserts that <c>list.ShouldContain(item)</c></summary>
        ///Synonym for <see cref="ShouldNotBeOneOf{T}(T,IEnumerable{T},string,objects[])"/>
        /// <returns><<paramref name="item"/></returns>
        public static T ShouldNotBeInList<T>(this T item, IEnumerable<T> notExpected, string comment = null, params object[] args)
        {
            notExpected.ShouldNotContain(item, comment, args); return item;
        }        
    }
}