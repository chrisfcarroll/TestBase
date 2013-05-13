using System.Text.RegularExpressions;

namespace TestBase
{
    public static class PatternHelper
    {
        public const string GuidPattern = "([0-9a-fA-F]){8}-([0-9a-fA-F]){4}-([0-9a-fA-F]){4}-([0-9a-fA-F]){4}-([0-9a-fA-F]){12}";
        public static readonly Regex GuidRegex = new Regex(GuidPattern);
    }
}
