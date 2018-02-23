using System;
using System.Linq;
using System.Reflection;

namespace TestBase
{
    //public static class WindsorExtensions
    //{
    //    public static IHandler[] GetAllHandlers(this IWindsorContainer container)
    //    {
    //        return GetHandlersFor(container, typeof(object));
    //    }

    //    public static IHandler[] GetHandlersFor(this IWindsorContainer container, Type type)
    //    {
    //        return container.Kernel.GetAssignableHandlers(type);
    //    }

    //    public static Type[] GetImplementationTypesFor(this IWindsorContainer container, Type type)
    //    {
    //        return
    //            container.GetHandlersFor(type)
    //                     .Select(h => h.ComponentModel.Implementation)
    //                     .OrderBy(t => t.Name)
    //                     .ToArray();
    //    }

    //    public static Type[] GetPublicClassesFromApplicationAssembly(this Assembly assembly, Predicate<Type> @where)
    //    {
    //        return
    //            assembly.GetExportedTypes()
    //                    .Where(t => t.IsClass)
    //                    .Where(t => t.IsAbstract == false)
    //                    .Where(@where.Invoke)
    //                    .OrderBy(t => t.Name)
    //                    .ToArray();
    //    }
    //}
}