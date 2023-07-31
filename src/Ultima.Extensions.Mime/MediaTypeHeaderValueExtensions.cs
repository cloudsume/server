namespace Ultima.Extensions.Mime;

using System;
using System.Net.Http.Headers;

public static class MediaTypeHeaderValueExtensions
{
    public static bool IsType(this MediaTypeHeaderValue? type, string expect) => string.Equals(type?.MediaType, expect, StringComparison.OrdinalIgnoreCase);
}
