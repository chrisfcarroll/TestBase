using System;
using System.Linq.Expressions;

namespace TestBase
{
    public static class ExpressionHelper 
    {
        public static Expression<Func<TFrom, TTo>> Chain<TFrom, TMiddle, TTo>(this Expression<Func<TFrom, TMiddle>> inner, Expression<Func<TMiddle, TTo>> outer ) 
        {
            return Expression.Lambda<Func<TFrom, TTo>>(
                new SwapVisitor(outer.Parameters[0], inner.Body).Visit(outer.Body),
                inner.Parameters
            );
        }
        public static Expression<Func<TFrom1, TFrom2, TTo>> Chain<TFrom1, TFrom2, TMiddle, TTo>(this Expression<Func<TFrom1, TFrom2, TMiddle>> inner, Expression<Func<TMiddle, TTo>> outer ) 
        {
            return Expression.Lambda<Func<TFrom1, TFrom2, TTo>>(
                new SwapVisitor(outer.Parameters[0], inner.Body).Visit(outer.Body),
                inner.Parameters
            );
        }
        public static Expression<Func<TFrom1, TFrom2, TFrom3, TTo>> Chain<TFrom1, TFrom2, TFrom3, TMiddle, TTo>(
                        this Expression<Func<TFrom1, TFrom2, TFrom3, TMiddle>> inner, 
                             Expression<Func<TMiddle, TTo>> outer ) 
        {
            return Expression.Lambda<Func<TFrom1, TFrom2, TFrom3, TTo>>(
                new SwapVisitor(outer.Parameters[0], inner.Body).Visit(outer.Body),
                inner.Parameters
            );
        }
    }

    class SwapVisitor : ExpressionVisitor {
        readonly Expression @from;
        readonly Expression to;

        public SwapVisitor(Expression from, Expression to) {
            this.@from = from;
            this.to = to;
        }

        public override Expression Visit(Expression node) { return node == @from ? to : base.Visit(node);}
    }
}
