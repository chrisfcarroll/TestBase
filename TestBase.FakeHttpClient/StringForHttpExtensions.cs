using System.Net;

namespace TestBase.HttpClient.Fake
{
    public static class StringForHttpExtensions
    {
        public static string ToUrlEncoded(this string text)     { return WebUtility.UrlEncode(text); }
        public static string ToUrlDecoded(this string fragment) { return WebUtility.UrlDecode(fragment); }

        public static string ToUrlDeDecoded(this string fragment, int loopLimit = 10)
        {
            var    i = 0;
            string decoded;
            while ((decoded = WebUtility.UrlDecode(fragment)) != fragment
                && i++                                        < loopLimit)
                fragment = decoded;
            return decoded;
        }
    }
}
