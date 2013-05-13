using System;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;

namespace TestBase.Shoulds
{
    public static class AttributeShoulds
    {
        public static Type ShouldValidateProperty(this Type @this, string property, Type attribute)
        {
            @this.GetProperty(property).GetCustomAttributes(attribute,true).Count().ShouldBeGreaterThan(0);
            return @this;
        }

        public static Expression<Func<TClass, TMember>> 
            ShouldBeValidationAttributeFor<TClass, TMember>(this Type attribute, Expression<Func<TClass, TMember>> member)
        {
            var memberExpression = member.Body as MemberExpression;
            Debug.Assert(memberExpression!=null, String.Format("{0} should be a member expression", member));

            memberExpression.Member.GetCustomAttributes(attribute, true).Count().ShouldBeGreaterThan(0);
            return member;
        }
    }
}