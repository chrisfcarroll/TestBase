using Newtonsoft.Json;

namespace TestBase.HttpClient.Fake
{
    static class JsonCompare
    {
        public static bool EqualsByJson<T>(this T left, object right)
        {
            return
            JsonConvert.SerializeObject(left,  BestEffortJsonSerializerSettings.Settings)
         == JsonConvert.SerializeObject(right, BestEffortJsonSerializerSettings.Settings);
        }
    }

    public static class BestEffortJsonSerializerSettings
    {
        /// <summary>
        ///     <c>
        ///         new JsonSerializerSettings
        ///         {
        ///         NullValueHandling = NullValueHandling.Ignore,
        ///         MissingMemberHandling = MissingMemberHandling.Ignore,
        ///         ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
        ///         Error = (sender, args) =&gt; { }
        ///         }
        ///     </c>
        /// </summary>
        public static readonly JsonSerializerSettings Settings =
        new JsonSerializerSettings
        {
        NullValueHandling     = NullValueHandling.Ignore,
        MissingMemberHandling = MissingMemberHandling.Ignore,
        ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
        Error                 = (sender, args) => { }
        };
    }
}
