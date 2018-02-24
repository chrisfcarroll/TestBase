using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace TestBase
{
    class TestableHttpListener : IDisposable
    {
        public class RequestAndBody {
            public string RequestAsJson { get; set; }
            public string RequestBody { get; set; }
            public RequestAndBody(string requestAsJson, string requestBody) { RequestAsJson = requestAsJson; RequestBody = requestBody; }
        }

        TextWriter consoleLogger = Console.Out;
        readonly HttpListener listener;
        static readonly byte[] EmptyUtf8String = Encoding.UTF8.GetBytes("");
        JsonSerializerSettings jsonSettingsIgnoreStreamsAndBody;

        public TestableHttpListener(params string[] prefixes) : this((IEnumerable<string>)prefixes) { }
        public TestableHttpListener(IEnumerable<string> prefixes)
        {
            if (!HttpListener.IsSupported) { throw new NotImplementedException("Windows XP SP2 or Server 2003 is required to use the HttpListener class."); }
            if (prefixes == null || !prefixes.Any()) { throw new ArgumentException("Prefixes must not be null or empty", "prefixes"); }
            //
            jsonSettingsIgnoreStreamsAndBody= new JsonSerializerSettings
            {
                ContractResolver = new DeSerializeExcludingFieldsContractResolver(
                    typeof(HttpListenerRequest),
                    p => typeof(Stream).IsAssignableFrom(p.PropertyType) 
                         || typeof(EndPoint).IsAssignableFrom(p.PropertyType)
                         || p.PropertyName.Matches("Certificate")
                )
            };
            listener = new HttpListener();
            foreach (var prefix in prefixes){listener.Prefixes.Add(prefix);}
            listener.Start();
            consoleLogger.WriteLine("Listening...");
        }

        /// <summary></summary>
        /// <param name="responseString">defaults to "&lt;!DOCTYPE html&gt;&lt;html&gt;&lt;body&gt;Hello world&lt;/body&gt;&lt;/html&gt;"</param>
        /// <returns>
        /// <see cref="JsonConvert.SerializeObject(object)"/>(<see cref="HttpListenerContext.Request"/>, <see cref="Formatting.Indented"/>)
        /// </returns>
        public /*async Task<*/ RequestAndBody FixedResponseAsync(Func<HttpListenerRequest,bool> isRequestMatching, string responseString)
        {
            responseString = responseString ?? "<!DOCTYPE html><html><body>Hello world</body></html>";
            string requestJson = "",content=null;

            //var context = await listener.GetContextAsync();

            //try{requestJson = JsonConvert.SerializeObject(context.Request, Formatting.Indented, jsonSettingsIgnoreStreamsAndBody);}catch(Exception e){consoleLogger.WriteLine(e);}
            //try { content = await new StreamReader(context.Request.InputStream).ReadToEndAsync(); }catch (Exception e) { consoleLogger.WriteLine(e); }
            //consoleLogger.WriteLine("[Incoming]::Start------\n{0}\n[Incoming]::End--------", requestJson);

            //if (isRequestMatching(context.Request))
            //{
            //    context.Response.StatusCode = 200;
            //    context.Response.StatusDescription = "OK";
            //    var responseBytes = Encoding.UTF8.GetBytes(responseString);
            //    context.Response.ContentLength64 = responseBytes.Length;
            //    context.Response.OutputStream.Write(responseBytes, 0, responseBytes.Length);
            //    context.Response.OutputStream.Close();
            //}
            //else
            //{
            //    context.Response.StatusCode = 404;
            //    context.Response.ContentLength64 = EmptyUtf8String.Length;
            //    context.Response.OutputStream.Write(EmptyUtf8String, 0, EmptyUtf8String.Length);
            //    context.Response.OutputStream.Close();
            //}
            return new RequestAndBody(requestJson,content);
        }

        public void Dispose()
        {
            if (listener != null)
            {
                listener.Stop();
                ((IDisposable)listener).Dispose();
            }
            consoleLogger = null;
        }
    }
}