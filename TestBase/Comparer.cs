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
using System.Linq;
using System.Reflection;

namespace TestBase
{
    /// <summary>
    /// Compares objects memberwise by value. All Properties (public or not), and public Fields, are considered in the comparison.
    /// </summary>
    public static class Comparer
    {
        /// <remarks>An abbreviation for <see cref="EqualsByValueJustOnCommonPublicReadableProperties"/></remarks>
        /// <param name="left"></param><param name="right"></param>
        /// <returns>
        /// True, if the readable properties of <paramref name="left"/> &amp; <paramref name="right"/> with identical names are all <see cref="EqualsByValue(object,object)"/>
        /// <see cref="BoolWithString.False"/> if not, with a description of the first mismatch found in <see cref="BoolWithString.ToString"/>()
        /// </returns>
        public static BoolWithString PropertiesMatch(this object left, object right)
        {
            return EqualsByValueJustOnCommonPropertiesSatisfying(left, right, p => p.CanRead);
        }

        ///<remarks>Can be abbreviated to <see cref="PropertiesMatch"/></remarks>
        /// <param name="left"></param><param name="right"></param>
        /// <returns>
        /// True, if the readable properties of <paramref name="left"/> &amp; <paramref name="right"/> with identical names are all <see cref="EqualsByValue(object,object)"/>
        /// <see cref="BoolWithString.False"/> if not, with a description of the first mismatch found in <see cref="BoolWithString.ToString"/>()
        /// </returns>
        public static BoolWithString EqualsByValueJustOnCommonPublicReadableProperties(this object left, object right)
        {
            return EqualsByValueJustOnCommonPropertiesSatisfying(left, right, p=>p.CanRead);
        }

        /// <param name="left"></param><param name="right"></param>
        /// <returns>
        /// True, if the writeable properties of <paramref name="left"/> &amp; <paramref name="right"/> with identical names are all <see cref="EqualsByValue(object,object)"/>
        /// <see cref="BoolWithString.False"/> if not, with a description of the first mismatch found in <see cref="BoolWithString.ToString"/>()
        /// </returns>
        /// <remarks>Writeable properties are often a subset of readable properties. Often, if an object is deserialized from a database or datastore, then it is expected to be fully specified by its writeable properties.</remarks>
        public static BoolWithString EqualsByValueJustOnCommonPublicWriteableProperties(this object left, object right)
        {
            return EqualsByValueJustOnCommonPropertiesSatisfying(left, right, p => p.CanWrite);
        }

