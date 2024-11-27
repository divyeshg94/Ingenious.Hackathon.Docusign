using DocuSign.eSign.Model;
using Ingenious.Hackathon.Docusign.Services;
using Ingenious.Hackathon.Docusign.Sql.DataModels;
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

        public async Task<IActionResult> Create()
        {
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

        [HttpPost]
        public async Task<IActionResult> CreateAgreement(AgreementCreationDto model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Read uploaded document
            using var stream = model.Document.OpenReadStream();
            using var memoryStream = new MemoryStream();
            await stream.CopyToAsync(memoryStream);
            var documentBase64 = Convert.ToBase64String(memoryStream.ToArray());

            // Create the document
            var document = new Document
            {
                DocumentBase64 = documentBase64,
                Name = model.AgreementName,
                FileExtension = Path.GetExtension(model.Document.FileName)?.Substring(1),
                DocumentId = "1"
            };

            // Create recipients from signers
            var signers = model.Signers.Select((signer, index) => new Signer
            {
                Email = signer.Email,
                Name = signer.Name,
                RecipientId = (index + 1).ToString(),
                RoutingOrder = (index + 1).ToString(),
                Tabs = new Tabs
                {
                    SignHereTabs = new List<SignHere>
            {
                new SignHere
                {
                    XPosition = model.XPosition.ToString(),
                    YPosition = model.YPosition.ToString(),
                    DocumentId = "1",
                    PageNumber = model.PageNumber.ToString()
                }
            }
                }
            }).ToList();

            // Create envelope
            var envelopeDefinition = new EnvelopeDefinition
            {
                EmailSubject = model.EmailSubject,
                EmailBlurb = model.EmailMessage,
                Documents = new List<Document> { document },
                Recipients = new Recipients { Signers = signers },
                Status = "sent" // Set to "sent" to send immediately
            };

            // Send envelope via DocuSign API
            var result = await _docuSignServices.SendEnvelope(envelopeDefinition);

            return View("Success", result);
        }
    }
}
