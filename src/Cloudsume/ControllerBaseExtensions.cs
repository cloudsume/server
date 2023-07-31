namespace Cloudsume;

using System;
using System.Linq;
using System.Net;
using Microsoft.AspNetCore.Mvc;

internal static class ControllerBaseExtensions
{
    public static IPAddress GetRemoteIp(this ControllerBase controller)
    {
        return controller.HttpContext.Connection.RemoteIpAddress ?? throw new InvalidOperationException("No remote IP address.");
    }

    public static string GetUserAgent(this ControllerBase controller)
    {
        return controller.HttpContext.Request.Headers.UserAgent.FirstOrDefault() ?? string.Empty;
    }
}