        /// <param name="left"></param><param name="right"></param>
        /// <returns>
        /// True, if the properties of <paramref name="left"/> &amp; <paramref name="right"/> with identical names are all <see cref="EqualsByValue(object,object)"/>
        /// <see cref="BoolWithString.False"/> if not, with a description of the first mismatch found in <see cref="BoolWithString.ToString"/>()
        /// </returns>
        static BoolWithString EqualsByValueJustOnCommonPropertiesSatisfying(this object left, object right, Func<PropertyInfo,bool> predicate)
        {
            var leftProperties = left.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance).Where(predicate);
            var rightProperties = right.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance).Where(predicate);
            var commonProperties = leftProperties.Join(rightProperties, l => $"{l.Name}-{l.PropertyType.FullName}", r => $"{r.Name}-{r.PropertyType.FullName}", (l, r) => l.Name);
            return MemberCompare(left, right, exclusionList: null, includeOnlyList: commonProperties, typesMustAlsoBeSame: false);
        }

        public static BoolWithString EqualsByValueJustOnPropertiesNamed(this object left, object right, IEnumerable<string> propertiesToCompare)
        {
            return MemberCompare(left, right, exclusionList: null, includeOnlyList: propertiesToCompare, typesMustAlsoBeSame: false);
        }

        public static BoolWithString EqualsByValueOrDiffers(this object left, object right, double floatTolerance=1e-14d)
        {
            return MemberCompare(left, right, exclusionList: null, includeOnlyList: null, floatTolerance: floatTolerance, typesMustAlsoBeSame: false);
        }

        public static BoolWithString EqualsByValueOrDiffersExceptFor(this object left, object right, IEnumerable<string> exclusionList, double floatTolerance = 1e-14d)
        {
            return MemberCompare(left, right, exclusionList: exclusionList, includeOnlyList: null, floatTolerance: floatTolerance, typesMustAlsoBeSame: false);
        }

        public static BoolWithString EqualsByValue(this object left, object right)
        {
            return MemberCompare(left, right, includeOnlyList: null, typesMustAlsoBeSame: false);
        }
        public static BoolWithString EqualsByValue(this object left, object right, double floatTolerance)
        {
            return MemberCompare(left, right, exclusionList: null,includeOnlyList: null, floatTolerance: floatTolerance, typesMustAlsoBeSame: false);
        }

        public static BoolWithString EqualsByValueExceptFor(this object left, object right, IEnumerable<string> exclusionList)
        {
            return MemberCompare(left, right, exclusionList: exclusionList, includeOnlyList: null, typesMustAlsoBeSame: false);
        }

        public static BoolWithString EqualsByValueExceptFor(this object left, object right, List<string> exclusionList, double floatTolerance)
        {
            return MemberCompare(left, right, exclusionList: exclusionList, includeOnlyList: null, floatTolerance: floatTolerance, typesMustAlsoBeSame: false);
        }

        public static BoolWithString AreEqual(object left, object right)
        {
            return MemberCompare(left, right, includeOnlyList: null, typesMustAlsoBeSame: false);
        }
        public static BoolWithString AreEqual(object left, object right, double floatTolerance)
        {
            return MemberCompare(left, right, exclusionList: null, includeOnlyList: null, floatTolerance: floatTolerance, typesMustAlsoBeSame: false);
        }

        /// <summary>
        /// Compare two objects by recursively iterating over their elements (if they are Enumerable) 
        /// and properties (public or private) and public fields.
        /// Recursion stops at value types and at types (including string) which override Equals()
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <param name="exclusionList">a possibly empty list of field names to exclude for the purposes of this 
        ///     comparison. To exclude fields of fields, provide the full dotted 'breadcrumb' to the property 
        ///     to exclude, e.g. new List&lt;string&gt;{"Id","SomeProperty.SomePropertyOfThat.FieldName"} 
        /// </param>
        /// <param name="includeOnlyList"></param>
        /// <param name="floatTolerance">The tolerance to apply to floating point equality comparison</param>
        /// <param name="typesMustAlsoBeSame"></param>
        /// <returns>a <see cref="BoolWithString"/>. In case of failure, the reason for failure is returned.</returns>
        public static BoolWithString MemberCompare(object left, object right, IEnumerable<string> exclusionList = null, IEnumerable<string> includeOnlyList = null, double floatTolerance = 1e-14d, bool typesMustAlsoBeSame=false)
        {
            var breadCrumb = new List<string>();
            return MemberCompare(left, right, new List<object>(), ref breadCrumb, exclusionList ?? new string[0], includeOnlyList, floatTolerance, typesMustAlsoBeSame);
        }

        static BoolWithString MemberCompare(object left, object right, List<object> done, ref List<string> breadcrumb, IEnumerable<string> exclusionList, IEnumerable<string> includeOnlyList, double floatTolerance, bool typesMustAlsoBeSame)
        {
            // null checking
            if (left == null && right == null
                || left == DBNull.Value && right == null
                || left == null && right == DBNull.Value)
            {
                return true;
            }

            // avoid cyclic references
            if (done.Contains(left) && left!=null && !left.GetType().GetTypeInfo().IsValueType)
            {
                return true;
            }
            done.Add(left);

            // same if references are equal
            if (ReferenceEquals(left, right))
            {
                return true;
            }

            if (left == null)
            {
                // allow null (not ininitialized) collection to be same as an empty collection
                if (right is IEnumerable)
                {
                    if ( ((IEnumerable)right).HasAnyElements() )
                    {
                        return BoolWithString.False("Left is null IEnumerable, Right is non-empty");
                    }
                    return true;
                }
            }

            if (right == null)
            {
                // allow null (not ininitialized) collection to be same as an empty collection
                if (left is IEnumerable leftasIEnumerable)
                {
                    if (leftasIEnumerable.HasAnyElements())
                    {
                        return BoolWithString.False("Left is non-empty IEnumerable, Right is null IEnumerable");
                    }
                    return true;
                }
            }

            // not the same if types are different.
            Type leftType = left.GetType();
            Type rightType = right.GetType();

            if (left is double || left is float || left is IComparable<double> || left is IComparable<float>)
            {
                try
                {
                    return ((double)left).EqualsOrDiffers( (double)right , floatTolerance);
                }
                catch (Exception) { /*comparing as doubles failed, so swallow the exception and carry on*/ }
            }

            /*Gotcha: boxed numbers may fail to equal each other and have to be unboxed correctly to use == */
            if (left is Int32 && right is Int64)
            {
                try { return (int)left == (long)right; }
                catch (Exception) { /*comparing as long failed, so swallow the exception and carry on*/ }
            }
            if (left is Int64 && right is Int32)
            {
                try { return (long)left == (int)right; }
                catch (Exception) { /*comparing as long failed, so swallow the exception and carry on*/ }
            }
            if ( left is Int32 && right is Int32 )
            {
                try{ return (int)left== (int)right; }
                catch (Exception) { /*comparing as long failed, so swallow the exception and carry on*/ }
            }
            if (left is Int64 && right is Int64)
            {
                try { return (long)left == (long)right; }
                catch (Exception) { /*comparing as long failed, so swallow the exception and carry on*/ }
            }
            if (left is ValueType && !left.GetType().IsGenericType /*ValueType comparison isn't always reliable*/)
            {
                // do a field comparison, or use the override if Equals is implemented: 
                return left.EqualsOrDiffers(right);
            }

            // check for override, but not the AnonymousType override because it rejects types of the sam structure 
            if (leftType != typeof(object) && !IsAnonymousType(leftType))
            {
                MethodInfo mI = leftType.GetMethod("Equals", new[] { leftType });
                if (leftType == mI.DeclaringType)
                {
                    // the Equals method is overridden, use it: 
                    return left.EqualsOrDiffers(right);
                }
            }

            // all Arrays, Lists, IEnumerable<> etc implement IEnumerable 
            if (left is IEnumerable)
            {
                if (!(right is IEnumerable))
                {
                    return BoolWithString.False(string.Format("Left is a {0}, Right is a {1}", leftType, rightType));
                }

                IEnumerator rightEnumerator = ((IEnumerable)right).GetEnumerator();
                IEnumerator leftEnumerator = ((IEnumerable)left).GetEnumerator();
                try { rightEnumerator.Reset(); } catch(Exception){}
                try { leftEnumerator.Reset(); } catch (Exception){}

                if (!leftEnumerator.MoveNext())
                {
                    if (rightEnumerator.MoveNext())
                    {
                        return BoolWithString.False("Left has no elements but right has at least one element");
                    }
                }

                rightEnumerator = ((IEnumerable)right).GetEnumerator();
                leftEnumerator = ((IEnumerable)left).GetEnumerator();
                try { rightEnumerator.Reset(); } catch(Exception){}
                try { leftEnumerator.Reset(); } catch (Exception){}

                int lefti = 0;
                foreach (object leftItem in (IEnumerable)left)
                {
                    // unequal amount of items 
                    if (!rightEnumerator.MoveNext())
                    {
                        return BoolWithString.False(string.Format("Left has {0} at index {1} but Right is only {2} items long", leftItem, lefti, lefti - 1));
                    }
                    var currentCompare = MemberCompare(leftItem, rightEnumerator.Current, done, ref breadcrumb, exclusionList, null, floatTolerance, typesMustAlsoBeSame);
                    if (!currentCompare)
                    {
                        return BoolWithString.False(string.Format("Mismatch at item {0} ", lefti)).Because(currentCompare);
                    }
                    lefti++;
                }

                return true;
            }

            // compare each property 
            foreach (PropertyInfo leftInfo in leftType.GetProperties(
                BindingFlags.Public |
                BindingFlags.NonPublic |
                BindingFlags.Instance |
                BindingFlags.GetProperty))
            {
                var breadCrumbAsDottedMember = string.Join(".", breadcrumb.Union(new[]{leftInfo.Name}));
                if (exclusionList.Contains(breadCrumbAsDottedMember)){ continue; }
                var mustInclude = includeOnlyList==null || includeOnlyList.Contains(breadCrumbAsDottedMember);
                if (!mustInclude) { continue; }
                try
                {
                    if(leftInfo.GetGetMethod().GetParameters().Length != 0) { continue;} /*skip indexer properties which require arguments*/

                    var rightInfo = rightType.GetProperty(leftInfo.Name,
                                                          BindingFlags.Public | BindingFlags.NonPublic |
                                                          BindingFlags.Instance | BindingFlags.GetProperty);
                    try
                    {
                        breadcrumb.Add( rightInfo.Name );
                        var memberCompare = MemberCompare(leftInfo.GetValue(left, null), rightInfo.GetValue(right, null), done, ref breadcrumb, exclusionList, null, floatTolerance, typesMustAlsoBeSame);
                        if (!memberCompare)
                        {
                            return BoolWithString.False(string.Format("Mismatch at member {0}", leftInfo.Name)).Because(memberCompare);
                        }
                    }
                    finally
                    {
                        breadcrumb.RemoveAt(breadcrumb.Count-1);
                    }
                }
                catch(Exception)
                {
                    return BoolWithString.False(string.Format("Left has property {0} but Right doesn't.", leftInfo.Name));
                }
            }

            // and compare each Public field 
            foreach (FieldInfo leftInfo in leftType.GetFields(
                BindingFlags.Public |
                BindingFlags.Instance |
                BindingFlags.GetField))
            {
                var breadCrumbAsDottedMember = string.Join(".", breadcrumb.Union(new[] { leftInfo.Name }));
                if (exclusionList.Contains(breadCrumbAsDottedMember)){ continue; }
                var mustInclude = includeOnlyList == null || includeOnlyList.Contains(breadCrumbAsDottedMember);
                if (!mustInclude) { continue; }
                try
                {
                    var rightInfo = rightType.GetField(leftInfo.Name,
                                                    BindingFlags.Public | BindingFlags.NonPublic |
                                                    BindingFlags.Instance | BindingFlags.GetProperty);
                    try
                    {
                        breadcrumb.Add(rightInfo.Name);
                        var memberCompare = MemberCompare(leftInfo.GetValue(left), rightInfo.GetValue(right), done, ref breadcrumb, exclusionList, null, floatTolerance, typesMustAlsoBeSame);
                        if (!memberCompare)
                        {
                            return BoolWithString.False(string.Format("Mismatch at member {0}", leftInfo.Name)).Because(memberCompare);
                        }
                    }
                    finally
                    {
                        breadcrumb.RemoveAt(breadcrumb.Count - 1);
                    }
                }
                catch (Exception)
                {
                    return BoolWithString.False(string.Format("Left has property {0} but Right doesn't.", leftInfo.Name));
                }
            }

            //Then check that there are no properties on RHS that LHS didn't have
            foreach (PropertyInfo rightInfo in rightType.GetProperties(
                BindingFlags.Public |
                BindingFlags.NonPublic |
                BindingFlags.Instance |
                BindingFlags.GetProperty))
            {
                var breadCrumbAsDottedMember = string.Join(".", breadcrumb.Union(new[] { rightInfo.Name }));
                if (exclusionList.Contains(breadCrumbAsDottedMember)){ continue; }
                var mustInclude = includeOnlyList == null || includeOnlyList.Contains(breadCrumbAsDottedMember);
                if (!mustInclude) { continue; }
                try
                {
                    leftType.GetProperty(rightInfo.Name,
                                        BindingFlags.Public | BindingFlags.NonPublic |
                                        BindingFlags.Instance | BindingFlags.GetProperty);
                }
                catch (Exception)
                {
                    return BoolWithString.False(string.Format("Right has property {0} but Left doesn't.", rightInfo.Name));
                }
            }
            //Then check that there are no public fields on RHS that LHS didn't have
            foreach (var rightInfo in rightType.GetFields(
                BindingFlags.Public |
                BindingFlags.Instance))
            {
                var breadCrumbAsDottedMember = string.Join(".", breadcrumb.Union(new[] { rightInfo.Name }));
                if (exclusionList.Contains(breadCrumbAsDottedMember)) { continue; }
                var mustInclude = includeOnlyList == null || includeOnlyList.Contains(breadCrumbAsDottedMember);
                if (!mustInclude) { continue; }
                try
                {
                    leftType.GetField(rightInfo.Name,
                                    BindingFlags.Public | BindingFlags.NonPublic |
                                    BindingFlags.Instance | BindingFlags.GetProperty);
                }
                catch (Exception)
                {
                    return BoolWithString.False(string.Format("Right has property {0} but Left doesn't.", rightInfo.Name));
                }
            }

            if (IsAnonymousType(leftType) && IsAnonymousType(rightType))
            {
                return true;
            }
            if (leftType == rightType || !typesMustAlsoBeSame)
            {
                return true;
            }
            return BoolWithString.False(string.Format("Left is a {0}, Right is a {1}", leftType, rightType));
        }

        internal static bool IsAnonymousType(Type type)
        {
            if (type == null) throw new ArgumentNullException("type");

            return 
                type.GetTypeInfo().IsGenericType 
                && (type.Name.Contains("AnonymousType") || /* for Mono: */ type.Name.Contains("AnonType"))
                && (type.Name.StartsWith("<>") || type.Name.StartsWith("VB$"))
                && (type.GetTypeInfo().Attributes & TypeAttributes.NotPublic) == TypeAttributes.NotPublic;
        }

        internal static bool HasAnyElements(this IEnumerable enumerable)
        {
            return enumerable.GetEnumerator().MoveNext();
        }

        internal static BoolWithString EqualsOrDiffers(this double left, double right, double tolerance)
        {
            var @equals = left > right - tolerance && left < right + tolerance;
            return @equals
                       ? (BoolWithString)true
                       : BoolWithString.False(
                                    string.Format("Values are different {0} vs {1} (with tolerance {2})", left, right, tolerance)
                                    );
        }

        internal static BoolWithString EqualsOrDiffers(this object left, object right)
        {
            return left.Equals(right)
                       ? (BoolWithString)true
                       : BoolWithString.False(
                                    string.Format("Values are different {0} vs {1}", left, right)
                                    );
        }

        internal static BoolWithString EqualsOrDiffers(this string left, string right)
        {
            return left.Equals(right)
                       ? (BoolWithString)true
                       : BoolWithString.False(
                                    string.Format("Values are different \"{0}\" vs \"{1}\"", left, right)
                                    );
        }
    }
}
