using System;
using System.Linq.Expressions;
using ExpressionToCodeLib;

namespace TestBase
{
    /// <summary>Extension methods for <see cref="Expression"/></summary>
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

        /// <summary>
        /// A better ToString() method for Expressions
        /// </summary>
        /// <typeparam name="TExpr"></typeparam>
        /// <param name="expression"></param>
        /// <returns></returns>
        public static String ToCodeString<TExpr>(this TExpr expression) where TExpr : Expression
        {
            return ExpressionToCode.ToCode(expression);
        }
        /// <summary>
        /// A better ToString() method for Expressions, which returns a multi-lined, annotated output
        /// </summary>
        public static String ToAnnotatedCodeString<TExpr>(this TExpr expression) where TExpr : Expression
        {
            return ExpressionToCode.AnnotatedToCode(expression);
        }
        /// <summary>
        /// A better ToString() method for Expressions
        /// </summary>
        public static String ToValuedCodeString<TResult>(this Expression<Func<TResult>> expression)
        {
            return expression.ToValuedCode();
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
