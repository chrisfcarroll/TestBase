using System;
using System.Runtime.InteropServices;
using NUnit.Framework;

namespace TestBase.Shoulds
{
    public static class NumericShouldsWithTolerance
    {
        /// <summary>Accepts 2 <see cref="double"/>s as equal if they are within <paramref name="tolerance"/> of each other.</summary>
        public static T ShouldEqualWithTolerance<T>(this T @this, double expectedValue, [Optional,DefaultParameterValue(1e-14d)] double tolerance, [Optional] string message, params object[] args)
        {
            if (tolerance < 0d) { tolerance = -tolerance; }
            if (expectedValue < 0d) { tolerance = -tolerance;}
            NUnit.Framework.Assert.That(@this, Is.InRange(expectedValue -tolerance, expectedValue+tolerance), message, args);
            return @this;
        }

        /// <summary>Accepts 2 <see cref="double"/>s as not equal only if they differ by at least <paramref name="tolerance"/>.</summary>
        public static T ShouldNotEqualWithMargin<T>(this T @this, double expectedValue, [Optional, DefaultParameterValue(1e-14d)] double minimumDifference, [Optional] string message, params object[] args)
        {
            NUnit.Framework.Assert.That(@this, Is.Not.InRange(expectedValue - minimumDifference, expectedValue + minimumDifference), message, args);
            return @this;
        }


        /// <summary>Assert that <paramref name="@this"/> is between <paramref name="left"/>-<paramref name="tolerance"/> and <paramref name="right"/>+<paramref name="tolerance"/></summary>
        public static T ShouldBeBetweenWithTolerance<T>(this T @this, double left, double right, [Optional, DefaultParameterValue(1e-14d)] double tolerance, [Optional] string message, params object[] args)
                        where T : IComparable<T>
        {
            NUnit.Framework.Assert.That(@this, Is.InRange(left - tolerance, right + tolerance), message, args);
            return @this;
        }

        /// <summary>Assert that <paramref name="@this"/> is greater than <paramref name="expectedValue"/>-<paramref name="tolerance"/></summary>
        public static T ShouldBeGreaterThanWithTolerance<T>(this T @this, double expectedValue, [Optional, DefaultParameterValue(1e-14d)] double tolerance, [Optional] string message, params object[] args)
        {
            NUnit.Framework.Assert.That( @this, Is.GreaterThan( expectedValue-tolerance ), message, args );
            return @this;
        }

        /// <summary>Assert that <paramref name="@this"/> is greater than or equal to <paramref name="expectedValue"/>-<paramref name="tolerance"/></summary>
        public static T ShouldBeGreaterThanOrEqualToWithTolerance<T>(this T @this, double expectedValue, [Optional, DefaultParameterValue(1e-14d)] double tolerance, [Optional] string message, params object[] args)
        {
            NUnit.Framework.Assert.That( @this, Is.GreaterThanOrEqualTo( expectedValue-tolerance ), message, args );
            return @this;
        }

        /// <summary>Assert that <paramref name="@this"/> is less than <paramref name="expectedValue"/>+<paramref name="tolerance"/></summary>
        public static T ShouldBeLessThanWithTolerance<T>(this T @this, double expectedValue, [Optional, DefaultParameterValue(1e-14d)] double tolerance, [Optional] string message, params object[] args)
        {
            NUnit.Framework.Assert.That(@this, Is.LessThanOrEqualTo(expectedValue+tolerance), message, args);
            return @this;
        }

        /// <summary>Assert that <paramref name="@this"/> is less than or equal to <paramref name="expectedValue"/>+<paramref name="tolerance"/></summary>
        public static T ShouldBeLessThanOrEqualToWithTolerance<T>(this T @this, double expectedValue, [Optional, DefaultParameterValue(1e-14d)] double tolerance, [Optional] string message, params object[] args)
        {
            NUnit.Framework.Assert.That(@this, Is.LessThanOrEqualTo(expectedValue + tolerance), message, args);
            return @this;
        }
    }
}
