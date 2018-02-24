// ***********************************************************************
// Copyright (c) 2008 Charlie Poole, Rob Prouse
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
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
// ***********************************************************************

using System;

namespace TestBase
{
    /// <summary>
    /// The Numerics class contains common operations on numeric values.
    /// </summary>
    public static class Numerics
    {
        #region Numeric Type Recognition
        /// <summary>
        /// Checks the type of the object, returning true if
        /// the object is a numeric type.
        /// </summary>
        /// <param name="obj">The object to check</param>
        /// <returns>true if the object is a numeric type</returns>
        public static bool IsNumericType(Object obj)
        {
            return IsFloatingPointNumeric(obj) || IsFixedPointNumeric(obj);
        }

        /// <summary>
        /// Checks the type of the object, returning true if
        /// the object is a floating point numeric type.
        /// </summary>
        /// <param name="obj">The object to check</param>
        /// <returns>true if the object is a floating point numeric type</returns>
        public static bool IsFloatingPointNumeric(Object obj)
        {
            if (null != obj)
            {
                if (obj is System.Double) return true;
                if (obj is System.Single) return true;
            }
            return false;
        }
        /// <summary>
        /// Checks the type of the object, returning true if
        /// the object is a fixed point numeric type.
        /// </summary>
        /// <param name="obj">The object to check</param>
        /// <returns>true if the object is a fixed point numeric type</returns>
        public static bool IsFixedPointNumeric(Object obj)
        {
            if (null != obj)
            {
                if (obj is System.Byte) return true;
                if (obj is System.SByte) return true;
                if (obj is System.Decimal) return true;
                if (obj is System.Int32) return true;
                if (obj is System.UInt32) return true;
                if (obj is System.Int64) return true;
                if (obj is System.UInt64) return true;
                if (obj is System.Int16) return true;
                if (obj is System.UInt16) return true;
                if (obj is System.Char) return true;
            }
            return false;
        }
        #endregion

        #region Numeric Equality
        /// <summary>
        /// Test two numeric values for equality, performing the usual numeric 
        /// conversions and using a provided or default tolerance. If the tolerance 
        /// provided is Empty, this method may set it to a default tolerance.
        /// </summary>
        /// <param name="expected">The expected value</param>
        /// <param name="actual">The actual value</param>
        /// <param name="tolerance">A reference to the tolerance in effect</param>
        /// <returns>True if the values are equal</returns>
        public static bool AreEqual(object expected, object actual, double tolerance=1e-15)
        {
            if (expected is double || actual is double)
                return AreEqual(Convert.ToDouble(expected), Convert.ToDouble(actual), tolerance);

            if (expected is float || actual is float)
                return AreEqual(Convert.ToSingle(expected), Convert.ToSingle(actual), tolerance);

            if (expected is decimal || actual is decimal)
                return AreEqual(Convert.ToDecimal(expected), Convert.ToDecimal(actual), tolerance);

            if (expected is ulong || actual is ulong)
                return AreEqual(Convert.ToUInt64(expected), Convert.ToUInt64(actual), tolerance);

            if (expected is long || actual is long)
                return AreEqual(Convert.ToInt64(expected), Convert.ToInt64(actual), tolerance);

            if (expected is uint || actual is uint)
                return AreEqual(Convert.ToUInt32(expected), Convert.ToUInt32(actual), tolerance);

            return AreEqual(Convert.ToInt32(expected), Convert.ToInt32(actual), tolerance);
        }

        private static bool AreEqual(double expected, double actual, double tolerance=1e-15)
        {
            if (double.IsNaN(expected) && double.IsNaN(actual))return true;

            // Handle infinity specially since subtracting two infinite values gives 
            // NaN and the following test fails. mono also needs NaN to be handled
            // specially although ms.net could use either method. Also, handle
            // situation where no tolerance is used.
            if (double.IsInfinity(expected) || double.IsNaN(expected) || double.IsNaN(actual))
            {
                return expected.Equals(actual);
            }

            return Math.Abs(expected - actual) <= tolerance;
        }

        private static bool AreEqual(float expected, float actual, double tolerance= 1e-15)
        {
            if (float.IsNaN(expected) && float.IsNaN(actual))return true;

            // handle infinity specially since subtracting two infinite values gives 
            // NaN and the following test fails. mono also needs NaN to be handled
            // specially although ms.net could use either method.
            if (float.IsInfinity(expected) || float.IsNaN(expected) || float.IsNaN(actual))
            {
                return expected.Equals(actual);
            }

            return Math.Abs(expected - actual) <= tolerance;
        }


        private static bool AreEqual(decimal expected, decimal actual, double tolerance=1e-15)
        {
            var decimalTolerance = Convert.ToDecimal(tolerance);
            if (decimalTolerance > 0m) return Math.Abs(expected - actual) <= decimalTolerance;
            return expected == actual;
        }

        private static bool AreEqual(ulong expected, ulong actual, double tolerance=0)
        {
            ulong ulongTolerance = Convert.ToUInt64(tolerance);
            if (ulongTolerance > 0ul)
            {
                ulong diff = expected >= actual ? expected - actual : actual - expected;
                return diff <= ulongTolerance;
            }
            return expected==actual;
        }

        private static bool AreEqual(long expected, long actual, double tolerance=0)
        {
            var longTolerance = Convert.ToInt64(tolerance);
            if (longTolerance > 0L) return Math.Abs(expected - actual) <= longTolerance;

            return expected.Equals(actual);
        }

        private static bool AreEqual(uint expected, uint actual, double tolerance=0)
        {
            uint uintTolerance = Convert.ToUInt32(tolerance);
            if (uintTolerance > 0)
            {
                uint diff = expected >= actual ? expected - actual : actual - expected;
                return diff <= uintTolerance;
            }

            return expected.Equals(actual);
        }

        private static bool AreEqual(int expected, int actual, double tolerance=0)
        {
            int intTolerance = Convert.ToInt32(tolerance);
            if (intTolerance > 0)return Math.Abs(expected - actual) <= intTolerance;

            return expected.Equals(actual);
        }
        #endregion

        #region Numeric Comparisons

        /// <summary>
        /// Compare two numeric values, performing the usual numeric conversions.
        /// </summary>
        /// <param name="expected">The expected value</param>
        /// <param name="actual">The actual value</param>
        /// <returns>The relationship of the values to each other</returns>
        public static int Compare(object expected, object actual)
        {
            if (!IsNumericType(expected) || !IsNumericType(actual))
                throw new ArgumentException("Both arguments must be numeric");

            if (IsFloatingPointNumeric(expected) || IsFloatingPointNumeric(actual))
                return Convert.ToDouble(expected).CompareTo(Convert.ToDouble(actual));

            if (expected is decimal || actual is decimal)
                return Convert.ToDecimal(expected).CompareTo(Convert.ToDecimal(actual));

            if (expected is ulong || actual is ulong)
                return Convert.ToUInt64(expected).CompareTo(Convert.ToUInt64(actual));

            if (expected is long || actual is long)
                return Convert.ToInt64(expected).CompareTo(Convert.ToInt64(actual));

            if (expected is uint || actual is uint)
                return Convert.ToUInt32(expected).CompareTo(Convert.ToUInt32(actual));

            return Convert.ToInt32(expected).CompareTo(Convert.ToInt32(actual));
        }
        #endregion
    }
}