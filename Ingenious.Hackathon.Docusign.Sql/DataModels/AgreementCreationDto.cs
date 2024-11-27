using Microsoft.AspNetCore.Http;

namespace Ingenious.Hackathon.Docusign.Sql.DataModels
{
    public class AgreementCreationDto
    {
        public string AgreementName { get; set; }
        public string EmailSubject { get; set; }
        public string EmailMessage { get; set; }
        public IFormFile Document { get; set; }
        public List<DocumentSigner> Signers { get; set; }
        public int PageNumber { get; set; }
        public int XPosition { get; set; }
        public int YPosition { get; set; }
    }

    public class DocumentSigner
    {
        public string Name { get; set; }
        public string Email { get; set; }
    }
}
