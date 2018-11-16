using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace TestBase
{
    public static class AttributeShoulds
    {
        /// <summary>Assert that <paramref name="@this"/> has a
        /// Property named<paramref name="property"/> which is Attributed by <paramref name="attribute"/> 
        /// </summary>
        /// <param name="this"></param>
        /// <param name="property"></param>
        /// <param name="attribute"></param>
        /// <returns><paramref name="@this"/></returns>
        public static Type ShouldValidateProperty(this Type @this, string property, Type attribute)
        {
            @this.GetProperty(property)
                .ShouldNotBeNull($"Type {@this} doesn't have a property called {property}")
                .GetCustomAttributes(attribute,true)
                .Count().ShouldBeGreaterThan(0, $"Type {@this}.{property} has no Attribute of type {attribute}");
            return @this;
        }

        public static Expression<Func<TClass, TMember>> ShouldBeValidationAttributeFor<TClass, TMember>(this Type attribute, Expression<Func<TClass, TMember>> member)
        {
            var memberExpression = member.Body as MemberExpression;
            Debug.Assert(memberExpression!=null, String.Format("{0} should be a member expression", member));

            memberExpression.Member.GetCustomAttributes(attribute, true).Count().ShouldBeGreaterThan<int>(0);
            return member;
        }

        public static PropertyInfo ShouldNotBeRequired(this PropertyInfo @this)
        {
            @this.ShouldNotHaveAttribute<RequiredAttribute>();
            return @this;
        }

        public static PropertyInfo ShouldBeRequired(this PropertyInfo @this)
        {
            @this.ShouldHaveAttribute<RequiredAttribute>();
            return @this;
        }

        public static T ShouldHaveAttribute<T>(this PropertyInfo @this) where T : Attribute
        {
            @this.GetCustomAttribute<T>().ShouldNotBeNull();
            return @this.GetCustomAttribute<T>();
        }

        public static Type ShouldHaveAttribute<T>(this Type @this)
        {
            @this.GetTypeInfo().GetCustomAttributes(typeof(T), true)
                .FirstOrDefault(a => a.GetType() == typeof(T))
                .ShouldNotBeNull();
            return @this;
        }

        public static Type ShouldNotHaveAttribute<T>(this Type @this)
        {
            @this.GetTypeInfo().GetCustomAttributes(typeof (T), true).Count(a => a.GetType() == typeof (T))
                  .ShouldEqual(0,"Expected to not find attribute {0} on type {1}", typeof(T), @this);
            return @this;
        }

        public static PropertyInfo ShouldHaveAttributeSatisfying<T>(this PropertyInfo @this, params Action<T>[] assertions) where T : Attribute
        {
            ShouldHaveAttribute<T>(@this);

            var attribute = @this.GetCustomAttribute<T>();

            foreach(var assert in assertions)
            {
                assert(attribute);
            }
            return @this;
        }

        public static PropertyInfo ShouldNotHaveAttribute<T>(this PropertyInfo @this) where T : Attribute
        {
            @this.GetCustomAttribute<T>().ShouldBeNull();
            return @this;
        }
    }
}