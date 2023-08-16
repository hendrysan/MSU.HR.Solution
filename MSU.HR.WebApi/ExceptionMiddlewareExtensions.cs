using Microsoft.AspNetCore.Diagnostics;
using MSU.HR.Models.Others;
using System.Net;

namespace GlobalErrorHandling.Extensions
{
    public static class ExceptionMiddlewareExtensions
    {
        public static void ConfigureExceptionHandler(this IApplicationBuilder app)
        {
            app.UseExceptionHandler(appError =>
            {
                appError.Run(async context =>
                {
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    context.Response.ContentType = "application/json";

                    var contextFeature = context.Features.Get<IExceptionHandlerFeature>();
                    if (contextFeature != null)
                    {
                        //logger.LogError($"Something went wrong: {contextFeature.Error}");

                        if (contextFeature.Error.Message.ToLower().Contains("badrequest"))
                        {
                            context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                            await context.Response.WriteAsJsonAsync(new ErrorModel()
                            {
                                IsSuccess = false,
                                ErrorCode = context.Response.StatusCode,
                                Message = contextFeature.Error.Message.Replace("badrequest", "").Trim()
                            });
                        }
                        else
                        {
                            await context.Response.WriteAsJsonAsync(new ErrorModel()
                            {
                                IsSuccess = false,
                                ErrorCode = context.Response.StatusCode,
                                Message = "Internal Server Error. " + contextFeature.Error.Message
                            });
                        }
                    }
                });
            });
        }
    }
}