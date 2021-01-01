using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace HandiCrafts.Web.Infrastructure.Security
{
    public class ErrorHandleMiddleware
    {
        private readonly RequestDelegate _next;
        public ErrorHandleMiddleware(RequestDelegate next)
        {
            _next = next ?? throw new ArgumentNullException(nameof(next));
        }

        public async Task Invoke(HttpContext context)
        {
            // Check Error in request content
            var originalBody = context.Request.Body;
            try
            {
                await _next(context).ConfigureAwait(false);

                var content = await ReadRequestBody(context);

                if (context.Response.StatusCode == 401)
                {
                    context.Response.Clear();
                    context.Response.Redirect("/Account/Login");
                }
            }
            finally
            {
                context.Request.Body = originalBody;
            }
        }

        private static async Task<string> ReadRequestBody(HttpContext context)
        {
            var buffer = new MemoryStream();
            await context.Request.Body.CopyToAsync(buffer);
            context.Request.Body = buffer;
            buffer.Position = 0;

            var encoding = Encoding.UTF8;

            var requestContent = await new StreamReader(buffer, encoding).ReadToEndAsync();
            //requestContent.
            context.Request.Body.Position = 0;

            return requestContent;
        }

    }

    public static class ErrorHandleMiddlewareExtension
    {
        public static IApplicationBuilder UseErrorHandleMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ErrorHandleMiddleware>();
        }
    }

    //********************************************************************************
    // Ajax Error Handle 
    //[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = true)]
    //public sealed class SiteAuthorizeAttribute : AuthorizeAttribute
    //{
    //    protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
    //    {
    //        if (filterContext.HttpContext.Request.IsAuthenticated)
    //        {
    //            throw new UnauthorizedAccessException(); //to avoid multiple redirects
    //        }
    //        else
    //        {
    //            handleAjaxRequest(filterContext);
    //            base.HandleUnauthorizedRequest(filterContext);
    //        }
    //    }

    //    private static void handleAjaxRequest(AuthorizationContext filterContext)
    //    {
    //        var ctx = filterContext.HttpContext;
    //        if (!ctx.Request.IsAjaxRequest())
    //            return;

    //        ctx.Response.StatusCode = (int)HttpStatusCode.Forbidden;
    //        ctx.Response.End();
    //    }
    //}
}
