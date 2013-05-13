using System;
using System.Linq.Expressions;
using Moq;
using TestBase.Shoulds;

namespace TestBase
{
    public static class MockVerifyPassthroughExtensions
    {
        public static void VerifyMethodUnderTestIsPassthrough<TMock,TResult>(this Mock<TMock> targetMock, Func<TResult> methodUnderTest, Expression<Func<TMock, TResult>> expectedToPassthroughTo) where TMock :class 
        {
            TResult mockReturn = default(TResult);
            targetMock.Setup(expectedToPassthroughTo).Returns(mockReturn);

            TResult actual= methodUnderTest();

            targetMock.Verify(expectedToPassthroughTo, Times.Once());

            if (typeof(TResult).IsValueType)
            {
                actual.ShouldEqual(mockReturn);
            }
            else
            {
                actual.ShouldEqual(mockReturn);
            }
        }

        public static void VerifyMethodUnderTestIsPassthrough<TMock>(this Mock<TMock> targetMock, Action methodUnderTest, Expression<Action<TMock>> expectedToPassthroughTo) where TMock : class
        {
            targetMock.Setup(expectedToPassthroughTo);
            methodUnderTest();
            targetMock.Verify(expectedToPassthroughTo, Times.Once());
        }
    }
}