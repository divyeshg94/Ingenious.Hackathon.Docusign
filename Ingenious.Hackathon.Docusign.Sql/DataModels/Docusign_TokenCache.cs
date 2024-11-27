namespace Ingenious.Hackathon.Docusign.Sql.DataModels
{
    public class DocusignTokenCache
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public DateTime Expiration { get; set; }

        public bool IsTokenExpired => DateTime.UtcNow >= Expiration;
    }
}
