using DocuSign.eSign.Model;
using Ingenious.Hackathon.Docusign.Services;
using Microsoft.AspNetCore.Mvc;

namespace Ingenious.Hackathon.Docusign.Controllers
{
    public class AgreementController : Controller
    {
        private readonly DocuSignServices _docuSignServices;
        public AgreementController(DocuSignServices docuSignServices)
        {
            _docuSignServices = docuSignServices;
        }

        public async Task<IActionResult> Index()
        {
            await ESignDocument();
            return View();
        }

        public async Task<IActionResult> ESignDocument()
        {
            var envelope = new EnvelopeDefinition
            {
                EmailSubject = "Please sign this document",
                Documents = new List<Document>
            {
                new Document
                {
                    DocumentBase64 = Convert.ToBase64String(System.IO.File.ReadAllBytes("wwwroot/World_Wide_Corp_lorem.pdf")),
                    Name = "Sample Document",
                    FileExtension = "pdf",
                    DocumentId = "1"
                }
            },
                Recipients = new Recipients
                {
                    Signers = new List<Signer>
                {
                    new Signer
                    {
                        Email = "recipient@example.com",
                        Name = "John Doe",
                        RecipientId = "1",
                        Tabs = new Tabs
                        {
                            SignHereTabs = new List<SignHere>
                            {
                                new SignHere
                                {
                                    AnchorString = "/sig/",
                                    AnchorYOffset = "-10",
                                    AnchorUnits = "pixels"
                                }
                            }
                        }
                    }
                }
                },
                Status = "sent"
            };

            var result = await _docuSignServices.SendEnvelope(envelope);

            return View(result);
        }
    }
}
