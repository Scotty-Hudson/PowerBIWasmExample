using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;

namespace PowerBIWasmExample.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AzureADTokenController : ControllerBase
    {

        private readonly IConfiguration _configuration;

        public AzureADTokenController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet]
        public async Task<ActionResult<string>> GetADToken()
        {
            var tenantId = _configuration["AzureAppInfo:TenantId"];
            var clientId = _configuration["AzureAppInfo:ClientId"];
            var clientSecret = _configuration["AzureAppInfo:ClientSecret"];
            var authorityUri = _configuration["AzureAppInfo:AuthorityUri"].Replace("TenantId", tenantId);
            var powerbiApiDefaultScope = _configuration["AzureAppInfo:Scope"];
            var scopes = new string[] { powerbiApiDefaultScope };

            var app = ConfidentialClientApplicationBuilder
                .Create(clientId)
                .WithClientSecret(clientSecret)
                .WithAuthority(authorityUri)
                .Build();

            try
            {
                var authResult = await app.AcquireTokenForClient(scopes).ExecuteAsync();
                return Ok(authResult.AccessToken);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
