using Microsoft.AspNetCore.Authentication;
using System.Net.Http.Headers;

namespace Mango.Services.ShoppingCartAPI.Utility
{
    public class BackendApiAuthenticationHttpClientHandler: DelegatingHandler
    {
        private readonly IHttpContextAccessor _context;

        public BackendApiAuthenticationHttpClientHandler(IHttpContextAccessor context)
        {
            _context = context;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var token = await _context.HttpContext.GetTokenAsync("access_token");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer",token);
            return await base.SendAsync(request, cancellationToken);

        }
    }
}
