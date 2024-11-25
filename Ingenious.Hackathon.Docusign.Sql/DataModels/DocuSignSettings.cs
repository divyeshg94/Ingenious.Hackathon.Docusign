namespace Ingenious.Hackathon.Docusign.Sql.Models
{
    public class DocuSignSettings
    {
        public string BasePath { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string RedirectUrl { get; set; }
        public string AccountId { get; set; }
    }

    public class DocuSignJWT
    {
        public string ClientId { get; set; }
        public string ImpersonatedUserId { get; set; }
        public string AuthServer { get; set; }
        public string PrivateKeyFile { get; set; }
    }
}
