using System.Collections;
using Newtonsoft.Json;

namespace TestBase
{
    public class ConsoleJson
    {
        public static void WriteLine(params object[] objects) { WriteLine((IEnumerable) objects); }

        static void WriteLine(IEnumerable objects)
        {
            foreach (var item in objects)
            {
                System.Console.WriteLine(JsonConvert.SerializeObject(item));
            }
        }
    }
}