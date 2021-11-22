using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;
using Microsoft.PowerBI.Api;
using Microsoft.PowerBI.Api.Models;
using Microsoft.Rest;
using PowerBIWasmExample.Shared;

namespace PowerBIWasmExample.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PowerBIController : ControllerBase
    {

        private readonly IConfiguration _configuration;

        public PowerBIController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet]
        public async Task<ActionResult<string>> GetReportEmbedding()
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

                var tokenCredentials = new TokenCredentials(authResult.AccessToken, "Bearer");
                var urlPowerBiServiceAppRoot = "https://api.powerbi.com/";
                var pbiClient = new PowerBIClient(new Uri(urlPowerBiServiceAppRoot), tokenCredentials);

                var workspaceId = new Guid(_configuration["PowerBI:WorkspaceId"]);
                var reportId = new Guid(_configuration["PowerBI:ReportId"]);
                var report = pbiClient.Reports.GetReportInGroup(workspaceId, reportId);

                var tokenRequest = new GenerateTokenRequest(TokenAccessLevel.View, report.DatasetId);
                var embedTokenResponse = await pbiClient.Reports.GenerateTokenAsync(workspaceId, reportId, tokenRequest);

                var reportViewModel = new EmbeddedReportViewModel(
                    report.Id.ToString(),
                    report.Name,
                    report.EmbedUrl,
                    embedTokenResponse.Token);

                return Ok(reportViewModel);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
