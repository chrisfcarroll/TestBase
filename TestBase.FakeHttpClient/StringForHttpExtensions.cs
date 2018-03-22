namespace TestBase.HttpClient.Fake
{
    public static class StringForHttpExtensions
    {
        public static string ToUrlEncoded(this string text){return System.Net.WebUtility.UrlEncode(text);}
        public static string ToUrlDecoded(this string fragment){return System.Net.WebUtility.UrlDecode(fragment);}

        public static string ToUrlDeDecoded(this string fragment, int loopLimit=10)
        {
            int i = 0;
            string decoded;
            while ( (decoded= System.Net.WebUtility.UrlDecode(fragment))!=fragment
                    && i++ < loopLimit )
            {
                fragment = decoded;
            }
            return decoded;
        }
    }
}
