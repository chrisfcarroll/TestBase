// See .e.g.  http://stackoverflow.com/questions/1680602/what-is-the-algorithm-used-by-the-memberwise-equality-test-in-net-structs
// for a near-identical algorithm
//
//  Chris F Carroll https://github.com/chrisfcarroll/TestBase/blob/master/TestBase/Comparer.cs/
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.using System;//
// Public Domain.
//

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace TestBase
{
    /// <summary>
    ///     Compares objects memberwise, recursively, by value. All Properties (public or not), and public Fields, are
    ///     considered in the comparison.
    ///     Recursion stops when it arrives at a value type, or at a type (such as <see cref="string" />) which override
    ///     <see cref="object.Equals(object)" />
    /// </summary>
    public static class Comparer
    {
        /// <remarks>A synonym for <see cref="EqualsByValueJustOnCommonPublicReadableProperties(object,object)" /></remarks>
        /// <summary>
        ///     Uses <see cref="MemberCompare(object,object,IEnumerable{string},IEnumerable{string},double,bool)" /> to recursively
        ///     compare the values of
        ///     the public readable Members of <paramref name="left" /> and <paramref name="right" /> with identical names,
        ///     and reports the first discrepancy, if any.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns>
        ///     <see cref="BoolWithString.True" /> if the public readable Members of <paramref name="left" /> and
        ///     <paramref name="right" />
        ///     with identical names are all <see cref="EqualsByValue(object,object)" />,
        ///     or a <see cref="BoolWithString.False" />
        ///     bearing a description of the first discrepancy if not.
        /// </returns>
        public static BoolWithString PropertiesMatch(this object left, object right)
        {
            return EqualsByValueJustOnCommonPropertiesSatisfying(left, right, p => p.CanRead);
        }

        /// <remarks>Can be abbreviated to <see cref="PropertiesMatch" /></remarks>
        /// <summary>
        ///     Uses <see cref="MemberCompare(object,object,IEnumerable{string},IEnumerable{string},double,bool)" /> to recursively
        ///     compare the values of
        ///     the public readable Members of <paramref name="left" /> and <paramref name="right" /> with identical names,
        ///     and reports the first discrepancy, if any.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns>
        ///     <see cref="BoolWithString.True" /> if the public readable Members of <paramref name="left" /> and
        ///     <paramref name="right" />
        ///     with identical names are all <see cref="EqualsByValue(object,object)" />,
        ///     or a <see cref="BoolWithString.False" />
        ///     bearing a description of the first discrepancy if not.
        /// </returns>
        public static BoolWithString EqualsByValueJustOnCommonPublicReadableProperties(this object left, object right)
        {
            return EqualsByValueJustOnCommonPropertiesSatisfying(left, right, p => p.CanRead);
        }

        /// <summary>
        ///     Uses <see cref="MemberCompare(object,object,IEnumerable{string},IEnumerable{string},double,bool)" /> to recursively
        ///     compare the values of
        ///     the writeable Members of <paramref name="left" /> and <paramref name="right" />
        ///     and reports the first discrepancy, if any.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns>
        ///     <see cref="BoolWithString.True" /> if the writeable Members of <paramref name="left" /> and
        ///     <paramref name="right" />
        ///     with identical names are all <see cref="EqualsByValue(object,object)" />,
        ///     or a <see cref="BoolWithString.False" />
        ///     bearing a description of the first discrepancy if not.
        /// </returns>
        /// <remarks>
        ///     Writeable properties are often a subset of readable properties. Often, if an object is deserialized from a
        ///     database or datastore, then it is expected to be fully specified by its writeable properties.
        /// </remarks>
        public static BoolWithString EqualsByValueJustOnCommonPublicWriteableProperties(this object left, object right)
        {
            return EqualsByValueJustOnCommonPropertiesSatisfying(left, right, p => p.CanWrite);
        }

        /// <summary>
        ///     Uses <see cref="MemberCompare(object,object,IEnumerable{string},IEnumerable{string},double,bool)" /> to recursively
        ///     compare the values of
        ///     those Members on <paramref name="left" /> and <paramref name="right" />
        ///     which satisfy <paramref name="predicate" /> and reports the first discrepancy, if any.
        ///     All Properties (public or not), and public Fields, are considered in the comparison.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <param name="predicate"></param>
        /// <returns>
        ///     <see cref="BoolWithString.True" /> if the Members of <paramref name="left" /> and <paramref name="right" />
        ///     which satisfy <paramref name="predicate" /> are equal by value, or a <see cref="BoolWithString.False" />
        ///     bearing a description of the first discrepancy if not.
        /// </returns>
        static BoolWithString EqualsByValueJustOnCommonPropertiesSatisfying(
            this object              left,
            object                   right,
            Func<PropertyInfo, bool> predicate)
        {
            var leftProperties =
            left.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance).Where(predicate);
            var rightProperties =
            right.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance).Where(predicate);
            var commonProperties = leftProperties.Join(rightProperties,
                                                       l => $"{l.Name}-{l.PropertyType.FullName}",
                                                       r => $"{r.Name}-{r.PropertyType.FullName}",
                                                       (l, r) => l.Name);
            return MemberCompare(left, right, null, commonProperties, typesMustAlsoBeSame: false);
        }

        /// <summary>
        ///     Uses <see cref="MemberCompare(object,object,IEnumerable{string},IEnumerable{string},double,bool)" /> to recursively
        ///     compare the values of
        ///     the specified Members on <paramref name="left" /> and <paramref name="right" />
        ///     and reports the first discrepancy, if any.
        ///     Named Properties (public or not), and public Fields, are considered in the comparison.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <param name="propertiesToCompare">
        ///     a possibly empty list of <em>Member</em> names to restrict the comparisons to only those members.
        ///     To specify members of child members, provide the full dotted 'breadcrumb' to the member
        ///     to include, e.g. new List&lt;string&gt;{"Id","SomeProperty.SomePropertyOfThat.FieldName"}
        /// </param>
        /// <returns>
        ///     <see cref="BoolWithString.True" /> if the Members of <paramref name="left" /> and <paramref name="right" />
        ///     named in <paramref name="propertiesToCompare" /> are equal by value, or a
        ///     <see cref="BoolWithString.False" /> bearing a description of the first discrepancy if not.
        /// </returns>
        [Obsolete("Use the correctly named method EqualsByValuesJustOnMembersNamed instead.")]
        public static BoolWithString EqualsByValueJustOnPropertiesNamed(
            this object         left,
            object              right,
            IEnumerable<string> propertiesToCompare)
        {
            return EqualsByValuesJustOnMembersNamed(left, right, propertiesToCompare);
        }

        /// <summary>
        ///     Uses <see cref="MemberCompare(object,object,IEnumerable{string},IEnumerable{string},double,bool)" /> to recursively
        ///     compare the values of
        ///     the specified Members on <paramref name="left" /> and <paramref name="right" />
        ///     and reports the first discrepancy, if any.
        ///     Named Properties (public or not), and public Fields, are considered in the comparison.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <param name="membersToCompare">
        ///     a possibly empty list of Member names to restrict the comparisons to only those members.
        ///     To specify members of child members, provide the full dotted 'breadcrumb' to the member
        ///     to include, e.g. new List&lt;string&gt;{"Id","SomeProperty.SomePropertyOfThat.FieldName"}
        /// </param>
        /// <returns>
        ///     <see cref="BoolWithString.True" /> if the Members of <paramref name="left" /> and <paramref name="right" />
        ///     named in <paramref name="membersToCompare" /> are equal by value, or a
        ///     <see cref="BoolWithString.False" /> bearing a description of the first discrepancy if not.
        /// </returns>
        public static BoolWithString EqualsByValuesJustOnMembersNamed(
            this object         left,
            object              right,
            IEnumerable<string> membersToCompare)
        {
            return MemberCompare(left, right, null, membersToCompare, typesMustAlsoBeSame: false);
        }

        /// <summary>
        ///     A synonym for <see cref="EqualsByValue(object,object)" />
        ///     Uses <see cref="MemberCompare(object,object,IEnumerable{string},IEnumerable{string},double,bool)" /> to recursively
        ///     compare the values of
        ///     the Members of <paramref name="left" /> and <paramref name="right" />
        ///     and reports the first discrepancy, if any.
        ///     All Properties (public or not), and public Fields, are considered in the comparison.
        ///     Recursion stops when it arrives at a value type, or at a type (such as <see cref="String" />)
        ///     which override <see cref="object.Equals(object)" />
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <param name="floatTolerance"></param>
        /// <returns>
        ///     <see cref="BoolWithString.True" /> if the Members of <paramref name="left" /> and <paramref name="right" /> are
        ///     equal by value, or a
        ///     <see cref="BoolWithString.False" /> bearing a description of the first discrepancy if not.
        /// </returns>
        public static BoolWithString EqualsByValueOrDiffers(
            this object left,
            object      right,
            double      floatTolerance = 1e-14d)
        {
            return MemberCompare(left, right, null, null, floatTolerance, false);
        }

        /// <summary>
        ///     Uses <see cref="MemberCompare(object,object,IEnumerable{string},IEnumerable{string},double,bool)" /> to recursively
        ///     compare the values of
        ///     the specified Members on <paramref name="left" /> and <paramref name="right" />
        ///     and reports the first discrepancy, if any.
        ///     Named Properties (public or not), and public Fields, are considered in the comparison.
        ///     Recursion stops when it arrives at a value type, or at a type (such as <see cref="String" />)
        ///     which override <see cref="object.Equals(object)" />
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <param name="exclusionList">
        ///     a possibly empty list of Member names to exclude for the purposes of this
        ///     comparison. To exclude members of child members, provide the full dotted 'breadcrumb' to the member
        ///     to exclude, e.g. new List&lt;string&gt;{"Id","SomeProperty.SomePropertyOfThat.FieldName"}
        /// </param>
        /// <param name="floatTolerance"></param>
        /// <returns>
        ///     <see cref="BoolWithString.True" /> if the Members of <paramref name="left" /> and <paramref name="right" />
        ///     �excluding those named in <paramref name="exclusionList" />� are equal by value, or a
        ///     <see cref="BoolWithString.False" /> bearing a description of the first discrepancy if not.
        /// </returns>
        public static BoolWithString EqualsByValueOrDiffersExceptFor(
            this object         left,
            object              right,
            IEnumerable<string> exclusionList,
            double              floatTolerance = 1e-14d)
        {
            return MemberCompare(left, right, exclusionList, null, floatTolerance, false);
        }

        /// <summary>
        ///     Uses <see cref="MemberCompare(object,object,IEnumerable{string},IEnumerable{string},double,bool)" /> to recursively
        ///     compare the values of
        ///     the specified Members on <paramref name="left" /> and <paramref name="right" />
        ///     and reports the first discrepancy, if any.
        ///     Named Properties (public or not), and public Fields, are considered in the comparison.
        ///     Recursion stops when it arrives at a value type, or at a type (such as <see cref="String" />)
        ///     which override <see cref="object.Equals(object)" />
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns>
        ///     <see cref="BoolWithString.True" /> if the Members of <paramref name="left" /> and <paramref name="right" />
        ///     are equal by value, or a
        ///     <see cref="BoolWithString.False" /> bearing a description of the first discrepancy if not.
        /// </returns>
        public static BoolWithString EqualsByValue(this object left, object right)
        {
            return MemberCompare(left, right, includeOnlyList: null, typesMustAlsoBeSame: false);
        }

        /// <summary>
        ///     Uses <see cref="MemberCompare(object,object,IEnumerable{string},IEnumerable{string},double,bool)" /> to recursively
        ///     compare the values of
        ///     the specified Members on <paramref name="left" /> and <paramref name="right" />
        ///     and reports the first discrepancy, if any.
        ///     Named Properties (public or not), and public Fields, are considered in the comparison.
        ///     Recursion stops when it arrives at a value type, or at a type (such as <see cref="String" />)
        ///     which override <see cref="object.Equals(object)" />
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <param name="floatTolerance"></param>
        /// <returns>
        ///     <see cref="BoolWithString.True" /> if the Members of <paramref name="left" /> and <paramref name="right" />
        ///     are equal by value, or a
        ///     <see cref="BoolWithString.False" /> bearing a description of the first discrepancy if not.
        /// </returns>
        public static BoolWithString EqualsByValue(this object left, object right, double floatTolerance)
        {
            return MemberCompare(left, right, null, null, floatTolerance, false);
        }

        /// <summary>
        ///     Uses <see cref="MemberCompare(object,object,IEnumerable{string},IEnumerable{string},double,bool)" /> to recursively
        ///     compare the values of
        ///     the specified Members on <paramref name="left" /> and <paramref name="right" />
        ///     and reports the first discrepancy, if any.
        ///     Named Properties (public or not), and public Fields, are considered in the comparison.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <param name="exclusionList">
        ///     a possibly empty list of Member names to exclude for the purposes of this
        ///     comparison. To exclude members of child members, provide the full dotted 'breadcrumb' to the member
        ///     to exclude, e.g. new List&lt;string&gt;{"Id","SomeProperty.SomePropertyOfThat.FieldName"}
        /// </param>
        /// <returns>
        ///     <see cref="BoolWithString.True" /> if the Members of <paramref name="left" /> and <paramref name="right" />
        ///     �excluding those named in <paramref name="exclusionList" />� are equal by value, or a
        ///     <see cref="BoolWithString.False" /> bearing a description of the first discrepancy if not.
        /// </returns>
        public static BoolWithString EqualsByValueExceptFor(
            this object         left,
            object              right,
            IEnumerable<string> exclusionList)
        {
            return MemberCompare(left, right, exclusionList, null, typesMustAlsoBeSame: false);
        }

        /// <summary>
        ///     Uses <see cref="MemberCompare(object,object,IEnumerable{string},IEnumerable{string},double,bool)" /> to recursively
        ///     compare the values of
        ///     the specified Members on <paramref name="left" /> and <paramref name="right" />
        ///     and reports the first discrepancy, if any.
        ///     Named Properties (public or not), and public Fields, are considered in the comparison.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <param name="exclusionList">
        ///     a possibly empty list of Member names to exclude for the purposes of this
        ///     comparison. To exclude members of child members, provide the full dotted 'breadcrumb' to the member
        ///     to exclude, e.g. new List&lt;string&gt;{"Id","SomeProperty.SomePropertyOfThat.FieldName"}
        /// </param>
        /// <param name="floatTolerance"></param>
        /// <returns>
        ///     <see cref="BoolWithString.True" /> if the Members of <paramref name="left" /> and <paramref name="right" />
        ///     �excluding those named in <paramref name="exclusionList" />� are equal by value, or else a
        ///     <see cref="BoolWithString.False" /> which includes a description and the location of the first discrepancy found.
        /// </returns>
        public static BoolWithString EqualsByValueExceptFor(
            this object  left,
            object       right,
            List<string> exclusionList,
            double       floatTolerance)
        {
            return MemberCompare(left, right, exclusionList, null, floatTolerance, false);
        }


        /// <summary>
        ///     Synonym for <see cref="MemberCompare(object,object,IEnumerable{string},IEnumerable{string},double,bool)" />
        ///     Compare two objects by recursively iterating over their elements (if they are Enumerable)
        ///     and over their properties �whether public or private� and over their public fields.
        ///     Recursion stops at value types and at types (including string) which override Equals()
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns>
        ///     A <see cref="BoolWithString" /> which, in case of mismatch, includes a description and the location of the
        ///     first mismatch found.
        /// </returns>
        public static BoolWithString AreEqual(object left, object right)
        {
            return MemberCompare(left, right, includeOnlyList: null, typesMustAlsoBeSame: false);
        }

        /// <summary>
        ///     Synonym for <see cref="MemberCompare(object,object,IEnumerable{string},IEnumerable{string},double,bool)" />
        ///     Compare two objects by recursively iterating over their elements (if they are Enumerable)
        ///     and over their properties �whether public or private� and over their public fields.
        ///     Recursion stops at value types and at types (including string) which override Equals()
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <param name="floatTolerance">The tolerance to apply to floating point equality comparison</param>
        /// <returns>
        ///     A <see cref="BoolWithString" /> which, in case of mismatch, includes a description and the location of the
        ///     first mismatch found
        /// </returns>
        public static BoolWithString AreEqual(object left, object right, double floatTolerance)
        {
            return MemberCompare(left, right, null, null, floatTolerance, false);
        }

        /// <summary>
        ///     Compare two objects by recursively iterating over their elements (if they are Enumerable)
        ///     and over their properties �whether public or private� and over their public fields.
        ///     Recursion stops when it arrives at either a value type, or at a type (such as <see cref="String" />) which override
        ///     <see cref="object.Equals(object)" />
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <param name="exclusionList">
        ///     a possibly empty list of Member names to exclude for the purposes of this
        ///     comparison. To exclude members of child members, provide the full dotted 'breadcrumb' to the member
        ///     to exclude, e.g. new List&lt;string&gt;{"Id","SomeProperty.SomePropertyOfThat.FieldName"}
        /// </param>
        /// <param name="includeOnlyList">
        ///     a possibly empty list of Member names to restrict the comparisons to only those members.
        ///     To specify members of child members, provide the full dotted 'breadcrumb' to the member
        ///     to include, e.g. new List&lt;string&gt;{"Id","SomeProperty.SomePropertyOfThat.FieldName"}
        /// </param>
        /// <param name="floatTolerance">The tolerance to apply to floating point equality comparison</param>
        /// <param name="typesMustAlsoBeSame">
        ///     If <c>true</c> then objects of different types may still compare as equal if their properties are all recursively
        ///     equal by value.
        ///     If false, then <paramref name="left" /> and <paramref name="right" /> must also be of the same <see cref="Type" />
        ///     to be considered equal.
        /// </param>
        /// <returns>
        ///     A <see cref="BoolWithString" /> which, in case of a mismatch, includes a description and the location of the
        ///     first mismatch found
        /// </returns>
        public static BoolWithString MemberCompare(
            object              left,
            object              right,
            IEnumerable<string> exclusionList       = null,
            IEnumerable<string> includeOnlyList     = null,
            double              floatTolerance      = 1e-14d,
            bool                typesMustAlsoBeSame = false)
        {
            var breadCrumb           = new List<string>();
            var exclusionListArray   = (exclusionList ?? new string[0]).ToArray();
            var includeOnlyListArray = includeOnlyList?.ToArray();
            return MemberCompare(left,
                                 right,
                                 new List<object>(),
                                 ref breadCrumb,
                                 exclusionListArray,
                                 includeOnlyListArray,
                                 floatTolerance,
                                 typesMustAlsoBeSame);
        }

        static BoolWithString MemberCompare(
            object           left,
            object           right,
            List<object>     done,
            ref List<string> breadcrumb,
            string[]         exclusionList,
            string[]         includeOnlyList,
            double           floatTolerance,
            bool             typesMustAlsoBeSame)
        {
            bool IsInIncludeList(string breadCrumbAsDottedMember)
            {
                return includeOnlyList == null
                    || includeOnlyList.Any(s => breadCrumbAsDottedMember.StartsWith(s + "."))
                    || includeOnlyList.Contains(breadCrumbAsDottedMember);
            }

            // null checking
            if (left == null         && right == null
             || left == DBNull.Value && right == null
             || left == null         && right == DBNull.Value)
                return true;

            // avoid cyclic references
            if (done.Contains(left) && left != null && !left.GetType().GetTypeInfo().IsValueType) return true;
            done.Add(left);

            // same if references are equal
            if (ReferenceEquals(left, right)) return true;

            if (left == null)
                if (right is IEnumerable)
                {
                    if (((IEnumerable) right).HasAnyElements())
                        return BoolWithString.False("Left is null IEnumerable, Right is non-empty");
                    return true;
                }

            if (right == null)
                if (left is IEnumerable leftasIEnumerable)
                {
                    if (leftasIEnumerable.HasAnyElements())
                        return BoolWithString.False("Left is non-empty IEnumerable, Right is null IEnumerable");
                    return true;
                }
            
            // probably not the same if types are different.
            var leftType  = left.GetType();
            var rightType = right.GetType();

            // avoid StackOverflows
            if (left is FileSystemInfo && right is FileSystemInfo)
            {
                return (left as FileSystemInfo).EqualsOrDiffers(right as FileSystemInfo);
            }
            else if (left is FileSystemInfo || right is FileSystemInfo)
            {
                return BoolWithString.False(
                    string.Format("Left is {0}, Right is {1}", leftType, rightType));
            }

            
            if (left is double || left is float || left is IComparable<double> || left is IComparable<float>)
                try { return ((double) left).EqualsOrDiffers((double) right, floatTolerance); } catch (Exception)
                {
                    /*comparing as doubles failed, so swallow the exception and carry on*/
                }

            /*Gotcha: boxed numbers may fail to equal each other and have to be unboxed correctly to use == */
            if (left is int && right is long)
                try { return (int) left == (long) right; } catch (Exception)
                {
                    /*comparing as long failed, so swallow the exception and carry on*/
                }

            if (left is long && right is int)
                try { return (long) left == (int) right; } catch (Exception)
                {
                    /*comparing as long failed, so swallow the exception and carry on*/
                }

            if (left is int && right is int)
                try { return (int) left == (int) right; } catch (Exception)
                {
                    /*comparing as long failed, so swallow the exception and carry on*/
                }

            if (left is long && right is long)
                try { return (long) left == (long) right; } catch (Exception)
                {
                    /*comparing as long failed, so swallow the exception and carry on*/
                }

            if (left is ValueType
             && !left.GetType().GetTypeInfo().IsGenericType /*ValueType comparison isn't always reliable*/
            ) return left.EqualsOrDiffers(right);

            // check for override, but not the AnonymousType override because it rejects types of the sam structure 
            if (leftType != typeof(object) && !IsAnonymousType(leftType))
            {
                var mI = leftType.GetMethod("Equals", new[] {leftType});
                if (leftType == mI.DeclaringType) return left.EqualsOrDiffers(right);
            }

            // all Arrays, Lists, IEnumerable<> etc implement IEnumerable 
            if (left is IEnumerable)
            {
                if (!(right is IEnumerable))
                    return BoolWithString.False(string.Format("Left is a {0}, Right is a {1}", leftType, rightType));

                var rightEnumerator = ((IEnumerable) right).GetEnumerator();
                var leftEnumerator  = ((IEnumerable) left).GetEnumerator();
                try { rightEnumerator.Reset(); } catch (Exception) { }

                try { leftEnumerator.Reset(); } catch (Exception) { }

                if (!leftEnumerator.MoveNext())
                    if (rightEnumerator.MoveNext())
                        return BoolWithString.False("Left has no elements but right has at least one element");

                rightEnumerator = ((IEnumerable) right).GetEnumerator();
                leftEnumerator  = ((IEnumerable) left).GetEnumerator();
                try { rightEnumerator.Reset(); } catch (Exception) { }

                try { leftEnumerator.Reset(); } catch (Exception) { }

                var lefti = 0;
                foreach (var leftItem in (IEnumerable) left)
                {
                    // unequal amount of items 
                    if (!rightEnumerator.MoveNext())
                        return
                        BoolWithString.False(string.Format("Left has {0} at index {1} but Right is only {2} items long",
                                                           leftItem,
                                                           lefti,
                                                           lefti - 1));
                    var currentCompare = MemberCompare(leftItem,
                                                       rightEnumerator.Current,
                                                       done,
                                                       ref breadcrumb,
                                                       exclusionList,
                                                       null,
                                                       floatTolerance,
                                                       typesMustAlsoBeSame);
                    if (!currentCompare)
                        return BoolWithString.False(string.Format("Mismatch at item {0} ", lefti))
                                             .Because(currentCompare);
                    lefti++;
                }

                return true;
            }

            // compare each property 
            foreach (var leftInfo in leftType.GetProperties(
                                                            BindingFlags.Public
                                                          | BindingFlags.NonPublic
                                                          | BindingFlags.Instance
                                                          | BindingFlags.GetProperty))
            {
                var breadCrumbAsDottedMember = string.Join(".", breadcrumb.Union(new[] {leftInfo.Name}));
                if (exclusionList.Contains(breadCrumbAsDottedMember)) continue;
                var mustInclude = IsInIncludeList(breadCrumbAsDottedMember);
                if (!mustInclude) continue;
                try
                {
                    if (leftInfo.GetGetMethod().GetParameters().Length != 0) continue;

                    var rightInfo = rightType.GetProperty(leftInfo.Name,
                                                          BindingFlags.Public
                                                        | BindingFlags.NonPublic
                                                        | BindingFlags.Instance
                                                        | BindingFlags.GetProperty);
                    try
                    {
                        breadcrumb.Add(rightInfo.Name);
                        var memberCompare = MemberCompare(leftInfo.GetValue(left, null),
                                                          rightInfo.GetValue(right, null),
                                                          done,
                                                          ref breadcrumb,
                                                          exclusionList,
                                                          null,
                                                          floatTolerance,
                                                          typesMustAlsoBeSame);
                        if (!memberCompare)
                            return BoolWithString.False(string.Format("Mismatch at member {0}", leftInfo.Name))
                                                 .Because(memberCompare);
                    }
                    finally { breadcrumb.RemoveAt(breadcrumb.Count - 1); }
                } catch (Exception)
                {
                    return BoolWithString.False(string.Format("Left has property {0} but Right doesn't.",
                                                              leftInfo.Name));
                }
            }

            // and compare each Public field 
            foreach (var leftInfo in leftType.GetFields(
                                                        BindingFlags.Public
                                                      | BindingFlags.Instance
                                                      | BindingFlags.GetField))
            {
                var breadCrumbAsDottedMember = string.Join(".", breadcrumb.Union(new[] {leftInfo.Name}));
                if (exclusionList.Contains(breadCrumbAsDottedMember)) continue;
                var mustInclude = IsInIncludeList(breadCrumbAsDottedMember);
                if (!mustInclude) continue;
                try
                {
                    var rightInfo = rightType.GetField(leftInfo.Name,
                                                       BindingFlags.Public
                                                     | BindingFlags.NonPublic
                                                     | BindingFlags.Instance
                                                     | BindingFlags.GetProperty);
                    try
                    {
                        breadcrumb.Add(rightInfo.Name);
                        var memberCompare = MemberCompare(leftInfo.GetValue(left),
                                                          rightInfo.GetValue(right),
                                                          done,
                                                          ref breadcrumb,
                                                          exclusionList,
                                                          null,
                                                          floatTolerance,
                                                          typesMustAlsoBeSame);
                        if (!memberCompare)
                            return BoolWithString.False(string.Format("Mismatch at member {0}", leftInfo.Name))
                                                 .Because(memberCompare);
                    }
                    finally { breadcrumb.RemoveAt(breadcrumb.Count - 1); }
                } catch (Exception)
                {
                    return BoolWithString.False(string.Format("Left has property {0} but Right doesn't.",
                                                              leftInfo.Name));
                }
            }

            //Then check that there are no properties on RHS that LHS didn't have
            foreach (var rightInfo in rightType.GetProperties(
                                                              BindingFlags.Public
                                                            | BindingFlags.NonPublic
                                                            | BindingFlags.Instance
                                                            | BindingFlags.GetProperty))
            {
                var breadCrumbAsDottedMember = string.Join(".", breadcrumb.Union(new[] {rightInfo.Name}));
                if (exclusionList.Contains(breadCrumbAsDottedMember)) continue;
                var mustInclude = IsInIncludeList(breadCrumbAsDottedMember);
                if (!mustInclude) continue;
                try
                {
                    leftType.GetProperty(rightInfo.Name,
                                         BindingFlags.Public
                                       | BindingFlags.NonPublic
                                       | BindingFlags.Instance
                                       | BindingFlags.GetProperty);
                } catch (Exception)
                {
                    return BoolWithString.False(string.Format("Right has property {0} but Left doesn't.",
                                                              rightInfo.Name));
                }
            }

            //Then check that there are no public fields on RHS that LHS didn't have
            foreach (var rightInfo in rightType.GetFields(
                                                          BindingFlags.Public | BindingFlags.Instance))
            {
                var breadCrumbAsDottedMember = string.Join(".", breadcrumb.Union(new[] {rightInfo.Name}));
                if (exclusionList.Contains(breadCrumbAsDottedMember)) continue;
                var mustInclude = IsInIncludeList(breadCrumbAsDottedMember);
                if (!mustInclude) continue;
                try
                {
                    leftType.GetField(rightInfo.Name,
                                      BindingFlags.Public
                                    | BindingFlags.NonPublic
                                    | BindingFlags.Instance
                                    | BindingFlags.GetProperty);
                } catch (Exception)
                {
                    return BoolWithString.False(string.Format("Right has property {0} but Left doesn't.",
                                                              rightInfo.Name));
                }
            }

            if (IsAnonymousType(leftType) && IsAnonymousType(rightType)) return true;
            if (leftType == rightType || !typesMustAlsoBeSame) return true;
            return BoolWithString.False(string.Format("Left is a {0}, Right is a {1}", leftType, rightType));
        }

        internal static bool IsAnonymousType(Type type)
        {
            if (type == null) throw new ArgumentNullException("type");

            return
            type.GetTypeInfo().IsGenericType
         && (type.Name.Contains("AnonymousType") || /* for Mono: */ type.Name.Contains("AnonType"))
         && (type.Name.StartsWith("<>")          || type.Name.StartsWith("VB$"))
         && (type.GetTypeInfo().Attributes & TypeAttributes.NotPublic) == TypeAttributes.NotPublic;
        }

        internal static bool HasAnyElements(this IEnumerable enumerable)
        {
            return enumerable.GetEnumerator().MoveNext();
        }

        internal static BoolWithString EqualsOrDiffers(this double left, double right, double tolerance)
        {
            var equals = left > right - tolerance && left < right + tolerance;
            return equals
                   ? (BoolWithString) true
                   : BoolWithString.False(
                                          string.Format("Values are different {0} vs {1} (with tolerance {2})",
                                                        left,
                                                        right,
                                                        tolerance)
                                         );
        }

        internal static BoolWithString EqualsOrDiffers(this object left, object right)
        {
            return left.Equals(right)
                   ? (BoolWithString) true
                   : BoolWithString.False(
                                          string.Format("Values are different {0} vs {1}", left, right)
                                         );
        }

        internal static BoolWithString EqualsOrDiffers(this string left, string right)
        {
            return left.Equals(right)
                   ? (BoolWithString) true
                   : BoolWithString.False(
                                          string.Format("Values are different \"{0}\" vs \"{1}\"", left, right)
                                         );
        }
        
        internal static BoolWithString EqualsOrDiffers<T>(this T left, T right) where T : FileSystemInfo
        {
            return left?.FullName==right?.FullName
                ? (BoolWithString) true
                : BoolWithString.False(
                    string.Format("{0} Paths differ \"{1}\" vs \"{2}\"", 
                        left.GetType().FullName,  left?.FullName, right?.FullName)
                );
        }
    }
}
