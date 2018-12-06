using System;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace TestBase
{
    class DeSerializeExcludingFieldsContractResolver : DefaultContractResolver
    {
        readonly Func<JsonProperty, bool> ignoreProperty;
        readonly Type type;

        public DeSerializeExcludingFieldsContractResolver(Type type, Func<JsonProperty, bool> ignoreProperty)
        {
            this.type           = type;
            this.ignoreProperty = ignoreProperty;
        }

        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            var property = base.CreateProperty(member, memberSerialization);
            property.Ignored = property.DeclaringType == type && ignoreProperty(property);
            return property;
        }
    }
}
