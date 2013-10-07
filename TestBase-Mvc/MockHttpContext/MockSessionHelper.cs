using System.Collections.Generic;
using System.Web;
using Moq;

namespace TestBase.MockHttpContext
{
    public class MockSessionHelper
    {
        private readonly Mock<HttpSessionStateBase> mockSession;
        private readonly IDictionary<string, object> sessionDictionary = new Dictionary<string, object>();

        public MockSessionHelper(Mock<HttpContextBase> mockContext)
        {
            mockSession = new Mock<HttpSessionStateBase>();
            mockContext.Setup(x => x.Session).Returns(mockSession.Object);
        }

        public void SetSessionFor(string key, object value)
        {
            var dictionary = sessionDictionary;
            mockSession.SetupSet(x => x[key] = It.IsAny<object>())
                .Callback((string name, object val) =>
                              {
                                  dictionary[name] = val;
                                  mockSession.SetupGet(x => x[name]).Returns(dictionary[name]);
                              }
                );
            mockSession.Object[key] = value;
        }

        public Mock<HttpSessionStateBase> MockSession { get { return mockSession; } }
    }
}