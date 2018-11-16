using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;

namespace TestBase
{
    /// <summary>Extension methods for setting up Request Cookies for testing a controller. The entire cookie collection must be set in one go.</summary>
    public static class RequestCookieExtension
    {
        /// <summary>Replace <see cref="HttpRequest.Cookies"/> with a new cookie collection created from <paramref name="cookies"/>
        /// The entire existing <see cref="HttpRequest.Cookies"/> will be overwritten.
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cookies">The new cookie values</param>
        /// <returns>The new <see cref="IRequestCookieCollection"/></returns>
        public static IRequestCookieCollection SetRequestCookies(this HttpRequest request, Dictionary<string, string> cookies)
        {
            request.Cookies= new RequestCookieCollection(cookies);
            return request.Cookies;
        }

        /// <summary>Replace <see cref="HttpRequest.Cookies"/> with a new cookie collection created from <paramref name="name1Value1Name2Value2Etc"/>
        /// The entire existing <see cref="HttpRequest.Cookies"/> will be overwritten.
        /// </summary>
        /// <param name="request"></param>
        /// <param name="name1Value1Name2Value2Etc">The new cookie values in the form <c>name1, value1, name2, value2, ...</c></param>
        /// <returns>The new <see cref="IRequestCookieCollection"/></returns>
        public static IRequestCookieCollection SetRequestCookies(this HttpRequest request, params string[] name1Value1Name2Value2Etc)
        {
            if (name1Value1Name2Value2Etc.Length % 2 == 1)
                throw new ArgumentException("This overload accepts cookies in the form name1, value1, name2, value2, ... ",
                    nameof(name1Value1Name2Value2Etc));

            var cookies= new Dictionary<string, string>();
            for (int i = 0; i < name1Value1Name2Value2Etc.Length; i+=2)
            {
                cookies.Add( name1Value1Name2Value2Etc[i], name1Value1Name2Value2Etc[i+1]);
            }
            request.Cookies= new RequestCookieCollection(cookies);
            return request.Cookies;
        }
    }
}