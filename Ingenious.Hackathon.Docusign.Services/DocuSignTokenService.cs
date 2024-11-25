using System.Text;
using DocuSign.eSign.Client;
using Ingenious.Hackathon.Docusign.Sql.DataModels;
using Ingenious.Hackathon.Docusign.Sql.Models;
using Microsoft.Extensions.Options;

namespace Ingenious.Hackathon.Docusign.Services
{
    public class DocuSignTokenService
    {
        private readonly ApiClient _apiClient;
        private DocusignTokenCache _tokenCache;
        private readonly DocuSignJWT _docuSignJWTSettings;
        private readonly DocuSignSettings _docuSignSettings;

        public DocuSignTokenService(IOptions<DocuSignJWT> docuSignJWT, IOptions<DocuSignSettings> settings)
        {
            _docuSignJWTSettings = docuSignJWT.Value;
            _docuSignSettings = settings.Value;
            _apiClient = new ApiClient(_docuSignSettings.BasePath);
            _tokenCache = new DocusignTokenCache();
        }

        public async Task<string> GetAccessTokenAsync()
        {
            if (_tokenCache.AccessToken != null && !_tokenCache.IsTokenExpired)
            {
                // Return cached token if it's still valid
                return _tokenCache.AccessToken;
            }

            // If no valid access token or refresh token, fetch a new token
            return await FetchNewTokenAsync();
        }

        private async Task<string> FetchNewTokenAsync()
        {
            var rsaKey = System.IO.File.ReadAllText(_docuSignJWTSettings.PrivateKeyFile);
            var jwtResponse = _apiClient.RequestJWTUserToken(
                _docuSignJWTSettings.ClientId,
                _docuSignJWTSettings.ImpersonatedUserId,
                _docuSignJWTSettings.AuthServer,
                ConvertStringToStream(rsaKey),
                3600, // Token validity in seconds
                new List<string> { "signature", "impersonation" }
            );

            if (jwtResponse == null || jwtResponse.access_token == null)
            {
                throw new Exception("Failed to obtain access token.");
            }

            // Cache the token
            _tokenCache = new DocusignTokenCache
            {
                AccessToken = jwtResponse.access_token,
                RefreshToken = jwtResponse.refresh_token,
                Expiration = DateTime.UtcNow.AddSeconds(jwtResponse.expires_in.Value)
            };

            return _tokenCache.AccessToken;
        }

        public static Stream ConvertStringToStream(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                throw new ArgumentNullException(nameof(input), "Input string cannot be null or empty.");
            }

            byte[] byteArray = Encoding.UTF8.GetBytes(input);
            return new MemoryStream(byteArray);
        }
    }
}
