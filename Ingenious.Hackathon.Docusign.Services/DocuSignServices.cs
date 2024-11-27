using DocuSign.eSign.Api;
using DocuSign.eSign.Client;
using DocuSign.eSign.Model;
using Ingenious.Hackathon.Docusign.Sql.DataModels;
using Ingenious.Hackathon.Docusign.Sql.Models;
using Microsoft.Extensions.Options;

namespace Ingenious.Hackathon.Docusign.Services
{
    public class DocuSignServices
    {
        private readonly DocuSignSettings _docuSignSettings;
        private readonly DocuSignJWT _docuSignJWTSettings;
        private ApiClient _apiClient;
        private DocusignTokenCache _tokenCache;
        private DocuSignTokenService _docuSignTokenService;

        public DocuSignServices(IOptions<DocuSignSettings> settings, IOptions<DocuSignJWT> docuSignJWT, DocuSignTokenService docuSignTokenService)
        {
            _docuSignSettings = settings.Value;
            _docuSignJWTSettings = docuSignJWT.Value;
            _docuSignTokenService = docuSignTokenService;
            _apiClient = new ApiClient(_docuSignSettings.BasePath);
        }

        public async Task<EnvelopeSummary> SendEnvelope(EnvelopeDefinition envelope)
        {

            var token = await _docuSignTokenService.GetAccessTokenAsync();
            _apiClient.Configuration.DefaultHeader["Authorization"] = $"Bearer {token}";

            var envelopesApi = new EnvelopesApi(_apiClient);
            var result = await envelopesApi.CreateEnvelopeAsync(_docuSignSettings.AccountId, envelope);
            return result;
        }
    }
}
