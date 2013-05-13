namespace TestBase
{
    static public class StringExtensions
    {
        public static string WithWhiteSpaceRemoved(this string @this)
        {
            return @this.Replace(" ", "").Replace("\n", "").Replace("\r", "").Replace("\t", "");
        }
    }
}