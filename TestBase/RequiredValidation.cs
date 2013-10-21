using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;

namespace TestBase
{
    public static class Validation<TValidation>
    {
        public static TValidation For<TModel>(TModel model, string propertyName)
        {
            var attributes = typeof(TModel).GetProperty(propertyName)
                                           .GetCustomAttributes(typeof(TValidation), false);

            return attributes.Cast<TValidation>().First();
        }

        public static TValidation For<TModel, TMember>(
            TModel model,
            Expression<Func<TModel, TMember>> member)
        {
            var memberExpression = member.Body as MemberExpression;
            Debug.Assert(memberExpression != null, String.Format("{0} should be a member expression", member));

            return memberExpression
                .Member
                .GetCustomAttributes(typeof(TValidation), false)
                .Cast<TValidation>().First();
        }
    }


    public static class RequiredValidation
    {
        public static RequiredAttribute For<TModel>(TModel model, string propertyName)
        {
            var attributes = typeof(TModel).GetProperty(propertyName)
                                           .GetCustomAttributes(typeof(RequiredAttribute), false);

            return attributes.Cast<RequiredAttribute>().First();
        }

        public static RequiredAttribute For<TModel, TMember>(
            TModel model,
            Expression<Func<TModel, TMember>> member)
        {
            var memberExpression = member.Body as MemberExpression;
            Debug.Assert(memberExpression != null, String.Format("{0} should be a member expression", member));

            return memberExpression
                .Member
                .GetCustomAttributes(typeof(RequiredAttribute), false)
                .Cast<RequiredAttribute>().First();
        }
    }
}