using System.Collections.Generic;
using Newtonsoft.Json;

namespace TestBase
{
    /// <summary>Create a <see cref="Dictionary{String,Object}"/> of an object's properties.
    /// </summary>
    public static class AnonymousObjectInspector
    {
        /// <summary>Inspect the public properties of <paramref name="obj"/> and return a dictionary using the Property name as
        /// a key, and the property value as a value.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="jsonReferenceLoopHandling"></param>
        /// <returns>a <see cref="Dictionary{String,Object}"/>of property name values</returns>
        public static Dictionary<string, object> ToPropertyDictionary(this object obj, ReferenceLoopHandling jsonReferenceLoopHandling=ReferenceLoopHandling.Ignore)
        {
            return JsonConvert.DeserializeObject<Dictionary<string, object>>(
                JsonConvert.SerializeObject(
                    obj,
                    new JsonSerializerSettings{ReferenceLoopHandling = jsonReferenceLoopHandling}));
        }
    }
}