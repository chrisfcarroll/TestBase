using System.Threading.Tasks;

namespace TestBase.HttpClient.Fake
{
    public static class AwaitableResultNoContext
    {
        public static T ConfigureFalseGetResult<T>(this Task<T> task)
        {
            return task.ConfigureAwait(false).GetAwaiter().GetResult();
        }

        public static void ConfigureFalseGetResult(this Task task)
        {
            task.ConfigureAwait(false).GetAwaiter().GetResult();
        }
    }
}
