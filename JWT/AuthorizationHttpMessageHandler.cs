using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace ClimateMeter.TestClient.JWT
{
    public class AuthorizationHttpMessageHandler: DelegatingHandler
    {
            private Func<string> _accessTokenProvider { get; set; }
        
            public AuthorizationHttpMessageHandler(Func<string> accessTokenProvider)
            {
                _accessTokenProvider = accessTokenProvider;
                InnerHandler = new HttpClientHandler();
            }

            protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                var token = _accessTokenProvider();
                request.Headers.Add("Authorization", $"Bearer {token}");
            
                return base.SendAsync(request, cancellationToken);
            }
        }
    }