using System;
using Microsoft.AspNetCore.SignalR.Client;

namespace ClimateMeter.TestClient.JWT
{
    public interface IHubAuthenticationBuilder
    {
        IHubAuthenticationBuilder WithTokenQueryStringParameterName(string queryStringParameterName);
        IHubConnectionBuilder WithUrl(string url);
        IHubConnectionBuilder WithUrl(Uri url);
    }
}
