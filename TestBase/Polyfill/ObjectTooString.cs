#if !NET5_0_OR_GREATER
namespace TestBase
{
    static class ObjectTooString
    {
        public static string TooString<T>(this T value)
        {
            return value?.ToString() ?? "null";
        }
    }
}
#endif