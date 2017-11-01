using System;
using System.Web;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.AspNetCore.Sockets.Client;

namespace ClimateMeter.TestClient.JWT
{
    public class HubAuthenticationBuilder: IHubAuthenticationBuilder
    {
        private readonly Func<string> _accessTokenProvider;
        private readonly IHubConnectionBuilder _connectionBuilder;
        private string _tokenQueryStringParameterName = "JWT_TOKEN";

        public HubAuthenticationBuilder(Func<string> accessTokenProvider, IHubConnectionBuilder connectionBuilder)
        {
            _accessTokenProvider = accessTokenProvider;
            _connectionBuilder = connectionBuilder;
        }

        public IHubAuthenticationBuilder WithTokenQueryStringParameterName(string queryStringParameterName)
        {
            if (String.IsNullOrEmpty(queryStringParameterName))
            {
                throw new ArgumentNullException(nameof(queryStringParameterName));
            }

            _tokenQueryStringParameterName = queryStringParameterName;

            return this;
        }

        public IHubConnectionBuilder WithUrl(string url)
        {
            return WithUrl(new Uri(url));
        }

        public IHubConnectionBuilder WithUrl(Uri url)
        {
            return _connectionBuilder
                .WithMessageHandler(new AuthorizationHttpMessageHandler(_accessTokenProvider))
                .WithUrl(BuildConnectionUrl(url));
        }

        private Uri BuildConnectionUrl(Uri url)
        {
            var uriBuilder = new UriBuilder(url);
            
            var queryString = HttpUtility.ParseQueryString(uriBuilder.Query);
            if (queryString[_tokenQueryStringParameterName] != null)
            {
                throw new ArgumentException($"Url should not contain Query String parameter named {_tokenQueryStringParameterName}", nameof(url));
            }
            
            var token = _accessTokenProvider();
            queryString[_tokenQueryStringParameterName] = token;

            uriBuilder.Query = queryString.ToString();

            return uriBuilder.Uri;
        }
    }
}
