using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace TestBase.Shoulds
{
    public static class AttributeShoulds
    {
        public static Type ShouldValidateProperty(this Type @this, string property, Type attribute)
        {
            @this.GetProperty(property).GetCustomAttributes(attribute,true).Count().ShouldBeGreaterThan(0);
            return @this;
        }

        public static Expression<Func<TClass, TMember>> ShouldBeValidationAttributeFor<TClass, TMember>(this Type attribute, Expression<Func<TClass, TMember>> member)
        {
            var memberExpression = member.Body as MemberExpression;
            Debug.Assert(memberExpression!=null, String.Format("{0} should be a member expression", member));

            memberExpression.Member.GetCustomAttributes(attribute, true).Count().ShouldBeGreaterThan(0);
            return member;
        }

        public static void ShouldNotBeRequired(this PropertyInfo @this)
        {
            @this.ShouldNotHaveAttribute<RequiredAttribute>();
        }

        public static void ShouldBeRequired(this PropertyInfo @this)
        {
            @this.ShouldHaveAttribute<RequiredAttribute>();
        }

        public static T PropertyAttributeOn<T>(ICustomAttributeProvider propertyInfo)
        {
            return propertyInfo.GetCustomAttributes(typeof(T), false)
                               .Cast<T>()
                               .FirstOrDefault();
        }

        public static T ShouldHaveAttribute<T>(this PropertyInfo @this)
        {
            var annotation = PropertyAttributeOn<T>(@this);
            annotation.ShouldNotBeNull();

            return annotation;
        }

        public static void ShouldHaveAttribute<T>(this PropertyInfo @this, params Action<T>[] assertions)
        {
            ShouldHaveAttribute<T>(@this);

            var attribute = PropertyAttributeOn<T>(@this);

            foreach(var assert in assertions)
            {
                assert(attribute);
            }
        }

        public static void ShouldNotHaveAttribute<T>(this PropertyInfo @this)
        {
            PropertyAttributeOn<T>(@this).ShouldBeNull();
        }
    }
}