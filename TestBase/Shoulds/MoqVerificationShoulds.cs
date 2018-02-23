//using System;
//using System.Linq.Expressions;
//using Moq;

//namespace TestBase.Shoulds
//{
//    public static class MoqVerificationShoulds
//    {
//        public static Action ShouldCall<TMock>(this Action methodUnderTest, Mock<TMock> mock, Expression<Action<TMock>> expectedToCall) where TMock : class
//        {
//            ShouldCall(methodUnderTest, mock, expectedToCall, Times.Once());
//            return methodUnderTest;
//        }

//        public static Action ShouldCall<TMock>(this Action methodUnderTest, Mock<TMock> mock, Expression<Action<TMock>> expectedToCall, Times times) where TMock : class
//        {
//            mock.Setup(expectedToCall);
//            methodUnderTest();
//            mock.Verify(expectedToCall, times);
//            return methodUnderTest;
//        }
//    }
//}