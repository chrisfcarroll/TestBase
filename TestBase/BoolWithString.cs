// Author: Chris F Carroll
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
namespace TestBase
{
    /// <summary>
    /// A  with an explanation. <see cref="BoolWithString"/> is convertible to and from <see cref="bool"/>
    /// </summary>
    public struct BoolWithString
    {
        /// <summary>True if both value and <see cref="ToString"/> match.</summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(BoolWithString other)
        {
            return value==other.value && message==other.message;
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            unchecked
            {
                return (value.GetHashCode()*397) ^ (message != null ? message.GetHashCode() : 0);
            }
        }

        readonly bool value;
        readonly string message;

        /// <summary>Create a <see cref="BoolWithString"/></summary>
        public BoolWithString(bool value, string message)
        {
            this.value = value;
            this.message = message;
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return string.IsNullOrEmpty(message) 
                ? value.ToString() 
                : value + " : " + message;
        }

        public bool AsBool => value;

        /// <summary>Return the inner <see cref="bool"/> value of <paramref name="value"/></summary>
        /// <param name="value"></param>
        public static implicit operator bool(BoolWithString value) => value.value;

        /// <summary>Create a new <see cref="BoolWithString"/> equal to <paramref name="value"/>, with a blank message</summary>
        /// <param name="value"></param>
        public static implicit operator BoolWithString(bool value) => new BoolWithString(value, "");

        /// <summary>Create a new <see cref="BoolWithString"/> equal to <c>false</c>, with the given <paramref name="message"/></summary>
        /// <param name="message"></param>
        public static BoolWithString False(string message="") { return new BoolWithString(false, message); }

        /// <summary>Create a new <see cref="BoolWithString"/> equal to <c>true</c>, with the given <paramref name="message"/></summary>
        /// <param name="message"></param>
        public static BoolWithString True(string message="") { return new BoolWithString(true, message); }

        public static bool operator ==(BoolWithString left, BoolWithString right){return left.Equals(right);}
        public static bool operator !=(BoolWithString left, BoolWithString right){return !(left == right);}
        public static bool operator ==(BoolWithString left, bool right) {return left.Equals(right);}
        public static bool operator !=(BoolWithString left, bool right) {return !(left == right);}
        public static bool operator ==(bool left, BoolWithString right) {return left==right.AsBool;}
        public static bool operator !=(bool left, BoolWithString right) {return left != right.AsBool;}

        public bool Equals(bool obj) { return value.Equals(obj); }
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (obj is bool actualbool) return actualbool == value;
            if (obj.GetType() != GetType()) return false;
            return Equals((BoolWithString) obj);
        }


        public BoolWithString Because(BoolWithString cause)
        {
            string becauseMessage = string.IsNullOrEmpty(message)
                                        ? "Because " + cause.message
                                        : message + " because " + cause.message;

            return new BoolWithString(value, becauseMessage);
        }
    }
}
