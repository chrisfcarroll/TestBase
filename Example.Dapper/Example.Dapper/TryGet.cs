using System;

namespace Example.Dapper
{
    public static class TryGet
    {
        public static T OrDefault<T>(Func<T> function)
        {
            try{ return function(); } catch(Exception){ return default(T); }
        }
    }
}