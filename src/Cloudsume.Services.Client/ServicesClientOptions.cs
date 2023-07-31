namespace Cloudsume.Services.Client;

using System;

public sealed class ServicesClientOptions
{
    public Uri ServerUri { get; set; } = new("http://localhost:5002");
}
