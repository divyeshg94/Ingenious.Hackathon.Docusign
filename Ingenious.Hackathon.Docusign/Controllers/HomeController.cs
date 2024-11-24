using Microsoft.AspNetCore.Mvc;

namespace Ingenious.Hackathon.Docusign.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        } 
        
        public IActionResult LoginSuccess()
        {
            return View();
        }
    }
}
