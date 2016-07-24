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
    public class BoolWithString
    {
        protected bool Equals(BoolWithString other)
        {
            return value.Equals(other.value) && string.Equals(message, other.message);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (value.GetHashCode()*397) ^ (message != null ? message.GetHashCode() : 0);
            }
        }

        readonly bool value;
        readonly string message;

        BoolWithString(bool value, string message)
        {
            this.value = value;
            this.message = message;
        }
        public override string ToString()
        {
            return string.IsNullOrEmpty(message) 
                ? value.ToString() 
                : value + " : " + message;
        }

        public bool AsBool {get { return value; } }

        public static implicit operator bool(BoolWithString value) { return value.value; }
        public static implicit operator BoolWithString(bool value) { return new BoolWithString(value, ""); }

        public static BoolWithString False(string message="") { return new BoolWithString(false, message); }
        public static BoolWithString True(string message="") { return new BoolWithString(true, message); }

        public static bool operator ==(BoolWithString left, BoolWithString right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(BoolWithString left, BoolWithString right)
        {
            return !(left == right);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
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
