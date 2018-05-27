using System.Net;
using System.Net.Http;

namespace TestBase
{
    public static class HttpResponseMessageShoulds
    {
        public static HttpResponseMessage ShouldBe_200Ok(this HttpResponseMessage response) => ShouldBe(response, HttpStatusCode.OK);
        public static HttpResponseMessage ShouldBe_202Accepted(this HttpResponseMessage response) => ShouldBe(response, HttpStatusCode.Accepted);
        public static HttpResponseMessage ShouldBe_302Found(this HttpResponseMessage response) => ShouldBe(response, HttpStatusCode.Found);

        public static HttpResponseMessage ShouldBe(this HttpResponseMessage response, int expectedHttpStatusCode) =>
            ShouldBe(response, (HttpStatusCode) expectedHttpStatusCode);

        public static HttpResponseMessage ShouldBe(this HttpResponseMessage response, HttpStatusCode expectedHttpStatusCode)
        {
            Assert.That(
                response,
                r => r.StatusCode == expectedHttpStatusCode,
                $"(Did you include a handler mapping for this event)? \nGot:\n{response}\nContent:\n{response.Content.ReadAsStringAsync().Result}");
            return response;
        }
    }
}