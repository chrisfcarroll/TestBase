using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
//Credits. ToDo. Credit the blogger who wrote & posted v1 of MemberCompare() in about 2009

namespace TestBase
{
    /// <summary>
    /// Compares objects memberwise by value
    /// </summary>
    public static class Comparer
    {
        public static BoolWithString EqualsByValueOrDiffers(this object left, object right)
        {
            return MemberCompare(left, right, new List<object>());
        }

        public static bool EqualsByValue(this object left, object right)
        {
            return MemberCompare(left, right, new List<object>());
        }

        public static bool AreEqual(object left, object right)
        {
            return MemberCompare(left, right, new List<object>());
        }

        /// <summary>
        /// Check to see if 2 given objects are same by comparing their properties.
        /// Following properties are ignored:-
        ///   Version
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <param name="done"></param>
        /// <returns></returns>
        public static BoolWithString MemberCompare(object left, object right, List<object> done)
        {
            // avoid cyclic references
            if (done.Contains(left))
            {
                return true;
            }
            done.Add(left);

            // same if references are equal
            if (ReferenceEquals(left, right))
            {
                return true;
            }

            // null checking
            if (left == null && right == null) return true;

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
                if (left is IEnumerable)
                {
                    if (((IEnumerable)left).HasAnyElements())
                    {
                        return BoolWithString.False("Left is non-empty IEnumerable, Right is null IEnumerable");
                    }
                    return true;
                }
            }

            // not the same if types are different.
            Type leftType = left.GetType();
            Type rightType = right.GetType();

            if (left is ValueType)
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
                    var currentCompare = MemberCompare(leftItem, rightEnumerator.Current, done);
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
                if (leftInfo.Name == "Version")
                {
                    // Ignore the version property as Nhibernate can increment version as it needs to when persisting objects.
                    continue;
                }
                try
                {
                    var rightInfo = rightType.GetProperty(leftInfo.Name,
                                                          BindingFlags.Public | BindingFlags.NonPublic |
                                                          BindingFlags.Instance | BindingFlags.GetProperty);
                    var memberCompare = MemberCompare(leftInfo.GetValue(left, null), rightInfo.GetValue(right, null), done);
                    if (!memberCompare)
                    {
                        return BoolWithString.False(string.Format("Mismatch at member {0}", leftInfo.Name)).Because(memberCompare);
                    }
                }
                catch(Exception e)
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
                try
                {
                    var leftInfo = leftType.GetProperty(rightInfo.Name,
                                                          BindingFlags.Public | BindingFlags.NonPublic |
                                                          BindingFlags.Instance | BindingFlags.GetProperty);
                }
                catch (Exception e)
                {
                    return BoolWithString.False(string.Format("Right has property {0} but Left doesn't.", rightInfo.Name));
                }
            }



            if(IsAnonymousType(leftType) && IsAnonymousType(rightType))
            {
                return true;
            }

            if (leftType == rightType)
            {
                return true;
            }

            return BoolWithString.False(string.Format("Left is a {0}, Right is a {1}", leftType, rightType));
        }

        internal static bool IsAnonymousType(Type type)
        {
            if (type == null) throw new ArgumentNullException("type");

            return Attribute.IsDefined(type, typeof(CompilerGeneratedAttribute), false)
                && type.IsGenericType && type.Name.Contains("AnonymousType")
                && (type.Name.StartsWith("<>") || type.Name.StartsWith("VB$"))
                && (type.Attributes & TypeAttributes.NotPublic) == TypeAttributes.NotPublic;
        }

        internal static bool HasAnyElements(this IEnumerable enumerable)
        {
            return enumerable.GetEnumerator().MoveNext();
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
