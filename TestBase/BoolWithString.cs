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

        private readonly bool value;
        private readonly string message;
        private BoolWithString(bool value, string message)
        {
            this.value = value;
            this.message = message;
        }
        public override string ToString()
        {
            if (string.IsNullOrEmpty(message)) return value.ToString();
            return value + " : " + message;
        }
        public static implicit operator bool(BoolWithString value) { return value.value; }
        public static implicit operator BoolWithString(bool value) { return new BoolWithString(value, ""); }

        public static BoolWithString False(string message) { return new BoolWithString(false, message); }
        public static BoolWithString True(string message) { return new BoolWithString(true, message); }

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