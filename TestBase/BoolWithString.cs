namespace TestBase
{
    public class BoolWithString
    {
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

        public BoolWithString Because(BoolWithString cause)
        {
            string becauseMessage = string.IsNullOrEmpty(message)
                                        ? "Because " + cause.message
                                        : message + " because " + cause.message;

            return new BoolWithString(value, becauseMessage);
        }
    }
}