using System.Linq;
using System.Reflection;
using TestBase.Shoulds;

namespace TestBase
{
    public static class AttributeExtensions
    {
        public static T PropertyAttributeOn<T>(this ICustomAttributeProvider propertyInfo)
        {
            return propertyInfo.GetCustomAttributes(typeof(T), false)
                               .Cast<T>()
                               .FirstOrDefault();
        }

    }
}