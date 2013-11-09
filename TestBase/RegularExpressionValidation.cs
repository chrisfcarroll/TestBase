using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;

namespace TestBase
{
    public static class RegularExpressionValidation
    {
        public static RegularExpressionAttribute For<TModel>(TModel model, string propertyName)
        {
            var attributes = typeof(TModel).GetProperty(propertyName)
                                           .GetCustomAttributes(typeof(RequiredAttribute), false);

            return attributes.Cast<RegularExpressionAttribute>().First();
        }
        public static RegularExpressionAttribute For<TModel, TMember>(
            TModel model,
            Expression<Func<TModel, TMember>> member)
        {
            var memberExpression = member.Body as MemberExpression;
            Debug.Assert(memberExpression != null, String.Format("{0} should be a member expression", member));

            return memberExpression
                .Member
                .GetCustomAttributes(typeof(RegularExpressionAttribute), false)
                .Cast<RegularExpressionAttribute>().First();
        }
    }
}